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
using OmniCRM_Web.ViewModels;
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StringConstant.SuperUser + "," + StringConstant.Admin)]
    public class TargetMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public TargetMastersController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/TargetMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TargetMaster>>> GetTargetMaster()
        {
            return await _context.TargetMaster.ToListAsync();
        }

        [HttpGet]
        [Route("GetTargetByMonth/{month}")]
        public List<TargetMasterViewModel> GetTargetByMonth(string month)
        {
            DateTime selectedMonth = Convert.ToDateTime(month);

            //var targetlist = from telecaller in _context.UserMaster.Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true)
            //                 join target in _context.TargetMaster.Where(p => p.MonthYear == selectedMonth) on telecaller.UserId equals target.TelecallerId into teleTarget
            //                 from target in teleTarget.DefaultIfEmpty(new TargetMaster())
            //                 select new { Telecaller = telecaller, Target = target };

            var telecallerList = _context.UserMaster.Include(p => p.TargetMaster).Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true).AsEnumerable();
            //var targetlist = _context.TargetMaster.Where(r => r.MonthYear == selectedMonth).AsEnumerable();

            List<TargetMasterViewModel> targetViewList = new List<TargetMasterViewModel>();
            foreach (var item in telecallerList)
            {
                var objTarget = item.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth);
                if (objTarget == null)
                    objTarget = new TargetMaster();
                targetViewList.Add(new TargetMasterViewModel() { TagetId = objTarget.TagetId, TelecallerId = item.UserId, TelecallerName = item.FirstName, Target = objTarget.Target, MonthYear = objTarget.MonthYear });
            }

            return targetViewList.OrderBy(p => p.TelecallerName).ToList();
        }

        // GET: api/TargetMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TargetMaster>> GetTargetMaster(int id)
        {
            var targetMaster = await _context.TargetMaster.FindAsync(id);

            if (targetMaster == null)
            {
                return NotFound();
            }

            return targetMaster;
        }

        // PUT: api/TargetMasters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTargetMaster(int id, TargetMaster targetMaster)
        {
            if (id != targetMaster.TagetId)
            {
                return BadRequest();
            }

            _context.Entry(targetMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TargetMasterExists(id))
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

        // POST: api/TargetMasters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<TargetMaster>> PostTargetMaster(TargetMaster targetMaster)
        {
            _context.TargetMaster.Add(targetMaster);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TargetMasterExists(targetMaster.TagetId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetTargetMaster", new { id = targetMaster.TagetId }, targetMaster);
        }

        // DELETE: api/TargetMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TargetMaster>> DeleteTargetMaster(int id)
        {
            var targetMaster = await _context.TargetMaster.FindAsync(id);
            if (targetMaster == null)
            {
                return NotFound();
            }

            _context.TargetMaster.Remove(targetMaster);
            await _context.SaveChangesAsync();

            return targetMaster;
        }

        private bool TargetMasterExists(int id)
        {
            return _context.TargetMaster.Any(e => e.TagetId == id);
        }
    }
}
