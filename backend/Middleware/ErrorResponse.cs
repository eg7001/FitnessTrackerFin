namespace FitnessTracker.Middleware
{
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? TraceId { get; set; }
    }
}
