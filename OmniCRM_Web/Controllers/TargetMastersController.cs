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
using static OmniCRM_Web.ViewModels.TargetMatrix;

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
        public async Task<ActionResult<IEnumerable<TargetMasterViewModel>>> GetTargetByMonth(string month)
        {
            try
            {
                DateTime selectedMonth = Convert.ToDateTime(month);
                selectedMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);

                Guid currentCompanyId = new Guid(User.Claims.FirstOrDefault(p => p.Type == "CompanyId").Value);

                var telecallerList = _context.UserMaster.Include(p => p.TargetMaster).Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true && r.CompanyId == currentCompanyId).AsEnumerable();

                List<TargetMasterViewModel> targetViewList = new List<TargetMasterViewModel>();
                foreach (var item in telecallerList)
                {
                    var objTarget = item.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth);
                    if (objTarget == null)
                        objTarget = new TargetMaster();
                    targetViewList.Add(new TargetMasterViewModel() { TagetId = objTarget.TagetId, TelecallerId = item.UserId, TelecallerName = item.FirstName, Target = objTarget.Target, MonthYear = objTarget.MonthYear });
                }

                return await Task.FromResult(targetViewList.OrderBy(p => p.TelecallerName).ToList());
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTargetByMonth: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("GetTargetMatrix/{month}")]
        public async Task<ActionResult<TargetMatrix>> GetTargetMatrix(string month)
        {
            try
            {

                DateTime selectedMonth = Convert.ToDateTime(month);
                selectedMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                int noOfWeek = GetWeekNumberOfMonth(selectedMonth);

                TargetMatrix targetMatrix = new TargetMatrix() { Header = new List<string>(), RowData = new List<RowsData>() };
                targetMatrix.Header.Add("Tele Caller");
                for (int i = 1; i <= noOfWeek; i++)
                    targetMatrix.Header.Add("Week " + i);

                Guid currentCompanyId = new Guid(User.Claims.FirstOrDefault(p => p.Type == "CompanyId").Value);
                var telecallerList = _context.UserMaster.Include(p => p.TargetMaster).Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true && r.CompanyId == currentCompanyId).AsEnumerable();

                foreach (var userMaster in telecallerList)
                {
                    targetMatrix.RowData.Add(new RowsData()
                    {
                        TCName = userMaster.FirstName,
                        Week1 = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth && p.WeekNumber == 1).Target,
                        Week2 = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth && p.WeekNumber == 2).Target,
                        Week3 = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth && p.WeekNumber == 3).Target,
                        Week4 = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth && p.WeekNumber == 4).Target,
                        Week5 = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth && p.WeekNumber == 5).Target,
                    });
                }

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetTargetMatrix: -get tele caller target matrix in admin");
                return await Task.FromResult(targetMatrix);
                //===================================================================================================
                //DateTime selectedMonth = Convert.ToDateTime(month);
                //selectedMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);

                //Guid currentCompanyId = new Guid(User.Claims.FirstOrDefault(p => p.Type == "CompanyId").Value);

                //var telecallerList = _context.UserMaster.Include(p => p.TargetMaster).Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true && r.CompanyId == currentCompanyId).AsEnumerable();

                //List<TargetMasterViewModel> targetViewList = new List<TargetMasterViewModel>();
                //foreach (var item in telecallerList)
                //{
                //    var objTarget = item.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth);
                //    if (objTarget == null)
                //        objTarget = new TargetMaster();
                //    targetViewList.Add(new TargetMasterViewModel() { TagetId = objTarget.TagetId, TelecallerId = item.UserId, TelecallerName = item.FirstName, Target = objTarget.Target, MonthYear = objTarget.MonthYear });
                //}

                //return await Task.FromResult(targetViewList.OrderBy(p => p.TelecallerName).ToList());
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTargetByMonth: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
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

        [HttpPost("PostTargetEntry/{month}")]
        public async Task<IActionResult> PostTargetEntry(string month, List<TargetMaster> collTargetEntry)
        {
            try
            {
                if (collTargetEntry.Count > 0)
                {
                    //_context.Entry(collTargetEntry).State = EntityState.Modified;

                    DateTime selectedMonth = Convert.ToDateTime(month);
                    selectedMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                    collTargetEntry.ForEach(p => p.MonthYear = selectedMonth);

                    //foreach (var item in collTargetEntry)
                    //{
                    //    if(_context.TargetMaster.Count(p=>p.it))
                    //}

                    _context.TargetMaster.UpdateRange(collTargetEntry);

                    await _context.SaveChangesAsync();

                    return Ok("Target Created successfully!");
                }
                else
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "PostTargetEntry: -target not exist");
                    return NotFound("Target not found!");
                }
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostTargetEntry: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

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

        private int GetWeekNumberOfMonth(DateTime date)
        {
            date = date.Date;
            DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            if (firstMonthMonday > date)
            {
                firstMonthDay = firstMonthDay.AddMonths(-1);
                firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            }
            return (date - firstMonthMonday).Days / 7 + 1;
        }
    }
}
