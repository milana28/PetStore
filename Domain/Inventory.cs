using System.Collections.Generic;

namespace PetStore.Domain
{
    public interface IInventory
    {
        Models.Inventory GetInventory();
    }

    public class Inventory : IInventory
    {
        private readonly IPet _pet;

        public Inventory(IPet pet)
        {
            _pet = pet;
        }

        private List<Models.Pet> GetAvailable()
        {
            return _pet.GetAvailablePets();
        }
        
        private List<Models.Pet> GetPending()
        {
            return _pet.GetPendingPets();
        }
        
        private List<Models.Pet> GetSold()
        {
            return _pet.GetSoldPets();
        }
        
        
        public Models.Inventory GetInventory()
        {
            var inventory = new Models.Inventory()
            {
                Available = GetAvailable().Count,
                Pending = GetPending().Count,
                Sold = GetSold().Count,
            };

            return inventory;
        }
    }
}