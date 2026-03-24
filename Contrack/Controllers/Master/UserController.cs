using Contrack;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    [IsUserLoggedIn(Order = 1)]
    [LogUserActivity(Order = 2)]
    [TrackNavigation(Order = 3)]
    public class UserController : Controller
    {
        private readonly ILoginService _loginService;

        public UserController()
        {
            ILoginRepository repo = new LoginRepository();
            IHubRepository hubRepo = new HubRepository();
            _loginService = new LoginService(repo, hubRepo);
        }

        public ActionResult List()
        {
            var filter = SessionManager.UserListFilter ?? new UserFilter();
            switch (Common.UserType)
            {
                case 1:
                    filter.UserType = Common.Encrypt(SessionManager.UserListTab);
                    break;
                case 2:
                    filter.UserType = Common.Encrypt(2);
                    break;
                default:
                    filter.UserType = Common.Encrypt(10);
                    break;
            }
            SessionManager.UserListFilter = filter;
            return View();
        }

        public ActionResult UsersType(string Type)
        {
            SessionManager.UserListTab = Common.Decrypt(Type);
            return RedirectToAction("List");
        }

        public ActionResult GetUserModal(string userid = "", string type = "", string entityid = "", string entityname = "")
        {
            UserDTO userDto;
            if (!string.IsNullOrEmpty(userid))
            {
                userDto = _loginService.GetUserById(userid);
            }
            else
            {
                userDto = new UserDTO();
                userDto.Type = new EncryptedData { EncryptedValue = type };
                userDto.EntityName = entityname;
                if (!string.IsNullOrEmpty(entityid))
                    userDto.EntityIDEncryptedList.Add(entityid);
            }

            ViewBag.UserTypeDropdown = Dropdowns.GetUserTypeDropdown();
            ViewBag.AgenciesDropdown = Dropdowns.GetAgenciesDropdown();
            ViewBag.RolesDropdown = Dropdowns.GetRoles();
            ViewBag.ClientDropdown = Dropdowns.EmptyDropdown();

            return PartialView("~/Views/Shared/Masters/_ModalLogin.cshtml", userDto);
        }

        public ActionResult OpenChangePassword(string userid)
        {
            UserDTO userDto = _loginService.GetUserById(userid);
            return PartialView("~/Views/Shared/Masters/_MasterChangePassword.cshtml", userDto);
        }

        public JsonResult CheckAvailability(string Username)
        {
            Result result = _loginService.CheckUsernameAvailability(Username);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveUser(UserDTO formData)
        {
            var result = _loginService.SaveUser(formData);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePassword(UpdatePasswordDTO passwordDto)
        {
            var result = _loginService.UpdatePassword(passwordDto);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateUserStatus(string UserIDEncrypted, int Status)
        {
            var result = _loginService.UpdateUserStatus(UserIDEncrypted, Status);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}


