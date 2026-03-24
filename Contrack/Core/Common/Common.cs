using Contrack;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Contrack
{

    public class TableCounts
    {
        public int totalnoofrows { get; set; } = 0;
        public int row_index { get; set; } = 0;
    }
    public class FormatDisplay
    {
        public string Text { get; set; }
        public string Delimiter { get; set; }
        public string Name { get; set; }
    }
    public class dropdown
    {
        public string id { get; set; }
        public string text { get; set; }
    }
    public class EncryptedData
    {
        public int NumericValue { get; set; } = 0;
        public string EncryptedValue { get; set; } = "";
    }
    public class KeyValuePair
    {
        public string UUID { get; set; } = Guid.NewGuid().ToString();
        public string KeyName { get; set; } = "";
        public string KeyValue { get; set; } = "";
    }
    public class NotFound
    {
        public string PageName { get; set; }
        public string Message { get; set; }
        public string ReturnURL { get; set; }
        public string ReturnButton { get; set; }
    }
    public class Result
    {
        public int ResultId { get; set; }
        public string ResultMessage { get; set; }
        public string TargetUUID { get; set; } = "";
        public int TargetID { get; set; } = 0;
    }
    public class LogResult
    {
        public string LogChangeKey { get; set; } // -- Modification
        public string FieldName { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public int Type { get; set; } // 1 - Added, 2- Edited
    }
    public class LogNames
    {
        public int DetailID { get; set; } = 0;
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
    }
    public class Roles
    {
        public int role_id { get; set; } = 0;
        public string role_name { get; set; } = "";
        public string description { get; set; } = "";
        public string icon { get; set; } = "";

        public List<Roles> GetRolesList()
        {
            var list = new List<Roles>();
            try
            {
                using (var Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.roles WHERE hubid='" + Common.HubID + "' AND is_active = true;");
                    list = (from DataRow dr in tbl.Rows
                            select new Roles()
                            {
                                role_id = Common.ToInt(dr["role_id"]),
                                role_name = Common.ToString(dr["role_name"]),
                                description = Common.ToString(dr["description"]),
                                icon = Common.ToString(dr["icon"]),
                            }).ToList();
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
    }
    public static class Common
    {
        public const string AESKey = "4F67A92B13D4E9FC1A7B8C3D5E6F0123";
        public const string AESIV = "A1B2C3D4E5F60789";
        public const string DBDateTimeformat = "yyyy-MM-dd HH:mm";
        public const string DBDateformat = "yyyy-MM-dd";
        public const string HumanDateTimeformat = "MMM dd, yyyy h:mm tt";
        public const string HumanDateformat = "MMM dd, yyyy";
        public const decimal CurrencyAdjustThreshold = -1;
        public const string VesselName = "Ship / Cost center";
        public const int MyAppID = 2;

        private static readonly (string Color, string Bg)[] ColorList = {
            ("#872094", "#FCE7FF"), // Purple bg → white text
            ("#944920", "#FFEADE"), // Brown bg → white text
            ("#217F1A", "#E6FFE4"), // Green bg → white text
            ("#205C94", "#DEF0FF")  // Blue bg → white text
        };
        public static (string Color, string Bg) GetColorFromName(string name)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                    return ("#808080", "#FFFFFF"); // default gray bg, white text

                name = name.Trim().ToUpper();

                int hash = 0;
                foreach (char c in name)
                {
                    hash = (hash * 31 + c) % 1000;
                }

                int index = hash % ColorList.Length;
                return ColorList[index];
            }
            catch (Exception)
            {
                return ColorList[0];
            }
        }

        public static string FlagFolder
        {
            get
            {
                return "/assets/Flags/";
            }
        }

        public static string IconFolder
        {
            get
            {
                return "/assets/dbicons/";
            }
        }

        public static string DocumentsFolder
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("DocumentsFolder");
            }
        }
        public static string LogPath
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("LogPath");
            }
        }

        public static string AttachmentFolder
        {
            get
            {
                return ConfigurationManager.AppSettings.Get("AttachmentFolder");
            }
        }
        public static string IPAddress
        {
            get
            {
                try
                {
                    return HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }
                catch (Exception ex)
                {
                    return "";
                }
            }
        }
        public static Result SuccessMessage(string message)
        {
            return new Result { ResultId = 1, ResultMessage = message };
        }

        public static Result ErrorMessage(string message)
        {
            return new Result { ResultId = 0, ResultMessage = message };
        }
        public static string Escape(string input)
        {
            if (input == null)
                input = "";
            input = input.Replace("'", "''");
            return input;
        }

        public static string GetUUID(string input)
        {
            return (string.IsNullOrEmpty(input) ? "null" : ("'" + input + "'"));
        }

        public static string GetNullDate(DateTime date)
        {
            return (date == DateTime.MinValue ? "null" : ("'" + date.ToString("yyyy-MM-dd hh:mm:ss") + "'"));
        }

        public static string GetNullValue(string input)
        {
            return (string.IsNullOrEmpty(input) ? "null" : ("'" + input + "'"));
        }


        public static string DisplayInfoTableWithAddEdit(string heading, string value, string href, string onclick, bool noaction = false)
        {
            string output = "<td class='py-3 master-detail-heading'>" + heading + "</td>";
            if (string.IsNullOrEmpty(value))
                output += "<td class='py-3 text-gray-700 text-2sm font-medium'>-</td>";
            else
                output += "<td class='py-3 text-gray-700 text-sm font-medium'>" + value + "</td>";

            if (!noaction)
            {
                if (string.IsNullOrEmpty(value))
                    output += "<td class='py-3 text-right'><a class='btn btn-link btn-sm' href='" + href + "' onclick='" + onclick + "'>Add</a></td>";
                else
                    output += "<td class='py-3 text-right'><a class='btn btn-sm btn-icon btn-clear btn-primary' href='" + href + "' onclick='" + onclick + "'><i class='ki-filled ki-notepad-edit'></i></a></td>";
            }
            else
            {
                output += "<td class='py-3 text-right'><a class='btn btn-sm btn-icon' href='javascript:void(0)'><i></i></a></td>";
            }

            return output;
        }





        public static string DisplayInfoTableWithAddEditDelete(string heading, string value, string href, string onclick, string ondelete, bool noaction = false)
        {
            string output = "<td class='py-3 master-detail-heading'>" + heading + "</td>";
            if (string.IsNullOrEmpty(value))
                output += "<td class='py-3 text-gray-700 text-2sm font-medium'>-</td>";
            else
                output += "<td class='py-3 text-gray-700 text-sm font-medium'>" + value + "</td>";

            if (!noaction)
            {
                if (string.IsNullOrEmpty(value))
                    output += "<td class='py-3 text-right'><a class='btn btn-link btn-sm' href='" + href + "' onclick='" + onclick + "'>Add</a></td>";
                else
                    output += "<td class='py-3 text-right'><a class='btn btn-sm btn-icon btn-clear btn-primary' href='" + href + "' onclick='" + onclick + "'><i class='ki-filled ki-notepad-edit'></i></a><a class='btn btn-sm btn-icon btn-clear btn-danger' href='javascript:void(0)' onclick='" + ondelete + "'><i class='ki-filled ki-filled ki-trash'></i></a></td>";
            }
            else
            {
                output += "<td class='py-3 text-right'><a class='btn btn-sm btn-icon' href='javascript:void(0)'><i></i></a></td>";
            }
            return output;
        }

        public static string LoginName
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToString(SessionManager.LoginSession.login.Name);
                else
                    return "";
            }
        }
        public static string UserName
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToString(SessionManager.LoginSession.login.UserName);
                else
                    return "";
            }
        }
        public static string Email
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToString(SessionManager.LoginSession.login.Email);
                else
                    return "";
            }
        }
        public static int LoginID
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToInt32(SessionManager.LoginSession.login.UserID.NumericValue);
                else
                    return 0;
            }
        }

        public static int HubID
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToInt32(SessionManager.LoginSession.login.HubID);
                else
                {
                    string hubid = ToString(HttpContext.Current.Request["HubID"]);
                    if (string.IsNullOrEmpty(hubid))
                        return 0;
                    else
                        return Decrypt(hubid);
                }

            }
        }

        public static string HubName
        {
            get
            {
                if (SessionManager.LoginSession != null)
                {
                    try
                    {
                        return ToString(SessionManager.LoginSession.HubInfo.hubname);
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                else
                    return "";
            }
        }

        public static int UserType
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToInt32(SessionManager.LoginSession.login.Type.NumericValue);
                else
                    return 0;
            }
        }
        public static int RoleID
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return Convert.ToInt32(SessionManager.LoginSession.login.RoleID.NumericValue);
                else
                    return 0;
            }
        }
        public static string RoleName
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return SessionManager.LoginSession.login.RoleName;
                else
                    return "";
            }
        }
        public static string RoleIcon
        {
            get
            {
                if (SessionManager.LoginSession != null)
                    return SessionManager.LoginSession.login.RoleIcon;
                else
                    return "";
            }
        }
        public static int ToInt(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? 0 : Convert.ToInt32(data);
            }
            catch (Exception ex)
            {
                return 0;

            }
        }

        public static long ToLong(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? 0 : Convert.ToInt64(data);
            }
            catch (Exception ex)
            {
                return 0;

            }
        }

        public static string ToString(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? "" : Convert.ToString(data);
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string GetOperatorName(int operatorId)
        {
            switch (operatorId)
            {
                case 1:
                    return "Owned";
                case 2:
                    return "Leased";
                case 3:
                    return "SOC";
                default:
                    return "COC";
            }
        }
        public static bool ToBool(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? false : Convert.ToBoolean(data);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string[] ToStringArray(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? new string[] { } : (string[])data;
            }
            catch (Exception ex)
            {
                return new string[] { };
            }
        }

        public static string[] ToGUIDStringArray(object data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.ToString()))
                    return Array.Empty<string>();

                // If it's already a string[]
                if (data is string[] stringArray)
                    return stringArray;

                // If it's a Guid[]
                if (data is Guid[] guidArray)
                    return guidArray.Select(g => g.ToString()).ToArray();

                // If it's a single Guid
                if (data is Guid guidValue)
                    return new[] { guidValue.ToString() };

                // If it's a single string
                if (data is string str)
                    return new[] { str };

                return Array.Empty<string>();
            }
            catch
            {
                return Array.Empty<string>();
            }
        }
        public static int[] ToIntArray(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? new int[] { } : (int[])data;
            }
            catch (Exception ex)
            {
                return new int[] { };
            }
        }
        public static Int64[] ToInt64Array(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? new Int64[] { } : (Int64[])data;
            }
            catch (Exception ex)
            {
                return new Int64[] { };
            }
        }

        public static int[] ToInt32Array(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? new int[] { } : (int[])data;
            }
            catch (Exception ex)
            {
                return new int[] { };
            }
        }


        public static DateTime ToDateTime(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? DateTime.MinValue : Convert.ToDateTime(data);
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        public static DateTime ToClientDateTime(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? DateTime.MinValue : Convert.ToDateTime(data).AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }
        public static int GetETADays(DateTime date, bool dateonly = true)
        {
            try
            {
                var currentdate = Display.GetClientDateTime(DateTime.UtcNow);
                if (dateonly)
                    currentdate = currentdate.Date;

                var datediff = (Convert.ToDateTime(date) - currentdate).Days;
                return datediff;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        public static string ToDateTimeString(object data, string format)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(data)))
                    return "";
                else
                    return Convert.ToDateTime(data).ToString(format);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static decimal ToDecimal(object data)
        {
            try
            {
                return string.IsNullOrEmpty(Convert.ToString(data)) ? 0 : Convert.ToDecimal(data);
            }
            catch (Exception ex)
            {
                return 0;

            }
        }

        public static DateTime ToDateTimeHumanFriendly(object data)
        {
            try
            {
                //data = "April 25, 2025 11:55 PM";
                CultureInfo provider = CultureInfo.InvariantCulture;

                DateTime result = DateTime.ParseExact(ToString(data), HumanDateTimeformat, provider);
                return result;
            }
            catch (Exception ex)
            {
                return new DateTime();

            }
        }

        public static DateTime GetCurrentDateTime()
        {
            try
            {
                DateTime date = DateTime.UtcNow;
                date = date.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
                return date;
            }
            catch (Exception ex)
            {
                return new DateTime();
            }
        }
        public static string HumanFriendlyCurrentDateTime()
        {
            try
            {
                DateTime date = DateTime.UtcNow;
                date = date.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
                return date.ToString(HumanDateTimeformat);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string Encrypt(int ID)
        {
            string output = "";
            try
            {
                byte[] bytes = Encryption.Encrypt(ID.ToString(), Encoding.ASCII.GetBytes(AESKey), Encoding.ASCII.GetBytes(AESIV));

                StringBuilder hex = new StringBuilder(bytes.Length * 2);
                foreach (byte b in bytes)
                    hex.AppendFormat("{0:x2}", b);

                return hex.ToString();

                //string EncryptedID = Convert.ToBase64String(Encryption.Encrypt(ID.ToString(), Encoding.ASCII.GetBytes(AESKey), Encoding.ASCII.GetBytes(AESIV)));
                //output = EncryptedID.Replace('+', '.').Replace('/', '_').Replace('=', '-');
            }
            catch (Exception ex)
            { }
            return output;
        }

        public static int Decrypt(string data)
        {
            int output = 0;
            try
            {
                if (string.IsNullOrEmpty(data))
                    return output;
                //data = data.Replace('.', '+').Replace('_', '/').Replace('-', '=').Replace(" ", "+");
                //output = Convert.ToInt32(Encryption.Decrypt(Convert.FromBase64String(data), Encoding.ASCII.GetBytes(AESKey), Encoding.ASCII.GetBytes(AESIV)));
                int length = data.Length / 2;
                byte[] bytes = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    bytes[i] = Convert.ToByte(data.Substring(i * 2, 2), 16);
                }
                output = Convert.ToInt32(Encryption.Decrypt(bytes, Encoding.ASCII.GetBytes(AESKey), Encoding.ASCII.GetBytes(AESIV)));
            }
            catch (Exception ex)
            { }
            return output;
        }

        public static string FormatNumberDecimal(long num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##k");
            }
            return num.ToString("0.00");
        }
        public static string FormatNumberInteger(long num)
        {
            if (num >= 100000000)
            {
                return (num / 1000000D).ToString("0.#M");
            }
            if (num >= 1000000)
            {
                return (num / 1000000D).ToString("0.##M");
            }
            if (num >= 100000)
            {
                return (num / 1000D).ToString("0.#k");
            }
            if (num >= 10000)
            {
                return (num / 1000D).ToString("0.##k");
            }
            return num.ToString("0");
        }

        public static string GetShortcode(string name)
        {
            string output = "";
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    name = name.Replace("  ", " ").Trim();
                    var splitArray = name.Split(' ');
                    var splitArrayDot = name.Split('.');

                    if (splitArray.Length > 1)
                    {
                        output = splitArray[0].Substring(0, 1) + splitArray[1].Substring(0, 1);
                    }
                    else if (splitArrayDot.Length > 1)
                    {
                        if (!string.IsNullOrEmpty(splitArrayDot[1]))
                        {
                            output = splitArrayDot[0].Substring(0, 1) + splitArrayDot[1].Substring(0, 1);
                        }
                        else
                        {
                            output = splitArrayDot[0].Length > 1
                                ? splitArrayDot[0].Substring(0, 2)
                                : splitArrayDot[0].Substring(0, 1);
                        }
                    }
                    else
                    {
                        output = name.Length > 1 ? name.Substring(0, 2) : name.Substring(0, 1);
                    }
                }
            }
            catch (Exception e)
            { }
            return output.ToUpper();
        }
        public static string[] SplitString(string data, string[] delimiter)
        {
            if (data == null) return new string[] { };

            var str = ToString(data);
            if (string.IsNullOrWhiteSpace(str)) return new string[] { };

            //var delimiters = new[] { ',', '\n', '\r' };

            return str
                .Split(delimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();
        }

        public static T ConvertJson<T>(string input)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(input);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static List<ListItem> AlterDropdown(List<ListItem> list, string selectedvalue, string selectedtext)
        {
            try
            {
                if (!string.IsNullOrEmpty(selectedvalue))
                {
                    if (Decrypt(selectedvalue) != 0)
                    {
                        var selected = list.Where(x => x.Value == selectedvalue).FirstOrDefault();
                        if (selected == null)
                        {
                            list.Add(new ListItem()
                            {
                                Value = selectedvalue,
                                Text = selectedtext,
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
            return list;
        }
        public static List<ListItem> AlterUOM(List<ListItem> uomlist, string selecteduom = "")
        {
            if (!string.IsNullOrEmpty(selecteduom))
            {
                var matcheduom = uomlist.Where(x => x.Value.ToUpper() == selecteduom.ToUpper()).FirstOrDefault();
                if (matcheduom == null)
                {
                    uomlist.Add(new System.Web.UI.WebControls.ListItem()
                    {
                        Text = selecteduom,
                        Value = selecteduom
                    });
                }
            }
            return uomlist;
        }
        public static IconDTO GetIcon(int iconid)
        {
            List<IconDTO> iconlist = SessionManager.Icons;
            if (iconlist == null)
            {
                ICommonRepository repo = new CommonRepository();
                iconlist = repo.GetIcons();
                SessionManager.Icons = iconlist;
            }
            return iconlist.Where(x => x.IconId == iconid).FirstOrDefault() ?? new IconDTO();
        }
        public static string GetIconPath(string iconfile)
        {
            return IconFolder + iconfile;
        }
        public static string GetIconPath(int iconid, bool filenameonly = false)
        {
            var icon = GetIcon(iconid);
            string iconfile = icon.icon;
            return icon.iscss ? iconfile : (filenameonly ? (IconFolder + iconfile) : "<img class='dynamicicon' src='" + (IconFolder + iconfile) + "'/>");
        }
        public static string GetSelectedIconPath(int iconid, bool filenameonly = false)
        {
            var icon = GetIcon(iconid);
            string iconfile = icon.iconselected;
            return icon.iscss ? iconfile : (filenameonly ? (IconFolder + iconfile) : "<img class='dynamicicon' src='" + (IconFolder + iconfile) + "'/>");
        }

        public static string BuildContactLine(string email, string phone)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(email))
                parts.Add(email);
            if (!string.IsNullOrWhiteSpace(phone))
                parts.Add("Ph: " + phone);
            return string.Join(" | ", parts);
        }

        public static string BuildMultilineText(IEnumerable<string> lines)
        {
            if (lines == null)
                return string.Empty;

            return string.Join(
                Environment.NewLine,
                lines.Where(l => !string.IsNullOrWhiteSpace(l))
            );
        }
        public static List<ListItem> EmptyDropdown(bool showselect = false)
        {
            List<ListItem> result = new List<ListItem>();
            if (showselect)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetCurrencyDropdown()
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable("Select * from masters.currency where COALESCE(isdeleted,false)=false;");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = ToString(dr["currencycode"]) + " - " + ToString(dr["currencyname"]),
                              Value = ToString(dr["currencycode"]).Trim()
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetUOMDropdown()
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("select * from procurement.uom where hubid='" + HubID.ToString() + "'");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = ToString(dr["uomname"]),
                              Value = ToString(dr["uomname"])
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetJobTypeDetailDropdown(string jobtypeid, bool showmisc = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("select * from procurement.get_jobtype_det('" + HubID.ToString() + "','" + jobtypeid + "')");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = ToString(dr["description"]),
                              Value = ToString(dr["jobtypedetailuuid"])
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            if (showmisc)
                result.Add(new ListItem() { Text = "Miscellaneous", Value = "-1" });
            return result;
        }
        public static void SetTabselection(int status, int type)
        {
            var stats = SessionManager.AllStatsSelected;
            if (type == 1)
                stats.PIList = status;
            else if (type == 11)
                stats.PIListSubTab = status;
            else if (type == 2)
                stats.POList = status;
            else if (type == 3)
                stats.QuoteComparisonList = status;
            else if (type == 4)
                stats.VendorInvoiceList = status;
            else if (type == 12)
                stats.VendorInvoiceListSub = status;
            else if (type == 5)
                stats.BatchVendorInvoiceList = status;
            else if (type == 6)
                stats.Dashboard = status;
            else if (type == 7)
                stats.DashboardVIHOD = status;
            else if (type == 8)
                stats.DashboardQuoteTech = status;
            else if (type == 9)
                stats.DashboardQuoteHOD = status;
            else if (type == 10)
                stats.DashboardBatch = status;
            else if (type == 13)
                stats.InvoiceList = status;
            else if (type == 14)
                stats.InvoiceListSub = status;

            SessionManager.AllStatsSelected = stats;
        }
        public static List<QuotationDetailDTO> PopulateQuotationItemsFromPricing(PricingTypeDTO pricinglist)
        {
            List<QuotationDetailDTO> list = new List<QuotationDetailDTO>();
            if (pricinglist.Details.Count > 0)
            {
                list = pricinglist.Details.Select(x => new QuotationDetailDTO
                {
                    description = x.LineItemDesc,
                    qty = x.Qty,
                    uom = x.UOM,
                    templateprice = ToDecimal(x.Amount.ToString("0.00")),
                    amount = ToDecimal(x.Amount.ToString("0.00")),
                    tax = 0,
                    containertypeid = x.ContainerTypeID,
                    containersizeid = x.ContainerSizeID,
                    TypeSizeCombinedValue = x.ContainerTypeID.EncryptedValue + "@@" + x.ContainerSizeID.EncryptedValue,
                    TypeSizeCombinedText = x.TypeSizeCombined
                }).ToList();

                list.OrderBy(x => x.containertypeid.NumericValue).ThenBy(x => x.containertypeid.NumericValue).ToList();
            }
            return list;
        }

        public static void WriteLog(string log)
        {
            //return;
            try
            {
                string FilePath = LogPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                System.IO.File.AppendAllText(FilePath, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " : " + log + "\n");
            }
            catch (Exception ex)
            {
            }
        }


    }
    public static class Constants
    {
        public static string DateFormat = "dd'/'MM'/'yyyy";
        public static string DateTimeFormat = "dd'/'MM'/'yyyy hh:mm:ss tt";
        public static string DateFormatInvoice = "dd'-'MMM'-'yyyy";
    }
    public class AllStats
    {
        public int VendorInvoiceList { get; set; } = 0;
        public int VendorInvoiceListSub { get; set; } = 0;
        public int BatchVendorInvoiceList { get; set; } = 0;
        public int PIList { get; set; } = 0;
        public int PIListSubTab { get; set; } = 0;
        public int POList { get; set; } = 0;
        public int QuoteComparisonList { get; set; } = 0;
        public int Dashboard { get; set; } = 0;
        public int DashboardQuoteTech { get; set; } = 0;
        public int DashboardQuoteHOD { get; set; } = 0;
        public int DashboardVIHOD { get; set; } = 0;
        public int DashboardBatch { get; set; } = 0;
        public int InvoiceList { get; set; } = 0;
        public int InvoiceListSub { get; set; } = 0;
    }
    public class EmptyTable
    {
        public string Heading { get; set; }
        public string SubHead { get; set; }
        public string Icon { get; set; }
        public string Button { get; set; }
        public bool NoleftRadius { get; set; } = false;

    }

}