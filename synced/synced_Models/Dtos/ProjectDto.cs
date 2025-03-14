using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_Models.Dtos
{
    public class ProjectDto
    {
        public string name { get; set; }

        public string description { get; set; }

        public DateOnly start_date { get; set; }

        public DateOnly end_date { get; private set; }

        public int owner { get; set; }
    }
}
