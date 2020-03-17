using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebApplication2.Hubs;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private AuthentificationContext _db;
        private UserManager<AppUser> _userManager;
        public SubscriptionController(AuthentificationContext db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<List<Subscription>> Message()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var temp = _db.subscriptions.Where(e => e.UserId == user.Id).ToList();
            temp.ForEach(e => e.User = null);
            return  temp;
        }

        [HttpPost]
        [Route("Sub")]
        public async Task<IActionResult> SetSub(Subscription model)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                model.User = user;
                _db.subscriptions.Add(model);
                _db.SaveChanges();
                model.User = null;
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

    }
}