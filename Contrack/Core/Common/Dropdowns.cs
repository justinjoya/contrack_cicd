using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;

namespace Contrack
{
    public class DropdownModal
    {
        public string Name { get; set; }
    }
    public static class Dropdowns
    {

        public static List<ListItem> GetCountryDropdown(bool showempty = false)
        {
            var result = new List<ListItem>();

            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable($"select * from masters.getcountrylist('{Common.HubID}')");

                result = (from DataRow dr in tbl.Rows
                          select new ListItem
                          {
                              Text = Common.ToString(dr["countryname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["countryid"]))
                          }).ToList();
            }

            if (showempty)
                result.Insert(0, new ListItem { Text = "-Select-", Value = "" });

            return result;
        }
        public static List<ListItem> GetPortDropdown(string countryid = "", bool showempty = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                if (countryid == "")
                    tbl = Db.GetDataTable("select * from masters.getportlist('" + Common.HubID.ToString() + "')");
                else
                    tbl = Db.GetDataTable("select * from masters.getportlist('" + Common.HubID.ToString() + "')");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["portname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["portid"]))
                          }).ToList();
            }
            if (showempty)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetVesselTypeDropdown(bool showempty = true)
        {
            var result = new List<ListItem>();

            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable($"select * from masters.getvesseltypelist('{Common.HubID}')");

                result = (from DataRow dr in tbl.Rows
                          select new ListItem
                          {
                              Text = Common.ToString(dr["typename"]),
                              Value = Common.Encrypt(Common.ToInt(dr["vesseltypeid"]))
                          }).ToList();
            }

            if (showempty)
                result.Insert(0, new ListItem { Text = "-Select-", Value = "" });

            return result;
        }
        public static List<ListItem> GetVesselSubTypeDropdown(bool showempty = true)
        {
            var result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable($"select * from masters.getvesselsubtypelist('{Common.HubID}')");

                result = (from DataRow dr in tbl.Rows
                          select new ListItem
                          {
                              Text = Common.ToString(dr["subtypename"]),
                              Value = Common.Encrypt(Common.ToInt(dr["subtypeid"]))
                          }).ToList();
            }

            if (showempty)
                result.Insert(0, new ListItem { Text = "-Select-", Value = "" });

            return result;
        }
        public static List<ListItem> GetClientsDropdown(string AgencyDetailID, bool useuuid = false, string multiple = "")
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getclientlist(" + Common.HubID + "," + Common.Decrypt(AgencyDetailID) + ");");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["clientname"]),
                              Value = useuuid ? Common.ToString(dr["clientuuid"]) : Common.Encrypt(Common.ToInt(dr["clientdetailid"]))
                          }).ToList();
            }
            if (multiple == "")
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetPODropdownByVendor(int vendorid, string search, string selectedpo, string piuuid, int AgencyDetailID)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable("select * from procurement.get_vendor_pos('" + vendorid + "','" + Common.HubID + "'," + Common.GetUUID(selectedpo) + "," + Common.GetUUID(piuuid) + ",'" + search + "','" + AgencyDetailID + "')");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              //Text = (ToBool(dr["istax"]) ? "[TAX]#" : "[OTHERS]#") + ToString(dr["chargename"]) + (ToBool(dr["ispecentage"]) ? " (%)" : ""),
                              Text = Codes.GetCodes(Common.ToInt(dr["poid"]), Common.ToDateTime(dr["createdat"]), "PO", Common.ToString(dr["pocode"])),
                              Value = Common.ToString(dr["pouuid"])
                          }).ToList();

            }

            result.Insert(0, new ListItem() { Text = "-- Select --", Value = "" });
            return result;
        }
        public static List<ListItem> GetVendorDropdown(string AgencyDetailID, string multiple = "", bool useuuid = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getvendorlist(" + Common.HubID + "," + Common.Decrypt(AgencyDetailID) + ");");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["vendorname"]),
                              Value = useuuid ? Common.ToString(dr["vendoruuid"]) : Common.Encrypt(Common.ToInt(dr["vendordetailid"]))
                          }).ToList();
            }
            if (multiple == "")
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        //public static object GetPortSearch_old(string search)
        //{
        //    ILocationRepository repo = new LocationRepository();
        //    var ports = repo.GetGroupedPorts(search);

        //    var grouped = ports.GroupBy(x => new { x.CountryName, x.CountryFlag })
        //                       .Select(g => new
        //                       {
        //                           text = $"{g.Key.CountryFlag}##{g.Key.CountryName}",
        //                           children = g.Select(p => new
        //                           {
        //                               id = p.PortID,
        //                               text = p.PortName,
        //                               countryName = p.CountryName,
        //                               flag = g.Key.CountryFlag,
        //                               portCode = p.PortCode,
        //                               portName = p.PortName,
        //                               countryId = p.CountryID
        //                           }).ToList()
        //                       }).ToList();
        //    return grouped;
        //}
        public static object GetPortSearch(string search)
        {
            ILocationRepository repo = new LocationRepository();
            var ports = repo.GetGroupedPorts(search);

            var grouped = ports.GroupBy(x => new { x.CountryID })
                               .Select(g => new
                               {
                                   id = g.Key.CountryID,
                                   text = g.First().CountryName,
                                   flag = Common.FlagFolder + g.First().CountryFlag,
                                   children = g.Select(p => new
                                   {
                                       id = p.PortID,
                                       text = p.PortName,
                                       portCode = p.PortCode,
                                       flag = Common.FlagFolder + g.First().CountryFlag,
                                       country = g.First().CountryName,
                                   }).ToList()
                               }).ToList();
            return grouped;
        }

        public static List<ClientDTO> GetCustomerSearch(string search)
        {
            IClientRepository repo = new ClientRepository();
            var list = repo.GetClientListFull(search);
            return list;
        }
        public static List<ClientDTO> GetCustomerSearchForPricing(string templateid, string search)
        {
            IClientRepository repo = new ClientRepository();
            var list = repo.GetClientListForPricingFull(templateid, search);
            return list;
        }

        public static object GetContainerModelSearch(string search)
        {
            ContainerModelFilter filter = new ContainerModelFilter
            {
                searchstr = search
            };
            IContainerModelRepository repo = new ContainerModelRepository();
            var models = repo.GetContainerModels(filter);

            var result = models.Where(x => x.sizes.Count(y => y.models.Count > 0) > 0).Select(type => new
            {
                id = type.containertypeid.EncryptedValue,
                text = type.typename,
                children = type.sizes
                            .SelectMany(size => size.models.Select(model => new
                            {
                                id = model.modeluuid,
                                text = string.IsNullOrEmpty(model.description) ? $"{size.sizename}" : $"{size.sizename} ({model.description})",
                                isocode = model.iso_code
                            })).ToList()
            });
            return result;
        }

        public static List<ListItem> EmptyDropdown(bool showselect = false)
        {
            List<ListItem> result = new List<ListItem>();
            if (showselect)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetTransferTypeDropdown(bool showselect = false)
        {
            List<ListItem> result = new List<ListItem>();
            result.Add(new ListItem() { Text = "Door to Door", Value = Common.Encrypt(1) });
            result.Add(new ListItem() { Text = "Door to Port", Value = Common.Encrypt(2) });
            result.Add(new ListItem() { Text = "Port to Port", Value = Common.Encrypt(3) });
            result.Add(new ListItem() { Text = "Port to Door", Value = Common.Encrypt(4) });
            if (showselect)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetFullOREmptyDropdown(bool showselect = false)
        {
            List<ListItem> result = new List<ListItem>();
            result.Add(new ListItem() { Text = "Empty", Value = "E" });
            result.Add(new ListItem() { Text = "Full", Value = "F" });
            if (showselect)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            else
                result.Insert(0, new ListItem() { Text = "None", Value = "" });
            return result;
        }
        public static List<ListItem> GetUserTypeDropdown()
        {
            var result = new List<ListItem>();
            using (var Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable("SELECT * FROM masters.usertypes WHERE COALESCE(isdeleted,false)=false;");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = dr["usertype"].ToString(),
                              Value = Common.Encrypt(Convert.ToInt32(dr["usertypeid"]))
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetVendorDropdownForFilter(List<string> AgencyUUIDs, bool multiple = true)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                string uuids = "";
                if (AgencyUUIDs != null)
                    uuids = string.Join(",", AgencyUUIDs);
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM masters.getvendorlistby_agencyuuid_userid(" + Common.HubID + ",'{" + uuids + "}','" + Common.LoginID + "');");
                if (tbl != null)
                {
                    result = (from DataRow dr in tbl.Rows
                              select new ListItem()
                              {
                                  Text = Common.ToString(dr["vendorname"]),
                                  Value = Common.ToString(dr["vendoruuid"])
                              }).ToList();
                }
            }
            if (!multiple)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetAgenciesDropdown(bool multiple = true)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getagencylist('" + Common.HubID + "','" + Common.LoginID + "');");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["agencyname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                          }).ToList();
            }
            if (!multiple)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetAgenciesDetailIDDropdown()
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getagencylist('" + Common.HubID + "','" + Common.LoginID + "');");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["agencyname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["agencydetailid"]))
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetAgenciesUUIDDropdown(bool multiple = true)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getagencylist('" + Common.HubID + "','" + Common.LoginID + "');");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["agencyname"]),
                              Value = Common.ToString(dr["uuid"]),
                          }).ToList();
            }
            if (!multiple)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetClientsByUserIDDropdown(bool multiple = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getclientlist_userid(" + Common.HubID + "," + Common.LoginID + ");");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["clientname"]),
                              Value = Common.ToString(dr["clientuuid"])
                          }).ToList();
            }
            if (multiple)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetRoles()
        {
            var result = new List<ListItem>();
            using (var Db = new SqlDB())
            {
                result = (from Roles dr in new Roles().GetRolesList()
                          select new ListItem()
                          {
                              Text = dr.role_name,
                              Value = Common.Encrypt(dr.role_id)
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-- Select --", Value = "" });
            return result;
        }
        public static List<ListItem> GetLocationTypeDropdown(bool showempty = true)
        {
            List<ListItem> result = new List<ListItem>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = $"SELECT locationtypeid, locationtypename FROM masters.location_type WHERE hubid = {Common.HubID} AND COALESCE(isdeleted,false)=false ORDER BY locationtypeid ASC";

                    DataTable tbl = Db.GetDataTable(query);

                    result = (from DataRow dr in tbl.Rows
                              select new ListItem()
                              {
                                  Text = Common.ToString(dr["locationtypename"]),
                                  Value = Common.Encrypt(Common.ToInt(dr["locationtypeid"]))
                              }).ToList();
                }

                if (showempty)
                    result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            }
            catch (Exception ex)
            {
            }
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
                              Text = Common.ToString(dr["currencycode"]) + " - " + Common.ToString(dr["currencyname"]),
                              Value = Common.ToString(dr["currencycode"]).Trim()
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetContainerTypesDropdown()
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            {
                DataTable tbl = Db.GetDataTable(
                    "SELECT * FROM masters.container_type_list_simple(" +
                    "p_hubid := '" + Common.HubID + "'" + ");");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["typename"]),
                              Value = Common.Encrypt(Common.ToInt(dr["containertypeid"]))
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetContainerTypeSizeDropdown()
        {
            List<ListItem> result = new List<ListItem>();

            var typelist = GetContainerTypesDropdown();
            var sizelist = GetContainerSizesDropdown();

            typelist.RemoveAll(x => x.Value == "");
            sizelist.RemoveAll(x => x.Value == "");

            result = (from type in typelist
                      from size in sizelist
                      select new ListItem
                      {
                          Text = $"{size.Text} {type.Text}",
                          Value = type.Value + "@@" + size.Value
                      }
                    ).ToList();

            result.Insert(0, new ListItem() { Text = "Common", Value = "" });
            return result;
        }
        public static List<ListItem> GetContainerModelsDropdown(bool showempty = true)
        {
            List<ListItem> result = new List<ListItem>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_type_list(" +
                        "p_hubid := " + Common.HubID +
                        ");");

                    result = (from DataRow dr in tbl.Rows
                              where Common.ToInt(dr["modelid"]) > 0
                              select new ListItem()
                              {
                                  Text = $"{Common.ToString(dr["iso_code"])} - {Common.ToString(dr["sizename"])} {Common.ToString(dr["typename"])}",
                                  Value = Common.ToString(dr["modeluuid"])
                              }).OrderBy(x => x.Text).ToList();
                }

                if (showempty)
                    result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static List<ListItem> GetContainerOperatorDropdown()
        {
            // 1=Owned, 2=Leased, 3=SOC, 4=COC
            var list = new List<ListItem>
        {
            new ListItem { Text = "Owned", Value = Common.Encrypt(1) },
            new ListItem { Text = "Leased", Value = Common.Encrypt(2) },
            new ListItem { Text = "SOC", Value = Common.Encrypt(3) },
            new ListItem { Text = "COC", Value = Common.Encrypt(4) }
        };
            list.Insert(0, new ListItem { Text = "-Select-", Value = "" });
            return list;
        }

        public static List<ListItem> GetCustomerTypeDropdown()
        {
            // 1=Owned, 2=Leased, 3=SOC, 4=COC
            var list = new List<ListItem>
        {
            new ListItem { Text = "Shipper", Value = Common.Encrypt(1) },
            new ListItem { Text = "Consignee", Value = Common.Encrypt(2) },
        };
            list.Insert(0, new ListItem { Text = "-Select-", Value = "" });
            return list;
        }

        public static List<ListItem> GetLocationDropdown(bool showempty = true)
        {
            List<ListItem> result = new List<ListItem>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_location_list(" +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_filters := '{}'," +
                        "p_userid := '" + Common.LoginID + "'" +
                        ");");

                    result = (from DataRow dr in tbl.Rows
                              select new ListItem()
                              {
                                  Text = $"{Common.ToString(dr["locationname"])} ({Common.ToString(dr["locationcode"])})",
                                  Value = Common.Encrypt(Common.ToInt(dr["locationdetailid"]))
                              }).ToList();
                }

                if (showempty)
                    result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static List<ListItem> GetContainerSizesDropdown()
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            {
                DataTable tbl = Db.GetDataTable(
                    "SELECT * FROM masters.container_size_list();");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["sizename"]),
                              Value = Common.Encrypt(Common.ToInt(dr["sizeid"]))
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetVesselByUserIDDropdown(string search, string multiple = "")
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getvessellist_userid(" + Common.HubID + "," + Common.LoginID + ",'" + search + "');");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["vesselname"]),
                              Value = Common.ToString(dr["vesselassignmentuuid"])
                          }).ToList();
            }
            if (multiple == "")
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetVesselDropdownSearch(string search, string multiple = "")
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getvessellist_userid(" + Common.HubID + "," + Common.LoginID + ",'" + search + "');");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["vesselname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["vesseldetailid"]))
                          }).ToList();
            }
            if (multiple == "")
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetVesselDropdown(string AgencyDetailID, string search, string multiple = "", bool useuuid = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.getvessellist(" + Common.HubID + "," + Common.Decrypt(AgencyDetailID) + ",'" + search + "');");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["vesselname"]),
                              Value = useuuid ? Common.ToString(dr["assignmentuuid"]) : Common.Encrypt(Common.ToInt(dr["vesseldetailid"]))
                          }).ToList();
            }
            if (multiple == "")
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }

        public static List<ListItem> GetPICDropdown(string ClientId, string multiple = "")
        {
            List<ListItem> result = new List<ListItem>();

            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();

                tbl = Db.GetDataTable(
                   "SELECT * FROM  masters.getpiclist('" + Common.Decrypt(ClientId) + "', '" + Common.HubID + "');"
                );

                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["fullname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["picid"]))
                          }).ToList();
            }

            //if (multiple == "")
            //    result.Insert(0, new ListItem() { Text = "-Select PIC-", Value = "" });

            return result;
        }
        public static List<SelectListItem> GetMonthDropdown()
        {
            var months = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames
                .Where(m => !string.IsNullOrEmpty(m))
                .Select((name, index) => new SelectListItem
                {
                    Text = name,
                    Value = (index + 1).ToString()
                }).ToList();
            months.Insert(0, new SelectListItem { Text = "Month", Value = "" });
            return months;
        }
        public static List<SelectListItem> GetYearDropdown()
        {
            var years = Enumerable.Range(DateTime.Now.Year - 100, 101)
                .OrderByDescending(y => y)
                .Select(y => new SelectListItem
                {
                    Text = y.ToString(),
                    Value = y.ToString()
                }).ToList();
            years.Insert(0, new SelectListItem { Text = "Year", Value = "" });
            return years;
        }
        public static List<ListItem> GetPICByDetailIDDropdown(string ClientdetailId, string multiple = "")
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable(
                   "SELECT * FROM masters.getpiclistbydetailid('" + Common.Decrypt(ClientdetailId) + "', '" + Common.HubID + "');"
                );

                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["fullname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["picid"]))

                          }).ToList();
            }
            //if (multiple == "")
            //    result.Insert(0, new ListItem() { Text = "-Select PIC-", Value = "" });
            return result;
        }
        public static List<ListItem> GetContainerModelDropDownByTypeSize(string typeid, string sizeid)
        {
            ContainerModelFilter filter = new ContainerModelFilter();
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            {
                filter.filters.containertypeid = Common.Decrypt(typeid);
                filter.filters.sizeid = Common.Decrypt(sizeid);
                string jsonFilter = JsonConvert.SerializeObject(filter);

                string query = "SELECT * FROM masters.container_model_list(" +
                               "p_hubid := '" + Common.HubID + "'," +
                               "p_filters := '" + jsonFilter + "'::jsonb" +
                               ");";

                DataTable tbl = Db.GetDataTable(query);

                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["iso_code"]),
                              Value = Common.ToString(dr["modeluuid"])
                          }).ToList();
            }
            //if (multiple == "")
            //    result.Insert(0, new ListItem() { Text = "-Select PIC-", Value = "" });
            return result;
        }
        public static List<VoyageDTO> GetVoyageSearch(string search, bool createnew = true)
        {
            IVoyageRepository repo = new VoyageRepository();
            var list = repo.SearchVoyage(search, createnew);
            return list;
        }

        public static List<ListItem> GetPackageTypeDropdown()
        {
            List<ListItem> result = new List<ListItem>();

            //using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            //{
            //    DataTable tbl = Db.GetDataTable(
            //        "SELECT * FROM masters.container_size_list();");

            //    result = (from DataRow dr in tbl.Rows
            //              select new ListItem()
            //              {
            //                  Text = Common.ToString(dr["sizename"]),
            //                  Value = Common.Encrypt(Common.ToInt(dr["sizeid"]))
            //              }).ToList();
            //}

            result.AddRange(new List<ListItem>
            {
                new ListItem { Text = "Wooden Cases",Value = Common.Encrypt(Common.ToInt(1))},
                new ListItem {Text = "Carton Boxes",Value = Common.Encrypt(Common.ToInt(2))},
                new ListItem {Text = "Palletized",Value = Common.Encrypt(Common.ToInt(3))}
            });
            result.Insert(0, new ListItem { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<ListItem> GetContainerStatusDropdown()
        {
            // Static values: 1 = Available, 2 = Booked, 3 = Damaged
            return new List<ListItem>
            {
                new ListItem { Text = "Available", Value = "1" },
                new ListItem { Text = "Booked",    Value = "2" },
                new ListItem { Text = "Damaged",   Value = "3" }
            };
        }

        public static List<SelectListItem> GetOwnershipDropdown()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Text = "Owned",  Value = "1" },
                new SelectListItem { Text = "Leased", Value = "2" },
                new SelectListItem { Text = "SOC",    Value = "3" }
            };
        }
        public static List<ListItem> GetUOMDropdown()
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("select * from procurement.uom where hubid='" + Common.HubID + "'");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["uomname"]),
                              Value = Common.ToString(dr["uomname"])
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            return result;
        }
        public static List<SelectListItem> GetVesselByDetailIDs(string detailids)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            using (SqlDB Db = new SqlDB())
            {
                string query =
                    "select * from procurement.getvesselsbydetailids('" +
                    Common.HubID + "','{" + detailids + "}')";

                DataTable tbl = Db.GetDataTable(query);

                result = (from DataRow dr in tbl.Rows
                          select new SelectListItem
                          {
                              Text = Common.ToString(dr["vesselname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["vesseldetailid"]))
                          }).ToList();
            }

            return result;
        }
        public static List<ListItem> GetLoginUsersByRole(string RoleEncrypted, string AgencyEncrypted, bool showselect = true, bool encrypt = true)
        {
            List<ListItem> result = new List<ListItem>();

            UserFilter login = new UserFilter
            {
                limit = -1,
                Role = RoleEncrypted,
                EntityID = new List<string> { AgencyEncrypted }
            };

            var repo = new LoginRepository();
            var list = repo.GetUserLoginList(login);

            result = list.Select(dr => new ListItem
            {
                Text = dr.Name,
                Value = encrypt
                    ? Common.Encrypt(Common.ToInt(dr.UserID.NumericValue))
                    : dr.UserID.NumericValue.ToString()
            }).ToList();

            if (showselect)
                result.Insert(0, new ListItem { Text = "-Select-", Value = "" });

            return result;
        }
        public static List<ListItem> GetVoyageStatusDropdown()
        {
            return new List<ListItem>
            {
                new ListItem { Text = "Live", Value = "1" },
                new ListItem { Text = "Past",    Value = "2" },
                new ListItem { Text = "Upcoming",   Value = "3" }
            };
        }
        public static List<ListItem> GetQuotationStatusDropdown()
        {
            return new List<ListItem>
            {
                new ListItem { Text = "Draft", Value = "1" },

            };
        }
        public static List<ListItem> GetJobTypeDetailDropdown(string jobtypeid, bool showmisc = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("select * from procurement.get_jobtype_det('" + Common.HubID + "','" + jobtypeid + "')");
                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["description"]),
                              Value = Common.ToString(dr["jobtypedetailuuid"])
                          }).ToList();
            }
            result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });
            if (showmisc)
                result.Add(new ListItem() { Text = "Miscellaneous", Value = "-1" });
            return result;
        }
        public static List<ListItem> GetStatusDropdown(int type, bool showall = true)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB())
            {
                DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.get_status_list_by_type(" +
                    "p_hubid := " + Common.HubID + ", " +
                    "p_type_id := " + type +
                ");");

                if (tbl != null && tbl.Rows.Count > 0)
                {
                    result = (from DataRow dr in tbl.Rows
                              select new ListItem()
                              {
                                  Value = Common.ToString(dr["status_id"]),
                                  Text = Common.ToString(dr["status_name"])
                              }).ToList();
                }
            }
            if (showall)
                result.Insert(0, new ListItem() { Text = "- All Statuses -", Value = "" });
            return result;
        }


        public static List<ListItem> GetMovesDropdown(bool showempty = true)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.moves_list('" + Common.HubID + "','" + Common.LoginID + "');");

                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["movesname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["movesid"]))
                          }).ToList();
            }
            if (showempty)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });

            return result;
        }

        public static List<ListItem> GetVoyageDropdown(bool showempty = true)
        {
            List<ListItem> result = new List<ListItem>();
            using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.voyage_dropdown_list('" + Common.HubID + "','" + Common.LoginID + "');");

                result = (from DataRow dr in tbl.Rows
                          select new ListItem()
                          {
                              Text = Common.ToString(dr["voyagenumber"]) + " - " + Common.ToString(dr["vesselname"]),
                              Value = Common.Encrypt(Common.ToInt(dr["voyageid"]))
                          }).ToList();
            }
            if (showempty)
                result.Insert(0, new ListItem() { Text = "-Select-", Value = "" });

            return result;
        }

        public static List<SelectListItem> GetDamageStatusDropdown()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Text = "Yes", Value = "1" },
            new SelectListItem { Text = "No", Value = "0" }
        };
        }
        public static List<SelectListItem> GetFullEmptyDropdown()
        {
            return new List<SelectListItem>
        {
            new SelectListItem { Text = "Full", Value = "1" },
            new SelectListItem { Text = "Empty", Value = "0" }
        };
        }

        public static List<MovesDTO> GetNewMovesDropdown(bool showempty = true)
        {
            List<MovesDTO> result = new List<MovesDTO>();
            using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
            {
                DataTable tbl = new DataTable();
                tbl = Db.GetDataTable("SELECT * FROM  masters.moves_list('" + Common.HubID + "','" + Common.LoginID + "');");

                result = (from DataRow dr in tbl.Rows
                          select new MovesDTO()
                          {
                              MovesName = Common.ToString(dr["movesname"]),
                              MovesId = new EncryptedData()
                              {
                                  EncryptedValue = Common.Encrypt(Common.ToInt(dr["movesid"]))
                              },
                              ShowVoyage = Common.ToBool(dr["showvoyage"])
                          }).ToList();
            }
            if (showempty)
                result.Insert(0, new MovesDTO()
                {
                    MovesName = "-Select-",
                    MovesId = new EncryptedData()
                    {
                        EncryptedValue = ""
                    },
                    ShowVoyage = false
                });

            return result;
        }
    }
}
