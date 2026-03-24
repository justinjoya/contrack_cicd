using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public static class Codes
    {
        public static string GetCodes(int id, DateTime date, string prefix, string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return prefix + "-CPG-" + date.ToString("yy") + "-" + id.ToString().PadLeft(3, '0');
                else
                    return code; 
            }
            catch (Exception ex)
            {
                return prefix + "-CPG-" + id.ToString();
            }
        }
    }
}