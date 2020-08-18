using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmniCRM_Web.GenericClasses;
using OmniCRM_Web.Models;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StringConstant.TeleCaller + "," + StringConstant.RelationshipManager + "," + StringConstant.Admin)]

    public class StateMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public StateMastersController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/StateMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StateMaster>>> GetStateMaster()
        {
            return await _context.StateMaster.ToListAsync();
        }

        [HttpGet("GetStateMasterByName/{term}")]
        public async Task<ActionResult<IEnumerable<StateMaster>>> GetStateMasterByName(string term)
        {
            return await _context.StateMaster.Where(p => p.StateName.ToLower().StartsWith(term.ToLower())).ToListAsync();
        }

        [HttpGet("GetCityMaster/{id}/{term}")]
        public async Task<ActionResult<IEnumerable<CityMaster>>> GetCityMaster(int id, string term)
        {
            return await _context.CityMaster.Where(p => p.StateId == id && p.CityName.ToLower().StartsWith(term.ToLower())).ToListAsync();
        }

        // GET: api/StateMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StateMaster>> GetStateMaster(int id)
        {
            var stateMaster = await _context.StateMaster.FindAsync(id);

            if (stateMaster == null)
            {
                return NotFound();
            }

            return stateMaster;
        }

        [HttpGet("GetCityMaster/{id}")]
        public async Task<ActionResult<CityMaster>> GetCityMaster(int id)
        {
            var cityMaster = await _context.CityMaster.FindAsync(id);

            if (cityMaster == null)
            {
                return NotFound();
            }

            return cityMaster;
        }

        // PUT: api/StateMasters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStateMaster(int id, StateMaster stateMaster)
        {
            if (id != stateMaster.StateId)
            {
                return BadRequest();
            }

            _context.Entry(stateMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StateMasterExists(id))
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

        // POST: api/StateMasters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<StateMaster>> PostStateMaster(StateMaster stateMaster)
        {
            _context.StateMaster.Add(stateMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStateMaster", new { id = stateMaster.StateId }, stateMaster);
        }

        // DELETE: api/StateMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<StateMaster>> DeleteStateMaster(int id)
        {
            var stateMaster = await _context.StateMaster.FindAsync(id);
            if (stateMaster == null)
            {
                return NotFound();
            }

            _context.StateMaster.Remove(stateMaster);
            await _context.SaveChangesAsync();

            return stateMaster;
        }

        private bool StateMasterExists(int id)
        {
            return _context.StateMaster.Any(e => e.StateId == id);
        }
    }
}
