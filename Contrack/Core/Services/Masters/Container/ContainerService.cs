using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Contrack
{
    public class ContainerService : CustomException, IContainerService
    {
        public Result result { get; private set; } = new Result();
        private readonly IContainerRepository _repo;
        public ContainerService(IContainerRepository repo)
        {
            _repo = repo;
        }
        public void SaveContainer(ContainerModal model)
        {
            try
            {
                var dto = model.container;
                if (model.MakeMonth > 0 && model.MakeYear > 0)
                {
                    try
                    {
                        DateTime dt = new DateTime(model.MakeYear, model.MakeMonth, 1);
                        dto.manufacturedate = FormatConvertor.ToDateTimeFormat(dt);
                    }
                    catch
                    {
                        dto.manufacturedate = FormatConvertor.ToDateTimeFormat(DateTime.MinValue);
                    }
                }
                else
                {
                    dto.manufacturedate = FormatConvertor.ToDateTimeFormat(DateTime.MinValue);
                }
                result = _repo.SaveContainer(dto);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }
        public ContainerModal GetContainerByID(string containerid)
        {
            try
            {
                var dto = _repo.GetContainerByID(containerid);
                var model = new ContainerModal { container = dto };

                if (dto.manufacturedate.Value != DateTime.MinValue)
                {
                    model.MakeMonth = dto.manufacturedate.Value.Month;
                    model.MakeYear = dto.manufacturedate.Value.Year;
                }
                return model;
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new ContainerModal();
            }
        }
        public ContainerModal GetContainerByUUID(string containeruuid)
        {
            try
            {
                var dto = _repo.GetContainerByUUID(containeruuid);

                var model = new ContainerModal { container = dto };

                if (dto.manufacturedate.Value != DateTime.MinValue)
                {
                    model.MakeMonth = dto.manufacturedate.Value.Month;
                    model.MakeYear = dto.manufacturedate.Value.Year;
                }
                return model;
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new ContainerModal();
            }
        }
        private void ProcessFilters(ContainerFilterPage filter)
        {
            if (filter.filters == null) filter.filters = new ContainerFilter();
            if (!string.IsNullOrEmpty(filter.filters.containertype_encry))
            {
                long id = (long)Common.Decrypt(filter.filters.containertype_encry);
                if (id > 0) filter.filters.containertypeids = new List<long> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.containersize_encry))
            {
                int id = Common.ToInt(Common.Decrypt(filter.filters.containersize_encry).ToString());
                if (id > 0) filter.filters.containersizeids = new List<int> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.containermodel_encry))
            {
                filter.filters.containermodeluuids = new List<string> { filter.filters.containermodel_encry };
            }
            if (!string.IsNullOrEmpty(filter.filters.location_encry))
            {
                long id = (long)Common.Decrypt(filter.filters.location_encry);
                if (id > 0) filter.filters.locationdetailids = new List<long> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.pol_encry))
            {
                int id = Common.Decrypt(filter.filters.pol_encry);
                if (id > 0) filter.filters.pols = new List<int> { id };
            }
            if (!string.IsNullOrEmpty(filter.filters.pod_encry))
            {
                int id = Common.Decrypt(filter.filters.pod_encry);
                if (id > 0) filter.filters.pods = new List<int> { id };
            }
            if (filter.filters.status > 0)
                filter.filters.status_list = new List<int> { filter.filters.status };
            else
                filter.filters.status_list = new List<int>();
        }
        public void PopulateStatusCounts(ContainerFilterPage filter)
        {
            try
            {
                ProcessFilters(filter);
                string json = JsonConvert.SerializeObject(filter);
                ContainerFilterPage countFilter = JsonConvert.DeserializeObject<ContainerFilterPage>(json);
                filter.StatusCount = _repo.GetContainerStatusCount(countFilter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
        }
        public List<ContainerDTO> GetContainerList(ContainerFilterPage filter)
        {
            try
            {
                ProcessFilters(filter);
                var list = _repo.GetContainerList(filter);
                list.ForEach(x => x.ageinyears = x.manufacturedate.NumericValue != 0 ? Math.Abs(x.manufacturedate.NumericValue / 365) : 0);
                return list;
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<ContainerDTO>();
            }
        }
        public List<ContainerStatusCountDTO> GetContainerStatusCount(ContainerFilterPage filter)
        {
            try
            {
                ProcessFilters(filter);
                return _repo.GetContainerStatusCount(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<ContainerStatusCountDTO>();
            }
        }
        public bool IsContainerAvailable(string containerid)
        {
            return _repo.IsContainerAvailable(containerid);
        }
        public List<ContainerAvailableDTO> IsContainerAvailable(List<string> containerids)
        {
            return _repo.IsContainerAvailable(containerids);
        }
    }
}