using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using PetStore.Models;

namespace PetStore.Domain
{
    public interface IPet
    {
        List<Models.Pet> GetPets(string? status);
        List<Models.Pet> GetPetsByTag(string? tag);
        Models.Pet SetPet(Models.Pet pet);
        Models.Pet GetPetByGuid(Guid guid);
        Models.Pet UpdatePet(Guid guid, Models.Pet pet);
        Models.Pet DeletePet(Guid guid);
        int GetPetStatusInt(string status);
        int GetAvailablePetsCount();
        int GetPendingPetsCount();
        int GetSoldPetsCount();
    }

    public class Pet : IPet
    {
        private readonly List<Models.Pet> _pets = new List<Models.Pet>();

        private readonly string databaseConnectionString =
            "Server=localhost;Database=petstore;User Id=sa;Password=yourStrong(!)Password;";

        public Models.Pet SetPet(Models.Pet pet)
        {
            using (DbConnection database = new SqlConnection(databaseConnectionString))
            {
                var petDao = new PetDAO()
                    {
                        Guid = Guid.NewGuid(),
                        Name = pet.Name, 
                        CategoryGuid = pet.Category.Guid,
                        PetStatus = pet.Status,
                    };
                    
                  
                    if (CheckIfCategoryExist(pet.Category.Guid, pet) == null)
                    {
                        return null;
                    }
                
                    const string insertQuery = "INSERT INTO PetStore.Pet VALUES (@guid, @name, @categoryGuid, @petStatus)";
                    database.Execute(insertQuery, petDao);
                    var newPet = TransformDaoToBusinessLogicPet(petDao);
                    var tags = CreateTags(pet.Tags, petDao.Guid);

                    if (tags == null)
                    {
                        return null;
                    }
                    
                    newPet.Tags = tags;
                    
                    return newPet;
            }
        }

        private List<Tag> CreateTags(List<Tag> tags, Guid petGuid)
        {
            var tagsList = new List<Tag>();
           
            foreach (var t in tags)
            {
                using IDbConnection database = new SqlConnection(databaseConnectionString);
                const string sql = "SELECT * FROM PetStore.Tag WHERE guid = @guid AND name = @name";
                var tag = database.QueryFirst<Tag>(sql, new { guid = t.Guid, name = t.Name }); 
                if (tag == null) continue;
                const string insertQuery = "INSERT INTO PetStore.Pet_tags VALUES (@pet_guid, @tag_guid)";
                database.Execute(insertQuery, new
                {
                    pet_guid = petGuid,
                    tag_guid = t.Guid
                });

                tagsList.Add(t);
            }

            return tagsList;
        }

        private List<Tag> GetTagsForPet(Guid petGuid)
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString); 
            const string petTags =
                    "SELECT * FROM PetStore.Pet_tags WHERE pet_guid = @pet_guid";
                
            var petTagsList =  database.Query<Pet_TagsDAO>(petTags, new {pet_guid = petGuid}).ToList();

            return petTagsList.Select(t => GetTagByGuid(t.Tag_guid)).ToList();
        }

        private Category CheckIfCategoryExist(Guid categoryGuid, Models.Pet pet)
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            var categories = database.Query<Category>("SELECT * FROM PetStore.Category").ToList();
            var category = GetCategoryByGuid(categoryGuid);
            var categoryList = categories.Where(c => c.Name == pet.Category.Name && c.Guid == pet.Category.Guid);
                
            return !categoryList.Any() ? null : category;
        }

        public Models.Pet GetPetByGuid(Guid guid)
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            const string sql = "SELECT * FROM PetStore.Pet WHERE guid = @guid";
            var petDao = database.QuerySingle<PetDAO>(sql, new {guid = guid});

            return TransformDaoToBusinessLogicPet(petDao);
        }

        private List<Models.Pet> GetAll()
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            var pets = database.Query<PetDAO>("SELECT * FROM PetStore.Pet").ToList();
            pets.ForEach(p => _pets.Add(TransformDaoToBusinessLogicPet(p)));
            
            return _pets;
        }
        
        private Models.Pet TransformDaoToBusinessLogicPet(PetDAO petDao)
        {
            var category = GetCategoryByGuid(petDao.CategoryGuid);
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            var tags = GetTagsForPet(petDao.Guid);

            return new Models.Pet
            {
                Guid = petDao.Guid,
                Name = petDao.Name,
                Category = category,
                Status = petDao.PetStatus,
                Tags = tags
            };
            
        }

        private Category GetCategoryByGuid(Guid guid)
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            const string sql = "SELECT * FROM PetStore.Category WHERE guid = @guid";
            
            return database.QueryFirst<Category>(sql, new {guid = guid});
        }

        private Tag GetTagByGuid(Guid guid)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Tag WHERE guid = @guid";
                return database.QuerySingle<Tag>(sql, new {guid = guid});
            }
        }

        private List<Tag> GetTagByName(string tag)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Tag WHERE name = @name";
                return database.Query<Tag>(sql, new {name = tag}).ToList();
            }
        }

        private List<Models.Pet> GetPetByStatus(string status)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                int statusInt = GetPetStatusInt(status);
                const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
                var pets = database.Query<PetDAO>(sql, new {status = statusInt}).ToList();
                var petsByStatus = new List<Models.Pet>();

                pets.ForEach(p => { petsByStatus.Add(TransformDaoToBusinessLogicPet(p)); });

                return petsByStatus;
            }
        }

        private List<Models.Pet> GetPetsByListOfTags(string tag)
        {

            IEnumerable<string> splittedTags = tag.Split(",");
            List<Models.Pet> petList = new List<Models.Pet>();

            if (splittedTags.Count() == 1)
            {
                GetPetsByTags(tag, petList);
            }
            else
            {
                foreach (var splittedTag in splittedTags)
                {
                    GetPetsByTags(splittedTag, petList);
                }
            }

            return petList;
        }

        private void GetPetsByTags(string tags, List<Models.Pet> petList)
        {
            GetTagByName(tags).ForEach(t =>
            {
                using IDbConnection database = new SqlConnection(databaseConnectionString);
                const string sql = 
                    "SELECT * FROM PetStore.Pet_tags WHERE tag_guid = @guid";
                var petGuidList = database.Query<Pet_TagsDAO>(sql, new {guid = t.Guid}).ToList();

                foreach (var p in petGuidList)
                {
                    const string pet = "SELECT * FROM PetStore.Pet WHERE guid = @guid";
                    var pets = database.Query<PetDAO>(pet, new {guid = p.Pet_guid}).ToList();
                    pets.ForEach(v => petList.Add(TransformDaoToBusinessLogicPet(v)));
                }
            });
        }

        public List<Models.Pet> GetPets(string? status)
        {
            return status == null ? GetAll() : GetPetByStatus(status);
        }

        public List<Models.Pet> GetPetsByTag(string? tag)
        {
            return tag == null ? GetAll() : GetPetsByListOfTags(tag);
        }

        public Models.Pet DeletePet(Guid guid)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                var petToDelete = GetPetByGuid(guid);
                const string sql = "DELETE FROM PetStore.Pet WHERE guid = @guid";
                database.ExecuteScalar<PetDAO>(sql, new {guid = guid});

                return petToDelete;
            }
        }

        public Models.Pet UpdatePet(Guid guid, Models.Pet pet)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                var tags = CreateTags(pet.Tags, guid);
                if (CheckIfCategoryExist(pet.Category.Guid, pet) == null || tags == null)
                {
                    return null;
                }

                const string sql =
                    "UPDATE PetStore.Pet SET name = @name, categoryGuid = @categoryGuid, petStatus = @status WHERE guid = @petGuid";
                database.Execute(sql, new {name = pet.Name, categoryGuid = pet.Category.Guid, status = pet.Status, petGuid = guid});
                
                return pet;
            }
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

        public int GetAvailablePetsCount()
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
            var availablePets = database.Query<PetDAO>(sql, new {status = PetStatuses.Available}).ToList();

            return availablePets.Count;
        }

        public int GetPendingPetsCount()
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
            var pendingPets = database.Query<PetDAO>(sql, new {status = PetStatuses.Pending}).ToList();

            return pendingPets.Count;
        }

        public int GetSoldPetsCount()
        {
            using IDbConnection database = new SqlConnection(databaseConnectionString);
            const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
            var soldPets = database.Query<PetDAO>(sql, new {status = PetStatuses.Sold}).ToList();

            return soldPets.Count;
        }
    }
}