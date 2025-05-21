using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MunchKing.DTOs;
using MunchKing.Models;
using MunchKing.Services;
using System.Security.Claims;
using MunchKing.Enums;
using Microsoft.EntityFrameworkCore;
using MunchKing.Context;

namespace MunchKing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly InvoiceService _invoiceService;
        private readonly ApplicationDbContext _context;

        public OrdersController(IOrderService orderService, InvoiceService invoiceService, ApplicationDbContext context)
        {
            _orderService = orderService;
            _invoiceService = invoiceService;
            _context = context;
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var orderId = await _orderService.PlaceOrderAsync(userId, request);
            return Ok(new { orderId });
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var orders = await _orderService.GetUserOrdersAsync(userId);
            return Ok(orders);
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            try
            {
                var updated = await _orderService.UpdateOrderStatusAsync(orderId, request.NewStatus);
                return updated ? Ok(new { message = "Order status updated." }) : NotFound();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }



        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("admin/all-orders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] string? status)
        {
            var orders = await _orderService.GetAllOrdersForAdminAsync(status);
            return Ok(orders);
        }


        [HttpGet("available-statuses")]
        public IActionResult GetAvailableStatuses()
        {
            var statuses = Enum.GetNames(typeof(OrderStatus));
            return Ok(statuses);
        }


        [HttpGet("invoice/{orderId}")]
        public async Task<IActionResult> GetInvoice(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            try
            {
                var pdf = await _invoiceService.GenerateInvoicePdfAsync(orderId, userId);
                return File(pdf, "application/pdf", $"invoice_{orderId}.pdf");
            }
            catch
            {
                return NotFound("Invoice generation failed.");
            }
        }


        [HttpDelete("{orderId}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                return NotFound(new { message = "Order not found or not authorized." });

            _context.OrderItems.RemoveRange(order.OrderItems); 
            _context.Orders.Remove(order);                     

            await _context.SaveChangesAsync();

            return Ok(new { message = "Order deleted successfully." });
        }











    }
}
