using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Web;
using System.Web.UI.WebControls;
using static Contrack.PageSessionManager;
namespace Contrack
{
    public static class SessionManager
    {
        public static Login LoginSession
        {
            get
            {
                Object obj = HttpContext.Current.Session["Login"];
                if (obj == null)
                    return null;
                else
                    return (Login)obj;
            }
            set
            {
                HttpContext.Current.Session["Login"] = value;
            }
        }

        public static List<string> EntityIDs
        {
            get
            {
                Object obj = HttpContext.Current.Session["EntityIDs"];
                if (obj == null)
                    return null;
                else
                    return (List<string>)obj;
            }
            set
            {
                HttpContext.Current.Session["EntityIDs"] = value;
            }
        }
        public static string TimeOffSet
        {
            get
            {
                try
                {
                    Object obj = HttpContext.Current.Session["TimeOffSet"];
                    if (obj == null)
                        return "0";
                    else
                        return (string)obj;
                }
                catch
                {
                    return "0";
                }
            }
            set
            {
                HttpContext.Current.Session["TimeOffSet"] = value;
            }
        }
        public static Stack<string> NavigationStack
        {
            get
            {
                Object obj = HttpContext.Current.Session["NavigationStack"];
                if (obj == null)
                {
                    obj = new Stack<string>();
                }
                return (Stack<string>)obj;
            }
            set
            {
                HttpContext.Current.Session["NavigationStack"] = value;
            }
        }

        public static VendorDTO CurrentVendor
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentVendor"];
                if (obj == null)
                    return null;
                else
                    return (VendorDTO)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentVendor"] = value;
            }
        }

        public static ClientListFilter ClientListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["ClientListFilter"];
                if (obj == null)
                    return null;
                else
                    return (ClientListFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["ClientListFilter"] = value;
            }
        }
        public static BookingListFilter BookingListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["BookingListFilter"];
                if (obj == null)
                    return null;
                else
                    return (BookingListFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["BookingListFilter"] = value;
            }
        }
        public static QuotationListFilter QuotationListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["QuotationListFilter"];
                if (obj == null)
                    return null;
                else
                    return (QuotationListFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["QuotationListFilter"] = value;
            }
        }
        public static ContainerFilterPage ContainerListFilter
        {
            get
            {
                object obj = HttpContext.Current.Session["ContainerListFilter"];
                if (obj == null)
                    return null;
                return (ContainerFilterPage)obj;
            }
            set
            {
                HttpContext.Current.Session["ContainerListFilter"] = value;
            }
        }
        public static UserFilter UserListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["UserListFilter"];
                return obj == null ? null : (UserFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["UserListFilter"] = value;
            }
        }

        public static int UserListTab
        {
            get
            {
                Object obj = HttpContext.Current.Session["UserListTab"];
                return obj == null ? 0 : (int)obj;
            }
            set
            {
                HttpContext.Current.Session["UserListTab"] = value;
            }
        }
        public static VesselFilter VesselListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["VesselListFilter"];
                if (obj == null)
                    return null;
                else
                    return (VesselFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["VesselListFilter"] = value;
            }
        }
        public static VoyageFilter VoyageListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["VoyageListFilter"];
                if (obj == null)
                    return null;
                else
                    return (VoyageFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["VoyageListFilter"] = value;
            }
        }
        public static VendorFilter VendorListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["VendorListFilter"];
                if (obj == null)
                    return null;
                else
                    return (VendorFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["VendorFilter"] = value;
            }
        }

        public static ClientDTO CurrentClient
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentClient"];
                if (obj == null)
                    return null;
                else
                    return (ClientDTO)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentClient"] = value;
            }
        }

        public static Agency CurrenyAgency
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrenyAgency"];
                if (obj == null)
                    return null;
                else
                    return (Agency)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrenyAgency"] = value;
            }
        }
        public static LocationListFilter LocationListFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["LocationListFilter"];

                if (obj == null)
                {
                    var newFilter = new LocationListFilter();
                    HttpContext.Current.Session["LocationListFilter"] = newFilter;
                    return newFilter;
                }

                return (LocationListFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["LocationListFilter"] = value;
            }
        }
        public static Client CurrenyClient
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrenyClient"];
                if (obj == null)
                    return null;
                else
                    return (Client)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrenyClient"] = value;
            }
        }
        public static List<PIJobType> PIJobTypes
        {
            get
            {
                Object obj = HttpContext.Current.Session["PIJobTypes"];
                if (obj == null)
                    return null;
                else
                    return (List<PIJobType>)obj;
            }
            set
            {
                HttpContext.Current.Session["PIJobTypes"] = value;
            }
        }

        public static List<KeyValuePair> CurrentPurchaseIntentMiscDetails
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentPurchaseIntentMiscDetails"];
                if (obj == null)
                    return null;
                else
                    return (List<KeyValuePair>)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentPurchaseIntentMiscDetails"] = value;
            }
        }
        public static List<PurchaseIntentDetailJob> CurrentPurchaseIntentDetails
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentPurchaseIntentDetails"];
                if (obj == null)
                    return null;
                else
                    return (List<PurchaseIntentDetailJob>)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentPurchaseIntentDetails"] = value;
            }
        }
        public static PurchaseIntentDTO CurrentPurchaseIntent
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentPurchaseIntent"];
                return obj == null ? null : (PurchaseIntentDTO)obj;
            }
            set { HttpContext.Current.Session["CurrentPurchaseIntent"] = value; }
        }

        public static VendorInvoiceDTO CurrentVendorInvoice
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentVendorInvoice"];
                return obj == null ? null : (VendorInvoiceDTO)obj;
            }
            set { HttpContext.Current.Session["CurrentVendorInvoice"] = value; }
        }
        public static List<VendorInvoiceDetail> CurrentVendorInvoiceDetails
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentVendorInvoiceDetails"];
                if (obj == null)
                    return null;
                else
                    return (List<VendorInvoiceDetail>)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentVendorInvoiceDetails"] = value;
            }
        }
        public static ContainerModelFilter ContainerModelFilter
        {
            get
            {
                object obj = HttpContext.Current.Session["ContainerModelFilter"];
                if (obj == null)
                    return null;
                return (ContainerModelFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["ContainerModelFilter"] = value;
            }
        }

        public static ContainerBooking Booking
        {
            get
            {
                object obj = HttpContext.Current.Session["Booking"];
                if (obj == null)
                    return null;
                return (ContainerBooking)obj;
            }
            set
            {
                HttpContext.Current.Session["Booking"] = value;
            }
        }

        public static BookingQuotation Quotation
        {
            get
            {
                object obj = HttpContext.Current.Session["CurrentQuotation"];
                if (obj == null)
                    return null;
                return (BookingQuotation)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentQuotation"] = value;
            }
        }
        public static List<QuotationDetailDTO> QuotationDetails
        {
            get
            {
                object obj = HttpContext.Current.Session["CurrentQuotationDetails"];
                if (obj == null)
                    return null;
                return (List<QuotationDetailDTO>)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentQuotationDetails"] = value;
            }
        }

        public static List<IconDTO> Icons
        {
            get
            {
                object obj = HttpContext.Current.Session["Icons"];
                if (obj == null)
                    return null;
                return (List<IconDTO>)obj;
            }
            set
            {
                HttpContext.Current.Session["Icons"] = value;
            }
        }
        public static Dictionary<string, PageState> UserPageStates
        {
            get
            {
                object obj = HttpContext.Current.Session["UserPageStates"];
                if (obj == null)
                    return new Dictionary<string, PageState>();
                return (Dictionary<string, PageState>)obj;
            }
            set
            {
                HttpContext.Current.Session["UserPageStates"] = value;
            }
        }

        public static ContainerSelection CurrentContainerSelection
        {
            get
            {
                object obj = HttpContext.Current.Session["CurrentContainerSelection"];
                if (obj == null)
                    return null;
                return (ContainerSelection)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentContainerSelection"] = value;
            }
        }
        //public static PurchaseOrder CurrentPO
        //{
        //    get
        //    {
        //        Object obj = HttpContext.Current.Session["CurrentPO"];
        //        if (obj == null)
        //            return null;
        //        else
        //            return (PurchaseOrder)obj;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CurrentPO"] = value;
        //    }
        //}
        public static List<PurchaseOrderDetailDTO> CurrentPODetails
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentPODetails"];
                if (obj == null)
                    return null;
                else
                    return (List<PurchaseOrderDetailDTO>)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentPODetails"] = value;
            }
        }

        //public static List<PurchaseOrderDetail> CurrentPODetails
        //{
        //    get
        //    {
        //        Object obj = HttpContext.Current.Session["CurrentPODetails"];
        //        if (obj == null)
        //            return null;
        //        else
        //            return (List<PurchaseOrderDetail>)obj;
        //    }
        //    set
        //    {
        //        HttpContext.Current.Session["CurrentPODetails"] = value;
        //    }
        //}
        public static PurchaseOrderModel CurrentPurchaseOrderModel
        {
            get
            {
                Object obj = HttpContext.Current.Session["CurrentPurchaseOrderModel"];
                if (obj == null)
                    return null;
                else
                    return (PurchaseOrderModel)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentPurchaseOrderModel"] = value;
            }
        }
        public static PurchaseOrderFilterPage PurchaseOrderFilter
        {
            get
            {
                Object obj = HttpContext.Current.Session["PurchaseOrderFilter"];
                if (obj == null)
                    return null;
                else
                    return (PurchaseOrderFilterPage)obj;
            }
            set
            {
                HttpContext.Current.Session["PurchaseOrderFilter"] = value;
            }
        }
        public static List<Status> AllStatus
        {
            get
            {
                Object obj = HttpContext.Current.Session["AllStatus"];
                if (obj == null)
                    return null;
                else
                    return (List<Status>)obj;
            }
            set
            {
                HttpContext.Current.Session["AllStatus"] = value;
            }
        }
        public static AllStats AllStatsSelected
        {
            get
            {
                Object obj = HttpContext.Current.Session["AllStatsSelected"];
                if (obj == null)
                    return new AllStats();
                else
                    return (AllStats)obj;
            }
            set
            {
                HttpContext.Current.Session["AllStatsSelected"] = value;
            }
        }
        public static EmailDTO CurrentEmail
        {
            get
            {
                object obj = HttpContext.Current.Session["CurrentEmail"];
                if (obj == null)
                    return null;
                else
                    return (EmailDTO)obj;
            }
            set
            {
                HttpContext.Current.Session["CurrentEmail"] = value;
            }
        }
        public static List<string> Emails
        {
            get
            {
                Object obj = HttpContext.Current.Session["Emails"];
                if (obj == null)
                    return null;
                else
                    return (List<string>)obj;
            }
            set
            {
                HttpContext.Current.Session["Emails"] = value;
            }
        }
        public static TrackingFilterPage TrackingListFilter
        {
            get
            {
                object obj = HttpContext.Current.Session["TrackingListFilter"];
                if (obj == null)
                    return null;
                return (TrackingFilterPage)obj;
            }
            set
            {
                HttpContext.Current.Session["TrackingListFilter"] = value;
            }
        }
        public static PricingListFilter PricingListFilter
        {
            get
            {
                object obj = HttpContext.Current.Session["PricingListFilter"];
                if (obj == null)
                    return null;
                return (PricingListFilter)obj;
            }
            set
            {
                HttpContext.Current.Session["PricingListFilter"] = value;
            }
        }
    }
}