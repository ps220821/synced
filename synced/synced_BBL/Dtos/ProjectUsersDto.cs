using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Dtos
{
    public class ProjectUsersDto
    {
        public required int project_id { get; set; }

        public required int user_id { get; set; }

        public Roles roles { get; set; }
    }
}
