﻿namespace AppView.Areas.Admin
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
