using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
    public class DynamicForms
    {
        DAL.DynamicForms _DynamicForms = new DAL.DynamicForms();
        public Int64 SaveFormSchema(Entities.FormSchemas objformschmas)
        {
            Int64 _status = 0;
            if (objformschmas != null)
            {
                _status = _DynamicForms.SaveFormSchema(objformschmas);
            }
            return _status;
        }
    }
}
