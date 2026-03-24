using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IVoyageRepository
    {
        Result SaveVoyage(VoyageExtendedDTO voyage);
        Result AddIntermediatePortToVoyage(VoyageExtendedDTO voyage);
        Result MarkAsArrived(VoyageExtendedDTO voyage);
        Result MarkAsDepartured(VoyageExtendedDTO voyage);
        Result DeleteVoyageDetail(int voyageDetailId, int voyageId);
        List<VoyageDTO> GetVoyageList(VoyageFilter voyageFilter);
        VoyageExtendedDTO GetVoyageByDetailID(int voyageDetailId);
        VoyageExtendedDTO GetVoyageById(int voyageId);
        VoyageDTO GetVoyageByUUID(string uuid);
        List<VoyageDTO> SearchVoyage(string search = "", bool createnew = true);
        List<VoyageDTO> GetDirectVoyageSearch(string originPortId, string destinationPortId);
        List<voyageStatusCountDTO> GetVoyageStatusCount(VoyageFilter filter);

    }
}
