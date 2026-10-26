using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
    public class CulturalParticipants
    {
        public Int64 RId { get; set; }

        public Int64 ParticipantID { get; set; }

        public Int64 CulturalRegistrationId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public Int64 Age { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }
        
    }
}
