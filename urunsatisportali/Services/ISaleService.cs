using System.Collections.Generic;
using urunsatisportali.Models;

namespace urunsatisportali.Services
{
    public interface ISaleService
    {
        ResultModel<Sale> CreateSale(Sale sale, List<int> productIds, List<int> quantities);
        ResultModel<List<Sale>> GetAllSales();
        ResultModel<Sale> GetSaleById(int id);
    }
}
