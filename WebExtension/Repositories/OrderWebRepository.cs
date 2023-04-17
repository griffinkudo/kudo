using Dapper;
using DirectScale.Disco.Extension.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebExtension.Models.Order;

namespace WebExtension.Repositories
{
    public interface IOrderWebRepository
    {
        List<int> GetFilteredOrderIds(string search, DateTime beginDate, DateTime endDate);
        Task<GetOrderDetailbyExtOrderNumberResponse> GetOrderDetailbyExtOrderNumber(GetOrderDetailbyExtOrderNumberRequest request);
    }
    public class OrderWebRepository : IOrderWebRepository
    {
        private readonly IDataService _dataService;

        public OrderWebRepository(IDataService dataService)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        }
        public List<int> GetFilteredOrderIds(string search, DateTime beginDate, DateTime endDate)
        {
            using (var dbConnection = new SqlConnection(_dataService.GetClientConnectionString().Result))
            {
                var parameters = new
                {
                    beginDate,
                    endDate
                };

                var queryStatement = $@"
                    SELECT DISTINCT
                            o.RecordNumber 
                    FROM ORD_Order o
                    JOIN CRM_Distributors D 
                        ON o.DistributorID = D.RecordNumber 
                    WHERE o.Void = 0
                        AND CAST(o.OrderDate AS DATE) >= @beginDate 
                        AND CAST(o.OrderDate AS DATE) <= @endDate
                    {BuildOrderFilterClause(search)}
                    ORDER BY o.RecordNumber DESC
                ";

                return dbConnection.Query<int>(queryStatement, parameters).ToList();
            }
        }
        private string BuildOrderFilterClause(string search)
        {
            var sql = string.Empty;

            if (!string.IsNullOrWhiteSpace(search))
            {
                sql += string.Format(@" AND (
                        o.Name LIKE '%{0}%' OR 
                        o.Email LIKE '%{0}%' OR 
                        o.RecordNumber LIKE {0} OR 
                        o.SpecialInstructions LIKE '%{0}%' OR 
                        p.Reference LIKE '%{0}%' OR 
                        d.BackofficeID = '{0}')", search);
            }

            return sql;
        }
        public async Task<GetOrderDetailbyExtOrderNumberResponse> GetOrderDetailbyExtOrderNumber(GetOrderDetailbyExtOrderNumberRequest request)
        {
            using (var dbConnection = new SqlConnection(await _dataService.GetClientConnectionString()))
            {
                var parameters = new
                {
                    request.ExtOrderNumber
                };

                var queryStatement = $@"
                    SELECT
	                    o.[recordnumber] AS OrderNumber, p.recordnumber AS PaymentId
                    FROM 
	                    [dbo].[ORD_Order] o JOIN [dbo].[ORD_Payments] p ON o.recordnumber=p.OrderNumber
                    WHERE 
	                    [ExtOrderNumber]=@ExtOrderNumber
                ";
                var result = await dbConnection.QueryAsync<GetOrderDetailbyExtOrderNumberResponse>(queryStatement, parameters);
                return result.FirstOrDefault();
            }
        }
    }
}
