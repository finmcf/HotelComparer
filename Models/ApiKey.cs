using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelComparer.Models
{
    public class ApiKey
    {
        public int Id { get; set; }

        [Required, MaxLength(256)] // Ensuring the ClientId is not too long
        public string ClientId { get; set; }

        [Required, MaxLength(256)] // Ensuring the ClientSecret is not too long
        public string ClientSecret { get; set; }

        public int RequestCount { get; set; }  // Added property to keep track of request counts

        public List<Permission> Permissions { get; set; }
    }

    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
