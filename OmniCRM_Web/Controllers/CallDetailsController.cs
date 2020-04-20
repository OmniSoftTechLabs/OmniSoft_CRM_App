﻿using System;
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
    [Authorize(Roles = StringConstant.SuperUser + "," + StringConstant.TeleCaller + "," + StringConstant.RelationshipManager)]

    public class CallDetailsController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public CallDetailsController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/CallDetails
        [HttpPost("GetCallDetailByCreatedBy/{id}")]
        public async Task<ActionResult<IEnumerable<CallDetailViewModel>>> GetCallDetailByCreatedBy(Guid id, [FromBody] FilterOptions filterOption)
        {
            try
            {
                filterOption.FromDate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.FromDate, GenericMethods.Indian_Zone);
                filterOption.Todate = TimeZoneInfo.ConvertTimeFromUtc(filterOption.Todate, GenericMethods.Indian_Zone);

                var callDetail = await _context.CallDetail.Include(p => p.AppointmentDetail).ToListAsync();

                List<CallDetailViewModel> listCallDetail = new List<CallDetailViewModel>();

                listCallDetail = (from lead in await _context.CallDetail.Include(p => p.OutCome).Include(p => p.AppointmentDetail).Where(p => p.CreatedBy == id
                                  && p.OutComeId == filterOption.Status
                                  //&& p.AppointmentDetail.Any(r => r.RelationshipManagerId.ToString() == (filterOption.AllocatedTo != "0" ? filterOption.AllocatedTo : r.RelationshipManagerId.ToString()))
                                  && filterOption.DateFilterBy == 1 ? (p.CreatedDate >= filterOption.FromDate && p.CreatedDate <= filterOption.Todate) :
                                                            (p.AppointmentDetail.Any(r => r.AppointmentDateTime >= filterOption.FromDate && r.AppointmentDateTime <= filterOption.Todate))
                                  ).ToListAsync()
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
                                      AllocatedToName = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? _context.UserMaster.AsEnumerable().FirstOrDefault(r => r.UserId == p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId).FirstName : "",
                                      AppointmentDateTime = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppointmentDateTime : (DateTime?)null,

                                  }).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetail: " + id + "-get all Lead by created user");

                return listCallDetail;

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "GetCallDetail: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpGet("GetCallDetailByRM/{id}")]
        public async Task<ActionResult<IEnumerable<CallDetailViewModel>>> GetCallDetailByRM(Guid id)
        {
            try
            {
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
                                      OutComeText = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppoinStatus.Status : "",
                                      AllocatedToName = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? _context.UserMaster.AsEnumerable().FirstOrDefault(r => r.UserId == p.AppointmentDetail.OrderBy(q => q.AppintmentId).AsEnumerable().LastOrDefault().RelationshipManagerId).FirstName : "",
                                      AppointmentDateTime = p.AppointmentDetail != null && p.AppointmentDetail.Count > 0 ? p.AppointmentDetail.OrderBy(q => q.AppintmentId).LastOrDefault().AppointmentDateTime : (DateTime?)null,
                                  }).ToList();

                GenericMethods.Log(LogType.ActivityLog.ToString(), "GetCallDetailByRM: " + id + "-get all Lead by RM");

                return listCallDetail;

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
                callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(p.AppointmentDateTime, GenericMethods.Indian_Zone));
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
                callDetail.AppointmentDetail.ToList().ForEach(p => p.AppointmentDateTime = TimeZoneInfo.ConvertTimeFromUtc(p.AppointmentDateTime, GenericMethods.Indian_Zone));
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
    }
}
