using Microsoft.AspNetCore.Identity;
using System.Numerics;

namespace Medical.PL.Data.Models
{
    public class User : IdentityUser<int>
    {
        
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
