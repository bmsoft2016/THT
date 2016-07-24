using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using THT.Model.Models;
using THT.Data.Repositories;
using THT.Data.Infrastructure;
using System.Security.Cryptography;
using THT.Service.Utilities;
using System.Security.Principal;

namespace THT.Service
{
    public interface IMembershipService
    {
        User GetSingleByUserName(string userName);
        string CreateSalt();
        string EncryptPassword(string password, string salt);
        User CreateUser(string username, string email, string password, int[] roles);
        MembershipContext ValidateUser(string username, string password);
        User GetUser(int userId);
        List<Role> GetUserRoles(string username);
        bool isUserValid(User user, string password);
        bool isPasswordValid(User user, string password);
        void addUserToRole(User user, int roleId);

    }
    public class MembershipService : IMembershipService
    {
        private IUserRepository _userRepository;
        private IRoleRepository _roleRepository;
        private IUserRoleRepository _userRoleRepository;
        private IUnitOfWork _unitOfWork;
        public MembershipService(IUserRepository userRepository, IRoleRepository roleReposiotry, IUserRoleRepository userRoleRepository, IUnitOfWork unitOfWork)
        {
            this._userRepository = userRepository;
            this._roleRepository = roleReposiotry;
            this._userRoleRepository = userRoleRepository;
            this._unitOfWork = unitOfWork;
        }
        public User GetSingleByUserName(string userName)
        {
            return _userRepository.GetAll().FirstOrDefault(x => x.UserName == userName);
        }

        public User CreateUser(string username, string email, string password, int[] roles)
        {
            //check user existing
            var userExisting = GetSingleByUserName(username);
            if (userExisting != null)
            {
                throw new Exception("username already existing");
            }
            var passwordSalt = CreateSalt();

            var user = new User()
            {
                UserName = username,
                Salt = passwordSalt,
                Email = email,
                IsLocked = false,
                HashedPassword = EncryptPassword(password, passwordSalt),
                CreatedDate = DateTime.Now
            };

            _userRepository.Add(user);

            _unitOfWork.Commit();

            if (roles != null || roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    addUserToRole(user, role);
                }
            }

            _unitOfWork.Commit();

            return user;
        }

        public string CreateSalt()
        {
            var data = new byte[0x10];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                cryptoServiceProvider.GetBytes(data);
                return Convert.ToBase64String(data);
            }
        }

        public string EncryptPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = string.Format("{0}{1}", salt, password);
                byte[] saltedPasswordAsBytes = Encoding.UTF8.GetBytes(saltedPassword);
                return Convert.ToBase64String(sha256.ComputeHash(saltedPasswordAsBytes));
            }
        }

        public MembershipContext ValidateUser(string username, string password)
        {
            var membershipCtx = new MembershipContext();

            var user = GetSingleByUserName(username);
            if (user != null && isUserValid(user, password))
            {
                var userRoles = GetUserRoles(user.UserName);
                membershipCtx.User = user;

                var identity = new GenericIdentity(user.UserName);
                membershipCtx.Principal = new GenericPrincipal(
                    identity,
                    userRoles.Select(x => x.Name).ToArray());
            }

            return membershipCtx;
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetSingleById(userId);
        }

        public List<Role> GetUserRoles(string username)
        {
            List<Role> _result = new List<Role>();

            var existingUser = GetSingleByUserName(username);

            if (existingUser != null)
            {
                foreach (var userRole in existingUser.UserRoles)
                {
                    _result.Add(userRole.Role);
                }
            }

            return _result.Distinct().ToList();
        }

        public bool isUserValid(User user, string password)
        {
            if (isPasswordValid(user, password))
            {
                return !user.IsLocked;
            }

            return false;
        }

        public bool isPasswordValid(User user, string password)
        {
            return string.Equals(EncryptPassword(password, user.Salt), user.HashedPassword);
        }

        public void addUserToRole(User user, int roleId)
        {
            var role = _roleRepository.GetSingleById(roleId);
            if (role == null)
                throw new ApplicationException("Role doesn't exist.");

            var userRole = new UserRole()
            {
                RoleId = role.ID,
                UserId = user.ID
            };
            _userRoleRepository.Add(userRole);
        }
    }
}
