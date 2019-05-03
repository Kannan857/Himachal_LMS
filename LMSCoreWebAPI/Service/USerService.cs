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
        void Update(User user, string password = null);
        void Delete(int id);
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

            var user = _context.User.Include(a => a.Userrole).Include(a=>a.Userinstitution).Include(a=>a.Userdepartment).SingleOrDefault(x => x.UserName == username);

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User;
        }

        public User GetById(int id)
        {
            return _context.User.Find(id);
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

            _context.User.Add(user);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(userDto.Department))
            {
                var dept = _context.Department.Where(r => r.DepartmentName.Equals(userDto.Department, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (dept != null)
                {
                    Userdepartment userDept = new Userdepartment();
                    userDept.UserId = user.UserId;
                    userDept.DepartmentId = dept.DepartmentId;

                    user.Userdepartment = new List<Userdepartment> { userDept };
                }
            }

            if (!string.IsNullOrEmpty(userDto.Role))
            {
                var role = _context.Role.Where(r => r.RoleName.Equals(userDto.Role, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (role != null)
                {
                    Userrole userRole = new Userrole();
                    userRole.RoleId = role.RoleId;
                    userRole.UserId = user.UserId;

                    user.Userrole = new List<Userrole> { userRole };
                }
            }

            var institution = _context.Institution.Where(i => i.InstitutionId == userDto.InstitutionId).FirstOrDefault();

            Userinstitution userIns = new Userinstitution();
            userIns.UserId = user.UserId;
            userIns.InstitutionId = institution.InstitutionId;
            user.Userinstitution = new List<Userinstitution> { userIns };

            _context.User.Update(user);
             _context.SaveChanges();

            return user;
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.User.Find(userParam.UserId);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.UserName != user.UserName)
            {
                // username has changed so check if the new username is already taken
                if (_context.User.Any(x => x.UserName == userParam.UserName))
                    throw new AppException("Username " + userParam.UserName + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.UserName = userParam.UserName;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
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
