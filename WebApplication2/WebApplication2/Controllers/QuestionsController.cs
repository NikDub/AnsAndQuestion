using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("[controller]")]
        [Authorize]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private UserManager<AppUser> _userManager;
        private AuthentificationContext _db;
        public QuestionsController(UserManager<AppUser> userManager, AuthentificationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        [HttpGet]
        [Route("getquestionswithanswer")]
        public async Task<List<QuestionAndAnswer>> GetQuestionWithAnswer()
        {
            string userId = User.Claims.First(c => c.Type == "UserID").Value;
            var user = await _userManager.FindByIdAsync(userId);
            var questions = _db.questionAndAnswers.Where(e => e.Answer != null && e.Answer != "" && e.User.Id == user.Id).ToList();
            questions.ForEach(e => e.User = null);
            return questions;
        }

        [HttpGet]
        [Route("getquestionswithanswer/{id}")]
        public async Task<List<QuestionAndAnswer>> GetQuestionWithAnswer(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var questions = _db.questionAndAnswers.Where(e => e.Answer != null && e.Answer != "" && e.User.Id == user.Id).ToList();
            questions.ForEach(e => e.User = null);
            return questions;
        }

        [HttpGet]
        [Route("getquestions")]
        public List<QuestionAndAnswer> GetQuestion()
        {
            var questions = _db.questionAndAnswers.Where(e => e.Answer == null || e.Answer == "").ToList();

            return questions;
        }

        [HttpPost]
        [Route("AddQuestion")]
        public async Task<IActionResult> SetQuestionProfile(QuestionAndAnswer quest)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                _db.questionAndAnswers.Add(quest);
                _db.SaveChanges();
                return Ok(quest);
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        [Route("AddAnswer")]
        public async Task<IActionResult> SetAnswerProfile(QuestionAndAnswer quest)
        {
            if (ModelState.IsValid)
            {
                string userId = User.Claims.First(c => c.Type == "UserID").Value;
                var user = await _userManager.FindByIdAsync(userId);
                quest.User = user;
                _db.questionAndAnswers.Update(quest);
                _db.SaveChanges();
                quest.User = null;
                return Ok(quest);
            }
            return BadRequest(ModelState);
        }

    }
}