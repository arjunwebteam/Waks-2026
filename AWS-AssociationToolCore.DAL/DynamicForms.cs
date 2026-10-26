using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.DAL
{
    public class DynamicForms
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 SaveFormSchema(Entities.FormSchemas objformschmas)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@FormName",(objformschmas.FormName == null ?DBNull.Value:(object)objformschmas.FormName)),
                     new SqlParameter("@Schema",(objformschmas.FormSchema == null ?DBNull.Value:(object)objformschmas.FormSchema)),
                      new SqlParameter("@FormId",(objformschmas.FormId == 0 ?DBNull.Value:(object)objformschmas.FormId)),
                    new SqlParameter("@QStatus",0),
                   
                    };
                _sqlP[3].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("SaveFormSchema", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[3].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
    }
}
