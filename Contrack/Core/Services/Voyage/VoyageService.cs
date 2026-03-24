using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contrack
{
    public class VoyageService : CustomException, IVoyageService
    {
        private readonly IVoyageRepository _voyageRepository;
        public Result result = new Result();
        public VoyageService(IVoyageRepository voyageRepository)
        {
            _voyageRepository = voyageRepository;
        }
        public void SaveVoyage(VoyageExtention voyage)
        {
            try
            {
                result = _voyageRepository.SaveVoyage(voyage.VoyageExtendedDTO);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public void AddIntermediatePort(VoyageExtention voyage)
        {
            try
            {
                result = _voyageRepository.AddIntermediatePortToVoyage(
                    voyage.VoyageExtendedDTO
                );
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }

        public void MarkAsArrived(VoyageExtention voyage)
        {
            try
            {
                result = _voyageRepository.MarkAsArrived(
                    voyage.VoyageExtendedDTO
                );
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }
        public void MarkAsDepartured(VoyageExtention voyage)
        {
            try
            {
                result = _voyageRepository.MarkAsDepartured(
                    voyage.VoyageExtendedDTO
                );
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }
        public void DeleteVoyageDetail(int voyageDetailId, int voyageId)
        {
            try
            {
                result = _voyageRepository.DeleteVoyageDetail(voyageDetailId, voyageId);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.Message);
                RecordException(ex);
            }
        }
        public List<Voyage> GetVoyageList(VoyageFilter filter)
        {
            try
            {
                filter.filters.originid = Common.Decrypt(filter.filters.origin_encry);
                filter.filters.destinationid = Common.Decrypt(filter.filters.dest_encry);
                filter.filters.vesselassignmentid = Common.Decrypt(filter.filters.vesselassignmentencrypted);
                var voyages = _voyageRepository.GetVoyageList(filter);
                return voyages.Select(v => new Voyage { VoyageDTO = v }).ToList();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<Voyage>();
            }
        }
        public VoyageExtention GetVoyageByDetailID(int voyageDetailId)
        {
            try
            {
                var dto = _voyageRepository.GetVoyageByDetailID(voyageDetailId);

                if (dto == null)
                    return new VoyageExtention();

                return new VoyageExtention
                {
                    VoyageExtendedDTO = dto
                };
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new VoyageExtention();
            }
        }
        public VoyageExtention GetVoyageById(int voyageId)
        {
            try
            {
                var dto = _voyageRepository.GetVoyageById(voyageId);

                if (dto == null)
                    return new VoyageExtention();
                return new VoyageExtention
                {
                    VoyageExtendedDTO = dto
                };
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new VoyageExtention();
            }
        }
        private void ProcessFilters(VoyageFilter filter)
        {
            if (filter.filters == null)
                filter.filters = new InnerFilters();

            if (!string.IsNullOrEmpty(filter.filters.vesselassignmentencrypted))
            {
                int id = Common.ToInt(Common.Decrypt(filter.filters.vesselassignmentencrypted));
                if (id > 0)
                    filter.filters.vesselassignmentid = id;
            }

            if (!string.IsNullOrEmpty(filter.filters.origin_encry))
            {
                int id = Common.ToInt(Common.Decrypt(filter.filters.origin_encry));
                if (id > 0)
                    filter.filters.originid = id;
            }

            if (!string.IsNullOrEmpty(filter.filters.dest_encry))
            {
                int id = Common.ToInt(Common.Decrypt(filter.filters.dest_encry));
                if (id > 0)
                    filter.filters.destinationid = id;
            }

            if (filter.filters.status > 0)
                filter.filters.status_list = new List<int> { filter.filters.status };
            else
                filter.filters.status_list = new List<int>();
        }

        public void PopulateStatusCounts(VoyageFilter filter)
        {
            try
            {
                ProcessFilters(filter);
                string json = JsonConvert.SerializeObject(filter);
                VoyageFilter countFilter = JsonConvert.DeserializeObject<VoyageFilter>(json);
                filter.StatusCount = _voyageRepository.GetVoyageStatusCount(countFilter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
        }
        public List<voyageStatusCountDTO> GetVoyageStatusCount(VoyageFilter filter)
        {
            try
            {
                ProcessFilters(filter);
                return _voyageRepository.GetVoyageStatusCount(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<voyageStatusCountDTO>();
            }
        }
        public Voyage GetVoyageByUUID(string uuid)
        {
            try
            {
                var voyage = _voyageRepository.GetVoyageByUUID(uuid);
                return new Voyage { VoyageDTO = voyage };
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new Voyage();
            }
        }
        public List<VoyageDTO> SearchVoyage(string search, bool createnew)
        {
            try
            {
                return _voyageRepository.SearchVoyage(search, createnew);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<VoyageDTO>();
            }
        }

        public List<VoyageDTO> GetDirectVoyageSearch(string originportid, string destinationportid, string selectedvoyageuuid = "", bool ischange = false)
        {
            List<VoyageDTO> list = new List<VoyageDTO>();
            try
            {
                list = _voyageRepository.GetDirectVoyageSearch(originportid, destinationportid) ?? new List<VoyageDTO>();
                if (list != null && list.Count > 0)
                {
                    if (!string.IsNullOrEmpty(selectedvoyageuuid))
                    {
                        if (ischange)
                        {
                            var matchedVoyage = list.FirstOrDefault(v => v.VoyageUuid != null && v.VoyageUuid.Equals(selectedvoyageuuid, StringComparison.OrdinalIgnoreCase));
                            if (matchedVoyage != null)
                            {
                                matchedVoyage.selectedvoyageuuid = selectedvoyageuuid;
                            }
                        }
                        else
                        {
                            var selectedVoyage = _voyageRepository.GetVoyageByUUID(selectedvoyageuuid);
                            if (selectedVoyage != null)
                            {
                                selectedVoyage.selectedvoyageuuid = selectedvoyageuuid;
                                return new List<VoyageDTO> { selectedVoyage };
                            }
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<VoyageDTO>();
            }
        }
    }
}
