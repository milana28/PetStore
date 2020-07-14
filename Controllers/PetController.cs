using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PetStore.Domain;
using PetStore.Models;

namespace PetStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PetController : ControllerBase
    {

        private readonly IPet _pet;
        private readonly ILogger<PetController> _logger;
        private readonly IMemoryCache _cache;
       
        public PetController(IPet pet, ILogger<PetController> logger, IMemoryCache cache)
        {
            _pet = pet;
            _logger = logger;
            _cache = cache;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.Pet> AddPet([FromForm] Models.Pet pet)
        {
            return _pet.SetPet(pet);
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Models.Pet>> GetPets([FromQuery(Name = "status")] string? status, [FromQuery(Name = "tags")]string? tag)
        {
            if (!_cache.TryGetValue("ListOfPets", out List<Models.Pet> pets))
            {
                _cache.Set("ListOfPets", pets, TimeSpan.FromSeconds(5));
            }
            else
            {
                return NoContent();
            }

            if (tag != null)
            {
                return _pet.GetPetsByTag(tag);
            }
                
            if (status != null && !Enum.IsDefined(typeof(PetStatuses), _pet.GetPetStatusInt(status)))
            {
                return BadRequest();
            }

            return _pet.GetPets(status);
        }

        [HttpGet("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Pet> GetPet(Guid guid)
        {
            var pet = _pet.GetPetByGuid(guid);
            if (pet == null)
            {
                return NotFound();
            }

            return pet;
        }

        [HttpPut("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Pet> UpdatePet(Guid guid, Models.Pet updatedPet)
        {
            try
            {
                var pet = _pet.GetPetByGuid(guid);
                if (updatedPet.Guid != guid)
                {
                    return BadRequest();
                }
                if (pet == null)
                {
                    return NotFound();
                }
                
                return _pet.UpdatePet(guid, updatedPet);
            }
            catch (Exception e)
            {
                // _logger.LogError("Error", e);
                // throw;
                return StatusCode(500, $"Internal server error: {e}");
            }
        }
        
        [HttpPost("{guid}/uploadImage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadPetImage(Guid guid, List<IFormFile> files)
        {
            var size = files.Sum(f => f.Length);
            var pet = _pet.GetPetByGuid(guid);
        
            foreach (var formFile in files)
            {
                var fileName = Path.GetRandomFileName();
                fileName = Path.ChangeExtension(fileName, ".jpg");
                var filePath = Path.Combine("wwwroot/images", fileName);

                if (formFile.Length <= 0) continue;
                await using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }
                pet.PhotoUrls = new List<string>()
                {
                    new string(fileName)
                };
                _pet.UpdatePet(guid, pet);
            }
           
            return Ok(new {count = files.Count, size});
        }

        [HttpDelete("{guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Pet> DeletePet(Guid guid)
        {
            var pet = _pet.GetPetByGuid(guid);
            if (pet == null)
            {
                return NotFound();
            }
            
            return _pet.DeletePet(pet.Guid);
        }
    }
    
}