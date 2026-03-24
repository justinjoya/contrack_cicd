using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ITrackingRepository
    {
        Result SaveTracking(TrackingDTO tracking);
        TrackingDTO GetTrackingByUUID(string trackingUuid);
        Result SaveTrackingDamage(TrackingDamageDTO trackingDamage, string trackingIdEncrypt);
        List<MovesDTO> GetMovesList();
        List<TrackingListDTO> GetTrackingList(TrackingFilterPage filter);
        Result SavePickSelection(List<TrackingSelectionDTO> containerBookingSelection);
        List<TrackingDetailsDTO> GetTrackingDetails(string conataineruuid, string bookinguuid);
    }
}
