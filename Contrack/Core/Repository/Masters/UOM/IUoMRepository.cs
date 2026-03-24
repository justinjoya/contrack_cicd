using System.Collections.Generic;

namespace Contrack
{
    public interface IUoMRepository
    {
        Result SaveUoM(UoMDTO dto);
        List<UoMDTO> GetUoMList(string search = "");
        Result DeleteUoM(int uomid);
    }
}
