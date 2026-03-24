using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using Newtonsoft.Json;

namespace Contrack
{
    public class VesselRepository : CustomException, IVesselRepository
    {
        public Result SaveVessel(VesselDTO vessel)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.savevessel(" +
                        "p_imono := '" + Common.Escape(vessel.imono) + "'," +
                        "p_mmsino := '" + Common.Escape(vessel.mmsino) + "'," +
                        "p_vesselassignmentid := '" + Common.Decrypt(vessel.vassignment.vesselassignmentid.EncryptedValue) + "'," +
                        "p_vesselname := '" + Common.Escape(vessel.vesselname) + "'," +
                        "p_vesseltypeid := '" + Common.Decrypt(vessel.vesseltypeid) + "'," +
                        "p_subtypeid := '" + Common.Decrypt(vessel.subtypeid) + "'," +
                        "p_flagcountryid := '" + Common.Decrypt(vessel.flagcountryid) + "'," +
                        "p_portofregistryid := '" + Common.Decrypt(vessel.portofregistryid) + "'," +
                        "p_userid := '" + Common.LoginID + "'," +
                        "p_hubid := '" + Common.HubID + "'," +
                        "p_agencyid := '" + Common.Decrypt(vessel.vassignment.agencyid.EncryptedValue) + "'," +
                        "p_vesseluuid := " + Common.GetUUID(vessel.vesseluuid) + "," +
                        "p_assignmentuuid := " + Common.GetUUID(vessel.vassignment.assignmentuuid) + "," +
                        "p_vesselid := '" + Common.Decrypt(vessel.vesselid.EncryptedValue) + "'," +
                        "p_grosstonnage := '" + vessel.grosstonnage + "'," +
                        "p_nettonnage := '" + vessel.nettonnage + "'," +
                        "p_deadweight := '" + vessel.deadweight + "'," +
                        "p_lengthoverall := '" + vessel.lengthoverall + "'," +
                        "p_lengthbp := '" + vessel.lengthbp + "'," +
                        "p_breadth := '" + vessel.breadth + "'," +
                        "p_depth := '" + vessel.depth + "'," +
                        "p_summerdraft := '" + vessel.summerdraft + "'," +
                        "p_enginepowerkw := '" + vessel.enginepowerkw + "'," +
                        "p_totalauxenginepowerkw := '" + vessel.totalauxenginepowerkw + "'," +
                        "p_bowthrusters := '" + vessel.bowthrusters + "'," +
                        "p_sternthrusters := '" + vessel.sternthrusters + "'," +
                        "p_scrubberinstalled := '" + vessel.scrubberinstalled + "'," +
                        "p_soxcompliant := '" + vessel.soxcompliant + "'," +
                        "p_specialinstructions := '" + Common.Escape(vessel.specialinstructions) + "'" +
                        ");";

                    DataTable tbl = Db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                        {
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                            result.TargetID = Convert.ToInt32(tbl.Rows[0][2]);
                        }
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot save Vessel");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }


        public Result DeleteVessel(VesselAssignmentDTO assignment)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.deletevessel(" +
                                   "'" + Common.Decrypt(assignment.vesselassignmentid.EncryptedValue) + "'," +
                                   "" + Common.GetUUID(assignment.assignmentuuid) + "," +
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
                        result = Common.ErrorMessage("Cannot delete Vessel");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result MoveVesselToHub(VesselAssignmentDTO vassignment)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.movevesseltohub(" +
                                   "'" + Common.Decrypt(vassignment.vesselassignmentid.EncryptedValue) + "'," +
                                   "" + Common.GetUUID(vassignment.assignmentuuid) + "," +
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
                        result = Common.ErrorMessage("Cannot move Vessel");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public VesselDTO GetVesselByUUID(string uuid)
        {
            VesselDTO vessel = new VesselDTO();
            try
            {
                if (!string.IsNullOrEmpty(uuid))
                {
                    using (SqlDB Db = new SqlDB())
                    {
                        DataTable tbl = Db.GetDataTable("SELECT * FROM masters.getvesselbyuuid('" + uuid + "','" + Common.HubID + "');");

                        if (tbl.Rows.Count > 0)
                        {
                            DataRow dr = tbl.Rows[0];

                            vessel.vassignment = new VesselAssignmentDTO()
                            {
                                vesselassignmentid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["vesselassignmentid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselassignmentid"]))
                                },
                                assignmentuuid = Common.ToString(dr["assignmentuuid"]),
                                agencyid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["agencyid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                },
                                agencyuuid = Common.ToString(dr["agencyuuid"]),
                                agencyname = Common.ToString(dr["agencyname"]),
                            };
                            vessel.vesselid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["vesselid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselid"]))
                            };
                            vessel.mmsino = Common.ToString(dr["mmsino"]);
                            vessel.vesseluuid = Common.ToString(dr["vesseluuid"]);
                            vessel.vesselname = Common.ToString(dr["vesselname"]);
                            vessel.imono = Common.ToString(dr["imono"]);
                            vessel.vesseltypeid = Common.Encrypt(Common.ToInt(dr["vesseltypeid"]));
                            vessel.subtypeid = Common.Encrypt(Common.ToInt(dr["subtypeid"]));
                            vessel.flagcountryid = Common.Encrypt(Common.ToInt(dr["flagcountryid"]));
                            vessel.portofregistryid = Common.Encrypt(Common.ToInt(dr["portofregistryid"]));
                            vessel.grosstonnage = Common.ToInt(dr["grosstonnage"]);
                            vessel.nettonnage = Common.ToInt(dr["nettonnage"]);
                            vessel.deadweight = Common.ToInt(dr["deadweight"]);
                            vessel.lengthoverall = Common.ToInt(dr["lengthoverall"]);
                            vessel.lengthbp = Common.ToInt(dr["lengthbp"]);
                            vessel.breadth = Common.ToInt(dr["breadth"]);
                            vessel.depth = Common.ToInt(dr["depth"]);
                            vessel.summerdraft = Common.ToInt(dr["summerdraft"]);
                            vessel.enginepowerkw = Common.ToInt(dr["enginepowerkw"]);
                            vessel.totalauxenginepowerkw = Common.ToInt(dr["totalauxenginepowerkw"]);
                            vessel.bowthrusters = Common.ToBool(dr["bowthrusters"]);
                            vessel.sternthrusters = Common.ToBool(dr["sternthrusters"]);
                            vessel.scrubberinstalled = Common.ToBool(dr["scrubberinstalled"]);
                            vessel.soxcompliant = Common.ToBool(dr["soxcompliant"]);
                            vessel.specialinstructions = Common.ToString(dr["specialinstructions"]);
                            vessel.flagcountry = Common.ToString(dr["flagcountry"]);
                            vessel.portofregistry = Common.ToString(dr["portofregistry"]);
                            vessel.vesseltype = Common.ToString(dr["ShipType"]);
                            vessel.vesselsubtype = Common.ToString(dr["ShipSubType"]);

                            if (vessel.vassignment.agencyid.NumericValue <= 0)
                                vessel.vassignment.agencyname = Common.HubName;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return vessel;
        }
        public List<VesselDTO> ValidateVessel(string vesselname, string imono, string mmsino)
        {
            List<VesselDTO> list = new List<VesselDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM masters.validatevessel('" +
                        Common.Escape(vesselname) + "','" +
                        Common.Escape(imono) + "','" +
                        Common.Escape(mmsino) + "','" +
                        Common.HubID + "');");

                    list = (from DataRow dr in tbl.Rows
                            select new VesselDTO()
                            {
                                vesselid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["vesselid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselid"]))
                                },
                                mmsino = Common.ToString(dr["mmsino"]),
                                vesseluuid = Common.ToString(dr["vesseluuid"]),
                                vesselname = Common.ToString(dr["vesselname"]),
                                imono = Common.ToString(dr["imono"]),
                                vesseltypeid = Common.Encrypt(Common.ToInt(dr["vesseltypeid"])),
                                subtypeid = Common.Encrypt(Common.ToInt(dr["subtypeid"])),
                                flagcountryid = Common.Encrypt(Common.ToInt(dr["flagcountryid"])),
                                portofregistryid = Common.Encrypt(Common.ToInt(dr["portofregistryid"])),
                                grosstonnage = Common.ToInt(dr["grosstonnage"]),
                                nettonnage = Common.ToInt(dr["nettonnage"]),
                                deadweight = Common.ToInt(dr["deadweight"]),
                                lengthoverall = Common.ToInt(dr["lengthoverall"]),
                                lengthbp = Common.ToInt(dr["lengthbp"]),
                                breadth = Common.ToInt(dr["breadth"]),
                                depth = Common.ToInt(dr["depth"]),
                                summerdraft = Common.ToInt(dr["summerdraft"]),
                                enginepowerkw = Common.ToInt(dr["enginepowerkw"]),
                                totalauxenginepowerkw = Common.ToInt(dr["totalauxenginepowerkw"]),
                                bowthrusters = Common.ToBool(dr["bowthrusters"]),
                                sternthrusters = Common.ToBool(dr["sternthrusters"]),
                                scrubberinstalled = Common.ToBool(dr["scrubberinstalled"]),
                                soxcompliant = Common.ToBool(dr["soxcompliant"]),
                                specialinstructions = Common.ToString(dr["specialinstructions"]),
                                vassignment = new VesselAssignmentDTO()
                                {
                                    vesselassignmentid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["vesselassignmentid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselassignmentid"]))
                                    },
                                    assignmentuuid = Common.ToString(dr["assignmentuuid"]),
                                    agencyid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["agencyid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                    },
                                    agencyuuid = Common.ToString(dr["agencyuuid"]),
                                    agencyname = Common.ToString(dr["agencyname"]),
                                }
                            }).ToList();
                }
                list.ForEach(x =>
                {
                    x.vassignment.agencyname = x.vassignment.agencyid.NumericValue > 0 ? x.vassignment.agencyname : Common.HubName;
                });
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }

        public List<VesselDTO> GetVesselList(VesselFilter filter)
        {
            List<VesselDTO> list = new List<VesselDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    string query = "SELECT * FROM masters.getvessellist(" +
                                   "p_hub_id := '" + Common.HubID + "'," +
                                   "p_agency_id := '{" + Common.Decrypt(filter.AgencyID) + "}'," +
                                   "p_search := '" + Common.Escape(filter.Search) + "'," +
                                   "p_limit := '" + filter.limit + "'," +
                                   "p_offset := '" + filter.offset + "'," +
                                   "p_userid := '" + Common.LoginID + "'," +
                                   "p_sort_column := '" + filter.sorting + "'," +
                                   "p_sort_direction := '" + filter.sortingorder + "'" +
                                   ");";

                    DataTable tbl = Db.GetDataTable(query);

                    list = (from DataRow dr in tbl.Rows
                            select new VesselDTO()
                            {
                                row_index = Common.ToInt(dr["row_index"]),
                                vesselid = new EncryptedData()
                                {
                                    NumericValue = Common.ToInt(dr["vesselid"]),
                                    EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselid"]))
                                },
                                vesselname = Common.ToString(dr["vesselname"]),
                                imono = Common.ToString(dr["imono"]),
                                mmsino = Common.ToString(dr["mmsino"]),
                                vesseltype = Common.ToString(dr["vesseltype"]),
                                vesselsubtype = Common.ToString(dr["vesselsubtype"]),
                                vesselpicname = Common.ToString(dr["vesselpicname"]),
                                vesselpicposition = Common.ToString(dr["vesselpicposition"]),
                                extras = new VesselExtra()
                                {
                                    shortcode = Common.GetShortcode(Common.ToString(dr["vesselname"])),
                                },
                                vassignment = new VesselAssignmentDTO()
                                {
                                    vesselassignmentid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["vesselassignmentid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselassignmentid"]))
                                    },
                                    assignmentuuid = Common.ToString(dr["vesseluuid"]),
                                    agencyid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["agencyid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["agencyid"]))
                                    },
                                    agencyname = Common.ToString(dr["agencyname"]),
                                    agencyuuid = Common.ToString(dr["agencyuuid"]),
                                },
                                flagcountry = Common.ToString(dr["flagcountry"]),
                                portofregistry = Common.ToString(dr["portofregistry"]),
                                totalnoofrows = Common.ToInt(dr["total_count"]),
                                createdat = Common.ToDateTime(dr["createdat"])
                            }).ToList();

                    try
                    {
                        list.ForEach(x =>
                        {
                            x.createdat = x.createdat.AddMinutes(Convert.ToDouble(SessionManager.TimeOffSet));
                            x.vassignment.agencyname = x.vassignment.agencyid.NumericValue > 0
                                ? x.vassignment.agencyname
                                : Common.HubName;
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
        public List<VesselContactDTO> GetContacts(string vesselAssignmentId)
        {
            List<VesselContactDTO> contacts = new List<VesselContactDTO>();
            try
            {
                using (var db = new SqlDB())
                {
                    string query = $"SELECT * FROM masters.getvesselpiclist('{Common.Decrypt(vesselAssignmentId)}','{Common.HubID}');";
                    DataTable tbl = db.GetDataTable(query);

                    contacts = (from DataRow dr in tbl.Rows
                                select new VesselContactDTO
                                {
                                    picid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["picid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["picid"]))
                                    },
                                    vesselassignmentid = new EncryptedData()
                                    {
                                        NumericValue = Common.ToInt(dr["vesselassignmentid"]),
                                        EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselassignmentid"]))
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
                contacts = new List<VesselContactDTO>();
            }

            return contacts;
        }


        public VesselContactDTO GetContactById(string picId)
        {
            VesselContactDTO contact = null;
            try
            {
                using (var db = new SqlDB())
                {
                    string query = $"SELECT * FROM masters.getvesselpicbyid('{Common.HubID}','{Common.Decrypt(picId)}');";
                    DataTable tbl = db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        DataRow dr = tbl.Rows[0];
                        contact = new VesselContactDTO
                        {
                            picid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["picid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["picid"]))
                            },
                            vesselassignmentid = new EncryptedData()
                            {
                                NumericValue = Common.ToInt(dr["vesselassignmentid"]),
                                EncryptedValue = Common.Encrypt(Common.ToInt(dr["vesselassignmentid"]))
                            },
                            fullname = Common.ToString(dr["fullname"]),
                            email = Common.ToString(dr["email"]),
                            phone = Common.ToString(dr["phone"]),
                            position = Common.ToString(dr["position"]),
                            isprimary = Common.ToBool(dr["isprimary"])
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
                contact = null;
            }

            return contact;
        }


        public Result SaveContact(VesselContactDTO contact)
        {
            Result result = new Result();
            try
            {
                using (var db = new SqlDB())
                {
                    string query = $@"
                                         SELECT * FROM masters.savevesselpic(
                                        p_vesselassignmentid := {Common.Decrypt(contact.vesselassignmentid.EncryptedValue)},
                                        p_fullname := '{Common.Escape(contact.fullname)}',
                                        p_email := '{Common.Escape(contact.email)}',
                                        p_phone := '{Common.Escape(contact.phone)}',
                                        p_position := '{Common.Escape(contact.position)}',
                                        p_isprimary := '{contact.isprimary}',
                                        p_userid := {Common.LoginID},
                                        p_picid := {Common.Decrypt(contact.picid.EncryptedValue)},
                                        p_hubid := {Common.HubID}
                                    );";

                    DataTable tbl = db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        string status = tbl.Rows[0][0].ToString();
                        string message = tbl.Rows[0][1].ToString();
                        if (status == "1")
                        {
                            result = Common.SuccessMessage(message);
                        }
                        else
                        {
                            result = Common.ErrorMessage(message);
                        }
                    }
                    else
                    {
                        result = Common.ErrorMessage("Cannot save Vessel Contact");
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

        public Result MakePrimary(string picIdInc)
        {
            Result result = new Result();
            try
            {
                using (var db = new SqlDB())
                {
                    string query = $"SELECT * FROM masters.updatevesselprimarypic(p_hub_id := '{Common.HubID}', p_picid := '{Common.Decrypt(picIdInc)}');";
                    DataTable tbl = db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot set contact as primary");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }

            return result;
        }


        public Result DeleteContact(string picIdInc)
        {
            Result result = new Result();
            try
            {
                using (var db = new SqlDB())
                {
                    string query = $"SELECT * FROM masters.vesselpicsoftdelete(p_hub_id := '{Common.HubID}', p_deleted_by := '{Common.LoginID}', p_picid := '{Common.Decrypt(picIdInc)}');";
                    DataTable tbl = db.GetDataTable(query);

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot delete Vessel Contact");
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
