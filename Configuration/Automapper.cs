using AutoMapper;
using MACUTION.Data;
using MACUTION.Model.Dto;
using Name;

namespace MACUTION.Configuration
{
    public class Automapper : Profile
    {
        Tokenget token;
       public Automapper()
        {
            this.token=new Tokenget();
            CreateMap<User,loginResponceDto>().ForMember(x=>x.token,opt=>opt.MapFrom(x=>token.getToken(x.Name,x.role,x.Id.ToString())))
            .ForMember(x=>x.email,opt=>opt.MapFrom(x=>x.Email)).ForMember(x=>x.name,opt=>opt.MapFrom(x=>x.Name))
            .ForMember(x=>x.imageurl,opt=>opt.MapFrom(y=>y.ProfileImageUrl))
            .ForMember(x=>x.id,opt=>opt.MapFrom(y=>y.Id)).ForMember(x=>x.request_accepted,opt=>opt.MapFrom(x=>x.request == null ? false : x.request.verified_admin)).
            ForMember(x=>x.addUserAccess,opt=>opt.MapFrom(x=>x.right_to_add));
        }
    }
}