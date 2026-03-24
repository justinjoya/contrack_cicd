using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IAgencyService
    {
        void SaveAgency(Agency agency);
        void SaveCustomAttribute(List<KeyValuePair> Attr, int AgencyID, string AgencyUUID);
        void SaveBankInfo(List<CustomBankInfo> Banks, int AgencyID, string AgencyUUID);
        AgencyDTO GetAgencyByID(int AgencyID);
        AgencyDTO GetAgencyByUUID(string AgencyUUID);
        AgencyDTO GetAgencyByDetailID(int AgencyDetailID);
        List<Agency> GetAgencyModificationList(string AgencyUUID);
        List<Agency> GetAgencyList(string search);
        void DeleteAgency(int AgencyID, string AgencyUUID);
    }
}
