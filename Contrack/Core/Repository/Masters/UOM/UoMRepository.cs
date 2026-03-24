using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class UoMRepository : CustomException, IUoMRepository
    {
        public List<UoMDTO> GetUoMList(string search = "")
        {
            List<UoMDTO> list = new List<UoMDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable($"SELECT * FROM procurement.get_uom('{Common.HubID}');");

                    list = (from DataRow dr in tbl.Rows
                            select new UoMDTO
                            {

                                uomid = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["uomid"])),
                                    NumericValue = Common.ToInt(dr["uomid"]),
                                },
                                uomname = Common.ToString(dr["uomname"])
                            }).ToList();

                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        search = search.ToLower();
                        list = list.Where(x => x.uomname.ToLower().Contains(search)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public Result SaveUoM(UoMDTO dto)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string[] uomArray = Common.SplitString(dto.uomname, new string[] { ",", "\n", "\r" });

                    string query = "SELECT * FROM procurement.save_uom(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_uomname := ARRAY['" + string.Join("','", uomArray) + "']::varchar[]" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Save Unit of Measure.");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("Error while saving UOM: " + ex.Message);
            }
            return result;
        }

        public Result DeleteUoM(int uomid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = $"SELECT * FROM procurement.delete_uom('{uomid}');";
                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Delete Unit of Measure.");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("Error while deleting UOM: " + ex.Message);
            }
            return result;
        }
    }
}
