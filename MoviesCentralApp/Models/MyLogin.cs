using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MoviesCentralApp.Models
{
    public class MyLogin
    {
        [Key]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
