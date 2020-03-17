using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;

namespace WebApplication2.Hubs
{
    public class MessageHub : Hub
    {

        private AuthentificationContext _db;
        public MessageHub(AuthentificationContext db)
        {
            _db = db;
        }
        public async Task Echo(QuestionAndAnswer message)
        {
            await Clients.All.SendAsync("send",message);
        }
    }
}
