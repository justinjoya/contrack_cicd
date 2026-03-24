using System.Collections.Generic;

namespace Contrack
{
    public interface IUoMService
    {
        void SaveUoM(UoMDTO dto);
        List<UoMDTO> GetUoMList(string search = "");
        void DeleteUoM(int uomid);
    }
}
