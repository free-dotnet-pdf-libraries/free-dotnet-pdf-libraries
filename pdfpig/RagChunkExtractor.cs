using System.Collections.Generic;
using UglyToad.PdfPig;

namespace FreeDotNetPdf.PdfPig;

/// <summary>
/// Use case: ingestion for search indexing / RAG / document-AI pipelines.
/// Apache-2.0 and pure managed — yields clean per-page text for embedding or indexing.
/// </summary>
public static class RagChunkExtractor
{
    public static IEnumerable<string> ExtractChunks(string pdfPath)
    {
        using var document = PdfDocument.Open(pdfPath);
        foreach (var page in document.GetPages())
            yield return page.Text;
    }
}
