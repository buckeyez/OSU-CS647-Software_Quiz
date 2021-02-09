using Microsoft.EntityFrameworkCore;
using OSU_CS467_Software_Quiz.Data;
using OSU_CS467_Software_Quiz.Models;
using OSU_CS467_Software_Quiz.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OSU_CS467_Software_Quiz.Repositories
{
  public class QuizzesRepo : IQuizzesRepo
  {
    private readonly AppDbContext _db;

    public QuizzesRepo(AppDbContext db)
    {
      _db = db;
    }

    public async Task<Quizzes> AddQuizAsync(string name)
    {
      var quizEntry = _db.Add<Quizzes>(new()
      {
        Name = name,
      });

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
        return null;
      }

      return quizEntry.Entity;
    }

    public async Task<QuizAssignments> AssignQuizAsync(QuizAssignmentNew quizAssignment)
    {
      var quizEntity = _db.Quizzes
        .AsQueryable()
        .Where(q => q.Id == quizAssignment.QuizId)
        .FirstOrDefault();

      var userEntity = _db.Users
        .AsQueryable()
        .Where(u => u.Id == quizAssignment.UserId)
        .FirstOrDefault();

      if (quizEntity == null || userEntity == null)
      {
        Console.WriteLine(
          $"QuizAssignment: Quiz ({quizAssignment.QuizId}) or User ({quizAssignment.UserId}) does not exist."
        );
        return null;
      }

      var assignmentExists = _db.QuizAssignments
        .AsQueryable()
        .Where(qa => qa.QuizId == quizEntity.Id && qa.User.Id == userEntity.Id)
        .FirstOrDefault();

      if (assignmentExists != null)
      {
        return null;
      }

      var quizAssignmentEntry = _db.QuizAssignments.Add(new()
      {
        Quiz = quizEntity,
        TimeAllotment = quizAssignment.TimeAllotment,
        User = userEntity,
      });

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
        return null;
      }

      return quizAssignmentEntry.Entity;
    }

    public async Task DeleteQuizAsync(int id)
    {
      var entity = _db.Quizzes
        .AsQueryable()
        .Where(q => q.Id == id)
        .FirstOrDefault();

      if (entity == null)
      {
        return;
      }

      _db.Quizzes.Remove(entity);

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
      }
    }

    public Task<Quizzes> GetQuiz(int id, bool partial)
    {
      if (partial)
      {
        return _db.Quizzes
          .AsQueryable()
          .Where(q => q.Id == id)
          .Include(qq => qq.QuizQuestions)
          .ThenInclude(qq => qq.Question)
          .FirstOrDefaultAsync();
      }

      return _db.Quizzes
        .AsQueryable()
        .Where(q => q.Id == id)
        .Include(qq => qq.QuizQuestions)
        .ThenInclude(qq => qq.Question)
        .ThenInclude(q => q.QuestionAnswers)
        .ThenInclude(qa => qa.Answer)
        .FirstOrDefaultAsync();
    }

    public Task<List<Quizzes>> GetQuizzes()
    {
      return _db.Quizzes.ToListAsync();
    }

    public Task<List<QuizAssignments>> GetQuizAssignments()
    {
      return _db.QuizAssignments
        .Include(qa => qa.Quiz)
        .Include(qa => qa.User)
        .ToListAsync();
    }

    public Task<List<AppUser>> GetUsersAssignedToQuiz(int id)
    {
      return _db.QuizAssignments
        .AsQueryable()
        .Where(qa => qa.QuizId == id)
        .Include(u => u.User)
        .Select(qa => qa.User)
        .ToListAsync();
    }

    public Task<List<Quizzes>> GetQuizAssignmentsForUser(string userId)
    {
      return _db.QuizAssignments
        .AsQueryable()
        .Where(qa => qa.UserId == userId)
        .Include(q => q.Quiz)
        .Select(qa => qa.Quiz)
        .ToListAsync();
    }

    public async Task RemoveQuizAssignmentAsync(int id)
    {
      var entity = _db.QuizAssignments
        .AsQueryable()
        .Where(qa => qa.Id == id)
        .FirstOrDefault();

      if (entity == null)
      {
        return;
      }

      _db.QuizAssignments.Remove(entity);

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
      }
    }

    public async Task<QuizAssignments> SubmitQuizAsync(QuizSubmission quizSubmission)
    {
      var quizAssignmentEntity = _db.QuizAssignments
        .AsQueryable()
        .Where(qa => qa.Id == quizSubmission.QuizAssignmentId)
        .FirstOrDefault();

      if (quizAssignmentEntity == null)
      {
        Console.WriteLine($"QuizSubmission: Quiz Assignment ({quizSubmission.QuizAssignmentId}) does not exist.");
        return null;
      }

      quizAssignmentEntity.TimeTaken = quizSubmission.TimeTaken;

      foreach (AnswerSubmission selection in quizSubmission.UserSelections)
      {
        var answer = _db.Answers
          .AsQueryable()
          .Where(a => a.Id == selection.AnswerId)
          .FirstOrDefault();

        var question = _db.Questions
          .AsQueryable()
          .Where(q => q.Id == selection.QuestionId)
          .FirstOrDefault();

        if ((selection.AnswerId != 0 && answer == null) || question == null)
        {
          Console.WriteLine(
            $"QuizSubmission: Question ({selection.QuestionId}) or Answer ({selection.AnswerId}) does not exist."
          );
          return null;
        }

        _db.QuizResults.Add(new()
        {
          Answer = answer,
          FreeResponse = selection.FreeResponse,
          Question = question,
          QuizAssignment = quizAssignmentEntity,
        });
      }

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
        return null;
      }

      return quizAssignmentEntity;
    }

    public async Task<Quizzes> UpdateQuizNameAsync(int id, string name)
    {
      var entity = _db.Quizzes
        .AsQueryable()
        .Where(q => q.Id == id)
        .FirstOrDefault();

      if (entity == null)
      {
        Console.WriteLine($"QuizName: Quiz ({id}) does not exist.");
        return null;
      }

      entity.Name = name;
      _db.Quizzes.Update(entity);

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
        return null;
      }

      return entity;
    }

    public async Task<Quizzes> UpdateQuizQuestionsAsync(int id, QuizUpdates entityUpdates)
    {
      var entity = _db.Quizzes
        .AsQueryable()
        .Where(q => q.Id == id)
        .Include(q => q.QuizQuestions)
        .ThenInclude(qq => qq.Question)
        .FirstOrDefault();

      if (entity == null)
      {
        Console.WriteLine($"QuizQuestions: Quiz ({id}) does not exist.");
        return null;
      }

      foreach (int questionId in entityUpdates.EntityIdsToAdd)
      {
        var questionEntity = _db.Questions
          .AsQueryable()
          .Where(q => q.Id == questionId)
          .FirstOrDefault();

        if (questionEntity == null)
        {
          Console.WriteLine($"QuizQuestions: Question ({questionId}) does not exist.");
          return null;
        }

        _db.QuizQuestions.Add(new()
        {
          Question = questionEntity,
          Quiz = entity,
        });
      }

      foreach (int questionId in entityUpdates.EntityIdsToRemove)
      {
        var qqEntity = _db.QuizQuestions
          .AsQueryable()
          .Where(qq => qq.QuestionId == questionId)
          .FirstOrDefault();

        if (qqEntity == null)
        {
          continue;
        }

        _db.QuizQuestions.Remove(qqEntity);
      }

      try
      {
        await _db.SaveChangesAsync();
      }
      catch (DbUpdateException e)
      {
        Console.WriteLine($"{e.Source}: {e.Message}");
        return null;
      }

      return entity;
    }
  }
}