using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName ="nvarchar(150)")]
        public string Name { get; set; }
        public byte[] Image { get; set; }

        public virtual ICollection<Subscription> subscriptions { get; set; }
        public virtual ICollection<QuestionAndAnswer> questionAndAnswers { get; set; }

        public AppUser()
        {
            subscriptions = new List<Subscription>();
            questionAndAnswers = new List<QuestionAndAnswer>();
        }
    }
}
