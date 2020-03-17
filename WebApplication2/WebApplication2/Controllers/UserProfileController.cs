using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.service;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private AuthentificationContext _db;
        public UserProfileController(UserManager<AppUser> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        [Authorize]
        public async Task<Object> GetUserProfile()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            return new
            {
                user.Name,
                user.Email,
                user.UserName,
                user.Image,
                user.Id
            };
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<Object> GetUserProfileById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user.Image == null)
            {
                user.Image = new ImageToByteArrayService().ImageToByteArray(Image.FromFile(@"C:\Users\Никита Дубовский\source\repos\WebApplication2\WebApplication2\noimage.jpg"));
            }
            return new
            {
                user.Name,
                user.Email,
                user.UserName,
                user.Image
            };
        }
    
    }
}