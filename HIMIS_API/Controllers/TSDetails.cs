using HIMIS_API.Data;
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
    public class TSDetail : Controller
    {
        private readonly DbContextData _context;
        public TSDetail(DbContextData context)
        {
            _context = context;
        }
        [HttpGet("TSPending")]
        public async Task<ActionResult<IEnumerable<TSSummaryDTO>>> TSPending(string RPType,string divisionid,string districtid,string mainschemeid)
        {
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and dis.District_ID  = '{districtid}'";

            }

            if (mainschemeid != "0")
            {

                whereClause += $" and w.MainSchemeID  = '{mainschemeid}'";

            }

            string query = "";
            if (RPType == "Total")
            {
                //divisionwise
                query = $@" select divisionid as ID,Division as Name,count(distinct work_id) as nosWorks, isnull(cast(sum(ASAmt)/100 as decimal(18,2)),0)  ASValuecr
,sum(WorksAbove2cr) as Above2crWork,sum(Worksbelow2cr) below2crWork from 
(
select w.work_id,dv.divname_en as Division,
 msc.Name as Head,dis.DBStart_Name_En as District,
 d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,w.letterno
,isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date
  ,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,w.tsDate, 105) as tsDate
  ,w.worklocation_id,hc.DETAILS_ENG,hc.Type_ID,TSLetterRef
  ,agd.divisionid,dis.DISTRICT_ID,w.MainSchemeID
  ,case when isnull(cast(w.AaAmt as decimal(18,2)),0)>200 then 1 else 0 end as WorksAbove2cr
   ,case when isnull(cast(w.AaAmt as decimal(18,2)),0)>200 then 0 else 1 end as Worksbelow2cr
from WorkMaster w 
 inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
inner join  SWDetails s on s.SWId=w.work_description_id 
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id             
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
where 1=1 and w.MainSchemeID not in (121)
and w.IsDeleted is null and isnull(w.TSAmount,0)=0  " + whereClause + @"
						) a 
						group by divisionid,Division  order by count(distinct work_id) ";
            }
            if (RPType == "Scheme")
            {
                //Scheme
                query = $@" select cast(MainSchemeID as varchar) as ID  ,Head as Name,count(distinct work_id) as nosWorks, isnull(cast(sum(ASAmt)/100 as decimal(18,2)),0)  ASValuecr
,sum(WorksAbove2cr) as Above2crWork,sum(Worksbelow2cr) below2crWork from 
(
select w.work_id,dv.divname_en as Division,
 msc.Name as Head,dis.DBStart_Name_En as District,
 d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,w.letterno
,isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date
  ,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,w.tsDate, 105) as tsDate
  ,w.worklocation_id,hc.DETAILS_ENG,hc.Type_ID,TSLetterRef
  ,agd.divisionid,dis.DISTRICT_ID,w.MainSchemeID
  ,case when isnull(cast(w.AaAmt as decimal(18,2)),0)>200 then 1 else 0 end as WorksAbove2cr
   ,case when isnull(cast(w.AaAmt as decimal(18,2)),0)>200 then 0 else 1 end as Worksbelow2cr
from WorkMaster w 
 inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
inner join  SWDetails s on s.SWId=w.work_description_id 
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id             
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
where 1=1 and w.MainSchemeID not in (121)
and w.IsDeleted is null and isnull(w.TSAmount,0)=0  " + whereClause + @"
						) a 
						group by MainSchemeID,Head
						order by count(distinct work_id) desc  ";
            }
            

            if (RPType == "District")
            {
               //districtwise
                query = $@"  select cast(District_ID as varchar) as ID,District as Name,count(distinct work_id) as nosWorks, isnull(cast(sum(ASAmt)/100 as decimal(18,2)),0)  ASValuecr
,sum(WorksAbove2cr) as Above2crWork,sum(Worksbelow2cr) below2crWork,Division from 
(
select w.work_id,dv.divname_en as Division,
 msc.Name as Head,dis.DBStart_Name_En as District,
 d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,w.letterno
,isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date
  ,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,w.tsDate, 105) as tsDate
  ,w.worklocation_id,hc.DETAILS_ENG,hc.Type_ID,TSLetterRef
  ,agd.divisionid,dis.DISTRICT_ID,w.MainSchemeID
  ,case when isnull(cast(w.AaAmt as decimal(18,2)),0)>200 then 1 else 0 end as WorksAbove2cr
   ,case when isnull(cast(w.AaAmt as decimal(18,2)),0)>200 then 0 else 1 end as Worksbelow2cr
from WorkMaster w 
 inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
inner join  SWDetails s on s.SWId=w.work_description_id 
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id             
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
where 1=1 and w.MainSchemeID not in (121)
and w.IsDeleted is null and isnull(w.TSAmount,0)=0  " + whereClause + @"
						) a 
						group by DISTRICT_ID,District,Division
						order by Division,count(distinct work_id) desc ";
            }

            return await _context.TSSummaryDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("TSDetails")]

        public async Task<ActionResult<IEnumerable<TSDetailsDTO>>> TSDetails(string divisionId, string mainSchemeId, string distid)
        {
            string? whereClause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and dis.District_ID= '{distid}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
          
            string query = "";

            query = $@" select dv.divname_en as Division, msc.Name as Head,dis.DBStart_Name_En as District,w.work_id,w.letterno,hc.DETAILS_ENG,
 d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,b.Block_Name_En
,isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date
,datediff(dd,w.AADate,getdate()) as DayssinceAS
  ,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,w.tsDate, 105) as tsDate
  ,w.worklocation_id,hc.Type_ID,TSLetterRef
  ,agd.divisionid
from WorkMaster w 
                        inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
                           inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
						inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
                         inner join SWDetails s on s.SWId=w.work_description_id 
                        inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
                     left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
						where 1=1 and w.MainSchemeID not in (121) and w.IsDeleted is null and isnull(w.TSAmount,0)=0  " + whereClause + @"

                        order by w.AADate ";
            
            return await _context.TSDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }




        }
}

