using AppApi.Features.OrderManagerment.DTOs;
using AppView.Areas.OrderManagerment.ViewModels;
using AutoMapper;

namespace AppView.Helper
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            // Map từ OrderDetailDto sang OrderDetailViewModel
            CreateMap<OrderDetailDto, OrderDetailViewModel>();

            // Map từ OrderItemDto sang OrderItemViewModel
            CreateMap<OrderItemDto, OrderItemViewModel>();

            // Các mapping khác nếu cần
            CreateMap<OrderListDto, OrderListViewModel>();
        }
    }
}
