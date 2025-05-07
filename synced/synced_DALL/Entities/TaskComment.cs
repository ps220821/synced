﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Entities
{
    public class TaskComment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } 
        public int TaskId { get; set; }
        public Task Task { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
