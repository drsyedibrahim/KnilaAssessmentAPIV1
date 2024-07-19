using KAAPI.BL.ICustomService;
using KAAPI.DataObject.Entity;
using KAAPI.DataObject.ViewEntity;
using KAAPI.DL.IDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAAPI.BL.CustomService
{
    public class AuthenticationBL : IAuthenticationBL
    {
        private IUnitofWork UnitofWork { get; }

        public AuthenticationBL(IUnitofWork unitofWork)
        {
            UnitofWork = unitofWork;
        }

        public async Task<ResponseEntity<string>> Registration(ContactViewEntity contactModel)
        {
            try
            {
                var model = new ContactEntity()
                {
                    ContactID = contactModel.ContactID,
                    FirstName = contactModel.FirstName,
                    LastName = contactModel.LastName,
                    Address = contactModel.Address,
                    Password = contactModel.Password,
                    City = contactModel.City,
                    Country = contactModel.Country,
                    Email = contactModel.Email,
                    PhoneNumber = contactModel.PhoneNumber,
                    PostalCode = contactModel.PostalCode,
                    State = contactModel.State,
                    IsActive = true
                };
                return await UnitofWork.AuthenicationDL.Registration(model);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseEntity<string>> Authentication(SigninRequestModel signModel)
        {
            return await UnitofWork.AuthenicationDL.Authentication(signModel);
        }

        public async Task<ResponseEntity<bool>> DeleteContact(int ContactID)
        {
            return await UnitofWork.AuthenicationDL.DeleteContact(ContactID);
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID)
        {
            return await UnitofWork.AuthenicationDL.GetByContactID(ContactID);
        } 

        public async Task<ResponseEntity<List<ContactEntity>>> GetContacts(string? search, string? sortColumn, bool isDescending, int page, int pageSize)
        {
            return await UnitofWork.AuthenicationDL.GetContacts(search,sortColumn,isDescending,page,pageSize);
        }
    }
}
