using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IAgencyRepository
    {
        Result SaveAgency(AgencyDTO agency);
        Result SaveCustomAttribute(List<KeyValuePair> Attr, int AgencyID, string AgencyUUID);
        Result SaveBankInfo(List<CustomBankInfo> Banks, int AgencyID, string AgencyUUID);
        AgencyDTO GetAgencyByID(int AgencyID);
        AgencyDTO GetAgencyByUUID(string AgencyUUID);
        AgencyDTO GetAgencyByDetailID(int AgencyDetailID);
        List<AgencyDTO> GetAgencyModificationList(string AgencyUUID);
        List<AgencyDTO> GetAgencyList();
        Result DeleteAgency(int AgencyID, string AgencyUUID);
    }
}
