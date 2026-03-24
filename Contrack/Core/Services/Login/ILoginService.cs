using Contrack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ILoginService
    {
        void ValidateLogin(LoginUI login);

        UserDTO GetUserById(string encryptedUserId);
        List<User> GetUserLoginList(UserFilter filter);
        Result SaveUser(UserDTO user);
        Result UpdatePassword(UpdatePasswordDTO update);
        Result UpdateUserStatus(string encryptedUserId, int status);
        Result CheckUsernameAvailability(string username);
    }
}