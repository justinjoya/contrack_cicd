using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IContainerModelRepository
    {
        Result SaveContainerModel(ContainerModelDTO model);
        Result DeleteContainerModel(int containermodelid);
        List<ContainerTypeDTO> GetContainerModels(ContainerModelFilter filter); // -- Will pass filter in future
        ContainerModelDTO GetContainerModelByID(int modelid);
        ContainerModelExtendedDTO GetContainerModelByUUID(string uuid);
        List<ContainerModelExtendedDTO> GetContainerModelList(ContainerModelFilter filter);
        List<ContainerModelExtendedDTO> GetContainerModelsByTypeSize(string typeid, string sizeid);
    }
}
