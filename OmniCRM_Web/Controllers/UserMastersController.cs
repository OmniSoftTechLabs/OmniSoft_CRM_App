﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OmniCRM_Web.GenericClasses;
using OmniCRM_Web.Models;
using OmniCRM_Web.ViewModels;
using static OmniCRM_Web.GenericClasses.Enums;

namespace OmniCRM_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserMastersController : ControllerBase
    {
        private readonly OmniCRMContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public UserMastersController(OmniCRMContext context, IHostingEnvironment hostingEnvironment, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _mapper = mapper;
        }

        // GET: api/UserMasters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserMaster>>> GetUserMaster()
        {
            return await _context.UserMaster.ToListAsync();
        }

        // GET: api/UserMasters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserMaster>> GetUserMaster(Guid id)
        {
            try
            {
                var userMaster = await _context.UserMaster.FindAsync(id);
                //bool isVerify = GenericMethods.VerifyPassword("Piyush@123", userMaster.PasswordHash, userMaster.PasswordSalt);

                if (userMaster == null)
                {
                    return NotFound();
                }
                return userMaster;
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostUserMaster: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        // PUT: api/UserMasters/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserMaster(Guid id, UserMaster userMaster)
        {
            if (id != userMaster.UserId)
            {
                return BadRequest();
            }

            _context.Entry(userMaster).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserMasterExists(id))
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

        // POST: api/UserMasters
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult> PostUserMaster([FromBody]UserMaster userMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!UserMasterExists(userMaster.Email))
                    {
                        _context.UserMaster.Add(userMaster);
                        await _context.SaveChangesAsync();
                        GenericMethods.Log(LogType.ActivityLog.ToString(), "PostUserMaster: " + userMaster.Email + "-User created successfully");

                        //CreatedAtAction("GetUserMaster", new { id = userMaster.UserId }, userMaster);

                        #region Email Body for activation

                        string FilePath = _hostingEnvironment.ContentRootPath + "//HTMLTemplate//CreateNewPwd.html";
                        StreamReader str = new StreamReader(FilePath);
                        string MailText = str.ReadToEnd();

                        string domain = _configuration.GetSection("Domains").GetSection("CurrentDomain").Value;
                        //string domain = _configuration.GetValue<string>("Domains:CurrentDomain");
                        MailText = MailText.Replace("#CREATE_PWD_LINK", domain + "/new-pwd/" + userMaster.UserId);
                        #endregion


                        GenericMethods.SendEmailNotification(userMaster.Email, "OmniCRM User activation link", MailText);
                        return Ok(StatusCodes.Status200OK);

                    }
                    else
                        return Conflict("User already exist!");
                }
                else
                    return BadRequest("Failed to create user!");
            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "PostUserMaster: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }

        }

        // DELETE: api/UserMasters/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserMaster>> DeleteUserMaster(Guid id)
        {
            var userMaster = await _context.UserMaster.FindAsync(id);
            if (userMaster == null)
            {
                return NotFound();
            }

            _context.UserMaster.Remove(userMaster);
            await _context.SaveChangesAsync();

            return userMaster;
        }

        private bool UserMasterExists(Guid id)
        {
            return _context.UserMaster.Any(e => e.UserId == id);
        }

        private bool UserMasterExists(string email)
        {
            return _context.UserMaster.Any(e => e.Email == email);
        }

        //POST: api/UserMasters/ResetPassword
        [HttpPost]
        [Route("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] CreatePassword pwdModel)
        {
            try
            {
                var userMaster = await _context.UserMaster.FindAsync(pwdModel.UserId);

                GenericMethods.HashSalt hashSalt = GenericMethods.GenerateHashSalt(pwdModel.Password);
                userMaster.PasswordSalt = hashSalt.saltPassword;
                userMaster.PasswordHash = hashSalt.hashPassword;

                _context.Entry(userMaster).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                GenericMethods.Log(LogType.ActivityLog.ToString(), "ResetPassword: " + userMaster.Email + "-Password created successfully");

            }
            catch (Exception ex)
            {
                GenericMethods.Log(LogType.ErrorLog.ToString(), "ResetPassword: " + ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!UserMasterExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return NoContent();
        }


        [HttpPost]
        [Route("CheckLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckLogin([FromBody] Credentials authModel)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    if (UserMasterExists(authModel.Username))
                    {
                        var userMaster = await _context.UserMaster.FirstOrDefaultAsync(p => p.Email == authModel.Username && p.Status == true);
                        if (userMaster != null)
                            if (GenericMethods.VerifyPassword(authModel.Password, userMaster.PasswordHash, userMaster.PasswordSalt))
                            {
                                userMaster.Role = await _context.RoleMaster.FirstOrDefaultAsync(p => p.RoleId == userMaster.RoleId);
                                GenericMethods.Log(LogType.ActivityLog.ToString(), "CheckLogin: " + authModel.Username + "-login successfull");
                                var objViewUser = _mapper.Map<UserMasterViewModel>(userMaster);
                                var key = _configuration.GetSection("TokenSettings").GetSection("JWT_Secret").Value;
                                var tokenDescriptor = new SecurityTokenDescriptor
                                {
                                    Subject = new ClaimsIdentity(new Claim[]
                                        {
                                            new Claim("UserID",objViewUser.UserId.ToString()),
                                            new Claim(ClaimTypes.Role,objViewUser.Role.RoleName),
                                        }),
                                    Issuer = _configuration.GetSection("TokenSettings").GetSection("Client_URL").Value,
                                    Audience = _configuration.GetSection("TokenSettings").GetSection("Client_URL").Value,

                                    Expires = DateTime.UtcNow.AddHours(6),
                                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
                                };
                                var tokenHandler = new JwtSecurityTokenHandler();
                                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                                var token = tokenHandler.WriteToken(securityToken);

                                objViewUser.Token = token;

                                return Ok(objViewUser);
                            }
                            else
                            {
                                GenericMethods.Log(LogType.ActivityLog.ToString(), "CheckLogin: " + authModel.Username + "-wrong password");
                                return NotFound("Password does not matched!");
                            }
                        else
                        {
                            GenericMethods.Log(LogType.ActivityLog.ToString(), "CheckLogin: " + authModel.Username + "-not active");
                            return NotFound("User is not active!");
                        }
                    }
                    else
                    {
                        GenericMethods.Log(LogType.ActivityLog.ToString(), "CheckLogin: " + authModel.Username + "-not found");
                        return NotFound("User not found!");
                    }
                }
                catch (Exception ex)
                {
                    GenericMethods.Log(LogType.ErrorLog.ToString(), "CheckLogin: " + ex.ToString());
                    return StatusCode(StatusCodes.Status500InternalServerError, ex);
                }
            }

            return this.BadRequest();
        }
    }
}
