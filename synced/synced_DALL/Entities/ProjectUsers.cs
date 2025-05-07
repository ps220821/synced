using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Entities
{
    public enum Roles
    {
        admin,
        member,
        viewer
    }

    public class ProjectUsers
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public Roles Role { get; set; }
    }
}
