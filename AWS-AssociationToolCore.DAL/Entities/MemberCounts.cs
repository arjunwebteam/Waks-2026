using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
  
        public class MemberCounts
        {
            public int AllCount { get; set; }
            public int ActiveCount { get; set; }
            public int InactiveCount { get; set; }
            public int PendingCount { get; set; }
        }
    
}
