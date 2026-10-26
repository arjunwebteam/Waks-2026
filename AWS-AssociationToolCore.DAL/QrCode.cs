using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AWS_AssociationToolCore.DAL
{
    public class QrCode
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;


        public Int64 InsertQrCode(Entities.QrCode objqrcode)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@QrCodeId",objqrcode.QrCodeId),
                    new SqlParameter("@QrLevel",objqrcode.qrLevel == null ?DBNull.Value:(object)objqrcode.qrLevel),
                    new SqlParameter("@labelText",objqrcode.labelText == null ?DBNull.Value:(object)objqrcode.labelText),
                    new SqlParameter("@pixelSize",(objqrcode.pixelSize == 0 ?DBNull.Value:(object)objqrcode.pixelSize)),
                    new SqlParameter("@RefUrl",objqrcode.RefUrl == null ?DBNull.Value:(object)objqrcode.RefUrl),
                    new SqlParameter("@fileName",objqrcode.fileName == null ?DBNull.Value:(object)objqrcode.fileName),
                    new SqlParameter("@OrderNo",(objqrcode.OrderNo == 0 ?DBNull.Value:(object)objqrcode.OrderNo)),
                    new SqlParameter("@IsActive",objqrcode.IsActive),
                    new SqlParameter("@InsertedBy",(objqrcode.InsertedBy==null?(object)DBNull.Value:objqrcode.InsertedBy)),
                    new SqlParameter("@InsertedDate",objqrcode.InsertedDate),
                    new SqlParameter("@UpdatedBy",(objqrcode.UpdatedBy==null?(object)DBNull.Value:objqrcode.UpdatedBy)),
                    new SqlParameter("@UpdatedDate",objqrcode.UpdatedDate),
                    new SqlParameter("@QStatus",0)

                    };
               
                _sqlP[12].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("QrCodeInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[12].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetQrCodeListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total)
                };

                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("QrCodeGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdateQrCodeStatus(Int64 QrCodeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QrCodeId",QrCodeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("QrCodeUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetQrCodeById(Int64 QrCodeId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QrCodeId",QrCodeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("QrCodeGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteQrCode(Int64 QrCodeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QrCodeId",QrCodeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("QrCodeDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateQrCodeDisplayOrder(int OrderNo, Int64 QrCodeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QrCodeId",QrCodeId),
                    new SqlParameter("@OrderNo",OrderNo),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("QrCodeUpdateDisplayOrder", ref _sqlP);

                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


    }
}
