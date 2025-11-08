using Firmeza.Core.Entities; // Corrected namespace
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
                page.Footer().AlignCenter().Text(x =>
                {
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
    }

    void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Sale Invoice").SemiBold().FontSize(24);
                column.Item().Text($"Invoice ID: {_sale.Id}");
                column.Item().Text($"Date: {_sale.SaleDate:yyyy-MM-dd}");
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(20);

            column.Item().Text("Customer Details").SemiBold();
            column.Item().Text($"Name: {_sale.Customer.FullName}");
            column.Item().Text($"Document: {_sale.Customer.Document}");
            column.Item().Text($"Email: {_sale.Customer.Email}");

            column.Item().Element(ComposeTable);

            column.Item().AlignRight().Text($"Total Amount: {_sale.TotalAmount:C}").SemiBold().FontSize(16);
        });
    }

    void ComposeTable(IContainer container)
    {
        var headerStyle = TextStyle.Default.SemiBold();

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
                header.Cell().Text("Product").Style(headerStyle);
                header.Cell().AlignRight().Text("Unit Price").Style(headerStyle);
                header.Cell().AlignRight().Text("Quantity").Style(headerStyle);
                header.Cell().AlignRight().Text("Total").Style(headerStyle);
            });

            foreach (var item in _sale.SaleDetails)
            {
                table.Cell().Text(item.Product.Name);
                table.Cell().AlignRight().Text(item.UnitPrice.ToString("C"));
                table.Cell().AlignRight().Text(item.Quantity.ToString());
                table.Cell().AlignRight().Text(item.TotalPrice.ToString("C"));
            }
        });
    }
}