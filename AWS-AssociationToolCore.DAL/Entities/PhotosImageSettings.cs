using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.Entities
{
    public class PhotosImageSettings
    {
        public Int64 PhotosSettingsid { get; set; }
        public string Eventsize { get; set; }

        public string Photosize { get; set; }

        public string Sponsorsize { get; set; }
        public string Bannersize { get; set; }
        public string Newssize { get; set; }
        public string committeesize { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }

        public string InsertedBy { get; set; }

        public DateTime InsertedDate { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedTime { get; set; }
        public string IsCultural { get; set; }
        public string IsSports { get; set; }
        public string TimeZone { get; set; }
        public string IsHelpDocument { get; set; }
        public string IsFeaturesDocument { get; set; }

    }
}
