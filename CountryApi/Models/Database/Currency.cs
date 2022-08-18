using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CountryApi.Models.Database
{
    public class CurrencyTable : Table.Currency
    {
        public CurrencyTable(Guid id,string name, bool active) : base(id,name,active)
        {
            Id = id;
            Name = name;
            Active = active;    
            Countries = new HashSet<CountryTable>();
        }
        public ICollection<CountryTable>? Countries { get; set; }
    }
}
