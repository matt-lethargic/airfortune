using System.ComponentModel.DataAnnotations;

namespace AirFortune.Models
{
    public class AddTable
    {
        [Required]
        public string BaseId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string FirstNameField { get; set; }

        [Required]
        public string LastNameField { get; set; }
    }
}
