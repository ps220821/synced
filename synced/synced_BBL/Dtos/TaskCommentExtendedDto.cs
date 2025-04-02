using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Dtos
{
    public class TaskCommentExtendedDto : TaskCommentDto
    {
        public string username { get; set; } // Change the setter to public
    }
}
