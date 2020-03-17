using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using WebApplication2.Redis;
using WebApplication2.service;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private RedisService redisController;
        private AuthentificationContext _db;
        private RpcClientService rpcClient;
        public ImageController(UserManager<AppUser> userManager, IEasyCachingProviderFactory cachingProviderFactory, AuthentificationContext db)
        {
            redisController = new RedisService(cachingProviderFactory);
            _userManager = userManager;
            rpcClient = new RpcClientService();
            _db = db;
        }

        [HttpPost]
        [Route("Upload")]
        [Authorize]
        public async Task<Object> Upload()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);

            IFormFile file = Request.Form.Files.FirstOrDefault();


            if (file != null)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(file.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)file.Length);
                }
                user.Image = imageData;
                var result = await _userManager.UpdateAsync(user);
                return Ok(result);
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("GetCrops")]
        public List<AppUserModel> GetCrops()
        {
            var users = _userManager.Users.ToList();
            string userId = User.Claims.First(c => c.Type == "UserID").Value;

            ImageService imageService = new ImageService(redisController, _db);
            imageService.FindAndTakeCrop(users);
            return imageService.GetCropImage(users, userId); ;
        }

    }
}