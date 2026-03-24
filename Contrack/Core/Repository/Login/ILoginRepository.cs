using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ILoginRepository
    {
        Result ValidateLogin(LoginDTO login);
        UserDTO GetUserByID(int UserID);
        Result UpdatePassword(UpdatePasswordDTO update);
        List<UserDTO> GetUserLoginList(UserFilter filter);
        Result CreateAccount(UserDTO user, string entityIds);
        Result UpdateUserStatus(string encryptedUserId, int status);
        Result CheckUsernameAvailability(string username);
    }
}