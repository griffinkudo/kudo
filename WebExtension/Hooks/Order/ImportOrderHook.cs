using DirectScale.Disco.Extension.Hooks.Orders;
using DirectScale.Disco.Extension.Hooks;
using DirectScale.Disco.Extension.Services;
using System.Threading.Tasks;
using System;
using WebExtension.Services;

namespace WebExtension.Hooks.Order
{
    public class ImportOrderHook : IHook<ImportOrderHookRequest, ImportOrderHookResponse>
    {
        private readonly IStatsService _statsService;
        private readonly IOrderService _orderService;
        private readonly ICustomLogService _customLogService;
        public ImportOrderHook(IStatsService statsService, IOrderService orderService, ICustomLogService customLogService)
        {
            _statsService = statsService;
            _orderService = orderService;
            _customLogService = customLogService;
        }

        public async Task<ImportOrderHookResponse> Invoke(ImportOrderHookRequest request, Func<ImportOrderHookRequest, Task<ImportOrderHookResponse>> func)
        {
            var result = await func(request);

            try
            {
                var orderDetail = await _orderService.GetOrderByOrderNumber(result.OrderNumber);
                await _statsService.RecalculateVolume(new DirectScale.Disco.Extension.VolumeSource
                {
                    AssociateId = orderDetail.AssociateId,
                    SourceId = orderDetail.OrderNumber,
                    SourceDate = orderDetail.OrderDate,
                    ProcessTime = System.DateTime.UtcNow,
                });
            }
            catch (Exception ex)
            {
                await _customLogService.SaveLog(request.AssociateId, 0, "Error", "ImportOrder-RecalculateVolume", ex.Message, "", "", "", "");
            }

            return result;
        }
    }
}
