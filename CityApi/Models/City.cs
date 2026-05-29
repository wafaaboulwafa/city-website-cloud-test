using System.ComponentModel.DataAnnotations;

namespace CityApi.Models
{
    public class City
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
