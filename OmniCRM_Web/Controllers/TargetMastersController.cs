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
                    targetViewList.Add(new TargetMasterViewModel() { TagetId = objTarget.TagetId, TelecallerId = item.UserId, TelecallerName = item.FirstName, TargetWeek1 = objTarget.TargetWeek1, MonthYear = objTarget.MonthYear });
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

                var ListofWeeks = GetWeekRange.GetListofWeeks(selectedMonth.Year, selectedMonth.Month);

                //int noOfWeek = GetWeekNumberOfMonth(selectedMonth);

                TargetMatrix targetMatrix = new TargetMatrix() { Header = new List<string>(), RowDataTargetMaster = new List<TargetMasterViewModel>() };
                targetMatrix.Header.Add("Tele Caller");
                foreach (dynamic item in ListofWeeks)
                {
                    DateTime FromDate = Convert.ToDateTime(item.DateFrom);
                    DateTime ToDate = Convert.ToDateTime(item.To);
                    targetMatrix.Header.Add(FromDate.Day.ToString() + " - " + ToDate.Day.ToString());
                }
                //for (int i = 1; i <= ListofWeeks.Count(); i++)
                //    targetMatrix.Header.Add("Week " + i);

                Guid currentCompanyId = new Guid(User.Claims.FirstOrDefault(p => p.Type == "CompanyId").Value);
                var telecallerList = _context.UserMaster.Include(p => p.TargetMaster).Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true && r.CompanyId == currentCompanyId).AsEnumerable();

                foreach (var userMaster in telecallerList)
                {
                    var objTarget = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth);
                    if (objTarget == null)
                        objTarget = new TargetMaster();

                    targetMatrix.RowDataTargetMaster.Add(new TargetMasterViewModel()
                    {
                        TagetId = objTarget.TagetId,
                        TelecallerName = userMaster.FirstName,
                        TelecallerId = userMaster.UserId,
                        TargetWeek1 = objTarget.TargetWeek1,
                        TargetWeek2 = objTarget.TargetWeek2,
                        TargetWeek3 = objTarget.TargetWeek3,
                        TargetWeek4 = objTarget.TargetWeek4,
                        TargetWeek5 = objTarget.TargetWeek5,
                        TargetWeek6 = objTarget.TargetWeek6,
                    });
                }

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetTargetMatrix: -get tele caller target matrix in admin");
                return await Task.FromResult(targetMatrix);
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTargetByMonth: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("PostTargetMatrix/{month}")]
        public async Task<IActionResult> PostTargetMatrix(string month, TargetMatrix targetMatrix)
        {
            try
            {
                if (targetMatrix.RowDataTargetMaster.Count > 0)
                {
                    DateTime selectedMonth = Convert.ToDateTime(month);
                    selectedMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);
                    int noOfWeek = GetWeekNumberOfMonth(selectedMonth);

                    List<TargetMaster> collTargetEntry = new List<TargetMaster>();
                    foreach (var item in targetMatrix.RowDataTargetMaster)
                    {
                        TargetMaster objTarget = new TargetMaster()
                        {
                            TagetId = item.TagetId,
                            TelecallerId = item.TelecallerId,
                            MonthYear = selectedMonth,
                            TargetWeek1 = item.TargetWeek1,
                            TargetWeek2 = item.TargetWeek2,
                            TargetWeek3 = item.TargetWeek3,
                            TargetWeek4 = item.TargetWeek4,
                            TargetWeek5 = item.TargetWeek5,
                            TargetWeek6 = item.TargetWeek6,
                        };
                        collTargetEntry.Add(objTarget);
                    }

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

        [HttpGet]
        [Route("GetTargetVsAchieve/{month}")]
        public async Task<ActionResult<TargetMatrix>> GetTargetVsAchieve(string month)
        {
            try
            {

                DateTime selectedMonth = Convert.ToDateTime(month);
                selectedMonth = new DateTime(selectedMonth.Year, selectedMonth.Month, 1);

                var ListofWeeks = GetWeekRange.GetListofWeeks(selectedMonth.Year, selectedMonth.Month);
                TargetMatrix targetVsAchieve = new TargetMatrix() { Header = new List<string>(), RowDataTargetMaster = new List<TargetMasterViewModel>() };
                targetVsAchieve.Header.Add("Tele Caller");
                foreach (dynamic item in ListofWeeks)
                {
                    DateTime FromDate = Convert.ToDateTime(item.DateFrom);
                    DateTime ToDate = Convert.ToDateTime(item.To);
                    targetVsAchieve.Header.Add(FromDate.Day.ToString() + " - " + ToDate.Day.ToString());
                }
                targetVsAchieve.Header.Add("Total");

                Guid currentCompanyId = new Guid(User.Claims.FirstOrDefault(p => p.Type == "CompanyId").Value);
                var telecallerList = _context.UserMaster.Include(p => p.TargetMaster).Where(r => r.RoleId == (int)Roles.TeleCaller && r.Status == true && r.CompanyId == currentCompanyId).AsNoTracking().AsEnumerable();

                foreach (var userMaster in telecallerList)
                {
                    var objTarget = userMaster.TargetMaster.FirstOrDefault(p => p.MonthYear == selectedMonth);
                    if (objTarget == null)
                        objTarget = new TargetMaster();

                    targetVsAchieve.RowDataTargetMaster.Add(new TargetMasterViewModel()
                    {
                        TagetId = objTarget.TagetId,
                        TelecallerName = userMaster.FirstName,
                        TelecallerId = userMaster.UserId,
                        TargetWeek1 = objTarget.TargetWeek1,
                        TargetWeek2 = objTarget.TargetWeek2,
                        TargetWeek3 = objTarget.TargetWeek3,
                        TargetWeek4 = objTarget.TargetWeek4,
                        TargetWeek5 = objTarget.TargetWeek5,
                        TargetWeek6 = objTarget.TargetWeek6,
                    });

                    int cntWeek = 0;
                    foreach (dynamic item in ListofWeeks)
                    {
                        cntWeek++;
                        DateTime FromDate = Convert.ToDateTime(item.DateFrom);
                        DateTime ToDate = Convert.ToDateTime(item.To);

                        //OmniCRMContext con_text = new OmniCRMContext();
                        var TeleCallerLeads = _context.CallTransactionDetail.AsEnumerable().Where(q => Convert.ToDateTime(q.CreatedDate).Date >= FromDate.Date && Convert.ToDateTime(q.CreatedDate).Date <= ToDate.Date
                                             && q.OutComeId != (int)Enums.CallOutcome.NoResponse
                                             && q.OutComeId != (int)Enums.CallOutcome.None
                                             && q.OutComeId != (int)Enums.CallOutcome.Dropped
                                             && q.OutComeId != (int)Enums.CallOutcome.Interested
                                             && q.CreatedBy == userMaster.UserId).GroupBy(x => x.CallId).Select(r => r.OrderBy(a => a.CallTransactionId).LastOrDefault());

                        var objAchive = targetVsAchieve.RowDataTargetMaster.FirstOrDefault(p => p.TelecallerId == userMaster.UserId);
                        switch (cntWeek)
                        {
                            case 1:
                                objAchive.AchieveWeek1 = TeleCallerLeads.Count();
                                break;
                            case 2:
                                objAchive.AchieveWeek2 = TeleCallerLeads.Count();
                                break;
                            case 3:
                                objAchive.AchieveWeek3 = TeleCallerLeads.Count();
                                break;
                            case 4:
                                objAchive.AchieveWeek4 = TeleCallerLeads.Count();
                                break;
                            case 5:
                                objAchive.AchieveWeek5 = TeleCallerLeads.Count();
                                break;
                            case 6:
                                objAchive.AchieveWeek6 = TeleCallerLeads.Count();
                                break;
                            default:
                                break;
                        }
                    }
                }

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetTargetVsAchieve: -get tele caller target vs achievement in admin");
                return await Task.FromResult(targetVsAchieve);
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTargetByMonth: " + ex.ToString());
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
            //date = date.Date;
            //DateTime firstMonthDay = new DateTime(date.Year, date.Month, 1);
            //DateTime firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            //if (firstMonthMonday > date)
            //{
            //    firstMonthDay = firstMonthDay.AddMonths(-1);
            //    firstMonthMonday = firstMonthDay.AddDays((DayOfWeek.Monday + 7 - firstMonthDay.DayOfWeek) % 7);
            //}
            //return (date - firstMonthMonday).Days / 7 + 1;


            //extract the month
            //int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
            //DateTime firstOfMonth = new DateTime(date.Year, date.Month, 1);
            ////days of week starts by default as Sunday = 0
            //int firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
            //int weeksInMonth = (int)Math.Ceiling(daysInMonth / 7.0);
            ////int weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);
            //return weeksInMonth;

            int countDays = DateTime.DaysInMonth(date.Year, date.Month);
            int numOfWeeks = 0;
            for (int i = 1; i <= countDays; i = i + 7)
                numOfWeeks++;
            return numOfWeeks;
            //zero-based array

            //DateTime first = new DateTime(date.Year, date.Month, 1);

            //int firstwkday = (int)first.DayOfWeek;
            ////int otherwkday = (int)wkstart;

            //int offset = (8 - firstwkday) % 7;
            //double weeks = (double)(DateTime.DaysInMonth(date.Year, date.Month) - offset) / 7d;
            //return (int)Math.Ceiling(weeks);
        }
    }
}
