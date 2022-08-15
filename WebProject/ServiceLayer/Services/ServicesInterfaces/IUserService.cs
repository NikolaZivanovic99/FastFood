using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.ServicesInterfaces
{
   public interface IUserService
    {
        UserRegistrationDto Register(UserRegistrationDto newUser);
        TokenDto Login(UserLoginDto user);
        UserDto GetUser(int id);
        List<UserDto> GetDelivery();
        UserDto UpdateUser(UserUpdateDto user);
        UserDto Verification(VerificationDto verification);
    }
}
