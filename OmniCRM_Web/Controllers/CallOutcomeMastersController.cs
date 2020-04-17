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
    [Authorize(Roles = StringConstant.SuperUser + "," + StringConstant.TeleCaller)]

    public class CallOutcomeMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public CallOutcomeMastersController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/CallOutcomeMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CallOutcomeMaster>>> GetCallOutcomeMaster()
        {
            return await _context.CallOutcomeMaster.ToListAsync();
        }

        // GET: api/CallOutcomeMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CallOutcomeMaster>> GetCallOutcomeMaster(int id)
        {
            var callOutcomeMaster = await _context.CallOutcomeMaster.FindAsync(id);

            if (callOutcomeMaster == null)
            {
                return NotFound();
            }

            return callOutcomeMaster;
        }

        // PUT: api/CallOutcomeMasters/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCallOutcomeMaster(int id, CallOutcomeMaster callOutcomeMaster)
        {
            if (id != callOutcomeMaster.OutComeId)
            {
                return BadRequest();
            }

            _context.Entry(callOutcomeMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CallOutcomeMasterExists(id))
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

        // POST: api/CallOutcomeMasters
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<CallOutcomeMaster>> PostCallOutcomeMaster(CallOutcomeMaster callOutcomeMaster)
        {
            _context.CallOutcomeMaster.Add(callOutcomeMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCallOutcomeMaster", new { id = callOutcomeMaster.OutComeId }, callOutcomeMaster);
        }

        // DELETE: api/CallOutcomeMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CallOutcomeMaster>> DeleteCallOutcomeMaster(int id)
        {
            var callOutcomeMaster = await _context.CallOutcomeMaster.FindAsync(id);
            if (callOutcomeMaster == null)
            {
                return NotFound();
            }

            _context.CallOutcomeMaster.Remove(callOutcomeMaster);
            await _context.SaveChangesAsync();

            return callOutcomeMaster;
        }

        private bool CallOutcomeMasterExists(int id)
        {
            return _context.CallOutcomeMaster.Any(e => e.OutComeId == id);
        }
    }
}
