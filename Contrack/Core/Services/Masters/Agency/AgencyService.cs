using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Contrack
{
    public class AgencyService : CustomException, IAgencyService
    {
        public Result result = new Result();
        private readonly IAgencyRepository _repo;
        public AgencyService(IAgencyRepository repo)
        {
            _repo = repo;
        }
        public void SaveAgency(Agency agency)
        {
            try
            {
                if (!string.IsNullOrEmpty(agency.emailtemp))
                {
                    if (!string.IsNullOrEmpty(agency.agency.email))
                    {
                        agency.agency.email = agency.agency.email + ";" + agency.emailtemp;
                    }
                    else
                    {
                        agency.agency.email = agency.emailtemp;
                    }
                }
                if (!string.IsNullOrEmpty(agency.accountsemailtemp))
                {
                    if (!string.IsNullOrEmpty(agency.agency.accountsemail))
                    {
                        agency.agency.accountsemail = agency.agency.accountsemail + ";" + agency.accountsemailtemp;
                    }
                    else
                    {
                        agency.agency.accountsemail = agency.accountsemailtemp;
                    }
                }
                result = _repo.SaveAgency(agency.agency);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }

        }

        public void SaveCustomAttribute(List<KeyValuePair> attr, int agencyid, string uuid)
        {
            result = _repo.SaveCustomAttribute(attr, agencyid, uuid);
        }
        public void SaveBankInfo(List<CustomBankInfo> BankInfo, int agencyid, string uuid)
        {
            result = _repo.SaveBankInfo(BankInfo, agencyid, uuid);
        }
        public void DeleteAgency(int agencyid, string uuid)
        {
            result = _repo.DeleteAgency(agencyid, uuid);
        }
        public AgencyDTO GetAgencyByID(int ID)
        {
            return _repo.GetAgencyByID(ID);
        }
        public AgencyDTO GetAgencyByUUID(string UUID)
        {
            return _repo.GetAgencyByUUID(UUID);

        }
        public AgencyDTO GetAgencyByDetailID(int DetailID)
        {
            return _repo.GetAgencyByDetailID(DetailID);
        }
        public List<Agency> GetAgencyList(string search)
        {
            List<AgencyDTO> list = _repo.GetAgencyList();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                list = list.Where(x => x.agencyname.ToLower().Contains(search)
                || x.imono.ToLower().Contains(search)
                || x.email.ToLower().Contains(search)
                || x.phone.ToLower().Contains(search)
                ).ToList();
            }
            return list.Select(x => new Agency()
            {
                agency = x,
                menu = new MasterMenus()
                {
                    edit = true//PagePermission.GetPermissions().Agency.edit
                }
            }).ToList();
        }

        public List<Agency> GetAgencyModificationList(string UUID)
        {
            List<AgencyDTO> list = _repo.GetAgencyModificationList(UUID);

            return list.Select(x => new Agency()
            {
                agency = x,
                menu = new MasterMenus()
                {
                    edit = true // or PagePermission.GetPermissions().Agency.edit if permission-based
                }
            }).ToList();
        }

    }
}