using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;

namespace FreeDotNetPdf.IText;

/// <summary>
/// Use case: fill named AcroForm fields from data and flatten them into static
/// content — the standard pattern for generating filled government/HR/contract forms.
/// </summary>
public static class FillAndFlattenForm
{
    public static void Fill(string templatePath, string outputPath)
    {
        using var pdf = new PdfDocument(new PdfReader(templatePath), new PdfWriter(outputPath));
        var form = PdfFormCreator.GetAcroForm(pdf, true);

        form.GetField("applicant_name").SetValue("Ada Lovelace");
        form.GetField("date").SetValue("2026-06-03");

        form.FlattenFields(); // bake values in so they can no longer be edited
    }
}
