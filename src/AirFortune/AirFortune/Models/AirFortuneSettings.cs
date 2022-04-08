using System.ComponentModel.DataAnnotations;

namespace AirFortune.Models
{
    public class AirFortuneSettings
    {
        [Required]
        public string? ApiKey { get; set; }
    }
}
