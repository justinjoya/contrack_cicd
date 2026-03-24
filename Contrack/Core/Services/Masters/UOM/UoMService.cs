using System;
using System.Collections.Generic;

namespace Contrack
{
    public class UoMService : CustomException, IUoMService
    {
        private readonly IUoMRepository _repo;
        public Result result = new Result();

        public UoMService(IUoMRepository repo)
        {
            _repo = repo;
        }

        public List<UoMDTO> GetUoMList(string search = "")
        {
            List<UoMDTO> list = new List<UoMDTO>();
            try
            {
                list = _repo.GetUoMList(search);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("Error while fetching UOM list: " + ex.Message);
            }
            return list;
        }

        public void SaveUoM(UoMDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    result = Common.ErrorMessage("Invalid UOM data.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(dto.uomname))
                {
                    result = Common.ErrorMessage("Unit of Measure cannot be empty.");
                    return;
                }

                result = _repo.SaveUoM(dto);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("Error while saving UOM: " + ex.Message);
            }
        }

        public void DeleteUoM(int uomid)
        {
            try
            {
                if (uomid <= 0)
                {
                    result = Common.ErrorMessage("Invalid UOM ID.");
                    return;
                }

                result = _repo.DeleteUoM(uomid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage("Error while deleting UOM: " + ex.Message);
            }
        }
    }
}
