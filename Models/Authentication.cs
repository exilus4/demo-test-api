using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace demo_test_api.Models
{
    public class Authentication
    {
        [Key]
        public Guid uuid { get; set; }
        public string username { get; set; }

        public string password { get; set; }
    }

    public class AuthenticationDTO
    {
        public string username { get; set; }

        public string password { get; set; }
    }
}
