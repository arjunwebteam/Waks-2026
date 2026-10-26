using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
   public class PromoCodes
    {
		public Int64 RId { get; set; }
		public Int64 PromoCodeId { get; set; }
		public string  PromoCode { get; set; }
		public string  Message { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public Int64 NoOfUsers { get; set; }
		public decimal MinimumOrderAmount { get; set; }
		public decimal Discount { get; set; }
        public decimal AfterDiscountTotal { get; set; }
        public string DiscountType { get; set; }
		public decimal MaxDiscountAmount { get; set; }
		public string RepeatUsage { get; set; }
		public Int64 NoOfRepeatUsage { get; set; }
		public string Status { get; set; }
		public Boolean IsActive { get; set; }
		public Int64 ServiceId { get; set; }
		public string Field1 { get; set; }
		public string Field2 { get; set; }
		public string Field3 { get; set; }
		public string InsertedBy { get; set; }
		public DateTime InsertedDate { get; set; }
		 public string UpdatedBy { get; set; }
		public DateTime UpdatedDate { get; set; }
        public string ServiceName { get; set; }

		public Decimal TotalDiscountAmount { get; set; }

	}
}
