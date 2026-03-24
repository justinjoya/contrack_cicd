using Contrack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Helpers;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Contrack
{
    public class LoginService : CustomException, ILoginService
    {
        public Result result { get; set; } = new Result();

        private readonly ILoginRepository _repo;
        private readonly IHubRepository _hubRepo;

        public LoginService(ILoginRepository repo, IHubRepository hubRepo)
        {
            _repo = repo;
            _hubRepo = hubRepo;
        }

        public void ValidateLogin(LoginUI loginui)
        {
            if (string.IsNullOrEmpty(loginui.UserName) || string.IsNullOrEmpty(loginui.Password))
            {
                result = Common.ErrorMessage("Invalid Username/Password");
                return;
            }

            var loginDto = new LoginDTO
            {
                UserName = loginui.UserName,
                Password = loginui.Password
            };

            Result validationResult = _repo.ValidateLogin(loginDto);

            if (validationResult.ResultId == 1)
            {
                var loginSessionData = new Login
                {
                    login = loginDto,
                    HubInfo = _hubRepo.GetHubByID(loginDto.HubID)
                };
                SessionManager.LoginSession = loginSessionData;
            }

            result = validationResult;
        }

        public UserDTO GetUserById(string encryptedUserId)
        {
            int userId = Common.Decrypt(encryptedUserId);
            return _repo.GetUserByID(userId);
        }

        public List<User> GetUserLoginList(UserFilter filter)
        {
            var userDtoList = _repo.GetUserLoginList(filter);
            return userDtoList.Select(dto => new User
            {
                user = dto,
                menus = new MasterMenus { edit = true }
            }).ToList();
        }

        public Result SaveUser(UserDTO user)
        {
            try
            {
                user.UserID.NumericValue = Common.Decrypt(user.UserID.EncryptedValue);
                user.Type.NumericValue = Common.Decrypt(user.Type.EncryptedValue);
                user.RoleID.NumericValue = Common.Decrypt(user.RoleID.EncryptedValue);

                if (user.Type.NumericValue == 1)
                {
                    user.EntityIDEncryptedList = new List<string> { Common.Encrypt(Common.HubID) };
                }
                if (user.EntityIDEncryptedList.Count == 0)
                {
                    return Common.ErrorMessage("Please select the entity this user belongs to.");
                }
                if (user.UserID.NumericValue == 0)
                {
                    if (string.IsNullOrWhiteSpace(user.Password))
                    {
                        return Common.ErrorMessage("Password is required for new users.");
                    }
                    string saltedHash = Encryption.CreatePasswordSalt(user.Password);
                    if (saltedHash.Contains(":"))
                    {
                        user.Password = saltedHash.Split(':')[1];
                        user.Salt = saltedHash.Split(':')[0];
                    }
                    else
                    {
                        return Common.ErrorMessage("Error creating password hash.");
                    }
                }
                else
                {
                    user.Password = "";
                }
                string entityids = "{" + string.Join(",", user.EntityIDEncryptedList.Select(x => Common.Decrypt(x).ToString()).ToArray()) + "}";
                return _repo.CreateAccount(user, entityids);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return Common.ErrorMessage(ex.Message);
            }
        }

        public Result UpdatePassword(UpdatePasswordDTO update)
        {
            try
            {
                var user = _repo.GetUserByID(Common.Decrypt(update.UserID.EncryptedValue));
                update.Salt = user.Salt;

                string saltedHash = Encryption.CreatePasswordSalt(update.Password, update.Salt);
                if (saltedHash.Contains(":"))
                {
                    update.Password = saltedHash.Split(':')[1];
                    return _repo.UpdatePassword(update);
                }
                return Common.ErrorMessage("Error creating password hash.");
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return Common.ErrorMessage(ex.Message);
            }
        }

        public Result UpdateUserStatus(string encryptedUserId, int status)
        {
            return _repo.UpdateUserStatus(encryptedUserId, status);
        }

        public Result CheckUsernameAvailability(string username)
        {
            return _repo.CheckUsernameAvailability(username);
        }
    }
}