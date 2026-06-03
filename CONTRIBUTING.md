# Contributing

Thanks for helping keep this an honest, accurate catalog of **free .NET PDF libraries**. Corrections and additions are welcome — the bar is *accuracy and evidence*, not enthusiasm.

## Scope

This list covers **free, open-source PDF libraries usable from C#/.NET**. "Free" means installable and usable at no cost under its license — including copyleft (AGPL/LGPL) and revenue-gated (e.g. MIT-with-threshold) libraries, as long as a no-cost path exists. Commercial-only libraries are out of scope except as clearly-labelled comparison context.

## How to propose a change

1. **Open an issue or a pull request.** For factual corrections (a license change, a new release, an archived project), a one-line PR with a citation is ideal.
2. **Cite your source.** Every non-obvious claim must be backed by a primary source: the project's own docs, repo, NuGet page, license file, or issue tracker — **not** a marketing page or a third-party blog. Add it to the `Sources` table in `README.md` when relevant.
3. **Keep the voice neutral and evidence-first.** No hype, no vendor spin. If a library is good, show why; if it has a catch, name it.

## Adding or correcting a library entry

Each library has a top-level folder (e.g. `questpdf/`) containing:

- `README.md` — the deep-dive analysis.
- 2–3 `*.cs` **reference snippets** — concise, idiomatic C# for the services that library excels at.

When you add or edit a library entry in the main `README.md`, follow the existing per-library template exactly:

1. **Positioning line** — one-sentence summary (engine, license, latest release, status).
2. **Context paragraph** — what it is and why teams choose it.
3. **Strengths** — bulleted.
4. **The catch** — bulleted; the honest downsides.
5. **Best for / Avoid if** — one line each.
6. **Code sample** — minimal, working C#.
7. **License reality** — the actual terms a closed-source commercial user faces.

### Code snippet conventions

- Namespace `FreeDotNetPdf.<Library>`.
- Keep each file focused on **one** use case, with a `<summary>` doc-comment explaining it.
- Snippets are **reference only** (no `.csproj`). They should be correct and idiomatic, but assume the reader supplies the NuGet package and a host project.
- Note any non-obvious requirement inline (e.g. PdfSharp's font resolver, iText's BouncyCastle adapter).

## Style & automation

- Descriptions are full sentences and end with a period.
- Match the existing Markdown formatting; the **Awesome Lint** CI workflow runs `awesome-lint` on every PR.
- The **Link Check** workflow validates all Markdown links on every PR and weekly — fix any broken link your change introduces.
- Update the `Last verified` badge/date in `README.md` when you revise capability or license claims.

## License

By contributing, you agree your contributions are dedicated to the public domain under [CC0 1.0 Universal](LICENSE).
