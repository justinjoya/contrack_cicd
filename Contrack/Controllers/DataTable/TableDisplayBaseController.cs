using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack.Controllers
{
    public class TableDisplayBaseController : Controller
    {
        protected readonly AgencyService _agencyService;
        protected readonly LoginService _loginService;
        protected readonly UoMService _uomService;
        protected readonly JobTypeService _jobTypeService;
        protected readonly ClientService _clientService;
        protected readonly VesselService _vesselService;
        protected readonly VendorService _vendorService;
        protected readonly SmtpConfigService _smtpConfigService;
        protected readonly VoyageService _voyageService;
        protected readonly LocationService _locationService;
        protected readonly ContainerModelService _containermodelservice;
        protected readonly ContainerService _containerService;
        protected readonly BookingService _bookingService;
        protected readonly TrackingService _trackingService;
        protected readonly TermsandConditionsService _TermsandConditionsService;
        protected readonly PricingService _pricingService;
        protected readonly PurchaseOrderService _poService;



        public TableDisplayBaseController()
        {
            IAgencyRepository agencyRepo = new AgencyRepository();
            ILoginRepository loginRepo = new LoginRepository();
            IHubRepository hubRepo = new HubRepository();
            IUoMRepository uomRepo = new UoMRepository();
            IJobTypeRepository jobTypeRepo = new JobTypeRepository();
            IClientRepository clientRepo = new ClientRepository();
            IVesselRepository vesselRepo = new VesselRepository();
            IDocumentRepository docRepo = new DocumentRepository();
            IVendorRepository vendorRepo = new VendorRepository(docRepo);
            ISmtpConfigRepository smtpConfigRepo = new SmtpConfigRepository();
            IVoyageRepository voyageRepo = new VoyageRepository();
            ILocationRepository locationRepo = new LocationRepository();
            IContainerModelRepository ContainerModelRepo = new ContainerModelRepository();
            IContainerRepository containerRepo = new ContainerRepository();
            IBookingRepository bookingRepo = new BookingRepository();
            ITrackingRepository trackingRepo = new TrackingRepository();
            ITermsandConditionsRepository TermsandConditionsRepo = new TermsandConditionsRepository();
            IPricingRepository pricingRepo = new PricingRepository();
            IPurchaseOrderRepository poRepo = new PurchaseOrderRepository();


            _agencyService = new AgencyService(agencyRepo);
            _loginService = new LoginService(loginRepo, hubRepo);
            _uomService = new UoMService(uomRepo);
            _jobTypeService = new JobTypeService(jobTypeRepo);
            _clientService = new ClientService(clientRepo);
            _vesselService = new VesselService(vesselRepo);
            _vendorService = new VendorService(vendorRepo);
            _smtpConfigService = new SmtpConfigService(smtpConfigRepo);
            _voyageService = new VoyageService(voyageRepo);
            _locationService = new LocationService(locationRepo);
            _containermodelservice = new ContainerModelService(ContainerModelRepo);
            _containerService = new ContainerService(containerRepo);
            _bookingService= new BookingService(bookingRepo);
            _trackingService = new TrackingService(trackingRepo);
            _TermsandConditionsService = new TermsandConditionsService(TermsandConditionsRepo);
            _pricingService = new PricingService(pricingRepo);
            _poService = new PurchaseOrderService(poRepo);


        }
    }
}