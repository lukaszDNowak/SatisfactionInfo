﻿using Microsoft.EntityFrameworkCore;
using SatisfactionInfo.Models.DAL.SQL;
using SatisfactionInfo.Models.DTO;
using SatisfactionInfo.Models.Repo.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatisfactionInfo.Models.Repo.SQL
{
    public class VUserQuestionnarieRepo : IVUserQuestionnarieRepo
    {
        private readonly SatisfactionInfoContext db;

        public VUserQuestionnarieRepo(SatisfactionInfoContext db)
        {
            this.db = db;
        }
        public async Task<UserQuestionnarieDTO> Get(string code)
        {
            var questionnarie = await db.Questionnaries.Where(a => a.Code.ToLower() == code.ToLower()).FirstOrDefaultAsync();
            if (questionnarie != null)
            {
                var questionnariesQuestion = await db.QuestionnariesQuestion.Where(a => a.QuestionnarieId == questionnarie.Id).ToListAsync();

                var result = new UserQuestionnarieDTO();
                result.Id = questionnarie.Id;
                result.Name = questionnarie.Name;
                result.Code = questionnarie.Code;
                result.Questions = await db.Questions.Where(a => questionnariesQuestion.Any(b => b.QuestionId == a.Id)).Select(a => new QuestionsDTO
                {
                    Id = a.Id,
                    AddWhy = a.AddWhy,
                    AddWhyName = a.AddWhyName,
                    AnswerType = a.AnswerType,
                    Question = a.Question
                }).ToListAsync();

                result.Questions.ForEach(q =>
                {
                    var questionsAnswerDTO = db.QuestionsAnswer.Where(a => a.QuestionId == q.Id).ToList();
                    q.AnswersDTOs = db.Answers.Where(a => questionsAnswerDTO.Any(b => b.AnswerId == a.Id)).Select(c => new AnswersDTO
                    {
                        Id = c.Id,
                        Answer = c.Answer,
                        Weight = c.Weight
                    }).ToList();
                });

                return result;
            }
            return null;
        }
    }
}
