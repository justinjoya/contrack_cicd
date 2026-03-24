using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IJobTypeService
    {
        void SaveJobType(JobType jobType);
        void SaveJobTypeDetails(JobTypeDetailDTO jobTypeDetail, int parentJobTypeId);

        JobTypeDTO GetJobTypeByID(int JobTypeID);
        JobTypeDetailDTO GetJobTypeDetailByID(int JobTypeDetailID);

        List<JobType> GetJobTypeList(string search);

        Result DeleteJobType(int JobTypeID, string JobTypeUUID);
        Result DeleteJobTypeDetails(int JobTypeDetailID);
    }
}