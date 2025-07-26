namespace AppView.Areas.Admin.Common
{
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; } 
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
    
}
