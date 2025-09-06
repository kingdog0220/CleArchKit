using AutoMapper;
using BlazorWasmTemplate.Application.Users.Dtos;
using BlazorWasmTemplate.Domain.Users.Entities;

namespace BlazorWasmTemplate.Api.Users
{
    /// <summary>
    /// Mapper profile
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
        }

    }
}