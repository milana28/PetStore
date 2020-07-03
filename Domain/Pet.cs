using System.Collections.Generic;
using PetStore.Models;

namespace PetStore.Domain
{
    public interface IPet
    {
        Models.Pet SetPet(Models.Pet pet);
        List<Models.Pet> GetAll();
        Models.Pet GetPetById(long id);
        List<Models.Pet> GetPets(string? status);
        List<Models.Pet> GetPetsByTag(string? tag);
        Models.Pet UpdatePet(long id, Models.Pet pet);
        Models.Pet DeletePet(Models.Pet pet);
        int GetPetStatusInt(string status); 
        List<Models.Pet> GetAvailablePets();
        List<Models.Pet> GetPendingPets();
        List<Models.Pet> GetSoldPets();
    }

    public class Pet : IPet
    {
        private static readonly List<Tag> TagsList = new List<Tag>()
        {
            new Tag() {Id = 0, Name = "tag1"},
            new Tag() {Id = 1, Name = "tag2"},
            new Tag() {Id = 2, Name = "tag3"}
        };
        private static readonly List<Tag> TagsList2 = new List<Tag>()
        {
            new Tag() {Id = 0, Name = "tag4"},
        };

        private static readonly Category Category = new Category() {Id = 0, Name = "Category 1"};

       private readonly List<Models.Pet> _pets = new List<Models.Pet>()
        {
            new Models.Pet()
            {
                Id = 0,
                Category = Category,
                Name = "Dog",
                Status = PetStatuses.Available,
                Tags = TagsList,
            },
            new Models.Pet()
            {
                Id = 1,
                Category = Category,
                Name = "Cat",
                Status = PetStatuses.Available,
                Tags = TagsList2,
            },
        };

       public List<Models.Pet> GetAll()
       {
           return _pets;
       }

    public Models.Pet SetPet(Models.Pet pet)
        {
            _pets.Add(pet);
            return pet;
        }

        public Models.Pet GetPetById(long id)
        {
            return _pets.Find(pet => pet.Id == id);
        }
        
        private List<Models.Pet> GetPetByStatus(string status)
        {
            return status switch
            {
                "available" => _pets.FindAll(p => p.Status == PetStatuses.Available),
                "pending" => _pets.FindAll(p => p.Status == PetStatuses.Pending),
                "sold" => _pets.FindAll(p => p.Status == PetStatuses.Sold),
                _ => null
            };
        }

        private List<Models.Pet> GetPetByTag(string tag)
        {
            List<Tag> tags = new List<Tag>();
            List<Models.Pet> petList = new List<Models.Pet>();
            
             _pets.ForEach(p =>
             {
                 p.Tags.ForEach(t =>
                 {
                     if (t.Name == tag)
                     {
                         tags.Add(t);
                     }
                 });
             });
             
             tags.ForEach(t =>  petList = _pets.FindAll(p => p.Tags.Contains(t)));

             return petList;
        }
        
        public List<Models.Pet> GetPets(string? status)
        {
            return status == null ? _pets : GetPetByStatus(status);
        }
        
        public List<Models.Pet> GetPetsByTag(string? tag)
        {
            return tag == null ? _pets : GetPetByTag(tag);
        }

        public Models.Pet DeletePet(Models.Pet pet)
        {
            _pets.Remove(pet);
            return pet;
        }

        public Models.Pet UpdatePet(long id, Models.Pet pet)
        {
            var index = _pets.FindIndex(p => p.Id == id);
            if (index != -1)
            { 
                _pets[index] = pet;
            }

            return _pets[index];
        }

        public int GetPetStatusInt(string status)
        {
            return status switch
            {
                "available" => 1,
                "pending" => 2,
                "sold" => 3,
                _ => 0
            };
        }

        public List<Models.Pet> GetAvailablePets()
        {
            var availablePets = _pets.FindAll(p => p.Status == PetStatuses.Available);
            return availablePets;
        }
        
        public List<Models.Pet> GetPendingPets()
        {
            var pendingPets = _pets.FindAll(p => p.Status == PetStatuses.Pending);
            return pendingPets;
        }
        
        public List<Models.Pet> GetSoldPets()
        {
            var soldPets = _pets.FindAll(p => p.Status == PetStatuses.Sold);
            return soldPets;
        }
    }
}