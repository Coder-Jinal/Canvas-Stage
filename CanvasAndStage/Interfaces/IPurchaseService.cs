using CanvasAndStage.Models;
using CanvasAndStage.Models.ViewModels;
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
        Task<PaginatedResult<PurchaseDto>> GetPaginatedPurchases(int page, int pageSize);
    }
}
