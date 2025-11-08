using Firmeza.Core.Entities; // Corrected namespace
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
                column.Item().Text("Rental Contract").SemiBold().FontSize(24);
                column.Item().Text($"Contract ID: {_rental.Id}");
                column.Item().Text($"Date: {_rental.CreatedAt:yyyy-MM-dd}");
            });
        });
    }

    void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(20);

            column.Item().Text("Customer Details").SemiBold();
            column.Item().Text($"Name: {_rental.Customer.FullName}");
            column.Item().Text($"Document: {_rental.Customer.Document}");
            column.Item().Text($"Email: {_rental.Customer.Email}");

            column.Item().Text("Vehicle Details").SemiBold();
            column.Item().Text($"Vehicle: {_rental.Vehicle.Name}");
            column.Item().Text($"Hourly Rate: {_rental.Vehicle.HourlyRate:C}");

            column.Item().Text("Rental Period").SemiBold();
            column.Item().Text($"Start Date: {_rental.StartDate:yyyy-MM-dd HH:mm}");
            column.Item().Text($"End Date: {_rental.EndDate:yyyy-MM-dd HH:mm}");

            column.Item().AlignRight().Text($"Total Amount: {_rental.TotalAmount:C}").SemiBold().FontSize(16);
        });
    }
}