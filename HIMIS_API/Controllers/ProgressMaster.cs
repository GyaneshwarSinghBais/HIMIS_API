using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace HIMIS_API.Controllers
{
    public class ProgressMaster : Controller
    {
        private readonly DbContextData _context;
        public IActionResult Index()
        {
            return View();
        }
        public ProgressMaster(DbContextData context)
        {
            _context = context;
        }


        [HttpGet("getMainScheme")]
        public async Task<ActionResult<IEnumerable<MainSchemeDTO>>> getMainScheme()
        {
            string qry = @"  select msc.MainSchemeID,msc.Name+'('+isnull(w.d,0)+')' as Name from MainSchemes msc
left outer join 
(
select cast(COUNT(w.work_id) as varchar) as d,MainSchemeID from WorkMaster w where 1=1 and w.IsDeleted is  null
group by MainSchemeID
) w on w.MainSchemeID=msc.MainSchemeID
where 1=1 and isnull(w.d,0)>0
order by w.d desc";

            var context = new MainSchemeDTO();

            var myList = _context.MainSchemeDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }


        [HttpGet("getDivision")]
        public async Task<ActionResult<IEnumerable<DivisionNameDTO>>> getDivision(string DivisionID)
        {
            string wh = "";
            if (DivisionID == "0")
            {

            }
            else
            {
                wh = " and a.DivisionID='" + DivisionID+"'";
            }
            string qry = @"  select Div_Id, DivName_En, a.DivisionID from Division d
inner join AgencyDivisionMaster a on a.DivisionName=d.Div_Id where 1=1 "+ wh + @"
order by DivName_En ";

            var context = new DivisionNameDTO();

            var myList = _context.DivisionNameDbSet
            .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();

            return myList;
        }
        [HttpGet("GetDistrict")]
        public async Task<ActionResult<IEnumerable<DistrictNameDTO>>> GetDistrict(string divisionid)
        {
            FacOperation ob = new FacOperation(_context);
           
            string whdivid = "";
            if (divisionid != "0")
            {
                Int32 fdivid = ob.getDivID(divisionid);
                whdivid = " and Div_Id=" + fdivid;
            }

            else
            {

            }
            string qry = @" select District_ID ,DBStart_Name_En as DistrictName,Div_Id  from Districts
where 1=1 " + whdivid + @"
order by DBStart_Name_En ";

            var myList = _context.DistrictNameDbSet
          .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

        [HttpGet("GetPProgressLevel")]
        public async Task<ActionResult<IEnumerable<ProgressLevelDTO>>> GetPProgressLevel(string PPID)
        {

            string whPPID = "";
            if (PPID != "0")
            {
                whPPID = " and wl.PPID=" + PPID;
            }

            else
            {

            }
            string qry = @"     select PPID, parentprogress  as PPStatus from WorkLevelParent wl inner join WorkLevelStatus ws on ws.wlstatusid=wl.SNHID
where Isvisible= 'Y' " + whPPID + @"
order by PPID ";

            var myList = _context.ProgressLevelDbSet
          .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }
        [HttpGet("GetHealthCentre")]
        public async Task<ActionResult<IEnumerable<HealthCentreDTO>>> GetHealthCentre(string distid,string divisionid)
        {

            string wh = "";
            if (divisionid != "0")
            {
                wh = " and a.DivisionID='" + divisionid + "'";
            }
            if (distid != "0")
            {
                wh = " and dis.District_ID='" + distid + "'";
            }


            string qry = @"    select ty.Type_ID,ty.DETAILS_ENG,count(w1.work_id) as nosworks  from dhrsHealthCentreType ty
inner join dhrsHealthCenter d on   ty.Type_ID=d.TYPE
inner join WorkMaster w1 on w1.worklocation_id=d.HC_ID
inner join Districts dis on dis.DISTRICT_ID=w1.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join AgencyDivisionMaster ag on ag.DivisionName=dv.Div_Id
where 1=1 and w1.IsDeleted is  null "+ wh + @"
group by ty.DETAILS_ENG,ty.Type_ID
order by ty.DETAILS_ENG ";

            var myList = _context.HealthCentreDbSet
          .FromSqlInterpolated(FormattableStringFactory.Create(qry)).ToList();
            return myList;
        }

    }
  
}
