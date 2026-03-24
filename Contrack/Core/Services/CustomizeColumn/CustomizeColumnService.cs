using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class CustomizeColumnService : CustomException, ICustomizeColumnService
    {
        public Result result = new Result();
        private readonly ICustomizeColumnRepository _repo;
        public CustomizeColumnService(ICustomizeColumnRepository repo)
        {
            _repo = repo;
        }
        public List<CustomizeColumn> GetMenus(int menutype)
        {
            List<CustomizeColumnDTO> list = _repo.GetMenus(menutype);

            return list.Select(x => new CustomizeColumn()
            {
                customizecolumn = x

            }).ToList();
        }
        public void Save(CustomizeColumnDTO customizecolumn)
        {
            try
            {
                result = _repo.Save(customizecolumn);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
    }
}