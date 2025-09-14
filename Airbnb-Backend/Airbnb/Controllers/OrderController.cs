using Airbnb.DTOs;
using Airbnb.Models;
using Airbnb.Services;
using Microsoft.AspNetCore.Mvc;


namespace Airbnb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(OrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // GET: api/order?term=xyz&hostId=123
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] FilterOrderDto filter)
        {
            try
            {
                _logger.LogDebug("Getting orders with filter {@filter}", filter);
                var orders = await _orderService.QueryAsync(filter);
                var responseDtos = orders.Select(order => new OrderDto
                {
                    Id = order.Id,
                    Buyer = new UserDto { Id = order.Buyer.Id, Fullname = order.Buyer.Fullname },
                    Stay = new StayDto { Id = order.Stay.Id, Name = order.Stay.Name },
                    Host = new UserDto { Id = order.Host.Id, Fullname = order.Host.Fullname },
                    TotalPrice = order.TotalPrice,
                    StartDate = order.StartDate,
                    EndDate = order.EndDate,
                    Status = order.Status,
                    Guests = order.Guests != null ? new GuestDto
                    {
                        Adults = order.Guests.Adults,
                        Children = order.Guests.Children,
                        Infants = order.Guests.Infants,
                        Pets = order.Guests.Pets,
                    }: null
                }).ToList();
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get orders");
                return StatusCode(500, new { err = "Failed to get orders" });
            }
        }

        // POST: api/order
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderCreateDto dto)
        {
            if (dto == null) 
                return BadRequest(new { err = "Order data is required" });
            try
            {
                var order = new Order
                {
                    BuyerId= dto.Buyer.Id,
                    HostId = dto.Host.Id,
                    StayId = dto.Stay.Id,
                    TotalPrice = dto.TotalPrice,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = dto.Status
                };
                order.Guests = new Guest
                {
                    Adults = dto.Guests.Adults,
                    Children = dto.Guests.Children,
                    Infants = dto.Guests.Infants,
                    Pets = dto.Guests.Pets,
                    Order = order,
                };

                var addedOrder = await _orderService.AddAsync(order);

                var responseDto = new OrderDto
                {
                    Id = addedOrder.Id,
                    Buyer = dto.Buyer,
                    Stay = dto.Stay,
                    Host = dto.Host,
                    TotalPrice = addedOrder.TotalPrice,
                    StartDate = addedOrder.StartDate,
                    EndDate = addedOrder.EndDate,
                    Status = addedOrder.Status,
                    Guests = new GuestDto
                    {
                        Adults = dto.Guests.Adults,
                        Children = dto.Guests.Children,
                        Infants = dto.Guests.Infants,
                        Pets = dto.Guests.Pets
                    }
                };


                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add order");
                return StatusCode(500, new { err = "Failed to add order" });
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound(new { err = "Order not found" });
            var responseDto = new OrderDto
            {
                Id = order.Id,
                Buyer = new UserDto { Id = order.Buyer.Id, Fullname = order.Buyer.Fullname },
                Stay = new StayDto { Id = order.Stay.Id, Name = order.Stay.Name },
                Host = new UserDto { Id = order.Host.Id, Fullname = order.Host.Fullname },
                TotalPrice = order.TotalPrice,
                StartDate = order.StartDate,
                EndDate = order.EndDate,
                Status = order.Status
            };
            return Ok(responseDto);
        }


        //PUT: api/order
        [HttpPut]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderUpdateDto dto)
        {
            if (dto == null)
                return BadRequest(new { err = "Order data is required" });
            try
            {
                var UpdatedOrderDto = new Order
                {
                    Id = dto.Id,
                    BuyerId = dto.Buyer.Id,
                    StayId = dto.Stay.Id,
                    HostId = dto.Host.Id,
                    TotalPrice = dto.TotalPrice,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Status = dto.Status,
                    Guests = new Guest
                    {
                        Adults = dto.Guests.Adults,
                        Children = dto.Guests.Children,
                        Infants = dto.Guests.Infants,
                        Pets = dto.Guests.Pets
                    }
                };
                var updatedOrder = await _orderService.UpdateAsync(UpdatedOrderDto);

                var responseDto = new OrderDto
                {
                    Id = updatedOrder.Id,
                    Buyer = dto.Buyer,
                    Stay = dto.Stay,
                    Host = dto.Host,
                    TotalPrice = updatedOrder.TotalPrice,
                    StartDate = updatedOrder.StartDate,
                    EndDate = updatedOrder.EndDate,
                    Status = updatedOrder.Status,
                    Guests = new GuestDto
                    {
                        Adults = dto.Guests.Adults,
                        Children = dto.Guests.Children,
                        Infants = dto.Guests.Infants,
                        Pets = dto.Guests.Pets
                    }
                };
                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order");
                return StatusCode(500, new { err = "Failed to update order" });
            }
        }

    }
}
