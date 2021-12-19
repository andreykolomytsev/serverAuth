using AutoMapper;
using Application.DTOs.Auth;
using Application.DTOs.Permissions;
using Application.DTOs.Roles;
using Application.DTOs.Tenants;
using Application.DTOs.Users;
using Application.DTOs.Tokens;
using Application.DTOs.MicroService;
using Infrastructure.Identity.Models;

namespace Application.Mappings
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<ModelUser, ResponseAuthentication>();
            CreateMap<ModelRefreshToken, ResponseRefreshToken>();
            CreateMap<ModelAccessToken, ResponseAccessToken>();

            CreateMap<ResponseRole, ModelRole>().ReverseMap();

            CreateMap<ResponsePermission, ModelPermission>()
                .ForMember(nameof(ModelPermission.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(ModelPermission.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();

            CreateMap<RequestPermission, ModelPermission>()
                .ForMember(nameof(ModelPermission.ClaimType), opt => opt.MapFrom(c => c.Type))
                .ForMember(nameof(ModelPermission.ClaimValue), opt => opt.MapFrom(c => c.Value))
                .ReverseMap();

            CreateMap<ResponseUser, ModelUser>().ReverseMap();

            CreateMap<ModelTenant, ResponseTenant>();
            CreateMap<RequestTenant, ModelTenant>();

            CreateMap<ModelService, ResponseMS>();
            CreateMap<RequestMS, ModelService>();
        }
    }
}
