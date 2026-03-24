using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    class CookieManager
    {
        private int DefaultExpireHours = 48;

        public void CreateCookie(string cookiename, string cookievalue, DateTime? expirydate = null)
        {
            HttpCookie cookie = new HttpCookie(cookiename);
            cookie.Value = cookievalue;
            if (expirydate == null)
                cookie.Expires = DateTime.Now.AddHours(DefaultExpireHours);
            else
                cookie.Expires = expirydate.Value;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public void SetCookie(string cookiename, string cookievalue, DateTime? expirydate = null)
        {
            if (GetCookie(cookiename) == "")
            {
                CreateCookie(cookiename, cookievalue, expirydate);
            }
            else
            {
                HttpCookie cookie = new HttpCookie(cookiename);
                cookie.Value = cookievalue;
                if (expirydate == null)
                    cookie.Expires = DateTime.Now.AddHours(DefaultExpireHours);
                else
                    cookie.Expires = expirydate.Value;
                HttpContext.Current.Response.SetCookie(cookie);
            }
        }

        public string GetCookie(string cookiename)
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[cookiename];
            if (myCookie != null)
            {
                return myCookie.Value;
            }
            return "";
        }

        public bool IsCookiePresent(string cookiename)
        {
            if (GetCookie(cookiename) == "")
                return false;
            else
                return true;
        }

        public void DeleteCookie(string cookiename)
        {
            if (IsCookiePresent(cookiename))
            {
                SetCookie(cookiename, "", DateTime.Now.AddDays(-1));
            }
        }

        public void DeleletAllCookies()
        {
            string[] myCookies = HttpContext.Current.Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                if (cookie.ToLower().Contains("InC2MVC".ToLower()))
                    SetCookie(cookie, "", DateTime.Now.AddDays(-1));
            }
        }
    }
}