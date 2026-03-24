using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ISmtpConfigRepository
    {
        List<SmtpConfigDTO> GetSmtpConfigList();
        SmtpConfigDTO GetSmtpConfigById(string refid);
        Result SaveSmtpConfig(SmtpConfigDTO smtpconfig);
        Result DeleteSmtpConfig (SmtpConfigDTO smtpConfig);
    }
}
