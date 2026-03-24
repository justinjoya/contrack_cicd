using Contrack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Web.DynamicData;

namespace Contrack
{
    public class BookingRepository : CustomException, IBookingRepository
    {
        public Result SaveCustomer(int bookingid, BookingCustomerDTO customer)
        {
            Result result = new Result();

            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query =
                        "SELECT * FROM booking.booking_save_customer(" +
                        "p_bookingid := '" + bookingid + "'," +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_agencydetailid := '" + Common.Decrypt(customer.agencydetailid.EncryptedValue) + "'," +
                        "p_bookingdate := '" + customer.bookingdate + "'," +
                        "p_clientdetailid := '" + Common.Decrypt(customer.client.clientdetailid.EncryptedValue) + "'," +
                        "p_customertype := '" + Common.Decrypt(customer.customertype.EncryptedValue) + "'," +
                        "p_mode := '" + Common.Decrypt(customer.mode.EncryptedValue) + "'," +
                        "p_emptyfull := '" + customer.fullempty + "'," +
                        "p_createdby := '" + Common.LoginID + "'" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        string resultId = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();
                        string uuid = tbl.Rows[0][2].ToString();

                        if (resultId == "1")
                        {
                            result = Common.SuccessMessage(message);
                            result.TargetUUID = uuid;
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save customer details.");
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

        public Result SaveLocation(string bookinguuid, BookingLocationDTO loc)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query =
                        "SELECT * FROM booking.booking_save_location(" +
                        "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'," +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_pol := '" + Common.Decrypt(loc.pol.EncryptedValue) + "'," +
                        "p_pod := '" + Common.Decrypt(loc.pod.EncryptedValue) + "'," +
                        "p_shipperdetailid := '" + Common.Decrypt(loc.shipperdetailid.EncryptedValue) + "'," +
                        "p_shippername := '" + Common.Escape(loc.shippername) + "'," +
                        "p_shipperpic := '" + Common.Decrypt(loc.shipperpic.EncryptedValue) + "'," +
                        "p_shipperpiccustom := '" + Common.Escape(loc.shipperpiccustom) + "'," +
                        "p_shipperemail := '" + Common.Escape(loc.shipperemail) + "'," +
                        "p_shipperphone := '" + Common.Escape(loc.shipperphone) + "'," +
                        "p_shipperaddress := '" + Common.Escape(loc.shipperaddress) + "'," +
                        "p_consigneedetailid := '" + Common.Decrypt(loc.consigneedetailid.EncryptedValue) + "'," +
                        "p_consigneename := '" + Common.Escape(loc.consigneename) + "'," +
                        "p_consigneepic := '" + Common.Decrypt(loc.consigneepic.EncryptedValue) + "'," +
                        "p_consigneepiccustom := '" + Common.Escape(loc.consigneepiccustom) + "'," +
                        "p_consigneeemail := '" + Common.Escape(loc.consigneeemail) + "'," +
                        "p_consigneephone := '" + Common.Escape(loc.consigneephone) + "'," +
                        "p_consigneeaddress := '" + Common.Escape(loc.consigneeaddress) + "'," +
                        "p_voyageuuid := " + Common.GetUUID(loc.voyageuuid) + "" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        string resultId = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();
                        string uuid = tbl.Rows[0][2].ToString();
                        if (resultId == "1")
                        {
                            result = Common.SuccessMessage(message);
                            result.TargetUUID = uuid;
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save booking location.");
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

        public Result SaveContainers(string bookingid, ContainerBookingDetailDTO model)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    var containerJsonObj = new[]
                    {
                         new
                         {
                             containermodeluuid = Common.Escape(model.containermodeluuid),
                             bookingdetailuuid =  Common.Escape(model.bookingdetailuuid),
                             ownership = Common.ToInt(model.ownership),
                             containertypeid = Common.Decrypt(model.containertypeid.EncryptedValue),
                             containersizeid = Common.Decrypt(model.sizeid.EncryptedValue),
                             qty = Common.ToInt(model.qty),
                             commodity = Common.ToString(model.commodity),
                             grossweight = Common.ToInt(model.grossweight),
                             volumeweight = Common.ToInt(model.volumeweight),
                             hscode = Common.ToInt(model.hscode),
                             cargovalue = Common.ToInt(model.cargovalue),
                             packagetype = Common.Decrypt(model.packagetype.EncryptedValue),
                             expectedstuffingdate = Common.ToDateTime(model.expectedstuffingdate.DBFormattedText),
                             stuffinglocation = Common.Escape(model.stuffinglocation),
                             pickuplocation = Common.Escape(model.pickuplocation),
                             isdg = Common.ToBool(model.isdg),
                             isreefer = Common.ToBool(model.isreefer),
                             empty_full = Common.ToString(model.empty_full),

                             services = model.services.Select(s => new
                                 {
                                     bookingdetailserviceid = Common.Decrypt(s.bookingdetailserviceid.EncryptedValue),
                                     serviceid = Common.Decrypt(s.serviceid.EncryptedValue),
                                     createdby = Common.LoginID
                                 })
                         }
                    };

                    string containersJson = System.Text.Json.JsonSerializer.Serialize(containerJsonObj);

                    string query = @"SELECT * FROM booking.booking_save_containerdetail(
                      p_bookingid := '" + Common.Decrypt(bookingid) + @"',
                      p_containers := '" + Common.Escape(containersJson) + @"'::jsonb,
                      p_createdby := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save container details.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }

            return result;
        }

        public Result SaveBooking(ContainerBookingDTO booking)
        {
            throw new NotImplementedException();
        }

        public ContainerBookingDTO GetbookingByUUID(string bookinguuid)
        {
            ContainerBookingDTO booking = new ContainerBookingDTO();
            try
            {
                booking.customer = GetBookingCustomerInfo(bookinguuid, booking);
                booking.location = GetBookingLocationInfo(bookinguuid, booking);
                booking.details = GetContainerBookingDetailByBookingUUId(bookinguuid);
                //booking = GetBookingAdditionalServices(bookinguuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return booking;
        }


        public BookingCustomerDTO GetBookingCustomerInfo(string bookinguuid, ContainerBookingDTO parent)
        {
            BookingCustomerDTO model = new BookingCustomerDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM booking.booking_get_customer(" +
                        "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'," +
                        "p_hubid := '" + Common.HubID + "'" +
                        ");");

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        if (parent != null && string.IsNullOrEmpty(parent.bookinguuid))
                        {
                            int bookingId = Common.ToInt(dr["bookingid"]);
                            parent.bookingid = new EncryptedData()
                            {
                                NumericValue = bookingId,
                                EncryptedValue = Common.Encrypt(bookingId)
                            };
                            parent.bookinguuid = Common.ToString(dr["bookinguuid"]);
                            parent.bookingno = Common.ToString(dr["bookingno"]);
                            parent.createdat = Common.ToDateTime(dr["createdat"]);
                            parent.createdusername = Common.ToString(dr["createdusername"]);
                        }
                        model.bookingdate = Common.ToDateTimeString(dr["bookingdate"], "yyyy-MM-dd HH:mm");
                        model.customertype = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["customertype"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["customertype"]))
                        };
                        model.agencydetailid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["agencydetailid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencydetailid"]))
                        };
                        model.agencyname = Common.ToString(dr["agencyname"]);
                        model.isconfirmed = Common.ToBool(dr["isconfirmed"]);
                        model.mode = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["mode"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["mode"]))
                        };
                        model.fullempty = Common.ToString(dr["fullempty"]);
                        model.client = new ClientDTO()
                        {
                            clientdetailid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["clientdetailid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientdetailid"]))
                            },
                            clientuuid = Common.ToString(dr["clientuuid"]),
                            clientname = Common.ToString(dr["clientname"]),
                            email = Common.ToString(dr["clientemail"]),
                            phone = Common.ToString(dr["clientphone"]),
                            address = Common.ToString(dr["clientaddress"]),
                            agency = new AgencyDTO()
                            {
                                uuid = Common.ToString(dr["agencyuuid"]),
                                agencyname = Common.ToString(dr["agencyname"]),
                                email = Common.ToString(dr["agencyemail"]),
                                phone = Common.ToString(dr["agencyphone"])
                            }
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return model;
        }

        public BookingLocationDTO GetBookingLocationInfo(string bookinguuid, ContainerBookingDTO parent)
        {
            BookingLocationDTO model = new BookingLocationDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM booking.booking_get_location(" +
                        "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'," +
                        "p_hubid := '" + Common.HubID + "'" +
                        ");");

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        if (parent != null && string.IsNullOrEmpty(parent.bookinguuid))
                        {
                            int bookingId = Common.ToInt(dr["bookingid"]);
                            parent.bookingid = new EncryptedData()
                            {
                                NumericValue = bookingId,
                                EncryptedValue = Common.Encrypt(bookingId)
                            };
                            parent.bookinguuid = Common.ToString(dr["bookinguuid"]);
                            parent.bookingno = Common.ToString(dr["bookingno"]);
                        }
                        model = new BookingLocationDTO()
                        {
                            //bookingid = new EncryptedData()
                            //{
                            //    NumericValue = Common.ToInt(dr["bookingid"]),
                            //    EncryptedValue = Common.Encrypt(Common.ToInt(dr["bookingid"]))
                            //},
                            //bookinguuid = Common.ToString(dr["bookinguuid"]),
                            pol = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["pol_id"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["pol_id"]))
                            },
                            pol_portname = Common.ToString(dr["pol_portname"]),
                            pol_portcode = Common.ToString(dr["pol_portcode"]),
                            pol_countryid = Common.ToInt(dr["pol_countryid"]),
                            pol_countryname = Common.ToString(dr["pol_countryname"]),
                            pol_countrycode = Common.ToString(dr["pol_countrycode"]),
                            pol_countryflag = Common.ToString(dr["pol_countryflag"]),
                            pod = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["pod_id"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["pod_id"]))
                            },
                            pod_portname = Common.ToString(dr["pod_portname"]),
                            pod_portcode = Common.ToString(dr["pod_portcode"]),
                            pod_countryid = Common.ToInt(dr["pod_countryid"]),
                            pod_countryname = Common.ToString(dr["pod_countryname"]),
                            pod_countrycode = Common.ToString(dr["pod_countrycode"]),
                            pod_countryflag = Common.ToString(dr["pod_countryflag"]),
                            shipperdetailid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["shipperdetailid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["shipperdetailid"]))
                            },
                            shippername = Common.ToString(dr["shippername"]),
                            shipperpic = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["shipperpic"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["shipperpic"]))
                            },
                            shipperpiccustom = Common.ToString(dr["shipperpiccustom"]),
                            shipperpic_name = Common.ToString(dr["shipperpic_name"]),
                            shipperpic_email = Common.ToString(dr["shipperpic_email"]),
                            shipperpic_phone = Common.ToString(dr["shipperpic_phone"]),
                            shipperemail = Common.ToString(dr["shipperemail"]),
                            shipperphone = Common.ToString(dr["shipperphone"]),
                            shipperaddress = Common.ToString(dr["shipperaddress"]),
                            consigneedetailid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["consigneedetailid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["consigneedetailid"]))
                            },
                            consigneename = Common.ToString(dr["consigneename"]),
                            consigneepic = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["consigneepic"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["consigneepic"]))
                            },
                            consigneepiccustom = Common.ToString(dr["consigneepiccustom"]),
                            consigneepic_name = Common.ToString(dr["consigneepic_name"]),
                            consigneepic_email = Common.ToString(dr["consigneepic_email"]),
                            consigneepic_phone = Common.ToString(dr["consigneepic_phone"]),
                            consigneeemail = Common.ToString(dr["consigneeemail"]),
                            consigneephone = Common.ToString(dr["consigneephone"]),
                            consigneeaddress = Common.ToString(dr["consigneeaddress"]),
                            voyageuuid = Common.ToString(dr["voyageuuid"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }

        public List<ContainerBookingListDTO> GetbookingList(BookingListFilter filter)
        {
            List<ContainerBookingListDTO> list = new List<ContainerBookingListDTO>();

            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string filters = JsonConvert.SerializeObject(filter);
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booking_list('" + Common.HubID + "','" + filters + "','" + Common.LoginID + "');");

                    list = (from DataRow dr in tbl.Rows
                            select new ContainerBookingListDTO
                            {
                                row_index = Common.ToInt(dr["row_index"]),
                                total_count = Common.ToInt(dr["total_count"]),
                                bookingid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["bookingid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["bookingid"]))
                                },
                                bookinguuid = Common.ToString(dr["bookinguuid"]),
                                bookingno = Common.ToString(dr["bookingno"]),
                                bookingdate = Common.ToDateTime(dr["bookingdate"]),
                                agencydetailid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["agencydetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencydetailid"]))
                                },
                                clientdetailid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["clientdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["clientdetailid"]))
                                },
                                agencyuuid = Common.ToString(dr["agencyuuid"]),
                                clientuuid = Common.ToString(dr["clientuuid"]),
                                createdat = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                                customertype = Common.ToInt(dr["customertype"]),
                                customertypename = IDReferences.GetCustomerTypeName(Common.ToInt(dr["customertype"])),
                                pol = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["pol"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["pol"]))
                                },
                                pol_portname = Common.ToString(dr["pol_portname"]),
                                pol_portcode = Common.ToString(dr["pol_portcode"]),
                                pol_countryname = Common.ToString(dr["pol_countryname"]),
                                pol_countrycode = Common.ToString(dr["pol_countrycode"]),
                                pol_countryflag = Common.ToString(dr["pol_countryflag"]),
                                pod = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["pod"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["pod"]))
                                },
                                pod_portname = Common.ToString(dr["pod_portname"]),
                                pod_portcode = Common.ToString(dr["pod_portcode"]),
                                pod_countryname = Common.ToString(dr["pod_countryname"]),
                                pod_countrycode = Common.ToString(dr["pod_countrycode"]),
                                pod_countryflag = Common.ToString(dr["pod_countryflag"]),
                                shipperdetailid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["shipperdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["shipperdetailid"]))
                                },
                                consigneedetailid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["consigneedetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["consigneedetailid"]))
                                },
                                voyageuuid = Common.ToString(dr["voyageuuid"]),
                                voyagenumber = Common.ToString(dr["voyagenumber"]),
                                vesselname = Common.ToString(dr["vesselname"]),
                                status = FormatConvertor.ToStatus(Common.ToInt(dr["status"]), StatusEnum.Booking),
                                createdby = Common.ToInt(dr["createdby"]),
                                createdbyusername = Common.ToString(dr["createdby_username"]),
                                agencyname = Common.ToString(dr["agencyname"]),
                                customername = Common.ToString(dr["customername"]),
                                booking_details = Common.ToString(dr["booking_details"]),
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public List<ShuntingServiceDTO> GetShuntingServices()
        {
            List<ShuntingServiceDTO> list = new List<ShuntingServiceDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM booking.shunting_service_list(" +
                        "p_hubid := '" + Common.HubID + "'" +
                        ");"
                    );

                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            ShuntingServiceDTO model = new ShuntingServiceDTO()
                            {
                                serviceid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["serviceid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["serviceid"]))
                                },
                                type = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["type"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["type"]))
                                },
                                servicename = Common.ToString(dr["servicename"]),
                                orderby = Common.ToInt(dr["orderby"])
                            };
                            list.Add(model);
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

        public List<ContainerBookingDetailDTO> GetContainerBookingDetailByBookingUUId(string bookinguuid)
        {
            List<ContainerBookingDetailDTO> list = new List<ContainerBookingDetailDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable($@"SELECT * FROM booking.booking_get_containerdetail(" + "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'" + "," + "p_hubid := '" + Common.HubID + "'" + ")");

                    var rows = tbl.AsEnumerable();
                    list = rows
                        .GroupBy(r => Common.ToInt(r["bookingdetailid"]))
                        .Select(containerGrp =>
                        {
                            var first = containerGrp.First();
                            int bookingDetailId = Common.ToInt(first["bookingdetailid"]);
                            var dto = new ContainerBookingDetailDTO
                            {
                                bookingdetailid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["bookingdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["bookingdetailid"]))
                                },
                                bookingdetailuuid = Common.ToString(first["bookingdetailuuid"]),
                                bookingid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["bookingid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["bookingid"]))
                                },
                                containermodeluuid = Common.ToString(first["modeluuid"]),
                                containertypeid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["typeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["typeid"]))
                                },
                                sizeid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["sizeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["sizeid"]))
                                },
                                ownership = Common.ToInt(first["ownership"]),
                                qty = Common.ToInt(first["qty"]),
                                commodity = Common.ToString(first["commodity"]),
                                grossweight = Common.ToDecimal(first["grossweight"]),
                                volumeweight = Common.ToDecimal(first["volumeweight"]),
                                hscode = Common.ToString(first["hscode"]),
                                cargovalue = Common.ToDecimal(first["cargovalue"]),
                                packagetype = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["packagetype"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["packagetype"]))
                                },
                                expectedstuffingdate = FormatConvertor.ToDateFormat(Common.ToDateTime(first["expectedstuffingdate"])),
                                stuffinglocation = Common.ToString(first["stuffinglocation"]),
                                pickuplocation = Common.ToString(first["pickuplocation"]),
                                isdg = Common.ToBool(first["isdg"]),
                                isreefer = Common.ToBool(first["isreefer"]),
                                sizename = Common.ToString(first["sizename"]),
                                length = Common.ToString(first["length"]),
                                width = Common.ToString(first["width"]),
                                height = Common.ToString(first["height"]),
                                isocode = Common.ToString(first["iso_code"]),
                                modeldescription = Common.ToString(first["description"]),
                                containertypeuuid = Common.ToString(first["typeuuid"]),
                                containertypename = Common.ToString(first["typename"]),
                                containertypeshortname = Common.ToString(first["typeshortname"]),
                                iconid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["iconid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["iconid"]))
                                },
                                icon = Common.ToString(first["icon"]),
                                empty_full = Common.ToString(first["empty_full"]),
                            };

                            dto.services = containerGrp
                                .Where(r => Common.ToInt(r["serviceid"]) > 0)
                                .GroupBy(r => Common.ToInt(r["serviceid"]))
                                .Select(serviceGrp =>
                                {
                                    var s = serviceGrp.First();
                                    return new ContainerBookingDetailServicesDTO
                                    {
                                        bookingdetailserviceid = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(s["bookingdetailserviceid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(s["bookingdetailserviceid"]))
                                        },
                                        bookingdetailid = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(first["bookingdetailid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(first["bookingdetailid"]))
                                        },
                                        serviceid = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(s["serviceid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(s["serviceid"]))
                                        },
                                        servicetype = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(s["servicetype"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(s["servicetype"]))
                                        },
                                        servicename = Common.ToString(s["servicename"]),
                                        serviceorderby = Common.ToInt(s["serviceorderby"])
                                    };
                                })
                                .OrderBy(s => s.serviceorderby)
                                .ToList();

                            return dto;
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result DeleteBookingContainerByUUID(string bookingdetailuuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * FROM booking.booking_delete_containerdetail(
                      p_bookingdetailuuid := '" + Common.Escape(bookingdetailuuid) + @"',
                      p_deletedby := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot delete container details.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public List<ContainerAdditionalServicesDTO> GetContainerAdditionalServices()
        {
            List<ContainerAdditionalServicesDTO> list = new List<ContainerAdditionalServicesDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM booking.container_additional_services_list(" +
                        "p_hubid := '" + Common.HubID + "'" + ");"
                    );

                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            ContainerAdditionalServicesDTO model = new ContainerAdditionalServicesDTO()
                            {
                                additionalserviceid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["additionalserviceid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["additionalserviceid"]))
                                },
                                additionalserviceuuid = Common.ToString(dr["additionalserviceuuid"]),
                                servicename = Common.ToString(dr["servicename"]),
                                description = Common.ToString(dr["description"]),
                                orderby = Common.ToInt(dr["orderby"])
                            };
                            list.Add(model);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list.OrderBy(x => x.orderby).ToList();
        }

        public List<BookingAdditionalServicesDTO> GetBookingAdditionalServices(string bookinguuid)
        {
            List<BookingAdditionalServicesDTO> services = new List<BookingAdditionalServicesDTO>();

            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable(
                        "SELECT * FROM booking.booking_get_additional_services(" +
                        "p_bookinguuid := '" + bookinguuid + "', " +
                        "p_hubid := '" + Common.HubID + "'" +
                        ");"
                    );

                    services = (from DataRow dr in tbl.Rows
                                select new BookingAdditionalServicesDTO
                                {
                                    bookingadditionalserviceid = new EncryptedData
                                    {
                                        NumericValue = Common.ToInt(dr["bookingadditionalserviceid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["bookingadditionalserviceid"]))
                                    },
                                    bookingid = new EncryptedData
                                    {
                                        NumericValue = Common.ToInt(dr["bookingid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["bookingid"]))
                                    },
                                    additionalserviceid = new EncryptedData
                                    {
                                        NumericValue = Common.ToInt(dr["additionalserviceid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["additionalserviceid"]))
                                    },
                                    locationtypeid = new EncryptedData
                                    {
                                        NumericValue = Common.ToInt(dr["locationtypeid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationtypeid"]))
                                    },
                                    uom = Common.ToString(dr["uom"]),
                                    additionalserviceuuid = Common.ToString(dr["additionalserviceuuid"]),
                                    quantity = Common.ToDecimal(dr["quantity"]),
                                    servicename = Common.ToString(dr["servicename"]),
                                    description = Common.ToString(dr["description"]),
                                    order = Common.ToInt(dr["orderby"]),
                                    type = Common.ToInt(dr["type"])
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return services;
        }

        public Result SaveBookingAdditionalServices(string bookingid, List<BookingAdditionalServicesDTO> services)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    var servicesJsonObj = services.Select(s => new
                    {
                        additionalserviceid = Common.Decrypt(s.additionalserviceid.EncryptedValue),
                        quantity = s.quantity,
                        locationtypeid = Common.Decrypt(s.locationtypeid.EncryptedValue),
                        uom = s.uom,
                    });

                    string servicesJson = System.Text.Json.JsonSerializer.Serialize(servicesJsonObj);

                    string query = @"SELECT * FROM booking.booking_save_additional_sevices(
                                     p_bookingid := '" + Common.Decrypt(bookingid) + @"',
                                     p_services := '" + Common.Escape(servicesJson) + @"'::jsonb,
                                     p_hubid := '" + Common.HubID + @"',
                                     p_createdby := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save additional services.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public Result SaveQuotation(string bookinguuid, QuotationHeaderDTO model)
        {
            Result result = new Result();
            try
            {
                var detailsJson = JsonConvert.SerializeObject(
                    model.details.Select(d => new
                    {
                        quotationdetailid = d.quotationdetailid != null ? Common.Decrypt(d.quotationdetailid.EncryptedValue) : 0,
                        quotationdetailuuid = d.quotationdetailuuid,
                        description = Common.Escape(d.description ?? ""),
                        remarks = Common.Escape(d.remarks ?? ""),
                        uom = Common.Escape(d.uom ?? ""),
                        qty = d.qty,
                        amount = d.amount,
                        isdeleted = d.isdeleted,
                        jobtypedetailuuid = d.jobtypedetailuuid,
                        containertypeid = Common.Decrypt(d.containertypeid.EncryptedValue),
                        containersizeid = Common.Decrypt(d.containersizeid.EncryptedValue),
                        fullempty = d.fullempty,
                        tax = d.tax,
                        templateprice = d.templateprice
                    })
                );

                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query =
                        "SELECT * FROM booking.quotation_save(" +
                        "p_quotationid := '" + Common.Decrypt(model.quotationid.EncryptedValue) + "'," +
                        "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'::uuid," +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_quotationno := '" + Common.Escape(model.quotationno) + "'," +
                        "p_currency := '" + Common.Escape(model.currency) + "'," +
                        "p_expirydate := '" + Common.ToDateTime(model.expirydate.DBFormattedText) + "'," +
                        "p_agencydetailid := '" + Common.Decrypt(model.agencydetailid.EncryptedValue) + "'," +
                        "p_agencyuuid := NULL," +
                        "p_quotedate := '" + Common.ToDateTime(model.quotedate.DBFormattedText) + "'," +
                        "p_billto := '" + Common.Escape(model.billto) + "'," +
                        "p_termsandconditions := '" + Common.Escape(model.termsandconditions) + "'," +
                        "p_remarks := '" + Common.Escape(model.remarks) + "'," +
                        "p_details := '" + Common.Escape(detailsJson) + "'::jsonb," +
                        "p_createdby := '" + Common.LoginID + "'," +
                        "p_quotationfor := '" + Common.Decrypt(model.quotationfor.EncryptedValue) + "'" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Common.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save Quotation.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public List<QuotationList> GetQuotationListBookingUUID(string bookinguuid)
        {
            List<QuotationList> quotations = new List<QuotationList>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query =
                        "SELECT * FROM booking.quotation_list_by_booking(" +
                        "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'::uuid, " +
                        "p_hubid := '" + Common.HubID + "'" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl == null || tbl.Rows.Count == 0)
                        return quotations;

                    foreach (DataRow row in tbl.Rows)
                    {
                        QuotationList dto = new QuotationList
                        {
                            quotationid = new EncryptedData
                            {
                                NumericValue = Common.ToInt(row["quotationid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(row["quotationid"]))
                            },
                            quotationuuid = Common.ToString(row["quotationuuid"]),
                            quotationno = Common.ToString(row["quotationno"]),
                            bookinguuid = Common.ToString(row["bookinguuid"]),
                            currency = Common.ToString(row["currency"]),
                            quotedate = FormatConvertor.ToDateFormat(Common.ToDateTime(row["quotedate"])),
                            expirydate = FormatConvertor.ToDateFormat(Common.ToDateTime(row["expirydate"])),
                            status = FormatConvertor.ToStatus(Common.ToInt(row["status"]), StatusEnum.Quotation),
                            approvalstatus = Common.ToInt(row["approvalstatus"]),
                            approvedby = Common.ToInt(row["approvedby"]),
                            approveddate = FormatConvertor.ToDateFormat(Common.ToDateTime(row["approveddate"])),
                            createdat = FormatConvertor.ToDateFormat(Common.ToDateTime(row["createdat"])),
                            agencydetailid = new EncryptedData
                            {
                                NumericValue = Common.ToInt(row["agencydetailid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(row["agencydetailid"]))
                            },
                            agencyuuid = Common.ToString(row["agencyuuid"]),
                            agencyname = Common.ToString(row["agencyname"]),
                            agencyemail = Common.ToString(row["agencyemail"]),
                            agencyphone = Common.ToString(row["agencyphone"]),
                            totalamount = Common.ToDecimal(row["totalamount"]),
                            totalitems = Common.ToInt(row["totalitems"]),
                            statusname = Common.ToString(row["statusname"]),
                            approvername = Common.ToString(row["approvername"]),
                            quotationfor = new EncryptedData
                            {
                                NumericValue = Common.ToInt(row["quotationfor"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(row["quotationfor"]))
                            }
                        };
                        quotations.Add(dto);
                    }
                    quotations.ForEach(x =>
                    {
                        x.quotationno = Codes.GetCodes(x.quotationid.NumericValue, x.createdat.Value, "QTN", x.quotationno);
                    });
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return quotations;
        }

        public QuotationHeaderDTO GetQuotationByUUID(string quotationuuid)
        {
            QuotationHeaderDTO dto = new QuotationHeaderDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = "SELECT * FROM booking.quotation_get_uuid(" +
                                   "p_quotationuuid := '" + Common.Escape(quotationuuid) + "'::uuid, " +
                                   "p_hubid := '" + Common.HubID + "'" + ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl == null || tbl.Rows.Count == 0)
                        return new QuotationHeaderDTO();

                    var rows = tbl.AsEnumerable();
                    var first = rows.First();

                    dto = new QuotationHeaderDTO
                    {
                        quotationid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(first["quotationid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(first["quotationid"]))
                        },
                        status = FormatConvertor.ToStatus(Common.ToInt(first["status"]), StatusEnum.Quotation),
                        quotationuuid = Common.ToString(first["quotationuuid"]),
                        currency = Common.ToString(first["currency"]),
                        quotedate = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(first["quotedate"])),
                        expirydate = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(first["expirydate"])),
                        billto = Common.ToString(first["billto"]),
                        termsandconditions = Common.ToString(first["termsandconditions"]),
                        remarks = Common.ToString(first["remarks"]),
                        bookinguuid = Common.ToString(first["bookinguuid"]),
                        agencydetailid = new EncryptedData
                        {
                            NumericValue = Common.ToInt(first["agencydetailid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(first["agencydetailid"]))
                        },
                        agencyuuid = Common.ToString(first["agencyuuid"]),
                        agencyname = Common.ToString(first["agencyname"]),
                        createdat = Common.ToDateTime(first["createdat"]),
                        quotationfor = new EncryptedData
                        {
                            NumericValue = Common.ToInt(first["quotationfor"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(first["quotationfor"]))
                        },
                        quotationforname = Common.ToString(first["clientname"]),
                    };
                    dto.quotationno = Codes.GetCodes(Common.ToInt(first["quotationid"]), Common.ToDateTime(first["createdat"]), "QTN", Common.ToString(first["quotationno"]));

                    dto.details = rows
                        .Where(r => Common.ToInt(r["quotationdetailid"]) > 0)
                        .GroupBy(r => Common.ToInt(r["quotationdetailid"]))
                        .Select(g =>
                        {
                            var d = g.First();
                            return new QuotationDetailDTO
                            {
                                quotationdetailid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(d["quotationdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(d["quotationdetailid"]))
                                },
                                quotationdetailuuid = Common.ToString(d["quotationdetailuuid"]),
                                quotationid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["quotationid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["quotationid"]))
                                },
                                description = Common.ToString(d["description"]),
                                remarks = Common.ToString(d["detailremarks"]),
                                uom = Common.ToString(d["uom"]),
                                qty = Common.ToDecimal(d["qty"]),
                                amount = Common.ToDecimal(d["amount"]),
                                jobtypedetailuuid = Common.ToString(d["jobtypedetailuuid"]),
                                containertypeid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(d["containertypeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(d["containertypeid"]))
                                },
                                containersizeid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(d["containersizeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(d["containersizeid"]))
                                },
                                fullempty = Common.ToString(d["fullempty"]),
                                containertypename = Common.ToString(d["containertypename"]),
                                containersizename = Common.ToString(d["containersizename"]),
                                tax = Common.ToDecimal(d["tax"]),
                                templateprice = Common.ToDecimal(d["templateprice"]),
                            };
                        })
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return dto;
        }

        public Result DeleteQuotationByUUID(string quotationuuid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * FROM booking.quotation_delete(
                      p_quotationuuid := '" + Common.Escape(quotationuuid) + @"',
                      p_hubid := '" + Common.HubID + @"',
                      p_deletedby := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        string resultId = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();
                        string uuid = tbl.Rows[0][2].ToString();
                        if (resultId == "1")
                        {
                            result = Common.SuccessMessage(message);
                            result.TargetUUID = uuid;
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot delete container details.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }


        public List<ContainerAllocationDTO> GetContainerAllocations(string bookingdetailuuid, string bookinguuid)
        {
            List<ContainerAllocationDTO> list = new List<ContainerAllocationDTO>();

            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * FROM booking.container_allocation_list(
                                     p_bookingdetailuuid := '" + bookingdetailuuid + @"',
                                     p_bookinguuid := '" + bookinguuid + @"', 
                                     p_hubid := '" + Common.HubID + @"');";

                    DataTable tbl = db.GetDataTable(query);

                    var rows = tbl.AsEnumerable();
                    list = rows.GroupBy(r => Common.ToInt(r["locationid"]))
                        .Select(allocationgrouping =>
                        {
                            var dr = allocationgrouping.First();
                            var dto = new ContainerAllocationDTO
                            {
                                LocationName = Common.ToString(dr["locationname"]),
                                LocationId = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["locationid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["locationid"]))
                                },
                                LocationUuid = Common.ToString(dr["locationuuid"]),
                                LocationType = Common.ToString(dr["locationtype"]),
                                LocationTypeIcon = Common.ToInt(dr["locationtypeicon"]),
                                PortName = Common.ToString(dr["portname"]),
                                CountryName = Common.ToString(dr["countryname"]),
                                CountryFlag = Common.ToString(dr["countryflag"]),
                                PortCode = Common.ToString(dr["portcode"]),
                                CountryCode = Common.ToString(dr["countrycode"]),
                            };

                            dto.Models = (from DataRow drinner in allocationgrouping
                                          select new ContainerAllocationModelDTO
                                          {
                                              ContainerTypeName = Common.ToString(drinner["containertypename"]),
                                              ContainerModelId = new EncryptedData
                                              {
                                                  NumericValue = Common.ToInt(drinner["containermodelid"]),
                                                  EncryptedValue = Common.Encrypt(Common.ToInt(drinner["containermodelid"]))
                                              },
                                              ContainerSizeName = Common.ToString(drinner["containersizename"]),
                                              ContainerModelUuid = Common.ToString(drinner["containermodeluuid"]),
                                              ContainerModelIso = Common.ToString(drinner["containermodeliso"]),
                                              AvailableCount = Common.ToInt(drinner["availablecount"]),
                                              BlockedCount = Common.ToInt(drinner["blockedcount"]),
                                              ComingSoon = Common.ToInt(drinner["comingsoon"]),
                                              SelectedCount = Common.ToInt(drinner["selectedcount"]),
                                          }).ToList();

                            return dto;
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public Result SaveContainerAllocation(string bookingid, string bookingdetailid, List<ContainerAllocationDTO> allocations)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    var details = allocations
                                    .SelectMany(location => location.Models
                                        .Where(model => model.SelectedCount > 0)
                                        .Select(model => new
                                        {
                                            reservationid = Common.Decrypt(model.ReservationID.EncryptedValue),
                                            locationid = Common.Decrypt(location.LocationId.EncryptedValue),
                                            containermodelid = Common.Decrypt(model.ContainerModelId.EncryptedValue),
                                            qty = model.SelectedCount
                                        })
                                    ).ToList();

                    string detailsJson = System.Text.Json.JsonSerializer.Serialize(details);

                    string query = @"SELECT * FROM booking.container_allocation_save(
                                     p_bookingid := '" + Common.Decrypt(bookingid) + @"',
                                     p_bookingdetailid := '" + Common.Decrypt(bookingdetailid) + @"',
                                     p_json := '" + Common.Escape(detailsJson) + @"'::jsonb,
                                     p_hubid := '" + Common.HubID + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save container allocation.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public Result DeleteContainerAllocation(string bookingid, string bookingdetailid, string locationid, string modelid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * FROM booking.container_allocation_delete(
                                     p_bookingid := '" + Common.Decrypt(bookingid) + @"',
                                     p_bookingdetailid := '" + Common.Decrypt(bookingdetailid) + @"',
                                     p_locationid := '" + Common.Decrypt(locationid) + @"',
                                     p_modelid := '" + Common.Decrypt(modelid) + @"',   
                                     p_hub_id := '" + Common.HubID + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot delete container allocation.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public List<ContainerSelectionDTO> GetContainerSelection(string bookinguuid)
        {
            List<ContainerSelectionDTO> list = new List<ContainerSelectionDTO>();

            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * 
                                    FROM booking.container_equip_selection_list(
                                    p_bookinguuid := '" + bookinguuid + @"',
                                    p_hubid := '" + Common.HubID + @"'
                                    );";

                    DataTable tbl = db.GetDataTable(query);
                    var rows = tbl.AsEnumerable();
                    list = rows
                            .GroupBy(r => Common.ToInt(r["locationid"]))
                            .Select(locationGroup =>
                            {
                                var loc = locationGroup.First();

                                int locationId = Common.ToInt(loc["locationid"]);

                                var dto = new ContainerSelectionDTO
                                {
                                    LocationName = Common.ToString(loc["locationname"]),
                                    LocationId = new EncryptedData
                                    {
                                        NumericValue = locationId,
                                        EncryptedValue = Common.Encrypt(locationId)
                                    },
                                    LocationUuid = Common.ToString(loc["locationuuid"]),
                                    LocationType = Common.ToString(loc["locationtypename"]),
                                    PortName = Common.ToString(loc["portname"]),
                                    PortCode = Common.ToString(loc["portcode"]),
                                    CountryName = Common.ToString(loc["countryname"]),
                                    CountryCode = Common.ToString(loc["countrycode"]),
                                    CountryFlag = Common.ToString(loc["flag"])
                                };

                                dto.Details = locationGroup
                                    .GroupBy(x => Common.ToString(x["modeluuid"]))
                                    .Select(modelGroup =>
                                    {
                                        var first = modelGroup.First();

                                        int reservationId = Common.ToInt(first["reservationid"]);
                                        int bookingDetailId = Common.ToInt(first["bookingdetailid"]);
                                        int modelId = Common.ToInt(first["modelid"]);

                                        var detail = new ContainerSelectionDetailDTO
                                        {
                                            ReservationID = new EncryptedData
                                            {
                                                NumericValue = reservationId,
                                                EncryptedValue = Common.Encrypt(reservationId)
                                            },
                                            RequiredCount = modelGroup.GroupBy(x => Common.ToInt(x["bookingdetailid"])).ToList().Sum(g => Common.ToInt(g.First()["qty"])),
                                            ContainerDetailID = new EncryptedData
                                            {
                                                NumericValue = bookingDetailId,
                                                EncryptedValue = Common.Encrypt(bookingDetailId)
                                            },
                                            ContainerTypeName = Common.ToString(first["typename"]),
                                            ContainerSizeName = Common.ToString(first["sizename"]),
                                            ContainerModelId = new EncryptedData
                                            {
                                                NumericValue = modelId,
                                                EncryptedValue = Common.Encrypt(modelId)
                                            },
                                            ContainerModelUuid = Common.ToString(first["modeluuid"]),
                                            ContainerModelIso = Common.ToString(first["iso_code"]),
                                            Containers = modelGroup.Select(c =>
                                            {
                                                int containerId = Common.ToInt(c["containerid"]);

                                                return new SelectionItemDTO
                                                {
                                                    ContainerID = new EncryptedData
                                                    {
                                                        NumericValue = containerId,
                                                        EncryptedValue = Common.Encrypt(containerId)
                                                    },
                                                    ContainerUUID = Common.ToString(c["containeruuid"]),
                                                    EquipmentNo = Common.ToString(c["equipmentno"]),
                                                    LastBookingDate = FormatConvertor.ToClientDateTimeFormat(
                                                        Common.ToDateTime(c["lastbookingdate"])
                                                    ),
                                                    AllocationBookingUUID = Common.ToString(c["allocation_bookinguuid"]),
                                                    AllocationDateTime = FormatConvertor.ToClientDateTimeFormat(
                                                        Common.ToDateTime(c["allocation_datetime"])
                                                    )
                                                };
                                            }).ToList()
                                        };

                                        return detail;
                                    })
                                    .ToList();

                                return dto;
                            })
                            .ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

            return list;
        }

        public Result SaveContainerBookingPickSelection(List<ContainerBookingDetailDTO> selections)
        {
            Result result = new Result();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    List<string> otherIds = new List<string>();
                    foreach (var item in selections)
                    {
                        var bookingdetailuuid = item.bookingdetailuuid;
                        otherIds.Add($"{bookingdetailuuid}");
                    }

                    DataTable tbl = db.GetDataTable(
                        "SELECT * FROM sandbox.create_pick_selection(" +
                        "p_hubid := " + Common.HubID + "," +
                        "p_uuids := NULL," +
                        "p_otherids := ARRAY['" + string.Join("','", otherIds) + "']::text[]," +
                        "p_createdby := " + Common.LoginID + ");"
                    );

                    if (tbl.Rows.Count != 0)
                    {
                        if (Common.ToInt(tbl.Rows[0][0]) > 0)
                        {
                            result = Common.SuccessMessage("Success");
                            result.TargetUUID = Convert.ToString(tbl.Rows[0][2].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot process request");
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

        public List<ContainerBookingDetailDTO> GetContainerBookingDetailByPickUUID(string pickuuid)
        {
            List<ContainerBookingDetailDTO> list = new List<ContainerBookingDetailDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        $@"SELECT * FROM booking.booking_get_containerdetail_by_pickuuid(
                           p_pick_uuid := '{Common.Escape(pickuuid)}',
                           p_hubid := '{Common.HubID}')");

                    var rows = tbl.AsEnumerable();
                    list = rows
                        .GroupBy(r => Common.ToInt(r["bookingdetailid"]))
                        .Select(containerGrp =>
                        {
                            var first = containerGrp.First();
                            var dto = new ContainerBookingDetailDTO
                            {
                                bookingdetailid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["bookingdetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["bookingdetailid"]))
                                },
                                bookingdetailuuid = Common.ToString(first["bookingdetailuuid"]),
                                bookingid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["bookingid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["bookingid"]))
                                },
                                bookinguuid = Common.ToString(first["bookinguuid"]),
                                containermodeluuid = Common.ToString(first["modeluuid"]),
                                containertypeid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["typeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["typeid"]))
                                },
                                sizeid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["sizeid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["sizeid"]))
                                },
                                ownership = Common.ToInt(first["ownership"]),
                                qty = Common.ToInt(first["qty"]),
                                commodity = Common.ToString(first["commodity"]),
                                grossweight = Common.ToDecimal(first["grossweight"]),
                                volumeweight = Common.ToDecimal(first["volumeweight"]),
                                hscode = Common.ToString(first["hscode"]),
                                cargovalue = Common.ToDecimal(first["cargovalue"]),
                                packagetype = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["packagetype"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["packagetype"]))
                                },
                                expectedstuffingdate = FormatConvertor.ToDateFormat(
                                    Common.ToDateTime(first["expectedstuffingdate"])
                                ),
                                stuffinglocation = Common.ToString(first["stuffinglocation"]),
                                pickuplocation = Common.ToString(first["pickuplocation"]),
                                isdg = Common.ToBool(first["isdg"]),
                                isreefer = Common.ToBool(first["isreefer"]),
                                sizename = Common.ToString(first["sizename"]),
                                length = Common.ToString(first["length"]),
                                width = Common.ToString(first["width"]),
                                height = Common.ToString(first["height"]),
                                isocode = Common.ToString(first["iso_code"]),
                                modeldescription = Common.ToString(first["description"]),
                                containertypeuuid = Common.ToString(first["typeuuid"]),
                                containertypename = Common.ToString(first["typename"]),
                                containertypeshortname = Common.ToString(first["typeshortname"]),
                                iconid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(first["iconid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(first["iconid"]))
                                },
                                icon = Common.ToString(first["icon"]),
                                empty_full = Common.ToString(first["empty_full"]),
                            };
                            dto.services = containerGrp
                                .Where(r => Common.ToInt(r["serviceid"]) > 0)
                                .GroupBy(r => Common.ToInt(r["serviceid"]))
                                .Select(serviceGrp =>
                                {
                                    var s = serviceGrp.First();
                                    return new ContainerBookingDetailServicesDTO
                                    {
                                        bookingdetailserviceid = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(s["bookingdetailserviceid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(s["bookingdetailserviceid"]))
                                        },
                                        bookingdetailid = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(first["bookingdetailid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(first["bookingdetailid"]))
                                        },
                                        serviceid = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(s["serviceid"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(s["serviceid"]))
                                        },
                                        servicetype = new EncryptedData
                                        {
                                            NumericValue = Common.ToInt(s["servicetype"]),
                                            EncryptedValue = Common.Encrypt(Common.ToInt(s["servicetype"]))
                                        },
                                        servicename = Common.ToString(s["servicename"]),
                                        serviceorderby = Common.ToInt(s["serviceorderby"])
                                    };
                                }).ToList();
                            return dto;
                        }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public List<QuotationList> GetQuotationList(QuotationListFilter filter)
        {
            List<QuotationList> list = new List<QuotationList>();

            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string filters = JsonConvert.SerializeObject(filter);
                    string query = @"SELECT * FROM booking.quotation_list(
                     p_hubid   := '" + Common.HubID + @"',
                     p_filters := '" + filters + @"'::jsonb,
                     p_userid  := '" + Common.LoginID + @"'
                 );";

                    DataTable tbl = db.GetDataTable(query);
                    if (tbl == null || tbl.Rows.Count == 0)
                        return list;
                    list = (from DataRow dr in tbl.Rows
                            select new QuotationList
                            {
                                row_index = Common.ToInt(dr["row_index"]),
                                total_count = Common.ToInt(dr["total_count"]),
                                quotationid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["quotationid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["quotationid"]))
                                },
                                quotationuuid = Common.ToString(dr["quotationuuid"]),
                                quotationno = Common.ToString(dr["quotationno"]),
                                bookingno = Common.ToString(dr["bookingno"]),
                                createdBy = Common.ToString(dr["createdby_username"]),
                                bookinguuid = Common.ToString(dr["bookinguuid"]),
                                currency = Common.ToString(dr["currency"]),
                                status = FormatConvertor.ToStatus(Common.ToInt(dr["status"]), StatusEnum.Quotation),
                                createdat = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                                quotedateOnly = Common.ToDateTimeString(dr["quotedate"], "MMM dd, yyyy"),
                                quotedate = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                                totalamountformatted = Display.DisplayCurrency(Common.ToDecimal(dr["totalamount"])),
                                expirydate = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["expirydate"])),
                                expirydateOnly = Common.ToDateTimeString(dr["expirydate"], "MMM dd, yyyy"),
                                timeago = LogConvertor.GetTimeAgo(Common.ToDateTime(dr["quotedate"]), false),
                                ETADays = Common.GetETADays(Common.ToDateTime(dr["expirydate"])),
                                agencydetailid = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["agencydetailid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencydetailid"]))
                                },
                                agencyuuid = Common.ToString(dr["agencyuuid"]),
                                agencyname = Common.ToString(dr["agencyname"]),
                                customername = Common.ToString(dr["customername"]),
                                customertype = Common.ToInt(dr["customername"]),
                                customertypename = IDReferences.GetCustomerTypeName(Common.ToInt(dr["customertype"])),
                                agencyemail = Common.ToString(dr["agencyemail"]),
                                agencyphone = Common.ToString(dr["agencyphone"]),
                                totalamount = Common.ToDecimal(dr["totalamount"]),
                                totalitems = Common.ToInt(dr["totalitems"]),
                                billto = Common.ToString(dr["billto"]),
                                quotationfor = new EncryptedData
                                {
                                    NumericValue = Common.ToInt(dr["quotationfor"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["quotationfor"]))
                                }
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public List<QuotationStatusCountDTO> GetQuotationStatusCount(QuotationListFilter filter)
        {
            List<QuotationStatusCountDTO> list = new List<QuotationStatusCountDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string jsonFilters = JsonConvert.SerializeObject(filter);

                    string query = "SELECT * FROM booking.quotation_status_count(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_filters := '" + Common.Escape(jsonFilters) + "'::jsonb," +
                                   "p_userid := " + Common.LoginID + "" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(new QuotationStatusCountDTO
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

        public List<BookingAllocationAcquireDTO> AcquireContainer(string bookinguuid, List<AcquireDTO> bookingids, string locationuuid, string modeluuid)
        {
            List<BookingAllocationAcquireDTO> list = new List<BookingAllocationAcquireDTO>();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string jsonFilters = JsonConvert.SerializeObject(bookingids.Select(x => new
                    {
                        containerid = Common.Decrypt(x.containerid),
                        isdeleted = x.isdeleted
                    }));

                    string query = "SELECT * FROM booking.container_selection_acquire(" +
                                   "p_hubid := " + Common.HubID + "," +
                                   "p_bookinguuid := " + Common.GetUUID(bookinguuid) + "," +
                                   "p_json := '" + Common.Escape(jsonFilters) + "'::jsonb," +
                                   "p_locationuuid := " + Common.GetUUID(locationuuid) + "," +
                                   "p_modeluuid := " + Common.GetUUID(modeluuid) + "," +
                                   "p_userid := " + Common.LoginID + "" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null)
                    {
                        foreach (DataRow dr in tbl.Rows)
                        {
                            list.Add(new BookingAllocationAcquireDTO
                            {
                                ContainerID = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["containerid"])),
                                    NumericValue = Common.ToInt(dr["containerid"]),
                                },
                                AllocationBookingUUID = Common.ToString(dr["allocation_bookinguuid"])
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
        public List<BookingStatusCountDTO> GetBookingStatusCount(BookingListFilter filter)
        {
            List<BookingStatusCountDTO> list = new List<BookingStatusCountDTO>();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string filters = JsonConvert.SerializeObject(filter);
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booking_status_count('" + Common.HubID + "','" + filters + "','" + Common.LoginID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new BookingStatusCountDTO
                            {
                                StatusId = Common.ToInt(dr["status_id"]),
                                StatusCount = Common.ToLong(dr["status_count"])

                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public Result SaveContainerSelection(string bookingid, List<ContainerSelectionDTO> selections)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    var details = selections
                                    .SelectMany(location => location.Details
                                        .SelectMany(model => model.Containers
                                        .Where(cont => cont.Selected)
                                        .Select(cont => new
                                        {
                                            containerid = Common.Decrypt(cont.ContainerID.EncryptedValue),
                                            isdeleted = cont.IsDeleted
                                        })
                                    )).ToList();

                    string detailsJson = System.Text.Json.JsonSerializer.Serialize(details);

                    string query = @"SELECT * FROM booking.container_selection_save(
                                     p_bookingid := '" + Common.Decrypt(bookingid) + @"',
                                     p_json := '" + Common.Escape(detailsJson) + @"'::jsonb,
                                     p_hubid := '" + Common.HubID + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save container selection.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public List<ContainerAllottedDTO> GetContainerAllotment(string bookinguuid)
        {
            List<ContainerAllottedDTO> list = new List<ContainerAllottedDTO>();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.container_selection_get('" + Common.HubID + "','" + bookinguuid + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new ContainerAllottedDTO
                            {
                                containerid = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["containerid"])),
                                    NumericValue = Common.ToInt(dr["containerid"]),
                                }

                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public Result ConfirmContainerSelection(string bookingid)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * FROM booking.container_selection_confirm(
                                     p_bookingid := '" + Common.Decrypt(bookingid) + @"',
                                     p_hubid := '" + Common.HubID + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot confirm container selection.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }
        public List<BookedContainerDTO> GetBookedContainers(string bookinguuid, BookedContainerFilter filters)
        {
            List<BookedContainerDTO> list = new List<BookedContainerDTO>();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string filterJson = System.Text.Json.JsonSerializer.Serialize(filters);
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booked_container_list('" + Common.HubID + "','" + bookinguuid + "','" + filterJson + "','" + Common.LoginID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new BookedContainerDTO
                            {
                                RowIndex = Common.ToInt(dr["row_index"]),
                                TotalCount = Common.ToLong(dr["total_count"]),

                                ContainerId = new EncryptedData()
                                {
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["containerid"])),
                                    NumericValue = Common.ToInt(dr["containerid"]),
                                },
                                ContainerUuid = Common.ToString(dr["containeruuid"]),
                                ContainerNo = Common.ToString(dr["containerno"]),

                                IsoCode = Common.ToString(dr["iso_code"]),
                                ContainerTypeName = Common.ToString(dr["containertypename"]),
                                ContainerSizeName = Common.ToString(dr["containersizename"]),

                                OperatorCode = Common.ToInt(dr["operator_code"]),
                                IsDamaged = Common.ToBool(dr["isdamaged"]),

                                VoyageUuid = Common.ToString(dr["voyageuuid"]),
                                VoyageNumber = Common.ToString(dr["voyagenumber"]),
                                //VesselName = Common.ToString(dr["vesselname"]), // if present in query

                                PolPortCode = Common.ToString(dr["pol_portcode"]),
                                PolPortName = Common.ToString(dr["pol_portname"]),
                                PolCountryCode = Common.ToString(dr["pol_countrycode"]),
                                PolCountryName = Common.ToString(dr["pol_countryname"]),
                                PolCountryFlag = Common.ToString(dr["pol_countryflag"]),

                                PodPortCode = Common.ToString(dr["pod_portcode"]),
                                PodPortName = Common.ToString(dr["pod_portname"]),
                                PodCountryCode = Common.ToString(dr["pod_countrycode"]),
                                PodCountryName = Common.ToString(dr["pod_countryname"]),
                                PodCountryFlag = Common.ToString(dr["pod_countryflag"]),

                                CurrentLocationName = Common.ToString(dr["current_locationname"]),
                                CurrentPortName = Common.ToString(dr["current_portname"]),
                                CurrentCountryName = Common.ToString(dr["current_countryname"]),
                                CurrentCountryFlag = Common.ToString(dr["current_countryflag"]),
                                MoveName = Common.ToString(dr["movename"]),
                                MoveIconID = Common.ToInt(dr["iconid"]),
                                IsEmpty = Common.ToBool(dr["isempty"]),
                                ContainerTypeID = Common.ToInt(dr["typeid"]),
                                ContainerSizeID = Common.ToInt(dr["sizeid"]),
                                LastMoveDate = Display.DisplayDateTimeHumanFriendly(Common.ToDateTime(dr["lastmovedatetime"]))

                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public Result SendQuotationForApproval(SendApproval approval)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string query = @"SELECT * FROM booking.quotation_approve(
                                     p_quoteuuid := '" + approval.UUID + @"',
                                     p_hubid := '" + Common.HubID + @"',
                                     p_approvestatus := '" + approval.ApproveStatus + @"',
                                     p_assignedto := '" + Common.Decrypt(approval.AssignedTo.EncryptedValue) + @"',
                                     p_approveorder := '" + approval.ApproveOrder + @"',
                                     p_approvaltype := '" + approval.ApprovalType + @"',
                                     p_comments := '" + Common.Escape(approval.Comments) + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot Approve quotation.");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public Result SaveSummaryInfo(BookingSummaryDTO summary)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    string otherFeesJson = JsonConvert.SerializeObject(summary.otherfees ?? new List<OtherFeesDTO>());

                    string query = "SELECT * FROM booking.booking_save_summary(" +
                        "p_summaryid := '" + Common.Decrypt(summary.summaryid.EncryptedValue) + "'," +
                        "p_bookinguuid := '" + summary.bookinguuid + "'," +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_intramodal_transport := '" + Common.ToBool(summary.intramodal_transport) + "'," +
                        "p_stuffing_unstuffing_units := '" + Common.ToString(summary.stuffing_unstuffing_units) + "'," +
                        "p_pickupandreturn_empty_rentalunits := '" + Common.ToString(summary.pickupandreturn_empty_rentalunits) + "'," +
                        "p_dropoff := '" + Common.ToString(summary.dropoff) + "'," +
                        "p_cutoffdatetime := null," +
                        "p_additionalterms := '" + Common.ToString(summary.additionalterms) + "'," +
                        "p_createdby := '" + Common.LoginID + "'," +
                        "p_currency := '" + summary.currency + "'," +
                        "p_freetimepol := " + Common.ToInt(summary.freetimepol) + "," +
                        "p_freetimepod := " + Common.ToInt(summary.freetimepod) + "," +
                        "p_freightchargeamount := " + Common.ToDecimal(summary.freightchargeamount) + "," +
                        "p_freightchargecomments := '" + Common.ToString(summary.freightchargecomments) + "'," +
                        "p_totalothercharges := " + Common.ToDecimal(summary.totalothercharges) + "," +
                        "p_otherfees := '" + otherFeesJson.Replace("'", "''") + "'::jsonb" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        string resultId = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();
                        string uuid = tbl.Rows[0][2].ToString();
                        if (resultId == "1")
                        {
                            result = Common.SuccessMessage(message);
                            result.TargetUUID = uuid;
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save booking summary.");
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

        public BookingSummaryDTO GetBookingSummaryInfo(string bookinguuid)
        {
            BookingSummaryDTO model = new BookingSummaryDTO();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = Db.GetDataTable(
                        "SELECT * FROM booking.booking_get_summary(" +
                        "p_bookinguuid := '" + Common.Escape(bookinguuid) + "'," +
                        "p_hubid := '" + Common.HubID + "'" + ");");

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        model.summaryid = new EncryptedData()
                        {
                            NumericValue = Common.ToInt(dr["summaryid"]),
                            EncryptedValue = Common.Encrypt(Common.ToInt(dr["summaryid"]))
                        };
                        model.summaryuuid = Common.ToString(dr["summaryuuid"]);
                        model.bookinguuid = Common.ToString(dr["bookinguuid"]);
                        model.intramodal_transport = Common.ToBool(dr["intramodal_transport"]);
                        model.stuffing_unstuffing_units = Common.ToString(dr["stuffing_unstuffing_units"]);
                        model.pickupandreturn_empty_rentalunits = Common.ToString(dr["pickupandreturn_empty_rentalunits"]);
                        model.dropoff = Common.ToString(dr["dropoff"]);
                        model.cutoffdatetime = FormatConvertor.ToDateTimeFormat(Common.ToDateTime(dr["cutoffdatetime"]));
                        model.additionalterms = Common.ToString(dr["additionalterms"]);
                        model.currency = Common.ToString(dr["currency"]);
                        model.freetimepol = Common.ToInt(dr["freetimepol"]);
                        model.freetimepod = Common.ToInt(dr["freetimepod"]);
                        model.freightchargeamount = Common.ToDecimal(dr["freightchargeamount"]);
                        model.freightchargecomments = Common.ToString(dr["freightchargecomments"]);
                        model.totalothercharges = Common.ToDecimal(dr["totalothercharges"]);
                        if (dr["otherfees"] != DBNull.Value)
                        {
                            string json = dr["otherfees"].ToString();
                            model.otherfees = !string.IsNullOrEmpty(json) ? JsonConvert.DeserializeObject<List<OtherFeesDTO>>(json) : new List<OtherFeesDTO>();
                        }
                        else
                        {
                            model.otherfees = new List<OtherFeesDTO>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return model;
        }

        public Result SaveShipmentConfimation(string bookinguuid, List<BookedContainerDTO> containers)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    var details = containers
                                        .Select(cont => new
                                        {
                                            ContainerUuid = cont.ContainerUuid,
                                            Stamp = cont.Stamp,
                                            Weight = cont.Weight
                                        }).ToList();

                    string detailsJson = System.Text.Json.JsonSerializer.Serialize(details);

                    string query = @"SELECT * FROM booking.booking_shipment_confirmation_save(
                                     p_bookinguuid := '" + bookinguuid + @"',
                                     p_json := '" + Common.Escape(detailsJson) + @"'::jsonb,
                                     p_hubid := '" + Common.HubID + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save shipment confirmation .");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }
        public List<ShipmentConfirmationDTO> GetShipmentConfimation(string bookinguuid)
        {
            List<ShipmentConfirmationDTO> list = new List<ShipmentConfirmationDTO>();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booking_shipment_confirmation_list('" + Common.HubID + "','" + bookinguuid + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new ShipmentConfirmationDTO
                            {
                                ContainerUuid = Common.ToString(dr["containeruuid"]),
                                Stamp = Common.ToString(dr["stamp"]),
                                Weight = Common.ToDecimal(Common.ToDecimal(dr["weight"]).ToString("0.00")),
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result SaveCabotage(string bookinguuid, List<string> ContainerUuids)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB(DatabaseCollection.Contrack))
                {

                    var details = ContainerUuids
                                        .Select(cont => new
                                        {
                                            ContainerUuid = cont,
                                        }).ToList();

                    string detailsJson = System.Text.Json.JsonSerializer.Serialize(details);

                    string query = @"SELECT * FROM booking.booking_cabatoge_save(
                                     p_bookinguuid := '" + bookinguuid + @"',
                                     p_json := '" + Common.Escape(detailsJson) + @"'::jsonb,
                                     p_hubid := '" + Common.HubID + @"',
                                     p_userid := '" + Common.LoginID + @"');";

                    DataTable tbl = Db.GetDataTable(query);
                    if (tbl != null && tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetUUID = Common.ToString(tbl.Rows[0][2]);
                        }
                        else
                        {
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save cabotage");
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }
        public List<ShipmentConfirmationDTO> GetCabotage(string cabotageuuid)
        {
            List<ShipmentConfirmationDTO> list = new List<ShipmentConfirmationDTO>();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booking_cabotage_get('" + Common.HubID + "','" + cabotageuuid + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new ShipmentConfirmationDTO
                            {
                                BookingUUID = Common.ToString(dr["bookinguuid"]),
                                ContainerUuid = Common.ToString(dr["containeruuid"]),
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public List<CabotageDTO> GetCabotageList(string bookinguuid)
        {
            List<CabotageDTO> list = new List<CabotageDTO>();
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booking_cabotage_list('" + Common.HubID + "','" + bookinguuid + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new CabotageDTO
                            {
                                cabotageuuid = Common.ToString(dr["cabotageuuid"]),
                                createdat = FormatConvertor.ToClientDateTimeFormat(Common.ToDateTime(dr["createdat"])),
                                fullname = Common.ToString(dr["fullname"]),
                                containercount = Common.ToInt(dr["containercount"]),
                            }).ToList();
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }
        public Result ValidateContainerQty(string bookinguuid, string bookingdetailuuid, int qty)
        {
            Result result = Common.SuccessMessage("Success");
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.booking_validate_container('" + bookinguuid + "','" + bookingdetailuuid + "','" + qty + "','" + Common.HubID + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot validate");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return result;
        }
        public Result DeleteContainerFromBooking(string bookinguuid, string containeruuid)
        {
            Result result = Common.ErrorMessage("Cannot Delete");
            try
            {
                using (SqlDB db = new SqlDB(DatabaseCollection.Contrack))
                {
                    DataTable tbl = db.GetDataTable("SELECT * FROM booking.container_selection_delete('" + Common.HubID + "','" + bookinguuid + "','" + containeruuid + "','" + Common.LoginID + "');");
                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Delete");
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return result;
        }
    }
}

