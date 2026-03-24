using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class LocationRepository : CustomException, ILocationRepository
    {
        public Result SaveLocation(LocationDTO location)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = "SELECT * FROM masters.container_location_save(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_countryid := " + Common.Decrypt(location.CountryID.EncryptedValue) + "," +
                                   "p_portid := " + Common.Decrypt(location.PortID.EncryptedValue) + "," +
                                   "p_locationcode := '" + Common.Escape(location.LocationCode) + "'," +
                                   "p_locationtypeid := " + Common.Decrypt(location.LocationType.LocationTypeID.EncryptedValue) + "," +
                                   "p_locationname := '" + Common.Escape(location.LocationName) + "'," +
                                   "p_address := '" + Common.Escape(location.Address) + "'," +
                                   "p_phone := '" + Common.Escape(location.Phone) + "'," +
                                   "p_email := '" + Common.Escape(location.Email) + "'," +
                                   "p_user_id := " + Common.LoginID + "," +
                                   "p_location_id := " + Common.Decrypt(location.LocationID.EncryptedValue) + "," +
                                   "p_locationuuid := " + Common.GetUUID(location.LocationUUID) + "" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0]["resultid"].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0]["resultmessage"].ToString());
                            result.TargetID = Convert.ToInt32(tbl.Rows[0]["primaryid"]);
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0]["resultmessage"].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save location");
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
        public List<PortGroupDTO> GetGroupedPorts(string search = "")
        {
            List<PortGroupDTO> list = new List<PortGroupDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string safeSearch = Common.Escape(search ?? "");

                    string query = "SELECT * FROM masters.Searchport(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_search_text := '" + safeSearch + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    foreach (DataRow dr in tbl.Rows)
                    {
                        list.Add(new PortGroupDTO
                        {
                            PortID = Common.Encrypt(Common.ToInt(dr["portid"])),
                            PortName = Common.ToString(dr["portname"]),
                            PortCode = Common.ToString(dr["portcode"]),
                            CountryID = Common.Encrypt(Common.ToInt(dr["countryid"])),
                            CountryName = Common.ToString(dr["countryname"]),
                            CountryFlag = Common.ToString(dr["country_flag"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result DisableLocation(int LocationID)
        {
            return Common.ErrorMessage("");
        }
        public LocationDTO GetLocationByID(int LocationID)
        {
            LocationDTO location = new LocationDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_location_getbyid(" +
                        "p_location_id := " + LocationID + ", " +
                        "p_hubid := " + Common.HubID + ");"
                    );
                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        location = ParseLocation(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return location;
        }
        public LocationDTO GetLocationByUUID(string locationUUID)
        {
            LocationDTO location = new LocationDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM masters.container_location_getbyuuid(" +
                        "p_location_uuid := " + Common.GetUUID(locationUUID) + ", " +
                        "p_hubid := " + Common.HubID + ");"
                    );

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        location = ParseLocation(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return location;
        }
        public PortGroupDTO GetPortById(int PortID)
        {
            PortGroupDTO port = new PortGroupDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.getportbyid(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_portid := " + PortID + "" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        port = new PortGroupDTO
                        {
                            PortID = Common.Encrypt(Common.ToInt(dr["portid"])),
                            PortName = Common.ToString(dr["portname"]),
                            PortCode = Common.ToString(dr["portcode"]),
                            CountryID = Common.Encrypt(Common.ToInt(dr["countryid"])),
                            CountryName = Common.ToString(dr["countryname"]),
                            CountryFlag = Common.ToString(dr["country_flag"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return port;
        }
        public List<LocationTypeDTO> GetLocationTypes()
        {
            return new List<LocationTypeDTO>();
        }
        public List<LocationDTO> GetLocationList(LocationListFilter filter)
        {
            List<LocationDTO> list = new List<LocationDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);

                    string query = "SELECT * FROM masters.container_location_list(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_filters := '" + Common.Escape(jsonFilters) + "'," +
                                   "p_userid := " + Common.LoginID + ");";

                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select ParseLocationList(dr)).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        private LocationDTO ParseLocationList(DataRow dr)
        {
            LocationDTO location = new LocationDTO();

            location.LocationID = new EncryptedData()
            {
                NumericValue = Common.ToInt(dr["locationid"]),
                EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationid"]))
            };
            location.LocationUUID = Common.ToString(dr["locationuuid"]);
            location.LocationCode = Common.ToString(dr["locationcode"]);
            location.LocationName = Common.ToString(dr["locationname"]);
            location.Address = Common.ToString(dr["address"]);
            location.Phone = Common.ToString(dr["phone"]);
            location.Email = Common.ToString(dr["email"]);
            location.CreatedAt = Common.ToDateTime(dr["createdat"]);

            location.CountryID = new EncryptedData()
            {
                NumericValue = Common.ToInt(dr["countryid"]),
                EncryptedValue = Common.Encrypt(Common.ToInt(dr["countryid"]))
            };
            location.CountryName = Common.ToString(dr["countryname"]);
            location.CountryFlag = Common.ToString(dr["flag"]);
            location.PortID = new EncryptedData()
            {
                NumericValue = Common.ToInt(dr["portid"]),
                EncryptedValue = Common.Encrypt(Common.ToInt(dr["portid"]))
            };
            location.PortName = Common.ToString(dr["portname"]);
            location.PortCode = Common.ToString(dr["portcode"]);

            location.LocationType = new LocationTypeDTO()
            {
                LocationTypeID = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["locationtypeid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationtypeid"]))
                },
                LocationTypeName = Common.ToString(dr["locationtypename"]),
                Icon = new Icon()
                {
                    iconid = new EncryptedData()
                    {
                        NumericValue = Common.ToInt(dr["iconid"]),
                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["iconid"]))
                    },
                    icon = Common.GetIconPath(Common.ToInt(dr["iconid"])),
                }
            };
            location.TotalCount = Common.ToInt(dr["total_count"]);
            location.AvailableCount = Common.ToInt(dr["available_count"]);
            location.BookedCount = Common.ToInt(dr["booked_count"]);
            location.BlockedCount = Common.ToInt(dr["blocked_count"]);
            location.DamagedCount = Common.ToInt(dr["damaged_count"]);

            return location;
        }
        private LocationDTO ParseLocation(DataRow dr)
        {
            LocationDTO location = new LocationDTO();

            location.LocationID = new EncryptedData()
            {
                NumericValue = Common.ToInt(dr["locationid"]),
                EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationid"]))
            };
            location.LocationUUID = Common.ToString(dr["locationuuid"]);
            location.LocationCode = Common.ToString(dr["locationcode"]);
            location.LocationName = Common.ToString(dr["locationname"]);
            location.Address = Common.ToString(dr["address"]);
            location.Phone = Common.ToString(dr["phone"]);
            location.Email = Common.ToString(dr["email"]);
            location.CreatedAt = Common.ToDateTime(dr["createdat"]);

            location.CountryID = new EncryptedData()
            {
                NumericValue = Common.ToInt(dr["countryid"]),
                EncryptedValue = Common.Encrypt(Common.ToInt(dr["countryid"]))
            };
            location.CountryName = Common.ToString(dr["countryname"]);
            location.CountryFlag = Common.ToString(dr["flag"]);
            location.PortID = new EncryptedData()
            {
                NumericValue = Common.ToInt(dr["portid"]),
                EncryptedValue = Common.Encrypt(Common.ToInt(dr["portid"]))
            };
            location.PortName = Common.ToString(dr["portname"]);
            location.PortCode = Common.ToString(dr["portcode"]);

            location.LocationType = new LocationTypeDTO()
            {
                LocationTypeID = new EncryptedData()
                {
                    NumericValue = Common.ToInt(dr["locationtypeid"]),
                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationtypeid"]))
                },
                LocationTypeName = Common.ToString(dr["locationtypename"]),
                Icon = new Icon()
                {
                    iconid = new EncryptedData()
                    {
                        NumericValue = Common.ToInt(dr["iconid"]),
                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["iconid"]))
                    },
                    icon = Common.ToString(dr["icon"])
                }
            };
            return location;
        }
    }
}