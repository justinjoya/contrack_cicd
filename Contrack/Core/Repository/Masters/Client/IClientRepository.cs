using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IClientRepository
    {
        Result SaveClient(ClientDTO client);
        Result SaveCustomAttribute(List<KeyValuePair> attributes, int clientId, string clientUUID);
        Result SaveBankInfo(List<CustomBankInfo> bankInfo, int clientId, string clientUUID);
        ClientDTO GetClientByUUID(string clientUUID);
        ClientDTO GetClientByDetailID(int clientDetailId);
        List<ClientDTO> GetClientModificationList(string clientUUID);
        List<ClientDTO> GetClientList(ClientListFilter filter);
        List<ClientDTO> GetClientListFull(string search = "");
        List<ClientDTO> GetClientListForPricingFull(string templateid, string search = "");
        Result DeleteClient(int clientId, string clientUUID);
        // Address
        Result MakePrimary(AddressDTO client);
        Result SaveAddress(AddressDTO client);
        List<AddressDTO> GetAddressList(string clientid);
        AddressDTO GetAddressByID(string addressid);
        Result DeleteAddress(AddressDTO client);
        // Client
        Result SaveContact(ClientContact contact);
        Result MakePrimaryContact(string picidinc);
        ClientContact GetContactByID(string picid);
        List<ClientContact> GetContactList(string clientid);
        Result DeleteContact(string picidinc);
        CustomerAnalyticsDTO GetClientEnquiryAnalytics(string clientid);

    }
}
