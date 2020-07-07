using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
using PetStore.Controllers;
using PetStore.Models;

namespace PetStore.Domain
{
    public interface IPet
    {
        Models.Pet SetPet(Models.Pet pet);
        List<Models.Pet> GetAll();
        Models.Pet GetPetByGuid(Guid guid);
        List<Models.Pet> GetPets(string? status);
        List<Models.Pet> GetPetsByTag(string? tag);
        Models.Pet UpdatePet(Guid guid, Models.Pet pet);
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
            new Tag() {Name = "tag1"},
            new Tag() {Name = "tag2"},
            new Tag() {Name = "tag3"}
        };
        private static readonly List<Tag> TagsList2 = new List<Tag>()
        {
            new Tag() {Name = "tag4"},
        };

        private static readonly Category Category = new Category() {Name = "Category 1"};

       private readonly List<Models.Pet> _pets = new List<Models.Pet>()
        {
            new Models.Pet()
            {
                Category = Category,
                Name = "Dog",
                Status = PetStatuses.Available,
                Tags = TagsList,
            },
            new Models.Pet()
            {
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

        public Models.Pet GetPetByGuid(Guid guid)
        {
            return _pets.Find(pet => pet.Guid == guid);
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
            IEnumerable<string> splittedTags = tag.Split(",");
            List<Models.Pet> petList = new List<Models.Pet>();

            if (splittedTags.Count() == 1)
            {
                GetTags(tag).ForEach(t =>  petList.Add(_pets.Find(p => p.Tags.Contains(t))));
            }
            else
            {
                foreach (var splittedTag in splittedTags)
                {
                    GetTags(splittedTag).ForEach(t => petList.Add(_pets.Find(p => p.Tags.Contains(t))));
                }
            }

            return petList;
        }

        private List<Tag> GetTags(string tag)
        {
            List<Tag> tags = new List<Tag>();
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

            return tags;
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

        public Models.Pet UpdatePet(Guid guid, Models.Pet pet)
        {
            var index = _pets.FindIndex(p => p.Guid == guid);
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