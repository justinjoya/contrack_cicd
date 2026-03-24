using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IContainerSizeRepository
    {
        List<ContainerSizeDTO> GetContainerSizes();
    }
}
