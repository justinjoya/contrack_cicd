using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contrack
{
    public class BookedContainer
    {
        public ContainerBookingDTO booking { get; set; } = new ContainerBookingDTO();
        public List<BookedContainerDTO> containers { get; set; } = new List<BookedContainerDTO>();
        public VoyageDTO voyage { get; set; } = new VoyageDTO();
    }
    public class BookedContainerFilter
    {
        public string sort_by { get; set; } = "createdat";
        public string sort_dir { get; set; } = "DESC";
        public int page_no { get; set; } = 0;
        public int page_size { get; set; } = -1;
        public string search_text { get; set; } = "";
        public BookedFilter filters { get; set; } = new BookedFilter();
    }
    public class BookedFilter
    {
        public string typeuuid { get; set; } = "";
        public int sizeid { get; set; } = 0;
        public string sizeidstr { get; set; } = "";
    }
}