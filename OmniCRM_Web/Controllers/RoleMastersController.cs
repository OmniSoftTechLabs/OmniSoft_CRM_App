using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmniCRM_Web.Models;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class RoleMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public RoleMastersController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/RoleMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleMaster>>> GetRoleMaster()
        {
            return await _context.RoleMaster.ToListAsync();
        }

        // GET: api/RoleMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleMaster>> GetRoleMaster(int id)
        {
            var roleMaster = await _context.RoleMaster.FindAsync(id);

            if (roleMaster == null)
            {
                return NotFound();
            }

            return roleMaster;
        }

        // PUT: api/RoleMasters/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoleMaster(int id, RoleMaster roleMaster)
        {
            if (id != roleMaster.RoleId)
            {
                return BadRequest();
            }

            _context.Entry(roleMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleMasterExists(id))
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

        // POST: api/RoleMasters
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<RoleMaster>> PostRoleMaster(RoleMaster roleMaster)
        {
            _context.RoleMaster.Add(roleMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoleMaster", new { id = roleMaster.RoleId }, roleMaster);
        }

        // DELETE: api/RoleMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RoleMaster>> DeleteRoleMaster(int id)
        {
            var roleMaster = await _context.RoleMaster.FindAsync(id);
            if (roleMaster == null)
            {
                return NotFound();
            }

            _context.RoleMaster.Remove(roleMaster);
            await _context.SaveChangesAsync();

            return roleMaster;
        }

        private bool RoleMasterExists(int id)
        {
            return _context.RoleMaster.Any(e => e.RoleId == id);
        }
    }
}
