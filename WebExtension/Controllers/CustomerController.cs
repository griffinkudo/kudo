using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebExtension.Helper;
using WebExtension.Models.Order;
using WebExtension.Repositories;
using WebExtension.Services;

namespace WebExtension.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : Controller
    {

        private readonly ICustomerWebService _customerWebService;
        public CustomerController(ICustomerWebService customerWebService)
        {
            _customerWebService = customerWebService ?? throw new ArgumentNullException(nameof(customerWebService));
        }

        [HttpPost]
        [Route("UpdateBasetypeAndCreateNewUser")]
        public async Task<IActionResult> UpdateBasetypeAndCreateNewUser([FromBody] UpdateBasetypeAndCreateNewUserRequest request)
        {
            try
            {
                return new Responses().OkResult(await _customerWebService.UpdateBasetypeAndCreateNewUser(request));
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
