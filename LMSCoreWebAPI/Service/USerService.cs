using LMSCoreWebAPI.DTO;
using LMSCoreWebAPI.Helper;
using LMSCoreWebAPI.lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;


namespace LMSCoreWebAPI.Service
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll();
        User GetById(int id);
        User Create(User user, string password, UserDto userDto);
        void Update(User user, string password = null, List<string> departments = null, List<string> roles = null);
        void Delete(int id);

        User UpdateUserPassword(User user, string password);

        IEnumerable<User> GetAllVerifiedUsers(int institutionId);
    }

    public class UserService : IUserService
    {
        private lmsContext _context;

        public UserService(lmsContext context)
        {
            _context = context;
        }

        public User Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.User.Include(a => a.Userrole).Include(a => a.Userinstitution).Include(a => a.Userdepartment).SingleOrDefault(x => x.UserName == username && x.IsVerified == 1);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User.Include(a => a.Userrole).Include(a => a.Userinstitution).Include(a => a.Userdepartment);
        }

        public IEnumerable<User> GetAllVerifiedUsers(int institutionId)
        {
            List<User> allUsers = _context.User.Include(a => a.Userrole).Include(a => a.Userinstitution).Include(a => a.Userdepartment).Where(u => u.IsVerified == 1).ToList();
            List<User> verifiedUser = new List<User>();
            if(allUsers != null && allUsers.Count > 0)
            {
                foreach(User user in allUsers)
                {
                    List<Userinstitution> userIns = user.Userinstitution.ToList();
                    if(userIns.Any(u=>u.InstitutionId == institutionId))
                    {
                        verifiedUser.Add(user);
                    }
                }                
            }
            return verifiedUser;
        }

        public User GetById(int id)
        {
            return _context.User.Include(a => a.Userrole).Include(a => a.Userinstitution).Include(a => a.Userdepartment).Where(u => u.UserId == id).SingleOrDefault();
        }

        public User UpdateUserPassword(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();

            return user;
        }

        public User Create(User user, string password, UserDto userDto)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            User userDBObj = _context.User.Where(x => x.UserName == user.UserName).FirstOrDefault();

            if (userDBObj != null)
                throw new AppException("Username \"" + user.UserName + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.UserStatusId = (int)UserStatus.Deactived;
            user.IsVerified = 0;
            user.UniqueId = Guid.NewGuid().ToString();

            _context.User.Add(user);
            _context.SaveChanges();

            if (userDto.Department != null && userDto.Department.Count > 0)
            {
                foreach (string department in userDto.Department)
                {
                    var dept = _context.Department.Where(r => r.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (dept != null)
                    {
                        Userdepartment userDept = new Userdepartment();
                        userDept.UserId = user.UserId;
                        userDept.DepartmentId = dept.DepartmentId;

                        user.Userdepartment = new List<Userdepartment> { userDept };
                    }
                }
            }



            if (userDto.Role != null && userDto.Role.Count > 0)
            {
                foreach (string roleName in userDto.Role)
                {
                    var role = _context.Role.Where(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (role != null)
                    {
                        Userrole userRole = new Userrole();
                        userRole.RoleId = role.RoleId;
                        userRole.UserId = user.UserId;

                        user.Userrole = new List<Userrole> { userRole };
                    }
                }

            }

            Institution institution = null;
            if (userDto.InstitutionId > 0)
            {
                institution = _context.Institution.Where(i => i.InstitutionId == userDto.InstitutionId).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(userDto.InstitutionName))
            {
                institution = _context.Institution.Where(i => i.InstitutionName.Contains(userDto.InstitutionName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(userDto.InstitutionUrl))
            {
                institution = _context.Institution.Where(i => i.InstitutionUrl.Contains(userDto.InstitutionUrl, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }

            Userinstitution userIns = new Userinstitution();
            userIns.UserId = user.UserId;
            userIns.InstitutionId = institution.InstitutionId;
            user.Userinstitution = new List<Userinstitution> { userIns };

            _context.User.Update(user);
            _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null, List<string> departments = null, List<string> roles = null)
        {
            var user = GetById(userParam.UserId);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                // username has changed so check if the new username is already taken
                if (_context.User.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            // update user properties
            user.FirstName = !string.IsNullOrEmpty(userParam.FirstName) ? userParam.FirstName : user.FirstName;
            user.LastName = !string.IsNullOrEmpty(userParam.LastName) ? userParam.LastName : user.LastName;
            user.UserName = !string.IsNullOrEmpty(userParam.UserName) ? userParam.UserName : user.UserName;
            user.MobileNumber = userParam.MobileNumber.HasValue ? userParam.MobileNumber.Value : user.MobileNumber;
            user.UserStatusId = userParam.UserStatusId.HasValue ? userParam.UserStatusId.Value : user.UserStatusId;
            user.ModfiedDate = System.DateTime.Now;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            if (departments != null && departments.Count > 0)
            {
                foreach (string department in departments)
                {
                    var dept = _context.Department.Where(r => r.DepartmentName.Equals(department, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (dept != null)
                    {
                        Userdepartment userDept = new Userdepartment();
                        userDept.UserId = user.UserId;
                        userDept.DepartmentId = dept.DepartmentId;

                        if (user.Userdepartment == null)
                        {
                            user.Userdepartment = new List<Userdepartment> { userDept };
                        }
                        else if (!user.Userdepartment.Any(d => d.DepartmentId == dept.DepartmentId))
                        {
                            user.Userdepartment.Add(userDept);
                        }
                    }
                }

            }

            if (roles != null && roles.Count > 0)
            {
                foreach (string roleName in roles)
                {
                    var role = _context.Role.Where(r => r.RoleName.Equals(roleName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                    if (role != null)
                    {
                        Userrole userRole = new Userrole();
                        userRole.RoleId = role.RoleId;
                        userRole.UserId = user.UserId;

                        if (user.Userrole == null)
                        {
                            user.Userrole = new List<Userrole> { userRole };
                        }
                        else if (!user.Userrole.Any(r => r.RoleId == role.RoleId))
                        {
                            user.Userrole.Add(userRole);
                        }
                    }
                }

            }

            _context.User.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.User.Find(id);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }
        }

        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
    }
}
