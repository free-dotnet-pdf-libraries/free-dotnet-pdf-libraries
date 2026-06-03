# iText Commercial License Cost: A CTO's Framework for the Three Legitimate Adoption Paths

*A decision framework for .NET teams reaching for `dotnet add package itext7` — read the dual-license structure deliberately, choose the path that fits your situation, and adopt with intent rather than by default.*

---

## The moment of adoption

Picture the most ordinary version of this decision. A senior .NET developer, mid-sprint, needs PDF generation for a customer feature. They open a terminal, type `dotnet add package itext7`, watch NuGet resolve the dependency, and move on. No license dialog. No compliance prompt. No conversation with legal. The package installs, the code compiles, the feature ships. The license terms — a substantive document with substantive obligations — were technically agreed to at the moment of `dotnet restore`, but the act of agreeing was structurally invisible.

I am Jacob Mellor, CTO and co-founder at Iron Software. *Full disclosure up front: my company sells [IronPDF](https://ironpdf.com/), a commercial .NET PDF library, so I am an interested party in any conversation about commercial PDF licensing.* Leading Iron Software's technical direction for 8+ years, I have had a front-row seat to how document processing has evolved in the .NET ecosystem. The conversation here is not about which library is better. It is about the difference between adopting a library and choosing one. For iText, those two things are not the same, and the difference matters enough to be worth a deliberate look.

This essay offers a framework. iText's licensing is publicly documented, technically clean, and defensible on the maintainer's terms. There are three legitimate paths a .NET team can take with it, and the goal is to make each path a chosen one rather than a discovered one.

## What iText is, and what it has earned

Before any license analysis, the technical reality. A library nobody wants to use does not generate licensing conversations.

iText has been in continuous development since 1999, and is one of a small number of PDF SDKs whose feature coverage maps to substantially the full ISO PDF specification rather than a commercially convenient subset. The .NET port (`itext` on NuGet) reached version [9.6.0 in April 2026](https://www.nuget.org/packages/itext/), with a public cadence that ships multiple minor versions per year. iText is owned by [Apryse](https://apryse.com/), which acquired the iText Group in [April 2022](https://apryse.com/blog/news/pdftron-acquires-itext) and rebranded from PDFTron in February 2023. The transition did not interrupt the engineering trajectory; the codebase has stayed healthy across two decades and a change of ownership.

What makes iText technically credible — and what most adopters are reaching for, even if they could not articulate it at install time — is the depth of standards coverage. PDF/A archival profiles (A-1 through A-4) for record-retention compliance. PDF/UA accessibility tagging for regulated disclosures. ISO-32000-2 (PDF 2.0) features. Full digital-signature workflows including PAdES levels and PKCS11 hardware-token signing. Post-quantum signing primitives in the [9.5 release](https://kb.itextpdf.com/itext/release-itext-core-9-5-0). Brotli-compressed streams, AES-256 encryption, XFA forms, page-level redaction with proper content-stream rewriting (genuinely hard to get right), OCR via the pdfOCR add-on, and a low-level object model that exposes essentially every node of the PDF tree.

That feature surface is uncommon. Regulated workflows in finance, life sciences, and the public sector require PDF/A archival output, digital signatures with timestamping, PDF/UA tagging, and audit-trail formats that map to specific ISO standards. The .NET ecosystem has many PDF libraries; the count whose feature breadth covers the full regulated-industry standards landscape is small, and iText sits at the top of it. When a team picks iText for a regulated-document workflow, the choice is rational on the technical merits — often the most rational technical choice available.

The maintainer's track record reinforces this. The [iText 9 release timeline](https://kb.itextpdf.com/itext/releases) shows steady minor releases, with iText Core 9.6.0 the most recent at the time of writing. iText 8 reaches end-of-life in October 2026 per the [iText Core 9.1 announcement](https://kb.itextpdf.com/itext/release-itext-core-9-1), giving teams a clear migration runway. Release notes read like the changelog of a library taken seriously: appropriate bug-fix density, deprecation cycles communicated in advance, ISO standards work tracked publicly. The post-acquisition Apryse era has not slowed the cadence. By the standards of long-lived open-source projects, iText sits in the top decile on maintainership consistency.

The dual-license structure is itself a virtue worth naming. iText publishes the AGPL terms on its own site in plain English, explains the obligations in the maintainer's voice, and offers a clearly delineated commercial alternative. The posture is not ambiguous, not silent, not requiring legal interpretation — it is consistently presented across iText's website, NuGet metadata, and source repositories. A team that wants to know what they are agreeing to can know in fifteen minutes of reading.

Legitimate use cases for iText AGPL are real and substantial. Open-source projects whose own license is AGPL-compatible can adopt iText with the obligations posing no marginal cost; the product is already open. Academic and reproducible-research workflows align naturally with source disclosure. Internal-only enterprise tools that are not network-deployed to external users sit outside §13's network-interaction trigger. Government and public-sector deployments where source transparency is itself a goal can choose AGPL deliberately. SaaS products explicitly built on AGPL-compatible terms are viable too; some companies treat source disclosure as a product feature rather than a constraint. None of these are workarounds; they are the use cases the maintainer designed the AGPL distribution to serve.

The commercial tier, in turn, is a reasonable professionally-supported path for teams whose deployment model does not align with AGPL. Customers receive warranted intellectual property, professional support and maintenance per the [How to buy](https://itextpdf.com/how-buy) page, release from the AGPL obligations, and a contractual relationship with a commercial entity. The maintainer's business model is to monetize commercial use through this tier, and that model is defensible: it funds the engineering depth that produces the standards coverage. Without commercial revenue, the breadth of the open-source distribution would not exist on this scale.

## The license, in iText's own words

iText is published under a [dual-license model](https://itextpdf.com/how-buy). The free distribution is licensed under the [GNU Affero General Public License version 3](https://www.gnu.org/licenses/agpl-3.0.en.html). The paid distribution is licensed under one of Apryse's commercial agreements: a Volume-based subscription priced on the quantity of PDF files processed, or an OEM Distribution license with custom terms. Both ship the same technical capability; the difference is the licensing terms.

iText's [AGPL documentation page](https://itextpdf.com/how-buy/AGPLv3-license) describes the obligations in the maintainer's voice. Three statements, quoted verbatim:

> "you must prominently mention iText and include the iText copyright and AGPL license in output file metadata, and also retain the producer line in every PDF that is created or manipulated using iText."

> "You may not deploy it on a network without disclosing the full source code of your own applications under the AGPL license. You must distribute all source code, including your own product and web-based applications."

> "It's a legal violation to use iText Core/Community and our open source add-ons in a non-AGPL environment."

Those statements come from the maintainer, not from a critic. The structural implication: iText's vendor explicitly characterizes use of the free distribution inside a closed-source product as outside the license's intended scope. Apryse intends the AGPL distribution as the deliberate-AGPL path and the commercial distribution as the closed-source-product path. Both are legitimate; the choice is the adopter's.

The legal mechanism behind the network-disclosure obligation is AGPL §13, quoted in full from the [Free Software Foundation's canonical text](https://www.gnu.org/licenses/agpl-3.0.en.html):

> "Notwithstanding any other provision of this License, if you modify the Program, your modified version must prominently offer all users interacting with it remotely through a computer network (if your version supports such interaction) an opportunity to receive the Corresponding Source of your version by providing access to the Corresponding Source from a network server at no charge, through some standard or customary means of facilitating copying of software."

§13 is what makes AGPL different from GPL. Standard GPL is triggered by *distribution* of the binary. AGPL adds a trigger for *network interaction*: a SaaS company that runs AGPL code on its own servers and never ships the binary is still inside §13's scope the moment a user interacts with the application over a network. The maintainer's plain-English summary is the same obligation expressed in product terms.

The implications are non-obvious to casual readers. A .NET team accustomed to MIT and Apache 2.0 dependencies, where the full set of obligations is "preserve the copyright notice," may model iText's license slot in a manifest as similar in shape. It is structurally different. That difference is what deserves the deliberate look.

## The AGPL trap and what it costs closed-source teams

The AGPL framing in the maintainer's own documentation is the second move in a two-step history that most teams installing `dotnet add package itext7` have not heard. The first move happened on December 1, 2009. Up to that date, iText shipped under the [MPL/LGPL dual-license](https://en.wikipedia.org/wiki/IText) — a permissive arrangement that placed iText in the same risk category as MIT or Apache 2.0 dependencies. iText 2.1.7 was the [last release under those terms](https://kb.itextpdf.com/it5kb/can-itext-2-1-7-itextsharp-4-1-6-or-earlier-be-use). iText 5 and everything after it shipped under AGPL v3.

That switch is the moment "iText is free" stopped being a complete sentence. Every team running iText 5, 7, 8, or 9 in a closed-source product is now operating under a copyleft license whose §13 network-interaction clause is structurally incompatible with the SaaS deployment model most .NET teams default to. The maintainer's commercial license is the documented escape hatch. The trap, as it is often experienced, is not the license itself — it is the installation that bypassed the license entirely.

The pattern repeats often enough to be predictable. A team prototypes with iText AGPL. The prototype ships. Two years later, a SOC 2 reviewer, an acquisition due-diligence team, or an enterprise customer's security questionnaire flags iText in the SBOM and asks whether the company holds a commercial license. The answer is no, because nobody ever bought one. The team's three options at that moment are the same as Apryse intends: open-source the entire codebase (rarely feasible), purchase a commercial subscription on terms negotiated under time pressure, or migrate to a different library at the cost of weeks of engineering time the roadmap did not budget for.

There is nothing dishonest about iText's posture. The license terms are published clearly, the maintainer explains them in their own voice, and the AGPL distribution is itself a legitimate offering for teams whose deployment model fits. The trap is structural, not editorial. It exists because `dotnet add package itext7` does not read a license file aloud, and because the version of "free" most developers carry in their head is closer to MIT than to AGPL. For closed-source commercial teams, the practical answer is to treat iText as a paid library by default and to engage Apryse sales before adoption rather than after the audit.

This is the case where "not recommended for commercial use" needs the qualifier "not recommended for closed-source commercial use without a commercial license." iText is unambiguously safe for AGPL-compatible projects and unambiguously expensive — in license fees, migration cost, or compliance exposure — for everything else.

## What the commercial path actually looks like

Apryse does not publish a price card; the [How to buy](https://itextpdf.com/how-buy) page directs interested customers to sales for both commercial offerings. The **Volume-based subscription** scales cost with usage — "based on the quantity of PDF files processed" per the maintainer's description. The transition from perpetual to subscription pricing happened on [April 2, 2020](https://itextpdf.com/blog/itext-news/itext-transitions-subscription-based-commercial-licenses), so any pricing comparison from before that date no longer applies. The **OEM Distribution license** is for vendors who embed iText in a shipped product; pricing is fully bespoke. Both tiers release the customer from AGPL obligations and provide professional support and maintenance.

The honest point about pricing is structural: cost-modeling for iText commercial requires a sales conversation. That is not a defect; it is consistent with a usage-shaped commercial model. It does mean the cost of Path B (below) cannot be modeled from public sources alone. A team considering iText commercial should engage Apryse sales early, scope the deployment honestly, and obtain a quote reflecting their actual usage profile before treating commercial-iText as a known-cost option. Engaging early in this context means what it means with any usage-priced enterprise vendor: an initial discovery conversation is normally free and does not commit either side to anything; an honest scope discussion of expected document volumes, deployment footprint, and any standards-coverage features the workload depends on costs nothing beyond the meeting time; and the resulting written quote is something the team can take into the build-vs-buy review without obligation. None of those steps commits a team to licensing — they exist so the team can model Path B accurately before the decision is in front of the budget owner.

## The three legitimate paths

For a .NET team standing at the moment of `dotnet add package itext7`, three paths are legitimate, and the right one depends on the team's situation.

### Path A: Adopt iText AGPL deliberately

For teams whose deployment context aligns with AGPL's obligations: open-source projects whose own license is AGPL-compatible, academic and reproducible-science workflows, internal-only enterprise tools that never expose iText-using code to external network users, government and public-sector projects where source disclosure aligns with mission, and SaaS products explicitly built on AGPL-compatible terms.

Criterion: the team has read AGPL §13, confirmed with counsel that the deployment model meets the obligations, and is choosing AGPL because it fits — not because it was the default at install time. Cost: ongoing engineering discipline around attribution and source-disclosure obligations. Benefit: zero license fee for a feature surface that would otherwise be expensive.

### Path B: Adopt iText commercial

For teams who need iText's specific feature set — PDF/A breadth, PAdES signing, PDF/UA tagging, advanced redaction, OCR — inside a closed-source commercial product. The commercial license is the professionally-supported channel for using iText in a closed-source context.

Criterion: the team needs coverage that iText offers and a non-iText alternative does not (or does not at acceptable engineering cost), and the deployment model is closed-source commercial. Cost: the commercial subscription, scaling with usage. Benefit: full feature surface with warranted IP, professional support, and a contractual counterparty for procurement. For SaaS deployments at scale, the line item is real and grows with the business; the implication is to engage sales early and model the cost honestly against the value the feature surface delivers, before the team has architected itself into a position where switching libraries would be expensive.

### Path C: Use a different library

For teams whose required feature set does not justify the iText commercial cost, or whose licensing posture is permissive-only (MIT, Apache 2.0) by policy. The .NET PDF ecosystem includes multiple legitimate alternatives across MIT-licensed open-source, permissively-licensed commercial, and free-for-commercial-use options. IronPDF, the library Iron Software builds, is one option in this space; there are others. Path C is not a single product recommendation; it is the deliberate choice to use a library whose license posture matches the team's deployment model, when neither iText path fits.

Criterion: the feature set the team actually needs is achievable on a non-iText library at acceptable engineering investment, and the team's licensing posture is misaligned with AGPL. Cost: the engineering work to evaluate alternatives and possibly accept a feature gap. Benefit: a license slot in the dependency manifest that does not require a sales conversation or an AGPL §13 reading to defend.

## Closing

The moment we opened with — a developer typing `dotnet add package itext7`, NuGet resolving, the feature shipping — deserves a deliberate answer from your team, not a default one. iText is a technically excellent library with a clearly documented dual-license structure, and either path within iText is legitimate when chosen on its merits: AGPL with intent, or commercial with contract. So is the third path, of using a different library when neither iText path fits. What is not legitimate is the unchosen path — an AGPL dependency inside a closed-source commercial product, adopted as a side-effect of `dotnet add` and never modeled against the obligations the maintainer has documented in plain English on its own website. That posture is the one this framework exists to make harder to fall into by default.

The deliberate read takes an afternoon. The implications of skipping it can compound for years.

---

*Jacob Mellor is CTO and co-founder of [Iron Software](https://ironsoftware.com/), maker of [IronPDF](https://ironpdf.com/) and other commercial .NET libraries. License terms, version numbers, and quoted text in this article were verified against publicly available sources in May 2026; AGPL §13 is quoted from the [Free Software Foundation's canonical license text](https://www.gnu.org/licenses/agpl-3.0.en.html).*
