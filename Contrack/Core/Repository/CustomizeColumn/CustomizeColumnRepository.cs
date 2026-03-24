
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class CustomizeColumRepository : CustomException, ICustomizeColumnRepository
    {
        public List<CustomizeColumnDTO> GetMenus(int menutype)
        {
            List<CustomizeColumnDTO> list = new List<CustomizeColumnDTO>();

            try
            {
                using (SqlDB Db = new SqlDB())
                {

                    DataTable tbl = Db.GetDataTable(
                             "SELECT * From rbac.get_user_menus(" + menutype + ", " + Common.LoginID + "," + Common.HubID + ");");

                    list = (from DataRow dr in tbl.Rows
                            select new CustomizeColumnDTO()
                            {
                                MenuTypeId = Common.ToLong(dr["menutypeid"]),
                                MenuType = Common.ToString(dr["menutype"]),
                                menuid = Common.ToInt(dr["menuid"]),
                                column_name = Common.ToString(dr["column_name"]),
                                UserMenuID = Common.ToLong(dr["usermenuid"]),
                                is_default = Common.ToBool(dr["is_default"]),
                                display_order = Common.ToInt(dr["display_order"]),
                                MappedToUser = Common.ToBool(dr["mapped_to_user"])
                            }).ToList();
                }
            }

            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result Save(CustomizeColumnDTO customizecolumn)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable(
                           "SELECT * FROM rbac.save_user_menus(" +
                           "p_user_id := " + Common.LoginID + ", " +
                           "p_menu_ids :=ARRAY[" + string.Join(",", customizecolumn.menuIds) + "]::bigint[]," +
                           "p_hub_id := " + Common.HubID +
                            ");");

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot Save Menus");
                    }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }
    }
}