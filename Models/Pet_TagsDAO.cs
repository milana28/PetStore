using System;

namespace PetStore.Models
{
    public class Pet_TagsDAO
    {
        public int Id { get; set; }
        public Guid Pet_guid { get; set; }
        public Guid Tag_guid { get; set; }
    }
}