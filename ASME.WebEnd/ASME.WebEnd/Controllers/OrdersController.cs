using System;
using System.Linq;
using ASME.WebEnd.Entities;
using ASME.WebEnd.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASME.WebEnd.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderValidator _orderValidator;
        private readonly IOrderRouter _orderRouter;
        private readonly IOrderBook _orderBook;

        public OrdersController(IOrderValidator orderValidator, IOrderRouter orderRouter, IOrderBook orderBook)
        {
            _orderValidator = orderValidator;
            _orderRouter = orderRouter;
            _orderBook = orderBook;
        }

        [HttpGet]
        public ActionResult<Order[]> Get()
        {
            try
            {
                return Ok(_orderBook.GetActiveOrders());
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost]
        public ActionResult<OrderResult> Post([FromBody] Order order)
        {
            var validationErrors = _orderValidator.Validate(order).ToArray();
            if (validationErrors.Any()) return BadRequest(string.Join(Environment.NewLine, validationErrors.Select(ve => ve.ErrorMessage)));

            try
            {
                return Ok(_orderRouter.CreateOrder(order));
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public ActionResult<OrderResult> Put(string id, [FromBody] Order order)
        {
            var validationErrors = _orderValidator.Validate(order).ToArray();
            if (validationErrors.Any()) return BadRequest(string.Join(Environment.NewLine, validationErrors.Select(ve => ve.ErrorMessage)));

            try
            {
                if (!_orderRouter.CancelOrder(id)) return NotFound(id);
            
                return Ok(_orderRouter.CreateOrder(order));
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<string> Delete(string id)
        {
            try
            {
                if (!_orderRouter.CancelOrder(id)) return NotFound(id);

                return Ok(id);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
