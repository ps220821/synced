using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateOnly Start_Date { get; set; }
        public DateOnly End_Date { get;  set; }
        public int Owner { get; set; }
        public User User { get; set; }
    }
}
