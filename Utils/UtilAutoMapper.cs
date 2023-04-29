using AutoMapper;
using demo_test_api.Models;

namespace demo_test_api.Utils
{
    public class UtilAutoMapper : Profile
    {
        public UtilAutoMapper() { 
            CreateMap<Authentication, AuthenticationDTO>()
                .ForMember(dest => dest.username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.password));
            CreateMap<AuthenticationDTO, Authentication>()
                .ForMember(dest => dest.uuid, opt => opt.MapFrom(src => new Guid()))
                .ForMember(dest => dest.username, opt => opt.MapFrom(src => src.username))
                .ForMember(dest => dest.password, opt => opt.MapFrom(src => src.password));

        }
    }
}
