using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public PetController(IPet pet, ILogger<PetController> logger)
        {
            _pet = pet;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<Models.Pet> AddPet(Models.Pet pet)
        {
            return _pet.SetPet(pet);
        }
        
        [HttpGet]
        public ActionResult<List<Models.Pet>> GetPets([FromQuery(Name = "status")] string? status, [FromQuery(Name = "tags")]string? tag)
        {
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

        [HttpGet("{id}")]
        public ActionResult<Models.Pet> GetPet(long id)
        {
            var pet = _pet.GetPetById(id);
            if (pet == null)
            {
                return NotFound();
            }

            return pet;
        }

        [HttpPut("{id}")]
        public ActionResult<Models.Pet> UpdatePet(long id, Models.Pet updatedPet)
        {
            try
            {
                var pet = _pet.GetPetById(id);
                if (updatedPet.Id != id)
                {
                    return BadRequest();
                }
                if (pet == null)
                {
                    return NotFound();
                }
                
                return _pet.UpdatePet(id, updatedPet);
            }
            catch (Exception e)
            {
                _logger.LogError("Error", e);
                throw;
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Models.Pet> DeletePet(long id)
        {
            var pet = _pet.GetPetById(id);
            if (pet == null)
            {
                return NotFound();
            }
            
            return _pet.DeletePet(pet);
        }
    }
    
}