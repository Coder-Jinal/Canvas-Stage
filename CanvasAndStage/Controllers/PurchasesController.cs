using CanvasAndStage.Interfaces;
using CanvasAndStage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CanvasAndStage.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PurchasesController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;

        public PurchasesController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<IEnumerable<PurchaseDto>>> ListPurchases()
        {
            var purchaseDtos = await _purchaseService.ListPurchases();
            return Ok(purchaseDtos);
        }

        [HttpGet("Find/{id}")]
        public async Task<ActionResult<PurchaseDto>> FindPurchase(int id)
        {
            var purchase = await _purchaseService.FindPurchase(id);

            if (purchase == null)
            {
                return NotFound($"Purchase with ID {id} doesn't exist.");
            }

            return Ok(purchase);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddPurchase(AddPurchaseDto dto)
        {
            ServiceResponse response = await _purchaseService.AddPurchase(dto);

            if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while adding the purchase." });
            }

            return CreatedAtAction("FindPurchase", new { id = response.CreatedId }, new
            {
                message = $"Purchase added successfully with ID {response.CreatedId}",
                purchaseId = response.CreatedId
            });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdatePurchase(int id, UpdatePurchaseDto dto)
        {
            if (id != dto.PurchaseId)
            {
                return BadRequest(new { message = "Purchase ID mismatch." });
            }

            ServiceResponse response = await _purchaseService.UpdatePurchase(id, dto);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Purchase not found." });
            }
            else if (response.Status == ServiceResponse.ServiceStatus.Error)
            {
                return StatusCode(500, new { error = "An error occurred while updating the purchase." });
            }

            return Ok(new { message = $"Purchase with ID {id} updated successfully." });
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            ServiceResponse response = await _purchaseService.DeletePurchase(id);

            if (response.Status == ServiceResponse.ServiceStatus.NotFound)
            {
                return NotFound(new { error = "Purchase not found." });
            }

            return Ok(new { message = $"Purchase with ID {id} deleted successfully." });
        }
    }
}
