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
    [Authorize(Roles = StringConstant.SuperUser + "," + StringConstant.RelationshipManager + "," + StringConstant.Admin)]
    public class AppoinmentStatusMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public AppoinmentStatusMastersController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/AppoinmentStatusMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppoinmentStatusMaster>>> GetAppoinmentStatusMaster()
        {
            return await _context.AppoinmentStatusMaster.ToListAsync();
        }

        // GET: api/AppoinmentStatusMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppoinmentStatusMaster>> GetAppoinmentStatusMaster(int id)
        {
            var appoinmentStatusMaster = await _context.AppoinmentStatusMaster.FindAsync(id);

            if (appoinmentStatusMaster == null)
            {
                return NotFound();
            }

            return appoinmentStatusMaster;
        }

        // PUT: api/AppoinmentStatusMasters/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppoinmentStatusMaster(int id, AppoinmentStatusMaster appoinmentStatusMaster)
        {
            if (id != appoinmentStatusMaster.AppoinStatusId)
            {
                return BadRequest();
            }

            _context.Entry(appoinmentStatusMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppoinmentStatusMasterExists(id))
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

        // POST: api/AppoinmentStatusMasters
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<AppoinmentStatusMaster>> PostAppoinmentStatusMaster(AppoinmentStatusMaster appoinmentStatusMaster)
        {
            _context.AppoinmentStatusMaster.Add(appoinmentStatusMaster);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppoinmentStatusMaster", new { id = appoinmentStatusMaster.AppoinStatusId }, appoinmentStatusMaster);
        }

        // DELETE: api/AppoinmentStatusMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AppoinmentStatusMaster>> DeleteAppoinmentStatusMaster(int id)
        {
            var appoinmentStatusMaster = await _context.AppoinmentStatusMaster.FindAsync(id);
            if (appoinmentStatusMaster == null)
            {
                return NotFound();
            }

            _context.AppoinmentStatusMaster.Remove(appoinmentStatusMaster);
            await _context.SaveChangesAsync();

            return appoinmentStatusMaster;
        }

        private bool AppoinmentStatusMasterExists(int id)
        {
            return _context.AppoinmentStatusMaster.Any(e => e.AppoinStatusId == id);
        }
    }
}
