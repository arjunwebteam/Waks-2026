using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;


namespace AWS_AssociationToolCore.BLL
{
    public class QrCode
    {

        AWS_AssociationToolCore.DAL.QrCode _QrCode = new AWS_AssociationToolCore.DAL.QrCode();


        public Int64 InsertQrCode(Entities.QrCode objqrcode)
        {
            Int64 _status = 0;
            if (objqrcode != null)
            {
                _status = _QrCode.InsertQrCode(objqrcode);

            }
            return _status;
        }

        public List<AWS_AssociationToolCore.Entities.QrCode> GetQrCodeGetListByVariable(string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.QrCode> lstQrCode = new List<AWS_AssociationToolCore.Entities.QrCode>();
            DataTable dt = _QrCode.GetQrCodeListByVariable(Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _QrCode.GetQrCodeListByVariable(Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.QrCode objqrcode = new AWS_AssociationToolCore.Entities.QrCode();

                    objqrcode.RId = Convert.ToInt64(dr["RId"]);
                    objqrcode.QrCodeId = Convert.ToInt64(dr["QrCodeId"].ToString());
                    objqrcode.qrLevel = dr["qrLevel"].ToString();
                    objqrcode.labelText = dr["labelText"].ToString();
                    objqrcode.pixelSize = (dr["pixelSize"] != DBNull.Value ? Convert.ToInt32(dr["pixelSize"]) : 0);
                    objqrcode.RefUrl = dr["RefUrl"].ToString();
                    objqrcode.fileName = dr["fileName"].ToString();
                    objqrcode.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objqrcode.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt64(dr["OrderNo"]) : 0);
                    objqrcode.InsertedBy = (dr["InsertedBy"] != DBNull.Value ? dr["InsertedBy"].ToString() : "");
                    objqrcode.InsertedDate = (dr["InsertedDate"] != DBNull.Value ? Convert.ToDateTime(dr["InsertedDate"]) : DateTime.MinValue);
                    objqrcode.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : "");
                    objqrcode.UpdatedDate = (dr["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedDate"]) : DateTime.MinValue);

                    lstQrCode.Add(objqrcode);
                }
            }
            return lstQrCode;
        }


        public Int64 UpdateQrCodeStatus(Int64 QrCodeId)
        {
            Int64 _status = 0;
            _status = _QrCode.UpdateQrCodeStatus(QrCodeId);
            return _status;
        }

        public AWS_AssociationToolCore.Entities.QrCode GetQrCodeById(Int64 QrCodeId, ref int status)
        {
            AWS_AssociationToolCore.Entities.QrCode objqrcode = new AWS_AssociationToolCore.Entities.QrCode();
            DataTable dt = new DataTable();
            if (QrCodeId != 0)
            {
                dt = _QrCode.GetQrCodeById(QrCodeId, ref status);
                if (dt.Rows.Count == 1)
                {
                    objqrcode.QrCodeId = Convert.ToInt64(dt.Rows[0]["QrCodeId"].ToString());
                    objqrcode.qrLevel = dt.Rows[0]["qrLevel"].ToString();
                    objqrcode.labelText = dt.Rows[0]["labelText"].ToString();
                    objqrcode.pixelSize = (dt.Rows[0]["pixelSize"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["pixelSize"]) : 0);
                    objqrcode.RefUrl = dt.Rows[0]["RefUrl"].ToString();
                    objqrcode.fileName = dt.Rows[0]["fileName"].ToString();
                    objqrcode.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                    objqrcode.OrderNo = (dt.Rows[0]["OrderNo"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["OrderNo"]) : 0);
                    objqrcode.InsertedDate = (dt.Rows[0]["InsertedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["InsertedDate"]) : DateTime.MinValue);
                    objqrcode.InsertedBy = (dt.Rows[0]["InsertedBy"] != DBNull.Value ? dt.Rows[0]["InsertedBy"].ToString() : null);
                    objqrcode.UpdatedBy = (dt.Rows[0]["UpdatedBy"] != DBNull.Value ? dt.Rows[0]["UpdatedBy"].ToString() : null);
                    objqrcode.UpdatedDate = (dt.Rows[0]["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]) : DateTime.MinValue);


                }
            }
            return objqrcode;
        }

        public Int64 DeleteQrCode(Int64 QrCodeId)
        {
            Int64 _status = 0;
            _status = _QrCode.DeleteQrCode(QrCodeId);
            return _status;
        }

        public Int64 UpdateQrCodeDisplayOrder(int OrderNo, Int64 QrCodeId)
        {
            Int64 _status = 0;
            _status = _QrCode.UpdateQrCodeDisplayOrder(OrderNo, QrCodeId);
            return _status;
        }

    }
}
