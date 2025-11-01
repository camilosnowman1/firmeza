using Firmeza.Web.Data.Entities;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Firmeza.Web.Documents;

public class SaleInvoiceDocument : IDocument
{
    private readonly Sale _sale;

    public SaleInvoiceDocument(Sale sale)
    {
        _sale = sale;
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
                column.Item().Text("FIRMEZA S.A.").Style(titleStyle);
                column.Item().Text($"Invoice #{_sale.Id}");
                column.Item().Text($"Date: {_sale.SaleDate:yyyy-MM-dd}");
            });

            row.ConstantItem(150).AlignRight().Text("Receipt").FontSize(14);
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
                    column.Item().Text("Bill To:").SemiBold();
                    column.Item().Text(_sale.Customer.FullName);
                    column.Item().Text(_sale.Customer.Document);
                    column.Item().Text(_sale.Customer.Email);
                });
            });

            column.Item().PaddingTop(20).Element(ComposeTable);

            var totalPrice = _sale.TotalAmount;
            column.Item().AlignRight().PaddingTop(10).Text($"Grand Total: {totalPrice:C}", TextStyle.Default.SemiBold().FontSize(14));
        });
    }

    void ComposeTable(IContainer container)
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
                header.Cell().Element(CellStyle).Text("Product");
                header.Cell().Element(CellStyle).AlignCenter().Text("Unit Price");
                header.Cell().Element(CellStyle).AlignCenter().Text("Quantity");
                header.Cell().Element(CellStyle).AlignRight().Text("Total");
                
                static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
            });

            foreach (var item in _sale.SaleDetails)
            {
                table.Cell().Element(CellStyle).Text(item.Product.Name);
                table.Cell().Element(CellStyle).AlignCenter().Text($"{item.UnitPrice:C}");
                table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).AlignRight().Text($"{item.TotalPrice:C}");
                
                static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5); 
            }
        });
    }
}