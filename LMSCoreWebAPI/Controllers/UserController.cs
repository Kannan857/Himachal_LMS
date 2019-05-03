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

namespace LMSCoreWebAPI.Controllers
{
    //[Authorize]
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

            int adminRoleId = roles.SingleOrDefault(r => r.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)).RoleId;
            int professorRoleId = roles.SingleOrDefault(r => r.RoleName.Equals("Professor", StringComparison.OrdinalIgnoreCase)).RoleId;

            bool isAdminUSer = userRole.Any(r=>r.RoleId == adminRoleId);
            bool isProfessorUSer = userRole.Any(r=>r.RoleId == professorRoleId);


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
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
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
                IsProfessor = isProfessorUSer
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]UserDto userDto)
        {
            if (string.IsNullOrEmpty(userDto.Password))
            {
                userDto.Password = "!Password1";
            }
            var user = _mapper.Map<User>(userDto);

            try
            {
                User userDBObj = _userService.Create(user, userDto.Password, userDto);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
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
            // map dto to entity and set id
            var user = _mapper.Map<User>(userDto);
            user.UserId = id;

            try
            {
                // save 
                _userService.Update(user, userDto.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
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