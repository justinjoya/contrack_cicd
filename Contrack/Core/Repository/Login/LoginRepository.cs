using Contrack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Security;
using System.Xml.Linq;

namespace Contrack
{
    public class LoginRepository : CustomException, ILoginRepository
    {
        public Result ValidateLogin(LoginDTO login)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.login_validation('" + login.UserName + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        int ResultID = Common.ToInt(tbl.Rows[0]["resultid"]);
                        string ResultMsg = Common.ToString(tbl.Rows[0]["resultmessage"]);
                        if (ResultID == 1)
                        {
                            string DBHash = Common.ToString(tbl.Rows[0]["passwordhash"]);
                            string DBSalt = Common.ToString(tbl.Rows[0]["salt"]);
                            bool isvalid = Encryption.IsPasswordValid(login.Password, DBSalt + ":" + DBHash);
                            if (isvalid)
                            {
                                result = Common.SuccessMessage("Login Success");
                                login.UserID = new EncryptedData { NumericValue = Common.ToInt(tbl.Rows[0]["userid"]) };
                                login.UserName = Common.ToString(tbl.Rows[0]["username"]);
                                login.Name = Common.ToString(tbl.Rows[0]["fullname"]);
                                login.Type = new EncryptedData { NumericValue = Common.ToInt(tbl.Rows[0]["usertype"]) };
                                login.HubID = Common.ToInt(tbl.Rows[0]["hubid"]);
                                login.RoleID = new EncryptedData { NumericValue = Common.ToInt(tbl.Rows[0]["role_id"]) };
                                login.RoleName = Common.ToString(tbl.Rows[0]["role_name"]);
                            }
                            else
                            {
                                result = Common.ErrorMessage("Invalid Username/Password");
                            }
                        }
                        else
                        {
                            result = Common.ErrorMessage(ResultMsg);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Your account doesn't exist");
                    }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message.ToString());
                RecordException(ex);
            }
            return result;
        }
        public List<UserDTO> GetUserLoginList(UserFilter filter)
        {
            var list = new List<UserDTO>();
            try
            {
                using (var Db = new SqlDB())
                {
                    string query = $"SELECT * FROM masters.getuserslist('{Common.HubID}', '{Common.Decrypt(filter.UserType)}', '{Common.Decrypt(filter.Role)}', '{{{(filter.EntityID.Any() ? string.Join(",", filter.EntityID.Select(Common.Decrypt)) : "0")}}}', '{Common.Escape(filter.Search)}', '{filter.limit}', '{filter.offset}', '{{{Common.LoginID}}}', '{filter.sorting}', '{filter.sortingorder}');";
                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select new UserDTO()
                            {
                                row_index = Common.ToInt(dr["row_index"]),
                                totalnoofrows = Common.ToInt(dr["totalnoofrows"]),
                                UserID = new EncryptedData { NumericValue = Common.ToInt(dr["userid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["userid"])) },
                                UserName = Common.ToString(dr["username"]),
                                Name = Common.ToString(dr["fullname"]),
                                Email = Common.ToString(dr["email"]),
                                Phone = Common.ToString(dr["phone"]),
                                Type = new EncryptedData { NumericValue = Common.ToInt(dr["usertypeid"]) },
                                TypeName = Common.ToString(dr["usertype"]),
                                EntityName = Common.ToString(dr["entity_name"]),
                                DateTimeCreated = Common.ToClientDateTime(dr["createdat"]),
                                Status = Common.ToInt(dr["isactive"]),
                                RoleName = Common.ToString(dr["role_names"]),
                                RoleIcon = Common.ToString(dr["role_icons"]),
                            }).ToList();
                    try
                    {
                        list.ForEach(x =>
                        {
                            x.extras.shortcode = Common.GetShortcode(x.Name);
                            var colors = Common.GetColorFromName(x.extras.shortcode);
                            x.extras.color = colors.Color;
                            x.extras.bgcolor = colors.Bg;
                        });
                    }
                    catch (Exception)
                    { }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public UserDTO GetUserByID(int userId)
        {
            var user = new UserDTO();
            try
            {
                using (var Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT * FROM masters.getuserbyid('{Common.HubID}', '{userId}');");
                    if (tbl.Rows.Count > 0)
                    {
                        DataRow row = tbl.Rows[0];
                        user.UserID = new EncryptedData { NumericValue = Common.ToInt(row["userid"]), EncryptedValue = Common.Encrypt(Common.ToInt(row["userid"])) };
                        user.UserName = Common.ToString(row["username"]);
                        user.Name = Common.ToString(row["fullname"]);
                        user.Email = Common.ToString(row["email"]);
                        user.Phone = Common.ToString(row["phone"]);
                        user.Salt = Common.ToString(row["salt"]);
                        user.Password = Common.ToString(row["passwordhash"]);
                        user.Type = new EncryptedData { NumericValue = Common.ToInt(row["usertypeid"]), EncryptedValue = Common.Encrypt(Common.ToInt(row["usertypeid"])) };
                        user.TypeName = Common.ToString(row["usertype"]);
                        user.EntityName = string.Join(",", tbl.AsEnumerable().Select(r => Common.ToString(r["entity_name"])).Distinct());
                        user.EntityIDEncryptedList = tbl.AsEnumerable().Select(r => Common.Encrypt(Common.ToInt(r["entityid"]))).Distinct().ToList();
                        user.DateTimeCreated = Common.ToDateTime(row["createdat"]);
                        user.Status = Common.ToInt(row["isactive"]);
                        user.RoleID = new EncryptedData { NumericValue = Common.ToInt(row["role_id"]), EncryptedValue = Common.Encrypt(Common.ToInt(row["role_id"])) };
                        user.RoleName = Common.ToString(row["role_name"]);
                        user.RoleIcon = Common.ToString(row["role_icon"]);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return user;
        }

        public Result CreateAccount(UserDTO user, string entityIds)
        {
            Result result;
            try
            {
                using (var Db = new SqlDB())
                {
                    string qry = $"SELECT * FROM masters.register_user(" +
                        $"'{user.UserName}', " +
                        $"'{user.Name}', " +
                        $"'{Common.Decrypt(user.UserID.EncryptedValue)}'," +
                        $"'{user.Password}', " +
                        $"'{user.Salt}'," +
                        $"'{user.Email}'," +
                        $"'{user.Phone}'," +
                        $"''," +
                        $"'{Common.HubID}'," +
                        $"'0', " +
                        $"'{Common.Decrypt(user.Type.EncryptedValue)}'," +
                        $"'true', " +
                        $"'{entityIds}', " +
                        $"'{Common.Decrypt(user.RoleID.EncryptedValue)}');";
                    DataTable tbl = Db.GetDataTable(qry);
                    if (tbl.Rows.Count > 0)
                    {
                        result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                    {
                        result = Common.ErrorMessage("Error in Account creation.");
                    }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return result;
        }

        public Result UpdatePassword(UpdatePasswordDTO update)
        {
            Result result;
            try
            {
                using (var Db = new SqlDB())
                {
                    string qry = $"CALL masters.updateuserpassword(" +
                                $"'{Common.HubID}'," +
                                $"'{Common.Decrypt(update.UserID.EncryptedValue)}'," +
                                $"'{update.Password}');";
                    result = Db.ExecuteProcedure(qry);
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return result;
        }

        public Result UpdateUserStatus(string encryptedUserId, int status)
        {
            Result result;
            try
            {
                using (var Db = new SqlDB())
                {
                    string qry = $"CALL masters.UpdateUserStatus('{Common.HubID}', '{Common.Decrypt(encryptedUserId)}', '{status}', 'false');";
                    result = Db.ExecuteProcedure(qry);
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return result;
        }

        public Result CheckUsernameAvailability(string username)
        {
            Result result = Common.ErrorMessage("Username is not available");
            try
            {
                using (var Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT * FROM masters.CheckUsernameExists('{username}');");
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage("Username is available");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return result;
        }
    }
}