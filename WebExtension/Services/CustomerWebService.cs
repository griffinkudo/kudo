using DirectScale.Disco.Extension.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebExtension.Models.Order;
using WebExtension.Repositories;
using DirectScale.Disco.Extension;

namespace WebExtension.Services
{
    public interface ICustomerWebService
    {
        Task<User> UpdateBasetypeAndCreateNewUser(UpdateBasetypeAndCreateNewUserRequest request);
    }

    public class CustomerWebService : ICustomerWebService
    {
        private readonly ICustomerWebRepository _customerWebRepository;

        public CustomerWebService(ICustomerWebRepository customerWebRepository)
        {
            _customerWebRepository = customerWebRepository ?? throw new ArgumentNullException(nameof(customerWebRepository));
        }
        public async Task<User> UpdateBasetypeAndCreateNewUser(UpdateBasetypeAndCreateNewUserRequest request)
        {
            return await _customerWebRepository.UpdateBasetypeAndCreateNewUser(request);
        }
    }
}
