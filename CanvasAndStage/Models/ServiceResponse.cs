using CanvasAndStage.Services;

namespace CanvasAndStage.Models
{
    public class ServiceResponse
    {
        public enum ServiceStatus { Success, Error, NotFound, Created, Updated, Deleted }

        public ServiceStatus Status { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public int? CreatedId { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
