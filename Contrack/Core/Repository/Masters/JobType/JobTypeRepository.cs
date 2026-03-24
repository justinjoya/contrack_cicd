using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Contrack
{
    public class JobTypeRepository : CustomException, IJobTypeRepository
    {

        public Result SaveJobType(JobTypeDTO jobType)
        {
            var result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.save_jobtype(" +
                        "p_hubid := " + Common.HubID + "," +
                        "p_jobtypename := '" + Common.Escape(jobType.jobtypename) + "'," +
                        "p_jobtypeid := " + jobType.jobtypeid.NumericValue + "," +
                        "p_jobshortcode := '" + Common.Escape(jobType.jobshortcode) + "'," +
                        "p_usemaster := " + jobType.useasmaster + " " +
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
                        result = Common.ErrorMessage("Cannot Save Job Type.");
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

        public Result DeleteJobType(int JobTypeID, string JobTypeUUID)
        {
            Result result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.soft_delete_jobtype(" +
                       "p_jobtypeid := " + JobTypeID + ", " +
                       "p_jobtypeuuid := " + Common.GetUUID(JobTypeUUID) + ", " +
                       "p_hubid := " + Common.HubID + ", " +
                       "p_deletedby := " + Common.LoginID + ");");

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else
                        result = Common.ErrorMessage("Cannot Delete Job Type");
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public JobTypeDTO GetJobTypeByID(int JobTypeID)
        {
            var jobType = new JobTypeDTO();
            try
            {
                if (JobTypeID != 0)
                {
                    jobType = ParseJobType("SELECT * FROM procurement.get_jobtype(" + JobTypeID + ", " + Common.HubID + ");");
                    if (jobType != null && jobType.jobtypeid.NumericValue > 0)
                    {
                        using (SqlDB Db = new SqlDB())
                        {
                            DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.get_jobtypedetail_list(" + JobTypeID + ", " + Common.HubID + ", false);");
                            jobType.JobTypeDetails = (from DataRow dr in tbl.Rows
                                                      select new JobTypeDetailDTO()
                                                      {
                                                          jobtypedetailid = new EncryptedData() { NumericValue = Common.ToInt(dr["jobtypedetailid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["jobtypedetailid"])) },
                                                          jobtypedetailuuid = Common.ToString(dr["jobtypedetailuuid"]),
                                                          jobtypename = Common.ToString(dr["jobtypename"]),
                                                          description = Common.ToString(dr["description"]),
                                                          isdeleted = Common.ToBool(dr["isdeleted"]),
                                                          totalcount = Common.ToLong(dr["total_count"]),
                                                      }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return jobType;
        }

        public List<JobTypeDTO> GetJobTypeList()
        {
            var list = new List<JobTypeDTO>();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.get_jobtype_list_with_detail_count('" + Common.HubID + "','" + Common.MyAppID + "');");
                    list = (from DataRow dr in tbl.Rows
                            select new JobTypeDTO()
                            {
                                jobtypeid = new EncryptedData() { NumericValue = Common.ToInt(dr["jobtypeid"]), EncryptedValue = Common.Encrypt(Common.ToInt(dr["jobtypeid"])) },
                                jobtypename = Common.ToString(dr["jobtypename"]),
                                jobshortcode = Common.ToString(dr["jobshortcode"]),
                                useasmaster = Common.ToBool(dr["usemaster"]),
                                lineitemcount = Common.ToInt(dr["lineitemcount"]),
                            }).ToList();
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return list;
        }

        private JobTypeDTO ParseJobType(string qry)
        {
            var jobType = new JobTypeDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable(qry);
                    if (tbl.Rows.Count != 0)
                    {
                        jobType.jobtypeid = new EncryptedData() { NumericValue = Common.ToInt(tbl.Rows[0]["jobtypeid"]), EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["jobtypeid"])) };
                        jobType.jobtypename = Common.ToString(tbl.Rows[0]["jobtypename"]);
                        jobType.jobshortcode = Common.ToString(tbl.Rows[0]["jobshortcode"]);
                        jobType.useasmaster = Common.ToBool(tbl.Rows[0]["usemaster"]);
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return jobType;
        }



        public Result SaveJobTypeDetail(JobTypeDetailDTO jobTypeDetail, int jobTypeId)
        {
            var result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.save_jobtypedetail(" +
                        "p_jobtypedetailid := " + jobTypeDetail.jobtypedetailid.NumericValue + "," +
                        "p_jobtypeid := " + jobTypeId + "," +
                        "p_hubid := " + Common.HubID + "," +
                        "p_description := '" + Common.Escape(jobTypeDetail.description) + "'," +
                        "p_createdby := " + Common.LoginID + "" +
                        ");");

                    if (tbl.Rows.Count > 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else { result = Common.ErrorMessage("Cannot Save Job Type Details"); }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public Result DeleteJobTypeDetail(int JobTypeDetailID)
        {
            var result = new Result();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable("SELECT * FROM procurement.soft_delete_jobtypedetail(" +
                    JobTypeDetailID + ", " + Common.HubID + ", " + Common.LoginID + ")");

                    if (tbl.Rows.Count != 0)
                    {
                        if (tbl.Rows[0][0].ToString() == "1")
                            result = Common.SuccessMessage(tbl.Rows[0][1].ToString());
                        else
                            result = Common.ErrorMessage(tbl.Rows[0][1].ToString());
                    }
                    else { result = Common.ErrorMessage("Cannot Delete Job Type Details"); }
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
            return result;
        }

        public JobTypeDetailDTO GetJobTypeDetailByID(int JobTypeDetailID)
        {
            var detail = new JobTypeDetailDTO();
            try
            {
                if (JobTypeDetailID != 0)
                {
                    detail = ParseJobTypeDetailsById("SELECT * FROM procurement.get_jobtypedetail(" + JobTypeDetailID + ", " + Common.HubID + ");");
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return detail;
        }

        private JobTypeDetailDTO ParseJobTypeDetailsById(string qry)
        {
            var detail = new JobTypeDetailDTO();
            try
            {
                using (SqlDB Db = new SqlDB())
                {
                    DataTable tbl = Db.GetDataTable(qry);
                    if (tbl.Rows.Count != 0)
                    {
                        detail.jobtypedetailid = new EncryptedData() { NumericValue = Common.ToInt(tbl.Rows[0]["jobtypedetailid"]), EncryptedValue = Common.Encrypt(Common.ToInt(tbl.Rows[0]["jobtypedetailid"])) };
                        detail.jobtypedetailuuid = Common.ToString(tbl.Rows[0]["jobtypedetailuuid"]);
                        detail.description = Common.ToString(tbl.Rows[0]["description"]);
                        detail.isdeleted = Common.ToBool(tbl.Rows[0]["isdeleted"]);
                    }
                }
            }
            catch (Exception ex) { RecordException(ex); }
            return detail;
        }

    }
}