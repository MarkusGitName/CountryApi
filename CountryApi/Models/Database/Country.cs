using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryApi.Models.Database
{
    [Table("Country")]
    public class CountryTable : Table.Country
    {
        public CountryTable(Guid id,string name, string alpha2, string alpha3, string numeric,bool active) : base(id,name, alpha2, alpha3, numeric,active)
        {
            Id = id;
            Alpha2 = alpha2;
            Alpha3 = alpha3;
            Numeric = numeric;
            Active = active;
            Currencies = new HashSet<CurrencyTable>();
        }

        public ICollection<CurrencyTable>? Currencies { get; set; }
    }
}
