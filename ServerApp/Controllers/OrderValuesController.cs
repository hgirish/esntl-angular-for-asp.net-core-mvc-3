using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace ServerApp.Controllers
{
    [Route("api/orders")]
    [Authorize(Roles ="Administrator")]
    [ApiController][AutoValidateAntiforgeryToken]
    public class OrderValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public OrderValuesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Order> GetOrders()
        {
            return _context.Orders
                .Include(o => o.Products)
                .Include(o => o.Payment);
        }

        [HttpPost("{id}")]
        public void MarkShipped(long id)
        {
            Order order = _context.Orders.Find(id);
            if (order != null)
            {
                order.Shipped = true;
                _context.SaveChanges();
            }
        }
        [HttpPost][AllowAnonymous]
        public IActionResult CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            order.OrderId = 0;
            order.Shipped = false;
            order.Payment.Total = GetPrice(order.Products);

            ProcessPayment(order.Payment);

            if(order.Payment.AuthCode != null)
            {
                _context.Add(order);
                _context.SaveChanges();
                return Ok(new
                {
                    orderID = order.OrderId,
                    authCode = order.Payment.AuthCode,
                    amount = order.Payment.Total
                });
            }
            return BadRequest("Payment rejected");
        }

        private decimal GetPrice(IEnumerable<CartLine> lines)
        {
            IEnumerable<long> ids = lines.Select(l => l.ProductId);
            IEnumerable<Product> products =
                _context.Products.Where(p => ids.Contains(p.ProductId));
            return products.Select(p => lines.First(l =>
            l.ProductId == p.ProductId).Quantity * p.Price)
                .Sum();
        }

        private void ProcessPayment(Payment payment)
        {
            // integrate your payment system here
            payment.AuthCode = "12345";
        }
    }
}
