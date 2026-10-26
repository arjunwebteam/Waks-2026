using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace AWS_AssociationToolCore.BLL
{
  public  class Services
    {

        DAL.Services _Services = new DAL.Services();

        #region Methods

        public Int64 DeleteServices(Int64 ServiceId)
        {
            Int64 _status = 0;
            if (ServiceId != 0)
            {
                _status = _Services.DeleteServices(ServiceId);
            }
            return _status;
        }

        public Int64 ServicesUpdateStatus(Int64 ServiceId)
        {
            Int64 _status = 0;
            if (ServiceId != 0)
            {
                _status = _Services.ServicesUpdateStatus(ServiceId);
            }
            return _status;
        }

        public Int64 UpdateServicesOrderNo(int DisplayOrder, Int64 ServiceId)
        {
            Int64 _status = 0;
            _status = _Services.UpdateServicesOrderNo(DisplayOrder, ServiceId);
            return _status;
        }

        public Int64 InsertServices(Entities.Services objServices, ref string DocumentUrl)
        {
            Int64 _status = 0;
            if (objServices != null)
            {
                _status = _Services.InsertServices(objServices, ref DocumentUrl);
            }
            return _status;
        }

        #endregion

        #region Entity Loading

        public List<Entities.Services> GetServicesListByVariable(Int64 ChapterId, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _Services.GetServicesListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
            List<Entities.Services> lstServices = new List<Entities.Services>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _Services.GetServicesListByVariable(ChapterId, Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Services objServices = new Entities.Services();

                    objServices.RId = Convert.ToInt64(dr["Rid"]);
                    objServices.ServiceId = Convert.ToInt64(dr["ServiceId"]);
                    objServices.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"].ToString()) : 0);
                    objServices.ServiceTitle = (dr["ServiceTitle"] != DBNull.Value ? dr["ServiceTitle"].ToString() : null);
                    //objServices.ServiceType = (dr["ServiceType"] != DBNull.Value ? dr["ServiceType"].ToString() : null);
                    //objServices.OrganisedBy = (dr["OrganisedBy"] != DBNull.Value ? dr["OrganisedBy"].ToString() : null);
                    //objServices.Purpose = (dr["Purpose"] != DBNull.Value ? dr["Purpose"].ToString() : null);
                    //objServices.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
                    //objServices.ShortDescription = (dr["ShortDescription"] != DBNull.Value ? dr["ShortDescription"].ToString() : null);
                    //objServices.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    //objServices.ExpiryDate = (dr["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ExpiryDate"].ToString()) : DateTime.MinValue);
                    //objServices.EstimationAmount = (dr["EstimationAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EstimationAmount"].ToString()) : 0);
                    //objServices.ServiceStatus = (dr["ServiceStatus"] != DBNull.Value ? dr["ServiceStatus"].ToString() : null);
                    objServices.IsDisplay = (dr["IsDisplay"] != DBNull.Value ? Convert.ToBoolean(dr["IsDisplay"].ToString()) : false);
                    objServices.ServiceDonationsCount = (dr["ServiceDonationsCount"] != DBNull.Value ? Convert.ToInt64(dr["ServiceDonationsCount"].ToString()) : 0);
                    objServices.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    //objServices.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    //objServices.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                    //objServices.UpdatedBy = dr["UpdatedBy"].ToString();
                    //objServices.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    //objServices.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);
                    //objServices.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
                    //objServices.ServiceTypeName = (dr["ServiceTypeName"] != DBNull.Value ? dr["ServiceTypeName"].ToString() : null);
                    //objServices.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : null);
                    //objServices.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"].ToString()) : 0);
                    //objServices.ServiceDonationsCount = (dr["ServiceDonationsCount"] != DBNull.Value ? Convert.ToInt64(dr["ServiceDonationsCount"].ToString()) : 0);
                    ////objServices.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["DisplayOrder"].ToString()) : 0);
                    //objServices.DisplayOrder = (dr["DisplayOrder"] != DBNull.Value ? Convert.ToInt32(dr["DisplayOrder"]) : 0);
                    lstServices.Add(objServices);
                }
            }
            return lstServices;
        }

        public Entities.Services GetServicesById(Int64 ServicesId, ref int status)
        {
            DataTable dt = _Services.GetServicesById(ServicesId, ref status);
            Entities.Services objServices = new Entities.Services();

            if (dt.Rows.Count == 1)
            {
                objServices.ServiceId = Convert.ToInt64(dt.Rows[0]["ServiceId"]);
                objServices.CategoryId = (dt.Rows[0]["CategoryId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["CategoryId"]) : 0);
                objServices.ChapterId = (dt.Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(dt.Rows[0]["ChapterId"]) : 0);
                objServices.ServiceTitle = (dt.Rows[0]["ServiceTitle"] != DBNull.Value ? dt.Rows[0]["ServiceTitle"].ToString() : null);
                objServices.ServiceType = (dt.Rows[0]["ServiceType"] != DBNull.Value ? dt.Rows[0]["ServiceType"].ToString() : null);
                objServices.OrganisedBy = (dt.Rows[0]["OrganisedBy"] != DBNull.Value ? dt.Rows[0]["OrganisedBy"].ToString() : null);
                objServices.Purpose = (dt.Rows[0]["Purpose"] != DBNull.Value ? dt.Rows[0]["Purpose"].ToString() : null);
                objServices.ShortDescription = (dt.Rows[0]["ShortDescription"] != DBNull.Value ? dt.Rows[0]["ShortDescription"].ToString() : null);
                objServices.Description = (dt.Rows[0]["Description"] != DBNull.Value ? dt.Rows[0]["Description"].ToString() : null);
                objServices.DocumentUrl = (dt.Rows[0]["DocumentUrl"] != DBNull.Value ? dt.Rows[0]["DocumentUrl"].ToString() : null);
                objServices.ExpiryDate = (dt.Rows[0]["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["ExpiryDate"].ToString()) : DateTime.MinValue);
                objServices.EstimationAmount = (dt.Rows[0]["EstimationAmount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["EstimationAmount"].ToString()) : 0);
                objServices.ServiceStatus = (dt.Rows[0]["ServiceStatus"] != DBNull.Value ? dt.Rows[0]["ServiceStatus"].ToString() : null);
                objServices.IsDisplay = Convert.ToBoolean(dt.Rows[0]["IsDisplay"].ToString());
                objServices.Field1 = (dt.Rows[0]["Field1"] != DBNull.Value ? dt.Rows[0]["Field1"].ToString() : null);
                objServices.Field2 = (dt.Rows[0]["Field2"] != DBNull.Value ? dt.Rows[0]["Field2"].ToString() : null);
                objServices.UpdatedBy = dt.Rows[0]["UpdatedBy"].ToString();
                objServices.UpdatedDate = Convert.ToDateTime(dt.Rows[0]["UpdatedDate"]);
                objServices.ChapterName = (dt.Rows[0]["ChapterName"] != DBNull.Value ? dt.Rows[0]["ChapterName"].ToString() : null);
                objServices.CategoryName = (dt.Rows[0]["CategoryName"] != DBNull.Value ? dt.Rows[0]["CategoryName"].ToString() : null);
                objServices.TotalAmount = (dt.Rows[0]["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["TotalAmount"].ToString()) : 0);
                objServices.ImageUrl = (dt.Rows[0]["ImageUrl"] != DBNull.Value ? dt.Rows[0]["ImageUrl"].ToString() : null);




            }

            return objServices;
        }

        public List<Entities.Services> GetServicesList(ref int Total)
        {
            DataTable dt = _Services.GetServicesList(ref Total);
            List<Entities.Services> lstServices = new List<Entities.Services>();

            if (dt.Rows.Count == 0)
            {
                dt = _Services.GetServicesList(ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Services objServices = new Entities.Services();

                    objServices.ServiceId = Convert.ToInt64(dr["ServiceId"]);
                    objServices.ServiceTitle = dr["ServiceTitle"].ToString();
                    objServices.IsDisplay = Convert.ToBoolean(dr["IsDisplay"]);
                    objServices.ServiceTitle = dr["ServiceTitle"].ToString();
                    lstServices.Add(objServices);
                }
            }
            return lstServices;
        }


        public Entities.Services FEGetServicesById(Int64 ServicesId, ref List<Entities.ServiceImages> lstServiceImages, ref List<Entities.ServiceDonations> lstServiceDonations, ref int status)
        {
            DataSet ds = _Services.FEGetServicesById(ServicesId, ref status);
            Entities.Services objServices = new Entities.Services();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objServices.ServiceId = Convert.ToInt64(ds.Tables[0].Rows[0]["ServiceId"]);
                objServices.CategoryId = (ds.Tables[0].Rows[0]["CategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["CategoryId"]) : 0);
                objServices.ChapterId = (ds.Tables[0].Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["ChapterId"]) : 0);
                objServices.ServiceTitle = (ds.Tables[0].Rows[0]["ServiceTitle"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceTitle"].ToString() : null);
                objServices.ServiceType = (ds.Tables[0].Rows[0]["ServiceType"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceType"].ToString() : null);
                objServices.OrganisedBy = (ds.Tables[0].Rows[0]["OrganisedBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["OrganisedBy"].ToString() : null);
                objServices.Purpose = (ds.Tables[0].Rows[0]["Purpose"] != DBNull.Value ? ds.Tables[0].Rows[0]["Purpose"].ToString() : null);
                objServices.ShortDescription = (ds.Tables[0].Rows[0]["ShortDescription"] != DBNull.Value ? ds.Tables[0].Rows[0]["ShortDescription"].ToString() : null);
                objServices.Description = (ds.Tables[0].Rows[0]["Description"] != DBNull.Value ? ds.Tables[0].Rows[0]["Description"].ToString() : null);
                objServices.DocumentUrl = (ds.Tables[0].Rows[0]["DocumentUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["DocumentUrl"].ToString() : null);
                objServices.ExpiryDate = (ds.Tables[0].Rows[0]["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["ExpiryDate"].ToString()) : DateTime.MinValue);
                objServices.EstimationAmount = (ds.Tables[0].Rows[0]["EstimationAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["EstimationAmount"].ToString()) : 0);
                objServices.ServiceStatus = (ds.Tables[0].Rows[0]["ServiceStatus"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceStatus"].ToString() : null);
                objServices.IsDisplay = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsDisplay"].ToString());
                objServices.Field1 = (ds.Tables[0].Rows[0]["Field1"] != DBNull.Value ? ds.Tables[0].Rows[0]["Field1"].ToString() : null);
                objServices.Field2 = (ds.Tables[0].Rows[0]["Field2"] != DBNull.Value ? ds.Tables[0].Rows[0]["Field2"].ToString() : null);
                objServices.UpdatedBy = ds.Tables[0].Rows[0]["UpdatedBy"].ToString();
                objServices.UpdatedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedDate"]);
                objServices.ChapterName = (ds.Tables[0].Rows[0]["ChapterName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ChapterName"].ToString() : null);
                objServices.CategoryName = (ds.Tables[0].Rows[0]["CategoryName"] != DBNull.Value ? ds.Tables[0].Rows[0]["CategoryName"].ToString() : null);
                objServices.TotalAmount = (ds.Tables[0].Rows[0]["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["TotalAmount"].ToString()) : 0);
                objServices.ImageUrl = (ds.Tables[0].Rows[0]["ImageUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["ImageUrl"].ToString() : null);
                  
            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceImages objServiceImages = new AWS_AssociationToolCore.Entities.ServiceImages();
                    objServiceImages.ServiceImageId = Convert.ToInt64(dr["ServiceImageId"]);
                    objServiceImages.ServiceId = Convert.ToInt64(dr["ServiceId"].ToString());
                    objServiceImages.ImageUrl = dr["ImageUrl"].ToString();
                    objServiceImages.IsDefault = Convert.ToBoolean(dr["IsDefault"].ToString());
                    objServiceImages.ImageDescription = (dr["ImageDescription"] != DBNull.Value ? dr["ImageDescription"].ToString() : null);
                    objServiceImages.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServiceImages.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);

                    lstServiceImages.Add(objServiceImages); 
                }
            }

            if (ds.Tables[2].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceDonations objServiceDonations = new AWS_AssociationToolCore.Entities.ServiceDonations();

                    objServiceDonations.ServiceDonationId = Convert.ToInt64(dr["ServiceDonationId"].ToString());
                    objServiceDonations.MemberId = (dr["MemberId"] != DBNull.Value ? Convert.ToInt64(dr["MemberId"]) : 0);
                    objServiceDonations.ServiceId = (dr["ServiceId"] != DBNull.Value ? Convert.ToInt64(dr["ServiceId"]) : 0);
                    objServiceDonations.DonationCause = dr["DonationCause"] != DBNull.Value ? dr["DonationCause"].ToString() : null;
                    objServiceDonations.IsActive = (dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false);
                    objServiceDonations.Amount = (dr["Amount"] != DBNull.Value ? Convert.ToDecimal(dr["Amount"]) : 0);
                    objServiceDonations.TransactionId = dr["TransactionId"] != DBNull.Value ? dr["TransactionId"].ToString() : null;
                    objServiceDonations.PaymentMethodId = (dr["PaymentMethodId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentMethodId"]) : 0); 
                    objServiceDonations.PaymentStatusId = (dr["PaymentStatusId"] != DBNull.Value ? Convert.ToInt64(dr["PaymentStatusId"]) : 0); 
                    objServiceDonations.OrderDate = (dr["OrderDate"] != DBNull.Value ? Convert.ToDateTime(dr["OrderDate"]) : DateTime.MinValue);
                    objServiceDonations.PaymentBy = (dr["PaymentBy"] != DBNull.Value ? dr["PaymentBy"].ToString() : null);
                    objServiceDonations.FirstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : null;
                    objServiceDonations.LastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : null;
                    objServiceDonations.Field3 = dr["Field3"] != DBNull.Value ? dr["Field3"].ToString() : null;

                    lstServiceDonations.Add(objServiceDonations);
                }
            }

            return objServices;
        }

        public Entities.Services FEGetServicesDataById(Int64 ServicesId, ref List<Entities.ServiceImages> lstServiceImages, ref int status)
        {
            DataSet ds = _Services.FEGetServicesDataById(ServicesId, ref status);
            Entities.Services objServices = new Entities.Services();

            if (ds.Tables[0].Rows.Count == 1)
            {
                objServices.ServiceId = Convert.ToInt64(ds.Tables[0].Rows[0]["ServiceId"]);
                objServices.CategoryId = (ds.Tables[0].Rows[0]["CategoryId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["CategoryId"]) : 0);
                objServices.ChapterId = (ds.Tables[0].Rows[0]["ChapterId"] != DBNull.Value ? Convert.ToInt64(ds.Tables[0].Rows[0]["ChapterId"]) : 0);
                objServices.ServiceTitle = (ds.Tables[0].Rows[0]["ServiceTitle"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceTitle"].ToString() : null);
                objServices.ServiceType = (ds.Tables[0].Rows[0]["ServiceType"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceType"].ToString() : null);
                objServices.OrganisedBy = (ds.Tables[0].Rows[0]["OrganisedBy"] != DBNull.Value ? ds.Tables[0].Rows[0]["OrganisedBy"].ToString() : null);
                objServices.Purpose = (ds.Tables[0].Rows[0]["Purpose"] != DBNull.Value ? ds.Tables[0].Rows[0]["Purpose"].ToString() : null);
                objServices.ShortDescription = (ds.Tables[0].Rows[0]["ShortDescription"] != DBNull.Value ? ds.Tables[0].Rows[0]["ShortDescription"].ToString() : null);
                objServices.Description = (ds.Tables[0].Rows[0]["Description"] != DBNull.Value ? ds.Tables[0].Rows[0]["Description"].ToString() : null);
                objServices.DocumentUrl = (ds.Tables[0].Rows[0]["DocumentUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["DocumentUrl"].ToString() : null);
                objServices.ExpiryDate = (ds.Tables[0].Rows[0]["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(ds.Tables[0].Rows[0]["ExpiryDate"].ToString()) : DateTime.MinValue);
                objServices.EstimationAmount = (ds.Tables[0].Rows[0]["EstimationAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["EstimationAmount"].ToString()) : 0);
                objServices.ServiceStatus = (ds.Tables[0].Rows[0]["ServiceStatus"] != DBNull.Value ? ds.Tables[0].Rows[0]["ServiceStatus"].ToString() : null);
                objServices.IsDisplay = Convert.ToBoolean(ds.Tables[0].Rows[0]["IsDisplay"].ToString());
                objServices.Field1 = (ds.Tables[0].Rows[0]["Field1"] != DBNull.Value ? ds.Tables[0].Rows[0]["Field1"].ToString() : null);
                objServices.Field2 = (ds.Tables[0].Rows[0]["Field2"] != DBNull.Value ? ds.Tables[0].Rows[0]["Field2"].ToString() : null);
                objServices.UpdatedBy = ds.Tables[0].Rows[0]["UpdatedBy"].ToString();
                objServices.UpdatedDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["UpdatedDate"]);
                objServices.ChapterName = (ds.Tables[0].Rows[0]["ChapterName"] != DBNull.Value ? ds.Tables[0].Rows[0]["ChapterName"].ToString() : null);
                objServices.CategoryName = (ds.Tables[0].Rows[0]["CategoryName"] != DBNull.Value ? ds.Tables[0].Rows[0]["CategoryName"].ToString() : null);
                objServices.TotalAmount = (ds.Tables[0].Rows[0]["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(ds.Tables[0].Rows[0]["TotalAmount"].ToString()) : 0);
                objServices.ImageUrl = (ds.Tables[0].Rows[0]["ImageUrl"] != DBNull.Value ? ds.Tables[0].Rows[0]["ImageUrl"].ToString() : null);

            }

            if (ds.Tables[1].Rows.Count != 0)
            {
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    AWS_AssociationToolCore.Entities.ServiceImages objServiceImages = new AWS_AssociationToolCore.Entities.ServiceImages();
                    objServiceImages.ServiceImageId = Convert.ToInt64(dr["ServiceImageId"]);
                    objServiceImages.ServiceId = Convert.ToInt64(dr["ServiceId"].ToString());
                    objServiceImages.ImageUrl = dr["ImageUrl"].ToString();
                    objServiceImages.IsDefault = Convert.ToBoolean(dr["IsDefault"].ToString());
                    objServiceImages.ImageDescription = (dr["ImageDescription"] != DBNull.Value ? dr["ImageDescription"].ToString() : null);
                    objServiceImages.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServiceImages.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);

                    lstServiceImages.Add(objServiceImages);
                }
            }
            
            return objServices;
        }

        #endregion



        #region FrontEnd

        public List<Entities.Services> FEServicesGetListByChapterId(Int64 ChapterId, ref int status)
        {
            DataTable dt = _Services.FEServicesGetListByChapterId(ChapterId, ref status);

            List<Entities.Services> lstServices = new List<Entities.Services>();

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Services objServices = new Entities.Services();

                    objServices.ServiceId = Convert.ToInt64(dr["ServiceId"]);
                    objServices.ServiceTitle = dr["ServiceTitle"].ToString();
                    objServices.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt32(dr["ChapterId"]) : 0);

                    lstServices.Add(objServices);
                }
            }

            return lstServices;
        }

        public List<Entities.Services> FEGetServicesListByVariable(Int64 MemberId, Int64 ChapterId, Int64 CategoryId, string ServiceType, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _Services.FEGetServicesListByVariable(MemberId, ChapterId, CategoryId, ServiceType, Search, Sort, PageNo, Items, ref Total);
            List<Entities.Services> lstServices = new List<Entities.Services>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _Services.FEGetServicesListByVariable(MemberId, ChapterId, CategoryId, ServiceType, Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Services objServices = new Entities.Services();

                    objServices.RId = Convert.ToInt64(dr["Rid"]);
                    objServices.ServiceId = Convert.ToInt64(dr["ServiceId"]);
                    objServices.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"].ToString()) : 0);
                    objServices.ServiceTitle = (dr["ServiceTitle"] != DBNull.Value ? dr["ServiceTitle"].ToString() : null);
                    objServices.ServiceType = (dr["ServiceType"] != DBNull.Value ? dr["ServiceType"].ToString() : null);
                    objServices.OrganisedBy = (dr["OrganisedBy"] != DBNull.Value ? dr["OrganisedBy"].ToString() : null);
                    objServices.Purpose = (dr["Purpose"] != DBNull.Value ? dr["Purpose"].ToString() : null);
                    objServices.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
                    objServices.ShortDescription = (dr["ShortDescription"] != DBNull.Value ? dr["ShortDescription"].ToString() : null);
                    objServices.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objServices.ExpiryDate = (dr["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ExpiryDate"].ToString()) : DateTime.MinValue);
                    objServices.EstimationAmount = (dr["EstimationAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EstimationAmount"].ToString()) : 0);
                    objServices.ServiceStatus = (dr["ServiceStatus"] != DBNull.Value ? dr["ServiceStatus"].ToString() : null);
                    objServices.IsDisplay = (dr["IsDisplay"] != DBNull.Value ? Convert.ToBoolean(dr["IsDisplay"].ToString()) : false);
                    objServices.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    objServices.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                    objServices.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServices.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    objServices.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);
                    objServices.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
                    objServices.ServiceTypeName = (dr["ServiceTypeName"] != DBNull.Value ? dr["ServiceTypeName"].ToString() : null);
                    objServices.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : null);
                    objServices.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"].ToString()) : 0);

                    lstServices.Add(objServices);
                }
            }
            return lstServices;
        }

        public List<Entities.Services> FEGetServicesListByVariableByYear(string Year, Int64 MemberId, Int64 ChapterId, Int64 CategoryId, string ServiceType, string Search, string Sort, int PageNo, int Items, ref int Total)
        {
            DataTable dt = _Services.FEGetServicesListByVariableByYear(Year, MemberId, ChapterId, CategoryId, ServiceType, Search, Sort, PageNo, Items, ref Total);
            List<Entities.Services> lstServices = new List<Entities.Services>();

            if (dt.Rows.Count == 0 && PageNo > 1)
            {
                dt = _Services.FEGetServicesListByVariableByYear(Year, MemberId, ChapterId, CategoryId, ServiceType, Search, Sort, PageNo, Items, ref Total);
            }

            if (dt.Rows.Count != 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Entities.Services objServices = new Entities.Services();

                    objServices.RId = Convert.ToInt64(dr["Rid"]);
                    objServices.ServiceId = Convert.ToInt64(dr["ServiceId"]);
                    objServices.ChapterId = (dr["ChapterId"] != DBNull.Value ? Convert.ToInt64(dr["ChapterId"].ToString()) : 0);
                    objServices.ServiceTitle = (dr["ServiceTitle"] != DBNull.Value ? dr["ServiceTitle"].ToString() : null);
                    objServices.ServiceType = (dr["ServiceType"] != DBNull.Value ? dr["ServiceType"].ToString() : null);
                    objServices.OrganisedBy = (dr["OrganisedBy"] != DBNull.Value ? dr["OrganisedBy"].ToString() : null);
                    objServices.Purpose = (dr["Purpose"] != DBNull.Value ? dr["Purpose"].ToString() : null);
                    objServices.Description = (dr["Description"] != DBNull.Value ? dr["Description"].ToString() : null);
                    objServices.ShortDescription = (dr["ShortDescription"] != DBNull.Value ? dr["ShortDescription"].ToString() : null);
                    objServices.DocumentUrl = (dr["DocumentUrl"] != DBNull.Value ? dr["DocumentUrl"].ToString() : null);
                    objServices.ExpiryDate = (dr["ExpiryDate"] != DBNull.Value ? Convert.ToDateTime(dr["ExpiryDate"].ToString()) : DateTime.MinValue);
                    objServices.EstimationAmount = (dr["EstimationAmount"] != DBNull.Value ? Convert.ToDecimal(dr["EstimationAmount"].ToString()) : 0);
                    objServices.ServiceStatus = (dr["ServiceStatus"] != DBNull.Value ? dr["ServiceStatus"].ToString() : null);
                    objServices.IsDisplay = (dr["IsDisplay"] != DBNull.Value ? Convert.ToBoolean(dr["IsDisplay"].ToString()) : false);
                    objServices.Field1 = (dr["Field1"] != DBNull.Value ? dr["Field1"].ToString() : null);
                    objServices.Field2 = (dr["Field2"] != DBNull.Value ? dr["Field2"].ToString() : null);
                    objServices.UpdatedBy = dr["UpdatedBy"].ToString();
                    objServices.UpdatedDate = Convert.ToDateTime(dr["UpdatedDate"]);
                    objServices.ChapterName = (dr["ChapterName"] != DBNull.Value ? dr["ChapterName"].ToString() : null);
                    objServices.CategoryName = (dr["CategoryName"] != DBNull.Value ? dr["CategoryName"].ToString() : null);
                    objServices.ServiceTypeName = (dr["ServiceTypeName"] != DBNull.Value ? dr["ServiceTypeName"].ToString() : null);
                    objServices.ImageUrl = (dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : null);
                    objServices.TotalAmount = (dr["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(dr["TotalAmount"].ToString()) : 0);

                    lstServices.Add(objServices);
                }
            }
            return lstServices;
        }















        #endregion
    }
}
