using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
namespace CourseManagement.Services;

public class CertificateService : ICertificateService
{
    public CertificateService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
    
    private static byte[] GenerateQrCode(string text)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrData);
        return qrCode.GetGraphic(20);
    }

    public     byte[] GenerateCertificate(
        string traineeName,
        string certificationName,
        string certificateId,
        string verifyUrl,
        DateOnly? achievedDate)
    {
        
        var qrBytes = GenerateQrCode(verifyUrl);
        
        var dateText = achievedDate?.ToString("MMMM dd, yyyy") ?? "N/A";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(15);
                page.PageColor("#FDFCF8");

                page.DefaultTextStyle(x =>
                    x.FontFamily("Times New Roman"));

                page.Content()
                    .Border(2)
                    .BorderColor("#13233F")
                    .Padding(6)
                    .Border(1)
                    .BorderColor("#10B6DA")
                    .Padding(25)
                    .Column(column =>
                    {
                        column.Spacing(8);

                        // Academy
                        column.Item()
                            .AlignCenter()
                            .Text("CourseManagement Academy")
                            .FontSize(22)
                            .Bold()
                            .FontColor("#13233F");

                        column.Item()
                            .AlignCenter()
                            .Text("PROFESSIONAL TRAINING & CERTIFICATION")
                            .FontSize(9)
                            .FontColor("#5B7196");

                        column.Item()
                            .AlignCenter()
                            .Text("CERTIFICATE OF COMPLETION")
                            .FontSize(11)
                            .Bold()
                            .FontColor("#0A93B2");

                        // Main Title
                        column.Item()
                            .PaddingTop(5)
                            .AlignCenter()
                            .Text("Certificate of Achievement")
                            .FontSize(36)
                            .SemiBold()
                            .FontColor("#13233F");

                        column.Item()
                            .PaddingTop(12)
                            .AlignCenter()
                            .Text("This is to certify that")
                            .FontSize(14)
                            .FontColor("#5B7196");

                        // Trainee
                        column.Item()
                            .AlignCenter()
                            .Text(traineeName)
                            .FontSize(42)
                            .Bold()
                            .FontColor("#13233F");

                        // Gold divider
                        column.Item()
                            .AlignCenter()
                            .Width(320)
                            .BorderBottom(1)
                            .BorderColor("#B08A3E");

                        column.Item()
                            .AlignCenter()
                            .Text("has successfully completed all requirements for")
                            .FontSize(13)
                            .FontColor("#5B7196");

                        // Certification
                        column.Item()
                            .PaddingTop(4)
                            .AlignCenter()
                            .Text(certificationName)
                            .FontSize(24)
                            .Bold()
                            .FontColor("#13233F");

                        column.Item()
                            .AlignCenter()
                            .Text("Professional Certification Track")
                            .FontSize(10)
                            .FontColor("#5B7196");

                        column.Item().PaddingTop(20);

                        // Footer
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item()
                                    .Width(150)
                                    .BorderBottom(1)
                                    .BorderColor("#13233F");

                                left.Item()
                                    .Text(dateText)
                                    .FontSize(13)
                                    .Bold();

                                left.Item()
                                    .Text("DATE OF ISSUE")
                                    .FontSize(8)
                                    .FontColor("#5B7196");
                            });

                            row.RelativeItem().AlignCenter().Column(center =>
                            {
                                center.Item()
                                    .AlignCenter()
                                    .Text("✓")
                                    .FontSize(34)
                                    .Bold()
                                    .FontColor("#10B6DA");

                                center.Item()
                                    .AlignCenter()
                                    .Text("CERTIFIED")
                                    .FontSize(9)
                                    .Bold()
                                    .FontColor("#13233F");
                            });

                            row.RelativeItem().AlignRight().Column(right =>
                            {
                                right.Item()
                                    .AlignRight()
                                    .Text(certificateId)
                                    .FontSize(10)
                                    .Bold()
                                    .FontColor("#13233F");

                                right.Item()
                                    .AlignRight()
                                    .Text("CERTIFICATE ID")
                                    .FontSize(8)
                                    .FontColor("#5B7196");

                                right.Item()
                                    .AlignRight()
                                    .Text("Scan QR code to verify")
                                    .FontSize(8)
                                    .Italic()
                                    .FontColor("#0A93B2");

                                // QR Code 
                                right.Item()
                                    .PaddingTop(5)
                                    .AlignRight()
                                    .Width(60)
                                    .Image(qrBytes);
                            });
                        });
                    });
            });
        }).GeneratePdf();
    }
}