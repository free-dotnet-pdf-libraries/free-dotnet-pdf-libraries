# DinkToPdf Alternative: The Dependency You Didn't Know You Took

Let me show you something I noticed.

If you've shipped a .NET service that generates PDFs in the last seven or eight years, there's a reasonable chance you reached for [DinkToPdf](https://github.com/rdvojmoc/DinkToPdf). It was the obvious answer in the early .NET Core era: a NuGet package, a tidy `IConverter` interface, and an HTML-to-PDF API that worked the first time you called it. The install-day experience really is friction-free. That's the part that's true.

A note up front so you know who's writing this: I'm a Developer Advocate at Iron Software, where we make [IronPDF](https://ironpdf.com/), which competes in this space. I'm obviously not neutral. But my job is to help developers see what they're actually shipping, not to push products, and there is something worth seeing about what DinkToPdf is.

The part that doesn't show up in the install-day experience is the dependency you took underneath it. DinkToPdf is a P/Invoke wrapper. The thing it wraps is wkhtmltopdf, which is software whose upstream GitHub repository was [archived on January 2, 2023](https://github.com/wkhtmltopdf/wkhtmltopdf), with the [whole organization archived on July 10, 2024](https://github.com/wkhtmltopdf). What you installed when you ran `dotnet add package DinkToPdf` is a thin .NET surface over a native library that has not received an upstream release since [version 0.12.6 in June 2020](https://github.com/wkhtmltopdf/wkhtmltopdf/releases).

Here's the install-day code that makes the experience feel so good:

```csharp
using DinkToPdf;
using DinkToPdf.Contracts;

IConverter converter = new SynchronizedConverter(new PdfTools());

byte[] pdf = converter.Convert(new HtmlToPdfDocument
{
    Objects = { new ObjectSettings { HtmlContent = "<h1>Hello, PDF</h1>" } }
});

File.WriteAllBytes("hello.pdf", pdf);
```

Two using-statements, one converter, one call. That is genuinely a clean .NET API. The reason DinkToPdf got popular is right there in those eight lines.

## What DinkToPdf Actually Does Well

Before I get into the dependency chain, let me give the library its due, because it earned the adoption it got. There are real reasons this was the default choice in 2017–2020 .NET Core projects, and several of those reasons still hold up today on their own terms.

**The .NET API is well-designed.** The `IConverter` interface, the `SynchronizedConverter` for thread-safety, the `HtmlToPdfDocument` model, the `ObjectSettings` and `GlobalSettings` separation: these are idiomatic .NET. Whoever wrote this knew the platform. The DI-friendly registration pattern (`services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()))`) drops cleanly into ASP.NET Core's container. For teams writing greenfield .NET Core code in 2018, the API shape was a real upgrade over the older Process.Start-the-CLI patterns that wkhtmltopdf wrappers had used in the .NET Framework era.

**The first PDF lands fast.** The eight-line example above is not pedagogical simplification. That really is the entire setup ceremony, once you've copied the [native binary into your output directory](https://github.com/rdvojmoc/DinkToPdf#nuget). For a developer with a deadline, the speed-to-first-result is meaningful.

**The HTML-rendering capability covers a lot of ground.** Because wkhtmltopdf wraps QtWebKit, you get a real (if dated) browser engine doing the layout. That covers most static HTML reports, invoice templates, statement formats, the whole "render this Razor view as a PDF" workflow that probably accounts for the majority of HTML-to-PDF use cases in line-of-business .NET applications. CSS works. Images work. Reasonable typography works.

**The community knowledge base is large.** Because DinkToPdf was popular for several years, there are answers on Stack Overflow for most of the gotchas. Threading patterns, container deployment recipes, the `useCmaps` flag, font-loading on Linux, the various `--page-size` and `--margin-*` flag mappings: all of it has been written down by someone. For a team taking the dependency in 2026, that institutional memory is part of what they're inheriting along with the library itself.

So the product is real. The API is good. The friction is low. None of what follows is intended to argue otherwise. The question I want to walk through is what is on the other side of the P/Invoke boundary, because that is the half of the picture the install-day experience never shows you.

## The Dependency Chain You Actually Took

Here is the literal dependency graph for a service using DinkToPdf:

```
Your ASP.NET Core service
        │
        ▼
DinkToPdf 1.0.8  ← .NET wrapper, last NuGet release April 2017
        │
        │ P/Invoke (DllImport on libwkhtmltox.{dll,so,dylib})
        ▼
wkhtmltox native library v0.12.4
   (shipped in the DinkToPdf repo under /v0.12.4/)
        │
        ▼
wkhtmltopdf 0.12.x
   (upstream repo archived 2023-01-02; last release 0.12.6, June 2020)
        │
        ▼
QtWebKit (Qt 4 fork)
   (QtWebKit deprecated by Qt in 2015; Qt 4 unsupported since 2015)
        │
        ▼
WebKit (the rendering engine)
   (the wkhtmltopdf-bundled WebKit fork has not been updated since ~2012,
    per the wkhtmltopdf project's own status page)
```

Five layers. The .NET package you reference in your `.csproj` is the top one. The bottom four came along for free, and the bottom three of them are software that the open-source world stopped maintaining at various points between 2012 and 2024. Two specific things are worth pointing out about this graph.

First: the [native binary the DinkToPdf repo distributes](https://github.com/rdvojmoc/DinkToPdf/tree/master/v0.12.4/64%20bit) is wkhtmltox v0.12.4. That is one minor version behind the final wkhtmltopdf release of 0.12.6. The DinkToPdf README still points users to the v0.12.4 folder for the binary, so the most-installed configuration of DinkToPdf in the wild is running a wkhtmltopdf binary that is slightly older than the last upstream release of an already-archived project.

Second: the [wkhtmltopdf project's own status page](https://wkhtmltopdf.org/status.html) — written by the maintainer before the repo was archived — was already explicit that "Qt 4 (which wkhtmltopdf uses) hasn't been supported since 2015, the WebKit in it hasn't been updated since 2012." That status page also recommends, in its own words, that users not pass untrusted HTML to wkhtmltopdf without sanitization, and points at WeasyPrint, Prince, or Puppeteer as alternatives for new projects. That recommendation was written by the people who knew the codebase best.

The dependency chain is not a secret. The DinkToPdf README mentions wkhtmltopdf in its first sentence. The wkhtmltopdf README links to the status page. Nothing here is being concealed. What I'm suggesting is that the documentation stops short of the implication: when you take DinkToPdf, the code you are actually relying on for the rendering work is upstream of the .NET wrapper, and that upstream has an archive flag on it.

## Why "DinkToPdf vs wkhtmltopdf" Is a Distinction Without a Difference

A common framing I see in 2026 production-readiness discussions is "we're not using wkhtmltopdf, we're using DinkToPdf, which is a .NET library." That framing makes the question feel like a .NET ecosystem question rather than an upstream-archive question. It isn't.

DinkToPdf is the wrapper. wkhtmltopdf is the engine. The PDF that gets written to disk is rendered by the engine, not the wrapper. Any rendering bug, any CSS support gap, any font-handling quirk, any security-relevant issue in QtWebKit's WebKit-from-2012 origin: all of those live downstream of the wrapper, in code the wrapper has no ability to fix because the upstream is not accepting changes.

Consider what "still maintained" would have to mean for this stack to behave like a maintained dependency:

- A new CSS feature would need wkhtmltopdf to ship a release. wkhtmltopdf's last release was [June 10, 2020](https://github.com/wkhtmltopdf/wkhtmltopdf/releases). The repo archive flag means there will not be another one without a fork takeover.
- A WebKit-derived security advisory would need a patched wkhtmltopdf binary. The WebKit version inside QtWebKit-inside-wkhtmltopdf hasn't been touched in the upstream sense since approximately 2012.
- A .NET wrapper bug fix would require DinkToPdf to ship a release. DinkToPdf's [last NuGet release was 1.0.8 in April 2017](https://www.nuget.org/packages/DinkToPdf). The GitHub repository still receives issue traffic in 2026, but no published package update has shipped in nine years.

The forks that exist (Haukcode.DinkToPdf, the various WkHtmlToPdf-DotNet community packages) modernize the .NET wrapper meaningfully, and credit to those maintainers for the work. But none of them can fix the rendering engine. The thing the wrapper wraps is the thing that does the work, and that thing is archived. For a production decision in 2026, "DinkToPdf" and "wkhtmltopdf" are operationally the same dependency wearing two different labels.

## The inherited abandonment: free until it costs you

There is a worth-stating gap between "free to install" and "free to operate," and DinkToPdf sits squarely inside it. The DinkToPdf NuGet package is MIT-licensed, costs nothing, and clears procurement review without a conversation. What the procurement review does not surface is the cost of operating a dependency whose .NET wrapper has not shipped a release since April 2017 and whose rendering engine has been archived by its maintainer since 2023.

The cost shows up in three places, and none of them are theoretical.

First, deployment friction. The DinkToPdf GitHub issue tracker carries a long-running pattern of native-binary-loading failures in container environments. [Issue #116](https://github.com/rdvojmoc/DinkToPdf/issues/116) documents `Unable to load shared library 'libwkhtmltox'` on Alpine Linux Docker images on .NET Core 3.1 — the musl-versus-glibc mismatch that Alpine-based deployments routinely hit. [Issue #138](https://github.com/rdvojmoc/DinkToPdf/issues/138) reports the same `DllNotFoundException` from container deployments more generally. Both issues are open or closed-as-stale; neither has been resolved in the published NuGet package because the published NuGet package has not changed since 2017. Teams running modern .NET on slim Linux containers — which is most teams in 2026 — pay for this in deployment-day debugging and in CI flakiness that the dependency's install-day experience never previewed.

Second, compliance exposure. The nine-year gap in DinkToPdf's release history is not a maintainer's stylistic choice; it is a fact a SOC 2 auditor, a SOC 2 customer questionnaire, or an enterprise security review will note. The wrapper carries the archived rendering engine into the dependency tree. CVE-2022-35583 (CVSS 9.8) and CVE-2020-21365 sit downstream of every DinkToPdf install with no patch path. For commercial workloads subject to vulnerability-management standards, "we use the wrapper, not the engine" is not a defense that survives the auditor's enumeration of the transitively-loaded native libraries.

Third, opportunity cost. Every quarter a team spends maintaining DinkToPdf in production — porting around the Alpine issue, isolating the network egress, documenting the compensating controls, justifying the dependency to the next reviewer — is a quarter not spent on the product. The wrapper was free in 2017. It is not free in 2026; it has simply moved the bill from a license line item to an engineering-time line item.

The honest framing for a 2026 commercial adoption is that DinkToPdf is no longer recommended for new projects, and the active community forks (Haukcode.DinkToPdf, the various WkHtmlToPdf-DotNet packages) do not change this conclusion because none of them can fix the engine. For teams carrying an existing DinkToPdf dependency, the migration is genuinely defensible work even when the renders themselves still produce acceptable output.

## How IronPDF's Architecture Differs

This is the part where I tell you about my own product, openly. [IronPDF](https://ironpdf.com/) is also engine-based: a Chromium rendering engine wrapped behind a managed .NET API. The category is the same as DinkToPdf in that respect. What differs is the relationship between the wrapper and the engine.

IronPDF's Chromium engine is bundled and versioned by Iron Software. The current package, [IronPdf 2026.5.2 on NuGet](https://www.nuget.org/packages/IronPdf/), ships with a Chromium build that the vendor packages, tests, and updates. There is no separate "copy this native binary into your output directory" step, no upstream-archive concern about the renderer, no Qt 4 anywhere in the stack. The engine upgrades arrive through normal NuGet updates on a release cadence. Whether that cadence and that vendor relationship are worth the [commercial license cost](https://ironpdf.com/licensing/) is a legitimate question for any team to ask. The architectural point I want to surface is that the engine's maintenance status lives with a vendor whose business depends on keeping it current, rather than with an archived open-source project whose maintainer has moved on.

That is the architectural contrast worth seeing clearly. It is not "free vs. paid." DinkToPdf-the-wrapper is free; the rendering engine it depends on is unmaintained. IronPDF-the-package is commercial; the rendering engine it depends on is vendor-maintained. Both choices are legitimate, and which one is right for a given team depends on what that team's tolerance is for owning the upstream-maintenance question themselves.

## When DinkToPdf Is Still Fine

I want to be specific about where DinkToPdf is still a reasonable choice in 2026, because the dependency-chain story does not turn it into a bad library. It turns it into a library with a clearly bounded fitness window.

DinkToPdf is fine for:

- Internal tools that render trusted HTML you control end-to-end
- Batch jobs running on infrastructure you patch yourself
- Prototypes where the proof-of-concept matters more than the production posture
- Existing services where the renders work and the migration cost would be larger than the value of moving.

None of those use cases is invalidated by anything in this article.

DinkToPdf becomes a harder sell for:

- Services that render HTML originating from outside the trust boundary (the [wkhtmltopdf status page's own recommendation](https://wkhtmltopdf.org/status.html) is to not do this without sanitization)
- Services subject to vulnerability-management standards where the dependency tree gets enumerated at audit time
- Services running on .NET 10 LTS where the multi-year support window outlives the practical viability of an archived rendering engine
- Any greenfield project where the dependency choice is being made today rather than carried forward.

The line is not "use this" or "don't." The line is whether your project is on the side of that boundary where the upstream-archive question matters.

## So What Do You Do With This?

Here is the question I'd actually like you to take away. Open your `.csproj` and look at your DinkToPdf reference. Ask the question the documentation didn't ask for you: do you know what's downstream of that line? Do you know what version of the native binary is in your output directory? Do you know when that binary was last built? Do you know whether your security team, the next time they enumerate your container image's dependencies, is going to ask why an archived project is doing the rendering work in your service?

If the answer to all of those is yes and you've decided the trade-off is fine, that's a real decision and I respect it. The point of writing this wasn't to talk you out of DinkToPdf. It was to make sure that when I said "let me show you something I noticed" at the top of this article, the thing I was showing you was a graph you could see, with dates and links you could check yourself, rather than a vibe.

The library is fine. The wrapper is clean. The dependency chain is the part worth looking at. What does yours look like, and when did you last actually look at it?
