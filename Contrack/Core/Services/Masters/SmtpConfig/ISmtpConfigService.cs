using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ISmtpConfigService
    {
        List<SmtpConfig> GetSmtpConfigList(string search);
        SmtpConfigDTO GetSmtpConfigById(string refid);
        void SaveSmtpConfig(SmtpConfig smtpconfig);
        void DeleteSmtpConfig(SmtpConfig smtpconfig);
    }
}
