using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IContainerRepository
    {
        Result SaveContainer(ContainerDTO container);
        ContainerDTO GetContainerByID(string containerid);
        ContainerDTO GetContainerByUUID(string containeruuid);
        List<ContainerDTO> GetContainerList(ContainerFilterPage filter);
        List<ContainerStatusCountDTO> GetContainerStatusCount(ContainerFilterPage filter);
        bool IsContainerAvailable(string containerid);
        List<ContainerAvailableDTO> IsContainerAvailable(List<string> containerids);
    }
}