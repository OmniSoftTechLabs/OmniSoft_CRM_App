using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OmniCRM_Web.GenericClasses;
using OmniCRM_Web.Models;
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = StringConstant.SuperUser)]
    public class CompanyMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public CompanyMastersController(OmniCRMContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        // GET: api/CompanyMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyMaster>>> GetCompanyMaster()
        {
            return await _context.CompanyMaster.ToListAsync();
        }

        // GET: api/CompanyMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyMaster>> GetCompanyMaster(Guid id)
        {
            var companyMaster = await _context.CompanyMaster.FindAsync(id);

            if (companyMaster == null)
            {
                return NotFound();
            }

            return companyMaster;
        }

        // PUT: api/CompanyMasters/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyMaster(Guid id, CompanyMaster companyMaster)
        {
            if (id != companyMaster.CompanyId)
            {
                return BadRequest();
            }

            _context.Entry(companyMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyMasterExists(id))
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

        // POST: api/CompanyMasters
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult> PostCompanyMaster([FromBody] CompanyMaster companyMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!UserMasterExists(companyMaster.UserMaster.FirstOrDefault().Email))
                    {
                        _context.CompanyMaster.Add(companyMaster);
                        await _context.SaveChangesAsync();

                        #region Email Body for activation

                        string FilePath = _hostingEnvironment.ContentRootPath + "//HTMLTemplate//CreateNewPwd.html";
                        StreamReader str = new StreamReader(FilePath);
                        string MailText = str.ReadToEnd();

                        string domain = _configuration.GetSection("Domains").GetSection("CurrentDomain").Value;
                        //string domain = _configuration.GetValue<string>("Domains:CurrentDomain");
                        MailText = MailText.Replace("#CREATE_PWD_LINK", domain + "/new-pwd/" + companyMaster.UserMaster.FirstOrDefault().UserId);
                        #endregion

                        GenericMethods.SendEmailNotification(companyMaster.UserMaster.FirstOrDefault().Email, "OmniCRM User activation link", MailText);

                        GenericMethods.Log(LogType.ActivityLog.ToString(), "PostCompanyMaster: " + companyMaster.CompanyName + "-Company created successfully");
                        return Ok(StatusCodes.Status200OK);
                    }
                    else
                        return Conflict("Email id already exist!");
                }
                else
                    return BadRequest("Failed to create company!");
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostCompanyMaster: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        private bool UserMasterExists(string email)
        {
            return _context.UserMaster.Any(e => e.Email == email && e.Status == true);
        }

        // DELETE: api/CompanyMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CompanyMaster>> DeleteCompanyMaster(Guid id)
        {
            var companyMaster = await _context.CompanyMaster.FindAsync(id);
            if (companyMaster == null)
            {
                return NotFound();
            }

            _context.CompanyMaster.Remove(companyMaster);
            await _context.SaveChangesAsync();

            return companyMaster;
        }

        private bool CompanyMasterExists(Guid id)
        {
            return _context.CompanyMaster.Any(e => e.CompanyId == id);
        }
    }
}
