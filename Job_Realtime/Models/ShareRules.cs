using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job_Realtime
{
    class ShareRules
    {


        #region Config 

        public static String ConnectionString = ConfigurationSettings.AppSettings["ConnectionString"].ToString();
        public static String Rabbitmq_User = ConfigurationSettings.AppSettings["Rabbitmq_User"].ToString();
        public static String Rabbitmq_Pass = ConfigurationSettings.AppSettings["Rabbitmq_Pass"].ToString();
        public static String Amqps_Realtime = ConfigurationSettings.AppSettings["Amqps_Realtime"].ToString();
        public static String Queue_Sales = ConfigurationSettings.AppSettings["Queue_Sales"].ToString();
        public static String Queue_Incremental = ConfigurationSettings.AppSettings["Queue_Incremental"].ToString();

        #endregion

        public static DBRealTimeDataContext ConnectDB(Boolean Save = false)
        {
            DBRealTimeDataContext db = null;
            try
            {
                if (Save) db = new DBRealTimeDataContext(ShareRules.ConnectionString);
                else db = new DBRealTimeDataContext(ShareRules.ConnectionString) { ObjectTrackingEnabled = false };
            }
            catch (Exception ex)
            {

            }

            return db;
        }

        public static Boolean CheckEmpty(object value)
        {


            if (value == null || value == "") return true;
            else return false;
        }

        public static String ConvertDateTimeForDisplay(object Input)
        {
            try
            {
                if (Input != null && Input.ToString() != string.Empty)
                {
                    return Convert.ToDateTime(Input.ToString()).ToString("dd/MM/yyyy");
                }
            }
            catch (Exception ex)
            {
                //Logs(ex.Message, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return string.Empty;
        }

 

        public static void WriteLog(string MethodName, string ErrorMessage)
        {
            String Result = string.Empty;
            DBRealTimeDataContext db = ConnectDB(true);

            try
            {


                TRN_log obj = new TRN_log();
                obj.CreateDate = DateTime.Now;
                obj.SystemName = "Job_Realtime";
                obj.MethodName = MethodName;
                obj.MessageError = ErrorMessage;
                db.TRN_logs.InsertOnSubmit(obj);


                db.SubmitChanges();



            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }



        }

    }
}