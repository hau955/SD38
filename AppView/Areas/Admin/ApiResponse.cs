namespace AppView.Areas.Admin
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public Dictionary<string, string[]> Errors { get; set; }
        public int StatusCode { get; set; }
        public static ApiResponse<T> Success(T data, string message = "Thành công", int statusCode = 200)
       => new ApiResponse<T> { IsSuccess = true, Data = data, Message = message, StatusCode = statusCode };

        public static ApiResponse<T> Fail(string message, int statusCode = 400)
            => new ApiResponse<T> { IsSuccess = false, Message = message, StatusCode = statusCode };
    }
}
