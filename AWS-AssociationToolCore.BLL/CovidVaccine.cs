using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWS_AssociationToolCore.BLL
{
    public class CovidVaccine
    {
        AWS_AssociationToolCore.DAL.CovidVaccine _CovidVaccine = new AWS_AssociationToolCore.DAL.CovidVaccine();

        public Int64 FECovidVaccineInsert(Entities.CovidVaccine objCovidVaccine, ref Int64 CovidVaccineId)
        {
            Int64 _status = 0;
            if (objCovidVaccine != null)
            {
                _status = _CovidVaccine.FECovidVaccineInsert(objCovidVaccine, ref CovidVaccineId);

            }
            return _status;
        }


        public Entities.CovidVaccine GetCovidVaccineGetById(Int64 CovidVaccineId, ref int status)
        {
            DataTable dt = _CovidVaccine.GetCovidVaccineGetById(CovidVaccineId, ref status);
            Entities.CovidVaccine objCovidVaccine = new Entities.CovidVaccine();

            if (dt.Rows.Count == 1)
            {
                objCovidVaccine.CovidVaccineId = Convert.ToInt64(dt.Rows[0]["CovidVaccineId"]);
                objCovidVaccine.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                objCovidVaccine.MemberId = (dt.Rows[0]["MemberId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["MemberId"]) : 0);
                objCovidVaccine.Email = (dt.Rows[0]["Email"] != DBNull.Value ? dt.Rows[0]["Email"].ToString() : null);
                objCovidVaccine.COVIDVaccinefor = (dt.Rows[0]["COVIDVaccinefor"] != DBNull.Value ? dt.Rows[0]["COVIDVaccinefor"].ToString() : null);
                objCovidVaccine.KNameOfChild = (dt.Rows[0]["KNameOfChild"] != DBNull.Value ? dt.Rows[0]["KNameOfChild"].ToString() : null);
                objCovidVaccine.KDOB = (dt.Rows[0]["KDOB"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["KDOB"].ToString()) : DateTime.MinValue);
                objCovidVaccine.KNameOfParent = (dt.Rows[0]["KNameOfParent"] != DBNull.Value ? dt.Rows[0]["KNameOfParent"].ToString() : null);
                objCovidVaccine.KMailingAddress = (dt.Rows[0]["KMailingAddress"] != DBNull.Value ? dt.Rows[0]["KMailingAddress"].ToString() : null);
                objCovidVaccine.KPhoneNumber = (dt.Rows[0]["KPhoneNumber"] != DBNull.Value ? dt.Rows[0]["KPhoneNumber"].ToString() : null);
                objCovidVaccine.KHaveAnInsurance = (dt.Rows[0]["KHaveAnInsurance"] != DBNull.Value ? dt.Rows[0]["KHaveAnInsurance"].ToString() : null);
                objCovidVaccine.KAdditionalInformation = (dt.Rows[0]["KAdditionalInformation"] != DBNull.Value ? dt.Rows[0]["KAdditionalInformation"].ToString() : null);
                objCovidVaccine.ABoosterShotSelection = (dt.Rows[0]["ABoosterShotSelection"] != DBNull.Value ? dt.Rows[0]["ABoosterShotSelection"].ToString() : null);
                objCovidVaccine.AName = (dt.Rows[0]["AName"] != DBNull.Value ? dt.Rows[0]["AName"].ToString() : null);
                objCovidVaccine.ADOB = (dt.Rows[0]["ADOB"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["ADOB"].ToString()) : DateTime.MinValue);
                objCovidVaccine.AMailingAddress = (dt.Rows[0]["AMailingAddress"] != DBNull.Value ? dt.Rows[0]["AMailingAddress"].ToString() : null);
                objCovidVaccine.APhoneNumber = (dt.Rows[0]["APhoneNumber"] != DBNull.Value ? dt.Rows[0]["APhoneNumber"].ToString() : null);
                objCovidVaccine.APreferredTimeSlots = (dt.Rows[0]["APreferredTimeSlots"] != DBNull.Value ? dt.Rows[0]["APreferredTimeSlots"].ToString() : null);
                objCovidVaccine.APrintTimeSlot = (dt.Rows[0]["APrintTimeSlot"] != DBNull.Value ? dt.Rows[0]["APrintTimeSlot"].ToString() : null);
                objCovidVaccine.IsActive = Convert.ToBoolean(dt.Rows[0]["IsActive"].ToString());
                objCovidVaccine.InsertedBy = dt.Rows[0]["InsertedBy"].ToString();
                objCovidVaccine.InsertedDate = Convert.ToDateTime(dt.Rows[0]["InsertedDate"]);

                objCovidVaccine.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objCovidVaccine.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]);
                
            }

            return objCovidVaccine;
        }

        public List<AWS_AssociationToolCore.Entities.CovidVaccine> GetCovidVaccineListByVariableId(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.CovidVaccine> lstCovidVaccine = new List<AWS_AssociationToolCore.Entities.CovidVaccine>();
            DataTable dt = _CovidVaccine.GetCovidVaccineListByVariableId(ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _CovidVaccine.GetCovidVaccineListByVariableId(ChapterId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CovidVaccine objCovidVaccine = new AWS_AssociationToolCore.Entities.CovidVaccine();

                    objCovidVaccine.RId = Convert.ToInt64(dr["RId"].ToString());
                    objCovidVaccine.CovidVaccineId = Convert.ToInt64(dr["CovidVaccineId"].ToString());
                    objCovidVaccine.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt64(dr["MemberId"]) : 0);
                    objCovidVaccine.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objCovidVaccine.Email = (dr["Email"] != DBNull.Value ? (dr["Email"].ToString()) : null);
                    objCovidVaccine.COVIDVaccinefor = (dr["COVIDVaccinefor"] != DBNull.Value ? (dr["COVIDVaccinefor"].ToString()) : null);
                    objCovidVaccine.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objCovidVaccine.InsertedBy = (dr["InsertedBy"] != DBNull.Value ? (dr["InsertedBy"].ToString()) : null);
                    objCovidVaccine.InsertedDate = (dr["InsertedDate"] != DBNull.Value ? Convert.ToDateTime(dr["InsertedDate"].ToString()) : DateTime.MinValue);                    
                    objCovidVaccine.ChapterName = (dr["ChapterName"] != DBNull.Value ? (dr["ChapterName"].ToString()) : null);

                    lstCovidVaccine.Add(objCovidVaccine);
                }
            }
            return lstCovidVaccine;
        }

        public Int64 DeleteCovidVaccine(Int64 CovidVaccineId)
        {
            Int64 _status = 0;
            if (CovidVaccineId != 0)
            {
                _status = _CovidVaccine.DeleteCovidVaccine(CovidVaccineId);
            }
            return _status;
        }

        public List<AWS_AssociationToolCore.Entities.CovidVaccine> GetCovidVaccineList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.CovidVaccine> lstCovidVaccine = new List<Entities.CovidVaccine>();
            DataTable dt = _CovidVaccine.GetCovidVaccineList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.CovidVaccine objCovidVaccine = new AWS_AssociationToolCore.Entities.CovidVaccine();

                    objCovidVaccine.CovidVaccineId = Convert.ToInt32(dr["CovidVaccineId"].ToString());
                    objCovidVaccine.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objCovidVaccine.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt64(dr["MemberId"]) : 0);
                    objCovidVaccine.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objCovidVaccine.COVIDVaccinefor = (dr["COVIDVaccinefor"] != DBNull.Value ? dr["COVIDVaccinefor"].ToString() : null);
                    objCovidVaccine.KNameOfChild = (dr["KNameOfChild"] != DBNull.Value ? dr["KNameOfChild"].ToString() : null);
                    objCovidVaccine.KDOB = (dr["KDOB"] != DBNull.Value ? Convert.ToDateTime(dr["KDOB"].ToString()) : DateTime.MinValue);
                    objCovidVaccine.KNameOfParent = (dr["KNameOfParent"] != DBNull.Value ? dr["KNameOfParent"].ToString() : null);
                    objCovidVaccine.KMailingAddress = (dr["KMailingAddress"] != DBNull.Value ? dr["KMailingAddress"].ToString() : null);
                    objCovidVaccine.KPhoneNumber = (dr["KPhoneNumber"] != DBNull.Value ? dr["KPhoneNumber"].ToString() : null);
                    objCovidVaccine.KHaveAnInsurance = (dr["KHaveAnInsurance"] != DBNull.Value ? dr["KHaveAnInsurance"].ToString() : null);
                    objCovidVaccine.KAdditionalInformation = (dr["KAdditionalInformation"] != DBNull.Value ? dr["KAdditionalInformation"].ToString() : null);
                    objCovidVaccine.ABoosterShotSelection = (dr["ABoosterShotSelection"] != DBNull.Value ? dr["ABoosterShotSelection"].ToString() : null);
                    objCovidVaccine.AName = (dr["AName"] != DBNull.Value ? dr["AName"].ToString() : null);
                    objCovidVaccine.ADOB = (dr["ADOB"] != DBNull.Value ? Convert.ToDateTime(dr["ADOB"].ToString()) : DateTime.MinValue);
                    objCovidVaccine.AMailingAddress = (dr["AMailingAddress"] != DBNull.Value ? dr["AMailingAddress"].ToString() : null);
                    objCovidVaccine.APhoneNumber = (dr["APhoneNumber"] != DBNull.Value ? dr["APhoneNumber"].ToString() : null);
                    objCovidVaccine.APreferredTimeSlots = (dr["APreferredTimeSlots"] != DBNull.Value ? dr["APreferredTimeSlots"].ToString() : null);
                    objCovidVaccine.APrintTimeSlot = (dr["APrintTimeSlot"] != DBNull.Value ? dr["APrintTimeSlot"].ToString() : null);
                    objCovidVaccine.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objCovidVaccine.InsertedBy = dr["InsertedBy"].ToString();
                    objCovidVaccine.InsertedDate = Convert.ToDateTime(dr["InsertedDate"]);

                    objCovidVaccine.UpdatedBy = dr["UpdatedBy"].ToString();
                    objCovidVaccine.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);


                    lstCovidVaccine.Add(objCovidVaccine);
                }

            }
            return lstCovidVaccine;
        }

        public Int64 UpdateCovidVaccine(Int64 CovidVaccineId)
        {
            Int64 _status = 0;
            _status = _CovidVaccine.UpdateCovidVaccineStatus(CovidVaccineId);
            return _status;
        }
    }
}
