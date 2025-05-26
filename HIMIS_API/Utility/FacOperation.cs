using HIMIS_API.Data;
using System.Runtime.CompilerServices;


using Microsoft.EntityFrameworkCore;

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




    }
}
