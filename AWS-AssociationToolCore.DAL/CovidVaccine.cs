using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
    public class CovidVaccine
    {
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        public Int64 FECovidVaccineInsert(Entities.CovidVaccine objCovidVaccine, ref Int64 CovidVaccineId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@CovidVaccineId",objCovidVaccine.CovidVaccineId),
                    new SqlParameter("@ChapterId",(objCovidVaccine.ChapterId==0?(object)DBNull.Value:objCovidVaccine.ChapterId)),
                    new SqlParameter("@MemberId",(objCovidVaccine.MemberId==0?(object)DBNull.Value:objCovidVaccine.MemberId)),
                    new SqlParameter("@Email",(objCovidVaccine.Email==null?DBNull.Value:(object)objCovidVaccine.Email)),
                    new SqlParameter("@COVIDVaccinefor",(objCovidVaccine.COVIDVaccinefor==null?DBNull.Value:(object)objCovidVaccine.COVIDVaccinefor)),
                    new SqlParameter("@KNameOfChild",(objCovidVaccine.KNameOfChild==null?DBNull.Value:(object)objCovidVaccine.KNameOfChild)),
                    new SqlParameter("@KDOB",(objCovidVaccine.KDOB==DateTime.MinValue?DBNull.Value:(object)objCovidVaccine.KDOB)),
                    new SqlParameter("@KNameOfParent",(objCovidVaccine.KNameOfParent==null?DBNull.Value:(object)objCovidVaccine.KNameOfParent)),
                    new SqlParameter("@KMailingAddress",(objCovidVaccine.KMailingAddress==null?DBNull.Value:(object)objCovidVaccine.KMailingAddress)),
                    new SqlParameter("@KPhoneNumber",(objCovidVaccine.KPhoneNumber==null?DBNull.Value:(object)objCovidVaccine.KPhoneNumber)),
                    new SqlParameter("@KHaveAnInsurance",(objCovidVaccine.KHaveAnInsurance==null?DBNull.Value:(object)objCovidVaccine.KHaveAnInsurance)),
                    new SqlParameter("@KAdditionalInformation",(objCovidVaccine.KAdditionalInformation==null?DBNull.Value:(object)objCovidVaccine.KAdditionalInformation)),
                    new SqlParameter("@ABoosterShotSelection",(objCovidVaccine.ABoosterShotSelection==null?DBNull.Value:(object)objCovidVaccine.ABoosterShotSelection)),
                    new SqlParameter("@AName",(objCovidVaccine.AName==null?DBNull.Value:(object)objCovidVaccine.AName)),
                    new SqlParameter("@ADOB",(objCovidVaccine.ADOB==DateTime.MinValue?DBNull.Value:(object)objCovidVaccine.ADOB)),
                    new SqlParameter("@AMailingAddress",(objCovidVaccine.AMailingAddress==null?DBNull.Value:(object)objCovidVaccine.AMailingAddress)),
                    new SqlParameter("@APhoneNumber",(objCovidVaccine.APhoneNumber==null?DBNull.Value:(object)objCovidVaccine.APhoneNumber)),
                    new SqlParameter("@APreferredTimeSlots",(objCovidVaccine.APreferredTimeSlots==null?DBNull.Value:(object)objCovidVaccine.APreferredTimeSlots)),
                    new SqlParameter("@APrintTimeSlot",(objCovidVaccine.APrintTimeSlot==null?DBNull.Value:(object)objCovidVaccine.APrintTimeSlot)),                    
                    new SqlParameter("@IsActive",objCovidVaccine.IsActive),
                    new SqlParameter("@InsertedBy",(objCovidVaccine.InsertedBy==null?DBNull.Value:(object)objCovidVaccine.InsertedBy)),                                        
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",(objCovidVaccine.UpdatedBy==null?DBNull.Value:(object)objCovidVaccine.UpdatedBy)),                                        
                    new SqlParameter("@UpdatedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                  
                    
                    };


                _sqlP[0].SqlDbType = SqlDbType.NVarChar;
                _sqlP[0].Size = 512;
                _sqlP[0].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[24].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CovidVaccineInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[24].Value);

                CovidVaccineId = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetCovidVaccineGetById(Int64 CovidVaccineId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CovidVaccineId",CovidVaccineId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CovidVaccineGetById", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetCovidVaccineListByVariableId(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@ChapterId",ChapterId),                    
                    new SqlParameter("@Search",Search),                    
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),                    
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",Total)
                };

                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CovidVaccineGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteCovidVaccine(Int64 CovidVaccineId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CovidVaccineId",CovidVaccineId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CovidVaccineDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateCovidVaccineStatus(Int64 CovidVaccineId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@CovidVaccineId",CovidVaccineId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("CovidVaccineStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetCovidVaccineList(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("CovidVaccineGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ExportToCovidVaccine(Int64 CovidVaccineId, string Search, string Sort)
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[] 
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@CovidVaccineId",CovidVaccineId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@Total",0)
                };
                _sqlP[3].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExporttoCOVIDVaccine", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[3].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

    }
}
