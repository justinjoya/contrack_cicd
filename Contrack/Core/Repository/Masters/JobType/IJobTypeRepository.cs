using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IJobTypeRepository
    {
        Result SaveJobType(JobTypeDTO jobType);
        Result DeleteJobType(int JobTypeID, string JobTypeUUID);
        JobTypeDTO GetJobTypeByID(int JobTypeID);
        List<JobTypeDTO> GetJobTypeList();
        Result SaveJobTypeDetail(JobTypeDetailDTO jobTypeDetail, int jobTypeId);
        Result DeleteJobTypeDetail(int JobTypeDetailID);
        JobTypeDetailDTO GetJobTypeDetailByID(int JobTypeDetailID);
    }
}