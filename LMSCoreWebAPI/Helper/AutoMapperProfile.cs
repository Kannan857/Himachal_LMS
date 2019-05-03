using AutoMapper;
using LMSCoreWebAPI.DTO;
using LMSCoreWebAPI.lms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMSCoreWebAPI.Helper
{
    public class AutoMapperProfile :Profile
    { 
         public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
