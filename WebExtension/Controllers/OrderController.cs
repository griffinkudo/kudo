using DirectScale.Disco.Extension;
using DirectScale.Disco.Extension.Middleware;
using DirectScale.Disco.Extension.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebExtension.Helper;
using WebExtension.Helper.Interface;
using WebExtension.Helper.Models;
using WebExtension.Models;
using WebExtension.Models.Order;
using WebExtension.Services;

namespace WebExtension.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        [ViewData]
        public string DSBaseUrl { get; set; }
        //
        private readonly IOrderWebService _orderWebService;
        public OrderController(
            IOrderWebService orderWebService
        )
        {
            _orderWebService = orderWebService ?? throw new ArgumentNullException(nameof(orderWebService));
        }

        [HttpPost]
        [Route("GetCustomOrderReport")]
        public IActionResult GetCustomOrderReport([FromBody] GetCustomOrderReportRequest request)
        {
            try
            {
                CustomOrderReportResponse model = new CustomOrderReportResponse();

                model.search = request?.search ?? "";
                CultureInfo provider = CultureInfo.InvariantCulture;
                model.begin = !string.IsNullOrEmpty(request?.begin) ? DateTime.ParseExact(request?.begin, DateTimeFormat.GetDateTimeFormat, provider, DateTimeStyles.None) : System.DateTime.Now.AddMonths(-1);
                model.end = !string.IsNullOrEmpty(request?.end) ? DateTime.ParseExact(request?.end, DateTimeFormat.GetDateTimeFormat, provider, DateTimeStyles.None) : System.DateTime.Now;
                model.orders = _orderWebService.GetFilteredOrders(model.search, model.begin, model.end).Result;
                return new Responses().OkResult(model);
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }

        [HttpPost]
        [Route("GetOrderDetailbyExtOrderNumber")]
        public IActionResult GetOrderDetailbyExtOrderNumber([FromBody] GetOrderDetailbyExtOrderNumberRequest request)
        {
            try
            {
                return new Responses().OkResult(_orderWebService.GetOrderDetailbyExtOrderNumber(request).GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                return new Responses().BadRequestResult(ex.Message);
            }
        }
    }
}
