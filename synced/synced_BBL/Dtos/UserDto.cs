using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Dtos
{
    public class UserDto
    {
        public string username { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public DateTime created_at { get; set; }
    }
}
