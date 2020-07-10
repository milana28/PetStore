using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using Dapper;
using Microsoft.EntityFrameworkCore;
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
        private List<Models.Pet> _pets = new List<Models.Pet>();
        private List<PetDAO> _petsDaoCategory = new List<PetDAO>();
        private List<PetDAO> _petsDaoTags = new List<PetDAO>();
        private readonly string databaseConnectionString = "Server=localhost;Database=petstore;User Id=sa;Password=yourStrong(!)Password;";
    
    public List<Models.Pet> GetAll()
       {
           using IDbConnection database = new SqlConnection(databaseConnectionString);
           
           _pets = database.Query<Models.Pet>("Select * From PetStore.Pet").ToList();
           _petsDaoCategory = database.Query<PetDAO>("Select * From PetStore.Pet as p Join PetStore.Category as c On c.categoryGuid = p.categoryGuid_fk").ToList();
           _petsDaoTags = database.Query<PetDAO>("Select * From PetStore.Pet as p Join PetStore.Tag as t On t.tagGuid = p.tagGuid_fk").ToList();
         
           AddCategory();

           AddTags();
        
         return _pets;
       }

    private void AddTags()
    {
        using IDbConnection database = new SqlConnection(databaseConnectionString);

        _petsDaoTags.ForEach(t =>
        {
            const string sqlForTag = "Select * From PetStore.Tag Where tagGuid = @tagGuid";
            var tag = database.QuerySingle<Tag>(sqlForTag, new {tagGuid = t.TagGuid});
            _pets.ForEach(v =>
            {
                if (!v.Guid.Equals(t.Guid)) return;
                v.Tags = new List<Tag>()
                {
                    new Tag()
                    {
                        TagGuid = tag.TagGuid,
                        TagName = tag.TagName
                    }
                };
            });
        });
    }
    
    private void AddCategory()
    {
        using IDbConnection database = new SqlConnection(databaseConnectionString);

        _petsDaoCategory.ForEach(p =>
        {
            const string sqlForCategory = "Select * From PetStore.Category Where categoryGuid = @guid";
            var category = database.QuerySingle<Category>(sqlForCategory, new {guid = p.CategoryGuid});
            _pets.ForEach(v =>
            {
                if (!v.Guid.Equals(p.Guid)) return;
                v.Category = category;
            });
        });
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
                    if (t.TagName == tag)
                    {
                        tags.Add(t);
                    }
                });
            });

            return tags;
        }

        public List<Models.Pet> GetPets(string? status)
        {
            return status == null ? GetAll() : GetPetByStatus(status);
        }
        
        public List<Models.Pet> GetPetsByTag(string? tag)
        {
            return tag == null ? GetAll() : GetPetByTag(tag);
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