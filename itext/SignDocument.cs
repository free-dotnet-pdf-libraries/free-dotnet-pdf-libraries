// NuGet: itext7, itext7.bouncy-castle-adapter
using System.IO;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Bouncycastle.Crypto;
using iText.Kernel.Pdf;
using iText.Signatures;

namespace FreeDotNetPdf.IText;

/// <summary>
/// Use case: apply a detached PKI digital signature (CMS/PAdES). iText 9 also ships a
/// dedicated signature-validation module (ISO/TS 32003 &amp; 32004) to verify signatures.
/// NOTE: the BouncyCastle adapter types depend on your iText bouncy-castle package
/// (itext.bouncy-castle-adapter); confirm exact types against your installed 9.x version.
/// </summary>
public static class SignDocument
{
    public static void Sign(string inputPath, string outputPath, IPrivateKey privateKey, IX509Certificate[] chain)
    {
        using var output = new FileStream(outputPath, FileMode.Create);
        var signer = new PdfSigner(new PdfReader(inputPath), output, new StampingProperties());

        IExternalSignature pks = new PrivateKeySignature(privateKey, DigestAlgorithms.SHA256);
        signer.SignDetached(new BouncyCastleDigest(), pks, chain,
            crlList: null, ocspClient: null, tsaClient: null, estimatedSize: 0,
            sigtype: PdfSigner.CryptoStandard.CMS);
    }
}
