using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IContainerService
    {
        void SaveContainer(ContainerModal model);
        ContainerModal GetContainerByID(string containerid);
        ContainerModal GetContainerByUUID(string containeruuid);
        List<ContainerDTO> GetContainerList(ContainerFilterPage filter);
        List<ContainerStatusCountDTO> GetContainerStatusCount(ContainerFilterPage filter);
        bool IsContainerAvailable(string containerid);
        List<ContainerAvailableDTO> IsContainerAvailable(List<string> containerids);
    }
}