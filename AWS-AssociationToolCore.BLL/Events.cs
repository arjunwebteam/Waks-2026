using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AWS_AssociationToolCore.BLL
{
    public class Events
    {
        DAL.Events _Events = new DAL.Events();
        //string logourl = System.Configuration.ConfigurationManager.AppSettings["uploadPath"] + "events/banners/";
        public Int64 InsertEvent(Entities.Events objEvents, ref string imageurl, ref string imageurl3)
        {
            Int64 _status = 0;
            if (objEvents != null)
            {
                _status = _Events.InsertEvent(objEvents, ref imageurl, ref imageurl3);
            }
            return _status;
        }

        public Int64 InsertEventUserInfo(Entities.EventUserInfo objEventUserInfo, ref Int64 EventUserInfoId, ref string DocumentUrl, ref string receipturl)
        {
            Int64 _status = 0;
            if (objEventUserInfo != null)
            {
                _status = _Events.InsertEventUserInfo(objEventUserInfo, ref EventUserInfoId, ref DocumentUrl, ref receipturl);
            }
            return _status;
        }

        public Int64 UpdateEventUserPaymentInfo(Entities.EventUserInfo objEventUserInfo)
        {
            Int64 _status = 0;
            if (objEventUserInfo != null)
            {
                _status = _Events.UpdateEventUserPaymentInfo(objEventUserInfo);
            }
            return _status;
        }
        public Int64 InsertEventUserInfo(Entities.EventUserInfo objEventUserInfo, ref string receipturl)
        {
            Int64 _status = 0;
            if (objEventUserInfo != null)
            {
                _status = _Events.InsertEventUserInfo(objEventUserInfo, ref receipturl);
            }
            return _status;
        }

        public Int64 UpdateEventUserInfo(Entities.EventUserInfo objEventUserInfo, ref string receipturl)
        {
            Int64 _status = 0;
            if (objEventUserInfo != null)
            {
                _status = _Events.UpdateEventUserInfo(objEventUserInfo, ref receipturl);
            }
            return _status;
        }

        public Int64 InsertEventParticipant(Entities.EventParticipants objEventParticipants)
        {
            Int64 _status = 0;
            if (objEventParticipants != null)
            {
                _status = _Events.InsertEventParticipant(objEventParticipants);
            }
            return _status;
        }

        public Int64 UpdateEventUserInfo(Entities.EventParticipants objEventParticipants)
        {
            Int64 _status = 0;
            if (objEventParticipants != null)
            {
                _status = _Events.UpdateEventParticipant(objEventParticipants);
            }
            return _status;
        }

        public Int64 InsertEventOrderDetail(Entities.EventOrderDetails objEventOrderDetails)
        {
            Int64 _status = 0;
            if (objEventOrderDetails != null)
            {
                _status = _Events.InsertEventOrderDetail(objEventOrderDetails);
            }
            return _status;
        }

        public Int64 UpdateEventOrderDetail(Entities.EventOrderDetails objUpdateEventOrderDetail)
        {
            Int64 _status = 0;
            if (objUpdateEventOrderDetail != null)
            {
                _status = _Events.UpdateEventOrderDetail(objUpdateEventOrderDetail);
            }
            return _status;
        }

        public Int64 UpdateEventRegistrationTypesDisplayOrder(int OrderNo, Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            _status = _Events.UpdateEventRegistrationTypesDisplayOrder(OrderNo, EventRegistrationTypeId);
            return _status;
        }

        public Entities.Events GetEventById(Int64 EventId, string EventName, ref List<Entities.EventRegistrationTypes> lstEventRegistrationTypes, ref List<Entities.EventDates> lstEventDates, ref List<Entities.RegistrationCategories> lstEventRegistrationCategories, ref int status)
        {
            DataSet ds = _Events.GetEventById(EventId, EventName, ref status);
            Entities.Events objEvent = new Entities.Events();
            List<Entities.ChapterEvents> lstChapterEvents = new List<Entities.ChapterEvents>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objEvent.EventId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventId"]);
                objEvent.ChapterId = (ds.Tables[0].Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["ChapterId"]) : 0);
                objEvent.BannerUrl = (ds.Tables[0].Rows[0]["BannerUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["BannerUrl"].ToString() : null);
                objEvent.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objEvent.ContactEmail = (ds.Tables[0].Rows[0]["ContactEmail"] != DBNull.Value ? ds.Tables[0].Rows[0]["ContactEmail"].ToString() : null);
                objEvent.EndDate = (ds.Tables[0].Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"]) : DateTime.MinValue);
                objEvent.EventCategoryId = (ds.Tables[0].Rows[0]["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["EventCategoryId"]) : 0);
                objEvent.EventCategory = (ds.Tables[0].Rows[0]["EventCategory"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventCategory"].ToString() : null);
                objEvent.EventDetails = (ds.Tables[0].Rows[0]["EventDetails"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventDetails"].ToString() : null);
                objEvent.EventName = ds.Tables[0].Rows[0]["EventName"].ToString();
                objEvent.IsRegistration = (ds.Tables[0].Rows[0]["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsRegistration"]) : false);
                objEvent.IsCulturalRegistration = (ds.Tables[0].Rows[0]["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsCulturalRegistration"]) : false);
                objEvent.Location = (ds.Tables[0].Rows[0]["Location"] != DBNull.Value ? ds.Tables[0].Rows[0]["Location"].ToString() : null);
                objEvent.MemberCount = (ds.Tables[0].Rows[0]["MemberCount"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["MemberCount"]) : 0);
                objEvent.RegistrationEndDate = (ds.Tables[0].Rows[0]["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationEndDate"]) : DateTime.MinValue);
                objEvent.RegistrationStartDate = (ds.Tables[0].Rows[0]["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationStartDate"]) : DateTime.MinValue);
                objEvent.StartDate = (ds.Tables[0].Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"]) : DateTime.MinValue);
                objEvent.StateName = (ds.Tables[0].Rows[0]["StateName"] != DBNull.Value ? ds.Tables[0].Rows[0]["StateName"].ToString() : null);
                objEvent.ZipCode = (ds.Tables[0].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZipCode"].ToString() : null);
                objEvent.TopLine = (ds.Tables[0].Rows[0]["TopLine"] != DBNull.Value ? ds.Tables[0].Rows[0]["TopLine"].ToString() : null);
                objEvent.PageTitle = (ds.Tables[0].Rows[0]["PageTitle"] != DBNull.Value ? ds.Tables[0].Rows[0]["PageTitle"].ToString() : null);
                objEvent.MetaKeywords = (ds.Tables[0].Rows[0]["MetaKeywords"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaKeywords"].ToString() : null);
                objEvent.MetaDescription = (ds.Tables[0].Rows[0]["MetaDescription"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaDescription"].ToString() : null);
                objEvent.UpdatedTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedTime"]);
                objEvent.ChapterName = (ds.Tables[0].Rows[0]["ChapterName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ChapterName"].ToString() : null);
                objEvent.UpdatedBy = (ds.Tables[0].Rows[0]["UpdatedBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["UpdatedBy"].ToString() : null);
                objEvent.Flyer = (ds.Tables[0].Rows[0]["Flyer"] != DBNull.Value ? ds.Tables[0].Rows[0]["Flyer"].ToString() : null);
                objEvent.IsQRCode = (ds.Tables[0].Rows[0]["IsQRCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["IsQRCode"].ToString() : null);
                objEvent.EarlyBirdCutOfDate = (ds.Tables[0].Rows[0]["EarlyBirdCutOfDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EarlyBirdCutOfDate"]) : DateTime.MinValue);
                objEvent.IsEarlyBird = (ds.Tables[0].Rows[0]["IsEarlyBird"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsEarlyBird"]) : false);
                objEvent.IsMemberAmount = (ds.Tables[0].Rows[0]["IsMemberAmount"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsMemberAmount"]) : false);
                objEvent.IsNonMemberAmount = (ds.Tables[0].Rows[0]["IsNonMemberAmount"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsNonMemberAmount"]) : false);
                 //objEvent.sdate = (ds.Tables[0].Rows[0]["sdate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["sdate"]) : DateTime.MinValue);
                //objEvent.edate = (ds.Tables[0].Rows[0]["edate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["edate"]) : DateTime.MinValue);





            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationTypes objEventRegistrationTypes = new AWS_AssociationToolCore.Entities.EventRegistrationTypes();
                    objEventRegistrationTypes.EventRegistrationTypeId = Convert.ToInt64(dr["EventRegistrationTypeId"].ToString());
                    objEventRegistrationTypes.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventRegistrationTypes.RegistrationTitle = dr["RegistrationTitle"].ToString();
                    objEventRegistrationTypes.RegCount = (dr["RegCount"] != DBNull.Value ? Convert.ToInt64(dr["RegCount"].ToString()) : 0);
                    objEventRegistrationTypes.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0); 
                    objEventRegistrationTypes.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objEventRegistrationTypes.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt64(dr["MemberCount"].ToString()) : 0);
                    objEventRegistrationTypes.EventRegCount = (dr["EventRegCount"] != DBNull.Value ? Convert.ToInt64(dr["EventRegCount"].ToString()) : 0);
                    objEventRegistrationTypes.EarlyBirdMemberAmount = (dr["EarlyBirdMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EarlyBirdMemberAmount"]) : 0);
                    objEventRegistrationTypes.EarlyBirdNonMemberAmount = (dr["EarlyBirdNonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EarlyBirdNonMemberAmount"]) : 0);
                    lstEventRegistrationTypes.Add(objEventRegistrationTypes);
                }
            }

            if (ds.Tables[2].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    Entities.ChapterEvents objChapterEvents = new Entities.ChapterEvents();

                    objChapterEvents.ChapterEventId = (dr["ChapterEventId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterEventId"].ToString()) : 0);
                    objChapterEvents.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"].ToString()) : 0);
                    objChapterEvents.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"].ToString()) : 0);

                    lstChapterEvents.Add(objChapterEvents);
                }
            }
            objEvent.lstChapterEvents = lstChapterEvents;

            if (ds.Tables[3].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventDates objEventDates = new AWS_AssociationToolCore.Entities.EventDates();
                    objEventDates.EventDatesId = Convert.ToInt64(dr["EventDatesId"].ToString());
                    objEventDates.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventDates.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"].ToString()) : DateTime.MinValue);
                    objEventDates.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"].ToString()) : DateTime.MinValue);
                    objEventDates.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEventDates.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());

                    lstEventDates.Add(objEventDates);
                }
            }

            if (ds.Tables[4].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    AWS_AssociationToolCore.Entities.RegistrationCategories objEventRegistrationCategories = new AWS_AssociationToolCore.Entities.RegistrationCategories();

                    objEventRegistrationCategories.RegistrationCategoryId = Convert.ToInt64(dr["RegistrationCategoryId"].ToString());
                    objEventRegistrationCategories.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEventRegistrationCategories.NAmount = (dr["NAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NAmount"].ToString()) : 0);
                    objEventRegistrationCategories.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);

                    lstEventRegistrationCategories.Add(objEventRegistrationCategories);
                }
            }
            return objEvent;
        }

        public Entities.Events GetEventByIdFE(Int64 EventId, string EventName, string Email, ref List<Entities.EventRegistrationTypes> lstEventRegistrationTypes, ref List<Entities.EventDates> lstEventDates, ref List<Entities.RegistrationCategories> lstEventRegistrationCategories, ref int status)
        {
            DataSet ds = _Events.GetEventByIdFE(EventId, EventName, Email, ref status);
            Entities.Events objEvent = new Entities.Events();
            List<Entities.ChapterEvents> lstChapterEvents = new List<Entities.ChapterEvents>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objEvent.EventId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventId"]);
                objEvent.ChapterId = (ds.Tables[0].Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["ChapterId"]) : 0);
                objEvent.BannerUrl = (ds.Tables[0].Rows[0]["BannerUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["BannerUrl"].ToString() : null);
                objEvent.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objEvent.ContactEmail = (ds.Tables[0].Rows[0]["ContactEmail"] != DBNull.Value ? ds.Tables[0].Rows[0]["ContactEmail"].ToString() : null);
                objEvent.EndDate = (ds.Tables[0].Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"]) : DateTime.MinValue);
                objEvent.EventCategoryId = (ds.Tables[0].Rows[0]["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["EventCategoryId"]) : 0);
                objEvent.EventCategory = (ds.Tables[0].Rows[0]["EventCategory"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventCategory"].ToString() : null);
                objEvent.EventDetails = (ds.Tables[0].Rows[0]["EventDetails"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventDetails"].ToString() : null);
                objEvent.EventName = ds.Tables[0].Rows[0]["EventName"].ToString();
                objEvent.IsRegistration = (ds.Tables[0].Rows[0]["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsRegistration"]) : false);
                objEvent.IsCulturalRegistration = (ds.Tables[0].Rows[0]["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsCulturalRegistration"]) : false);
                objEvent.Location = (ds.Tables[0].Rows[0]["Location"] != DBNull.Value ? ds.Tables[0].Rows[0]["Location"].ToString() : null);
                objEvent.MemberCount = (ds.Tables[0].Rows[0]["MemberCount"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["MemberCount"]) : 0);
                objEvent.RegistrationEndDate = (ds.Tables[0].Rows[0]["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationEndDate"]) : DateTime.MinValue);
                objEvent.RegistrationStartDate = (ds.Tables[0].Rows[0]["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationStartDate"]) : DateTime.MinValue);
                objEvent.StartDate = (ds.Tables[0].Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"]) : DateTime.MinValue);
                objEvent.StateName = (ds.Tables[0].Rows[0]["StateName"] != DBNull.Value ? ds.Tables[0].Rows[0]["StateName"].ToString() : null);
                objEvent.ZipCode = (ds.Tables[0].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZipCode"].ToString() : null);
                objEvent.TopLine = (ds.Tables[0].Rows[0]["TopLine"] != DBNull.Value ? ds.Tables[0].Rows[0]["TopLine"].ToString() : null);
                objEvent.PageTitle = (ds.Tables[0].Rows[0]["PageTitle"] != DBNull.Value ? ds.Tables[0].Rows[0]["PageTitle"].ToString() : null);
                objEvent.MetaKeywords = (ds.Tables[0].Rows[0]["MetaKeywords"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaKeywords"].ToString() : null);
                objEvent.MetaDescription = (ds.Tables[0].Rows[0]["MetaDescription"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaDescription"].ToString() : null);
                objEvent.UpdatedTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedTime"]);
                objEvent.ChapterName = (ds.Tables[0].Rows[0]["ChapterName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ChapterName"].ToString() : null);
                objEvent.UpdatedBy = (ds.Tables[0].Rows[0]["UpdatedBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["UpdatedBy"].ToString() : null);
                objEvent.IsQRCode = (ds.Tables[0].Rows[0]["IsQRCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["IsQRCode"].ToString() : null);
                objEvent.EmaisToBrevo = (ds.Tables[0].Rows[0]["EmaisToBrevo"] != DBNull.Value ? ds.Tables[0].Rows[0]["EmaisToBrevo"].ToString() : null);
                objEvent.EarlyBirdCutOfDate = (ds.Tables[0].Rows[0]["EarlyBirdCutOfDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EarlyBirdCutOfDate"]) : DateTime.MinValue);
                objEvent.IsEarlyBird = (ds.Tables[0].Rows[0]["IsEarlyBird"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsEarlyBird"]) : false);
                objEvent.IsMemberAmount = (ds.Tables[0].Rows[0]["IsMemberAmount"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsMemberAmount"]) : false);
                objEvent.IsNonMemberAmount = (ds.Tables[0].Rows[0]["IsNonMemberAmount"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsNonMemberAmount"]) : false);

            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationTypes objEventRegistrationTypes = new AWS_AssociationToolCore.Entities.EventRegistrationTypes();
                    objEventRegistrationTypes.EventRegistrationTypeId = Convert.ToInt64(dr["EventRegistrationTypeId"].ToString());
                    objEventRegistrationTypes.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventRegistrationTypes.RegistrationTitle = dr["RegistrationTitle"].ToString();
                    objEventRegistrationTypes.RegCount = (dr["RegCount"] != DBNull.Value ? Convert.ToInt64(dr["RegCount"].ToString()) : 0);
                    objEventRegistrationTypes.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEventRegistrationTypes.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objEventRegistrationTypes.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt64(dr["MemberCount"].ToString()) : 0);
                    objEventRegistrationTypes.EventRegCount = (dr["EventRegCount"] != DBNull.Value ? Convert.ToInt64(dr["EventRegCount"].ToString()) : 0);
                    objEventRegistrationTypes.MERegCount = (dr["MERegCount"] != DBNull.Value ? Convert.ToInt64(dr["MERegCount"].ToString()) : 0);
                    objEventRegistrationTypes.EarlyBirdMemberAmount = (dr["EarlyBirdMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EarlyBirdMemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.EarlyBirdNonMemberAmount = (dr["EarlyBirdNonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EarlyBirdNonMemberAmount"].ToString()) : 0);

                    lstEventRegistrationTypes.Add(objEventRegistrationTypes);
                }
            }

            if (ds.Tables[2].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    Entities.ChapterEvents objChapterEvents = new Entities.ChapterEvents();

                    objChapterEvents.ChapterEventId = (dr["ChapterEventId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterEventId"].ToString()) : 0);
                    objChapterEvents.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"].ToString()) : 0);
                    objChapterEvents.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"].ToString()) : 0);

                    lstChapterEvents.Add(objChapterEvents);
                }
            }
            objEvent.lstChapterEvents = lstChapterEvents;

            if (ds.Tables[3].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventDates objEventDates = new AWS_AssociationToolCore.Entities.EventDates();
                    objEventDates.EventDatesId = Convert.ToInt64(dr["EventDatesId"].ToString());
                    objEventDates.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventDates.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"].ToString()) : DateTime.MinValue);
                    objEventDates.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"].ToString()) : DateTime.MinValue);
                    objEventDates.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEventDates.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());

                    lstEventDates.Add(objEventDates);
                }
            }

            if (ds.Tables[4].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    AWS_AssociationToolCore.Entities.RegistrationCategories objEventRegistrationCategories = new AWS_AssociationToolCore.Entities.RegistrationCategories();

                    objEventRegistrationCategories.RegistrationCategoryId = Convert.ToInt64(dr["RegistrationCategoryId"].ToString());
                    objEventRegistrationCategories.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEventRegistrationCategories.NAmount = (dr["NAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NAmount"].ToString()) : 0);
                    objEventRegistrationCategories.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);

                    lstEventRegistrationCategories.Add(objEventRegistrationCategories);
                }
            }
            return objEvent;
        }
        public Int64 DeleteEventById(Int64 EventId)
        {
            Int64 _status = 0;
            _status = _Events.DeleteEventById(EventId);
            return _status;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetEventsListByVariable(Int64 MemberId,Int64 EventStatusId, string EventType, string StartDate, string EndDate, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<AWS_AssociationToolCore.Entities.Events>();
            DataTable dt = _Events.GetEventsListByVariable(MemberId,EventStatusId, EventType, StartDate, EndDate, ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Events.GetEventsListByVariable(MemberId,EventStatusId, EventType, StartDate, EndDate, ChapterId,Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ?Convert.ToInt64(dr["EventCategoryId"].ToString()) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"]) : false);
                    objEvent.IsCulturalRegistration = (dr["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsCulturalRegistration"]) : false);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.UsersCount = (dr["UsersCount"] != DBNull.Value ? Convert.ToInt32(dr["UsersCount"]) : 0);
                    objEvent.CUsersCount = (dr["CUsersCount"] != DBNull.Value ? Convert.ToInt32(dr["CUsersCount"]) : 0);
                    objEvent.RId = Convert.ToInt64(dr["RId"]);
                    objEvent.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    //objEvent.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : true);
                    objEvent.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);


                    lstEvents.Add(objEvent);
                }
            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetApiEventsListByVariable(Int64 MemberId, Int64 EventStatusId, string EventType, string StartDate, string EndDate, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<AWS_AssociationToolCore.Entities.Events>();
            DataTable dt = _Events.GetApiEventsListByVariable(MemberId, EventStatusId, EventType, StartDate, EndDate, ChapterId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Events.GetApiEventsListByVariable(MemberId, EventStatusId, EventType, StartDate, EndDate, ChapterId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(dr["EventCategoryId"].ToString()) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"]) : false);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.UsersCount = (dr["UsersCount"] != DBNull.Value ? Convert.ToInt32(dr["UsersCount"]) : 0);
                    objEvent.RId = Convert.ToInt64(dr["RId"]);
                    objEvent.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);
                    objEvent.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);

                    lstEvents.Add(objEvent);
                }
            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetEventsList(string Type,Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.GetEventsList(Type, ChapterId, Search, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    //objEvent.IsChild = (dr["IsChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsChild"].ToString()) : false);
                    //objEvent.ChildAmount = (dr["ChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["ChildAmount"]) : 0);
                    //objEvent.NonChildAmount = (dr["NonChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonChildAmount"]) : 0);
                    //objEvent.IsAdult = (dr["IsAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsAdult"].ToString()) : false);
                    //objEvent.AdultAmount = (dr["AdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["AdultAmount"]) : 0);
                    //objEvent.NonAdultAmount = (dr["NonAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonAdultAmount"]) : 0);
                    //objEvent.IsCouple = (dr["IsCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsCouple"].ToString()) : false);
                    //objEvent.CoupleAmount = (dr["CoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["CoupleAmount"]) : 0);
                    //objEvent.NonCoupleAmount = (dr["NonCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonCoupleAmount"]) : 0);
                    //objEvent.IsFamily = (dr["IsFamily"] != DBNull.Value ? Convert.ToBoolean(dr["IsFamily"].ToString()) : false);
                    //objEvent.FamilyAmount = (dr["FamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["FamilyAmount"]) : 0);
                    //objEvent.NonFamilyAmount = (dr["NonFamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonFamilyAmount"]) : 0);
                    //objEvent.IsVIP = (dr["IsVIP"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIP"].ToString()) : false);
                    //objEvent.VIPAmount = (dr["VIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPAmount"]) : 0);
                    //objEvent.NonVIPAmount = (dr["NonVIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPAmount"]) : 0);
                    //objEvent.IsVIPChild = (dr["IsVIPChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPChild"].ToString()) : false);
                    //objEvent.VIPChildAmount = (dr["VIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPChildAmount"]) : 0);
                    //objEvent.NonVIPChildAmount = (dr["NonVIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPChildAmount"]) : 0);
                    //objEvent.IsVIPSingleAdult = (dr["IsVIPSingleAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPSingleAdult"].ToString()) : false);
                    //objEvent.VIPSingleAdultAmount = (dr["VIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPSingleAdultAmount"]) : 0);
                    //objEvent.NonVIPSingleAdultAmount = (dr["NonVIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPSingleAdultAmount"]) : 0);
                    //objEvent.IsVIPCouple = (dr["IsVIPCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPCouple"].ToString()) : false);
                    //objEvent.VIPCoupleAmount = (dr["VIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPCoupleAmount"]) : 0);
                    //objEvent.NonVIPCoupleAmount = (dr["NonVIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPCoupleAmount"]) : 0);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> FEGetEventsList(Int64 ChapterId, string Type, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.FEGetEventsList(ChapterId, Type,  Search, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"]) : 0);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.UpdatedBy = (dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : null);

                    //objEvent.IsChild = (dr["IsChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsChild"].ToString()) : false);
                    //objEvent.ChildAmount = (dr["ChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["ChildAmount"]) : 0);
                    //objEvent.NonChildAmount = (dr["NonChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonChildAmount"]) : 0);
                    //objEvent.IsAdult = (dr["IsAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsAdult"].ToString()) : false);
                    //objEvent.AdultAmount = (dr["AdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["AdultAmount"]) : 0);
                    //objEvent.NonAdultAmount = (dr["NonAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonAdultAmount"]) : 0);
                    //objEvent.IsCouple = (dr["IsCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsCouple"].ToString()) : false);
                    //objEvent.CoupleAmount = (dr["CoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["CoupleAmount"]) : 0);
                    //objEvent.NonCoupleAmount = (dr["NonCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonCoupleAmount"]) : 0);
                    //objEvent.IsFamily = (dr["IsFamily"] != DBNull.Value ? Convert.ToBoolean(dr["IsFamily"].ToString()) : false);
                    //objEvent.FamilyAmount = (dr["FamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["FamilyAmount"]) : 0);
                    //objEvent.NonFamilyAmount = (dr["NonFamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonFamilyAmount"]) : 0);
                    //objEvent.IsVIP = (dr["IsVIP"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIP"].ToString()) : false);
                    //objEvent.VIPAmount = (dr["VIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPAmount"]) : 0);
                    //objEvent.NonVIPAmount = (dr["NonVIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPAmount"]) : 0);
                    //objEvent.IsVIPChild = (dr["IsVIPChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPChild"].ToString()) : false);
                    //objEvent.VIPChildAmount = (dr["VIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPChildAmount"]) : 0);
                    //objEvent.NonVIPChildAmount = (dr["NonVIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPChildAmount"]) : 0);
                    //objEvent.IsVIPSingleAdult = (dr["IsVIPSingleAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPSingleAdult"].ToString()) : false);
                    //objEvent.VIPSingleAdultAmount = (dr["VIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPSingleAdultAmount"]) : 0);
                    //objEvent.NonVIPSingleAdultAmount = (dr["NonVIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPSingleAdultAmount"]) : 0);
                    //objEvent.IsVIPCouple = (dr["IsVIPCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPCouple"].ToString()) : false);
                    //objEvent.VIPCoupleAmount = (dr["VIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPCoupleAmount"]) : 0);
                    //objEvent.NonVIPCoupleAmount = (dr["NonVIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPCoupleAmount"]) : 0);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetEventsListall(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.GetEventsListall(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetEventsList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.GetEventsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetUpcomingEventsList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.GetUpcomingEventsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.Events> RegistrationGetEventsList(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.RegistrationGetEventsList(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.IsCulturalRegistration = (dr["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsCulturalRegistration"].ToString()) : false);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    //objEvent.IsChild = (dr["IsChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsChild"].ToString()) : false);
                    //objEvent.ChildAmount = (dr["ChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["ChildAmount"]) : 0);
                    //objEvent.NonChildAmount = (dr["NonChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonChildAmount"]) : 0);
                    //objEvent.IsAdult = (dr["IsAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsAdult"].ToString()) : false);
                    //objEvent.AdultAmount = (dr["AdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["AdultAmount"]) : 0);
                    //objEvent.NonAdultAmount = (dr["NonAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonAdultAmount"]) : 0);
                    //objEvent.IsCouple = (dr["IsCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsCouple"].ToString()) : false);
                    //objEvent.CoupleAmount = (dr["CoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["CoupleAmount"]) : 0);
                    //objEvent.NonCoupleAmount = (dr["NonCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonCoupleAmount"]) : 0);
                    //objEvent.IsFamily = (dr["IsFamily"] != DBNull.Value ? Convert.ToBoolean(dr["IsFamily"].ToString()) : false);
                    //objEvent.FamilyAmount = (dr["FamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["FamilyAmount"]) : 0);
                    //objEvent.NonFamilyAmount = (dr["NonFamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonFamilyAmount"]) : 0);
                    //objEvent.IsVIP = (dr["IsVIP"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIP"].ToString()) : false);
                    //objEvent.VIPAmount = (dr["VIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPAmount"]) : 0);
                    //objEvent.NonVIPAmount = (dr["NonVIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPAmount"]) : 0);
                    //objEvent.IsVIPChild = (dr["IsVIPChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPChild"].ToString()) : false);
                    //objEvent.VIPChildAmount = (dr["VIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPChildAmount"]) : 0);
                    //objEvent.NonVIPChildAmount = (dr["NonVIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPChildAmount"]) : 0);
                    //objEvent.IsVIPSingleAdult = (dr["IsVIPSingleAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPSingleAdult"].ToString()) : false);
                    //objEvent.VIPSingleAdultAmount = (dr["VIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPSingleAdultAmount"]) : 0);
                    //objEvent.NonVIPSingleAdultAmount = (dr["NonVIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPSingleAdultAmount"]) : 0);
                    //objEvent.IsVIPCouple = (dr["IsVIPCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPCouple"].ToString()) : false);
                    //objEvent.VIPCoupleAmount = (dr["VIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPCoupleAmount"]) : 0);
                    //objEvent.NonVIPCoupleAmount = (dr["NonVIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPCoupleAmount"]) : 0);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public Int64 UpdateEventStatus(Int64 EventId)
        {
            Int64 _status = 0;
            _status = _Events.UpdateEventStatus(EventId);
            return _status;
        }

        public List<AWS_AssociationToolCore.Entities.Events> GetEnquiryEventsListall(ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.GetEnquiryEventsListall(ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public Int64 InsertRegistrationTypes(Entities.EventRegistrationTypes objRegistrationTypes)
        {
            Int64 _status = 0;
            if (objRegistrationTypes != null)
            {
                _status = _Events.InsertRegistrationTypes(objRegistrationTypes);
            }
            return _status;
        }

        public Int64 UpdateEventDates(Entities.EventDates objEventDates)
        {
            Int64 _status = 0;
            if (objEventDates != null)
            {
                _status = _Events.UpdateEventDates(objEventDates);
            }
            return _status;
        }

        #region EventUserInfo

        public Int64 DeleteEventUserInfoById(Int64 EventUserInfoId)
        {
            Int64 _status = 0;
            _status = _Events.DeleteEventUserInfoById(EventUserInfoId);
            return _status;
        }

        public Entities.EventUserInfo GetEventUserInfoById(Int64 EventUserInfoGetById, ref int status)
        {
            DataTable dt = _Events.GetEventUserInfoById(EventUserInfoGetById, ref status);
            Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();

            if (dt.Rows.Count == 1)
            {

                objEventUserInfo.EventId = Convert.ToInt64(dt.Rows[0]["EventId"]);
                objEventUserInfo.EventName = dt.Rows[0]["EventName"].ToString();
                objEventUserInfo.EventUserInfoId = Convert.ToInt64(dt.Rows[0]["EventUserInfoId"]);
                objEventUserInfo.LastName = dt.Rows[0]["LastName"].ToString();
                objEventUserInfo.EventCategoryId = Convert.ToInt64(dt.Rows[0]["EventCategoryId"]);
                objEventUserInfo.EventCategory = dt.Rows[0]["EventCategory"].ToString();
                objEventUserInfo.FirstName = dt.Rows[0]["FirstName"].ToString();
                objEventUserInfo.Email = dt.Rows[0]["Email"].ToString();
                objEventUserInfo.Gender = (dt.Rows[0]["Gender"] != DBNull.Value ? dt.Rows[0]["Gender"].ToString() : null);
                objEventUserInfo.Address = (dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null);
                objEventUserInfo.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : null);
                objEventUserInfo.State = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString() : null);
                objEventUserInfo.Mobile = (dt.Rows[0]["Mobile"] != DBNull.Value ? dt.Rows[0]["Mobile"].ToString() : null);
                objEventUserInfo.ItemName = dt.Rows[0]["ItemName"].ToString();
                objEventUserInfo.ItemCategory = dt.Rows[0]["ItemCategory"].ToString();
                objEventUserInfo.ItemDescription = dt.Rows[0]["ItemDescription"].ToString();
                objEventUserInfo.ItemDuration = (dt.Rows[0]["ItemDuration"] != DBNull.Value ?Convert.ToInt32(dt.Rows[0]["ItemDuration"].ToString()) : 0);
                objEventUserInfo.IsApproved =Convert.ToBoolean(dt.Rows[0]["IsApproved"]);
                objEventUserInfo.DocumentUrl = (dt.Rows[0]["DocumentUrl"] != DBNull.Value ? dt.Rows[0]["DocumentUrl"].ToString() : null);
                objEventUserInfo.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objEventUserInfo.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"]);
                objEventUserInfo.TransactionId = (dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null);
                objEventUserInfo.AmountPaid = (dt.Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["AmountPaid"]) : 0);
                objEventUserInfo.PaymentStatus = (dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null);
                objEventUserInfo.PaymentMethod = (dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null);
                //objEventUserInfo.PaymentDate = Convert.ToDateTime(dt.Rows[0]["PaymentDate"]);
            }

            return objEventUserInfo;
        }

        public Entities.EventUserInfo GetEventUserInfoFullDetailsById(Int64 EventUserInfoGetById, Int64 EventId, ref Entities.Events objEvent, ref List<Entities.EventRegistrationCounts> lstEventRegistrationCounts, ref List<Entities.EventsTickets> lstEventsTickets, ref List<Entities.EventRegTickets> lstEventRegTickets, ref int status)
        {
            DataSet ds = _Events.GetEventUserInfoFullDetailsById(EventUserInfoGetById, EventId, ref status);
            Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
            List<Entities.EventParticipants> lstEventParticipants = new List<Entities.EventParticipants>();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objEventUserInfo.EventId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventId"]);
                objEventUserInfo.EventName = ds.Tables[0].Rows[0]["EventName"].ToString();
                objEventUserInfo.EventUserInfoId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventUserInfoId"]);
                objEventUserInfo.LastName = ds.Tables[0].Rows[0]["LastName"].ToString();
                objEventUserInfo.EventCategoryId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventCategoryId"]);
                objEventUserInfo.EventCategory = ds.Tables[0].Rows[0]["EventCategory"].ToString();
                objEventUserInfo.FirstName = ds.Tables[0].Rows[0]["FirstName"].ToString();
                objEventUserInfo.Email = ds.Tables[0].Rows[0]["Email"].ToString();
                objEventUserInfo.Gender = (ds.Tables[0].Rows[0]["Gender"] != DBNull.Value ? ds.Tables[0].Rows[0]["Gender"].ToString() : null);
                objEventUserInfo.Address = (ds.Tables[0].Rows[0]["Address"] != DBNull.Value ? ds.Tables[0].Rows[0]["Address"].ToString() : null);
                objEventUserInfo.IsQRCode = (ds.Tables[0].Rows[0]["IsQRCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["IsQRCode"].ToString() : null);
                objEventUserInfo.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objEventUserInfo.State = (ds.Tables[0].Rows[0]["State"] != DBNull.Value ? ds.Tables[0].Rows[0]["State"].ToString() : null);
                objEventUserInfo.Mobile = (ds.Tables[0].Rows[0]["Mobile"] != DBNull.Value ? ds.Tables[0].Rows[0]["Mobile"].ToString() : null);
                objEventUserInfo.ItemName = ds.Tables[0].Rows[0]["ItemName"].ToString();
                objEventUserInfo.ItemCategory = ds.Tables[0].Rows[0]["ItemCategory"].ToString();
                objEventUserInfo.ItemDescription = ds.Tables[0].Rows[0]["ItemDescription"].ToString();
                objEventUserInfo.OtherPaymentId = (ds.Tables[0].Rows[0]["OtherPaymentId"] != DBNull.Value ? ds.Tables[0].Rows[0]["OtherPaymentId"].ToString() : null);
                objEventUserInfo.ItemDuration = (ds.Tables[0].Rows[0]["ItemDuration"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["ItemDuration"].ToString()) : 0);
                objEventUserInfo.IsApproved = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsApproved"]);
                objEventUserInfo.DocumentUrl = (ds.Tables[0].Rows[0]["DocumentUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["DocumentUrl"].ToString() : null);
                objEventUserInfo.UpdatedBy = ds.Tables[0].Rows[0]["UpdatedBy"].ToString();
                objEventUserInfo.UpdatedTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedTime"]);
                objEventUserInfo.Comments = (ds.Tables[0].Rows[0]["Comments"] != DBNull.Value ? ds.Tables[0].Rows[0]["Comments"].ToString() : null);
                objEventUserInfo.Status = (ds.Tables[0].Rows[0]["Status"] != DBNull.Value ? ds.Tables[0].Rows[0]["Status"].ToString() : null);
                objEventUserInfo.Comment = (ds.Tables[0].Rows[0]["Comment"] != DBNull.Value ? ds.Tables[0].Rows[0]["Comment"].ToString() : null);
                objEventUserInfo.CheckinCount = (ds.Tables[0].Rows[0]["CheckinCount"] != DBNull.Value ? ds.Tables[0].Rows[0]["CheckinCount"].ToString() : null);
                objEventUserInfo.ReceiptUrl = (ds.Tables[0].Rows[0]["ReceiptUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["ReceiptUrl"].ToString() : null);
                objEventUserInfo.ZellePartnerName = (ds.Tables[0].Rows[0]["ZellePartnerName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZellePartnerName"].ToString() : null);
                objEventUserInfo.ChequeHolderName = (ds.Tables[0].Rows[0]["ChequeHolderName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ChequeHolderName"].ToString() : null);
                objEventUserInfo.Zelle = (ds.Tables[0].Rows[0]["Zelle"] != DBNull.Value ? ds.Tables[0].Rows[0]["Zelle"].ToString() : null);
                objEventUserInfo.Location = (ds.Tables[0].Rows[0]["Location"] != DBNull.Value ? ds.Tables[0].Rows[0]["Location"].ToString() : null);


                objEventUserInfo.StartDate = (ds.Tables[0].Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"].ToString()) : DateTime.MinValue);
                objEventUserInfo.EndDate = (ds.Tables[0].Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"].ToString()) : DateTime.MinValue);


                //  objEventUserInfo.TicketTypesWithCount = (ds.Tables[0].Rows[0]["TicketTypesWithCount"] != DBNull.Value ? ds.Tables[0].Rows[0]["TicketTypesWithCount"].ToString() : null);
                // objEventUserInfo.TotalAmount = (ds.Tables[0].Rows[0]["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["TotalAmount"].ToString()) : 0);
                //  objEventUserInfo.AmountPaid = (ds.Tables[0].Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["AmountPaid"].ToString()) : 0);

            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    Entities.EventParticipants objEventParticipants = new Entities.EventParticipants();

                    objEventParticipants.EventParticipantId = Convert.ToInt64(dr["EventParticipantId"]);
                    objEventParticipants.EventId = Convert.ToInt64(dr["EventId"]);
                    objEventParticipants.FirstName = dr["FirstName"].ToString();
                    objEventParticipants.LastName = dr["LastName"].ToString();
                    objEventParticipants.Email = dr["Email"].ToString();
                    objEventParticipants.Age = (dr["Age"] != DBNull.Value ? dr["Age"].ToString() : null);
                    objEventParticipants.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objEventParticipants.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objEventParticipants.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstEventParticipants.Add(objEventParticipants);
                }

            }

            if (ds.Tables[2].Rows.Count == 1)
            {
                objEventUserInfo.objEventOrderDetails.IsApproved = (ds.Tables[2].Rows[0]["IsApproved"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[2].Rows[0]["IsApproved"].ToString()) : false);
                objEventUserInfo.objEventOrderDetails.EventUserInfoId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventUserInfoId"]);
                objEventUserInfo.objEventOrderDetails.ApprovedDate = (ds.Tables[2].Rows[0]["ApprovedDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[2].Rows[0]["ApprovedDate"].ToString()) : DateTime.MinValue);
                objEventUserInfo.objEventOrderDetails.PaymentMethod = (ds.Tables[2].Rows[0]["PaymentMethod"] != DBNull.Value ? ds.Tables[2].Rows[0]["PaymentMethod"].ToString() : null);
                objEventUserInfo.objEventOrderDetails.PaymentMethodId = (ds.Tables[2].Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[2].Rows[0]["PaymentMethodId"].ToString()) : 0);
                objEventUserInfo.objEventOrderDetails.PaymentStatus = (ds.Tables[2].Rows[0]["PaymentStatus"] != DBNull.Value ? ds.Tables[2].Rows[0]["PaymentStatus"].ToString() : null);
                objEventUserInfo.objEventOrderDetails.PaymentStatusId = (ds.Tables[2].Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[2].Rows[0]["PaymentStatusId"].ToString()) : 0);
                objEventUserInfo.objEventOrderDetails.TransactionId = (ds.Tables[2].Rows[0]["TransactionId"] != DBNull.Value ? ds.Tables[2].Rows[0]["TransactionId"].ToString() : null);
                objEventUserInfo.objEventOrderDetails.AmountPaid = (ds.Tables[2].Rows[0]["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[2].Rows[0]["AmountPaid"]) : 0);
                objEventUserInfo.objEventOrderDetails.PaymentDate = (ds.Tables[2].Rows[0]["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[2].Rows[0]["PaymentDate"].ToString()) : DateTime.MinValue);
                
                objEventUserInfo.objEventOrderDetails.UpdatedBy = ds.Tables[2].Rows[0]["UpdatedBy"].ToString();
                objEventUserInfo.objEventOrderDetails.UpdatedTime = Convert.ToDateTime(ds.Tables[2].Rows[0]["UpdatedTime"]);
            }

            objEventUserInfo.lstEventParticipant = lstEventParticipants;

            if (ds.Tables[3].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[3].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationCounts objEventRegistrationCounts = new AWS_AssociationToolCore.Entities.EventRegistrationCounts();
                    objEventRegistrationCounts.EventRegCountId = Convert.ToInt64(dr["EventRegCountId"].ToString());
                    objEventRegistrationCounts.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventRegistrationCounts.EventUserInfoId = Convert.ToInt64(dr["EventUserInfoId"].ToString());
                    objEventRegistrationCounts.EventRegistrationTypeId = Convert.ToInt64(dr["EventRegistrationTypeId"].ToString());

                    objEventRegistrationCounts.RegistrationTitle = dr["RegistrationTitle"].ToString();
                    objEventRegistrationCounts.Count = (dr["Count"] != DBNull.Value ? Convert.ToInt64(dr["Count"].ToString()) : 0);
                    objEventRegistrationCounts.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"].ToString()) : 0);
                    objEventRegistrationCounts.EarlyBirdMemberAmount = (dr["EarlyBirdMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EarlyBirdMemberAmount"].ToString()) : 0);

                    objEventRegistrationCounts.TotalTickets = (dr["TotalTickets"] != DBNull.Value ? Convert.ToInt32(dr["TotalTickets"]) : 0);
                    objEventRegistrationCounts.CheckedInCount = (dr["CheckedInCount"] != DBNull.Value ? Convert.ToInt32(dr["CheckedInCount"]) : 0);
                    objEventRegistrationCounts.OverallStatus = (dr["OverallStatus"] != DBNull.Value ? dr["OverallStatus"].ToString() : null);
                    objEventRegistrationCounts.CheckInDetails = (dr["CheckInDetails"] != DBNull.Value ? dr["CheckInDetails"].ToString() : null);
                    objEventRegistrationCounts.CheckinCount = (dr["CheckinCount"] != DBNull.Value ? dr["CheckinCount"].ToString() : null);


                    lstEventRegistrationCounts.Add(objEventRegistrationCounts);
                }
            }
            if (ds.Tables[4].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegTickets eventregtickets = new AWS_AssociationToolCore.Entities.EventRegTickets();

                    eventregtickets.EventRegTicketId = Convert.ToInt64(dr["EventRegTicketId"].ToString());
                    eventregtickets.EventRegCountId = Convert.ToInt64(dr["EventRegCountId"].ToString());
                    eventregtickets.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    eventregtickets.EventUserInfoId = (dr["EventUserInfoId"] != DBNull.Value ? Convert.ToInt64(dr["EventUserInfoId"].ToString()) : 0);
                    eventregtickets.EventRegistrationTypeId = (dr["EventRegistrationTypeId"] != DBNull.Value ? Convert.ToInt64(dr["EventRegistrationTypeId"].ToString()) : 0);
                    eventregtickets.SequenceNo = (dr["SequenceNo"] != DBNull.Value ? Convert.ToInt32(dr["SequenceNo"].ToString()) : 0);
                    eventregtickets.FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : null);
                    eventregtickets.LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : null);
                    eventregtickets.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    eventregtickets.PhoneNo = (dr["PhoneNo"] != DBNull.Value ? dr["PhoneNo"].ToString() : null);
                    eventregtickets.Gothram = (dr["Gothram"] != DBNull.Value ? dr["Gothram"].ToString() : null);
                    eventregtickets.Nakshatram = (dr["Nakshatram"] != DBNull.Value ? dr["Nakshatram"].ToString() : null);
                    eventregtickets.Status = (dr["Status"] != DBNull.Value ? dr["Status"].ToString() : null);
                    eventregtickets.GeneratedDate = (dr["GeneratedDate"] != DBNull.Value ? Convert.ToDateTime(dr["GeneratedDate"].ToString()) : DateTime.MinValue);
                    eventregtickets.RegistrationTitle = (dr["RegistrationTitle"] != DBNull.Value ? dr["RegistrationTitle"].ToString() : null);
                    eventregtickets.CheckInStatus = (dr["CheckInStatus"] != DBNull.Value ? dr["CheckInStatus"].ToString() : null);
                    eventregtickets.Comments = (dr["Comments"] != DBNull.Value ? dr["Comments"].ToString() : null);
                    eventregtickets.CheckInDate = (dr["CheckInDate"] != DBNull.Value ? Convert.ToDateTime(dr["CheckInDate"].ToString()) : DateTime.MinValue);
                    eventregtickets.TicketId = (dr["TicketId"] != DBNull.Value ? dr["TicketId"].ToString() : null);
                    eventregtickets.SeatNo = (dr["SeatNo"] != DBNull.Value ? dr["SeatNo"].ToString() : null);

                    lstEventRegTickets.Add(eventregtickets);
                }
            }
            //if (ds.Tables[4].Rows.Count != 0)
            //{
            //    foreach (DataRow dr in ds.Tables[4].Rows)
            //    {
            //        AWS_AssociationToolCore.Entities.EventsTickets objEventsTickets = new AWS_AssociationToolCore.Entities.EventsTickets();
            //        objEventsTickets.Barcode = Convert.ToInt64(dr["Barcode"]);
            //        objEventsTickets.Count = (dr["Count"] != DBNull.Value ? Convert.ToInt32(dr["Count"].ToString()) : 0);
            //        objEventsTickets.Amount = Convert.ToDecimal(dr["Amount"].ToString());
            //        objEventsTickets.Field1 = dr["Field1"].ToString();
            //        objEventsTickets.RegistrationTitle = dr["RegistrationTitle"].ToString();

            //        lstEventsTickets.Add(objEventsTickets);
            //    }
            //}

            //if (ds.Tables[5].Rows.Count == 1)
            //{
            //    objEvent.EventId = Convert.ToInt64(ds.Tables[5].Rows[0]["EventId"]);
            //    objEvent.BannerUrl = (ds.Tables[5].Rows[0]["BannerUrl"] != DBNull.Value ? ds.Tables[5].Rows[0]["BannerUrl"].ToString() : null);
            //    objEvent.City = (ds.Tables[5].Rows[0]["City"] != DBNull.Value ? ds.Tables[5].Rows[0]["City"].ToString() : null);
            //    objEvent.ContactEmail = (ds.Tables[5].Rows[0]["ContactEmail"] != DBNull.Value ? ds.Tables[5].Rows[0]["ContactEmail"].ToString() : null);
            //    objEvent.EndDate = (ds.Tables[5].Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[5].Rows[0]["EndDate"]) : DateTime.MinValue);
            //    objEvent.EventCategoryId = (ds.Tables[5].Rows[0]["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[5].Rows[0]["EventCategoryId"]) : 0);
            //    objEvent.EventCategory = (ds.Tables[5].Rows[0]["EventCategory"] != DBNull.Value ? ds.Tables[5].Rows[0]["EventCategory"].ToString() : null);
            //    objEvent.EventDetails = (ds.Tables[5].Rows[0]["EventDetails"] != DBNull.Value ? ds.Tables[5].Rows[0]["EventDetails"].ToString() : null);
            //    objEvent.EventName = ds.Tables[5].Rows[0]["EventName"].ToString();
            //    objEvent.IsRegistration = (ds.Tables[5].Rows[0]["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[5].Rows[0]["IsRegistration"]) : false);
            //    objEvent.IsCulturalRegistration = (ds.Tables[5].Rows[0]["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[5].Rows[0]["IsCulturalRegistration"]) : false);
            //    objEvent.Location = (ds.Tables[5].Rows[0]["Location"] != DBNull.Value ? ds.Tables[5].Rows[0]["Location"].ToString() : null);
            //    objEvent.MemberCount = (ds.Tables[5].Rows[0]["MemberCount"] != DBNull.Value ? Convert.ToInt32(ds.Tables[5].Rows[0]["MemberCount"]) : 0);
            //    objEvent.RegistrationEndDate = (ds.Tables[5].Rows[0]["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[5].Rows[0]["RegistrationEndDate"]) : DateTime.MinValue);
            //    objEvent.RegistrationStartDate = (ds.Tables[5].Rows[0]["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[5].Rows[0]["RegistrationStartDate"]) : DateTime.MinValue);
            //    objEvent.StartDate = (ds.Tables[5].Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[5].Rows[0]["StartDate"]) : DateTime.MinValue);
            //    objEvent.StateName = (ds.Tables[5].Rows[0]["StateName"] != DBNull.Value ? ds.Tables[5].Rows[0]["StateName"].ToString() : null);
            //    objEvent.ZipCode = (ds.Tables[5].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[5].Rows[0]["ZipCode"].ToString() : null);
            //    objEvent.TopLine = (ds.Tables[5].Rows[0]["TopLine"] != DBNull.Value ? ds.Tables[5].Rows[0]["TopLine"].ToString() : null);
            //    objEvent.PageTitle = (ds.Tables[5].Rows[0]["PageTitle"] != DBNull.Value ? ds.Tables[5].Rows[0]["PageTitle"].ToString() : null);
            //    objEvent.MetaKeywords = (ds.Tables[5].Rows[0]["MetaKeywords"] != DBNull.Value ? ds.Tables[5].Rows[0]["MetaKeywords"].ToString() : null);
            //    objEvent.MetaDescription = (ds.Tables[5].Rows[0]["MetaDescription"] != DBNull.Value ? ds.Tables[5].Rows[0]["MetaDescription"].ToString() : null);
            //    objEvent.UpdatedTime = Convert.ToDateTime(ds.Tables[5].Rows[0]["UpdatedTime"]);
            //}

            return objEventUserInfo;
        }

        public List<AWS_AssociationToolCore.Entities.EventUserInfo> GetEventUserInfoListByVariable(Int64 EventId, string EventCategory, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventUserInfo> lstEventUserInfo = new List<AWS_AssociationToolCore.Entities.EventUserInfo>();
            DataTable dt = _Events.GetEventUserInfoListByVariable(EventId,EventCategory, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Events.GetEventUserInfoListByVariable(EventId,EventCategory, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventUserInfo objEvent = new AWS_AssociationToolCore.Entities.EventUserInfo();

                    objEvent.RId = Convert.ToInt64(dr["Rid"]);
                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventUserInfoId = Convert.ToInt64(dr["EventUserInfoId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.LastName = dr["LastName"].ToString();
                    objEvent.EventCategory = dr["EventCategory"].ToString();
                    objEvent.FirstName = dr["FirstName"].ToString();
                    objEvent.Email = dr["Email"].ToString();
                    objEvent.Gender = (dr["Gender"] != DBNull.Value ? dr["Gender"].ToString() : null);
                    objEvent.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objEvent.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objEvent.ItemName = dr["ItemName"].ToString();
                    objEvent.ItemCategory = dr["ItemCategory"].ToString();
                    objEvent.ItemDescription = dr["ItemDescription"].ToString();
                    objEvent.ItemDuration = (dr["ItemDuration"] != DBNull.Value ?Convert.ToInt32(dr["ItemDuration"].ToString()) : 0);
                    objEvent.IsApproved = (dr["IsApproved"] != DBNull.Value ? Convert.ToBoolean(dr["IsApproved"]) : false);

                    objEvent.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objEvent.UpdatedBy = dr["UpdatedBy"].ToString();
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.InsertedTime = (dr["InsertedTime"] != DBNull.Value ?  Convert.ToDateTime(dr["InsertedTime"].ToString()) : DateTime.MinValue);
                    objEvent.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objEvent.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null);
                    objEvent.AmountPaid = (dr["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dr["AmountPaid"].ToString()) : 0);
                    objEvent.TransactionId = (dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null);
                    objEvent.UpdatedTime = (dr["UpdatedTime"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedTime"].ToString()) : DateTime.MinValue);
                    objEvent.Status = (dr["Status"] != DBNull.Value ? dr["Status"].ToString() : null);
                    objEvent.Comment = (dr["Comment"] != DBNull.Value ? dr["Comment"].ToString() : null);
                    lstEventUserInfo.Add(objEvent);
                }
            }
            return lstEventUserInfo;
        }

        public List<AWS_AssociationToolCore.Entities.EventUserInfo> RegistrationGetEventUserInfoListByVariable(Int64 EventId, string EventCategory, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventUserInfo> lstEventUserInfo = new List<AWS_AssociationToolCore.Entities.EventUserInfo>();
            DataTable dt = _Events.RegistrationGetEventUserInfoListByVariable(EventId, EventCategory, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Events.RegistrationGetEventUserInfoListByVariable(EventId, EventCategory, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventUserInfo objEvent = new AWS_AssociationToolCore.Entities.EventUserInfo();

                    objEvent.RId = Convert.ToInt64(dr["Rid"]);
                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventUserInfoId = Convert.ToInt64(dr["EventUserInfoId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.LastName = dr["LastName"].ToString();
                    objEvent.EventCategory = dr["EventCategory"].ToString();
                    objEvent.FirstName = dr["FirstName"].ToString();
                    objEvent.Email = dr["Email"].ToString();
                    objEvent.Gender = (dr["Gender"] != DBNull.Value ? dr["Gender"].ToString() : null);
                    objEvent.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objEvent.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objEvent.ItemName = dr["ItemName"].ToString();
                    objEvent.ItemCategory = dr["ItemCategory"].ToString();
                    objEvent.ItemDescription = dr["ItemDescription"].ToString();
                    objEvent.ItemDuration = (dr["ItemDuration"] != DBNull.Value ? Convert.ToInt32(dr["ItemDuration"].ToString()) : 0);
                    objEvent.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objEvent.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objEvent.UpdatedBy = dr["UpdatedBy"].ToString();
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.InsertedTime = (dr["InsertedTime"] != DBNull.Value ? Convert.ToDateTime(dr["InsertedTime"].ToString()) : DateTime.MinValue);
                    objEvent.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objEvent.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null);
                    lstEventUserInfo.Add(objEvent);
                }
            }
            return lstEventUserInfo;
        }

        public List<AWS_AssociationToolCore.Entities.EventUserInfo> ExportToEventUserInfoList(Int64 EventId, string Search, string Sort)
        {
            List<AWS_AssociationToolCore.Entities.EventUserInfo> lstEventUserInfo = new List<AWS_AssociationToolCore.Entities.EventUserInfo>();
            DataTable dt = _Events.ExportToEventUserInfoList(EventId, Search, Sort);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventUserInfo objEvent = new AWS_AssociationToolCore.Entities.EventUserInfo();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventUserInfoId = Convert.ToInt64(dr["EventUserInfoId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.LastName = dr["LastName"].ToString();
                    objEvent.EventCategory = dr["EventCategory"].ToString();
                    objEvent.FirstName = dr["FirstName"].ToString();
                    objEvent.Email = dr["Email"].ToString();
                    objEvent.Gender = (dr["Gender"] != DBNull.Value ? dr["Gender"].ToString() : null);
                    objEvent.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objEvent.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objEvent.ItemName = dr["ItemName"].ToString();
                    objEvent.ItemCategory = dr["ItemCategory"].ToString();
                    objEvent.ItemDescription = dr["ItemDescription"].ToString();
                    objEvent.ItemDuration = (dr["ItemDuration"] != DBNull.Value ? Convert.ToInt32(dr["ItemDuration"].ToString()) : 0);
                    objEvent.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objEvent.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objEvent.UpdatedBy = dr["UpdatedBy"].ToString();
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.InsertedTime = (dr["InsertedTime"] != DBNull.Value ? Convert.ToDateTime(dr["InsertedTime"].ToString()) : DateTime.MinValue);
                    objEvent.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objEvent.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null);
                    lstEventUserInfo.Add(objEvent);
                }
            }
            return lstEventUserInfo;
        }

        public Int64 UpdateEventUserInfoStatus(Int64 EventUserInfoGetById)
        {
            Int64 _status = 0;
            _status = _Events.UpdateEventUserInfoStatus(EventUserInfoGetById);
            return _status;
        }

        #endregion

        #region EventRegistrationTypes

        public Int64 DeleteEventRegistrationTypes(Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            _status = _Events.DeleteEventRegistrationTypes(EventRegistrationTypeId);

            return _status;
        }

        public Int64 DeleteEventDates(Int64 EventDatesId)
        {
            Int64 _status = 0;
            _status = _Events.DeleteEventDates(EventDatesId);

            return _status;
        }

        public List<AWS_AssociationToolCore.Entities.EventRegistrationTypes> GetEventRegistrationTypesList(Int64 EventId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventRegistrationTypes> lstEventRegistrationTypes = new List<AWS_AssociationToolCore.Entities.EventRegistrationTypes>();
            DataTable dt = _Events.GetEventRegistrationTypesList(EventId, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationTypes objEvent = new AWS_AssociationToolCore.Entities.EventRegistrationTypes();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventRegistrationTypeId = Convert.ToInt64(dr["EventRegistrationTypeId"]);
                    objEvent.RegistrationTitle = dr["RegistrationTitle"].ToString();
                    objEvent.RegCount = (dr["RegCount"] != DBNull.Value ? Convert.ToInt64(dr["RegCount"].ToString()) : 0);
                    objEvent.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEvent.IsActive = Convert.ToBoolean(dr["IsActive"]);
                    objEvent.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEvent.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"].ToString()) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt64(dr["MemberCount"].ToString()) : 0);
                    objEvent.EarlyBirdMemberAmount = (dr["EarlyBirdMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EarlyBirdMemberAmount"].ToString()) : 0);

                    lstEventRegistrationTypes.Add(objEvent);
                }

            }
            return lstEventRegistrationTypes;
        }

        public List<AWS_AssociationToolCore.Entities.EventDates> GetEventsDatesList(Int64 EventId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventDates> lstEventDates = new List<AWS_AssociationToolCore.Entities.EventDates>();
            DataTable dt = _Events.GetEventsDatesList(EventId, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventDates objEventDates = new AWS_AssociationToolCore.Entities.EventDates();

                    objEventDates.EventId = Convert.ToInt64(dr["EventId"]);
                    objEventDates.EventDatesId = Convert.ToInt64(dr["EventDatesId"]);
                    objEventDates.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"].ToString()) : DateTime.MinValue);
                    objEventDates.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"].ToString()) : DateTime.MinValue);
                    objEventDates.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEventDates.IsActive = Convert.ToBoolean(dr["IsActive"]);

                    lstEventDates.Add(objEventDates);
                }

            }
            return lstEventDates;
        }

        public Int64 UpdateEventRegistrationTypesStatus(Int64 EventRegistrationTypeId)
        {
            Int64 _status = 0;
            _status = _Events.UpdateEventRegistrationTypesStatus(EventRegistrationTypeId);
            return _status;
        }


        #endregion

        #region API
        public List<AWS_AssociationToolCore.Entities.Events> APIGetEventsList(string Type, Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.GetEventsList(Type, ChapterId, Search, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    //objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? logourl + dr["BannerUrl"].ToString() : "");
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }
        #endregion
         

        #region FE

        public Entities.Events FEGetEventById(Int64 EventId, string EventName, ref List<Entities.EventRegistrationTypes> lstEventRegistrationTypes, ref int status)
        {
            DataSet ds = _Events.FEGetEventById(EventId, EventName, ref status);
            Entities.Events objEvent = new Entities.Events();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objEvent.EventId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventId"]);
                objEvent.BannerUrl = (ds.Tables[0].Rows[0]["BannerUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["BannerUrl"].ToString() : null);
                objEvent.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objEvent.ContactEmail = (ds.Tables[0].Rows[0]["ContactEmail"] != DBNull.Value ? ds.Tables[0].Rows[0]["ContactEmail"].ToString() : null);
                objEvent.EndDate = (ds.Tables[0].Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"]) : DateTime.MinValue);
                objEvent.EventCategoryId = (ds.Tables[0].Rows[0]["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["EventCategoryId"]) : 0);
                objEvent.EventCategory = (ds.Tables[0].Rows[0]["EventCategory"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventCategory"].ToString() : null);
                objEvent.EventDetails = (ds.Tables[0].Rows[0]["EventDetails"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventDetails"].ToString() : null);
                objEvent.EventName = ds.Tables[0].Rows[0]["EventName"].ToString();
                objEvent.IsRegistration = (ds.Tables[0].Rows[0]["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsRegistration"]) : false);
                objEvent.IsCulturalRegistration = (ds.Tables[0].Rows[0]["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsCulturalRegistration"]) : false);
                objEvent.Location = (ds.Tables[0].Rows[0]["Location"] != DBNull.Value ? ds.Tables[0].Rows[0]["Location"].ToString() : null);
                objEvent.MemberCount = (ds.Tables[0].Rows[0]["MemberCount"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["MemberCount"]) : 0);
                objEvent.RegistrationEndDate = (ds.Tables[0].Rows[0]["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationEndDate"]) : DateTime.MinValue);
                objEvent.RegistrationStartDate = (ds.Tables[0].Rows[0]["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationStartDate"]) : DateTime.MinValue);
                objEvent.StartDate = (ds.Tables[0].Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"]) : DateTime.MinValue);
                objEvent.StateName = (ds.Tables[0].Rows[0]["StateName"] != DBNull.Value ? ds.Tables[0].Rows[0]["StateName"].ToString() : null);
                objEvent.ZipCode = (ds.Tables[0].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZipCode"].ToString() : null);
                objEvent.TopLine = (ds.Tables[0].Rows[0]["TopLine"] != DBNull.Value ? ds.Tables[0].Rows[0]["TopLine"].ToString() : null);
                objEvent.PageTitle = (ds.Tables[0].Rows[0]["PageTitle"] != DBNull.Value ? ds.Tables[0].Rows[0]["PageTitle"].ToString() : null);
                objEvent.MetaKeywords = (ds.Tables[0].Rows[0]["MetaKeywords"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaKeywords"].ToString() : null);
                objEvent.MetaDescription = (ds.Tables[0].Rows[0]["MetaDescription"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaDescription"].ToString() : null);
                objEvent.UpdatedTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedTime"]);
            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationTypes objEventRegistrationTypes = new AWS_AssociationToolCore.Entities.EventRegistrationTypes();
                    objEventRegistrationTypes.EventRegistrationTypeId = Convert.ToInt64(dr["EventRegistrationTypeId"].ToString());
                    objEventRegistrationTypes.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventRegistrationTypes.RegistrationTitle = dr["RegistrationTitle"].ToString();
                    objEventRegistrationTypes.RegCount = (dr["RegCount"] != DBNull.Value ? Convert.ToInt64(dr["RegCount"].ToString()) : 0);
                    objEventRegistrationTypes.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEventRegistrationTypes.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objEventRegistrationTypes.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"].ToString()) : 0);

                    lstEventRegistrationTypes.Add(objEventRegistrationTypes);
                }
            }

            return objEvent;
        }

        public Entities.Events FEGetEventByCategoryId(Int64 EventId, Int64 RegistrationCategoryId, string EventName, ref List<Entities.EventRegistrationTypes> lstEventRegistrationTypes, ref int status)
        {
            DataSet ds = _Events.FEGetEventByCategoryId(EventId, RegistrationCategoryId, EventName, ref status);
            Entities.Events objEvent = new Entities.Events();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objEvent.EventId = Convert.ToInt64(ds.Tables[0].Rows[0]["EventId"]);
                objEvent.BannerUrl = (ds.Tables[0].Rows[0]["BannerUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["BannerUrl"].ToString() : null);
                objEvent.City = (ds.Tables[0].Rows[0]["City"] != DBNull.Value ? ds.Tables[0].Rows[0]["City"].ToString() : null);
                objEvent.ContactEmail = (ds.Tables[0].Rows[0]["ContactEmail"] != DBNull.Value ? ds.Tables[0].Rows[0]["ContactEmail"].ToString() : null);
                objEvent.EndDate = (ds.Tables[0].Rows[0]["EndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["EndDate"]) : DateTime.MinValue);
                objEvent.EventCategoryId = (ds.Tables[0].Rows[0]["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["EventCategoryId"]) : 0);
                objEvent.EventCategory = (ds.Tables[0].Rows[0]["EventCategory"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventCategory"].ToString() : null);
                objEvent.EventDetails = (ds.Tables[0].Rows[0]["EventDetails"] != DBNull.Value ? ds.Tables[0].Rows[0]["EventDetails"].ToString() : null);
                objEvent.EventName = ds.Tables[0].Rows[0]["EventName"].ToString();
                objEvent.IsRegistration = (ds.Tables[0].Rows[0]["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsRegistration"]) : false);
                objEvent.IsCulturalRegistration = (ds.Tables[0].Rows[0]["IsCulturalRegistration"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsCulturalRegistration"]) : false);
                objEvent.Location = (ds.Tables[0].Rows[0]["Location"] != DBNull.Value ? ds.Tables[0].Rows[0]["Location"].ToString() : null);
                objEvent.MemberCount = (ds.Tables[0].Rows[0]["MemberCount"] != DBNull.Value ? Convert.ToInt32(ds.Tables[0].Rows[0]["MemberCount"]) : 0);
                objEvent.RegistrationEndDate = (ds.Tables[0].Rows[0]["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationEndDate"]) : DateTime.MinValue);
                objEvent.RegistrationStartDate = (ds.Tables[0].Rows[0]["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["RegistrationStartDate"]) : DateTime.MinValue);
                objEvent.StartDate = (ds.Tables[0].Rows[0]["StartDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["StartDate"]) : DateTime.MinValue);
                objEvent.StateName = (ds.Tables[0].Rows[0]["StateName"] != DBNull.Value ? ds.Tables[0].Rows[0]["StateName"].ToString() : null);
                objEvent.ZipCode = (ds.Tables[0].Rows[0]["ZipCode"] != DBNull.Value ? ds.Tables[0].Rows[0]["ZipCode"].ToString() : null);
                objEvent.TopLine = (ds.Tables[0].Rows[0]["TopLine"] != DBNull.Value ? ds.Tables[0].Rows[0]["TopLine"].ToString() : null);
                objEvent.PageTitle = (ds.Tables[0].Rows[0]["PageTitle"] != DBNull.Value ? ds.Tables[0].Rows[0]["PageTitle"].ToString() : null);
                objEvent.MetaKeywords = (ds.Tables[0].Rows[0]["MetaKeywords"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaKeywords"].ToString() : null);
                objEvent.MetaDescription = (ds.Tables[0].Rows[0]["MetaDescription"] != DBNull.Value ? ds.Tables[0].Rows[0]["MetaDescription"].ToString() : null);
                objEvent.UpdatedTime = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedTime"]);
                //objEvent.IsFood = (ds.Tables[0].Rows[0]["IsFood"] != DBNull.Value ? Convert.ToBoolean(ds.Tables[0].Rows[0]["IsFood"]) : false);
                objEvent.MemberAmount = (ds.Tables[0].Rows[0]["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["MemberAmount"].ToString()) : 0);
                objEvent.NAmount = (ds.Tables[0].Rows[0]["NAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["NAmount"].ToString()) : 0);
                objEvent.CategoryName = (ds.Tables[0].Rows[0]["CategoryName"] != DBNull.Value ? ds.Tables[0].Rows[0]["CategoryName"].ToString() : null);
                objEvent.UpdatedBy = (ds.Tables[0].Rows[0]["UpdatedBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["UpdatedBy"].ToString() : null);
            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationTypes objEventRegistrationTypes = new AWS_AssociationToolCore.Entities.EventRegistrationTypes();
                    objEventRegistrationTypes.EventRegistrationTypeId = Convert.ToInt64(dr["EventRegistrationTypeId"].ToString());
                    objEventRegistrationTypes.EventId = Convert.ToInt64(dr["EventId"].ToString());
                    objEventRegistrationTypes.RegistrationTitle = dr["RegistrationTitle"].ToString();
                    objEventRegistrationTypes.RegCount = (dr["RegCount"] != DBNull.Value ? Convert.ToInt64(dr["RegCount"].ToString()) : 0);
                    objEventRegistrationTypes.OrderNo = (dr["OrderNo"] != DBNull.Value ? Convert.ToInt32(dr["OrderNo"].ToString()) : 0);
                    objEventRegistrationTypes.IsActive = Convert.ToBoolean(dr["IsActive"].ToString());
                    objEventRegistrationTypes.MemberAmount = (dr["MemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["MemberAmount"].ToString()) : 0);
                    objEventRegistrationTypes.NonMemberAmount = (dr["NonMemberAmount"] != DBNull.Value ? Convert.ToDecimal(dr["NonMemberAmount"].ToString()) : 0);

                    lstEventRegistrationTypes.Add(objEventRegistrationTypes);
                }
            }

            return objEvent;
        }

        public List<AWS_AssociationToolCore.Entities.Events> FEGetEventsList(string Type, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<Entities.Events>();
            DataTable dt = _Events.FEGetEventsList(Type, Search, Sort, PageNo, Items, ref Total);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objEvent = new AWS_AssociationToolCore.Entities.Events();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);
                    objEvent.StartDate = (dr["StartDate"] != DBNull.Value ? Convert.ToDateTime(dr["StartDate"]) : DateTime.MinValue);
                    objEvent.EndDate = (dr["EndDate"] != DBNull.Value ? Convert.ToDateTime(dr["EndDate"]) : DateTime.MinValue);
                    objEvent.RegistrationStartDate = (dr["RegistrationStartDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationStartDate"]) : DateTime.MinValue);
                    objEvent.RegistrationEndDate = (dr["RegistrationEndDate"] != DBNull.Value ? Convert.ToDateTime(dr["RegistrationEndDate"]) : DateTime.MinValue);
                    objEvent.BannerUrl = (dr["BannerUrl"] != DBNull.Value ? dr["BannerUrl"].ToString() : null);
                    objEvent.EventCategoryId = (dr["EventCategoryId"] != DBNull.Value ? Convert.ToInt32(dr["EventCategoryId"]) : 0);
                    objEvent.MemberCount = (dr["MemberCount"] != DBNull.Value ? Convert.ToInt32(dr["MemberCount"]) : 0);
                    objEvent.Location = (dr["Location"] != DBNull.Value ? dr["Location"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.StateName = (dr["StateName"] != DBNull.Value ? dr["StateName"].ToString() : null);
                    objEvent.ZipCode = (dr["ZipCode"] != DBNull.Value ? dr["ZipCode"].ToString() : null);
                    objEvent.EventDetails = (dr["EventDetails"] != DBNull.Value ? dr["EventDetails"].ToString() : null);
                    objEvent.IsRegistration = (dr["IsRegistration"] != DBNull.Value ? Convert.ToBoolean(dr["IsRegistration"].ToString()) : false);
                    objEvent.ContactEmail = (dr["ContactEmail"] != DBNull.Value ? dr["ContactEmail"].ToString() : null);
                    objEvent.EventCategory = (dr["EventCategory"] != DBNull.Value ? dr["EventCategory"].ToString() : null);
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);

                    //objEvent.IsChild = (dr["IsChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsChild"].ToString()) : false);
                    //objEvent.ChildAmount = (dr["ChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["ChildAmount"]) : 0);
                    //objEvent.NonChildAmount = (dr["NonChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonChildAmount"]) : 0);
                    //objEvent.IsAdult = (dr["IsAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsAdult"].ToString()) : false);
                    //objEvent.AdultAmount = (dr["AdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["AdultAmount"]) : 0);
                    //objEvent.NonAdultAmount = (dr["NonAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonAdultAmount"]) : 0);
                    //objEvent.IsCouple = (dr["IsCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsCouple"].ToString()) : false);
                    //objEvent.CoupleAmount = (dr["CoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["CoupleAmount"]) : 0);
                    //objEvent.NonCoupleAmount = (dr["NonCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonCoupleAmount"]) : 0);
                    //objEvent.IsFamily = (dr["IsFamily"] != DBNull.Value ? Convert.ToBoolean(dr["IsFamily"].ToString()) : false);
                    //objEvent.FamilyAmount = (dr["FamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["FamilyAmount"]) : 0);
                    //objEvent.NonFamilyAmount = (dr["NonFamilyAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonFamilyAmount"]) : 0);
                    //objEvent.IsVIP = (dr["IsVIP"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIP"].ToString()) : false);
                    //objEvent.VIPAmount = (dr["VIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPAmount"]) : 0);
                    //objEvent.NonVIPAmount = (dr["NonVIPAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPAmount"]) : 0);
                    //objEvent.IsVIPChild = (dr["IsVIPChild"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPChild"].ToString()) : false);
                    //objEvent.VIPChildAmount = (dr["VIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPChildAmount"]) : 0);
                    //objEvent.NonVIPChildAmount = (dr["NonVIPChildAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPChildAmount"]) : 0);
                    //objEvent.IsVIPSingleAdult = (dr["IsVIPSingleAdult"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPSingleAdult"].ToString()) : false);
                    //objEvent.VIPSingleAdultAmount = (dr["VIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPSingleAdultAmount"]) : 0);
                    //objEvent.NonVIPSingleAdultAmount = (dr["NonVIPSingleAdultAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPSingleAdultAmount"]) : 0);
                    //objEvent.IsVIPCouple = (dr["IsVIPCouple"] != DBNull.Value ? Convert.ToBoolean(dr["IsVIPCouple"].ToString()) : false);
                    //objEvent.VIPCoupleAmount = (dr["VIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["VIPCoupleAmount"]) : 0);
                    //objEvent.NonVIPCoupleAmount = (dr["NonVIPCoupleAmount"] != DBNull.Value ? Convert.ToInt64(dr["NonVIPCoupleAmount"]) : 0);

                    lstEvents.Add(objEvent);
                }

            }
            return lstEvents;
        }

        public List<AWS_AssociationToolCore.Entities.EventUserInfo> FEGetEventMemberInfoListByVariable(Int64 MemberId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventUserInfo> lstEventUserInfo = new List<AWS_AssociationToolCore.Entities.EventUserInfo>();
            DataTable dt = _Events.FEGetEventMemberInfoListByVariable(MemberId, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Events.FEGetEventMemberInfoListByVariable(MemberId, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventUserInfo objEvent = new AWS_AssociationToolCore.Entities.EventUserInfo();

                    objEvent.RId = Convert.ToInt64(dr["Rid"]);
                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventUserInfoId = Convert.ToInt64(dr["EventUserInfoId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.LastName = dr["LastName"].ToString();
                    objEvent.EventCategory = dr["EventCategory"].ToString();
                    objEvent.FirstName = dr["FirstName"].ToString();
                    objEvent.Email = dr["Email"].ToString();
                    objEvent.Gender = (dr["Gender"] != DBNull.Value ? dr["Gender"].ToString() : null);
                    objEvent.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objEvent.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objEvent.ItemName = dr["ItemName"].ToString();
                    objEvent.ItemCategory = dr["ItemCategory"].ToString();
                    objEvent.ItemDescription = dr["ItemDescription"].ToString();
                    objEvent.ItemDuration = (dr["ItemDuration"] != DBNull.Value ? Convert.ToInt32(dr["ItemDuration"].ToString()) : 0);
                    objEvent.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objEvent.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objEvent.UpdatedBy = dr["UpdatedBy"].ToString();
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.InsertedTime = (dr["InsertedTime"] != DBNull.Value ? Convert.ToDateTime(dr["InsertedTime"].ToString()) : DateTime.MinValue);
                    objEvent.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objEvent.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null);

                    objEvent.AmountPaid = (dr["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dr["AmountPaid"].ToString()) : 0);
                    objEvent.TransactionId = (dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null);
                    objEvent.UpdatedTime = (dr["UpdatedTime"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedTime"].ToString()) : DateTime.MinValue);
                    objEvent.PaymentDate = (dr["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dr["PaymentDate"].ToString()) : DateTime.MinValue);

                    lstEventUserInfo.Add(objEvent);
                }
            }
            return lstEventUserInfo;
        }

        #endregion

        #region EventTicketMaster

        public List<AWS_AssociationToolCore.Entities.EventRegistrationCounts> EventTicketMasterGetListByVariable(Int64 EventId, string Mobile, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventRegistrationCounts> lstEventRegistrationCounts = new List<AWS_AssociationToolCore.Entities.EventRegistrationCounts>();
            DataTable dt = _Events.EventTicketMasterGetListByVariable(EventId, Mobile, Search, Sort, PageNo, Items, ref Total);
            if (dt.Rows.Count == 0 && PageNo != 0)
            {
                dt = _Events.EventTicketMasterGetListByVariable(EventId, Mobile, Search, Sort, PageNo - 1, Items, ref Total);
            }
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventRegistrationCounts objRegistrationCount = new AWS_AssociationToolCore.Entities.EventRegistrationCounts();

                    objRegistrationCount.RId = Convert.ToInt64(dr["Rid"]);
                    objRegistrationCount.EventRegCountId = Convert.ToInt64(dr["EventRegCountId"]);
                    objRegistrationCount.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt32(dr["EventId"].ToString()) : 0);
                    objRegistrationCount.EventUserInfoId = (dr["EventUserInfoId"] != DBNull.Value ? Convert.ToInt32(dr["EventUserInfoId"].ToString()) : 0);
                    objRegistrationCount.EventRegistrationTypeId = (dr["EventRegistrationTypeId"] != DBNull.Value ? Convert.ToInt32(dr["EventRegistrationTypeId"].ToString()) : 0);
                    objRegistrationCount.Count = (dr["Count"] != DBNull.Value ? Convert.ToInt32(dr["Count"].ToString()) : 0);
                    objRegistrationCount.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"].ToString()) : 0);
                    objRegistrationCount.VisitCount = (dr["VisitCount"] != DBNull.Value ? Convert.ToInt32(dr["VisitCount"].ToString()) : 0);
                    objRegistrationCount.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    objRegistrationCount.Field2 = (dr["Field2"] != DBNull.Value ? Convert.ToBoolean(dr["Field2"].ToString()) : false);
                    objRegistrationCount.Email = (dr["Email"] != DBNull.Value ? dr["Email"].ToString() : null);
                    objRegistrationCount.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objRegistrationCount.TransactionId = (dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null);
                    objRegistrationCount.RegistrationTitle = (dr["RegistrationTitle"] != DBNull.Value ? dr["RegistrationTitle"].ToString() : null);

                    lstEventRegistrationCounts.Add(objRegistrationCount);
                }
            }
            return lstEventRegistrationCounts;
        }

        public Int64 EventTicketMasterVisitCountUpdate(int VisitCount, Int64 EventRegCountId)
        {
            Int64 _status = 0;
            _status = _Events.EventTicketMasterVisitCountUpdate(VisitCount, EventRegCountId);
            return _status;
        }

        public Entities.EventUserInfo GetEventUserInfoFullDetailsByMobile(Int64 EventUserInfoGetById, Int64 EventId, string Mobile, string Search, ref int status)
        {
            DataTable dt = _Events.GetEventUserInfoFullDetailsByMobile(EventUserInfoGetById, EventId, Mobile, Search, ref status);
            Entities.EventUserInfo objEventUserInfo = new Entities.EventUserInfo();
            List<Entities.EventParticipants> lstEventParticipants = new List<Entities.EventParticipants>();

            if (dt.Rows.Count == 1)
            {
                objEventUserInfo.EventId = Convert.ToInt64(dt.Rows[0]["EventId"]);
                objEventUserInfo.EventName = dt.Rows[0]["EventName"].ToString();
                objEventUserInfo.EventUserInfoId = Convert.ToInt64(dt.Rows[0]["EventUserInfoId"]);
                objEventUserInfo.LastName = dt.Rows[0]["LastName"].ToString();
                objEventUserInfo.EventCategoryId = Convert.ToInt64(dt.Rows[0]["EventCategoryId"]);
                objEventUserInfo.EventCategory = dt.Rows[0]["EventCategory"].ToString();
                objEventUserInfo.FirstName = dt.Rows[0]["FirstName"].ToString();
                objEventUserInfo.Email = dt.Rows[0]["Email"].ToString();
                objEventUserInfo.Gender = (dt.Rows[0]["Gender"] != DBNull.Value ? dt.Rows[0]["Gender"].ToString() : null);
                objEventUserInfo.Address = (dt.Rows[0]["Address"] != DBNull.Value ? dt.Rows[0]["Address"].ToString() : null);
                objEventUserInfo.City = (dt.Rows[0]["City"] != DBNull.Value ? dt.Rows[0]["City"].ToString() : null);
                objEventUserInfo.State = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString() : null);
                objEventUserInfo.Mobile = (dt.Rows[0]["Mobile"] != DBNull.Value ? dt.Rows[0]["Mobile"].ToString() : null);
                objEventUserInfo.ItemName = dt.Rows[0]["ItemName"].ToString();
                objEventUserInfo.ItemCategory = dt.Rows[0]["ItemCategory"].ToString();
                objEventUserInfo.ItemDescription = dt.Rows[0]["ItemDescription"].ToString();
                objEventUserInfo.OtherPaymentId = (dt.Rows[0]["OtherPaymentId"] != DBNull.Value ? dt.Rows[0]["OtherPaymentId"].ToString() : null);
                objEventUserInfo.ItemDuration = (dt.Rows[0]["ItemDuration"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["ItemDuration"].ToString()) : 0);
                objEventUserInfo.IsApproved = Convert.ToBoolean(dt.Rows[0]["IsApproved"]);
                objEventUserInfo.DocumentUrl = (dt.Rows[0]["DocumentUrl"] != DBNull.Value ? dt.Rows[0]["DocumentUrl"].ToString() : null);
                objEventUserInfo.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objEventUserInfo.UpdatedTime = Convert.ToDateTime(dt.Rows[0]["UpdatedTime"]);
                objEventUserInfo.TicketTypesWithCount = (dt.Rows[0]["TicketTypesWithCount"] != DBNull.Value ? dt.Rows[0]["TicketTypesWithCount"].ToString() : null);
                objEventUserInfo.TotalAmount = (dt.Rows[0]["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["TotalAmount"].ToString()) : 0);
                objEventUserInfo.PaymentMethod = (dt.Rows[0]["PaymentMethod"] != DBNull.Value ? dt.Rows[0]["PaymentMethod"].ToString() : null);
                objEventUserInfo.PaymentStatus = (dt.Rows[0]["PaymentStatus"] != DBNull.Value ? dt.Rows[0]["PaymentStatus"].ToString() : null);
                objEventUserInfo.TransactionId = (dt.Rows[0]["TransactionId"] != DBNull.Value ? dt.Rows[0]["TransactionId"].ToString() : null);
                objEventUserInfo.InsertedBy = (dt.Rows[0]["InsertedBy"] != DBNull.Value ? dt.Rows[0]["InsertedBy"].ToString() : null);
                objEventUserInfo.PaymentStatusId = (dt.Rows[0]["PaymentStatusId"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["PaymentStatusId"]) : 0);
                objEventUserInfo.PaymentMethodId = (dt.Rows[0]["PaymentMethodId"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["PaymentMethodId"]) : 0);

            }



            return objEventUserInfo;
        }


        public List<AWS_AssociationToolCore.Entities.Events> GetEventsByCategory(Int64 RegistrationCategoryId, ref int status)
        {
            List<AWS_AssociationToolCore.Entities.Events> lstEvents = new List<AWS_AssociationToolCore.Entities.Events>();
            DataTable dt = _Events.GetEventsByCategory(RegistrationCategoryId, ref status);

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.Events objlstEvents = new AWS_AssociationToolCore.Entities.Events();


                    objlstEvents.EventId = (dr["EventId"] != DBNull.Value ? Convert.ToInt64(dr["EventId"]) : 0);
                    objlstEvents.EventName = (dr["EventName"] != DBNull.Value ? dr["EventName"].ToString() : null);


                    lstEvents.Add(objlstEvents);
                }

            }
            return lstEvents;
        }
        #endregion

        public void EventRegistrationsAddRequirements(ref List<Entities.PaymentMethods> lstPaymentMethods, ref List<Entities.PaymentStatus> lstPaymentStatus, ref int status)
        {
            AWS_AssociationToolCore.Entities.EventUserInfo objEventRegistrations = new AWS_AssociationToolCore.Entities.EventUserInfo();
            DataSet ds = new DataSet();
            ds = _Events.EventRegistrationsAddRequirements(ref status);

            //PaymentMethods
            if (ds.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentMethods objPaymentMethods = new AWS_AssociationToolCore.Entities.PaymentMethods();

                    objPaymentMethods.PaymentMethodId = Convert.ToInt64(dr["PaymentMethodId"].ToString());
                    objPaymentMethods.PaymentMethod = dr["PaymentMethod"].ToString();

                    lstPaymentMethods.Add(objPaymentMethods);
                }
            }

            //PaymentStatus
            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.PaymentStatus objPaymentStatus = new AWS_AssociationToolCore.Entities.PaymentStatus();

                    objPaymentStatus.PaymentStatusId = Convert.ToInt64(dr["PaymentStatusId"].ToString());
                    objPaymentStatus.PaymentStatus1 = dr["PaymentStatus"].ToString();

                    lstPaymentStatus.Add(objPaymentStatus);
                }
            }
            
        }


        public Int64 UpdateEventRegTicketsstatus(Entities.EventComments objEvents)
        {
            Int64 _status = 0;
            if (objEvents != null)
            {
                _status = _Events.UpdateEventRegTicketsstatus(objEvents);
            }
            return _status;
        }
        public Int64 InsertEventComments(Entities.EventComments objEvents)
        {
            Int64 _status = 0;
            if (objEvents != null)
            {
                _status = _Events.InsertEventComments(objEvents);
            }
            return _status;
        }






        public Int64 UpdateWebsiteBannersStatus(Int64 EventId)
        {
            Int64 _status = 0;
            _status = _Events.UpdateWebsiteBannersStatus(EventId);
            return _status;
        }




        public List<AWS_AssociationToolCore.Entities.EventUserInfo> FEEventsRegistrationGetListByEmail(string Email,Int64 ChapterId, ref int Total)
        {
            List<AWS_AssociationToolCore.Entities.EventUserInfo> lstEventUserInfo = new List<AWS_AssociationToolCore.Entities.EventUserInfo>();
            DataTable dt = _Events.FEEventsRegistrationGetListByEmail(Email, ChapterId, ref Total);
            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    AWS_AssociationToolCore.Entities.EventUserInfo objEvent = new AWS_AssociationToolCore.Entities.EventUserInfo();

                    objEvent.EventId = Convert.ToInt64(dr["EventId"]);
                    objEvent.EventUserInfoId = Convert.ToInt64(dr["EventUserInfoId"]);
                    objEvent.EventName = dr["EventName"].ToString();
                    objEvent.LastName = dr["LastName"].ToString();
                    objEvent.EventCategory = dr["EventCategory"].ToString();
                    objEvent.FirstName = dr["FirstName"].ToString();
                    objEvent.Email = dr["Email"].ToString();
                    objEvent.Gender = (dr["Gender"] != DBNull.Value ? dr["Gender"].ToString() : null);
                    objEvent.Address = (dr["Address"] != DBNull.Value ? dr["Address"].ToString() : null);
                    objEvent.City = (dr["City"] != DBNull.Value ? dr["City"].ToString() : null);
                    objEvent.State = (dr["State"] != DBNull.Value ? dr["State"].ToString() : null);
                    objEvent.Mobile = (dr["Mobile"] != DBNull.Value ? dr["Mobile"].ToString() : null);
                    objEvent.ItemName = dr["ItemName"].ToString();
                    objEvent.ItemCategory = dr["ItemCategory"].ToString();
                    objEvent.ItemDescription = dr["ItemDescription"].ToString();
                    objEvent.ItemDuration = (dr["ItemDuration"] != DBNull.Value ? Convert.ToInt32(dr["ItemDuration"].ToString()) : 0);
                    objEvent.IsApproved = Convert.ToBoolean(dr["IsApproved"]);
                    objEvent.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objEvent.UpdatedBy = dr["UpdatedBy"].ToString();
                    objEvent.UpdatedTime = Convert.ToDateTime(dr["UpdatedTime"]);
                    objEvent.InsertedTime = (dr["InsertedTime"] != DBNull.Value ? Convert.ToDateTime(dr["InsertedTime"].ToString()) : DateTime.MinValue);
                    objEvent.PaymentMethod = (dr["PaymentMethod"] != DBNull.Value ? dr["PaymentMethod"].ToString() : null);
                    objEvent.PaymentStatus = (dr["PaymentStatus"] != DBNull.Value ? dr["PaymentStatus"].ToString() : null);
                    objEvent.TransactionId = (dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null);
                    objEvent.AmountPaid = (dr["AmountPaid"] != DBNull.Value ? Convert.ToDecimal(dr["AmountPaid"].ToString()) : 0);
                    objEvent.PaymentDate = (dr["PaymentDate"] != DBNull.Value ? Convert.ToDateTime(dr["PaymentDate"].ToString()) : DateTime.MinValue);

                    lstEventUserInfo.Add(objEvent);
                }

            }
            return lstEventUserInfo;
        }

        #region EventCheckInInsert

        public Int64 EventCheckInInsert(Entities.EventRegCheckIn objEventCheckIn)
        {
            Int64 _status = 0;
            if (objEventCheckIn != null)
            {
                _status = _Events.EventCheckInInsert(objEventCheckIn);
            }
            return _status;
        }
        public Entities.EventRegTickets EventUserInfoIndividualGetById(
    Int64 EventUserInfoId,
    Int64 EventId,
    Int64 EventRegTicketId,
    ref int status)
        {
            DataTable dt = _Events.EventUserInfoIndividualGetById(EventUserInfoId, EventId, EventRegTicketId, ref status);
            Entities.EventRegTickets objEventRegTickets = new Entities.EventRegTickets();

            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];

                // Event & Ticket Info
                objEventRegTickets.EventRegTicketId = row["EventRegTicketId"] != DBNull.Value ? Convert.ToInt64(row["EventRegTicketId"]) : 0;
                objEventRegTickets.EventRegCountId = row["EventRegCountId"] != DBNull.Value ? Convert.ToInt64(row["EventRegCountId"]) : 0;
                objEventRegTickets.EventRegistrationTypeId = row["EventRegistrationTypeId"] != DBNull.Value ? Convert.ToInt64(row["EventRegistrationTypeId"]) : 0;
                objEventRegTickets.EventId = row["EventId"] != DBNull.Value ? Convert.ToInt64(row["EventId"]) : 0;
                objEventRegTickets.EventUserInfoId = row["EventUserInfoId"] != DBNull.Value ? Convert.ToInt64(row["EventUserInfoId"]) : 0;

                // User Info
                objEventRegTickets.FirstName = row["FirstName"]?.ToString();
                objEventRegTickets.LastName = row["LastName"]?.ToString();
                objEventRegTickets.Email = row["Email"]?.ToString();
                objEventRegTickets.Gender = row["Gender"] != DBNull.Value ? row["Gender"].ToString() : null;
                objEventRegTickets.Address = row["Address"] != DBNull.Value ? row["Address"].ToString() : null;
                //objEventRegTickets.Address2 = row["Address2"] != DBNull.Value ? row["Address2"].ToString() : null;
                objEventRegTickets.City = row["City"] != DBNull.Value ? row["City"].ToString() : null;
                objEventRegTickets.State = row["State"] != DBNull.Value ? row["State"].ToString() : null;
                //objEventRegTickets.ZipCode = row["ZipCode"] != DBNull.Value ? row["ZipCode"].ToString() : null;
                objEventRegTickets.PhoneNo = row["Mobile"]?.ToString();

                // Item / Event Details
                objEventRegTickets.ItemName = row["ItemName"]?.ToString();
                objEventRegTickets.ItemCategory = row["ItemCategory"]?.ToString();
                objEventRegTickets.ItemDescription = row["ItemDescription"]?.ToString();
                objEventRegTickets.ItemDuration = row["ItemDuration"]?.ToString();
                objEventRegTickets.EventName = row["EventName"]?.ToString();

                // Event Category
                objEventRegTickets.EventCategoryId = row["EventCategoryId"] != DBNull.Value ? Convert.ToInt64(row["EventCategoryId"]) : 0;
                objEventRegTickets.EventCategory = row["EventCategory"]?.ToString();

                // Comments & Documents
                objEventRegTickets.Comments = row["Comments"]?.ToString();
                objEventRegTickets.DocumentUrl = row["DocumentUrl"]?.ToString();
                objEventRegTickets.ReceiptUrl = row["ReceiptUrl"]?.ToString();

                // Payment Info
                objEventRegTickets.OtherPaymentId = row["OtherPaymentId"]?.ToString();
                objEventRegTickets.Zelle = row["Zelle"]?.ToString();
                objEventRegTickets.ZellePartnerName = row["ZellePartnerName"]?.ToString();
                objEventRegTickets.ChequeHolderName = row["ChequeHolderName"]?.ToString();
                objEventRegTickets.UpdatedTime = row["UpdatedTime"] != DBNull.Value ? Convert.ToDateTime(row["UpdatedTime"]) : DateTime.MinValue;
                objEventRegTickets.PaymentMethod = row["PaymentMethod"]?.ToString();
                objEventRegTickets.PaymentStatus = row["PaymentStatus"]?.ToString();
                objEventRegTickets.PaidAmount = row["PaidAmount"] != DBNull.Value ? Convert.ToDecimal(row["PaidAmount"]) : 0;
                objEventRegTickets.PurchasedCount = row["PurchasedCount"] != DBNull.Value ? Convert.ToInt32(row["PurchasedCount"]) : 0;

                // Approval & Ticket Status
                objEventRegTickets.IsApproved = row["IsApproved"] != DBNull.Value ? Convert.ToBoolean(row["IsApproved"]) : false;
                objEventRegTickets.Status = row["TicketStatus"]?.ToString();
                objEventRegTickets.TicketComments = row["TicketComments"]?.ToString();

                // Check-in
                objEventRegTickets.CheckInDate = row["CheckInDate"] != DBNull.Value ? Convert.ToDateTime(row["CheckInDate"]) : DateTime.MinValue;

                // Registration
                objEventRegTickets.RegistrationTitle = row["RegistrationTitle"]?.ToString();

                objEventRegTickets.TicketId = row["TicketId"] != DBNull.Value ? row["TicketId"].ToString() : null;
                objEventRegTickets.SequenceNo = row["SequenceNo"] != DBNull.Value ? Convert.ToInt32(row["SequenceNo"]) : 0;
            }

            return objEventRegTickets;
        }
        public Int64 UpdateSeatAllocation(long ticketId, string seatNo, string InsertedBy, bool Notify)
        {
            Int64 _status = 0;
            if (ticketId != 0)
            {
                _status = _Events.UpdateSeatAllocation(ticketId, seatNo, InsertedBy, Notify);
            }
            return _status;
        }

        #endregion




    }
}
