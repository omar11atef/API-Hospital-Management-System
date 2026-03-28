using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;

public class PatientReportDocument : IDocument
{
    private readonly PatientPdfReportData _data;
    private readonly byte[] _logoImage;

    public PatientReportDocument(PatientPdfReportData data, byte[] logoImage)
    {
        _data = data;
        _logoImage = logoImage;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(40);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily(Fonts.Arial));

                // ربط الأقسام الثلاثة للصفحة
                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
    }

    // 1. تصميم الترويسة العلوية (الشعار ومعلومات المريض)
    private void ComposeHeader(IContainer container)
    {
        var primaryTeal = "#00A09E";
        var primaryGreen = "#2AA850";

        container.PaddingBottom(20).Row(row =>
        {
            if (_logoImage != null)
            {
                row.ConstantItem(120).Height(120).Image(_logoImage, ImageScaling.FitArea);
            }

            row.RelativeItem().PaddingLeft(20).Column(column =>
            {
                column.Item().Text("Valerio Medical Clinic")
                    .FontSize(24).SemiBold().FontColor(primaryTeal);

                column.Item().Text("Your Health, Our Passion")
                    .FontSize(10).Italic().FontColor(primaryGreen);

                column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                column.Item().PaddingTop(10).Text("PATIENT MEDICAL REPORT").FontSize(16).SemiBold();
                column.Item().Text($"Patient Name: {_data.PatientName}").SemiBold();
                //column.Item().Text($"Email: {_data.Email} | Blood Type: {_data.BloodType}");
                column.Item().Text($"Date Generated: {_data.GeneratedAt:dd MMM yyyy, HH:mm}");
            });
        });
    }

    // 2. تصميم المحتوى (جدول المواعيد)
    private void ComposeContent(IContainer container)
    {
        var primaryTeal = "#00A09E";
        var primaryGreen = "#2AA850";

        container.PaddingVertical(20).Column(column =>
        {
            column.Spacing(10);

            // عنوان القسم
            column.Item().Text("Appointment History")
                .FontSize(14).SemiBold().FontColor(primaryTeal);

            // رسم الجدول
            column.Item().Table(table =>
            {
                // تقسيم الأعمدة
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2); // Date
                    columns.RelativeColumn(3); // Doctor
                    columns.RelativeColumn(2); // Department
                    columns.RelativeColumn(2); // Room
                    columns.RelativeColumn(2); // Status
                });

                // ترويسة الجدول (بخلفية خضراء وخط أبيض)
                table.Header(header =>
                {
                    header.Cell().Background(primaryGreen).Padding(5).Text("Date").FontColor(Colors.White).SemiBold();
                    header.Cell().Background(primaryGreen).Padding(5).Text("Doctor").FontColor(Colors.White).SemiBold();
                    header.Cell().Background(primaryGreen).Padding(5).Text("Department").FontColor(Colors.White).SemiBold();
                    header.Cell().Background(primaryGreen).Padding(5).Text("Room").FontColor(Colors.White).SemiBold();
                    header.Cell().Background(primaryGreen).Padding(5).Text("Status").FontColor(Colors.White).SemiBold();
                });

                // تعبئة البيانات
                if (_data.Appointments != null)
                {
                    foreach (var item in _data.Appointments)
                    {
                        // إضافة حدود سفلية خفيفة لكل صف لتسهيل القراءة
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Date);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.DoctorName);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.DepartmentName);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.RoomNumber);
                        table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(item.Status);
                    }
                }
            });
        });
    }

    // 3. تصميم الترويسة السفلية (أرقام الصفحات)
    private void ComposeFooter(IContainer container)
    {
        container.AlignCenter().Text(x =>
        {
            x.Span("Page ").FontColor(Colors.Grey.Medium);
            x.CurrentPageNumber().FontColor(Colors.Grey.Medium);
            x.Span(" of ").FontColor(Colors.Grey.Medium);
            x.TotalPages().FontColor(Colors.Grey.Medium);
        });
    }
}