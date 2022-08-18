using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryApi.Models.Table
{
    public class Country
    {
        public Country(Guid id,string name,string alpha2,string alpha3,string numeric,bool active )
        {
            Id = id;
            Name = name;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            Numeric = numeric;
            Active = active;
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Name")]
        public string? Name { get; set; }
        [Display(Name = "Alpha2")]
        public string? Alpha2 { get; set; }
        [Display(Name = "Alpha3")]
        public string? Alpha3 { get; set; }
        [Display(Name = "Numeric")]
        public string? Numeric { get; set; }
        [Display(Name = "Active")]
        public bool Active{ get; set; }
    }
}
