using KAAPI.DataObject.Entity;
using KAAPI.DataObject.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAAPI.BL.ICustomService
{
    public interface IAuthenticationBL
    {
        public Task<ResponseEntity<string>> Registration(ContactViewEntity contactModel);
        public Task<ResponseEntity<string>> Authentication(SigninRequestModel signModel);
        public Task<ResponseEntity<bool>> DeleteContact(int ContactID);
        Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID);
        Task<ResponseEntity<List<ContactEntity>>> GetContacts(string? search, string? sortColumn, bool isDescending, int page, int pageSize);
    }
}
