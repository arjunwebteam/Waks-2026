using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.DAL
{
    public class Events
    {
        //spdv
        DBAccess _dbAccess = new DBAccess();
        SqlParameter[] _sqlP;

        #region Admin

        public Int64 InsertEvent(AWS_AssociationToolCore.Entities.Events objEvent, ref string imageurl, ref string imageurl3)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventId",objEvent.EventId),
                    new SqlParameter("@ChapterId",(objEvent.ChapterId==0?(object)DBNull.Value:objEvent.ChapterId)),
                    new SqlParameter("@EventName",objEvent.EventName ),
                    new SqlParameter("@StartDate",(objEvent.StartDate==DateTime.MinValue?(object)DBNull.Value:objEvent.StartDate)),
                    new SqlParameter("@EndDate",(objEvent.EndDate==DateTime.MinValue?(object)DBNull.Value:objEvent.EndDate)),
                    new SqlParameter("@RegistrationStartDate",(objEvent.RegistrationStartDate==DateTime.MinValue?(object)DBNull.Value:objEvent.RegistrationStartDate)),
                    new SqlParameter("@RegistrationEndDate",(objEvent.RegistrationEndDate==DateTime.MinValue?(object)DBNull.Value:objEvent.RegistrationEndDate)),
                    new SqlParameter("@BannerUrl",(imageurl==null?(object)DBNull.Value:imageurl)),
                    new SqlParameter("@Flyer",(imageurl3==null?(object)DBNull.Value:imageurl3)),
                    new SqlParameter("@EventCategoryId",(objEvent.EventCategoryId==0?(object)DBNull.Value:objEvent.EventCategoryId)),
                    new SqlParameter("@MemberCount",(objEvent.MemberCount==0?(object)DBNull.Value:objEvent.MemberCount)),
                    new SqlParameter("@Location",(objEvent.Location==null?(object)DBNull.Value:objEvent.Location)),
                    new SqlParameter("@City",(objEvent.City==null?(object)DBNull.Value:objEvent.City)),
                    new SqlParameter("@StateName",(objEvent.StateName==null?(object)DBNull.Value:objEvent.StateName)),
                    new SqlParameter("@ZipCode",(objEvent.ZipCode==null?(object)DBNull.Value:objEvent.ZipCode)),
                    new SqlParameter("@EventDetails",(objEvent.EventDetails==null?(object)DBNull.Value:objEvent.EventDetails)),
                    new SqlParameter("@ContactEmail",(objEvent.ContactEmail==null?(object)DBNull.Value:objEvent.ContactEmail)),
                    new SqlParameter("@IsRegistration",(objEvent.IsRegistration==false?(object)DBNull.Value:objEvent.IsRegistration)),
                    new SqlParameter("@IsCulturalRegistration",(objEvent.IsCulturalRegistration==false?(object)DBNull.Value:objEvent.IsCulturalRegistration)),
                    new SqlParameter("@PageTitle",(objEvent.PageTitle==null?(object)DBNull.Value:objEvent.PageTitle)),
                    new SqlParameter("@MetaDescription",(objEvent.MetaDescription==null?(object)DBNull.Value:objEvent.MetaDescription)),
                    new SqlParameter("@MetaKeywords",(objEvent.MetaKeywords==null?(object)DBNull.Value:objEvent.MetaKeywords)),
                    new SqlParameter("@TopLine",(objEvent.TopLine==null?(object)DBNull.Value:objEvent.TopLine)),
                    new SqlParameter("@UpdatedBy",objEvent.UpdatedBy),
                    new SqlParameter("@UpdatedTime",objEvent.UpdatedTime),
                    new SqlParameter("@XMLRegistrations",(objEvent.XMLRegistrations== null ?DBNull.Value:(object)objEvent.XMLRegistrations)),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@ChapterIds",(objEvent.ChapterIds == null ?DBNull.Value:(object)objEvent.ChapterIds)),
                    new SqlParameter("@IsQrcode",(objEvent.IsQRCode == null ?DBNull.Value:(object)objEvent.IsQRCode)),
                    new SqlParameter("@XMLDates",(objEvent.XMLDates== null ?DBNull.Value:(object)objEvent.XMLDates)),
                    new SqlParameter("@XMLRegistrationsCat",(objEvent.XMLRegistrationsCat== null ?DBNull.Value:(object)objEvent.XMLRegistrationsCat)),
                    new SqlParameter("@EarlyBirdCutOfDate",(objEvent.EarlyBirdCutOfDate==DateTime.MinValue?(object)DBNull.Value:objEvent.EarlyBirdCutOfDate)),
                    new SqlParameter("@IsEarlyBird",(objEvent.IsEarlyBird==false?(object)DBNull.Value:objEvent.IsEarlyBird)),
                    new SqlParameter("@IsMemberAmount",(objEvent.IsMemberAmount==false?(object)DBNull.Value:objEvent.IsMemberAmount)),
                    new SqlParameter("@IsNonMemberAmount",(objEvent.IsNonMemberAmount==false?(object)DBNull.Value:objEvent.IsNonMemberAmount)),
                       new SqlParameter("@IsActive",objEvent.IsActive),

                };

                _sqlP[7].SqlDbType = SqlDbType.NVarChar;
                _sqlP[7].Size = 256;
                _sqlP[7].Direction = System.Data.ParameterDirection.InputOutput;
                _sqlP[8].SqlDbType = SqlDbType.NVarChar;
                _sqlP[8].Size = 256;
                _sqlP[8].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[26].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[26].Value);

                imageurl = _sqlP[7].Value.ToString();
                imageurl3 = _sqlP[8].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
        public DataSet GetEventById(Int64 EventId, string EventName, ref int Status)
        {
            DataSet dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@EventName",EventName),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataSet("EventsGetById", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[2].Value);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataSet GetEventByIdFE(Int64 EventId, string EventName, string Email, ref int Status)
        {
            DataSet dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@EventName",EventName),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@Email",Email)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataSet("EventsGetByIdFE", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[2].Value);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteEventById(Int64 EventId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventsDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetEventsListByVariable(Int64 MemberId, Int64 EventStatusId, string EventType, string StartDate, string EndDate, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventStatusId",EventStatusId),
                    new SqlParameter("@EventType",EventType),
                    new SqlParameter("@StartDate",StartDate),
                    new SqlParameter("@EndDate",EndDate),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@MemberId",MemberId),


                };
                _sqlP[8].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[8].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetApiEventsListByVariable(Int64 MemberId, Int64 EventStatusId, string EventType, string StartDate, string EndDate, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventStatusId",EventStatusId),
                    new SqlParameter("@EventType",EventType),
                    new SqlParameter("@StartDate",StartDate),
                    new SqlParameter("@EndDate",EndDate),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@MemberId",MemberId),


                };
                _sqlP[8].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ApiEventsGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[8].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventsList(string Type, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Type",Type),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    new SqlParameter("@ChapterId",ChapterId)
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventsGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable FEGetEventsList(Int64 ChapterId, string Type, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Type",Type),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0),
                    new SqlParameter("@ChapterId",ChapterId)
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEEventsGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable RegistrationGetEventsList(ref int Status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {

                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("RegistrationEventsGetList", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdateEventStatus(Int64 EventId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertEventUserInfo(AWS_AssociationToolCore.Entities.EventUserInfo objEventUserInfo, ref Int64 EventUserInfoId, ref string DocumentUrl, ref string receipturl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventId",objEventUserInfo.EventId),
                    new SqlParameter("@EventUserInfoId",EventUserInfoId),
                     new SqlParameter("@ReceiptUrl",receipturl),
                    new SqlParameter("@FirstName",objEventUserInfo.FirstName ),
                    new SqlParameter("@LastName",objEventUserInfo.LastName),
                    new SqlParameter("@Email",objEventUserInfo.Email),
                    new SqlParameter("@MemberId",(objEventUserInfo.MemberId==0?(object)DBNull.Value:objEventUserInfo.MemberId)),
                    new SqlParameter("@Gender",(objEventUserInfo.Gender==null?(object)DBNull.Value:objEventUserInfo.Gender)),
                    new SqlParameter("@Address",(objEventUserInfo.Address==null?(object)DBNull.Value:objEventUserInfo.Address)),
                    new SqlParameter("@City",(objEventUserInfo.City==null?(object)DBNull.Value:objEventUserInfo.City)),
                    new SqlParameter("@State",(objEventUserInfo.State==null?(object)DBNull.Value:objEventUserInfo.State)),
                    new SqlParameter("@Mobile",(objEventUserInfo.Mobile==null?(object)DBNull.Value:objEventUserInfo.Mobile)),
                    new SqlParameter("@ItemName",(objEventUserInfo.ItemName==null?(object)DBNull.Value:objEventUserInfo.ItemName)),
                    new SqlParameter("@ItemCategory",(objEventUserInfo.ItemCategory==null?(object)DBNull.Value:objEventUserInfo.ItemCategory)),
                    new SqlParameter("@ItemDescription",(objEventUserInfo.ItemDescription==null?(object)DBNull.Value:objEventUserInfo.ItemDescription)),
                    new SqlParameter("@ItemDuration",(objEventUserInfo.ItemDuration==0?(object)DBNull.Value:objEventUserInfo.ItemDuration)),
                    new SqlParameter("@IsApproved",objEventUserInfo.IsApproved),
                    new SqlParameter("@ApprovedDate",DateTime.UtcNow),
                    new SqlParameter("@DocumentUrl",DocumentUrl),
                    new SqlParameter("@AmountPaid",(objEventUserInfo.AmountPaid==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AmountPaid)),
                    new SqlParameter("@ChildCount",(objEventUserInfo.ChildCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.ChildCount)),
                    new SqlParameter("@AdultCount",(objEventUserInfo.AdultCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AdultCount)),
                    new SqlParameter("@CoupleCount",(objEventUserInfo.CoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.CoupleCount)),
                    new SqlParameter("@FamilyCount",(objEventUserInfo.FamilyCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.FamilyCount)),
                    new SqlParameter("@VIPCount",(objEventUserInfo.VIPCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPCount)),
                    new SqlParameter("@VIPSingleAdultCount",(objEventUserInfo.VIPSingleAdultCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPSingleAdultCount)),
                    new SqlParameter("@VIPCoupleCount",(objEventUserInfo.VIPCoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPCoupleCount)),
                    new SqlParameter("@VIPChildCount",(objEventUserInfo.VIPChildCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPChildCount)),
                    new SqlParameter("@PaymentStatusId",(objEventUserInfo.PaymentStatusId==0?(object)DBNull.Value:objEventUserInfo.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objEventUserInfo.PaymentMethodId==0?(object)DBNull.Value:objEventUserInfo.PaymentMethodId)),
                    new SqlParameter("@Comments",(objEventUserInfo.Comments==null?(object)DBNull.Value:objEventUserInfo.Comments)),
                    new SqlParameter("@OtherPaymentId",(objEventUserInfo.OtherPaymentId==null?(object)DBNull.Value:objEventUserInfo.OtherPaymentId)),
                    new SqlParameter("@PaymentMethod",(objEventUserInfo.PaymentMethod==null?(object)DBNull.Value:objEventUserInfo.PaymentMethod)),
                    new SqlParameter("@PaymentStatus",(objEventUserInfo.PaymentStatus==null?(object)DBNull.Value:objEventUserInfo.PaymentStatus)),
                    new SqlParameter("@InsertedBy",objEventUserInfo.InsertedBy),
                    new SqlParameter("@UpdatedBy",objEventUserInfo.UpdatedBy),
                    new SqlParameter("@InsertedTime",objEventUserInfo.InsertedTime),
                     new SqlParameter("@UpdatedTime",objEventUserInfo.UpdatedTime),

                    //new SqlParameter("@UpdatedTime",TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"))),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@XMLRegistrationsCounts",(objEventUserInfo.XMLRegistrationsCounts==null?(object)DBNull.Value:objEventUserInfo.XMLRegistrationsCounts)),
                    new SqlParameter("@Zelle",(objEventUserInfo.Zelle  == null?DBNull.Value: (object)objEventUserInfo.Zelle)),
                    new SqlParameter("@ZellePartnerName",(objEventUserInfo.ZellePartnerName  == null?DBNull.Value: (object)objEventUserInfo.ZellePartnerName)),
                    new SqlParameter("@ChequeHolderName",(objEventUserInfo.ChequeHolderName  == null?DBNull.Value: (object)objEventUserInfo.ChequeHolderName)),


                };

                _sqlP[18].SqlDbType = SqlDbType.NVarChar;
                _sqlP[18].Size = 512;
                _sqlP[18].Direction = System.Data.ParameterDirection.InputOutput;


                _sqlP[2].SqlDbType = SqlDbType.NVarChar;
                _sqlP[2].Size = 256;
                _sqlP[2].Direction = System.Data.ParameterDirection.InputOutput;


                _sqlP[1].Direction = System.Data.ParameterDirection.InputOutput;
                _sqlP[38].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("FEInsertEventUserInfo", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[38].Value);

                EventUserInfoId = Convert.ToInt64(_sqlP[1].Value);

                DocumentUrl = _sqlP[18].Value.ToString();

                receipturl = _sqlP[2].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventUserPaymentInfo(AWS_AssociationToolCore.Entities.EventUserInfo objEventUserInfo)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventId",objEventUserInfo.EventId),
                    new SqlParameter("@EventUserInfoId",objEventUserInfo.EventUserInfoId),
                    new SqlParameter("@AmountPaid",(objEventUserInfo.AmountPaid==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AmountPaid)),
                    new SqlParameter("@PaymentStatusId",(objEventUserInfo.PaymentStatusId==0?(object)DBNull.Value:objEventUserInfo.PaymentStatusId)),
                    new SqlParameter("@PaymentMethodId",(objEventUserInfo.PaymentMethodId==0?(object)DBNull.Value:objEventUserInfo.PaymentMethodId)),
                    new SqlParameter("@PaymentMethod",(objEventUserInfo.PaymentMethod==null?(object)DBNull.Value:objEventUserInfo.PaymentMethod)),
                    new SqlParameter("@PaymentStatus",(objEventUserInfo.PaymentStatus==null?(object)DBNull.Value:objEventUserInfo.PaymentStatus)),
                    new SqlParameter("@TransactionId",(objEventUserInfo.TransactionId==null?(object)DBNull.Value:objEventUserInfo.TransactionId )),
                    new SqlParameter("@UpdatedBy",objEventUserInfo.UpdatedBy),
                    new SqlParameter("@PaymentDate",objEventUserInfo.PaymentDate),
                    new SqlParameter("@UpdatedTime",objEventUserInfo.UpdatedTime),
                    new SqlParameter("@Email",(objEventUserInfo.Email==null?(object)DBNull.Value:objEventUserInfo.Email)),
                    new SqlParameter("@QStatus",0),

                    };


                _sqlP[12].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("UpdateEventUserPaymentInfo", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[12].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
        public Int64 InsertEventUserInfo(AWS_AssociationToolCore.Entities.EventUserInfo objEventUserInfo, ref string receipturl)
        {
            Int64 _status = 0;
            try
            {

                _sqlP = new[]
                    {
                    new SqlParameter("@EventUserInfoId",objEventUserInfo.EventUserInfoId),
                    new SqlParameter("@MemberId",(objEventUserInfo.MemberId==0?(object)DBNull.Value:objEventUserInfo.MemberId)),

                    new SqlParameter("@FirstName",objEventUserInfo.FirstName ),
                    new SqlParameter("@LastName",objEventUserInfo.LastName),
                    new SqlParameter("@Email",objEventUserInfo.Email),
                     new SqlParameter("@ApprovedDate",(objEventUserInfo.ApprovedDate==DateTime.MinValue?(object)DBNull.Value:objEventUserInfo.ApprovedDate)),
                       new SqlParameter("@Comments",(objEventUserInfo.Comments==null?(object)DBNull.Value:objEventUserInfo.Comments)),
                    new SqlParameter("@OtherPaymentId",(objEventUserInfo.OtherPaymentId==null?(object)DBNull.Value:objEventUserInfo.OtherPaymentId)),

                    new SqlParameter("@Gender",(objEventUserInfo.Gender==null?(object)DBNull.Value:objEventUserInfo.Gender)),
                    new SqlParameter("@Address",(objEventUserInfo.Address==null?(object)DBNull.Value:objEventUserInfo.Address)),
                    new SqlParameter("@City",(objEventUserInfo.City==null?(object)DBNull.Value:objEventUserInfo.City)),
                    new SqlParameter("@State",(objEventUserInfo.State==null?(object)DBNull.Value:objEventUserInfo.State)),
                    new SqlParameter("@Mobile",(objEventUserInfo.Mobile==null?(object)DBNull.Value:objEventUserInfo.Mobile)),
                    new SqlParameter("@ItemName",(objEventUserInfo.ItemName==null?(object)DBNull.Value:objEventUserInfo.ItemName)),
                    new SqlParameter("@ItemCategory",(objEventUserInfo.ItemCategory==null?(object)DBNull.Value:objEventUserInfo.ItemCategory)),
                    new SqlParameter("@ItemDescription",(objEventUserInfo.ItemDescription==null?(object)DBNull.Value:objEventUserInfo.ItemDescription)),
                    new SqlParameter("@ItemDuration",(objEventUserInfo.ItemDuration==0?(object)DBNull.Value:objEventUserInfo.ItemDuration)),
                    new SqlParameter("@IsApproved",objEventUserInfo.IsApproved),
                    new SqlParameter("@DocumentUrl",(objEventUserInfo.DocumentUrl==null?(object)DBNull.Value:objEventUserInfo.DocumentUrl)),
                    new SqlParameter("@UpdatedBy",objEventUserInfo.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@EventOrderId",objEventUserInfo.EventOrderId),
                    new SqlParameter("@EventId",objEventUserInfo.EventId),
                    new SqlParameter("@PaymentStatusId",(objEventUserInfo.PaymentStatusId==0?(object)DBNull.Value:objEventUserInfo.PaymentStatusId)),
                    new SqlParameter("@PaymentDate",(objEventUserInfo.PaymentDate==DateTime.MinValue?(object)DBNull.Value:objEventUserInfo.PaymentDate)),
                    new SqlParameter("@PaymentMethodId",(objEventUserInfo.PaymentMethodId==0?(object)DBNull.Value:objEventUserInfo.PaymentMethodId)),
                    new SqlParameter("@TransactionId",(objEventUserInfo.TransactionId==null?(object)DBNull.Value:objEventUserInfo.TransactionId)),
                    new SqlParameter("@AmountPaid",(objEventUserInfo.AmountPaid==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AmountPaid)),
                    new SqlParameter("@ChildCount",(objEventUserInfo.ChildCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.ChildCount)),
                    new SqlParameter("@AdultCount",(objEventUserInfo.AdultCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AdultCount)),
                    new SqlParameter("@CoupleCount",(objEventUserInfo.CoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.CoupleCount)),
                    new SqlParameter("@FamilyCount",(objEventUserInfo.FamilyCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.FamilyCount)),
                    new SqlParameter("@VIPCount",(objEventUserInfo.VIPCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPCount)),
                     new SqlParameter("@VIPSingleAdultCount",(objEventUserInfo.VIPSingleAdultCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPSingleAdultCount)),
                    new SqlParameter("@VIPCoupleCount",(objEventUserInfo.VIPCoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPCoupleCount)),
                    new SqlParameter("@VIPChildCount",(objEventUserInfo.VIPChildCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPChildCount)),
                    new SqlParameter("@IsAttended",objEventUserInfo.IsAttended),
                    new SqlParameter("@InsertedBy",objEventUserInfo.InsertedBy),
                    new SqlParameter("@InsertedDate",objEventUserInfo.InsertedDate),
                    new SqlParameter("@QStatus",0),

                    new SqlParameter("@ReceiptUrl",receipturl),
                    new SqlParameter("@InsertedTime",DateTime.UtcNow),
                     new SqlParameter("@Zelle",(objEventUserInfo.Zelle  == null?DBNull.Value: (object)objEventUserInfo.Zelle)),
                    new SqlParameter("@ZellePartnerName",(objEventUserInfo.ZellePartnerName  == null?DBNull.Value: (object)objEventUserInfo.ZellePartnerName)),
                     new SqlParameter("@XMLRegistrationsCounts",(objEventUserInfo.XMLRegistrationsCounts==null?(object)DBNull.Value:objEventUserInfo.XMLRegistrationsCounts)),

                    new SqlParameter("@ChequeHolderName",(objEventUserInfo.ChequeHolderName  == null?DBNull.Value: (object)objEventUserInfo.ChequeHolderName)),
                    };

                _sqlP[40].SqlDbType = SqlDbType.NVarChar;
                _sqlP[40].Size = 256;
                _sqlP[40].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[39].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventrigistrationsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[39].Value);




                receipturl = _sqlP[36].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventUserInfo(AWS_AssociationToolCore.Entities.EventUserInfo objEventUserInfo, ref string receipturl)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    //new SqlParameter("@EventUserInfoId",objEventUserInfo.EventUserInfoId),
                    new SqlParameter("@EventUserInfoId",(objEventUserInfo.EventUserInfoId==0?(object)DBNull.Value:objEventUserInfo.EventUserInfoId)),

                    new SqlParameter("@FirstName",objEventUserInfo.FirstName ),
                    new SqlParameter("@LastName",objEventUserInfo.LastName),
                    new SqlParameter("@Email",objEventUserInfo.Email),
                    new SqlParameter("@Gender",(objEventUserInfo.Gender==null?(object)DBNull.Value:objEventUserInfo.Gender)),
                    new SqlParameter("@Address",(objEventUserInfo.Address==null?(object)DBNull.Value:objEventUserInfo.Address)),
                    new SqlParameter("@City",(objEventUserInfo.City==null?(object)DBNull.Value:objEventUserInfo.City)),
                    new SqlParameter("@State",(objEventUserInfo.State==null?(object)DBNull.Value:objEventUserInfo.State)),
                    new SqlParameter("@Mobile",(objEventUserInfo.Mobile==null?(object)DBNull.Value:objEventUserInfo.Mobile)),
                    new SqlParameter("@ItemName",(objEventUserInfo.ItemName==null?(object)DBNull.Value:objEventUserInfo.ItemName)),
                    new SqlParameter("@ItemCategory",(objEventUserInfo.ItemCategory==null?(object)DBNull.Value:objEventUserInfo.ItemCategory)),
                    new SqlParameter("@ItemDescription",(objEventUserInfo.ItemDescription==null?(object)DBNull.Value:objEventUserInfo.ItemDescription)),
                    new SqlParameter("@ItemDuration",(objEventUserInfo.ItemDuration==0?(object)DBNull.Value:objEventUserInfo.ItemDuration)),
                    new SqlParameter("@IsApproved",objEventUserInfo.IsApproved),
                    new SqlParameter("@DocumentUrl",(objEventUserInfo.DocumentUrl==null?(object)DBNull.Value:objEventUserInfo.DocumentUrl)),
                    new SqlParameter("@UpdatedBy",objEventUserInfo.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@EventOrderId",(objEventUserInfo.EventOrderId==0?(object)DBNull.Value:objEventUserInfo.EventOrderId)),
                                        new SqlParameter("@EventId",(objEventUserInfo.EventId==0?(object)DBNull.Value:objEventUserInfo.EventId)),

                    new SqlParameter("@PaymentStatusId",(objEventUserInfo.PaymentStatusId==0?(object)DBNull.Value:objEventUserInfo.PaymentStatusId)),
                    new SqlParameter("@PaymentDate",(objEventUserInfo.PaymentDate==DateTime.MinValue?(object)DBNull.Value:objEventUserInfo.PaymentDate)),
                    new SqlParameter("@PaymentMethodId",(objEventUserInfo.PaymentMethodId==0?(object)DBNull.Value:objEventUserInfo.PaymentMethodId)),
                    new SqlParameter("@TransactionId",(objEventUserInfo.TransactionId==null?(object)DBNull.Value:objEventUserInfo.TransactionId)),
                    new SqlParameter("@AmountPaid",(objEventUserInfo.AmountPaid==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AmountPaid)),
                    new SqlParameter("@ChildCount",(objEventUserInfo.ChildCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.ChildCount)),
                    new SqlParameter("@AdultCount",(objEventUserInfo.AdultCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.AdultCount)),
                    new SqlParameter("@CoupleCount",(objEventUserInfo.CoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.CoupleCount)),
                    new SqlParameter("@FamilyCount",(objEventUserInfo.FamilyCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.FamilyCount)),
                    new SqlParameter("@VIPCount",(objEventUserInfo.VIPCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPCount)),
                     new SqlParameter("@VIPSingleAdultCount",(objEventUserInfo.VIPSingleAdultCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPSingleAdultCount)),
                    new SqlParameter("@VIPCoupleCount",(objEventUserInfo.VIPCoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPCoupleCount)),
                    new SqlParameter("@VIPChildCount",(objEventUserInfo.VIPChildCount==Decimal.MinValue?(object)DBNull.Value:objEventUserInfo.VIPChildCount)),
                    new SqlParameter("@IsAttended",objEventUserInfo.IsAttended),
                    new SqlParameter("@InsertedBy",objEventUserInfo.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@ReceiptUrl",receipturl),
                     new SqlParameter("@Zelle",(objEventUserInfo.Zelle  == null?DBNull.Value: (object)objEventUserInfo.Zelle)),
                    new SqlParameter("@ZellePartnerName",(objEventUserInfo.ZellePartnerName  == null?DBNull.Value: (object)objEventUserInfo.ZellePartnerName)),
                    new SqlParameter("@ChequeHolderName",(objEventUserInfo.ChequeHolderName  == null?DBNull.Value: (object)objEventUserInfo.ChequeHolderName)),
                    };

                _sqlP[36].SqlDbType = SqlDbType.NVarChar;
                _sqlP[36].Size = 256;
                _sqlP[36].Direction = System.Data.ParameterDirection.InputOutput;

                _sqlP[35].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventUserInfoUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[35].Value);




                receipturl = _sqlP[36].Value.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertEventParticipant(AWS_AssociationToolCore.Entities.EventParticipants objEventParticipant)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventParticipantId",objEventParticipant.EventParticipantId),
                    new SqlParameter("@EventId",objEventParticipant.EventId),
                    new SqlParameter("@EventUserInfoId",objEventParticipant.EventUserInfoId ),
                    new SqlParameter("@FirstName",objEventParticipant.FirstName),
                    new SqlParameter("@LastName",objEventParticipant.LastName),
                    new SqlParameter("@Email",(objEventParticipant.Email==null?(object)DBNull.Value:objEventParticipant.Email)),
                    new SqlParameter("@Age",(objEventParticipant.Age==null?(object)DBNull.Value:objEventParticipant.Age)),
                    new SqlParameter("@Mobile",(objEventParticipant.Mobile==null?(object)DBNull.Value:objEventParticipant.Mobile)),
                    new SqlParameter("@IsApproved",objEventParticipant.IsApproved),
                    new SqlParameter("@UpdatedBy",objEventParticipant.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventParticipantsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventParticipant(AWS_AssociationToolCore.Entities.EventParticipants objEventParticipant)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventParticipantId",objEventParticipant.EventParticipantId),
                    new SqlParameter("@EventId",objEventParticipant.EventId),
                    new SqlParameter("@EventUserInfoId",objEventParticipant.EventUserInfoId ),
                    new SqlParameter("@FirstName",objEventParticipant.FirstName),
                    new SqlParameter("@LastName",objEventParticipant.LastName),
                    new SqlParameter("@Email",objEventParticipant.Email),
                    new SqlParameter("@Age",(objEventParticipant.Age==null?(object)DBNull.Value:objEventParticipant.Age)),
                    new SqlParameter("@Mobile",(objEventParticipant.Mobile==null?(object)DBNull.Value:objEventParticipant.Mobile)),
                    new SqlParameter("@IsApproved",objEventParticipant.IsApproved),
                    new SqlParameter("@UpdatedBy",objEventParticipant.UpdatedBy),
                    new SqlParameter("@UpdatedTime",objEventParticipant.UpdatedTime),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventParticipantsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 InsertEventOrderDetail(AWS_AssociationToolCore.Entities.EventOrderDetails objEventOrderDetail)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventOrderId",objEventOrderDetail.EventOrderId),
                    new SqlParameter("@EventUserInfoId",objEventOrderDetail.EventUserInfoId),
                    new SqlParameter("@EventId",objEventOrderDetail.EventId ),
                    new SqlParameter("@IsApproved",(objEventOrderDetail.IsApproved==false?(object)DBNull.Value:objEventOrderDetail.IsApproved)),
                    new SqlParameter("@ApprovedDate",(objEventOrderDetail.ApprovedDate==DateTime.MinValue?(object)DBNull.Value:objEventOrderDetail.ApprovedDate)),
                    new SqlParameter("@PaymentStatusId",(objEventOrderDetail.PaymentStatusId==0?(object)DBNull.Value:objEventOrderDetail.PaymentStatusId)),
                    new SqlParameter("@PaymentDate",(objEventOrderDetail.PaymentDate==DateTime.MinValue?(object)DBNull.Value:objEventOrderDetail.PaymentDate)),
                    new SqlParameter("@PaymentMethodId",(objEventOrderDetail.PaymentMethodId==0?(object)DBNull.Value:objEventOrderDetail.PaymentMethodId)),
                    new SqlParameter("@TransactionId",(objEventOrderDetail.TransactionId==null?(object)DBNull.Value:objEventOrderDetail.TransactionId)),
                    new SqlParameter("@PaymentMethod",(objEventOrderDetail.PaymentMethod==null?(object)DBNull.Value:objEventOrderDetail.PaymentMethod)),
                    new SqlParameter("@PaymentStatus",(objEventOrderDetail.PaymentStatus==null?(object)DBNull.Value:objEventOrderDetail.PaymentStatus)),
                    new SqlParameter("@AmountPaid",(objEventOrderDetail.AmountPaid==Decimal.MinValue?(object)DBNull.Value:objEventOrderDetail.AmountPaid)),
                    new SqlParameter("@ChildCount",(objEventOrderDetail.ChildCount==0?(object)DBNull.Value:objEventOrderDetail.ChildCount)),
                    new SqlParameter("@AdultCount",(objEventOrderDetail.AdultCount==0?(object)DBNull.Value:objEventOrderDetail.AdultCount)),
                    new SqlParameter("@CoupleCount",(objEventOrderDetail.CoupleCount==0?(object)DBNull.Value:objEventOrderDetail.CoupleCount)),
                    new SqlParameter("@FamilyCount",(objEventOrderDetail.FamilyCount==0?(object)DBNull.Value:objEventOrderDetail.FamilyCount)),
                    new SqlParameter("@VIPCount",(objEventOrderDetail.VIPCount==0?(object)DBNull.Value:objEventOrderDetail.VIPCount)),
                     new SqlParameter("@VIPSingleAdultCount",(objEventOrderDetail.VIPSingleAdultCount==Decimal.MinValue?(object)DBNull.Value:objEventOrderDetail.VIPSingleAdultCount)),
                    new SqlParameter("@VIPCoupleCount",(objEventOrderDetail.VIPCoupleCount==Decimal.MinValue?(object)DBNull.Value:objEventOrderDetail.VIPCoupleCount)),
                    new SqlParameter("@VIPChildCount",(objEventOrderDetail.VIPChildCount==Decimal.MinValue?(object)DBNull.Value:objEventOrderDetail.VIPChildCount)),
                    new SqlParameter("@IsAttended",(objEventOrderDetail.IsAttended==false?(object)DBNull.Value:objEventOrderDetail.IsAttended)),
                    new SqlParameter("@InsertedBy",objEventOrderDetail.InsertedBy),
                    new SqlParameter("@InsertedDate",DateTime.UtcNow),
                    new SqlParameter("@UpdatedBy",objEventOrderDetail.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[25].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventOrderDetailsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[25].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventOrderDetail(AWS_AssociationToolCore.Entities.EventOrderDetails objEventOrderDetail)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventOrderId",objEventOrderDetail.EventOrderId),
                    new SqlParameter("@EventUserInfoId",objEventOrderDetail.EventUserInfoId),
                    new SqlParameter("@EventId",objEventOrderDetail.EventId ),
                    new SqlParameter("@IsApproved",(objEventOrderDetail.IsApproved==false?(object)DBNull.Value:objEventOrderDetail.IsApproved)),
                    new SqlParameter("@ApprovedDate",(objEventOrderDetail.ApprovedDate==DateTime.MinValue?(object)DBNull.Value:objEventOrderDetail.ApprovedDate)),
                    new SqlParameter("@PaymentStatusId",(objEventOrderDetail.PaymentStatusId==0?(object)DBNull.Value:objEventOrderDetail.PaymentStatusId)),
                    new SqlParameter("@PaymentDate",(objEventOrderDetail.PaymentDate==DateTime.MinValue?(object)DBNull.Value:objEventOrderDetail.PaymentDate)),
                    new SqlParameter("@PaymentMethodId",(objEventOrderDetail.PaymentMethodId==0?(object)DBNull.Value:objEventOrderDetail.PaymentMethodId)),
                    new SqlParameter("@TransactionId",(objEventOrderDetail.TransactionId==null?(object)DBNull.Value:objEventOrderDetail.TransactionId)),
                    new SqlParameter("@AmountPaid",(objEventOrderDetail.AmountPaid==Decimal.MinValue?(object)DBNull.Value:objEventOrderDetail.AmountPaid)),
                    new SqlParameter("@IsAttended",(objEventOrderDetail.IsAttended==false?(object)DBNull.Value:objEventOrderDetail.IsAttended)),
                    new SqlParameter("@UpdatedBy",objEventOrderDetail.UpdatedBy),
                    new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                    new SqlParameter("@QStatus",0)
                    };


                _sqlP[13].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventOrderDetailsUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[13].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetEventsListall(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventGetListAll", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventsList(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetUpcomingEventsList(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("UpcomingEventGetList", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEnquiryEventsListall(ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EnquiryEventGetListAll", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 InsertRegistrationTypes(AWS_AssociationToolCore.Entities.EventRegistrationTypes objRegistrationTypes)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                   new SqlParameter("@EventRegistrationTypeId",(objRegistrationTypes.EventRegistrationTypeId!= 0?(object)objRegistrationTypes.EventRegistrationTypeId:DBNull.Value.ToString())),
                    new SqlParameter("@EventId",objRegistrationTypes.EventId),
                    new SqlParameter("@RegistrationTitle",objRegistrationTypes.RegistrationTitle),
                    new SqlParameter("@MemberAmount",(objRegistrationTypes.MemberAmount == 0 ?DBNull.Value:(object)objRegistrationTypes.MemberAmount)),
                     new SqlParameter("@NonMemberAmount",(objRegistrationTypes.NonMemberAmount == 0 ?DBNull.Value:(object)objRegistrationTypes.NonMemberAmount)),

                    new SqlParameter("@RegCount",(objRegistrationTypes.RegCount!= 0?(object)objRegistrationTypes.RegCount:DBNull.Value.ToString())),
                     new SqlParameter("@OrderNo",(objRegistrationTypes.OrderNo==0?(object)DBNull.Value:objRegistrationTypes.OrderNo)),
                     new SqlParameter("@IsActive",objRegistrationTypes.IsActive),
                     new SqlParameter("@UpdatedBy",objRegistrationTypes.UpdatedBy),
                     new SqlParameter("@UpdatedTime",DateTime.UtcNow),
                     new SqlParameter("@MemberCount",(objRegistrationTypes.MemberCount!= 0?(object)objRegistrationTypes.MemberCount:DBNull.Value.ToString())),
                     new SqlParameter("@QStatus",0),
                     new SqlParameter("@EarlyBirdMemberAmount",(objRegistrationTypes.EarlyBirdMemberAmount == 0 ?DBNull.Value:(object)objRegistrationTypes.EarlyBirdMemberAmount)),
                     new SqlParameter("@EarlyBirdNonMemberAmount",(objRegistrationTypes.EarlyBirdNonMemberAmount == 0 ?DBNull.Value:(object)objRegistrationTypes.EarlyBirdNonMemberAmount)),
                };
                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventRegistrationTypesInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

        #region EventUserInfo

        public Int64 DeleteEventUserInfoById(Int64 EventUserInfoId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventUserInfoId",EventUserInfoId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventUserInfoDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataSet GetEventUserInfoFullDetailsById(Int64 EventUserInfoId, Int64 EventId, ref int Status)
        {
            DataSet ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventUserInfoId",EventUserInfoId),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EventId",EventId)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataSet("EventUserInfoFullDetailsGetById", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }

        public DataTable GetEventUserInfoById(Int64 EventUserInfoId, ref int Status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventUserInfoId",EventUserInfoId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventUserInfoGetById", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventUserInfoListByVariable(Int64 EventId, string EventCategory, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@EventCategory",EventCategory),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };
                _sqlP[6].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventUserInfoGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[6].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable RegistrationGetEventUserInfoListByVariable(Int64 EventId, string EventCategory, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@EventCategory",EventCategory),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };
                _sqlP[6].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("RegistrationEventUserInfoGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[6].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable ExportToEventUserInfoList(Int64 EventId, string Search, string Sort)
        {
            DataTable dt = null;
            Int32 Total = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@Total",0)
                };
                _sqlP[3].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("ExporttoEventUserInfo", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[3].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 UpdateEventUserInfoStatus(Int64 EventUserInfoId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventUserInfoId",EventUserInfoId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventUserInfoUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

        #region  EventRegistration Types

        public DataTable GetEventRegistrationTypesList(Int64 EventId, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Qstatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventRegistrationTypesGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataTable GetEventsDatesList(Int64 EventId, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Qstatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventDatesGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 DeleteEventRegistrationTypes(Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventRegistrationTypeId",EventRegistrationTypeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventRegistrationTypesDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 DeleteEventDates(Int64 EventDatesId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventDatesId",EventDatesId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventDatesDelete", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventRegistrationTypesStatus(Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventRegistrationTypeId",EventRegistrationTypeId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventRegistrationTypesUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public Int64 UpdateEventRegistrationTypesDisplayOrder(int OrderNo, Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventRegistrationTypeId",EventRegistrationTypeId),
                    new SqlParameter("@OrderNo",OrderNo),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventRegistrationTypesUpdateDisplayOrder", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable FEGetEventMemberInfoListByVariable(Int64 MemberId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@MemberId",MemberId),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEGetEventMemberInfoListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion

        #region

        public DataSet FEGetEventById(Int64 EventId, string EventName, ref int Status)
        {
            DataSet dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@EventName",EventName),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataSet("FEEventsGetById", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public DataTable FEGetEventsList(string Type, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Type",Type),
                    new SqlParameter("@Search",Search),
                    new SqlParameter("@Sort",Sort),
                    new SqlParameter("@PageNo",PageNo),
                    new SqlParameter("@Items",Items),
                    new SqlParameter("@Total",0)
                };
                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("FEEventsGetList", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[5].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public DataSet FEGetEventByCategoryId(Int64 EventId, Int64 RegistrationCategoryId, string EventName, ref int Status)
        {
            DataSet dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@RegistrationCategoryId",RegistrationCategoryId),
                    new SqlParameter("@EventName",EventName),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[3].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataSet("FEGetEventByCategoryId", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[3].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        #endregion

        #region EventTicketMaster

        public DataTable EventTicketMasterGetListByVariable(Int64 EventId, string Mobile, string Search, string Sort, int PageNo, int Items, ref int Total)
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
                    new SqlParameter("@Total",0),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Mobile",Mobile)
                };
                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventTicketMasterGetListByVariable", ref _sqlP);
                Total = Convert.ToInt32(_sqlP[4].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        public Int64 EventTicketMasterVisitCountUpdate(int VisitCount, Int64 EventRegCountId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventRegCountId",EventRegCountId),
                    new SqlParameter("@VisitCount",VisitCount),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventTicketMasterVisitCountUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        public DataTable GetEventUserInfoFullDetailsByMobile(Int64 EventUserInfoId, Int64 EventId, string Mobile, string Search, ref int Status)
        {
            DataTable ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventUserInfoId",EventUserInfoId),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@Mobile",Mobile),
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@Search",Search)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataTable("EventUserInfoFullDetailsGetByMobile", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }


        public DataTable GetEventsByCategory(Int64 RegistrationCategoryId, ref int status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@RegistrationCategoryId",RegistrationCategoryId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("GetEventsByCategory", ref _sqlP);
                status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        #endregion

        #region EventDates

        public Int64 UpdateEventDates(AWS_AssociationToolCore.Entities.EventDates objEventDates)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventDatesId",(objEventDates.EventDatesId!= 0?(object)objEventDates.EventDatesId:DBNull.Value.ToString())),
                     new SqlParameter("@StartDate",(objEventDates.StartDate== DateTime.MinValue?(object)DBNull.Value:objEventDates.StartDate)),
                     new SqlParameter("@EndDate",(objEventDates.EndDate== DateTime.MinValue?(object)DBNull.Value:objEventDates.EndDate)),
                     new SqlParameter("@QStatus",0)
                    };
                _sqlP[3].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventDatesUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[3].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }

        #endregion

        public DataSet EventRegistrationsAddRequirements(ref int status)
        {
            DataSet dt = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataSet("EventRegistrationsAddRequirements", ref _sqlP);
                status = Convert.ToInt32(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }




        public Int64 InsertEventComments(AWS_AssociationToolCore.Entities.EventComments objEventCom)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventCommentId",objEventCom.EventCommentId),
                    new SqlParameter("@EventUserInfoId",objEventCom.EventUserInfoId ),
                    new SqlParameter("@Status",(objEventCom.Status==null?(object)DBNull.Value:objEventCom.Status)),
                    new SqlParameter("@Comment",(objEventCom.Comment==null?(object)DBNull.Value:objEventCom.Comment)),
                    new SqlParameter("@InsertedBy",(objEventCom.InsertedBy==null?(object)DBNull.Value:objEventCom.InsertedBy)),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EmployeeRefUrl",(objEventCom.EmployeeRefUrl==null?(object)DBNull.Value:objEventCom.EmployeeRefUrl)),
                    new SqlParameter("@EmployeeCode",(objEventCom.EmployeeCode==null?(object)DBNull.Value:objEventCom.EmployeeCode)),
                 
                };



                _sqlP[5].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCommentsInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[5].Value);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
        public Int64 UpdateEventRegTicketsstatus(AWS_AssociationToolCore.Entities.EventComments objEventCom)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@CheckinCount",(objEventCom.CheckinCount==0?(object)DBNull.Value:objEventCom.CheckinCount)),
                    new SqlParameter("@EventRegistrationTypeId",(objEventCom.EventRegistrationTypeId==0?(object)DBNull.Value:objEventCom.EventRegistrationTypeId)),
                    new SqlParameter("@EventUserInfoId",(objEventCom.EventUserInfoId==0?(object)DBNull.Value:objEventCom.EventUserInfoId)),

                };
                _sqlP[0].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventRegTicketsstatuscountUpdate", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[0].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        public Int64 UpdateWebsiteBannersStatus(Int64 EventId)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@EventId",EventId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("AWSEventsUpdateStatus", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }



        public DataTable FEEventsRegistrationGetListByEmail(string Email, Int64 ChapterId,ref int status)
        {
            DataTable ds = null;
            try
            {
                _sqlP = new[]
                {
                    new SqlParameter("@Email",Email),
                   new SqlParameter("@ChapterId",ChapterId),
                    new SqlParameter("@QStatus",0)
                };
                _sqlP[2].Direction = System.Data.ParameterDirection.Output;
                ds = _dbAccess.GetDataTable("FEEventsRegistrationGetListByEmail", ref _sqlP);
                status = Convert.ToInt32(_sqlP[2].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ds;
        }


        #region EventRegCheckIn

        public Int64 EventCheckInInsert(AWS_AssociationToolCore.Entities.EventRegCheckIn objEventCheckIn)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventRegCountId",objEventCheckIn.EventRegCountId),
                    new SqlParameter("@EventUserInfoId",objEventCheckIn.EventUserInfoId ),
                    new SqlParameter("@EventRegistrationTypeId",(objEventCheckIn.EventRegistrationTypeId==0?(object)DBNull.Value:objEventCheckIn.EventRegistrationTypeId)),

                    new SqlParameter("@Status",(objEventCheckIn.Status==null?(object)DBNull.Value:objEventCheckIn.Status)),
                    new SqlParameter("@Comments",(objEventCheckIn.Comments==null?(object)DBNull.Value:objEventCheckIn.Comments)),
                    new SqlParameter("@CheckInDate",(objEventCheckIn.CheckInDate==DateTime.MinValue?(object)DBNull.Value:objEventCheckIn.CheckInDate)),
                    new SqlParameter("@InsertedBy",(objEventCheckIn.InsertedBy==null?(object)DBNull.Value:objEventCheckIn.InsertedBy)),
                    new SqlParameter("@EventId",(objEventCheckIn.EventId==0?(object)DBNull.Value:objEventCheckIn.EventId)),
                    new SqlParameter("@EventCommentId",(objEventCheckIn.EventCommentId==0?(object)DBNull.Value:objEventCheckIn.EventCommentId)),
                    new SqlParameter("@EmployeeRefUrl",(objEventCheckIn.EmployeeRefUrl==null?(object)DBNull.Value:objEventCheckIn.EmployeeRefUrl)),
                    new SqlParameter("@EmployeeCode",(objEventCheckIn.EmployeeCode==null?(object)DBNull.Value:objEventCheckIn.EmployeeCode)),
                    new SqlParameter("@QStatus",0),
                    new SqlParameter("@EventRegTicketId",(objEventCheckIn.EventRegTicketId==0?(object)DBNull.Value:objEventCheckIn.EventRegTicketId))


                };
                _sqlP[11].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("EventCheckInInsert", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[11].Value);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }
        public DataTable EventUserInfoIndividualGetById(Int64 EventId, Int64 EventUserInfoId, Int64 EventRegTicketId, ref int Status)
        {
            DataTable dt = null;
            try
            {
                _sqlP = new[]
                {
                  new SqlParameter("@EventId",EventId),
                    new SqlParameter("@QStatus",0),

                     new SqlParameter("@EventUserInfoId",EventUserInfoId),
                    new SqlParameter("@EventRegTicketId",EventRegTicketId)

                };
                _sqlP[1].Direction = System.Data.ParameterDirection.Output;
                dt = _dbAccess.GetDataTable("EventUserInfoIndividualGetById", ref _sqlP);
                Status = Convert.ToInt32(_sqlP[1].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }
        public Int64 UpdateSeatAllocation(long ticketId, string seatNo, string InsertedBy, bool Notify)
        {
            Int64 _status = 0;
            try
            {
                _sqlP = new[]
                    {
                    new SqlParameter("@EventRegTicketId",ticketId),
                    new SqlParameter("@seatNo",seatNo),
                    new SqlParameter("@InsertedBy",InsertedBy),
                    new SqlParameter("@Notify",Notify),
                    new SqlParameter("@QStatus",0),

                };
                _sqlP[4].Direction = System.Data.ParameterDirection.Output;
                _dbAccess.SP_ExecuteScalar("AllocateSeatForTicket", ref _sqlP);
                _status = Convert.ToInt64(_sqlP[4].Value);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _status;
        }


        #endregion

    }
}
