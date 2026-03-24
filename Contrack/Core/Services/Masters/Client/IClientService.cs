using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contrack
{
    public interface IClientService
    {
        void SaveClient(ClientDTO client);
        void SaveCustomAttribute(List<KeyValuePair> attributes, int clientId, string clientUUID);
        void SaveBankInfo(List<CustomBankInfo> banks, int clientId, string clientUUID);
        void DeleteClient(int clientId, string clientUUID);
        ClientDTO GetClientByUUID(string clientUUID);
        ClientDTO GetClientByDetailID(int clientDetailId);
        List<ClientDTO> GetClientModificationList(string clientUUID);
        List<Client> GetClientList(ClientListFilter filter);
        List<Client> GetClientListFull(string search = "");
        List<Client> GetClientListForPricingFull(string templateid, string search = "");
        // Address
        void MakePrimary(AddressDTO client);
        void SaveAddress(AddressDTO client);
        List<AddressDTO> GetAddressList(string clientid);
        AddressDTO GetAddressByID(string addressid);
        void DeleteAddress(AddressDTO client);
        // Client Contact
        void SaveContact(ClientContact contact);
        void MakePrimaryContact(string picidinc);
        ClientContact GetContactByID(string picid);
        List<ClientContact> GetContactList(string clientid);
        void DeleteContact(string picidinc);
        BookingCustomerDTO GetCustomerBookingWithAnalytics(int clientdetailid);
    }
}
