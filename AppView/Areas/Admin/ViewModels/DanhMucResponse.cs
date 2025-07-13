namespace AppView.Areas.Admin.ViewModels
{
    public class DanhMucResponse
    {
        public Guid DanhMucId { get; set; }
        public string TenDanhMuc { get; set; } = null!;
    }
    public class ApiDanhMucResponse
    {
        public string Message { get; set; }
        public List<DanhMucResponse> Data { get; set; }
    }


}
