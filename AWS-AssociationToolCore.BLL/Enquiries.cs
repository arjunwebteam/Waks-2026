using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
   public class Enquiries
    {
       AWS_AssociationToolCore.DAL.Enquiries _Enquiries = new AWS_AssociationToolCore.DAL.Enquiries();

        #region Methods

           public Int64 InsertEnquiries(Entities.Enquiries objEnquiries, ref Int64 EnquiryId)
            {
                Int64 _status = 0;
                if (objEnquiries != null)
                {
                    _status = _Enquiries.InsertEnquiries(objEnquiries, ref EnquiryId);

                }
                return _status;
            }

           public Int64 DeleteEnquiry(Int64 EnquiryId)
            {
                Int64 _status = 0;
                _status = _Enquiries.DeleteEnquiry(EnquiryId);
                return _status;
            }

        public Int64 UpdateEnquiriesStatus(Int64 EnquiryId)
        {
            Int64 _status = 0;
            _status = _Enquiries.UpdateEnquiriesStatus(EnquiryId);
            return _status;
        }

        #endregion

        #region Entities filling

        public List<AWS_AssociationToolCore.Entities.Enquiries> GetEnquiriesList(ref int status)
            {
                List<AWS_AssociationToolCore.Entities.Enquiries> lstEnquiries = new List<Entities.Enquiries>();
                DataTable dt = _Enquiries.GetEnquiriesList(ref status);

                if (dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        AWS_AssociationToolCore.Entities.Enquiries objlstEnquiries = new AWS_AssociationToolCore.Entities.Enquiries();

                        objlstEnquiries.EnquiryId = Convert.ToInt64(dr["EnquiryId"].ToString());
                        objlstEnquiries.Name = dr["Name"].ToString();
                        objlstEnquiries.Email = dr["Email"].ToString();
                        objlstEnquiries.Subject = dr["Subject"].ToString();
                        objlstEnquiries.Description = dr["Description"].ToString();
                        objlstEnquiries.PhoneNo = dr["PhoneNo"].ToString();
                        objlstEnquiries.IsActive = Convert.ToBoolean(dr["IsActive"]);
                        objlstEnquiries.InsertedTime = Convert.ToDateTime(dr["InsertedTime"].ToString());
                        objlstEnquiries.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                        objlstEnquiries.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);

                        lstEnquiries.Add(objlstEnquiries);
                    }

                }
                return lstEnquiries;
            }

           public AWS_AssociationToolCore.Entities.Enquiries GetEnquirysById(Int64 EnquiryId, ref int status)
           {
               AWS_AssociationToolCore.Entities.Enquiries objEnquiries = new AWS_AssociationToolCore.Entities.Enquiries();
               DataTable dt = new DataTable();
               if (EnquiryId != 0)
               {
                   dt = _Enquiries.GetEnquiryById(EnquiryId, ref status);
                   if (dt.Rows.Count == 1)
                   {
                       objEnquiries.EnquiryId = Convert.ToInt64(dt.Rows[0]["EnquiryId"].ToString());
                       objEnquiries.Name = dt.Rows[0]["Name"].ToString();
                       objEnquiries.Email = dt.Rows[0]["Email"].ToString();
                       objEnquiries.Description = dt.Rows[0]["Description"].ToString();
                       objEnquiries.Subject = dt.Rows[0]["Subject"].ToString();
                       objEnquiries.PhoneNo = dt.Rows[0]["PhoneNo"].ToString();
                       objEnquiries.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"]);
                       objEnquiries.InsertedTime = Convert.ToDateTime(dt.Rows[0]["InsertedTime"].ToString());
                       objEnquiries.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : null);
                       objEnquiries.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : null);
                   }
               }
               return objEnquiries;
           }

           public List<AWS_AssociationToolCore.Entities.Enquiries> GetEnquiriesListByVariable(Int64 EventId,string FromDate,string ToDate, string Search, string Sort, int PageNo, int Items, ref int Total)
           {
               List<AWS_AssociationToolCore.Entities.Enquiries> lstEnquiries = new List<AWS_AssociationToolCore.Entities.Enquiries>();
               DataTable dt = _Enquiries.GetEnquiriesListByVariable(EventId, FromDate, ToDate, Search, Sort, PageNo, Items, ref Total);
               if (dt.Rows.Count == 0 && PageNo != 0)
               {
                   dt = _Enquiries.GetEnquiriesListByVariable(EventId, FromDate, ToDate, Search, Sort, PageNo - 1, Items, ref Total);
               }
               if (dt.Rows.Count != 0)
               {
                   foreach (DataRow dr in dt.Rows)
                   {
                       AWS_AssociationToolCore.Entities.Enquiries objEnquiries = new AWS_AssociationToolCore.Entities.Enquiries();

                       objEnquiries.RId = Convert.ToInt64(dr["RId"].ToString());
                       objEnquiries.EnquiryId = Convert.ToInt64(dr["EnquiryId"].ToString());
                       objEnquiries.Name = dr["Name"].ToString();
                       objEnquiries.Email = dr["Email"].ToString();
                       objEnquiries.Description = dr["Description"].ToString();
                       objEnquiries.Subject = dr["Subject"].ToString();
                       objEnquiries.PhoneNo = dr["PhoneNo"].ToString();
                       objEnquiries.IsActive = Convert.ToBoolean(dr["IsActive"]);
                       objEnquiries.InsertedTime = Convert.ToDateTime(dr["InsertedTime"].ToString());
                       objEnquiries.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                       objEnquiries.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                       objEnquiries.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);


                    
                       lstEnquiries.Add(objEnquiries);
                   }
               }
               return lstEnquiries;
           }    

        #endregion
    }
}
