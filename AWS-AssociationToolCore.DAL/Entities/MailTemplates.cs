using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
   public class MailTemplates
    {
       public Int64 RId { get; set; }

       public Int64 MailTemplateId { get; set; }

        public string Heading { get; set; }

        public string Subject { get; set; }

        public string Description { get; set; }

        public string MailType { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }
    }

   public class SendMail
   {
       public string EmailFrom { get; set; }

       public string EmailTo { get; set; }

       public string MailTemplateName { get; set; }

       public string Heading { get; set; }

       public string Subject { get; set; }

       public string Description { get; set; }
   }
}
