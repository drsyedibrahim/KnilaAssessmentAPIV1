using KAAPI.BL.CustomService;
using KAAPI.BL.ICustomService;
using KAAPI.DataObject.ViewEntity;
using KAAPI.DL.DataService;
using KAAPI.DL.IDataService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KAAPI.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private IunitofService _unitofService { get; set; }
        public AuthController(IunitofService unitofService)
        {
            _unitofService = unitofService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Authentication(SigninRequestModel model)
        {
            var result = await _unitofService.AuthenticationBL.Authentication(model);
            return new JsonResult(new
            {
                AuthenticationDetails = result.Result,
                result.IsSuccess,
                result.StatusCode,
                result.StatusMessage,
                result.Token,
                result.RefreshToken
            });
        }

        [HttpPost("AddOrEditContact")]
        public async Task<IActionResult> RegisterContact(ContactViewEntity ModelEntity)
        {
            if (ModelState.IsValid)
            {
                var result = await _unitofService.AuthenticationBL.Registration(ModelEntity);
                return new JsonResult(new 
                {
                    resultData = result.Result,
                    result.IsSuccess,
                    result.StatusCode,
                    result.StatusMessage,
                });
            }
            return BadRequest(ModelState);
        }

        [HttpGet("DeleteContactByContactID")]
        public async Task<IActionResult> DeleteContact(int ContactID)
        {
            var result = await _unitofService.AuthenticationBL.DeleteContact(ContactID);
            return new JsonResult(new
            {
                resultData = result.Result,
                result.IsSuccess,
                result.StatusCode,
                result.StatusMessage,
            });
        }

        [HttpGet("GetByContactID")]
        public async Task<IActionResult> GetByContactID(int ContactID)
        {
            var result = await _unitofService.AuthenticationBL.GetByContactID(ContactID);
            return new JsonResult(new
            {
                resultData = result.Result,
                result.IsSuccess,
                result.StatusCode,
                result.StatusMessage,
            });
        }

        [HttpGet("GetAllContactList")]
        public async Task<IActionResult>GetAllContactList(string? search, string? sortColumn, bool isAsc, int page = 1, int pageSize = 5)
        {
            return new JsonResult(await _unitofService.AuthenticationBL.GetContacts(search, sortColumn, isAsc, page, pageSize));
        }
    }
}
