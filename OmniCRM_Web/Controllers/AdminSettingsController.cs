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
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminSettingsController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public AdminSettingsController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/AdminSettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminSetting>>> GetAdminSetting()
        {
            return await _context.AdminSetting.ToListAsync();
        }

        // GET: api/AdminSettings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdminSetting>> GetAdminSetting(int id)
        {
            var adminSetting = await _context.AdminSetting.FindAsync(id);

            if (adminSetting == null)
            {
                return NotFound();
            }

            return adminSetting;
        }

        [HttpGet("GetAdminSettingLast")]
        public async Task<ActionResult<AdminSetting>> GetAdminSettingLast()
        {
            try
            {
                int lastSettingId = _context.AdminSetting.Max(p => p.SettingId);
                var adminSetting = await _context.AdminSetting.FindAsync(lastSettingId);

                if (adminSetting == null)
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "GetAdminSettingLast: " + "setting not exist.");
                    return NotFound("Current setting is not exist!");
                }

                return adminSetting;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetAdminSettingLast: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // PUT: api/AdminSettings/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdminSetting(int id, AdminSetting adminSetting)
        {
            if (id != adminSetting.SettingId)
            {
                return BadRequest();
            }

            _context.Entry(adminSetting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdminSettingExists(id))
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

        // POST: api/AdminSettings
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        [Obsolete]
        [Authorize(Roles = StringConstant.SuperUser + "," + StringConstant.Admin)]
        public async Task<ActionResult> PostAdminSetting(AdminSetting adminSetting)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    adminSetting.SettingId = 0;
                    adminSetting.CreatedDate = DateTime.Now;
                    //adminSetting.DailyEmailTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(adminSetting.DailyEmailTime), GenericMethods.Indian_Zone);
                    adminSetting.DailyEmailTime = TimeZone.CurrentTimeZone.ToLocalTime(Convert.ToDateTime(adminSetting.DailyEmailTime));
                    _context.AdminSetting.Add(adminSetting);
                    await _context.SaveChangesAsync();

                    GenericMethods.Log(LogType.ActivityLog.ToString(), "PostAdminSetting: " + adminSetting.CreatedBy + "-Setting created successfully");
                    return Ok("Settings saved successfully!");
                }
                else
                    return BadRequest("Failed to create settings!");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostAdminSetting: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // DELETE: api/AdminSettings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AdminSetting>> DeleteAdminSetting(int id)
        {
            var adminSetting = await _context.AdminSetting.FindAsync(id);
            if (adminSetting == null)
            {
                return NotFound();
            }

            _context.AdminSetting.Remove(adminSetting);
            await _context.SaveChangesAsync();

            return adminSetting;
        }

        private bool AdminSettingExists(int id)
        {
            return _context.AdminSetting.Any(e => e.SettingId == id);
        }
    }
}
