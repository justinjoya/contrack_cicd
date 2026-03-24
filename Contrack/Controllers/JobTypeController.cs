using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers
{

    public class JobTypeController : Controller
    {
        private readonly IJobTypeService _jobTypeService;
        public JobTypeController()
        {
            IJobTypeRepository repo = new JobTypeRepository();
            _jobTypeService = new JobTypeService(repo);
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult JobTypeDetails(string refid)
        {
            var model = new JobType();
            if (!string.IsNullOrEmpty(refid))
            {
                int jobTypeId = Common.Decrypt(refid);
                model.jobtype = _jobTypeService.GetJobTypeByID(jobTypeId);
            }
            return View(model);
        }
        public ActionResult GetJobTypeModal(string refid)
        {
            var model = new JobType();
            if (!string.IsNullOrEmpty(refid))
            {
                int jobTypeId = Common.Decrypt(refid);
                model.jobtype = _jobTypeService.GetJobTypeByID(jobTypeId);
            }
            return PartialView("~/Views/JobType/_ModalJobType.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveJobType(JobType model)
        {
            _jobTypeService.SaveJobType(model);
            if (model.result.ResultId == 1)
            {
                model.result.ResultMessage = Url.Action("List", "JobType");
            }
            return Json(model.result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetJobTypeDetailModal(string refid, string jobtypeid)
        {
            var model = new JobTypeDetailDTO();
            int parentJobTypeId = Common.Decrypt(jobtypeid);
            var parentDto = _jobTypeService.GetJobTypeByID(parentJobTypeId);

            if (parentDto != null)
            {
                model.jobtypename = parentDto.jobtypename;
            }

            if (!string.IsNullOrEmpty(refid))
            {
                int detailId = Common.Decrypt(refid);
                model = _jobTypeService.GetJobTypeDetailByID(detailId);
                model.jobtypename = parentDto?.jobtypename;
            }

            ViewBag.ParentJobTypeIdEncrypted = jobtypeid;

            return PartialView("~/Views/JobType/_ModalJobTypeDetails.cshtml", model);
        }

        [HttpPost]
        public ActionResult SaveJobTypeDetails(JobTypeDetailDTO jobtypedetail, string parentJobTypeIdEncrypted)
        {
            Result result;
            try
            {
                int parentJobTypeId = Common.Decrypt(parentJobTypeIdEncrypted);
                _jobTypeService.SaveJobTypeDetails(jobtypedetail, parentJobTypeId);

                result = Common.SuccessMessage("Job Type Details Updated Successfully!");
                result.ResultMessage = Url.Action("JobTypeDetails", "JobType", new { refid = parentJobTypeIdEncrypted });
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteJobTypeDetails(string detailRefId, string parentRefId)
        {
            try
            {
                int detailId = Common.Decrypt(detailRefId);
                var result = _jobTypeService.DeleteJobTypeDetails(detailId);

                if (result.ResultId == 1)
                {
                    result.ResultMessage = Url.Action("JobTypeDetails", "JobType", new { refid = parentRefId });
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(Common.ErrorMessage(ex.Message), JsonRequestBehavior.AllowGet);
            }
        }
    }
}