using DirectScale.Disco.Extension.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using WebExtension.Models.Order;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using static WebExtension.Repositories.CustomerWebRepository;
using DirectScale.Disco.Extension;
using WebExtension.Models.Customer;
using WebExtension.Services;
using WebExtension.Models.Enums;
using Newtonsoft.Json;

namespace WebExtension.Repositories
{
    public interface ICustomerWebRepository
    {
        Task<User> UpdateBasetypeAndCreateNewUser(UpdateBasetypeAndCreateNewUserRequest request);
    }
    public class CustomerWebRepository : ICustomerWebRepository
    {
        private readonly IDataService _dataService;
        private readonly IUserService _userService;
        private readonly IAssociateService _associateService;
        private readonly ICustomLogService _customLogService;
        private readonly IWebsiteService _websiteService;

        public CustomerWebRepository(IDataService dataService, IUserService userService, IAssociateService associateService, ICustomLogService customLogService, IWebsiteService websiteService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _associateService = associateService ?? throw new ArgumentNullException(nameof(associateService));
            _customLogService = customLogService ?? throw new ArgumentNullException(nameof(customLogService));
            _websiteService = websiteService ?? throw new ArgumentNullException(nameof(websiteService));
        }
        public async Task<User> UpdateBasetypeAndCreateNewUser(UpdateBasetypeAndCreateNewUserRequest request)
        {
            User response = null;
            Associate associate = await _associateService.GetAssociate(Convert.ToInt32(request.associateID));
            try
            {
                if(associate != null)
                {

                    Associate updateAssociateRequest = associate;
                    updateAssociateRequest.AssociateType = associate.AssociateType;
                    updateAssociateRequest.AssociateBaseType = request.associateBaseType;


                    //Update Associate Base Type
                    await _associateService.UpdateAssociate(updateAssociateRequest);
                    Associate newAssociate = await _associateService.GetAssociate(Convert.ToInt32(request.associateID));

                    //Create a new Website ( webalias )
                    await _websiteService.CreateWebsite(newAssociate.AssociateId, request.username, "", 0, 1);

                    // Create a New User
                    var RegisterUserRequest = new RegisterUserRequest() {
                        displayName = newAssociate?.DisplayFirstName,
                        username = request.username,
                        password = request.password,
                        email = newAssociate?.EmailAddress,
                        role = Enum.GetName(typeof(AssociateRole), newAssociate.AssociateType),
                        Id = request.associateID,
                        corporateUser = false
                    };
                    response = await RegisterUser(RegisterUserRequest);
                    
                }
            }
            catch (Exception ex)
            {
                await _customLogService.SaveLog(associate.AssociateId, 0, "", "", "", "", "", "", ex.Message);
                throw;
            }
            return response;

        }

        private async Task<User> RegisterUser(RegisterUserRequest request)
        {
            dynamic resp = null;

            try
            {
                resp = await _userService.RegisterUser(request.displayName, request.username, request.password, request.email, request.role, request.Id, request.corporateUser);
                await _customLogService.SaveLog(Convert.ToInt32(request.Id), 0, "", "", "", "", "", "", JsonConvert.SerializeObject(resp));


            }
            catch (Exception ex)
            {
                await _customLogService.SaveLog(Convert.ToInt32(request.Id), 0, "", "", "", "", "", "", ex.Message);
                throw;
            }
            return resp;
        }
    }
}
