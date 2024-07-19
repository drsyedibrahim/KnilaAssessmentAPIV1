using KAAPI.DataObject.Context;
using KAAPI.DataObject.Entity;
using KAAPI.DataObject.ViewEntity;
using KAAPI.DL.IDataService;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;


namespace KAAPI.DL.DataService
{
    public class AuthenicationDL : IAuthenicationDL
    {
        private IApplicationDBContext _context { get; }
        private ILogger<IAuthenicationDL> Logger { get; }
        private readonly IConfiguration _configuration;
        public AuthenicationDL(IApplicationDBContext context , IConfiguration configuration, ILogger<IAuthenicationDL> logger)
        {
            _context = context;
            _configuration = configuration;
            Logger = logger;
        }
        public async Task<ResponseEntity<string>> Registration(ContactEntity contactModel)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    string responseMessage = "";
                    var contactData = await _context.Contact.Where(k => k.ContactID == contactModel.ContactID).FirstOrDefaultAsync();
                    if (contactData != null)
                    {
                        var patchDta = new JsonPatchDocument<ContactEntity>()
                            .Replace(p => p.FirstName, contactModel.FirstName)
                            .Replace(p => p.LastName, contactModel.LastName)
                            .Replace(p => p.Email, contactModel.Email)
                            .Replace(p => p.Password, contactModel.Password)
                            .Replace(p => p.Address, contactModel.Address)
                            .Replace(p => p.City, contactModel.City)
                            .Replace(p => p.State, contactModel.State)
                            .Replace(p => p.Country, contactModel.Country)
                            .Replace(p => p.UpdatedDate, contactModel.UpdatedDate)
                            .Replace(P => P.PhoneNumber, contactModel.PhoneNumber);
                        patchDta.ApplyTo(contactData);
                        responseMessage = "Contact Updated Successfully!";
                    }
                    else
                    {
                        await _context.Contact.AddAsync(contactModel);
                        responseMessage = "Contact Added Successfully!";
                    }

                    await _context.SaveChangesAsync(default);
                    await transaction.CommitAsync();
                    return new ResponseEntity<string>()
                    {
                        Result = responseMessage,
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Logger.LogError($"Exception Message - {ex.Message} ({ex.InnerException?.Message})", ex);
                    return new ResponseEntity<string>()
                    {
                        Result = null,
                        IsSuccess = false,
                        ResponseMessage = ex.Message,
                        StatusCode = 500,
                        StatusMessage = "Failed"
                    };
                }
            }
        }

        public async  Task<ResponseEntity<string>> Authentication(SigninRequestModel signinRequestModel)
        {
            try
            {
                var authenticationResponse = await _context.Contact.Where(x => x.Email == signinRequestModel.EmailId && x.Password == signinRequestModel.Password && x.IsActive == true).FirstOrDefaultAsync();
                if (authenticationResponse != null)
                {
                    var authClaims = new List<Claim>
                        {
                        new Claim("UserRefID", authenticationResponse.ContactID.ToString()),
                        new Claim("FirstName", authenticationResponse.FirstName),
                        new Claim("LastName", authenticationResponse.LastName),
                        new Claim("guid", Guid.NewGuid().ToString()),
                        new Claim("date", DateTime.UtcNow.ToString()),
                     };
                    var token = GenerateToken(authClaims);
                    var authToken = new JwtSecurityTokenHandler().WriteToken(token);
                    var refreshToken = GenerateRefreshToken();
                    return new ResponseEntity<string>
                    {
                        Result = "Successfully Logined",
                        Token = authToken.ToString(),
                        RefreshToken = refreshToken,
                        IsSuccess = true,
                        ResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted).RequestMessage?.ToString(),
                        StatusMessage = HttpStatusCode.OK.ToString()
                    };
                }
                else
                {
                    return new ResponseEntity<string>
                    {
                        IsSuccess = false,
                        ResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted).RequestMessage?.ToString(),
                        StatusMessage = HttpStatusCode.OK.ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResponseEntity<string>()
                {
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusMessage = HttpStatusCode.InternalServerError.ToString()
                };
            }
        }

        public async Task<ResponseEntity<bool>> DeleteContact(int ContactID)
        {
            try
            {
                var employeeData = await _context.Contact.Where(l => l.ContactID == ContactID).FirstOrDefaultAsync();
                if (employeeData != null)
                {
                    var patchDta = new JsonPatchDocument<ContactEntity>()
                       .Replace(p => p.IsActive, false)
                       .Replace(p => p.UpdatedDate, DateTime.Now);
                    patchDta.ApplyTo(employeeData);
                    await _context.SaveChangesAsync(default);
                    return new ResponseEntity<bool>()
                    {
                        Result = true,
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                return new ResponseEntity<bool>()
                {
                    Result = false,
                    IsSuccess = false,
                    ResponseMessage = "Data not found !",
                    StatusCode = 400,
                    StatusMessage = "Error"
                };

            }
            catch (Exception ex)
            {

                return new ResponseEntity<bool>()
                {
                    Result = false,
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusCode = 500,
                    StatusMessage = "Failed"
                };
            }
        }

        public async Task<ResponseEntity<ContactViewEntity>> GetByContactID(int ContactID)
        {
            try
            {
                var userByID = await _context.Contact.Where(p => p.ContactID == ContactID && p.IsActive == true).Select(o => new ContactViewEntity()
                {
                    ContactID = o.ContactID,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Address = o.Address,
                    Email = o.Email,
                    City = o.City,
                    Password = o.Password,
                    Country = o.Country,
                    State = o.State,
                    PostalCode = o.PostalCode,
                    PhoneNumber = o.PhoneNumber,
                }).FirstOrDefaultAsync();
                if (userByID != null)
                {
                    return new ResponseEntity<ContactViewEntity>
                    {
                        Result = userByID,
                        IsSuccess = true,
                        ResponseMessage = "Successfully",
                        StatusCode = 200,
                        StatusMessage = "Success"
                    };
                }
                return new ResponseEntity<ContactViewEntity>
                {
                    Result = null,
                    IsSuccess = false,
                    ResponseMessage = "data not found",
                    StatusCode = 400,
                    StatusMessage = "error"
                };
            }
            catch (Exception ex)
            {
                return new ResponseEntity<ContactViewEntity>()
                {
                    Result = null,
                    IsSuccess = false,
                    ResponseMessage = ex.Message,
                    StatusMessage = HttpStatusCode.InternalServerError.ToString()
                };
            }
        }

        public async Task<ResponseEntity<List<ContactEntity>>> GetContacts(string? search, string? sortColumn, bool isAcs, int page, int pageSize)
        {
            var query = _context.Contact.Where(x=> x.IsActive == true).AsQueryable();

            // Searching
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FirstName.Contains(search) || c.LastName.Contains(search) || c.Email.Contains(search));
            }

            // Calculate total records before pagination
            var totalRecords = await query.CountAsync();

            // Sorting
            switch (sortColumn?.ToLower())
            {
                case "firstname":
                    query = isAcs ? query.OrderBy(c => c.FirstName) : query.OrderByDescending(c => c.FirstName);
                    break;
                case "lastname":
                    query = isAcs ? query.OrderBy(c => c.LastName) : query.OrderByDescending(c => c.LastName);
                    break;
                case "email":
                    query = isAcs ? query.OrderBy(c => c.Email) : query.OrderByDescending(c => c.Email);
                    break;
                default:
                    query = query.OrderByDescending(c => c.ContactID);
                    break;
            }

            // Pagination
            //query = query.Skip((page - 1) * pageSize).Take(pageSize);

            // Calculate total pages
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            return new ResponseEntity<List<ContactEntity>>()
            {
                Result = await query.ToListAsync(),
                IsSuccess = true,
                StatusCode = 200,
                StatusMessage = "OK",
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        #region Helper
        public JwtSecurityToken GenerateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["Jwt:AccessTokenExpiryinMinutes"])),
                signingCredentials: signIn
            );
            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        #endregion
    }
}
