using Contrack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Web.Helpers;

namespace Contrack
{
    public class ClientRepository : CustomException, IClientRepository
    {
        public Result SaveClient(ClientDTO client)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.saveclient(" +
                                   "p_imo := '" + Common.Escape(client.imono) + "'," +
                                   "p_name := '" + Common.Escape(client.clientname) + "'," +
                                   "p_address := '" + Common.Escape(client.address) + "'," +
                                   "p_billing_address := '" + Common.Escape(client.billingaddress) + "'," +
                                   "p_email := '" + Common.Escape(client.email) + "'," +
                                   "p_phone := '" + Common.Escape(client.phone) + "'," +
                                   "p_created_by := '" + Common.LoginID + "'," +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_uuid := " + Common.GetUUID(client.clientuuid) + "," +
                                   "p_client_id := '" + Common.Decrypt(client.clientid.EncryptedValue) + "'," +
                                   "p_accounts_email := '" + Common.Escape(client.accountsemail) + "'," +
                                   "p_preferred_currency := '" + Common.Escape(client.preferredcurrency) + "'," +
                                   "p_standing_instructions := '" + Common.Escape(client.standardinstructions) + "'," +
                                   "p_payment_terms := '" + Common.Escape(client.paymentterms.ToString()) + "'," +
                                   "p_agency_id := '" + Common.Decrypt(client.agency.agencyid.EncryptedValue) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetID = Convert.ToInt32(tbl.Rows[0][2].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save client");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveCustomAttribute(List<KeyValuePair> attributes, int clientId, string clientUUID)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string customAttributes = JsonConvert.SerializeObject(attributes);
                    string query = "SELECT * FROM masters.updateclientcustomattributes(" +
                                   "'" + Common.HubID + "'," +
                                   "'" + clientId + "'," +
                                   "" + Common.GetUUID(clientUUID) + "," +
                                   "'" + Common.LoginID + "'," +
                                   "'" + Common.Escape(customAttributes) + "'" +
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
                        result = Common.ErrorMessage("Cannot save client attributes");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result SaveBankInfo(List<CustomBankInfo> bankInfo, int clientId, string clientUUID)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string bankinfo = JsonConvert.SerializeObject(bankInfo);

                    string query = "SELECT * FROM masters.updateclientbankaccount(" +
                                   "'" + Common.HubID + "'," +
                                   "'" + clientId + "'," +
                                   "" + Common.GetUUID(clientUUID) + "," +
                                   "'" + Common.LoginID + "'," +
                                   "'" + Common.Escape(bankinfo) + "'" +
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
                        result = Common.ErrorMessage("Cannot save bank info");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result DeleteClient(int clientId, string clientUUID)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.deleteclient(" +
                                   "'" + clientId + "'," +
                                   "" + Common.GetUUID(clientUUID) + "," +
                                   "'" + Common.LoginID + "'," +
                                   "'" + Common.HubID + "');";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete client");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        //public ClientDTO GetClientByUUID(string clientUUID)
        //{
        //    ClientDTO client = new ClientDTO();
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(clientUUID))
        //        {
        //            client = ParseClient("SELECT * FROM masters.getclientbyuuid('" + clientUUID + "','" + Common.HubID + "');");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RecordException(ex);
        //    }
        //    return client;
        //}

        public ClientDTO GetClientByUUID(string clientUUID)
        {
            ClientDTO client = new ClientDTO();

            try
            {
                if (string.IsNullOrWhiteSpace(clientUUID))
                    return client;

                string qry = @"SELECT * FROM masters.getclientbyuuid('" + clientUUID + @"', '" + Common.HubID + @"');";
                using (SqlDB db = new SqlDB())
                {
                    DataTable tbl = db.GetDataTable(qry);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        DataRow row = tbl.Rows[0];
                        client.agency = new AgencyDTO()
                        {
                            agencyid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(row["agencyid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(row["agencyid"]))
                            },
                            uuid = Common.ToString(row["agencyuuid"]),
                            agencyname = Common.ToString(row["agencyname"])
                        };
                        client.clientid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(row["clientid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(row["clientid"]))
                        };
                        client.clientuuid = Common.ToString(row["clientuuid"]);
                        client.clientname = Common.ToString(row["name"]);
                        client.imono = Common.ToString(row["imo"]);
                        client.address = Common.ToString(row["address"]);
                        client.billingaddress = Common.ToString(row["billingaddress"]);
                        client.email = Common.ToString(row["email"]);
                        client.accountsemail = Common.ToString(row["accountsemail"]);
                        client.phone = Common.ToString(row["phone"]);
                        client.paymentterms = Common.ToInt(row["paymentterms"]);
                        client.preferredcurrency = Common.ToString(row["preferredcurrency"]).Trim();
                        client.standardinstructions = Common.ToString(row["standinginstructions"]);
                        try
                        {
                            client.CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(row["customattributes"])) ?? new List<KeyValuePair>();
                            client.BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(row["bankaccounts"])) ?? new List<CustomBankInfo>();
                            client.BankInfo.ForEach(x => x.FillBankKeyValues());
                        }
                        catch
                        {
                            client.CustomAttributes = new List<KeyValuePair>();
                            client.BankInfo = new List<CustomBankInfo>();
                        }
                        client.agency.agencyname = client.agency.agencyid.NumericValue > 0 ? client.agency.agencyname : Common.HubName;
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return client;
        }


        //public ClientDTO GetClientByDetailID(int clientDetailId)
        //{
        //    ClientDTO client = new ClientDTO();
        //    try
        //    {
        //        if (clientDetailId != 0)
        //        {
        //            client = ParseClient("SELECT * FROM masters.getclientbydetailid('" + Common.HubID + "','" + clientDetailId + "');");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RecordException(ex);
        //    }
        //    return client;
        //}

        public ClientDTO GetClientByDetailID(int clientDetailId)
        {
            ClientDTO client = new ClientDTO();

            try
            {
                if (clientDetailId <= 0)
                    return client;

                string qry = @"SELECT * FROM masters.getclientbydetailid('" + Common.HubID + @"','" + clientDetailId + @"');";
                using (SqlDB db = new SqlDB())
                {
                    DataTable tbl = db.GetDataTable(qry);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        DataRow row = tbl.Rows[0];
                        client.agency = new AgencyDTO()
                        {
                            agencyid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(row["agencyid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(row["agencyid"]))
                            },
                            uuid = Common.ToString(row["agencyuuid"]),
                            agencyname = Common.ToString(row["agencyname"])
                        };
                        client.clientid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(row["clientid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(row["clientid"]))
                        };
                        client.clientdetailid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(row["clientdetailid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(row["clientdetailid"]))
                        };
                        client.clientuuid = Common.ToString(row["clientuuid"]);
                        client.imono = Common.ToString(row["imo"]);
                        client.clientname = Common.ToString(row["name"]);
                        client.email = Common.ToString(row["email"]);
                        client.phone = Common.ToString(row["phone"]);
                        client.accountsemail = Common.ToString(row["accountsemail"]);
                        client.address = Common.ToString(row["address"]);
                        client.billingaddress = Common.ToString(row["billingaddress"]);
                        client.paymentterms = Common.ToInt(row["paymentterms"]);
                        client.preferredcurrency = Common.ToString(row["preferredcurrency"]).Trim();
                        client.standardinstructions = Common.ToString(row["standinginstructions"]);
                        try
                        {
                            client.CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(row["customattributes"])) ?? new List<KeyValuePair>();
                            client.BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(row["bankaccounts"])) ?? new List<CustomBankInfo>();
                            client.BankInfo.ForEach(x => x.FillBankKeyValues());
                        }
                        catch
                        {
                            client.CustomAttributes = new List<KeyValuePair>();
                            client.BankInfo = new List<CustomBankInfo>();
                        }
                        client.agency.agencyname = client.agency.agencyid.NumericValue > 0 ? client.agency.agencyname : Common.HubName;
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return client;
        }


        //private ClientDTO ParseClient(string qry)
        //{
        //    ClientDTO client = new ClientDTO();
        //    try
        //    {
        //        using (SqlDB Db = new SqlDB())
        //        {
        //            DataTable tbl = Db.GetDataTable(qry);
        //            if (tbl.Rows.Count != 0)
        //            {
        //                client.agency = new AgencyDTO()
        //                {
        //                    agencyid = new EncryptedData()
        //                    {
        //                        NumericValue = Common.ToInt(tbl.Rows[0]["agencyid"]),
        //                        EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["agencyid"])),
        //                    },
        //                    uuid = Common.ToString(tbl.Rows[0]["agencyuuid"]),
        //                    agencyname = Common.ToString(tbl.Rows[0]["agencyname"]),
        //                };
        //                client.clientid = new EncryptedData()
        //                {
        //                    NumericValue = Common.ToInt(tbl.Rows[0]["clientid"]),
        //                    EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["clientid"])),
        //                };
        //                client.clientuuid = Common.ToString(tbl.Rows[0]["clientuuid"]);
        //                client.clientname = Common.ToString(tbl.Rows[0]["name"]);
        //                client.imono = Common.ToString(tbl.Rows[0]["imo"]);
        //                client.address = Common.ToString(tbl.Rows[0]["address"]);
        //                client.billingaddress = Common.ToString(tbl.Rows[0]["billingaddress"]);
        //                client.email = Common.ToString(tbl.Rows[0]["email"]);
        //                client.accountsemail = Common.ToString(tbl.Rows[0]["accountsemail"]);
        //                client.phone = Common.ToString(tbl.Rows[0]["phone"]);
        //                client.paymentterms = Common.ToInt(tbl.Rows[0]["paymentterms"]);
        //                client.preferredcurrency = Common.ToString(tbl.Rows[0]["preferredcurrency"]).Trim();
        //                client.standardinstructions = Common.ToString(tbl.Rows[0]["standinginstructions"]);
        //                //hcreatedat = Common.ToDateTime(tbl.Rows[0]["hcreatedat"]);
        //                try
        //                {
        //                    client.CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(tbl.Rows[0]["customattributes"]));
        //                    if (client.CustomAttributes == null)
        //                        client.CustomAttributes = new List<KeyValuePair>();

        //                    client.BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(tbl.Rows[0]["bankaccounts"]));
        //                    if (client.BankInfo == null)
        //                    {
        //                        client.BankInfo = new List<CustomBankInfo>();
        //                    }
        //                    client.BankInfo.ForEach(x => x.FillBankKeyValues());
        //                }
        //                catch (Exception)
        //                { }

        //                client.agency.agencyname = client.agency.agencyid.NumericValue > 0 ? client.agency.agencyname : Common.HubName;

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        RecordException(ex);
        //    }
        //    return client;
        //}

        public List<ClientDTO> GetClientModificationList(string clientUUID)
        {
            List<ClientDTO> list = new List<ClientDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.getclientlogsbyuuid('" + clientUUID + "','" + Common.HubID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new ClientDTO()
                            {
                                clientid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["clientid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientid"]))
                                },
                                clientuuid = Common.ToString(dr["clientuuid"]),
                                clientname = Common.ToString(dr["name"]),
                                imono = Common.ToString(dr["imo"]),
                                address = Common.ToString(dr["address"]),
                                billingaddress = Common.ToString(dr["billingaddress"]),
                                email = Common.ToString(dr["email"]),
                                accountsemail = Common.ToString(dr["accountsemail"]),
                                phone = Common.ToString(dr["phone"]),
                                paymentterms = Common.ToInt(dr["paymentterms"]),
                                preferredcurrency = Common.ToString(dr["preferredcurrency"]),
                                standardinstructions = Common.ToString(dr["standinginstructions"]),
                                ModifiedBy = Common.ToString(dr["ModifiedBy"]),
                                AgencyModifiedAt = Common.ToDateTime(dr["ClientModifiedAt"]),
                                CustomAttributes = JsonConvert.DeserializeObject<List<KeyValuePair>>(Common.ToString(dr["customattributes"])),
                                BankInfo = JsonConvert.DeserializeObject<List<CustomBankInfo>>(Common.ToString(dr["bankaccounts"]))
                            }).ToList();
                    try
                    {
                        list.ForEach(x => x.AgencyModifiedAt = x.AgencyModifiedAt.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet)));
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public List<ClientDTO> GetClientList(ClientListFilter filter)
        {
            List<ClientDTO> list = new List<ClientDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.GetClientlist(" +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_agency_id := '{" + Common.Decrypt(filter.AgencyID) + "}'," +
                                   "p_search := '" + Common.Escape(filter.Search) + "'," +
                                   "p_limit := '" + filter.limit + "'," +
                                   "p_offset := '" + filter.offset + "'," +
                                   "p_user_ids := '" + Common.LoginID + "'," +
                                   "p_sort_column := '" + filter.sorting + "'," +
                                   "p_sort_direction := '" + filter.sortingorder + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select new ClientDTO()
                            {
                                row_index = Common.ToInt(dr["row_index"]),
                                totalnoofrows = Common.ToInt(dr["total_count"]),

                                clientid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["clientid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientid"]))
                                },
                                clientuuid = Common.ToString(dr["clientuuid"]),
                                clientname = Common.ToString(dr["name"]),
                                imono = Common.ToString(dr["imo"]),
                                email = Common.ToString(dr["email"]),
                                phone = Common.ToString(dr["phone"]),
                                agency = new AgencyDTO()
                                {
                                    agencyid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["agencyid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                    },
                                    uuid = Common.ToString(dr["agencyuuid"]),
                                    agencyname = Common.ToString(dr["agencyname"])
                                },
                                extras = new ClientExtra()
                                {
                                    clientshortcode = Common.GetShortcode(Common.ToString(dr["name"])),
                                },
                                picname = Common.ToString(dr["fullname"]),
                                picshort = Common.GetShortcode(Common.ToString(dr["fullname"])),
                                picdesignation = Common.ToString(dr["picposition"]),
                                preferredcurrency = Common.ToString(dr["preferredcurrency"]),
                                hcreatedat = Common.ToDateTime(dr["createdat"]),

                            }).ToList();
                    try
                    {
                        list.ForEach(x =>
                        {
                            x.hcreatedat = x.hcreatedat.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
                            x.agency.agencyname = x.agency.agencyid.NumericValue > 0 ? x.agency.agencyname : Common.HubName;
                            var colors = Common.GetColorFromName(x.extras.clientshortcode);
                            x.extras.clientcolor = colors.Color;
                            x.extras.clientbgcolor = colors.Bg;
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

        public List<ClientDTO> GetClientListFull(string search)
        {
            List<ClientDTO> list = new List<ClientDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.getclientlist_full(" +
                                   "p_hubid := '" + Common.HubID + "'," +
                                   "p_search := '" + Common.Escape(search) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select new ClientDTO()
                            {
                                clientdetailid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["clientdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientdetailid"]))
                                },
                                extras = new ClientExtra()
                                {
                                    clientshortcode = Common.GetShortcode(Common.ToString(dr["clientname"])),
                                },
                                clientuuid = Common.ToString(dr["clientuuid"]),
                                clientname = Common.ToString(dr["clientname"]),
                                email = Common.ToString(dr["email"]),
                                phone = Common.ToString(dr["phone"]),
                                address = Common.ToString(dr["address"]),
                            }).ToList();

                    try
                    {
                        list.ForEach(x =>
                        {
                            var colors = Common.GetColorFromName(x.extras.clientshortcode);
                            x.extras.clientcolor = colors.Color;
                            x.extras.clientbgcolor = colors.Bg;
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
        public List<ClientDTO> GetClientListForPricingFull(string pricinguuid, string search)
        {
            List<ClientDTO> list = new List<ClientDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = "SELECT * FROM booking.quotation_pricing_client_search(" +
                                   "p_hubid := '" + Common.HubID + "'," +
                                   "p_pricinguuid := '" + pricinguuid + "'," +
                                   "p_search := '" + Common.Escape(search) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select new ClientDTO()
                            {
                                clientdetailid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["clientdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientdetailid"]))
                                },
                                extras = new ClientExtra()
                                {
                                    clientshortcode = Common.GetShortcode(Common.ToString(dr["clientname"])),
                                },
                                clientuuid = Common.ToString(dr["clientuuid"]),
                                clientname = Common.ToString(dr["clientname"]),
                                email = Common.ToString(dr["email"]),
                                phone = Common.ToString(dr["phone"]),
                                address = Common.ToString(dr["address"]),
                            }).ToList();

                    try
                    {
                        list.ForEach(x =>
                        {
                            var colors = Common.GetColorFromName(x.extras.clientshortcode);
                            x.extras.clientcolor = colors.Color;
                            x.extras.clientbgcolor = colors.Bg;
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


        #region Address
        public Result MakePrimary(AddressDTO client)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from masters.updateprimaryaddress(" +
                      "p_hub_id := '" + Common.HubID + "'," +
                      "p_created_by := '" + Common.LoginID + "'," +
                      "p_addressid :='" + Common.Decrypt(client.addressid.EncryptedValue) + "'" +
                      ");");

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Address");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return result;
        }
        public Result SaveAddress(AddressDTO client)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * from masters.saveclientaddress(" +
                      "p_client_id := '" + Common.Decrypt(client.clientid.EncryptedValue) + "'," +
                      "p_address :='" + Common.Escape(client.address) + "'," +
                      "p_address_name := '" + Common.Escape(client.addressname) + "'," +
                      "p_is_default :='" + client.isdefault + "'," +
                      "p_created_by := '" + Common.LoginID + "'," +
                      "p_address_id := '" + Common.Decrypt(client.addressid.EncryptedValue) + "'," +
                      "p_address_type := '" + Common.Decrypt(client.addresstypeid.EncryptedValue) + "'," +
                      "p_hub_id := '" + Common.HubID + "'" +
                      ");");

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Address");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return result;
        }
        public List<AddressDTO> GetAddressList(string clientid)
        {
            List<AddressDTO> list = new List<AddressDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.getaddresslist('" + Common.Decrypt(clientid) + "','" + Common.HubID + "');");

                    list = (from DataRow dr in tbl.Rows
                            select new AddressDTO()
                            {
                                addressid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["addressid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["addressid"]))
                                },
                                clientid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["clientid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientid"]))
                                },

                                idreftypegroup = Common.ToInt(dr["idreftypegroup"]),

                                addresstypeid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["addresstypeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["addresstypeid"]))
                                },

                                address = Common.ToString(dr["address"]),
                                addressname = Common.ToString(dr["addressname"]),
                                typevalue = Common.ToString(dr["typevalue"]),
                                isdefault = Common.ToBool(dr["isdefault"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public AddressDTO GetAddressByID(string addressid)
        {
            AddressDTO address = new AddressDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.getaddressbyid('" + Common.HubID + "','" + Common.Decrypt(addressid) + "');");
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        address.addressid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["addressid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["addressid"]))
                        };
                        address.clientid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["clientid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientid"]))
                        };
                        address.idreftypegroup = Common.ToInt(dr["idreftypegroup"]);
                        address.addresstypeid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["addresstypeid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["addresstypeid"]))
                        };
                        address.address = Common.ToString(dr["address"]);
                        address.addressname = Common.ToString(dr["addressname"]);
                        address.isdefault = Common.ToBool(dr["isdefault"]);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return address;
        }
        public Result DeleteAddress(AddressDTO client)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {

                    DataTable tbl = Db.GetDataTable("SELECT * from masters.deleteclientaddress(" +
                      "p_hub_id := '" + Common.HubID + "'," +
                      "p_deleted_by := '" + Common.LoginID + "'," +
                      "p_addressid :='" + Common.Decrypt(client.addressid.EncryptedValue) + "'" +
                      ");");

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete address");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return result;
        }
        #endregion

        #region Client Contact
        public Result SaveContact(ClientContact contact)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * from masters.saveclientpic(" +
                                   "p_clientid := '" + Common.Decrypt(contact.clientid.EncryptedValue) + "'," +
                                   "p_fullname :='" + Common.Escape(contact.fullname) + "'," +
                                   "p_email := '" + Common.Escape(contact.email) + "'," +
                                   "p_phone := '" + Common.Escape(contact.phone) + "'," +
                                   "p_position := '" + Common.Escape(contact.position) + "'," +
                                   "p_isprimary :='" + contact.isprimary + "'," +
                                   "p_createdby := '" + Common.LoginID + "'," +
                                   "p_picid := '" + Common.Decrypt(contact.picid.EncryptedValue) + "'," +
                                   "p_hub_id := '" + Common.HubID + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Contact");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result MakePrimaryContact(string picidinc)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * from masters.updateprimarypic(" +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_picid :='" + Common.Decrypt(picidinc) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot update primary contact");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }
        public ClientContact GetContactByID(string picid)
        {
            ClientContact contact = new ClientContact();
            try
            {
                if (!string.IsNullOrEmpty(picid))
                {
                    using (SqlDB Db = new SqlDB())
                    {
                        DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getpicbyid('" + Common.HubID + "','" + Common.Decrypt(picid) + "');");
                        if (tbl.Rows.Count != 0)
                        {
                            contact.picid = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["picid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["picid"])
                            };
                            contact.fullname = Common.ToString(tbl.Rows[0]["fullname"]);
                            contact.email = Common.ToString(tbl.Rows[0]["email"]);
                            contact.phone = Common.ToString(tbl.Rows[0]["phone"]);
                            contact.position = Common.ToString(tbl.Rows[0]["position"]);
                            contact.isprimary = Common.ToBool(tbl.Rows[0]["isprimary"]);
                            contact.clientid = new EncryptedData()
                            {
                                EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["clientid"])),
                                NumericValue = Common.ToInt(tbl.Rows[0]["clientid"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return contact;
        }

        public List<ClientContact> GetContactList(string clientid)
        {
            List<ClientContact> list = new List<ClientContact>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM  masters.getpiclist('" + Common.Decrypt(clientid) + "','" + Common.HubID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new ClientContact()
                            {
                                picid = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["picid"])),
                                    NumericValue = Common.ToInt(dr["picid"])
                                },
                                fullname = Common.ToString(dr["fullname"]),
                                email = Common.ToString(dr["email"]),
                                phone = Common.ToString(dr["phone"]),
                                position = Common.ToString(dr["position"]),
                                isprimary = Common.ToBool(dr["isprimary"])
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result DeleteContact(string picidinc)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * from masters.clientpicsoftdelete(" +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_deleted_by := '" + Common.LoginID + "'," +
                                   "p_picid :='" + Common.Decrypt(picidinc) + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete Contact");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public CustomerAnalyticsDTO GetClientEnquiryAnalytics(string clientid)
        {
            return new CustomerAnalyticsDTO();
        }

        #endregion
    }
}
