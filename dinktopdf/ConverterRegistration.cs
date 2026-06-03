using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace FreeDotNetPdf.DinkToPdf;

/// <summary>
/// Use case: register the thread-safe native converter once as a singleton in ASP.NET Core.
/// SynchronizedConverter serializes calls into the non-reentrant native wkhtmltopdf library,
/// which is the correct, crash-avoiding pattern for web apps.
/// </summary>
public static class ConverterRegistration
{
    public static void AddDinkToPdf(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
    }
}
