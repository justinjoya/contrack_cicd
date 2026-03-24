using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class BookingList
    {
        public MasterMenus menu { get; set; } = new MasterMenus();
        public ContainerBookingListDTO containerbookinglist { get; set; } = new ContainerBookingListDTO();
        public Result result { get; set; }
    }
}