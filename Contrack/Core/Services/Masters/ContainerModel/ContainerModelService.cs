using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Contrack
{
    public class ContainerModelService : CustomException, IContainerModelService
    {
        public Result Result = new Result();
        private readonly IContainerModelRepository _repo;
        public ContainerModelService(IContainerModelRepository repo)
        {
            _repo = repo;
        }

        public void SaveContainerModel(ContainerModelDTO model)
        {
            try
            {
                Result = _repo.SaveContainerModel(model);
            }
            catch (Exception ex)
            {
                Result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void DeleteContainerModel(int containermodelid)
        {
            try
            {
                Result = _repo.DeleteContainerModel(containermodelid);
            }
            catch (Exception ex)
            {
                Result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public List<ContainerTypeDTO> GetContainerModels(ContainerModelFilter filter)
        {
            try
            {
                filter.filters.containertypeid = Common.Decrypt(filter.filters.containertypeenc);
                filter.filters.sizeid = Common.Decrypt(filter.filters.sizeenc);
                return _repo.GetContainerModels(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<ContainerTypeDTO>();
            }
        }

        public ContainerModelDTO GetContainerModelByID(int modelid)
        {
            try
            {
                return _repo.GetContainerModelByID(modelid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new ContainerModelDTO();
            }
        }

        public ContainerModelExtendedDTO GetContainerModelByUUID(string uuid)
        {
            try
            {
                return _repo.GetContainerModelByUUID(uuid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new ContainerModelExtendedDTO();
            }
        }

        public List<ContainerModelExtended> GetContainerModelList(ContainerModelFilter filter)
        {
            try
            {
                filter.filters.containertypeid = Common.Decrypt(filter.filters.containertypeenc);
                filter.filters.sizeid = Common.Decrypt(filter.filters.sizeenc);
                List<ContainerModelExtendedDTO> dtoList = _repo.GetContainerModelList(filter);
                return dtoList.Select(dto => new ContainerModelExtended()
                {
                    model = dto,
                    menu = new MasterMenus()
                    {
                        edit = true
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<ContainerModelExtended>();
            }
        }

        public List<ContainerModelExtended> GetContainerModelsByTypeSize(string typeid, string sizeid)
        {
            try
            {
                List<ContainerModelExtendedDTO> dtoList = _repo.GetContainerModelsByTypeSize(typeid, sizeid);
                return dtoList.Select(dto => new ContainerModelExtended()
                {
                    model = dto,
                    menu = new MasterMenus()
                    {
                        edit = true
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<ContainerModelExtended>();
            }
        }
    }
}
