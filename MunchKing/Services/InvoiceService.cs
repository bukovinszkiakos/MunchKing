using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using MunchKing.Context;
using Microsoft.EntityFrameworkCore;

namespace MunchKing.Services
{
    public class InvoiceService
    {
        private readonly ApplicationDbContext _context;

        public InvoiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int orderId, string userId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.FoodItem)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null) throw new Exception("Order not found");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(50);

                    page.Header().Text($"Invoice - Order #{order.Id}").FontSize(20).Bold();

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Product");
                            header.Cell().Element(CellStyle).Text("Unit Price");
                            header.Cell().Element(CellStyle).Text("Qty");
                            header.Cell().Element(CellStyle).Text("Total");

                            static IContainer CellStyle(IContainer container) =>
                                container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5);
                        });

                        foreach (var item in order.OrderItems)
                        {
                            table.Cell().Text(item.FoodItem.Name);
                            table.Cell().Text($"{item.UnitPrice:C}");
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text($"{item.UnitPrice * item.Quantity:C}");
                        }
                    });

                    var total = order.OrderItems.Sum(i => i.UnitPrice * i.Quantity);
                    page.Footer().AlignRight().Text($"Total: {total:C}").Bold().FontSize(14);
                });
            });

            return document.GeneratePdf();
        }
    }
}
