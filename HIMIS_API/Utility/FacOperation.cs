using HIMIS_API.Data;
using Microsoft.EntityFrameworkCore;
using SMSRef;
using System.Runtime.CompilerServices;

namespace HIMIS_API.Utility
{
    public class FacOperation
    {
        private readonly DbContextData _context;
        public FacOperation(DbContextData context)
        {
            _context = context;
        }

        public Int32 getDivID(string divisionid)
        {
            string qry = @" select Div_Id, DivName_En, a.DivisionID from Division d
inner join AgencyDivisionMaster a on a.DivisionName=d.Div_Id where 1=1 and a.DivisionID ='"+divisionid+@"'
order by DivName_En " ;

          

            var myList = _context.DivisionNameDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            Int32 divid = 0;
            if (myList.Count > 0)
            {
                divid = Convert.ToInt32(myList[0].DIV_ID); // Assuming IssueItemID is an integer

            }
           
            return divid;
        }

        public string paraId(string paraid)
        {
            string strpara = "";
            if (paraid == "1")

            {
                //greater than 10cr
                strpara = " and w.AaAmt >1000 ";
            }
            else if (paraid == "2")

            {
                //greater than 5-10cr
                strpara = " and  w.AaAmt between  500 and 1000 ";
            }
            else if (paraid == "3")

            {
                //greater than 1-5cr
                strpara = " and  w.AaAmt between  100 and 500 ";
            }
            else if (paraid == "4")

            {
                // 40 to 1 cr lac
                strpara = " and  w.AaAmt between  40 and 100 ";
            }
            else if (paraid == "5")

            {
                ///20-40 lac
                strpara = " and  w.AaAmt between  20 and 40 ";
            }
            else if (paraid == "6")

            {
                ///10-20 lac
                strpara = " and  w.AaAmt between  10 and 20 ";
            }
            else
            {
                //below  10 lacs
                strpara = " and w.AaAmt <10 ";

            }



            return strpara;
        }

        public string sendOtpSms(string mobNo)
        {
            // string mobNo = "9691611103";
            string portal = "Feedback";
            string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            string sRandomOTP = GenerateRandomOTP(5, saAllowedCharacters);
            string now = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            //string senddata = "OTP for Login on DPDMIS is " + sRandomOTP;
            string senddata = " OTP for submission in " + portal + " is " + sRandomOTP + ". You may use this for New Login/Forgot Password.Please do not share with anyone." ;
            getLoginSMS(mobNo.ToString(), senddata);   
            return sRandomOTP;
        }


        //public string insertUpdateOTP1(string userid, string mobNo)
        //{
           
        //    // string mobNo = "9691611103";

        //    string[] saAllowedCharacters = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
        //    string sRandomOTP = GenerateRandomOTP(5, saAllowedCharacters);
        //    string now = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
        //    string senddata = "OTP for Login on DPDMIS is " + sRandomOTP;
        //    getLoginSMS(mobNo.ToString(), senddata);


        //  //  string strUpdateQuery = "Update usrUsers Set OTP = '" + sRandomOTP + "' , OTPUPDATEDT = TO_DATE('" + now + "','MM/DD/YYYY hh24:mi:ss') where userid = " + userid;
        //  //  var myList = _context.ProgressRecDbSet
        //  //.FromSqlInterpolated(FormattableStringFactory.Create(strUpdateQuery)).ToList();

        //  //  // insert OTPrecord
        //  //  string strInsertQuery = "insert into otprecord(updatedt, otp, mob,  userid,EntryDate,IsLogin)values(TO_DATE('" + now + "','MM/DD/YYYY hh24:mi:ss'), '" + sRandomOTP + "', '" + mobNo + "',  " + userid + ", TO_DATE('" + now + "','MM/DD/YYYY hh24:mi:ss'),'Y' )";
        //  //  var myListInsert = _context.ProgressRecDbSet
        //  //    .FromSqlInterpolated(FormattableStringFactory.Create(strInsertQuery)).ToList();

        //    return sRandomOTP;


        //}

        private string GenerateRandomOTP(int iOTPLength, string[] saAllowedCharacters)
        {

            string sOTP = String.Empty;

            string sTempChars = String.Empty;

            Random rand = new Random();

            for (int i = 0; i < iOTPLength; i++)
            {

                int p = rand.Next(0, saAllowedCharacters.Length);

                sTempChars = saAllowedCharacters[rand.Next(0, saAllowedCharacters.Length)];

                sOTP += sTempChars;

            }

            return sOTP;

        }

        public void getLoginSMS(String mobNumber, String OTP)
        {

            var client = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap);

            //var response = client.sendsmsHIMISAsync(mobNumber, OTP, "1407161537152057950");
            var response = client.sendsmsHIMISAsync(mobNumber, OTP, "1407163911599431374");
        }


    }
}
