using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using Task = synced_DALL.Entities.TaskComment;

namespace synced_BBL.Services
{
    public class TaskCommentService : ITaskCommentService
    {
        private readonly ItaskCommentRepository _taskCommentrRepository;

        public TaskCommentService(ItaskCommentRepository taskCommentrRepository)
        {
            _taskCommentrRepository = taskCommentrRepository;
        }

        public async Task<OperationResult<int>> AddComment(TaskCommentDto taskCommentDto)
        {
            try
            {
                if (taskCommentDto == null)
                    return OperationResult<int>.Failure("Comment data cannot be null.");
                if (string.IsNullOrWhiteSpace(taskCommentDto.Comment))
                    return OperationResult<int>.Failure("Comment cannot be empty.");

                Task newComment = Task.Create(
                    taskCommentDto.UserId,
                    taskCommentDto.TaskId,
                    taskCommentDto.Comment,
                    DateTime.UtcNow
                );

                int newCommentId = await _taskCommentrRepository.CreateAsync(newComment);
                return (newCommentId > 0)
                    ? OperationResult<int>.Success(newCommentId)
                    : OperationResult<int>.Failure("Failed to add comment.");
            }
            catch (ArgumentException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<int>.Failure("An unexpected error occurred while adding the comment.");
            }
        }

        public async Task<OperationResult<List<TaskCommentExtendedDto>>> GetTaskComments(int taskId)
        {
            try
            {
                var comments = await _taskCommentrRepository.GetAllAsync(taskId);

                var commentDtos = comments.Select(comment => new TaskCommentExtendedDto
                {
                    Id = comment.Id,
                    UserId = comment.UserId,
                    TaskId = comment.TaskId,
                    Comment = comment.Comment,
                    Username = comment.User.Username,
                    CreatedAt = comment.CreatedAt
                }).ToList();

                return OperationResult<List<TaskCommentExtendedDto>>.Success(commentDtos);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<List<TaskCommentExtendedDto>>.Failure(ex.Message);
            }
            catch (Exception)
            {
                return OperationResult<List<TaskCommentExtendedDto>>.Failure("An unexpected error occurred while fetching comments.");
            }
        }
    }
}
