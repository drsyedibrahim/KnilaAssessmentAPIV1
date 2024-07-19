using KAAPI.DataObject.Entity;
using KAAPI.DataObject.ViewEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAAPI.DL.IDataService
{
    public interface IAuthenicationDL
    {
        Task<ResponseEntity<string>> Registration(ContactEntity contactModel);
        Task<ResponseEntity<string>> Authentication(SigninRequestModel signinRequestModel);
        Task<ResponseEntity<bool>> DeleteContact(int ContactID);
        Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID);
        Task<ResponseEntity<List<ContactEntity>>> GetContacts(string? search, string? sortColumn, bool isDescending, int page, int pageSize);
    }
}
