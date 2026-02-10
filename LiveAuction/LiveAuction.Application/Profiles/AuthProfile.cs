using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using LiveAuction.Application.Dtos.AuthModel;
using LiveAuction.Core.Entites;

namespace LiveAuction.Application.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {

            CreateMap<RegisterModel, ApplicationUser>();
            CreateMap<AuthModel, ApplicationUser>();
            CreateMap<ApplicationUser, AuthModel>();
        }
    }
}
