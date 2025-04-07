using CanvasAndStage.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Interfaces
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseDto>> ListPurchases();
        Task<PurchaseDto?> FindPurchase(int id);
        Task<ServiceResponse> AddPurchase(AddPurchaseDto dto);
        Task<ServiceResponse> UpdatePurchase(int id, UpdatePurchaseDto dto);
        Task<ServiceResponse> DeletePurchase(int id);
    }
}
