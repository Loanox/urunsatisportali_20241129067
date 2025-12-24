using Microsoft.AspNetCore.SignalR;

namespace urunsatisportali.Hubs
{
    public class GeneralHub : Hub
    {
        // İhtiyaç duyulursa client'ların tetikleyebileceği metodlar buraya eklenebilir.
        // Şimdilik sadece server-to-client bildirimleri yapacağız (SaleService üzerinden).
    }
}
