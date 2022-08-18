using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CountryApi.Models;
using CountryApi.Models.Database;

namespace CountryApi
{
    public class ModelsCountryApiContext : DbContext
    {
        public ModelsCountryApiContext (DbContextOptions<ModelsCountryApiContext> options)
            : base(options)
        {
        }

        public DbSet<CountryTable> Country { get; set; } = default!;
        public DbSet<CurrencyTable> Currency { get; set; } = default!;
    }
}
