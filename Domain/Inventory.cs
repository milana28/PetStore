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
        
        public Models.Inventory GetInventory()
        {
            var inventory = new Models.Inventory()
            {
                Available = _pet.GetAvailablePetsCount(),
                Pending = _pet.GetPendingPetsCount(),
                Sold = _pet.GetSoldPetsCount(),
            };

            return inventory;
        }
    }
}