using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Redis;

namespace WebApplication2.service
{
    public class ImageService:ImageToByteArrayService
    {
        private RedisService redisController;
        private RpcClientService rpcClient;
        private AuthentificationContext _db;

        public ImageService(RedisService cachingProviderFactory, AuthentificationContext db)
        {
            redisController = cachingProviderFactory;
            rpcClient = new RpcClientService();
            _db = db;
        }

    
        public List<AppUserModel> GetCropImage(List<AppUser> users, string userId)
        {
            List<AppUserModel> UserCropArray = new List<AppUserModel>();

            foreach (var item in users)//get crop for users
            {
                var temp = new AppUserModel();
                var crop = redisController.getItem(item.Id);
                if (crop != null && item.Id != userId)
                {
                    temp.Id = item.Id;
                    temp.Image = crop;
                    temp.Name = item.Name;
                    temp.count = _db.questionAndAnswers.Where(e => e.UserId == item.Id).Count();
                    temp.UserName = item.UserName;
                    UserCropArray.Add(temp);
                }
                else if (item.Id != userId)
                {
                    temp.Id = item.Id;
                    temp.Image = ImageToByteArray(Image.FromFile(@"C:\Users\Никита Дубовский\source\repos\WebApplication2\WebApplication2\noimage.jpg"));
                    temp.Name = item.Name;
                    temp.UserName = item.UserName;
                    temp.count = _db.questionAndAnswers.Where(e => e.UserId == item.Id).Count();
                    UserCropArray.Add(temp);
                }
            }

            return UserCropArray;
        }

        public void FindAndTakeCrop(List<AppUser> users)
        {
            foreach (var item in users)///find users without crop
            {
                if (redisController.getItem(item.Id) == null && item.Image != null)
                {
                    redisController.setItem(item.Id, rpcClient.Call(item.Id));
                }
            }
        }

    }

    public class ImageToByteArrayService
    {
        public ImageToByteArrayService()
        {
                
        }
        public byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

    }
}
