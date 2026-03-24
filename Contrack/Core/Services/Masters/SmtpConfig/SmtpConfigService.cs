using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class SmtpConfigService : CustomException, ISmtpConfigService
    {
        public Result result = new Result();
        private readonly ISmtpConfigRepository _repo;
        public SmtpConfigService(ISmtpConfigRepository repo)
        {
            _repo = repo;
        }
        public List<SmtpConfig> GetSmtpConfigList(string search)
        {
            List<SmtpConfigDTO> list = _repo.GetSmtpConfigList();
            if (!string.IsNullOrEmpty(search))
            {

                search = search.ToLower();
                list = list.Where(x => x.smtp_host.ToLower().Contains(search)
                || x.smtp_username.ToLower().Contains(search)
                || x.agency.agencyname.ToLower().Contains(search)).ToList();
            }
            return list.Select(x => new SmtpConfig()
            {
                smtpconfig = x,
                menus = new MasterMenus()
                {
                    edit = true
                }
            }).ToList();
        }

        public SmtpConfigDTO GetSmtpConfigById(string refid)
        {
            return _repo.GetSmtpConfigById(refid);
        }

        public void SaveSmtpConfig(SmtpConfig smtpconfig)
        {
            try
            {
                if (Common.Decrypt(smtpconfig.smtpconfig.TypeEncrypted) == 1)
                    smtpconfig.smtpconfig.agency.agencyid.EncryptedValue = Common.Encrypt(0);

                result = _repo.SaveSmtpConfig(smtpconfig.smtpconfig);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void DeleteSmtpConfig(SmtpConfig smtpconfig)
        {
            smtpconfig.smtpconfig.smtp_id.NumericValue = Common.Decrypt(smtpconfig.smtpconfig.smtp_id.EncryptedValue);
            result = _repo.DeleteSmtpConfig(smtpconfig.smtpconfig);
        }
    }
}