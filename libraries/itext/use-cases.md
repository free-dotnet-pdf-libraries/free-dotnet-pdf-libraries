# iText — 3 Use Cases Where It Excels

iText is the most **complete** PDF library in .NET: full-spec creation, editing, forms, signatures, PDF/A, PDF/UA, and redaction. Its use cases are the ones no lightweight library can touch. The cost is licensing — **AGPL v3 or a commercial license** — so these are "worth it" exactly when the feature is non-negotiable.

> Verified against [itext7 9.6.0 on NuGet](https://www.nuget.org/packages/itext7), the [iText 9.0 release notes](https://itextpdf.com/blog/technical-notes/itext-suite-90-new-security-standards-signature-validation), and [iText Suite 9.3/9.4](https://itextpdf.com/blog/itext-suite-93-smarter-validation-enhanced-ocr-smaller-files) (June 2026). API names may shift slightly across 9.x point releases — check the docs for your exact version.

---

### 1. Fill and flatten AcroForm / interactive PDF forms

iText reads existing form fields by name, sets values, and flattens them into static content — the standard pattern for generating filled government, HR, and contract forms from data.

```csharp
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;

using var pdf = new PdfDocument(new PdfReader("template.pdf"), new PdfWriter("filled.pdf"));
var form = PdfFormCreator.GetAcroForm(pdf, true);

form.GetField("applicant_name").SetValue("Ada Lovelace");
form.GetField("date").SetValue("2026-06-03");
form.FlattenFields();          // bake values in so they can't be edited
```

### 2. Digitally sign and validate documents

iText 9 finalized a dedicated signature **validation** module (ISO/TS 32003 & 32004) alongside signing — so it both applies and verifies PKI signatures, the core of audit-trail and compliance workflows.

```csharp
using iText.Kernel.Pdf;
using iText.Signatures;

var signer = new PdfSigner(
    new PdfReader("contract.pdf"),
    new FileStream("signed.pdf", FileMode.Create),
    new StampingProperties());

IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
signer.SignDetached(new BouncyCastleDigest(), pks, certificateChain,
    null, null, null, 0, PdfSigner.CryptoStandard.CMS);
```

### 3. Irreversible redaction with pdfSweep

iText's `pdfSweep` add-on truly removes content (not just a black box over it), and can auto-target by regex — the right tool for scrubbing PII/PHI before disclosure.

```csharp
using iText.Kernel.Pdf;
using iText.PdfCleanup;
using iText.PdfCleanup.Autosweep;

using var pdf = new PdfDocument(new PdfReader("source.pdf"), new PdfWriter("redacted.pdf"));

var strategy = new RegexBasedCleanupStrategy(@"\d{3}-\d{2}-\d{4}");   // e.g. US SSNs
PdfCleaner.AutoSweepCleanUp(pdf, strategy);
```

> **License note:** AGPL v3 by default — using iText in a closed-source product or hosted service requires a commercial license. Also evaluate it for **PDF/A archival** and **PDF/UA accessible** output, both first-class in iText.
