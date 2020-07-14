using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
                        TagGuid = pet.Tags[0].Guid
                    };

                if (CheckIfCategoryExist(pet.Category.Guid, pet) == null || CheckIfTagExist(pet.Tags[0].Guid, pet) == null)
                {
                    return null;
                }
                
                // const string insertQuery = @"INSERT INTO [PetStore].[Pet]([guid], [name], [categoryGuid], [tagGuid], [petStatus])";
                const string insertQuery = "INSERT INTO PetStore.Pet VALUES (@guid, @name, @categoryGuid, @tagGuid, @petStatus)";
                database.Execute(insertQuery, petDao);

                return TransformDaoToBusinessLogicPet(petDao);
            }
        }

        private Category CheckIfCategoryExist(Guid categoryGuid, Models.Pet pet)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                var categories = database.Query<Category>("SELECT * FROM PetStore.Category").ToList();
                var category = GetCategoryByGuid(categoryGuid);
                var categoryList = categories.Where(c => c.Name == pet.Category.Name && c.Guid == pet.Category.Guid);
                
                return !categoryList.Any() ? null : category;
            }
        }

        private Tag CheckIfTagExist(Guid tagGuid, Models.Pet pet)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                var tags = database.Query<Tag>("SELECT * FROM PetStore.Tag").ToList();
                var tag = GetTagByGuid(tagGuid);
                // var tagList = tags.Select(el => pet.Tags.Where(t => t.Guid == el.Guid && t.Name == el.Name));
                var tagList = new List<Tag>();
                tags.ForEach(t =>
                {
                    if (t == pet.Tags[0])
                    {
                        tagList.Add(pet.Tags[0]);
                    }
                });

                return !tagList.Any() ? null : tag;
            }
        }
        
        public Models.Pet GetPetByGuid(Guid guid)
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Pet WHERE guid = @guid";
                var petDao = database.QuerySingle<PetDAO>(sql, new {guid = guid});

                return TransformDaoToBusinessLogicPet(petDao);
            }
        }

        private List<Models.Pet> GetAll()
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                var pets = database.Query<PetDAO>("SELECT * FROM PetStore.Pet").ToList();
                pets.ForEach(p => _pets.Add(TransformDaoToBusinessLogicPet(p)));
                return _pets;
            }
        }
        
        private Models.Pet TransformDaoToBusinessLogicPet(PetDAO petDao)
        {
            var category = GetCategoryByGuid(petDao.CategoryGuid);
            var tag = GetTagByGuid(petDao.TagGuid);
            var tags = new List<Tag> {tag};

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
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Category WHERE guid = @guid";
                return database.QueryFirst<Category>(sql, new {guid = guid});
            }
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
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                GetTagByName(tags).ForEach(t =>
                {
                    const string sql = "SELECT * FROM PetStore.Pet WHERE tagGuid = @guid";
                    var pets = database.Query<PetDAO>(sql, new {guid = t.Guid}).ToList();
                    pets.ForEach(p => petList.Add(TransformDaoToBusinessLogicPet(p)));
                });
            }
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
                if (CheckIfCategoryExist(pet.Category.Guid, pet) == null || CheckIfTagExist(pet.Tags[0].Guid, pet) == null)
                {
                    return null;
                }

                const string sql =
                    "UPDATE PetStore.Pet SET name = @name, petStatus = @status, categoryGuid = @categooryGuid, tagGuid = @tagGuid WHERE guid = @guid";
                // tags.ForEach(el => database.ExecuteScalar<PetDAO>(sql,
                //     new
                //     {
                //         name = pet.Name, guid = pet.Guid, status = pet.Status, categooryGuid = pet.Category.Guid,
                //         tagGuid = el.Guid
                //     }));

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
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
                var availablePets = database.Query<PetDAO>(sql, new {status = PetStatuses.Available}).ToList();

                return availablePets.Count;
            }
        }

        public int GetPendingPetsCount()
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
                var pendingPets = database.Query<PetDAO>(sql, new {status = PetStatuses.Pending}).ToList();

                return pendingPets.Count;
            }
        }

        public int GetSoldPetsCount()
        {
            using (IDbConnection database = new SqlConnection(databaseConnectionString))
            {
                const string sql = "SELECT * FROM PetStore.Pet WHERE petStatus = @status";
                var soldPets = database.Query<PetDAO>(sql, new {status = PetStatuses.Sold}).ToList();

                return soldPets.Count;
            }
        }
    }
}