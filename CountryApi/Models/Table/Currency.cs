using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryApi.Models.Table
{
    public class Currency
    {
        public Currency(Guid id, string name, bool active)
        {
            Id = id;
            Name = name;
            Active = active;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        [Display(Name = "Active")]
        public bool Active { get; set; }
    }
}
