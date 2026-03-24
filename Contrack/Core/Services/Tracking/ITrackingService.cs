using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface ITrackingService
    {
        void SaveTracking(Tracking tracking);
        Tracking GetTrackingByUUID(string trackingUuid);
        void SaveTrackingDamage(TrackingDamage trackingDamage, string trackingIdEncrypt);
        List<Moves> GetMovesList();
        void SavePickSelection(List<TrackingSelectionDTO> model);
        TrackingDetails GetTrackingDetails(string containeruuid, string bookinguuid, ContainerBooking booking);
    }
}
