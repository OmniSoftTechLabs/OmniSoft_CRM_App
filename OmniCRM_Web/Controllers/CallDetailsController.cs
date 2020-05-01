using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OmniCRM_Web.GenericClasses;
using OmniCRM_Web.Models;
using OmniCRM_Web.ViewModels;
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StringConstant.TeleCaller + "," + StringConstant.RelationshipManager)]

    public class CallDetailsController : ControllerBase
    {
        private readonly OmniCRMContext _context;
        private readonly IMapper _mapper;

        public CallDetailsController(OmniCRMContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

        }

        // GET: api/CallDetails
        [HttpPost("GetCallDetailByCreatedBy/{id}")]
        public async Task<ActionResult<IEnumerable<CallDetailViewModel>>> GetCallDetailByCreatedBy(Guid id, [FromBody] FilterOptions filterOption)
        {
            try
            {
                filterOption.FromDate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.FromDate, GenericMethods.Indian_Zone);
                filterOption.Todate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.Todate, GenericMethods.Indian_Zone);

                //var callDetail = await _context.CallDetail.Include(p => p.OutCome).Include(p => p.AppointmentDetail).Where(p => p.CreatedBy == id).ToListAsync();
                //var listLead = _mapper.Map<List<CallDetailViewModel>>(callDetail);
                //List<CallDetailViewModel> listCallDetail = new List<CallDetailViewModel>();

                var listCallDetail = (from lead in await _context.CallDetail.Include(p => p.OutCome).Include(p => p.AppointmentDetail).Where(p => p.CreatedBy == id).ToListAsync()
                                      select lead).Select(p => new CallDetailViewModel()
                                      {
                                          CallId = p.CallId,
                                          CreatedDate = p.CreatedDate,
                                          FirstName = p.FirstName,
                                          LastName = p.LastName,
                                          MobileNumber = p.MobileNumber,
                                          FirmName = p.FirmName,
                                          Address = p.Address,
                                          LastChangedDate = p.LastChangedDate,
                                          OutComeId = p.OutComeId,
                                          Remark = p.Remark,
                                          CreatedByName = _context.UserMaster.FirstOrDefault(r => r.UserId == p.CreatedBy).FirstName,
                                          OutComeText = p.OutCome.OutCome,
                                          AllocatedToId = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId : Guid.Empty,
                                          AllocatedToName = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? _context.UserMaster.AsEnumerable().FirstOrDefault(r => r.UserId == p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId).FirstName : "",
                                          AppointmentDateTime = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppointmentDateTime : (DateTime?)null,

                                      }).ToList();


                if (filterOption.DateFilterBy == 1)
                    listCallDetail = listCallDetail.Where(p => p.CreatedDate.Date >= filterOption.FromDate.Date && p.CreatedDate.Date <= filterOption.Todate.Date).ToList();
                else if (filterOption.DateFilterBy == 2)
                    listCallDetail = listCallDetail.Where(p => Convert.ToDateTime(p.AppointmentDateTime).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.AppointmentDateTime).Date <= filterOption.Todate.Date).ToList();

                if (filterOption.AllocatedTo != "0")
                    listCallDetail = listCallDetail.Where(p => p.AllocatedToId.ToString() == filterOption.AllocatedTo).ToList();

                if (filterOption.Status > 0)
                    listCallDetail = listCallDetail.Where(p => p.OutComeId == filterOption.Status).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetail: " + id + "-get all Lead by created user");
                return await Task.FromResult(listCallDetail);

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetCallDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost("GetCallDetailByRM/{id}")]
        public async Task<ActionResult<IEnumerable<CallDetailViewModel>>> GetCallDetailByRM(Guid id, [FromBody] FilterOptions filterOption)
        {
            try
            {
                filterOption.FromDate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.FromDate, GenericMethods.Indian_Zone);
                filterOption.Todate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.Todate, GenericMethods.Indian_Zone);

                List<CallDetailViewModel> listCallDetail = new List<CallDetailViewModel>();

                var callDetail = await _context.CallDetail.Include(p => p.OutCome).Include(p => p.AppointmentDetail).ThenInclude(p => p.AppoinStatus).ToListAsync();

                //.Where(p => p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId == id)

                listCallDetail = (from lead in callDetail.Where(p => p.AppointmentDetail.Count > 0 && p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().RelationshipManagerId == id)
                                  select lead).Select(p => new CallDetailViewModel()
                                  {
                                      CallId = p.CallId,
                                      CreatedDate = p.CreatedDate,
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      MobileNumber = p.MobileNumber,
                                      FirmName = p.FirmName,
                                      Address = p.Address,
                                      LastChangedDate = p.LastChangedDate,
                                      OutComeId = p.OutComeId,
                                      Remark = p.Remark,
                                      CreatedByName = _context.UserMaster.FirstOrDefault(r => r.UserId == p.CreatedBy).FirstName,
                                      CreatedById = _context.UserMaster.FirstOrDefault(r => r.UserId == p.CreatedBy).UserId,
                                      AppoinStatusId = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppoinStatusId : (int?)null,
                                      OutComeText = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppoinStatus.Status : "",
                                      AllocatedToName = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? _context.UserMaster.AsEnumerable().FirstOrDefault(r => r.UserId == p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId).FirstName : "",
                                      AppointmentDateTime = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppointmentDateTime : (DateTime?)null,
                                  }).ToList();


                if (filterOption.DateFilterBy == 1)
                    listCallDetail = listCallDetail.Where(p => p.CreatedDate.Date >= filterOption.FromDate.Date && p.CreatedDate.Date <= filterOption.Todate.Date).ToList();
                else if (filterOption.DateFilterBy == 2)
                    listCallDetail = listCallDetail.Where(p => Convert.ToDateTime(p.AppointmentDateTime).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.AppointmentDateTime).Date <= filterOption.Todate.Date).ToList();

                if (filterOption.CreatedBy != "0")
                    listCallDetail = listCallDetail.Where(p => p.CreatedById.ToString() == filterOption.CreatedBy).ToList();

                if (filterOption.Status > 0)
                    listCallDetail = listCallDetail.Where(p => p.AppoinStatusId == filterOption.Status).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetailByRM: " + id + "-get all Lead by RM");
                return await Task.FromResult(listCallDetail);

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetCallDetailByRM: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        // GET: api/CallDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CallDetail>> GetCallDetail(int id)
        {
            try
            {
                var callDetail = await _context.CallDetail.Include(p => p.AppointmentDetail).FirstOrDefaultAsync(p => p.CallId == id);

                if (callDetail == null)
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetail: " + id + "-lead not found");
                    return NotFound("Lead not found!");
                }

                callDetail.AppointmentDetail = callDetail.AppointmentDetail.TakeLast(1).ToList();
                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetail: " + id + "-get single lead detail");
                return callDetail;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetCallDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        // PUT: api/CallDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCallDetail(int id, CallDetail callDetail)
        {
            try
            {
                if (id != callDetail.CallId)
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "PutCallDetail: " + id + "-lead not matched");
                    return BadRequest("Lead not matched!");
                }

                if (!CallDetailExists(id))
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "PutCallDetail: -lead not exist");
                    return NotFound("Lead not found!");
                }

                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                callDetail.LastChangedDate = indianTime;
                callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(p.AppointmentDateTime), GenericMethods.Indian_Zone));
                _context.Entry(callDetail).State = EntityState.Modified;

                var lastTrans = await _context.CallTransactionDetail.OrderBy(p => p.CallTransactionId).LastOrDefaultAsync(p => p.CallId == callDetail.CallId);
                if (callDetail.OutComeId != lastTrans.OutComeId)
                    callDetail.CallTransactionDetail.Add(new CallTransactionDetail()
                    {
                        //CallId = callDetail.CallId,
                        CreatedBy = callDetail.CreatedBy,
                        OutComeId = callDetail.OutComeId,
                        Remarks = callDetail.Remark,
                    });

                _context.AppointmentDetail.UpdateRange(callDetail.AppointmentDetail);

                GenericMethods.Log(LogType.ActivityLog.ToString(), "PutCallDetail: " + id + "-lead updated successfully");
                await _context.SaveChangesAsync();
                return Ok("Lead updated successfully!");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PutFollowupDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }


        [HttpPut("PutFollowupDetail/{id}")]
        public async Task<IActionResult> PutFollowupDetail(int id, CallDetail callDetail)
        {
            try
            {
                if (id != callDetail.CallId)
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "PutFollowupDetail: " + id + "-lead not matched");
                    return BadRequest("Lead not matched!");
                }
                if (!CallDetailExists(id))
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "PutFollowupDetail: -lead not exist");
                    return NotFound("Lead not found!");
                }

                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                callDetail.LastChangedDate = indianTime;
                callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(p.AppointmentDateTime), GenericMethods.Indian_Zone));
                _context.Entry(callDetail).State = EntityState.Modified;
                _context.AppointmentDetail.UpdateRange(callDetail.AppointmentDetail);
                _context.FollowupHistory.UpdateRange(callDetail.FollowupHistory);

                GenericMethods.Log(LogType.ActivityLog.ToString(), "PutFollowupDetail: " + id + "-Followup created successfully");
                await _context.SaveChangesAsync();
                return Ok("Followup created successfully!");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PutFollowupDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }


        // POST: api/CallDetails
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult> PostCallDetail(CallDetail callDetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    callDetail.LastChangedDate = DateTime.Now;
                    callDetail.CallTransactionDetail.Add(new CallTransactionDetail()
                    {
                        //CallId = callDetail.CallId,
                        CreatedBy = callDetail.CreatedBy,
                        OutComeId = callDetail.OutComeId,
                        Remarks = callDetail.Remark,
                    });

                    _context.CallDetail.Add(callDetail);
                    await _context.SaveChangesAsync();
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "PostCallDetail: " + callDetail.CallId + "-lead created successfully");

                    return Ok("Lead created successfully!");
                }
                else
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "PostCallDetail: " + callDetail.FirstName + "-failed to create lead");
                    return BadRequest("Failed to create lead!");
                }
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostCallDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

            //return CreatedAtAction("GetCallDetail", new { id = callDetail.CallId }, callDetail);
        }

        // DELETE: api/CallDetails/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CallDetail>> DeleteCallDetail(int id)
        {
            var callDetail = await _context.CallDetail.FindAsync(id);
            if (callDetail == null)
            {
                return NotFound();
            }

            _context.CallDetail.Remove(callDetail);
            await _context.SaveChangesAsync();

            return callDetail;
        }

        private bool CallDetailExists(int id)
        {
            return _context.CallDetail.Any(e => e.CallId == id);
        }

        [HttpGet]
        [Route("GetRelationshipManagerList")]
        public async Task<ActionResult<IEnumerable<RMangerViewModel>>> GetRelationshipManagerList()
        {
            try
            {

                List<RMangerViewModel> listManager = new List<RMangerViewModel>();

                //var listUser = await _context.UserMaster.Where(p => p.RoleId == (int)Roles.RelationshipManager).ToListAsync();
                //foreach (var item in listUser)
                //{
                //    listManager.Add(new RMangerViewModel() { UserId = item.UserId, FirstName = item.FirstName, LastName = item.LastName });
                //}


                listManager = (from user in await _context.UserMaster.Where(p => p.RoleId == (int)Roles.RelationshipManager).ToListAsync()
                               select user).Select(p => new RMangerViewModel() { UserId = p.UserId, FirstName = p.FirstName, LastName = p.LastName }).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetRelationshipManagerList: " + "-get all relationship manager user");

                return listManager;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetRelationshipManagerList: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("GetTeleCallerList")]
        public async Task<ActionResult<IEnumerable<RMangerViewModel>>> GetTeleCallerList()
        {
            try
            {

                List<RMangerViewModel> listTeleCaller = new List<RMangerViewModel>();

                //var listUser = await _context.UserMaster.Where(p => p.RoleId == (int)Roles.RelationshipManager).ToListAsync();
                //foreach (var item in listUser)
                //{
                //    listManager.Add(new RMangerViewModel() { UserId = item.UserId, FirstName = item.FirstName, LastName = item.LastName });
                //}


                listTeleCaller = (from user in await _context.UserMaster.Where(p => p.RoleId == (int)Roles.TeleCaller).ToListAsync()
                                  select user).Select(p => new RMangerViewModel() { UserId = p.UserId, FirstName = p.FirstName, LastName = p.LastName }).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetTeleCallerList: " + "-get all tele caller user");

                return listTeleCaller;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTeleCallerList: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("GetCallTransDetail/{id}")]
        public async Task<ActionResult<IEnumerable<CallTransactionDetail>>> GetCallTransDetail(int id)
        {
            try
            {
                var callTransDetail = await _context.CallTransactionDetail.Include(p => p.OutCome).Include(p => p.CreatedByNavigation).Where(p => p.CallId == id).OrderByDescending(p => p.CreatedDate).ToListAsync();

                if (callTransDetail == null)
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallTransDetail: " + id + "-call trans not found");
                    return NotFound("Call transaction not found!");
                }

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallTransDetail: " + id + "-get call trans detail");
                return callTransDetail;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetCallTransDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet]
        [Route("GetFollowupHistory/{id}")]
        public async Task<ActionResult<IEnumerable<FollowupHistory>>> GetFollowupHistory(int id)
        {
            try
            {
                var followupHistory = await _context.FollowupHistory.Include(p => p.AppoinStatus).Include(p => p.CreatedByRmanager).Where(p => p.CallId == id).OrderByDescending(p => p.CreatedDate).ToListAsync();

                if (followupHistory == null)
                {
                    GenericMethods.Log(LogType.ActivityLog.ToString(), "GetFollowupHistory: " + id + "-followup history not found");
                    return NotFound("followup history not found!");
                }

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetFollowupHistory: " + id + "-get followup history");
                return followupHistory;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetFollowupHistory: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost]
        [Route("UploadExcelData/{id}")]
        public async Task<ActionResult> UploadExcelData(Guid id)
        {
            try
            {
                if (HttpContext.Request.Form.Files.Count > 0)
                {
                    var file = HttpContext.Request.Form.Files[0];
                    if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx"))
                        return BadRequest("Invalid file input!");

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    Stream stream = file.OpenReadStream();
                    using (ExcelPackage package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[0];

                        int firstName = workSheet.Cells["1:1"].First(c => c.Value.ToString() == "First Name").Start.Column;
                        int lastName = workSheet.Cells["1:1"].First(c => c.Value.ToString() == "Last Name").Start.Column;
                        int mobileNumber = workSheet.Cells["1:1"].First(c => c.Value.ToString() == "Mobile Number").Start.Column;
                        int address = workSheet.Cells["1:1"].First(c => c.Value.ToString() == "Address").Start.Column;
                        var remarksCell = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value.ToString() == "Remarks");
                        int remarks = 0;
                        if (remarksCell != null)
                            remarks = remarksCell.Start.Column;

                        int totalRows = workSheet.Dimension.Rows;

                        List<CallDetail> callDetail = new List<CallDetail>();

                        for (int i = 2; i <= totalRows; i++)
                        {
                            callDetail.Add(new CallDetail()
                            {
                                CreatedBy = id,
                                FirstName = workSheet.Cells[i, firstName].Value.ToString(),
                                LastName = workSheet.Cells[i, lastName].Value.ToString(),
                                MobileNumber = workSheet.Cells[i, mobileNumber].Value.ToString(),
                                Address = workSheet.Cells[i, address].Value.ToString(),
                                LastChangedDate = DateTime.Now,
                                OutComeId = (int)CallOutcome.NoResponse,
                                Remark = remarks > 0 ? workSheet.Cells[i, remarks].Value.ToString() : "Uploaded from excel",
                                CallTransactionDetail = new List<CallTransactionDetail>()
                                {
                                    new CallTransactionDetail()
                                    {
                                        CreatedBy=id,
                                        OutComeId=(int)CallOutcome.NoResponse,
                                        Remarks= remarks > 0 ? workSheet.Cells[i, remarks].Value.ToString() : "Uploaded from excel"
                                    }
                                }
                            });
                        }

                        await _context.CallDetail.AddRangeAsync(callDetail);
                        await _context.SaveChangesAsync();
                    }
                    return Ok("Data uploaded successfully!");
                }
                return NotFound("File not found!");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "UploadExcelData: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Route("GetTeleCallerDashboard/{id}")]
        public async Task<ActionResult<TeleCallerDashboard>> GetTeleCallerDashboard(Guid id)
        {
            try
            {
                var callDetail = await _context.CallDetail.Where(p => p.CreatedBy == id).ToListAsync();


                TeleCallerDashboard objTeleDash = new TeleCallerDashboard()
                {
                    TotalLeads = callDetail.Count(),
                    NoResponse = callDetail.Where(r => r.OutComeId == (int)Enums.CallOutcome.NoResponse).Count(),
                    AppoinmentTaken = callDetail.Where(r => r.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken).Count(),
                    NotInterested = callDetail.Where(r => r.OutComeId == (int)Enums.CallOutcome.NotInterested).Count(),
                };

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetTeleCallerDashboard: " + id + "-get telecaller dashboard");
                return objTeleDash;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTeleCallerDashboard: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }
    }
}
