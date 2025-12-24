using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using urunsatisportali.Data;
using urunsatisportali.Hubs;
using urunsatisportali.Models;

namespace urunsatisportali.Services
{
    public class SaleService(ApplicationDbContext context, IHubContext<GeneralHub> hubContext) : ISaleService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHubContext<GeneralHub> _hubContext = hubContext;

        public ResultModel<Sale> CreateSale(Sale sale, List<int> productIds, List<int> quantities)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // 1. Temel Validasyonlar
                if (productIds == null || quantities == null || productIds.Count == 0 || productIds.Count != quantities.Count)
                {
                    return new ResultModel<Sale>
                    {
                        IsSuccess = false,
                        Message = "Ürün listesi geçersiz.",
                        StatusCode = 400
                    };
                }

                // 2. Müşteri Kontrolü (Varsa)
                if (sale.CustomerId.HasValue && sale.CustomerId > 0)
                {
                    var customer = _context.Customers.Find(sale.CustomerId.Value);
                    if (customer == null || customer.IsDeleted)
                    {
                        return new ResultModel<Sale>
                        {
                            IsSuccess = false,
                            Message = "Seçilen müşteri bulunamadı.",
                            StatusCode = 404
                        };
                    }
                }
                else
                {
                    sale.CustomerId = null;
                }

                decimal subtotal = 0m;
                sale.SaleItems = []; // Collection expression

                // 3. Ürünlerin Kontrolü ve İşlenmesi
                for (int i = 0; i < productIds.Count; i++)
                {
                    var productId = productIds[i];
                    var qty = quantities[i];

                    if (qty <= 0) continue; // 0 veya negatif adetleri atla

                    var product = _context.Products.Find(productId);
                    if (product == null || product.IsDeleted || !product.IsActive)
                    {
                        transaction.Rollback();
                        return new ResultModel<Sale>
                        {
                            IsSuccess = false,
                            Message = $"Ürün bulunamadı veya aktif değil (ID: {productId}).",
                            StatusCode = 404
                        };
                    }

                    // Stok Kontrolü (Deduct StockQuantity)
                    if (product.StockQuantity < qty)
                    {
                        transaction.Rollback();
                        return new ResultModel<Sale>
                        {
                            IsSuccess = false,
                            Message = $"{product.Name} için yeterli stok yok. (Mevcut: {product.StockQuantity}, İstenen: {qty})",
                            StatusCode = 400
                        };
                    }

                    // Stok Düşme
                    product.StockQuantity -= qty;
                    _context.Products.Update(product);

                    // SaleItem Oluşturma (Transfer CartItems to SaleItems)
                    var lineTotal = product.Price * qty;
                    subtotal += lineTotal;

                    sale.SaleItems.Add(new SaleItem
                    {
                        ProductId = product.Id,
                        Quantity = qty,
                        UnitPrice = product.Price,
                        TotalPrice = lineTotal,
                        CreatedAt = DateTime.Now
                    });
                }

                if (sale.SaleItems.Count == 0)
                {
                    transaction.Rollback();
                    return new ResultModel<Sale>
                    {
                        IsSuccess = false,
                        Message = "Satış için geçerli ürün bulunamadı.",
                        StatusCode = 400
                    };
                }

                // 4. Tutar Hesaplamaları
                var taxAmount = subtotal * (sale.Tax / 100m);
                var discountAmount = subtotal * (sale.Discount / 100m);

                sale.TotalAmount = subtotal;
                sale.FinalAmount = subtotal + taxAmount - discountAmount;

                // Otomatik atamalar
                sale.SaleNumber = $"SALE-{DateTime.Now:yyyyMMddHHmmss}";
                sale.SaleDate = DateTime.Now;
                sale.CreatedAt = DateTime.Now;
                sale.Status = "Completed"; // "Clear Cart after successful checkout" mantığı burada işlem tamamlandı olarak işaretleniyor.

                // 5. Kayıt
                _context.Sales.Add(sale);
                _context.SaveChanges();

                // 6. İşlemi Onayla
                transaction.Commit();

                // SignalR Bildirimi (Fire and forget)
                _ = _hubContext.Clients.All.SendAsync("ReceiveSaleUpdate", new
                {
                    SaleId = sale.Id,
                    TotalAmount = sale.FinalAmount,
                    Message = $"Yeni satış: {sale.SaleNumber} ({sale.FinalAmount:C2})"
                });

                return new ResultModel<Sale>
                {
                    IsSuccess = true,
                    Data = sale,
                    Message = "Satış başarıyla oluşturuldu.",
                    StatusCode = 201
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new ResultModel<Sale>
                {
                    IsSuccess = false,
                    Message = "Satış işlemi sırasında hata: " + ex.Message,
                    StatusCode = 500
                };
            }
        }

        public ResultModel<List<Sale>> GetAllSales()
        {
            try
            {
                var sales = _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.User)
                    .Where(s => !s.IsDeleted)
                    .OrderByDescending(s => s.SaleDate)
                    .ToList();

                return new ResultModel<List<Sale>>
                {
                    IsSuccess = true,
                    Data = sales,
                    StatusCode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<List<Sale>>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }

        public ResultModel<Sale> GetSaleById(int id)
        {
            try
            {
                var sale = _context.Sales
                    .Include(s => s.Customer)
                    .Include(s => s.User)
                    .Include(s => s.SaleItems)
                    .ThenInclude(si => si.Product)
                    .FirstOrDefault(s => s.Id == id && !s.IsDeleted);

                if (sale == null)
                {
                    return new ResultModel<Sale>
                    {
                        IsSuccess = false,
                        Message = "Not Found",
                        StatusCode = 404
                    };
                }

                return new ResultModel<Sale>
                {
                    IsSuccess = true,
                    Data = sale,
                    StatusCode = 200,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                return new ResultModel<Sale>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }
    }
}
