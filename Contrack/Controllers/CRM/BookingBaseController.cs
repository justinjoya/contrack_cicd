using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contrack
{
    public class BookingBaseController : Controller
    {
        public readonly BookingService _service;
        public readonly ClientService _clientService;
        public readonly ContainerModelService _containermodelservice;
        public readonly VoyageService _voyageService;
        public readonly AgencyService _agencyService;
        public readonly TermsandConditionsService _termsandconditions;
        public readonly ChatService _chatService;
        public readonly PricingService _pricingService;

        public BookingBaseController()
        {
            ITermsandConditionsRepository tcrepo = new TermsandConditionsRepository();
            _termsandconditions = new TermsandConditionsService(tcrepo);
            IVoyageRepository VoyageRepository = new VoyageRepository();
            _voyageService = new VoyageService(VoyageRepository);
            IPricingRepository pricingRepository = new PricingRepository();
            _pricingService = new PricingService(pricingRepository);
            IClientRepository client = new ClientRepository();
            _clientService = new ClientService(client);

            IBookingRepository repo = new BookingRepository();
            _service = new BookingService(repo, tcrepo, VoyageRepository, pricingRepository, client);

            IContainerModelRepository ContainerModel = new ContainerModelRepository();
            _containermodelservice = new ContainerModelService(ContainerModel);

            IAgencyRepository agencyRepository = new AgencyRepository();
            _agencyService = new AgencyService(agencyRepository);
            IChatRepository chatRepository = new ChatRepository();
            _chatService = new ChatService(chatRepository);
        }
    }
}