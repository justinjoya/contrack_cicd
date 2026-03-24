using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace Contrack
{
    public class NavigationStackManager
    {
        private static List<string> GetQuoteListUrl()
        {
            List<string> URLs = new List<string>();
            URLs.Add("Dashboard/Index");
            URLs.Add("Booking/QuotationList");
            URLs.Add("Booking/BookingQuotationList");
            return URLs;
        }
        //private static List<string> GetVendorInvoiceListUrl()
        //{
        //    List<string> URLs = new List<string>();
        //    URLs.Add("Dashboard/Index");
        //    URLs.Add("VendorInvoice/PIVIList");
        //    URLs.Add("VendorInvoice/VIList");
        //    return URLs;
        //}
        //private static List<string> GetBatchListUrl()
        //{
        //    List<string> URLs = new List<string>();
        //    URLs.Add("Dashboard/Index");
        //    URLs.Add("Batch/BatchList");
        //    return URLs;
        //}
        public static string GetListURL(string type)
        {
            string outputurl = "";
            var stack = SessionManager.NavigationStack;
            if (stack != null)
            {
                List<string> URLs = new List<string>();
                switch (type)
                {
                    case ChatType.Quotation:
                        URLs = GetQuoteListUrl();
                        break;
                    default:
                        break;
                }

                outputurl = stack.FirstOrDefault(stackUrl =>
                URLs.Any(partial => stackUrl.ToLower().Contains(partial.ToLower())));

            }
            return outputurl;
        }
        public static void PushUrl(string url)
        {
            try
            {
                var stack = SessionManager.NavigationStack;

                var incomingUri = new Uri(HttpContext.Current.Request.Url, url);
                var incomingPath = incomingUri.AbsolutePath;

                if (stack.Count > 0)
                {
                    var lastUrl = stack.Peek();
                    var lastUri = new Uri(HttpContext.Current.Request.Url, lastUrl);
                    var lastPath = lastUri.AbsolutePath;

                    if (string.Equals(lastPath, incomingPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

                stack.Push(url);
                SessionManager.NavigationStack = stack;
            }
            catch (Exception ex)
            {
            }
        }
        public static string PopUrl()
        {
            try
            {
                var stack = SessionManager.NavigationStack;
                if (stack.Count > 1)
                {
                    stack.Pop();
                    SessionManager.NavigationStack = stack;
                    return stack.Peek();
                }
            }
            catch (Exception)
            { }
            return "/Dashboard/Index";
        }

        public static void Clear()
        {
            try
            {
                SessionManager.NavigationStack = new Stack<string>();
            }
            catch (Exception ex)
            { }

        }
    }
}