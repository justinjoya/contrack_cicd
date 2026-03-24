using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IContainerModelService
    {
        void SaveContainerModel(ContainerModelDTO model);
        void DeleteContainerModel(int containermodelid);
        List<ContainerTypeDTO> GetContainerModels(ContainerModelFilter filter);
        ContainerModelDTO GetContainerModelByID(int modelid);
        ContainerModelExtendedDTO GetContainerModelByUUID(string uuid);
        List<ContainerModelExtended> GetContainerModelList(ContainerModelFilter filter);
        List<ContainerModelExtended> GetContainerModelsByTypeSize(string typeid, string sizeid);
    }
}
