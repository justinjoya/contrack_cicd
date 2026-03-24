using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class JobTypeService : CustomException, IJobTypeService
    {
        public Result result = new Result();
        private readonly IJobTypeRepository _repo;

        public JobTypeService(IJobTypeRepository repo)
        {
            _repo = repo;
        }


        public void SaveJobType(JobType model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.jobtype.jobtypename))
                {
                    model.result = Common.ErrorMessage("Job Name cannot be empty.");
                    return;
                }
                model.result = _repo.SaveJobType(model.jobtype);
            }
            catch (Exception ex)
            {
                model.result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }

        public Result DeleteJobType(int JobTypeID, string JobTypeUUID)
        {
            result = _repo.DeleteJobType(JobTypeID, JobTypeUUID);
            return result;
        }

        public JobTypeDTO GetJobTypeByID(int JobTypeID)
        {
            return _repo.GetJobTypeByID(JobTypeID);
        }

        public List<JobType> GetJobTypeList(string search)
        {
            var list = new List<JobType>();
            try
            {
                List<JobTypeDTO> dtoList = _repo.GetJobTypeList();

                if (!string.IsNullOrEmpty(search))
                {
                    search = search.ToLower();
                    dtoList = dtoList.Where(x => x.jobtypename.ToLower().Contains(search)
                                              || x.jobshortcode.ToLower().Contains(search)).ToList();
                }

                list = dtoList.Select(dto => new JobType()
                {
                    jobtype = dto,
                    menu = new MasterMenus()
                    {
                        edit = true
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list;
        }



        public void SaveJobTypeDetails(JobTypeDetailDTO jobTypeDetail, int parentJobTypeId)
        {
            try
            {
                if (jobTypeDetail.jobtypedetailid.NumericValue <= 0)
                    jobTypeDetail.jobtypedetailid.NumericValue = Common.Decrypt(jobTypeDetail.jobtypedetailid.EncryptedValue);

                _repo.SaveJobTypeDetail(jobTypeDetail, parentJobTypeId);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                throw;
            }
        }

        public Result DeleteJobTypeDetails(int JobTypeDetailID)
        {
            result = _repo.DeleteJobTypeDetail(JobTypeDetailID);
            return result;
        }

        public JobTypeDetailDTO GetJobTypeDetailByID(int JobTypeDetailID)
        {
            return _repo.GetJobTypeDetailByID(JobTypeDetailID);
        }

    }
}