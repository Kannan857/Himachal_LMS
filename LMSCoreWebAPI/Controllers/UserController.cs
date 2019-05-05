using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LMSCoreWebAPI.DTO;
using LMSCoreWebAPI.Helper;
using LMSCoreWebAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using LMSCoreWebAPI.lms;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace LMSCoreWebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly Appsettings _appSettings;
        private lmsContext _context;

        public UserController(
           IUserService userService,
           IMapper mapper,
           IOptions<LMSCoreWebAPI.Helper.Appsettings> appSettings,
           lmsContext context)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserDto userDto)
        {
            var user = _userService.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            Userinstitution userInstitution = user.Userinstitution.FirstOrDefault();
            List<Userrole> userRole = user.Userrole.ToList();
            List<Role> roles = _context.Role.ToList();

            int superAdminRoleId = roles.SingleOrDefault(r => r.RoleName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)).RoleId;
            int adminRoleId = roles.SingleOrDefault(r => r.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)).RoleId;
            int professorRoleId = roles.SingleOrDefault(r => r.RoleName.Equals("Professor", StringComparison.OrdinalIgnoreCase)).RoleId;

            bool isSuperAdminUSer = userRole.Any(r => r.RoleId == superAdminRoleId);
            bool isAdminUSer = userRole.Any(r => r.RoleId == adminRoleId);
            bool isProfessorUSer = userRole.Any(r => r.RoleId == professorRoleId);


            if (userInstitution == null)
            {
                return BadRequest(new { message = "User is associated with any institution" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim("InsId", userInstitution.InstitutionId.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                Id = user.UserId,
                InstitutionId = userInstitution.InstitutionId,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Token = tokenString,
                IsAdminUser = isAdminUSer,
                IsProfessor = isProfessorUSer,
                IsSuperAdminUSer = isSuperAdminUSer
            });
        }

        [AllowAnonymous]
        [HttpGet("SendUserActivationEmail")]
        public IActionResult SendUserActivationEmail(int userId)
        {
            var user = _userService.GetById(userId);

            try
            {

                EmailService.SendUserActivationEmail(_appSettings.EmaiFromAddress, _appSettings.EmailPassword, user);

                return Ok(new
                {
                    Id = user.UserId
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("SendUserActivationEmail")]
        public IActionResult SendResetPasswordEmail(string userName)
        {
            var user = _context.User.Where(u => u.UserName == userName).SingleOrDefault();
            user.ResetPasswordToken = Guid.NewGuid().ToString();

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            try
            {

                EmailService.SendUserActivationEmail(_appSettings.EmaiFromAddress, _appSettings.EmailPassword, user);

                return Ok(new
                {
                    Id = user.UserId
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string userName, string password)
        {
            var user = _context.User.Where(u => u.UserName == userName).SingleOrDefault();
            user.ResetPasswordToken = null;

            try
            {

                User updatedUser = _userService.UpdateUserPassword(user, password);

                return Ok(new
                {
                    Id = updatedUser.UserId
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);

            try
            {
                User userDBObj = _userService.Create(user, userDto.Password, userDto);

                EmailService.SendUserActivationEmail(_appSettings.EmaiFromAddress, _appSettings.EmailPassword, userDBObj);

                return Ok(new
                {
                    Id = userDBObj.UserId
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("activate")]
        public IActionResult Activate(string uniqueId)
        {
            var user = _context.User.Where(u => u.UniqueId == uniqueId).FirstOrDefault();

            if (user == null)
            {
                return BadRequest(new { message = "User dosen't exist in the system" });
            }

            try
            {
                user.IsVerified = 1;
                _context.User.Update(user);
                _context.SaveChanges();

                return Ok(new
                {
                    Id = user.UserId,
                    Message = "Verification Successful"
                });
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("GetAllVerifiedUsers")]
        public IActionResult GetAllVerifiedUsers()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var InstitutionId = claimsIdentity.FindFirst("InsId")?.Value;

            var users = _userService.GetAllVerifiedUsers(int.Parse(InstitutionId));
            var userDtos = _mapper.Map<IList<UserDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            user.UserId = id;

            try
            {
                _userService.Update(user, userDto.Password, userDto.Role, userDto.Department);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}