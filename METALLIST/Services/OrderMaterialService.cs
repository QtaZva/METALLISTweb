using METALLIST.Models;
using Microsoft.EntityFrameworkCore;

namespace METALLIST.Services
{
    public class OrderMaterialService
    {
        private readonly ApplicationDbContext _db;

        public OrderMaterialService(ApplicationDbContext context)
        {
            _db = context;
        }

        // Получить список материалов для заказа
        public async Task<List<OrderMaterial>> GetMaterialsForOrderAsync(int orderId)
        {
            return await _db.OrderMaterials
                .Include(om => om.Material)
                .Where(om => om.OrderId == orderId)
                .ToListAsync();
        }

        // Добавить материал к заказу
        public async Task AddMaterialToOrderAsync(int orderId, int materialId, int quantity)
        {
            var orderMaterial = new OrderMaterial
            {
                OrderId = orderId,
                MaterialId = materialId,
                Quantity = quantity
            };

            _db.OrderMaterials.Add(orderMaterial);
            await _db.SaveChangesAsync();
        }

        // Удалить материал из заказа
        public async Task RemoveMaterialFromOrderAsync(int orderId, int materialId)
        {
            var orderMaterial = await _db.OrderMaterials
                .FirstOrDefaultAsync(om => om.OrderId == orderId && om.MaterialId == materialId);

            if (orderMaterial != null)
            {
                _db.OrderMaterials.Remove(orderMaterial);
                await _db.SaveChangesAsync();
            }
        }
    }
}
