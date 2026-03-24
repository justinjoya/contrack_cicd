using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Contrack
{
    public class ClientService : CustomException, IClientService
    {
        public Result result = new Result();
        private readonly IClientRepository _repo;
        public ClientService(IClientRepository repo)
        {
            _repo = repo;
        }

        public void SaveClient(ClientDTO client)
        {
            try
            {
                if (!string.IsNullOrEmpty(client.emailtemp))
                {
                    if (!string.IsNullOrEmpty(client.email))
                        client.email = client.email + ";" + client.emailtemp;
                    else
                        client.email = client.emailtemp;
                }

                if (!string.IsNullOrEmpty(client.accountsemailtemp))
                {
                    if (!string.IsNullOrEmpty(client.accountsemail))
                        client.accountsemail = client.accountsemail + ";" + client.accountsemailtemp;
                    else
                        client.accountsemail = client.accountsemailtemp;
                }

                if (Common.Decrypt(client.TypeEncrypted) == 1)
                    client.agency.agencyid.EncryptedValue = Common.Encrypt(0);

                result = _repo.SaveClient(client);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveCustomAttribute(List<KeyValuePair> attributes, int clientId, string clientUUID)
        {
            try
            {
                result = _repo.SaveCustomAttribute(attributes, clientId, clientUUID);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveBankInfo(List<CustomBankInfo> bankInfo, int clientId, string clientUUID)
        {
            try
            {
                result = _repo.SaveBankInfo(bankInfo, clientId, clientUUID);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void DeleteClient(int clientId, string clientUUID)
        {
            try
            {
                result = _repo.DeleteClient(clientId, clientUUID);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public ClientDTO GetClientByUUID(string clientUUID)
        {
            ClientDTO client = _repo.GetClientByUUID(clientUUID);
            SessionManager.CurrentClient = client;
            return client;
        }

        public ClientDTO GetClientByDetailID(int clientDetailId)
        {
            return _repo.GetClientByDetailID(clientDetailId);
        }

        public List<ClientDTO> GetClientModificationList(string clientUUID)
        {
            return _repo.GetClientModificationList(clientUUID);
        }

        public List<Client> GetClientList(ClientListFilter filter)
        {
            List<ClientDTO> list = _repo.GetClientList(filter);
            return list.Select(x => new Client()
            {
                client = x,
                menus = new MasterMenus()
                {
                    edit = true//PagePermission.GetPermissions().Agency.edit
                }
            }).ToList();
        }
        public List<Client> GetClientListFull(string search)
        {
            List<ClientDTO> list = _repo.GetClientListFull(search);
            return list.Select(x => new Client()
            {
                client = x,
            }).ToList();
        }
        public List<Client> GetClientListForPricingFull(string templateid, string search)
        {
            List<ClientDTO> list = _repo.GetClientListForPricingFull(templateid, search);
            return list.Select(x => new Client()
            {
                client = x,
            }).ToList();
        }


        #region Address
        public void MakePrimary(AddressDTO address)
        {
            try
            {
                if (address == null)
                {
                    result = Common.ErrorMessage("Address data cannot be null");
                    return;
                }
                result = _repo.MakePrimary(address);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void SaveAddress(AddressDTO address)
        {
            try
            {
                if (address == null)
                {
                    result = Common.ErrorMessage("Address data cannot be null");
                    return;
                }
                if (string.IsNullOrWhiteSpace(address.address))
                {
                    result = Common.ErrorMessage("Address cannot be empty");
                    return;
                }
                if (string.IsNullOrWhiteSpace(address.addressname))
                {
                    result = Common.ErrorMessage("Address name cannot be empty");
                    return;
                }
                if (Common.Decrypt(address.clientid.EncryptedValue) == 0)
                {
                    result = Common.ErrorMessage("Invalid Client");
                    return;
                }
                result = _repo.SaveAddress(address);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public List<AddressDTO> GetAddressList(string clientid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(clientid))
                    return new List<AddressDTO>();

                return _repo.GetAddressList(clientid) ?? new List<AddressDTO>();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<AddressDTO>();
            }
        }

        public AddressDTO GetAddressByID(string addressid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(addressid))
                    return new AddressDTO();

                return _repo.GetAddressByID(addressid) ?? new AddressDTO();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new AddressDTO();
            }
        }

        public void DeleteAddress(AddressDTO address)
        {
            try
            {
                if (address == null)
                {
                    result = Common.ErrorMessage("Address data cannot be null");
                    return;
                }
                result = _repo.DeleteAddress(address);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        #endregion

        #region Client Contact
        public void SaveContact(ClientContact contact)
        {
            try
            {
                if (Common.Decrypt(contact.clientid.EncryptedValue) == 0)
                {
                    result = Common.ErrorMessage("Invalid Client");
                    return;
                }
                result = _repo.SaveContact(contact);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public void MakePrimaryContact(string picidinc)
        {
            try
            {
                result = _repo.MakePrimaryContact(picidinc);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }

        public ClientContact GetContactByID(string picid)
        {
            try
            {
                return _repo.GetContactByID(picid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new ClientContact();
            }
        }

        public List<ClientContact> GetContactList(string clientid)
        {
            try
            {
                return _repo.GetContactList(clientid);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                return new List<ClientContact>();
            }
        }

        public void DeleteContact(string picidinc)
        {
            try
            {
                result = _repo.DeleteContact(picidinc);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
        }
        #endregion

        public BookingCustomerDTO GetCustomerBookingWithAnalytics(int clientDetailId)
        {
            BookingCustomerDTO info = new BookingCustomerDTO();
            try
            {
                info.client = _repo.GetClientByDetailID(clientDetailId);
                info.analytics = _repo.GetClientEnquiryAnalytics(info.client.clientid.EncryptedValue);
            }
            catch (Exception ex)
            {
                result = Common.ErrorMessage(ex.ToString());
                RecordException(ex);
            }
            return info;
        }
    }
}