using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CourseManagement.Services;

public class CertificateService : ICertificateService
{
    public CertificateService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateCertificate(string traineeName, string certificationName, DateOnly? achievedDate)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header().Text("Certificate of Completion").SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(x =>
                {
                    x.Spacing(20);
                    x.Item().Text("This is to certify that").FontSize(20);
                    x.Item().Text(traineeName).FontSize(30).SemiBold();
                    x.Item().Text("has successfully completed the").FontSize(20);
                    x.Item().Text(certificationName).FontSize(30).SemiBold();
                    x.Item().Text($"on {achievedDate?.ToString("yyyy-MM-dd")}").FontSize(20);
                });
            });
        });

        return document.GeneratePdf();
    }
}
