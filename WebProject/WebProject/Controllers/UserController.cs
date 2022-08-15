using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.Services.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register([FromForm] UserRegistrationDto newUser) 
        {
            try
            {
                var createUser = _userService.Register(newUser);
                return Created("api/user/register", createUser);
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto user) 
        {
            try
            {
                return Ok(_userService.Login(user));
            }
            catch (Exception e) 
            {
                return Unauthorized(e.Message);
            }
        }
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetUser(int id) 
        {
            try
            {
                return Ok(_userService.GetUser(id));
            }
            catch (Exception e) 
            {
               return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("delivery")]
       // [Authorize]
        public IActionResult GetDelivery() 
        {
            try
            {
                return Ok(_userService.GetDelivery());
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("update")]
        //[Authorize]
        public IActionResult UserUpdate([FromForm] UserUpdateDto user) 
        {
            try
            {
                return Ok(_userService.UpdateUser(user));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("verification")]
        //[Authorize]
        public IActionResult Verification(VerificationDto verification) 
        {
            try
            {
                return Ok(_userService.Verification(verification));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

    }
}
