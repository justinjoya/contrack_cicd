using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    public class TrackingService : CustomException, ITrackingService
    {
        private readonly ITrackingRepository _repo;
        public TrackingService(ITrackingRepository repo)
        {
            _repo = repo;
        }
        public Result result = new Result();
        public void SaveTracking(Tracking tracking)
        {
            try
            {
                bool isValid = true;
                var moveId = tracking.Trackingmodel.Moves.MovesId.EncryptedValue;
                var nextMoveId = tracking.Trackingmodel.NextMoves.MovesId.EncryptedValue;
                var nextLocationId = tracking.Trackingmodel.NextLocationDetailId.EncryptedValue;
                var nextVoyageId = tracking.Trackingmodel.NextVoyageId.EncryptedValue;
                var nextDateTime = tracking.Trackingmodel.NextDateTime;

                if (string.IsNullOrWhiteSpace(moveId))
                {
                    result = Common.ErrorMessage("Please select a move type.");
                    isValid = false;
                }

                if (!string.IsNullOrWhiteSpace(nextMoveId))
                {
                    if (string.IsNullOrWhiteSpace(nextLocationId) && string.IsNullOrWhiteSpace(nextVoyageId))
                    {
                        result = Common.ErrorMessage("Please select next location or voyage");
                        isValid = false;
                    }

                    if (string.IsNullOrWhiteSpace(nextDateTime))
                    {
                        result = Common.ErrorMessage("Please select next date time.");
                        isValid = false;
                    }
                }

                if (!string.IsNullOrWhiteSpace(nextLocationId) ||
                    !string.IsNullOrWhiteSpace(nextDateTime))
                {
                    if (string.IsNullOrWhiteSpace(nextMoveId))
                    {
                        result = Common.ErrorMessage("Please select next move.");
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    var model = tracking.Trackingmodel;

                    if (!string.IsNullOrWhiteSpace(model.CurrentVoyageId.EncryptedValue))
                        model.LocationDetailId.EncryptedValue = "";
                    else if (!string.IsNullOrWhiteSpace(model.LocationDetailId.EncryptedValue))
                        model.CurrentVoyageId.EncryptedValue = "";

                    if (!string.IsNullOrWhiteSpace(model.NextVoyageId.EncryptedValue))
                        model.NextLocationDetailId.EncryptedValue = "";
                    else if (!string.IsNullOrWhiteSpace(model.NextLocationDetailId.EncryptedValue))
                        model.NextVoyageId.EncryptedValue = "";

                    result = _repo.SaveTracking(model);
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public Tracking GetTrackingByUUID(string trackingUuid)
        {
            Tracking tracking = new Tracking();
            try
            {
                if (!string.IsNullOrEmpty(trackingUuid))
                {
                    tracking.Trackingmodel = _repo.GetTrackingByUUID(trackingUuid);
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return tracking;
        }

        public void SaveTrackingDamage(TrackingDamage trackingDamage, string trackingIdEncrypt)
        {
            try
            {
                result = _repo.SaveTrackingDamage(trackingDamage.Damage, trackingIdEncrypt);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        public List<TrackingListDTO> GetTrackingList(TrackingFilterPage filter)
        {
            try
            {
                ProcessFilters(filter);
                return _repo.GetTrackingList(filter);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<TrackingListDTO>();
            }
        }
        private void ProcessFilters(TrackingFilterPage filter)
        {
            if (filter.filters == null) filter.filters = new TrackingFilter();
            if (!string.IsNullOrEmpty(filter.filters.activity_encrypted))
            {
                filter.filters.activityid = Common.ToInt(Common.Decrypt(filter.filters.activity_encrypted));
            }
            else
            {
                filter.filters.activityid = 0;
            }
            if (string.IsNullOrWhiteSpace(filter.filters.startdate))
                filter.filters.startdate = null;
            if (string.IsNullOrWhiteSpace(filter.filters.enddate))
                filter.filters.enddate = null;
        }
        public List<Moves> GetMovesList()
        {
            List<MovesDTO> list = new List<MovesDTO>();
            try
            {
                list = _repo.GetMovesList();
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return list.Select(x => new Moves()
            {
                Movesmodel = x
            }).ToList();
        }

        public void SavePickSelection(List<TrackingSelectionDTO> model)
        {
            try
            {
                if (model == null || model.Count == 0)
                {
                    result = Common.ErrorMessage("Please select the conatiner to track.");
                }
                else
                {
                    result = _repo.SavePickSelection(model);
                }
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public TrackingDetails GetTrackingDetails(string containeruuid, string bookinguuid, ContainerBooking booking)
        {
            List<TrackingDetailsDTO> list = new List<TrackingDetailsDTO>();
            try
            {
                if (!string.IsNullOrEmpty(containeruuid) && !string.IsNullOrEmpty(bookinguuid))
                {
                    list = _repo.GetTrackingDetails(containeruuid, bookinguuid);
                    if (list.Count > 0)
                    {
                        AddNextMoveOfLastMove(list);
                        AddVoyageOfBooking(list, booking);
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }
            return new TrackingDetails()
            {
                Trackingdetails = list
            };
        }

        private void AddNextMoveOfLastMove(List<TrackingDetailsDTO> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    var lastmove = list.Last();
                    if (lastmove.NextMoveId.NumericValue > 0)
                    {
                        list.Add(new TrackingDetailsDTO()
                        {
                            MoveTypeId = lastmove.NextMoveId,
                            CurrentMovesName = lastmove.NextMovesName,
                            CurrentMovesIcon = lastmove.NextMovesIcon,
                            LocationDetailId = lastmove.NextLocationDetailId,
                            CurrentLocationName = lastmove.NextLocationName,
                            CurrentLocationCode = lastmove.NextLocationCode,
                            CurrentCountryCode = lastmove.NextCountryCode,
                            CurrentCountryFlag = lastmove.NextCountryFlag,
                            CurrentPortId = lastmove.NextPortId,
                            CurrentPortCode = lastmove.NextPortCode,
                            CurrentPortName = lastmove.NextPortName,
                            RecordDateTime = lastmove.NextDateTime,
                            CurrentCountryName = lastmove.NextCountryName,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

        }
        private void AddVoyageOfBooking(List<TrackingDetailsDTO> list, ContainerBooking booking)
        {
            try
            {
                list.Add(new TrackingDetailsDTO()
                {
                    MoveTypeId = new EncryptedData(),
                    CurrentMovesName = "Port of Discharge",
                    CurrentMovesIcon = "",
                    LocationDetailId = new EncryptedData(),
                    CurrentLocationName = "",
                    CurrentLocationCode = "",
                    CurrentCountryCode = "",
                    CurrentCountryFlag = "",
                    CurrentPortId = new EncryptedData(),
                    CurrentPortCode = booking.voyage.Vesselname,
                    CurrentPortName = booking.voyage.VoyageNumber,
                    RecordDateTime = booking.booking.location.pod_portname,
                    CurrentCountryName = "",
                });
            }
            catch (Exception ex)
            {
                RecordException(ex);
            }

        }
    }
}