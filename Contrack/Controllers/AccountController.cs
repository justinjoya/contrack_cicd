using Contrack;
using System.Web.Helpers;
using System.Web.Mvc;
namespace Contrack.Controllers
{
    [LogUserActivity(Order = 1)]
    public class AccountController : Controller
    {
        private readonly LoginService _loginService;
        public AccountController()
        {
            // This decides which service to be used
            ILoginRepository repo = new LoginRepository(); // It can be changed to any other implementation of ILoginRepository
            IHubRepository hubrepo = new HubRepository(); // It can be changed to any other implementation of IHubRepository
            _loginService = new LoginService(repo, hubrepo);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginUI login)
        {
            _loginService.ValidateLogin(login);
            if (_loginService.result.ResultId == 1)
            {
                //CookieManager cookie = new CookieManager();
                //cookie.SetCookie("ConnectMVCUserName", login.login.UserName, DateTime.Now.AddDays(7));
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                return View(new Login() { result = _loginService.result });
            }
        }

        [AllowAnonymous]
        public ActionResult SetTimeOffSet(int UTC)
        {
            SessionManager.TimeOffSet = (-1 * UTC).ToString();
            return Json(UTC, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult VerifySession()
        {
            if (SessionManager.LoginSession == null)
                return Json(Common.ErrorMessage("Expired"), JsonRequestBehavior.AllowGet);
            else
                return Json(Common.SuccessMessage("Success"), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Logout()
        {
            CookieManager cookie = new CookieManager();
            SessionManager.LoginSession = null;
            Session.Clear();
            Session.Abandon();
            cookie.DeleletAllCookies();
            return RedirectToAction("Login", "Account");
        }
    }
}