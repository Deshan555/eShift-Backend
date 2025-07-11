using System;

namespace eShift.Models
{
    public class ApiResponse<T>
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Message { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public int Status { get; set; }
        public T? Data { get; set; } // Make Data nullable
    }
}
