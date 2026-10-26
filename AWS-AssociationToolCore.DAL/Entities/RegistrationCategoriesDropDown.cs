using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.Entities
{
  public  class RegistrationCategoriesDropDown
    {
        public Int64 RId { get; set; }
        public Int64 RegistrationCategoriesCategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public Int64 OrderNo { get; set; }
        public Boolean IsActive { get; set; }
        public decimal Price { get; set; }
        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedTime { get; set; }

    }
}
