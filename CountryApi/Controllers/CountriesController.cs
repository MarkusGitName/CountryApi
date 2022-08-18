using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CountryApi;
using CountryApi.Models.Database;
using CountryApi.Models;
using CountryApi.Models.Transfer;

namespace CountryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ModelsCountryApiContext _context;

        public CountriesController(ModelsCountryApiContext context)
        {
            _context = context;
        }

        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryTransfer>>> GetCountry()
        {
          if (_context.Country == null)
          {
              return NotFound();
          }
          List<CountryTransfer> countryList = new ();
          foreach(CountryTable country in  await _context.Country.Include(fields => fields.Currencies).ToListAsync())
          {
              countryList.Add(new(country));
          }
          return countryList;
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CountryTransfer>> GetCountry(Guid id)
        {
          if (_context.Country == null)
          {
              return NotFound();
          }
            var country = await _context.Country.Include(fields => fields.Currencies).FirstOrDefaultAsync(country => country.Id == id);

            if (country == null)
            {
                return NotFound();
            }

            return new CountryTransfer(country);
        }

        // GET: api/Countries/5
        [HttpGet("ByPredicate/{predicate}")]
        public async Task<ActionResult<IEnumerable<CountryTransfer>>> GetCountry(string predicate)
        {
          if (_context.Country == null)
          {
              return NotFound();
          }
            var country =  await _context.Country.Include(fields => fields.Currencies).Where(country => country.Alpha2 == predicate || country.Alpha3 == predicate|| country.Numeric== predicate).ToListAsync();

            if (country == null)
            {
                return NotFound();
            }

            List<CountryTransfer> countryList = new();
            foreach (CountryTable countryView in country)
            {
                countryList.Add(new(countryView));
            }
            return countryList;
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(Guid id, CountryTable country)
        {
            if (id != country.Id)
            {
                return BadRequest();
            }
            CountryTable existing = _context.Country.Include(country=>country.Currencies).FirstOrDefaultAsync(country=>country.Id == id).Result;
            existing.Currencies.Clear();
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            // _context.Entry(country).State = EntityState.Modified;
            _context.Attach(country);
    
            var entries = _context.ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                var type = entry.Entity.GetType().Name;
                if (type == "Country")
                {
                    entry.State = EntityState.Modified;
                }
                if(type == "Currency" && entry.State != EntityState.Added)
                {
                    entry.State = EntityState.Modified;
                }
                else if (type != "Country" && type != "Currency")
                {
                    entry.State = EntityState.Added;
                }
                //entry.State = EntityState.Modified;
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CountryTransfer>> PostCountry(CountryTable country)
        {
            if (_context.Country == null)
            {
                return Problem("Entity set 'ModelsCountryApiContext.Country'  is null.");
            }
            foreach (CurrencyTable currency in country.Currencies)
            {
                if (currency.Id != Guid.Empty)
                {
                    currency.Active = true;
                    _context.Entry(currency).State = EntityState.Modified;
                }
            }
            _context.Country.Add(country);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCountry", new { id = country.Id },new CountryTransfer(country));
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(Guid id)
        {
            if (_context.Country == null)
            {
                return NotFound();
            }
            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _context.Country.Remove(country);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        private bool CountryExists(Guid id)
        {
            return (_context.Country?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
