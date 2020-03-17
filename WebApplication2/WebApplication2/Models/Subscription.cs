using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string User_Sub { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }
    }
}
