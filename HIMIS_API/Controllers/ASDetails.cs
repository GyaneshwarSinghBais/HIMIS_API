using HIMIS_API.Data;
using HIMIS_API.Models.AS;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.TS;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ASDetails : Controller
    {
        private readonly DbContextData _context;
        public ASDetails(DbContextData context)
        {
            _context = context;
        }
        [HttpGet("ASPending")]
        public async Task<ActionResult<IEnumerable<ASPendingDTO>>> ASPendingSummary()
        {
                 
            string query = "";      
                //all AS Pending
            query = $@" select a.asid, l.login_name,msc.name as Head,a.Letterno,convert(varchar,ADDate,105) as ASDate,
cast(a.noofworks as varchar) as TotalWorks ,isnull(nosworks,0) as enteredWorks
,a.noofworks-isnull(nosworks,0) BaltobeEnter,
cast(a.TotalASAmt as decimal(16,2)) as TotalASAmt,cast(isnull(AaAmt,0) as decimal(16,2)) EnteredTotalAS,cast(isnull(a.TotalASAmt,0)-isnull(AaAmt,0)as decimal(16,2))  as BalanceASAmount from WorkMasteras  a
            inner join MainSchemes msc on msc.MainSchemeID = a.Headid
			inner join Login l on l.Login_id=a.ApprovedBy
            left outer join 
            (
            select count(work_id) as nosworks,ASID,sum(w.AaAmt) as AaAmt from WorkMaster w
            where w.IsDeleted is null and  w.MainSchemeID not in (121) 
            and w.ASID is not null
            group by w.ASID
            ) w on w.ASID=a.ASID
            where  1=1 and 
            (a.noofworks)-isnull(nosworks,0) >0 
			order by ADDate desc ";
            
        
            return await _context.ASPendingDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }
        [HttpGet("ASCompleted")]
        public async Task<ActionResult<IEnumerable<ASEnteredSummyDTO>>> ASCompleted()
        {

            string query = "";
            //all entered AS
            query = $@" select a.asid, l.login_name,msc.name as Head,a.Letterno,convert(varchar,ADDate,105) as ASDate,
cast(a.noofworks as varchar) as TotalWorks ,isnull(nosworks,0) as enteredWorks 
,a.noofworks-isnull(nosworks,0) BaltobeEnter,
cast(a.TotalASAmt as decimal(16,2)) as TotalASAmt,
cast(isnull(AaAmt,0) as decimal(16,2)) EnteredTotalAS,cast(isnull(a.TotalASAmt,0)-isnull(AaAmt,0)as decimal(16,2))  as BalanceASAmount
from WorkMasteras  a
	inner join Login l on l.Login_id=a.ApprovedBy
            inner join MainSchemes msc on msc.MainSchemeID = a.Headid
            left outer join 
            (
            select count(work_id) as nosworks,ASID,sum(w.AaAmt) as AaAmt from WorkMaster w
            where w.IsDeleted is null and  w.MainSchemeID not in (121) 
            and w.ASID is not null
            group by w.ASID
            ) w on w.ASID=a.ASID
            where  1=1 
			order by ADDate desc ";


            return await _context.ASEnteredSummyDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }




        [HttpGet("DivisionWiseASPending")]
        public async Task<ActionResult<IEnumerable<ASDivsionPendingDTO>>> DivisionWiseASPending(string divisionId, string mainSchemeId)
        {
            string? whereClause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
         
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }

            string query = "";
            //all AS Pending divisionwise
            query = $@" select dv.divname_en as Division, l.login_name,msc.name as Head,a.Letterno,convert(varchar,ADDate,105) as ASDate,
ad.totalworks,isnull(nosworks,0) as enteredWorks 
,ad.totalworks-isnull(nosworks,0) as BalanceWork

, a.asid,agd.DivisionID,cast(a.asid as varchar)+agd.DivisionID as ID
from WorkMasteras  a
	inner join Login l on l.Login_id=a.ApprovedBy
inner join WorkMasterAS_Division ad on ad.asid=a.asid
inner join agencydivisionmaster  agd on agd.DivisionID=ad.divisionid and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join MainSchemes msc on msc.MainSchemeID = a.Headid
left outer join 
(
select count(work_id) as nosworks,ASID,w.AllotedDivisionID from WorkMaster w
where w.IsDeleted is null and  w.MainSchemeID not in (121) 
and w.ASID is not null 
group by w.ASID,w.AllotedDivisionID
) w on w.ASID=a.ASID and w.AllotedDivisionID=ad.divisionid
where  1=1  and 
(ad.totalworks)-isnull(nosworks,0) >0 " + whereClause + @"
order by ADDate  ";


            return await _context.ASDivsionPendingDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

    

        [HttpGet("ASEnteredDetails")]

        public async Task<ActionResult<IEnumerable<ASEnteredDetailsDTO>>> ASEnteredDetails(string ASID, string divisionId, string mainSchemeId)
        {

            string? whereClause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }

            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            string query = "";

            query = $@" select dv.divname_en as Division, l.login_name,msc.name as Head,wa.Letterno,convert(varchar,ADDate,105) as ASDate,cast(AaAmt as decimal(18,2)) as ASAmt
,work_id,dis.DBStart_Name_En as District,b.Block_Name_En,
  d.NAME_ENG+' - '+s.SWName as workname ,w.ASID from WorkMaster w
  inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join WorkMasteras wa on wa.ASID=w.ASID
inner join Login l on l.Login_id=wa.approvedby
inner join SWDetails s on s.SWId=w.work_description_id 
inner join MainSchemes msc on msc.MainSchemeID = wa.Headid
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
where w.IsDeleted is null and  w.MainSchemeID not in (121) 
and w.ASID is not null and w.ASID="+ ASID+@"   " + whereClause + @" 
order by dv.divname_en,dis.DBStart_Name_En ,b.Block_Name_En,w.created_on ";

            return await _context.ASEnteredDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getASFile")]

        public async Task<ActionResult<IEnumerable<ASFileNameDTO>>> getASFile(string ASID, string workid)
        {
            string? whereClause = "";
            string query = "";
            string p = "https://cgmsc.gov.in/himisr/UploadAS/";
            if (ASID != "0")
            {
                //select cast(ASID as varchar) as ID, ASPath,'https://cgmsc.gov.in/himisr/UploadAS/' + ASLetterName as ASLetterName,ASLetterName as Filename from WorkMasterAS where ASID = 1


                query = @"select cast(ASID as varchar) as ID, ASPath,'https://cgmsc.gov.in/himisr/UploadAS/' + ASLetterName as ASLetterName,ASLetterName as Filename from WorkMasterAS where ASID = " + ASID;
            }
            else
            {
               
                query = @" select work_id as ID, ASPath,case when w.ASID is null then ('https://cgmsc.gov.in/himisr/Upload/' + ASLetter) else ('https://cgmsc.gov.in/himisr/UploadAS/' + ASLetter) end as ASLetterName,
 ASLetter  as Filename  
 from WorkMaster w where work_id ='"+ workid + "'";
            }
            return await _context.ASFileNameDbSet
         .FromSqlRaw(query)
         .ToListAsync();

            

        }


    }
}

