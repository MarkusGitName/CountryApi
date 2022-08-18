using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CountryApi;
using CountryApi.Models;
using CountryApi.Models.Database;
using CountryApi.Models.Transfer;

namespace CountryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ModelsCountryApiContext _context;

        public CurrenciesController(ModelsCountryApiContext context)
        {
            _context = context;
        }

        // GET: api/Currencies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyTable>>> GetCurrency()
        {
          if (_context.Currency == null)
          {
              return NotFound();
          }
            return await _context.Currency.ToListAsync();
        }

        // GET: api/Currencies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CurrencyTable>> GetCurrency(Guid id)
        {
          if (_context.Currency == null)
          {
              return NotFound();
          }
            var currency = await _context.Currency.FindAsync(id);

            if (currency == null)
            {
                return NotFound();
            }

            return currency;
        }
        // GET: api/Currencies/5
        [HttpGet("CountriesByCurrency/{predicate}")]
        public async Task<ActionResult<CurrencyTransfer>> GetCountriesByCurrency(string predicate)
        {
          if (_context.Currency == null)
          {
              return NotFound();
          }
            try
            {
                var currency = await _context.Currency.IgnoreAutoIncludes().Include("Countries").FirstOrDefaultAsync(currency => currency.Id == Guid.Parse(predicate));
                if (currency == null)
                {
                    return NotFound();
                }

                return new CurrencyTransfer(currency);
            }
            catch
            {
                var currency = await _context.Currency.Include("Countries").FirstOrDefaultAsync(currency => currency.Name == predicate);
                if (currency == null)
                {
                    return NotFound();
                }

                return new CurrencyTransfer(currency);
            }

        }

        // PUT: api/Currencies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCurrency(Guid id, CurrencyTable currency)
        {
            if (id != currency.Id)
            {
                return BadRequest();
            }

            _context.Entry(currency).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyExists(id))
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

        // POST: api/Currencies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CurrencyTable>> PostCurrency(CurrencyTable currency)
        {
          if (_context.Currency == null)
          {
              return Problem("Entity set 'ModelsCountryApiContext.Currency'  is null.");
          }
            currency.Active = true;
            _context.Currency.Add(currency);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCurrency", new { id = currency.Id }, currency);
        }

        // DELETE: api/Currencies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCurrency(Guid id)
        {
            if (_context.Currency == null)
            {
                return NotFound();
            }
            var currency = await _context.Currency.FindAsync(id);
            if (currency == null)
            {
                return NotFound();
            }

            _context.Currency.Remove(currency);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CurrencyExists(Guid id)
        {
            return (_context.Currency?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
