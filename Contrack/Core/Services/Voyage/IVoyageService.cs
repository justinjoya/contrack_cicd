using Contrack;
using System.Collections.Generic;

public interface IVoyageService
{
    void SaveVoyage(VoyageExtention voyage);
    void AddIntermediatePort(VoyageExtention voyage);
    void MarkAsArrived(VoyageExtention voyage);
    void MarkAsDepartured(VoyageExtention voyage);
    void DeleteVoyageDetail(int voyageDetailId, int voyageId);
    List<Voyage> GetVoyageList(VoyageFilter voyageFilter);
    VoyageExtention GetVoyageByDetailID(int voyageDetailId);
    VoyageExtention GetVoyageById(int voyageId);
    List<voyageStatusCountDTO> GetVoyageStatusCount(VoyageFilter filter);

    Voyage GetVoyageByUUID(string uuid);
    List<VoyageDTO> SearchVoyage(string search = "", bool createnew = true);
    List<VoyageDTO> GetDirectVoyageSearch(string originportid, string destinationportid, string selectedvoyageuuid, bool ischange);
}

