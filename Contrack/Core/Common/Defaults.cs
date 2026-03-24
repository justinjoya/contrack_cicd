using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public static class Defaults
    {
        public static EncryptedData Agency()
        {
            EncryptedData data = new EncryptedData();
            try
            {
                var entityids = SessionManager.EntityIDs;
                if (entityids == null)
                {
                    ILoginRepository repo = new LoginRepository();
                    IHubRepository hubrepo = new HubRepository();
                    ILoginService loginservice = new LoginService(repo, hubrepo);
                    UserDTO user = loginservice.GetUserById(Common.Encrypt(Common.LoginID));
                    SessionManager.EntityIDs
                            = entityids
                            = user.EntityIDEncryptedList;
                }
                if (entityids?.Count == 1)
                {
                    data = new EncryptedData
                    {
                        EncryptedValue = entityids[0],
                        NumericValue = Common.Decrypt(entityids[0])
                    };
                }
            }
            catch (Exception ex)
            { }
            return data;
        }

        public static List<System.Web.UI.WebControls.ListItem> SetAgencyDefault(dynamic agency)
        {
            var agencydropdowns = Dropdowns.GetAgenciesDropdown();
            var actualagencyitems = agencydropdowns.Where(x => x.Value != "").ToList();
            if (actualagencyitems.Count() == 1)
            {
                agency.AgencyDetailID = new EncryptedData()
                {
                    EncryptedValue = actualagencyitems[0].Value,
                    NumericValue = Common.Decrypt(actualagencyitems[0].Value)
                };
                agency.AgencyName = actualagencyitems[0].Text;
            }

            return agencydropdowns;
        }
    }
}