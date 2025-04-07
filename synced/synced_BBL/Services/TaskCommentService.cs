using Microsoft.Data.SqlClient;
using synced.Core.Results;
using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL;
using synced_DAL.Entities;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using synced_DALL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using velocitaApi.Mappers;

namespace synced_BBL.Services
{

    public class TaskCommentService : ITaskCommentService
    {
        private readonly ItaskCommentRepository _taskCommentrRepository;


        public TaskCommentService(ItaskCommentRepository taskCommentrRepository)
        {
            this._taskCommentrRepository = taskCommentrRepository;
        }

        public async Task<OperationResult<int>> AddComment(TaskCommentDto taskCommentDto)
        {
            try
            {
                if (taskCommentDto == null)
                {
                    return OperationResult<int>.Failure("Comment data cannot be null.");
                }

                if (string.IsNullOrEmpty(taskCommentDto.comment))
                {
                    return OperationResult<int>.Failure("Comment cannot be empty.");
                }

                TaskComment newComment = Mapper.MapCreate<TaskComment>(taskCommentDto);
                if (newComment == null)
                {
                    return OperationResult<int>.Failure("Comment mapping failed.");
                }

                int newCommentId =  _taskCommentrRepository.CreateAsync(newComment);

                if (newCommentId > 0)
                {
                    return OperationResult<int>.Success(newCommentId);
                }

                return OperationResult<int>.Failure("Failed to add comment.");
            }
            catch (DatabaseException ex)
            {
                return OperationResult<int>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<int>.Failure(DatabaseHelper.GetErrorMessage(ex));
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
                var comments =  _taskCommentrRepository.GetAllAsync(taskId);

                List<TaskCommentExtendedDto> commentDtos = comments.Select(comment => new TaskCommentExtendedDto
                {
                    id = comment.id,
                    user_id = comment.user_id,
                    task_id = comment.task_id,
                    comment = comment.comment,
                    username = comment.username,
                    created_at = comment.created_at
                }).ToList();

                return OperationResult<List<TaskCommentExtendedDto>>.Success(commentDtos);
            }
            catch (DatabaseException ex)
            {
                return OperationResult<List<TaskCommentExtendedDto>>.Failure(ex.Message);
            }
            catch (SqlException ex)
            {
                return OperationResult<List<TaskCommentExtendedDto>>.Failure(DatabaseHelper.GetErrorMessage(ex));
            }
            catch (Exception)
            {
                return OperationResult<List<TaskCommentExtendedDto>>.Failure("An unexpected error occurred while fetching comments.");
            }
        }
    }
}
