using BookStore.Api.Models;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BookStore.Api.Areas.Admin.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Orders> _orderRepository;
        private readonly IRepository<OrdersItem> _ordersItemRepository;

        public OrdersController(IRepository<Orders> orderRepository , IRepository<OrdersItem> ordersItemRepository)
        {
            _orderRepository = orderRepository;
            _ordersItemRepository = ordersItemRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var orders =await _orderRepository.GetAsync(includes: [e => e.Items], cancellationToken: cancellationToken, tracked: false);

            return Ok(orders);
        }
        [HttpGet("{id}")]
        public IActionResult GetOne(int id , CancellationToken cancellationToken)
        {
            var order = _orderRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken, includes: [e => e.Items], tracked: false);

            if(order is null)
                return NotFound( new
                {
                    msg =" Order Not Found"
                });

            return Ok(order);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest createOrderRequest , CancellationToken cancellationToken)
        {
            Orders order = createOrderRequest.Adapt<Orders>();

           
           await _orderRepository.AddAsync(order, cancellationToken);
           await _orderRepository.CommitAsync(cancellationToken);


            return CreatedAtAction(nameof(GetOne), new { id = order.Id }, new
            {
                success_notifaction = "Create Category Successfully"
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var order =await _orderRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (order is null)
                return NotFound(new
                {
                    msg = "Order not found"
                });
            _orderRepository.Update(order);
            await _orderRepository.CommitAsync(cancellationToken);

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken)
        {
            var order =await _orderRepository.GetOneAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if (order is null)
                return NotFound(new
                {
                    msg = "Order not found"
                });

            _orderRepository.Delete(order);
           await _orderRepository.CommitAsync(cancellationToken);

            return NoContent();
        }
    }
}
