using CountryApi.Models.Database;
using CountryApi.Models.Table;

namespace CountryApi.Models.Transfer
{
    public class CurrencyTransfer: Currency
    {

        public CurrencyTransfer(CurrencyTable currency) : base(currency.Id,currency.Name, currency.Active)
        {
            Name = currency.Name;
            Id = currency.Id;

            foreach (CountryTable country in currency.Countries)
            {
                Countries.Add(new Country(country.Id,country.Name,country.Alpha2,country.Alpha3,country.Numeric,country.Active));
            }

        }
        public List<Country>? Countries { get; set; }= new List<Country>(); 
    }
}
