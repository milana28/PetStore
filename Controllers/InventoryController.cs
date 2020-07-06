using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetStore.Domain;

namespace PetStore.Controllers
{
    [ApiController]
    [Route("store/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventory _inventory;

        public InventoryController(IInventory inventory)
        {
            _inventory = inventory;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Models.Inventory> GetInventory()
        {
            return _inventory.GetInventory();
        }

    }
}