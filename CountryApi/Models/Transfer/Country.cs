using CountryApi.Models.Database;
using CountryApi.Models.Table;

namespace CountryApi.Models.Transfer
{
    public class CountryTransfer : Country
    {
        public CountryTransfer(CountryTable country) : base(country.Id,country.Name, country.Alpha2, country.Alpha3, country.Numeric,country.Active)
        {
            Id= country.Id;
            Name = country.Name;
            Alpha2 = country.Alpha2;
            Alpha3 = country.Alpha3;
            Numeric = country.Numeric;
            Active = country.Active;
            foreach(CurrencyTable currency in country.Currencies)
            {
                Currencies.Add(new Currency(currency.Id,currency.Name, currency.Active));
            }
        }
        public List<Currency>? Currencies { get; set; } = new List<Currency>();
    }
}
