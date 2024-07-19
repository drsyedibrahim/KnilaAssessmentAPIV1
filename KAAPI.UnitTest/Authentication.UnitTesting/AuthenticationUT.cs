using KAAPI.BL.CustomService;
using KAAPI.BL.ICustomService;
using KAAPI.DataObject.ViewEntity;
using KAAPI.WebAPI.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KAAPI.UnitTest.Authentication.UnitTesting
{
    [TestClass]
    public class AuthenticationUT
    {
        private Mock<IunitofService> _mockUnitOfService;
        private Mock<IAuthenticationBL> _mockAuthenticationBL;
        private AuthController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfService = new Mock<IunitofService>();
            _mockAuthenticationBL = new Mock<IAuthenticationBL>();
            _mockUnitOfService.Setup(u => u.AuthenticationBL).Returns(_mockAuthenticationBL.Object);
            _controller = new AuthController(_mockUnitOfService.Object);
        }
        [TestMethod]
        public async Task GetByContactIDUT()
        {
            int testContactID = 1;
            var mockResult = new ContactViewEntity
            {
                ContactID = 1,
                FirstName = "Syed",
                LastName = "Ibrahim",
                Email = "syed@gmail.com",
                Address = "Ring Road",
                City = "Madurai",
                State = "TamilNadu",
                Country = "India",
                PhoneNumber = "7418529630",
                PostalCode = "625003",
                Password = "Raj@2306",
            };
            _mockAuthenticationBL.Setup(x => x.GetByContactID(1))
               .ReturnsAsync(new ResponseEntity<ContactViewEntity>
               {
                   Result = mockResult,
                   IsSuccess = true,
                   ResponseMessage = new HttpResponseMessage(HttpStatusCode.Accepted).RequestMessage?.ToString(),
                   StatusMessage = HttpStatusCode.OK.ToString(),
                   StatusCode = StatusCodes.Status200OK,
               });
            var result = await _controller.GetByContactID(testContactID) as JsonResult;
            Assert.IsNotNull(result);

            if (result is JsonResult jsonResult)
            {
                Assert.IsInstanceOfType(result, typeof(JsonResult));
                var httpContext = new DefaultHttpContext();
                httpContext.Response.Body = new MemoryStream();
                _controller.ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,
                };
                var httpStatusCode = _controller.HttpContext.Response.StatusCode;
                Assert.IsTrue(httpStatusCode >= 200 && httpStatusCode < 300);
            }
            else
            {
                Assert.Fail("Unexpected return type");
            }
        }

        [TestMethod]
        public async Task RegisterContactUT()
        {
            var mockContact = new ContactViewEntity
            {
                ContactID = 1,
                FirstName = "Syed",
                LastName = "Ibrahim",
                Email = "syed@gmail.com",
                Address = "Ring Road",
                City = "Madurai",
                State = "TamilNadu",
                Country = "India",
                PhoneNumber = "7418529630",
                PostalCode = "625003",
                Password = "Raj@2306",
            };
            string? response = string.Empty;
            if (mockContact.ContactID != 0)
                response = "Contact Updated Successfully!";
            else
                response = "Contact Added Successfully!";
            var mockResponse = new ResponseEntity<string>
            {
                Result = response,
                IsSuccess = true,
                StatusCode = StatusCodes.Status200OK,
                StatusMessage = "Success",
            };

            _mockAuthenticationBL.Setup(x => x.Registration(It.IsAny<ContactViewEntity>()))
                .ReturnsAsync(mockResponse);

            // Act
            var result = await _controller.RegisterContact(mockContact) as JsonResult;

            // Assert
            Assert.IsNotNull(result);

            if (result is JsonResult jsonResult)
            {
                Assert.IsInstanceOfType(result, typeof(JsonResult));

                var json = JsonConvert.SerializeObject(jsonResult.Value);
                var resultData = JsonConvert.DeserializeObject<RegisterContactResponse>(json);

                Assert.IsNotNull(resultData);
                Assert.AreEqual(response, resultData.resultData);
                Assert.AreEqual(mockResponse.IsSuccess, resultData.IsSuccess);
                Assert.AreEqual(mockResponse.StatusCode, resultData.StatusCode);
                Assert.AreEqual(mockResponse.StatusMessage, resultData.StatusMessage);
            }
            else
            {
                Assert.Fail("Unexpected return type");
            }
        }

        public class RegisterContactResponse
        {
            public string resultData { get; set; }
            public bool IsSuccess { get; set; }
            public int StatusCode { get; set; }
            public string StatusMessage { get; set; }
        }

    }
}
