using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProductsApi.Models; // For Item, Order, OrderDto, etc.
using ProductsApi.Models.Dtos; // For OrderDto
using Product.Models.Interfaces; // Correct namespace for IItemRepository
using ProductsApi.Models.Interfaces;

namespace ProductsApi.Controllers
{
    [Authorize]
    [Route("api/items")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _itemRepository;


        private readonly IOrderRepository _orderRepository;

        public ItemsController(IItemRepository itemRepository, IOrderRepository orderRepository)
        {
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
        }



        [HttpPost("order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDto orderRequest)
        {
            try
            {
                if (orderRequest == null || orderRequest.Quantity <= 0 || orderRequest.ItemId <= 0) // Changed ProductId to ItemId
                {
                    return BadRequest("Invalid order data.");
                }

                // Get the item by ID
                var item = await _itemRepository.GetItemByIdAsync(orderRequest.ItemId); // Changed ProductId to ItemId

                if (item == null)
                {
                    return NotFound($"Item with ID {orderRequest.ItemId} not found."); // Changed ProductId to ItemId
                }

                // Calculate the total price based on the item price and quantity
                decimal totalPrice = item.Price * orderRequest.Quantity;

                // Create the order
                var order = new Order
                {
                    ItemId = orderRequest.ItemId, // Changed ProductId to ItemId
                    Quantity = orderRequest.Quantity,
                    TotalPrice = totalPrice, // Calculate TotalPrice backend-side
                    Username = orderRequest.Username
                };

                // Save the order using OrderRepository
                var isCreated = await _orderRepository.CreateOrderAsync(order);

                if (!isCreated)
                {
                    return StatusCode(500, "An error occurred while placing the order.");
                }

                return Ok(new { Success = true, Message = "Order placed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred while placing the order." });
            }
        }


        [HttpGet("get")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetItems()
        {
            try
            {
                var items = await _itemRepository.GetAllItemsAsync();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the items.");
            }
        }

        [HttpGet("get/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetItemById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ID.");
                }

                var item = await _itemRepository.GetItemByIdAsync(id);

                if (item == null)
                {
                    return NotFound($"Item with ID {id} not found.");
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the item.");
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateItem(Item item)
        {
            try
            {
                if (item == null)
                {
                    return BadRequest("Invalid item data.");
                }

                // Validate that Price is not negative
                if (item.Price < 0)
                {
                    return BadRequest("Price cannot be negative.");
                }

                var isCreated = await _itemRepository.CreateItemAsync(item);
                if (!isCreated)
                {
                    return StatusCode(500, "An error occurred while creating the item.");
                }

                return Ok(new { Success = true, Message = "Item created successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the item.");
            }
        }


        [HttpPut("update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateItem(int id, Item item)
        {
            try
            {
                if (item == null || id <= 0)
                {
                    return BadRequest("Invalid item data.");
                }

                // Validate that Price is not negative
                if (item.Price < 0)
                {
                    return BadRequest("Price cannot be negative.");
                }

                item.Id = id;
                var isUpdated = await _itemRepository.UpdateItemAsync(item);

                if (!isUpdated)
                {
                    return NotFound($"Item with ID {id} not found.");
                }

                return Ok(new { Success = true, Message = "Item updated successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("asdasdasd");
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred while updating the item." });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("Invalid ID.");
                }

                var isDeleted = await _itemRepository.DeleteItemAsync(id);
                if (!isDeleted)
                {
                    return NotFound($"Item with ID {id} not found.");
                }

                return Ok(new { Success = true, Message = "Item deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An unexpected error occurred. Please try again later." });
            }
        }

        [HttpGet("orders")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = "An error occurred while fetching the orders." });
            }
        }


    }
}
