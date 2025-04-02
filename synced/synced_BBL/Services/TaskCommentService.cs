using synced_BBL.Dtos;
using synced_BBL.Interfaces;
using synced_DAL.Entities;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
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

        public int AddComment(TaskCommentDto taskComment)
        {
            TaskComment newComment  = Mapper.MapCreate<TaskComment>(taskComment);
            return this._taskCommentrRepository.CreateAsync(newComment);
        }

        public List<TaskCommentExtendedDto> GetTaskComments(int taskId)
        {
            var comments = this._taskCommentrRepository.GetAllAsync(taskId);

            List<TaskCommentExtendedDto> com = comments.Select(comment => new TaskCommentExtendedDto
            {
                id = comment.id,
                user_id = comment.user_id,
                task_id = comment.task_id,
                comment = comment.comment,
                username = comment.username,
                created_at = comment.created_at
                
            }).ToList();

            return com;
        }
    }
}
