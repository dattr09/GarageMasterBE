using GarageMasterBE.Models;
using GarageMasterBE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageMasterBE.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class OrderController : ControllerBase
  {
    private readonly OrderService _orderService;

    public OrderController(OrderService orderService)
    {
      _orderService = orderService;
    }

    // POST: api/Order
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] Order order)
    {
      if (order == null || order.Items == null || order.Items.Count == 0)
        return BadRequest("Đơn hàng không hợp lệ.");

      order.CreatedAt = DateTime.UtcNow;
      var created = await _orderService.CreateAsync(order);
      return Ok(created);
    }
  }
}