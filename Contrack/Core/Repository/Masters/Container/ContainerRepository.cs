using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class ContainerRepository : CustomException, IContainerRepository
    {
        public Result SaveContainer(ContainerDTO container)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = "SELECT * FROM masters.container_equip_save(" +
                                   "p_containerid := " + Common.Decrypt(container.containerid.EncryptedValue) + "," +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_equipmentno := '" + Common.Escape(Common.ToString(container.equipmentno)) + "'," +
                                   "p_containermodeluuid := " + Common.GetUUID(container.containermodeluuid) + "::uuid," +
                                   "p_operator := " + Common.Decrypt(container.operatorid.EncryptedValue) + "," +
                                   "p_locationdetailid := " + Common.Decrypt(container.currentlocationid.EncryptedValue) + "," +
                                   "p_islive := " + (container.islive ? "true" : "false") + "," +
                                    "p_manufacturedate := " + Common.GetNullDate(container.manufacturedate.Value) + "," +
                                  "p_userid := " + Common.LoginID + "," +
                                  "p_isempty := " + (container.is_empty == true ? "true" : "false") + "" +
               ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0]["resultid"].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0]["resultmessage"].ToString());
                            if (tbl.Columns.Contains("primaryid"))
                                result.TargetID = Common.ToInt(tbl.Rows[0]["primaryid"]);
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0]["resultmessage"].ToString());
                    }
                    else
                        result = Common.ErrorMessage("No response from database.");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage("System Error: " + ex.Message);
                RecordException(ex);
            }
            return result;
        }
        public bool IsContainerAvailable(string containerid)
        {
            bool result = true;
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.container_equip_available_check('" + Common.Decrypt(containerid) + "','" + Common.HubID + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        int count = Common.ToInt(tbl.Rows[0][0]);
                        if (count <= 0)
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return result;
        }

        public List<ContainerAvailableDTO> IsContainerAvailable(List<string> containerids)
        {
            List<ContainerAvailableDTO> list = new List<ContainerAvailableDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    string query = "SELECT * FROM masters.container_equip_available_bulk_check(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_containerids := '[" + string.Join(",", containerids.Select(x => Common.Decrypt(x))) + "]'::jsonb" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(new ContainerAvailableDTO
                            {
                                locationname = Common.ToString(dr["locationname"]),
                                iso_code = Common.ToString(dr["iso_code"]),
                                planned_qty = Common.ToInt(dr["planned_qty"]),
                                stock_qty = Common.ToInt(dr["stock_qty"]),
                                is_available = Common.ToBool(dr["is_available"]),
                                short_qty = Common.ToInt(dr["short_qty"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public ContainerDTO GetContainerByID(string containerid)
        {
            ContainerDTO model = new ContainerDTO();
            try
            {
                long decryptedId = Common.Decrypt(containerid);
                if (decryptedId <= 0) return model;
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.container_equip_get_byid(" + decryptedId + "," + Common.HubID + ");");
                    if (tbl != null && tbl.Rows.Count > 0)
                        model = ParseContainerDetail(tbl.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }
        public ContainerDTO GetContainerByUUID(string containeruuid)
        {
            ContainerDTO model = new ContainerDTO();
            try
            {
                if (string.IsNullOrEmpty(containeruuid)) return model;
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.container_equip_get_byuuid('" + containeruuid + "'," + Common.HubID + ");");
                    if (tbl != null && tbl.Rows.Count > 0)
                        model = ParseContainerDetail(tbl.Rows[0]);
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }

        public List<ContainerDTO> GetContainerList(ContainerFilterPage filter)
        {
            List<ContainerDTO> list = new List<ContainerDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);

                    string query = "SELECT * FROM masters.container_equip_list(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_filters := '" + Common.Escape(jsonFilters) + "'::jsonb," +
                                   "p_userid := " + Common.LoginID + "" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(ParseContainerList(dr));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public List<ContainerStatusCountDTO> GetContainerStatusCount(ContainerFilterPage filter)
        {
            List<ContainerStatusCountDTO> list = new List<ContainerStatusCountDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);

                    string query = "SELECT * FROM masters.container_equip_status_count(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_filters := '" + Common.Escape(jsonFilters) + "'::jsonb," +
                                   "p_userid := " + Common.LoginID + "" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(new ContainerStatusCountDTO
                            {
                                status_code = Common.ToInt(dr["status_code"]),
                                status_name = Common.ToString(dr["status_name"]),
                                status_count = Common.ToLong(dr["status_count"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        private ContainerDTO ParseContainerList(DataRow dr)
        {
            var formattedAge = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["manufacturedate"]));
            if (!string.IsNullOrEmpty(formattedAge.SubText))
            {
                formattedAge.SubText = formattedAge.SubText.Replace("ago", "old");
            }
            var formattedLastBooking = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["lastbookingdate"]));
            if (!string.IsNullOrEmpty(formattedLastBooking.SubText))
            {
                formattedLastBooking.SubText = formattedLastBooking.SubText.Replace("ago", "");
            }
            return new ContainerDTO()
            {
                rowcount = new TableCounts
                {
                    row_index = Common.ToInt(dr["row_index"]),
                    totalnoofrows = Common.ToInt(dr["total_count"])
                },
                containerid = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["containerid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["containerid"]))
                },
                containeruuid = Common.ToString(dr["containeruuid"]),
                equipmentno = Common.ToString(dr["equipmentno"]),
                model_iso_code = Common.ToString(dr["model_iso_code"]),
                sizename = Common.ToString(dr["sizename"]),
                type_name = Common.ToString(dr["typename"]),
                operatorname = Common.GetOperatorName(Common.ToInt(dr["operatorid"])),
                locationname = Common.ToString(dr["locationname"]),
                portname = Common.ToString(dr["portname"]),
                countryflag = !string.IsNullOrEmpty(Common.ToString(dr["countryflag"])) ? Common.FlagFolder + Common.ToString(dr["countryflag"]) : "",
                bookingid = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["bookingid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["bookingid"]))
                },
                bookingno = Common.ToString(dr["bookingno"]),
                bookinguuid = Common.ToString(dr["bookinguuid"]),
                customername = Common.ToString(dr["customername"]),
                islive = Common.ToBool(dr["islive"]),
                isdamaged = dr.Table.Columns.Contains("isdamaged") ? Common.ToBool(dr["isdamaged"]) : false,
                isblocked = dr.Table.Columns.Contains("isblocked") ? Common.ToBool(dr["isblocked"]) : false,
                polname = Common.ToString(dr["pol_portname"]),
                polcode = Common.ToString(dr["pol_portcode"]),
                pol_flag = !string.IsNullOrEmpty(Common.ToString(dr["pol_flag"])) ? Common.FlagFolder + Common.ToString(dr["pol_flag"]) : "",
                podcode = Common.ToString(dr["pod_portcode"]),
                podname = Common.ToString(dr["pod_portname"]),
                pod_flag = !string.IsNullOrEmpty(Common.ToString(dr["pod_flag"])) ? Common.FlagFolder + Common.ToString(dr["pod_flag"]) : "",
                manufacturedate = formattedAge,
                lastbookingdate = formattedLastBooking,
                locationtypename = Common.ToString(dr["locationtypename"]),
                locationicon = Common.GetIconPath(Common.ToInt(dr["locationtypeiconid"])),
                moveicon = Common.GetSelectedIconPath(Common.ToInt(dr["moveiconid"])),
                lastmove = Common.ToString(dr["movesname"]),
                is_empty = Common.ToBool(dr["is_empty"]),
                status_code = Common.ToInt(dr["status_code"]),
                lastmovedatetime = Common.ToDateTimeString(Common.ToDateTime(dr["lastmovedatetime"]), Common.HumanDateTimeformat),
                containercreatedat = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["containercreatedat"]))
            };
        }
        private ContainerDTO ParseContainerDetail(DataRow dr)
        {
            var formattedAge = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["manufacturedate"]));
            if (!string.IsNullOrEmpty(formattedAge.SubText))
            {
                formattedAge.SubText = formattedAge.SubText.Replace("ago", "old");
            }
            return new ContainerDTO()
            {
                containerid = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["containerid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["containerid"]))
                },
                operatorid = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["operatorid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["operatorid"]))
                },

                currentlocationid = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["locationdetailid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationdetailid"]))
                },
                containeruuid = Common.ToString(dr["containeruuid"]),
                equipmentno = Common.ToString(dr["equipmentno"]),
                containermodeluuid = Common.ToString(dr["containermodeluuid"]),
                model_iso_code = Common.ToString(dr["model_iso_code"]),
                sizename = Common.ToString(dr["sizename"]),
                type_name = Common.ToString(dr["typename"]),
                operatorname = Common.GetOperatorName(Common.ToInt(dr["operatorid"])),
                locationname = Common.ToString(dr["locationname"]),
                portname = Common.ToString(dr["portname"]),
                islive = Common.ToBool(dr["islive"]),
                manufacturedate = formattedAge,
                bookingid = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["bookingid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["bookingid"]))
                },
                is_empty = dr.Table.Columns.Contains("is_empty") ? Common.ToBool(dr["is_empty"]) : false,
                ageinyears = formattedAge.NumericValue != 0 ? Math.Abs(formattedAge.NumericValue / 365) : 0,
                moveicon = Common.GetSelectedIconPath(Common.ToInt(dr["moveiconid"])),
                lastmove = Common.ToString(dr["movesname"]),
                status_code = Common.ToInt(dr["status_code"]),
                bookingno = Common.ToString(dr["bookingno"]),
                bookinguuid = Common.ToString(dr["bookinguuid"]),
                lastmovedatetime = Common.ToDateTimeString(Common.ToDateTime(dr["lastmovedatetime"]), Common.HumanDateTimeformat),
            };
        }
    }
}