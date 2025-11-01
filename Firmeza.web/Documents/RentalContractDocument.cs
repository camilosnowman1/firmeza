using Firmeza.Web.Data.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Web.Documents;

public class RentalContractDocument : IDocument
{
    private readonly Rental _rental;

    public RentalContractDocument(Rental rental)
    {
        _rental = rental;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container
            .Page(page =>
            {
                page.Margin(50);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);

                page.Footer().AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
    }

    void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("FIRMEZA S.A. - Rental Contract").Style(titleStyle);
                column.Item().Text($"Contract #{_rental.Id}");
                column.Item().Text($"Date: {_rental.CreatedAt:yyyy-MM-dd}");
            });

            row.ConstantItem(150).AlignRight().Text("Rental Agreement").FontSize(14);
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Rented To:").SemiBold();
                    column.Item().Text(_rental.Customer.FullName);
                    column.Item().Text(_rental.Customer.Document);
                    column.Item().Text(_rental.Customer.Email);
                });
            });

            column.Item().PaddingTop(20).Element(ComposeRentalDetails);

            column.Item().PaddingTop(20).Text("Terms and Conditions:").SemiBold();
            column.Item().Text("1. The vehicle must be returned in the same condition as received.");
            column.Item().Text("2. Any damage will be charged to the customer.");
            column.Item().Text("3. Late returns will incur additional charges.");

            var totalAmount = _rental.TotalAmount;
            column.Item().AlignRight().PaddingTop(20).Text($"Total Amount: {totalAmount:C}", TextStyle.Default.SemiBold().FontSize(14));
        });
    }

    void ComposeRentalDetails(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Vehicle");
                header.Cell().Element(CellStyle).AlignCenter().Text("Hourly Rate");
                header.Cell().Element(CellStyle).AlignCenter().Text("Start Date");
                header.Cell().Element(CellStyle).AlignRight().Text("End Date");
                
                static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            });

            table.Cell().Element(CellStyle).Text(_rental.Vehicle.Name);
            table.Cell().Element(CellStyle).AlignCenter().Text($"{_rental.Vehicle.HourlyRate:C}");
            table.Cell().Element(CellStyle).AlignCenter().Text($"{_rental.StartDate:yyyy-MM-dd HH:mm}");
            table.Cell().Element(CellStyle).AlignRight().Text($"{_rental.EndDate:yyyy-MM-dd HH:mm}");
            
            static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5); 
        });
    }
}