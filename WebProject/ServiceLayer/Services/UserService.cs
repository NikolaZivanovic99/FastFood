using AutoMapper;
using DataLayer;
using DataLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.DTOs;
using ServiceLayer.Services.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        private readonly IConfigurationSection _configurationSection;
        private readonly IEmailSender _emailSender;

        public UserService(DataContext dataContext,IMapper mapper,IConfiguration configurationSection,IEmailSender emailSender)
        {
            _dataContext = dataContext;
            _mapper = mapper;
            _configurationSection = configurationSection.GetSection("SecretKey");
            _emailSender = emailSender;
        }
        public UserRegistrationDto Register(UserRegistrationDto newUser)
        {
            if (newUser.Username.Equals(string.Empty) || newUser.Password.Equals(string.Empty) || !IsValidEmail(newUser.Email) || newUser.FirstName.Equals(string.Empty) || newUser.LastName.Equals(string.Empty) || newUser.Address.Equals(string.Empty))
            {
                throw new Exception("Invalid user data!");
            }
            else 
            {
                newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password);
                var user = (User)_mapper.Map(newUser, typeof(UserRegistrationDto), typeof(User));
                if (user.UserType == UserType.Admin)
                {

                }
                else if (GetUserByUserName(newUser.Username) != null) 
                {
                    throw new Exception("User with this username already exists!");
                }
                string path = "";
                if (newUser.File != null && newUser.File.Length > 0) 
                {
                    path = SaveImage(newUser.File, newUser.Username);
                }
                user.Image = path;
                _dataContext.Users.Add(user);
                _dataContext.SaveChanges();
                return _mapper.Map<UserRegistrationDto>(user);
            }
        }

        public User GetUserByUserName(string username) 
        {
            User user = _dataContext.Users.Where(x => x.Username == username).FirstOrDefault();
            return user;
        }
        public string SaveImage(IFormFile image, string username) 
        {
            string path = Path.Combine(Path.GetDirectoryName(Environment.CurrentDirectory), "ServiceLayer", "Images", username);
            using (Stream fileStream = new FileStream(path, FileMode.Create)) 
            {
                image.CopyTo(fileStream);
            }
            return path;
        }
        private bool IsValidEmail(string email)
        {

            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }

        }

        public TokenDto Login(UserLoginDto user)
        {
            User dbUser = GetUserByUserName(user.Username);
            if (dbUser == null) 
            {
                throw new Exception("User with this username does not exist");
            }
            if (BCrypt.Net.BCrypt.Verify(user.Password, dbUser.Password))
            {
                List<Claim> claims = new List<Claim>();
                if (dbUser.UserType == UserType.Admin)
                {
                    claims.Add(new Claim(ClaimTypes.Role, UserType.Admin.ToString()));
                }
                else if (dbUser.UserType == UserType.Consumer)
                {
                    claims.Add(new Claim(ClaimTypes.Role, UserType.Consumer.ToString()));
                }
                else if (dbUser.UserType == UserType.Deliverer)
                {
                    claims.Add(new Claim(ClaimTypes.Role, UserType.Deliverer.ToString()));
                }
                claims.Add(new Claim("id", dbUser.Id.ToString()));

                SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationSection.Value));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenOptions = new JwtSecurityToken(
                    issuer: "https://localhost:44347",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: signingCredentials);
                string token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
                return new TokenDto { Token = token };
            }
            else 
            {
                throw new Exception("Password is not vailid!");
            }
        }

        public UserDto GetUser(int id)
        {
            var dbUser = _dataContext.Users.Include(x=>x.Orders).Include(x=>x.Delivery).Where(x=>x.Id==id).FirstOrDefault();
            if (dbUser == null)
            {
                throw new Exception("User with this id does not exist!");
            }
            else 
            {
                if (!dbUser.Image.Equals(string.Empty))
                {
                    byte[] imageByte = File.ReadAllBytes(dbUser.Image);
                    dbUser.Image = Convert.ToBase64String(imageByte);
                }
                return _mapper.Map<UserDto>(dbUser);
            }
            
        }

        public List<UserDto> GetDelivery()
        {
            var delivery = _dataContext.Users.Where(x => x.UserType == UserType.Deliverer).ToList();
            return _mapper.Map<List<UserDto>>(delivery);
        }

        public UserDto UpdateUser(UserUpdateDto user)
        {
            var dbUser = _dataContext.Users.Where(x => x.Id == user.Id).FirstOrDefault();
            if (dbUser != GetUserByUserName(user.Username)) 
            {
                throw new Exception("You cannot change your username");
            }
            if (BCrypt.Net.BCrypt.Verify(user.OldPassword, dbUser.Password)) 
            {
                if (user.NewPassword != null && !user.NewPassword.Equals(string.Empty)) 
                {
                    dbUser.Password = BCrypt.Net.BCrypt.HashPassword(user.NewPassword);
                }
                if (user.FirstName != null) 
                {
                    dbUser.FirstName = user.FirstName.Equals(string.Empty) ? dbUser.FirstName : user.FirstName;
                }
                if (user.LastName != null) 
                {
                    dbUser.LastName = user.LastName.Equals(string.Empty) ? dbUser.LastName : user.LastName;
                }
                if (user.Email != null)
                {
                    dbUser.Email = user.Email.Equals(string.Empty) ? dbUser.Email : user.Email;
                }
                if (user.Address != null)
                {
                    dbUser.Address = user.Address.Equals(string.Empty) ? dbUser.Address : user.Address;
                }
                if (user.DateOfBirth != null)
                {
                    dbUser.DateOfBirth = (DateTime)user.DateOfBirth;
                }
                if (user.File != null)
                {
                    string path = SaveImage(user.File, dbUser.Username);
                    dbUser.Image = path;
                }
                _dataContext.Users.Update(dbUser);
                _dataContext.SaveChanges();
                if (!dbUser.Image.Equals(string.Empty)) 
                {
                    byte[] imageByte = File.ReadAllBytes(dbUser.Image);
                    dbUser.Image = Convert.ToBase64String(imageByte);
                }
                return _mapper.Map<UserDto>(dbUser);
            }
            throw new Exception("Error with authentication");
        }

        public UserDto Verification(VerificationDto verification)
        {
            var delivery = _dataContext.Users.Where(x => x.Id == verification.DeliveryId).FirstOrDefault();
            if (delivery == null)
            {
                throw new Exception("Delivery does not exist!");
            }
            else if (delivery.Verification == verification.Verification || delivery.UserType != UserType.Deliverer) 
            {
                throw new Exception("it is not possible to change the verification");
            }
            string status = delivery.Verification ? "Verified" : "Rejected";
            string newStatus = verification.Verification ? "Verified" : "Rejected";

            var message = new Message(new string[] { delivery.Email }, "Notification of a change in the status of your account", $"Your account status has changed.\n" +
               $"The new status is {newStatus}.\nThe previous status is {status}.");
            _emailSender.SendEmail(message);
            delivery.Verification = verification.Verification;
            _dataContext.Users.Update(delivery);
            _dataContext.SaveChanges();

            return _mapper.Map<UserDto>(delivery);
        }
    }
}
