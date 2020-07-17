using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace PetStore.Models
{
    public class PhotosDAO
    {
        public int Id { get; set; }
        public Guid Pet_guid { get; set; }
        public string Url { get; set; }
    }
}