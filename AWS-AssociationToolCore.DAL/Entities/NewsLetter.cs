using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class NewsLetter
    {
        public Int64 LetterId { get; set; }

        public string EmailId { get; set; }

        public DateTime RegisteredDate { get; set; }

        public Boolean IsActive { get; set; }

        public Int64 RId { get; set; }
    }
}
