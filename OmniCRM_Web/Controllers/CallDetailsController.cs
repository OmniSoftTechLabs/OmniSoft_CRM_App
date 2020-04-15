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
    [Authorize(Roles = StringConstant.SuperUser + "," + StringConstant.TeleCaller)]

    public class CallDetailsController : ControllerBase
    {
        private readonly OmniCRMContext _context;

        public CallDetailsController(OmniCRMContext context)
        {
            _context = context;
        }

        // GET: api/CallDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CallDetail>>> GetCallDetail()
        {
            return await _context.CallDetail.ToListAsync();
        }

        // GET: api/CallDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CallDetail>> GetCallDetail(int id)
        {
            var callDetail = await _context.CallDetail.FindAsync(id);

            if (callDetail == null)
            {
                return NotFound();
            }

            return callDetail;
        }

        // PUT: api/CallDetails/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCallDetail(int id, CallDetail callDetail)
        {
            if (id != callDetail.CallId)
            {
                return BadRequest();
            }

            _context.Entry(callDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CallDetailExists(id))
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
