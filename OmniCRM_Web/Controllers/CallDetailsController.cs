using System;
using System.Collections.Generic;
using System.Globalization;
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
using SQLitePCL;
using static OmniCRM_Web.GenericClasses.Enums;
using static OmniCRM_Web.ViewModels.RelaManagerStatusReport;
using static OmniCRM_Web.ViewModels.TeleCallerStatusReport;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StringConstant.TeleCaller + "," + StringConstant.RelationshipManager + "," + StringConstant.Admin + "," + StringConstant.SuperUser)]

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
                bool isAdmin = _context.UserMaster.FirstOrDefault(p => p.UserId == id).RoleId == (int)Roles.Admin;

                var listCallDetail = (from lead in await _context.CallDetail.Include(p => p.OutCome).Include(p => p.AppointmentDetail).Where(p => isAdmin == false ? p.CreatedBy == id : true).ToListAsync()
                                      select lead).Select(p => new CallDetailViewModel()
                                      {
                                          CallId = p.CallId,
                                          CreatedDate = p.CreatedDate,
                                          FirstName = p.FirstName,
                                          LastName = p.LastName,
                                          MobileNumber = p.MobileNumber,
                                          FirmName = p.FirmName,
                                          EmailId = p.EmailId,
                                          Address = p.Address,
                                          StateName = p.StateId != null ? _context.StateMaster.FirstOrDefault(r => r.StateId == p.StateId).StateName : "",
                                          CityName = p.CityId != null ? _context.CityMaster.FirstOrDefault(r => r.CityId == p.CityId).CityName : "",
                                          LastChangedDate = p.LastChangedDate,
                                          OutComeId = p.OutComeId,
                                          NextCallDate = p.NextCallDate,
                                          Remark = p.Remark,
                                          CreatedById = p.CreatedBy,
                                          CreatedByName = _context.UserMaster.FirstOrDefault(r => r.UserId == p.CreatedBy).FirstName,
                                          OutComeText = p.OutCome.OutCome,
                                          AllocatedToId = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId : Guid.Empty,
                                          AllocatedToName = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? _context.UserMaster.AsEnumerable().FirstOrDefault(r => r.UserId == p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId).FirstName : "",
                                          AppointmentDateTime = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppointmentDateTime : (DateTime?)null,
                                          IsDeleted = p.IsDeleted,
                                      }).ToList();

                listCallDetail = listCallDetail.Where(p => p.IsDeleted != true).ToList();

                if (filterOption.DateFilterBy == 1)
                    listCallDetail = listCallDetail.Where(p => p.CreatedDate.Date >= filterOption.FromDate.Date && p.CreatedDate.Date <= filterOption.Todate.Date
                        || (p.OutComeId == (int)CallOutcome.CallLater && Convert.ToDateTime(p.NextCallDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.NextCallDate).Date <= filterOption.Todate.Date)).ToList();
                else if (filterOption.DateFilterBy == 2)
                    listCallDetail = listCallDetail.Where(p => Convert.ToDateTime(p.AppointmentDateTime).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.AppointmentDateTime).Date <= filterOption.Todate.Date).ToList();
                else if (filterOption.DateFilterBy == 3)
                    listCallDetail = listCallDetail.Where(p => Convert.ToDateTime(p.LastChangedDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.LastChangedDate).Date <= filterOption.Todate.Date).ToList();

                if (filterOption.AllocatedTo != "0" && filterOption.AllocatedTo != null)
                    listCallDetail = listCallDetail.Where(p => p.AllocatedToId.ToString() == filterOption.AllocatedTo).ToList();

                if (filterOption.CreatedBy != "0" && filterOption.CreatedBy != null)
                    listCallDetail = listCallDetail.Where(p => p.CreatedById.ToString() == filterOption.CreatedBy).ToList();

                if (filterOption.Status.Count > 0)
                    listCallDetail = listCallDetail.Where(p => filterOption.Status.Any(r => r == p.OutComeId)).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetail: " + id + "-get all Lead by created user");
                return await Task.FromResult(listCallDetail.Skip(filterOption.ToSkip).Take(filterOption.ToTake).ToList());

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
                bool isAdmin = _context.UserMaster.FirstOrDefault(p => p.UserId == id).RoleId == (int)Roles.Admin;

                //.Where(p => p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId == id)

                listCallDetail = (from lead in callDetail.Where(p => p.AppointmentDetail.Count > 0 && (isAdmin == false ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().RelationshipManagerId == id : p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().RelationshipManagerId != null))
                                  select lead).Select(p => new CallDetailViewModel()
                                  {
                                      CallId = p.CallId,
                                      CreatedDate = p.CreatedDate,
                                      FirstName = p.FirstName,
                                      LastName = p.LastName,
                                      MobileNumber = p.MobileNumber,
                                      FirmName = p.FirmName,
                                      EmailId = p.EmailId,
                                      Address = p.Address,
                                      StateName = p.StateId != null ? _context.StateMaster.FirstOrDefault(r => r.StateId == p.StateId).StateName : "",
                                      CityName = p.CityId != null ? _context.CityMaster.FirstOrDefault(r => r.CityId == p.CityId).CityName : "",
                                      LastChangedDate = p.LastChangedDate,
                                      OutComeId = p.OutComeId,
                                      Remark = p.Remark,
                                      CreatedByName = _context.UserMaster.FirstOrDefault(r => r.UserId == p.CreatedBy).FirstName,
                                      CreatedById = _context.UserMaster.FirstOrDefault(r => r.UserId == p.CreatedBy).UserId,
                                      AppoinStatusId = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppoinStatusId : (int?)null,
                                      OutComeText = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppoinStatus.Status : "",
                                      AllocatedToId = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId : Guid.Empty,
                                      AllocatedToName = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? _context.UserMaster.AsEnumerable().FirstOrDefault(r => r.UserId == p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId).FirstName : "",
                                      AppointmentDateTime = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppointmentDateTime : (DateTime?)null,
                                  }).ToList();

                if (filterOption.DateFilterBy == 1)
                    listCallDetail = listCallDetail.Where(p => p.CreatedDate.Date >= filterOption.FromDate.Date && p.CreatedDate.Date <= filterOption.Todate.Date).ToList();
                else if (filterOption.DateFilterBy == 2)
                    listCallDetail = listCallDetail.Where(p => Convert.ToDateTime(p.AppointmentDateTime).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.AppointmentDateTime).Date <= filterOption.Todate.Date).ToList();
                else if (filterOption.DateFilterBy == 3)
                    listCallDetail = listCallDetail.Where(p => Convert.ToDateTime(p.LastChangedDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(p.LastChangedDate).Date <= filterOption.Todate.Date).ToList();

                if (filterOption.AllocatedTo != "0" && filterOption.AllocatedTo != null)
                    listCallDetail = listCallDetail.Where(p => p.AllocatedToId.ToString() == filterOption.AllocatedTo).ToList();

                if (filterOption.CreatedBy != "0" && filterOption.CreatedBy != null)
                    listCallDetail = listCallDetail.Where(p => p.CreatedById.ToString() == filterOption.CreatedBy).ToList();

                if (filterOption.Status.Count > 0)
                    listCallDetail = listCallDetail.Where(p => filterOption.Status.Any(r => r == p.AppoinStatusId)).ToList();

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
        [HttpGet("GetLeadById/{id}")]
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

                _context.Entry(callDetail).State = EntityState.Modified;

                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                callDetail.LastChangedDate = indianTime;

                if (callDetail.NextCallDate != null)
                    callDetail.NextCallDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(callDetail.NextCallDate), GenericMethods.Indian_Zone);


                var ObjAppointment = callDetail.AppointmentDetail.LastOrDefault();
                if (ObjAppointment != null && ObjAppointment.AppointmentDateTime != null)
                {
                    callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(p.AppointmentDateTime), GenericMethods.Indian_Zone));

                    /*
                    List<AppointmentDetail> CollAppointments = new List<AppointmentDetail>();
                    CollAppointments = await _context.AppointmentDetail.Where(p => p.RelationshipManagerId == ObjAppointment.RelationshipManagerId
                        && p.AppointmentDateTime.Value.Date == ObjAppointment.AppointmentDateTime.Value.Date).AsNoTracking().ToListAsync();

                    if (CollAppointments.Count > 0)
                    {
                        if (CollAppointments.Count(p => p.AppointmentDateTime.Value.AddMinutes(15) > ObjAppointment.AppointmentDateTime && p.AppointmentDateTime.Value.AddMinutes(-15) < ObjAppointment.AppointmentDateTime) > 0)
                            return BadRequest("Appointment time is already allocated on this day!");
                    }
                     */
                }


                var lastTrans = await _context.CallTransactionDetail.OrderBy(p => p.CallTransactionId).LastOrDefaultAsync(p => p.CallId == callDetail.CallId);
                if (callDetail.OutComeId != lastTrans.OutComeId || callDetail.NextCallDate != null)
                    callDetail.CallTransactionDetail.Add(new CallTransactionDetail()
                    {
                        //CallId = callDetail.CallId,
                        //CreatedBy = callDetail.CreatedBy,
                        CreatedBy = new Guid(User.Claims.FirstOrDefault().Value),
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
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PutCallDetail: " + ex.ToString());
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

                _context.Entry(callDetail).State = EntityState.Modified;

                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                callDetail.LastChangedDate = indianTime;


                var ObjAppointment = callDetail.AppointmentDetail.LastOrDefault();
                if (ObjAppointment != null && ObjAppointment.AppointmentDateTime != null)
                {
                    callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(p.AppointmentDateTime), GenericMethods.Indian_Zone));

                    /*
                    List<AppointmentDetail> CollAppointments = new List<AppointmentDetail>();
                    CollAppointments = await _context.AppointmentDetail.Where(p => p.RelationshipManagerId == ObjAppointment.RelationshipManagerId
                        && p.AppointmentDateTime.Value.Date == ObjAppointment.AppointmentDateTime.Value.Date).AsNoTracking().ToListAsync();

                    if (CollAppointments.Count > 0)
                    {
                        if (CollAppointments.Count(p => p.AppointmentDateTime.Value.AddMinutes(15) > ObjAppointment.AppointmentDateTime && p.AppointmentDateTime.Value.AddMinutes(-15) < ObjAppointment.AppointmentDateTime) > 0)
                            return BadRequest("Appointment time is already allocated on this day!");
                    }
                    */
                }


                _context.AppointmentDetail.UpdateRange(callDetail.AppointmentDetail);
                _context.FollowupHistory.AddRange(callDetail.FollowupHistory);

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


        [HttpPut("DismissLeads")]
        public async Task<IActionResult> DismissLeads(List<CallDetail> collCallDetail)
        {
            try
            {
                if (collCallDetail.Count > 0)
                {
                    foreach (var lead in collCallDetail)
                    {
                        var objLead = await GetCallDetail(lead.CallId);
                        var objAppointment = objLead.Value.AppointmentDetail.LastOrDefault();
                        objAppointment.Remarks = "Lead Dismissed";
                        objAppointment.AppoinStatusId = 8;

                        FollowupHistory objFollowup = new FollowupHistory();
                        objFollowup.CallId = objLead.Value.CallId;
                        objFollowup.CreatedByRmanagerId = objAppointment.RelationshipManagerId;
                        objFollowup.FollowupType = string.Empty;
                        objFollowup.AppoinDate = Convert.ToDateTime(objAppointment.AppointmentDateTime);
                        objFollowup.AppoinStatusId = 8;
                        objFollowup.Remarks = "Lead Dismissed";
                        objAppointment.AppointmentDateTime = null;

                        objLead.Value.FollowupHistory.Add(objFollowup);
                        await PutFollowupDetail(lead.CallId, objLead.Value);
                        GenericMethods.Log(LogType.ActivityLog.ToString(), "DismissLeads: " + lead.CallId + "-Lead dismissed successfully");
                    }

                    return Ok("Leads dismissed successfully!");
                }
                else
                    return NotFound("Leads not found!");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "DismissLeads: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPut("RemindMeLater/{strDate}")]
        public async Task<IActionResult> RemindMeLater(string strDate, List<CallDetail> collCallDetail)
        {
            try
            {
                if (collCallDetail.Count > 0)
                {
                    DateTime date = DateTime.Parse(strDate);

                    foreach (var lead in collCallDetail)
                    {
                        var objLead = await GetCallDetail(lead.CallId);
                        var objAppointment = objLead.Value.AppointmentDetail.LastOrDefault();
                        objAppointment.Remarks = "Remind Me Later";
                        objAppointment.AppointmentDateTime = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0).ToUniversalTime();

                        FollowupHistory objFollowup = new FollowupHistory();
                        objFollowup.CallId = objLead.Value.CallId;
                        objFollowup.CreatedByRmanagerId = objAppointment.RelationshipManagerId;
                        objFollowup.FollowupType = string.Empty;
                        objFollowup.AppoinDate = Convert.ToDateTime(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(objAppointment.AppointmentDateTime), GenericMethods.Indian_Zone));
                        objFollowup.AppoinStatusId = objAppointment.AppoinStatusId;
                        objFollowup.Remarks = "Remind Me Later";

                        objLead.Value.FollowupHistory.Add(objFollowup);
                        await PutFollowupDetail(lead.CallId, objLead.Value);
                        GenericMethods.Log(LogType.ActivityLog.ToString(), "RemindMeLater: " + lead.CallId + "-Next Followup Updated successfully");
                    }

                    return Ok("Leads next followup updated successfully!");
                }
                else
                    return NotFound("Leads not found!");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "RemindMeLater: " + ex.ToString());
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
                    DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                    callDetail.LastChangedDate = indianTime;

                    if (callDetail.NextCallDate != null)
                        callDetail.NextCallDate = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(callDetail.NextCallDate), GenericMethods.Indian_Zone);


                    var ObjAppointment = callDetail.AppointmentDetail.LastOrDefault();
                    if (ObjAppointment != null && ObjAppointment.AppointmentDateTime != null)
                    {
                        callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(p.AppointmentDateTime), GenericMethods.Indian_Zone));

                        /*
                        List<AppointmentDetail> CollAppointments = new List<AppointmentDetail>();
                        CollAppointments = await _context.AppointmentDetail.Where(p => p.RelationshipManagerId == ObjAppointment.RelationshipManagerId
                            && p.AppointmentDateTime.Value.Date == ObjAppointment.AppointmentDateTime.Value.Date).AsNoTracking().ToListAsync();

                        if (CollAppointments.Count > 0)
                        {
                            if (CollAppointments.Count(p => p.AppointmentDateTime.Value.AddMinutes(15) > ObjAppointment.AppointmentDateTime && p.AppointmentDateTime.Value.AddMinutes(-15) < ObjAppointment.AppointmentDateTime) > 0)
                                return BadRequest("Appointment time is already allocated on this day!");
                        }
                        */
                    }


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
        public async Task<ActionResult<CallDetail>> DeletCallDetail(int id)
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

        // Delete Set isDelete
        [HttpPut("DeleteCallDetail")]
        public async Task<IActionResult> DeleteCallDetail(List<CallDetail> collCallDetail)
        {
            try
            {
                if (collCallDetail.Count > 0)
                {
                    foreach (var lead in collCallDetail)
                    {
                        var objLead = await GetCallDetail(lead.CallId);

                        _context.Entry(objLead.Value).State = EntityState.Modified;

                        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                        objLead.Value.LastChangedDate = indianTime;
                        objLead.Value.IsDeleted = true;
                        GenericMethods.Log(LogType.ActivityLog.ToString(), "DeleteCallDetail: " + objLead.Value.CallId + "-Deleted successfully");
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Lead deleted successfully!");
                }
                else
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "DeleteCallDetail: -lead not exist");
                    return NotFound("Leads not found!");
                }
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "DeleteCallDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
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
                        int mobileNumber = workSheet.Cells["1:1"].First(c => c.Value.ToString() == "Mobile Number").Start.Column;

                        var lastNameCell = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value.ToString() == "Last Name");
                        int lastName = 0;
                        if (lastNameCell != null)
                            lastName = lastNameCell.Start.Column;

                        var addressCell = workSheet.Cells["1:1"].FirstOrDefault(c => c.Value.ToString() == "Address");
                        int address = 0;
                        if (addressCell != null)
                            address = addressCell.Start.Column;

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
                                MobileNumber = workSheet.Cells[i, mobileNumber].Value.ToString(),
                                LastName = lastName > 0 ? Convert.ToString(workSheet.Cells[i, lastName].Value) : null,
                                Address = address > 0 ? Convert.ToString(workSheet.Cells[i, address].Value) : null,
                                LastChangedDate = DateTime.Now,
                                OutComeId = (int)CallOutcome.None,
                                Remark = remarks > 0 ? Convert.ToString(workSheet.Cells[i, remarks].Value) : null,
                                CallTransactionDetail = new List<CallTransactionDetail>()
                                {
                                    new CallTransactionDetail()
                                    {
                                        CreatedBy = id,
                                        OutComeId = (int)CallOutcome.NoResponse,
                                        Remarks = remarks > 0 ? Convert.ToString(workSheet.Cells[i, remarks].Value) : null
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
                var callDetail = await _context.CallDetail.Where(p => p.CreatedBy == id && p.IsDeleted != true).ToListAsync();
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                int lastMonth = DateTime.Now.AddMonths(-1).Month;


                TeleCallerDashboard objTeleDash = new TeleCallerDashboard()
                {
                    TotalLeads = callDetail.Count(),
                    NoResponse = callDetail.Count(r => r.OutComeId == (int)Enums.CallOutcome.NoResponse),
                    AppoinmentTaken = callDetail.Count(r => r.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                    NotInterested = callDetail.Count(r => r.OutComeId == (int)Enums.CallOutcome.NotInterested),

                    MonthlyTotalLeads = callDetail.Count(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear),
                    MonthlyNoResponse = callDetail.Count(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear && p.OutComeId == (int)Enums.CallOutcome.NoResponse),
                    MonthlyAppoinmentTaken = callDetail.Count(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear && p.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                    MonthlyNotInterested = callDetail.Count(p => p.CreatedDate.Month == currentMonth && p.CreatedDate.Year == currentYear && p.OutComeId == (int)Enums.CallOutcome.NotInterested),

                    LastMonthTotalLeads = callDetail.Count(p => p.CreatedDate.Month == lastMonth && p.CreatedDate.Year == (currentYear == 1 ? 12 : currentYear)),
                    LastMonthNoResponse = callDetail.Count(p => p.CreatedDate.Month == lastMonth && p.CreatedDate.Year == (currentYear == 1 ? 12 : currentYear) && p.OutComeId == (int)Enums.CallOutcome.NoResponse),
                    LastMonthAppoinmentTaken = callDetail.Count(p => p.CreatedDate.Month == lastMonth && p.CreatedDate.Year == (currentYear == 1 ? 12 : currentYear) && p.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                    LastMonthNotInterested = callDetail.Count(p => p.CreatedDate.Month == lastMonth && p.CreatedDate.Year == (currentYear == 1 ? 12 : currentYear) && p.OutComeId == (int)Enums.CallOutcome.NotInterested),
                };


                objTeleDash.CollChartData = callDetail.Where(p => p.CreatedDate.Year == currentYear)
                    .GroupBy(p => new { Month = p.CreatedDate.Month }).Select(p => new ChartData()
                    {
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p.Key.Month).Substring(0, 3),
                        AppoinTaken = p.Count(p => p.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                        NotInterest = p.Count(p => p.OutComeId == (int)Enums.CallOutcome.NotInterested),

                    }).ToList();



                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetTeleCallerDashboard: " + id + "-get telecaller dashboard");
                return objTeleDash;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetTeleCallerDashboard: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }


        [HttpGet("{id}")]
        [Route("GetRManagerDashboard/{id}")]
        public async Task<ActionResult<RManagerDashboard>> GetRManagerDashboard(Guid id)
        {
            try
            {
                var callDetail = await _context.CallDetail.Include(p => p.AppointmentDetail).ThenInclude(p => p.AppoinStatus).Include(p => p.FollowupHistory).Where(p => p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().RelationshipManagerId == id).ToListAsync();
                int currentYear = DateTime.Now.Year;
                int currentMonth = DateTime.Now.Month;
                int lastMonth = DateTime.Now.AddMonths(-1).Month;


                RManagerDashboard objManagerDash = new RManagerDashboard()
                {
                    TotalAllocatedLeads = callDetail.Count(),
                    SoldLeads = callDetail.Count(r => r.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Sold),
                    HoldLeads = callDetail.Count(r => r.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Hold),
                    DroppedLeads = callDetail.Count(r => r.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Dropped),

                    MonthlySold = callDetail.Count(p => p.FollowupHistory.Count > 0 && p.FollowupHistory.LastOrDefault().CreatedDate.Month == currentMonth && p.FollowupHistory.LastOrDefault().CreatedDate.Year == currentYear && p.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Sold),
                    MonthlyHold = callDetail.Count(p => p.FollowupHistory.Count > 0 && p.FollowupHistory.LastOrDefault().CreatedDate.Month == currentMonth && p.FollowupHistory.LastOrDefault().CreatedDate.Year == currentYear && p.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Hold),
                    MonthlyDropped = callDetail.Count(p => p.FollowupHistory.Count > 0 && p.FollowupHistory.LastOrDefault().CreatedDate.Month == currentMonth && p.FollowupHistory.LastOrDefault().CreatedDate.Year == currentYear && p.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Dropped),
                };


                objManagerDash.CollChartData = callDetail.Where(p => p.FollowupHistory.Count > 0 && p.FollowupHistory.LastOrDefault().CreatedDate.Year == currentYear)
                    .GroupBy(p => new { p.FollowupHistory.LastOrDefault().CreatedDate.Month }).Select(p => new ChartDataMnager()
                    {
                        MonthNumber = p.Key.Month,
                        Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p.Key.Month).Substring(0, 3),
                        Sold = p.Count(p => p.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Sold),
                        Dropped = p.Count(p => p.AppointmentDetail.LastOrDefault().AppoinStatusId == (int)Enums.AppoinmentStatus.Dropped),
                    }).OrderBy(p => p.MonthNumber).ToList();

                objManagerDash.CollCalendarEvents = (from call in callDetail.Where(p => p.AppointmentDetail.LastOrDefault().AppointmentDateTime != null) select call).Select(p => new EventCalendar()
                {
                    CallId = p.CallId,
                    AppointmentTime = Convert.ToDateTime(p.AppointmentDetail.LastOrDefault().AppointmentDateTime),
                    AppointStatus = p.AppointmentDetail.LastOrDefault().AppoinStatus.Status,
                    ClientName = p.FirstName + " " + p.LastName,
                    AppointStatusId = p.AppointmentDetail.LastOrDefault().AppoinStatusId
                }).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetRManagerDashboard: " + id + "-get manager dashboard");
                return objManagerDash;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetRManagerDashboard: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        [HttpPost("GetAdminDashboard")]
        public async Task<ActionResult<AdminDashboard>> GetAdminDashboard(FilterOptions filterOption)
        {
            try
            {
                //int currentYear = DateTime.Now.Year;
                //int currentMonth = DateTime.Now.Month;
                //int lastMonth = DateTime.Now.AddMonths(-1).Month;
                filterOption.FromDate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.FromDate, GenericMethods.Indian_Zone);
                filterOption.Todate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.Todate, GenericMethods.Indian_Zone);
                AdminDashboard objAdminDash = new AdminDashboard();

                var TeleCallerLeads = from Teleuser in await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.TeleCaller).ToListAsync()
                                      join Leads in _context.CallTransactionDetail.AsEnumerable().Where(q => Convert.ToDateTime(q.CreatedDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(q.CreatedDate).Date <= filterOption.Todate.Date).ToList() on Teleuser.UserId equals Leads.CreatedBy into UserLead
                                      from TeleLeads in UserLead.DefaultIfEmpty()
                                      select new { Teleuser.FirstName, UserLead, TeleLeads };


                objAdminDash.CollTeleChartData = TeleCallerLeads.GroupBy(p => new { createdBy = p.FirstName }).Select(r => new TeleCallerChartData()
                {
                    Telecaller = r.Key.createdBy,
                    NoResponse = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.NoResponse),
                    NotInterested = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.NotInterested),
                    AppoinmentTaken = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                    CallLater = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.CallLater),
                    WrongNumber = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.WrongNumber),
                    None = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.None),

                }).ToList();


                var ManagerLeads = from Manauser in await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.RelationshipManager).ToListAsync()
                                   join Leads in _context.FollowupHistory.AsEnumerable().Where(q => Convert.ToDateTime(q.CreatedDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(q.CreatedDate).Date <= filterOption.Todate.Date).ToList() on Manauser.UserId equals Leads.CreatedByRmanagerId into UserLead
                                   from ManaLeads in UserLead.DefaultIfEmpty()
                                   select new { Manauser.FirstName, UserLead, ManaLeads };


                objAdminDash.CollMangerChartData = ManagerLeads.GroupBy(p => new { createdBy = p.FirstName }).Select(r => new ManagerChartData()
                {
                    Manager = r.Key.createdBy,
                    FirstMeeting = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.FirstMeeting),
                    SecondMeeting = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.SecondMeeting),
                    Sold = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Sold),
                    Dropped = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Dropped),
                    Hold = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Hold),
                    NotInterested = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.NotInterested),
                    Pending = r.Count(q => q.ManaLeads != null && q.ManaLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Pending),

                }).ToList();


                //var teleUserLeads = await _context.UserMaster.Include(p => p.CallDetail).Where(p => p.Status == true && p.RoleId == (int)Roles.TeleCaller).ToListAsync();
                //var managerUsers = await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.RelationshipManager).ToListAsync();

                //var managerLeads = from user in managerUsers
                //                   join leads in _context.AppointmentDetail.Include(p => p.Call) on user.UserId equals leads.RelationshipManagerId into usrled
                //                   from mngrld in usrled.DefaultIfEmpty()
                //                   select new { user.FirstName, usrled, mngrld };

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetAdminDashboard: -get admin dashboard");
                return objAdminDash;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetAdminDashboard: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        [HttpPost("PostReAllocateRM")]
        public async Task<IActionResult> PostReAllocateRM(List<AppointmentDetail> collAppointmentDetail)
        {
            try
            {
                if (collAppointmentDetail.Count > 0)
                {
                    foreach (var item in collAppointmentDetail)
                    {
                        var objLead = await GetCallDetail(item.CallId);
                        _context.Entry(objLead.Value).State = EntityState.Modified;

                        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);
                        objLead.Value.LastChangedDate = indianTime;
                        item.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(item.AppointmentDateTime), GenericMethods.Indian_Zone);
                        objLead.Value.AppointmentDetail.Add(item);

                        GenericMethods.Log(LogType.ActivityLog.ToString(), "PostReAllocateRM: " + item.CallId + " -RM Re-allocated successfully");
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Lead re-allocated successfully!");
                }
                else
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "PostReAllocateRM: -lead not exist");
                    return NotFound("Leads not found!");
                }
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostReAllocateRM: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        [HttpPost("PostReAllocateTC")]
        public async Task<IActionResult> PostReAllocateTC(List<CallDetail> collCallDetail)
        {
            try
            {
                if (collCallDetail.Count > 0)
                {
                    foreach (var lead in collCallDetail)
                    {
                        var objLead = await GetCallDetail(lead.CallId);
                        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, GenericMethods.Indian_Zone);

                        _context.Entry(objLead.Value).State = EntityState.Modified;
                        objLead.Value.CreatedBy = lead.CreatedBy;
                        objLead.Value.LastChangedDate = indianTime;

                        GenericMethods.Log(LogType.ActivityLog.ToString(), "PostReAllocateTC: " + objLead.Value.CallId + " -TC Re-allocated successfully");
                    }
                    await _context.SaveChangesAsync();
                    return Ok("Lead re-allocated successfully!");
                }
                else
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "PostReAllocateTC: -lead not exist");
                    return NotFound("Leads not found!");
                }
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostReAllocateTC: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        [HttpPost("GetAdminTCSummaryReport")]
        public async Task<ActionResult<TeleCallerStatusReport>> GetAdminTCSummaryReport(FilterOptions filterOption)
        {
            try
            {
                TeleCallerStatusReport TCStatusReport = new TeleCallerStatusReport() { Header = new List<string>(), TCRowsData = new List<RowsData>() };
                TCStatusReport.Header.Add("Tele Caller");
                foreach (var item in await _context.CallOutcomeMaster.ToListAsync())
                {
                    TCStatusReport.Header.Add(item.OutCome);
                }
                TCStatusReport.Header.Add("Total");

                //foreach (var user in await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.TeleCaller).ToListAsync())
                //{
                //    TCStatusReport.TCRowsData.Add(new TeleCallerStatusReport.RowsData()
                //    {
                //        TCName = user.FirstName + " " + user.LastName,
                //        AppoinmentTaken = 
                //    });
                //}

                var TeleCallerLeads = from Teleuser in await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.TeleCaller).ToListAsync()
                                      join Leads in _context.CallDetail.AsEnumerable().Where(q => Convert.ToDateTime(q.CreatedDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(q.CreatedDate).Date <= filterOption.Todate.Date).ToList() on Teleuser.UserId equals Leads.CreatedBy into UserLead
                                      from TeleLeads in UserLead.DefaultIfEmpty()
                                      select new { Teleuser.FirstName, UserLead, TeleLeads };


                TCStatusReport.TCRowsData = TeleCallerLeads.GroupBy(p => new { createdBy = p.FirstName }).Select(r => new RowsData()
                {
                    TCName = r.Key.createdBy,
                    NoResponse = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.NoResponse),
                    NotInterested = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.NotInterested),
                    AppoinmentTaken = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                    CallLater = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.CallLater),
                    WrongNumber = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.WrongNumber),
                    None = r.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.None),
                    Total = r.Count(q => q.TeleLeads != null)
                }).ToList();

                var totalRow = new RowsData()
                {
                    TCName = "Total",
                    NoResponse = TeleCallerLeads.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.NoResponse),
                    NotInterested = TeleCallerLeads.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.NotInterested),
                    AppoinmentTaken = TeleCallerLeads.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.AppoinmentTaken),
                    CallLater = TeleCallerLeads.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.CallLater),
                    WrongNumber = TeleCallerLeads.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.WrongNumber),
                    None = TeleCallerLeads.Count(q => q.TeleLeads != null && q.TeleLeads.OutComeId == (int)Enums.CallOutcome.None),
                    Total = TeleCallerLeads.Count(q => q.TeleLeads != null)
                };

                TCStatusReport.TCRowsData.Add(totalRow);

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetAdminTCSummaryReport: -get tele caller summary report in admin");
                return TCStatusReport;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetAdminTCSummaryReport: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        [HttpPost("GetAdminRMSummaryReport")]
        public async Task<ActionResult<RelaManagerStatusReport>> GetAdminRMSummaryReport(FilterOptions filterOption)
        {
            try
            {
                RelaManagerStatusReport RMStatusReport = new RelaManagerStatusReport() { Header = new List<string>(), RMRowsData = new List<RowsDataRM>() };
                RMStatusReport.Header.Add("Relationship Manager");
                foreach (var item in await _context.AppoinmentStatusMaster.Where(p => p.AppoinStatusId != (int)AppoinmentStatus.Dismissed).ToListAsync())
                {
                    RMStatusReport.Header.Add(item.Status);
                }
                RMStatusReport.Header.Add("Total");

                //foreach (var user in await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.TeleCaller).ToListAsync())
                //{
                //    TCStatusReport.TCRowsData.Add(new TeleCallerStatusReport.RowsData()
                //    {
                //        TCName = user.FirstName + " " + user.LastName,
                //        AppoinmentTaken = 
                //    });
                //}

                var RelaManagerLeads = from Relauser in await _context.UserMaster.Where(p => p.Status == true && p.RoleId == (int)Roles.RelationshipManager).ToListAsync()
                                       join Leads in _context.AppointmentDetail.AsEnumerable().Where(q => q.AppoinStatusId != (int)AppoinmentStatus.Dismissed && Convert.ToDateTime(q.CreatedDate).Date >= filterOption.FromDate.Date && Convert.ToDateTime(q.CreatedDate).Date <= filterOption.Todate.Date).ToList() on Relauser.UserId equals Leads.RelationshipManagerId into UserLead
                                       from ReleLeads in UserLead.DefaultIfEmpty()
                                       select new { Relauser.FirstName, UserLead, ReleLeads };


                RMStatusReport.RMRowsData = RelaManagerLeads.GroupBy(p => new { createdBy = p.FirstName }).Select(r => new RowsDataRM()
                {
                    RMName = r.Key.createdBy,
                    FirstMeeting = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.FirstMeeting),
                    SecondMeeting = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.SecondMeeting),
                    Sold = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Sold),
                    Dropped = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Dropped),
                    Hold = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Hold),
                    NotInterested = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.NotInterested),
                    Pending = r.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Pending),
                    Total = r.Count(q => q.ReleLeads != null)
                }).ToList();

                var totalRow = new RowsDataRM()
                {
                    RMName = "Total",
                    FirstMeeting = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.FirstMeeting),
                    SecondMeeting = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.SecondMeeting),
                    Sold = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Sold),
                    Dropped = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Dropped),
                    Hold = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Hold),
                    NotInterested = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.NotInterested),
                    Pending = RelaManagerLeads.Count(q => q.ReleLeads != null && q.ReleLeads.AppoinStatusId == (int)Enums.AppoinmentStatus.Pending),
                    Total = RelaManagerLeads.Count(q => q.ReleLeads != null)
                };

                RMStatusReport.RMRowsData.Add(totalRow);

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetAdminRMSummaryReport: -get relationship manager summary report in admin");
                return RMStatusReport;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetAdminRMSummaryReport: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }
    }
}
