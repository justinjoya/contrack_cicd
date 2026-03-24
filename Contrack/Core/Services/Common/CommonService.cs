using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class CommonService : CustomException, ICommonService
    {
        public Result result = new Result();
        private readonly ICommonRepository _repo;
        public CommonService(ICommonRepository repo)
        {
            _repo = repo;
        }
        public List<IconDTO> GetIcons()
        {
            return _repo.GetIcons();
        }
    }
}