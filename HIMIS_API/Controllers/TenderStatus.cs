using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.Tender;
using HIMIS_API.Models.TS;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static MongoDB.Driver.WriteConcern;
using System.Security.Cryptography;
using System.Net;
using System;
using HIMIS_API.Models.EMS;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenderStatus : Controller
    {
        private readonly DbContextData _context;
        public TenderStatus(DbContextData context)
        {
            _context = context;
        }
        [HttpGet("LiveTender")]
        public async Task<ActionResult<IEnumerable<TenderAbstractDTO>>> LiveTender(string RPType, string divisionid, string districtid, string mainschemeid, string TimeStatus)
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

            if (TimeStatus == "Live")
            {
                whereClause += $" and t.enddt>=getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null  ";
            }
            if (TimeStatus == "Timeover")
            {
                whereClause += $" and t.enddt<getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null  ";
            }

            string query = "";
            if (RPType == "Total")
            {
                //Total
                query = $@"  select cast(1 as varchar) ID, 'Total' as Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,tw.TOpnedDT,103) TOpnedDT,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas

from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (3)
) pl on pl.Work_id=tw.work_id

where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @"
) a ";
            }
            if (RPType == "Division")
            {
                //Divsion
                query = $@" select  ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
 from 
 (
 select 
 tw.work_id,
 t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
 convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,tw.TOpnedDT,103) TOpnedDT,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,agd.DivisionID as ID,dv.DivName_En as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (3)
) pl on pl.Work_id=tw.work_id

where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @"
) a group by ID, Name
order by count(work_id) desc ";
            }

            if (RPType == "Scheme")
            {
                //schemwise
                query = $@" select  ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
 from 
 (
 select 
 tw.work_id,
 t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
 convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,tw.TOpnedDT,103) TOpnedDT,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,cast(msc.MainSchemeID as varchar) as ID,msc.name as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
  inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (3)
) pl on pl.Work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
where  1=1 and w.IsDeleted is null and t.IsZonal is null  " + whereClause + @"
) a group by ID, Name
order by count(work_id) desc  ";
            }
            if (RPType == "District")
            {
                //districtwise
                query = $@" select  ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
 from 
 (
 select 
 tw.work_id,
 t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
 convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,tw.TOpnedDT,103) TOpnedDT,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,dis.DISTRICT_ID as ID,dis.DBStart_Name_En as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
 inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')

inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (3)
) pl on pl.Work_id=tw.work_id

where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @"
) a group by ID, Name
order by count(work_id) desc  ";
            }

            return await _context.TenderAbstractDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getTenderDetails")]

        public async Task<ActionResult<IEnumerable<LiveTenderDetails>>> getTenderDetails(string divisionId, string mainSchemeId, string distid, string TimeStatus)
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
            if (TimeStatus == "Live")
            {
                whereClause += $" and t.enddt>=getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null  ";
            }
            if (TimeStatus == "Timeover")
            {
                whereClause += $" and t.enddt<getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null  ";
            }

            string query = "";

            query = $@" select dv.divname_en as Division, msc.Name as Head,dis.DBStart_Name_En as District,w.work_id,w.letterno,hc.DETAILS_ENG,
 d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,b.Block_Name_En
,isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date
,datediff(dd,w.AADate,getdate()) as DayssinceAS
  ,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,w.tsDate, 105) as tsDate
  ,w.worklocation_id,hc.Type_ID,TSLetterRef
  ,agd.divisionid, convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt,isnull(tw.Noofcalls,0) as Noofcalls
,datediff(dd,t.enddt,getdate()) as DaystoEnd,cast(t.tenderno as varchar) as tenderno ,cast(t.eprocno as varchar) eprocno
from WorkMaster w 
inner join MasTenderWorks tw on tw.work_id=w.work_id
       inner join MasTender t on t.TenderID=tw.tenderid
                        inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
                           inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
						inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
                         inner join SWDetails s on s.SWId=w.work_description_id 
                        inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
                     left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
						where 1=1 and w.MainSchemeID not in (121) and w.IsDeleted is null " + whereClause + @" order by t.enddt  ";

            return await _context.LiveTenderDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getTenderEvaluationDetails")]

        public async Task<ActionResult<IEnumerable<EvaluatinoDetailsDTO>>> getTenderEvaluationDetails(string divisionId, string mainSchemeId, string distid)
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

            whereClause += $" and t.rejid is null and t.topnedpricedt is null and (t.topneddt is not null or t.topnedbdt is not null) ";
            string query = "";

            query = $@" select dv.divname_en as Division, msc.Name as Head,dis.DBStart_Name_En as District,w.work_id,w.letterno,hc.DETAILS_ENG,
 d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,b.Block_Name_En
,isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date
,datediff(dd,w.AADate,getdate()) as DayssinceAS
  ,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,w.tsDate, 105) as tsDate
  ,w.worklocation_id,hc.Type_ID,TSLetterRef
  ,agd.divisionid, convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt,isnull(tw.Noofcalls,0) as Noofcalls
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen,cast(t.tenderno as varchar) as tenderno ,cast(t.eprocno as varchar) eprocno
from WorkMaster w 
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (24,21)
) pl on pl.Work_id=w.work_id

inner join MasTenderWorks tw on tw.work_id=w.work_id
       inner join MasTender t on t.TenderID=tw.tenderid
                        inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
                           inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
						inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
                         inner join SWDetails s on s.SWId=w.work_description_id 
                        inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
                     left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
						where 1=1 and w.MainSchemeID not in (121) and w.IsDeleted is null " + whereClause + @" order by t.topneddt desc  ";

            return await _context.EvaluatinoDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();


        }



        [HttpGet("getPriceEvaluationDetails")]

        public async Task<ActionResult<IEnumerable<PriceOpnedDTO>>> getPriceEvaluationDetails(string divisionId, string mainSchemeId, string distid)
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

            whereClause += $" and t.rejid is null and t.topnedpricedt is not null ";
            string query = "";

            query = $@"  select 
                        tw.work_id, w.letterno,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,dv.divname_en as Division, msc.Name as Head,dis.DBStart_Name_En as District,
                        cast(t.tenderno as varchar) as tenderno ,cast(t.eprocno as varchar) eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,convert(varchar,w.AADate, 105) as AA_RAA_Date,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
, case when t.topnedbdt is not null then convert(varchar,t.topnedbdt,105) else convert(varchar,t.topneddt,105) end as topneddt   ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then cast(TSAmount as decimal(18,2)) else cast(AaAmt as decimal(18,2)) end as Valueworksinlas
,datediff(dd,t.topnedpricedt,getdate()) daysSinceOpen,convert(varchar,t.topnedpricedt,105) topnedpricedt 
,tw.CID,tw.SanctionDetail,cast(tw.SanctionRate as decimal(18,2)) as SanctionRate,isnull(c.Title,'')+' '+c.EnglishName as CName,isnull(c.MobNo,'-') as MobNo 
from MasTenderWorks tw
inner join ContractMaster c on rtrim(ltrim(c.Contractorid))=rtrim(ltrim(tw.CID))
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id

inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (22)
) pl on pl.Work_id=tw.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
inner join SWDetails s on s.SWId=w.work_description_id 
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
where  1=1 and w.IsDeleted is null and t.IsZonal is null 
" + whereClause + @" order by  topnedpricedt desc ";

            return await _context.PriceOpnedDbSet
            .FromSqlRaw(query)
            .ToListAsync();


        }

        [HttpGet("TenderEvaluation")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> TenderEvaluation(string RPType, string divisionid, string districtid, string mainschemeid)
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


            whereClause += $" and t.rejid is null and t.topnedpricedt is null and (t.topneddt is not null or t.topnedbdt is not null) ";

            string query = "";
            if (RPType == "Total")
            {
                //Total
                query = $@"  select cast(1 as varchar) ID, 'Total' as Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (24,21)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a order by count(work_id) ";
            }
            if (RPType == "Division")
            {
                //Divsion
                query = $@" 

select ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
,agd.DivisionID as ID,dv.DivName_En as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (24,21)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a 
group by ID, Name
order by count(work_id) ";
            }

            if (RPType == "Scheme")
            {
                //schemwise
                query = $@" select ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
,cast(msc.MainSchemeID as varchar) as ID,msc.name as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
 inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (24,21)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a 
group by ID, Name
order by count(work_id)  ";
            }
            if (RPType == "District")
            {
                //districtwise
                query = $@" select ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
,dis.DISTRICT_ID as ID,dis.DBStart_Name_En as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (24,21)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a 
group by ID, Name
order by count(work_id) ";
            }

            return await _context.EvaluationDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }


        [HttpGet("PriceEvaluation")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> PriceEvaluation(string RPType, string divisionid, string districtid, string mainschemeid)
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


            whereClause += $" and t.rejid is null and t.topnedpricedt is not null  ";

            string query = "";
            if (RPType == "Total")
            {
                //Total
                query = $@"  select cast(1 as varchar) ID, 'Total' as Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,t.topnedpricedt,getdate()) daysSinceOpen
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (22)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a order by count(work_id) ";
            }
            if (RPType == "Division")
            {
                //Divsion
                query = $@" 

select ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
,agd.DivisionID as ID,dv.DivName_En as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (22)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a 
group by ID, Name
order by count(work_id) ";
            }

            if (RPType == "Scheme")
            {
                //schemwise
                query = $@" select ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
,cast(msc.MainSchemeID as varchar) as ID,msc.name as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
 inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (22)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a 
group by ID, Name
order by count(work_id)  ";
            }
            if (RPType == "District")
            {
                //districtwise
                query = $@" select ID, Name,count(work_id) as nosWorks,count(distinct TenderID) as nosTender,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr
,round(sum(daysSinceOpen)/count(work_id),0) as AvgDaysSince
 from 
 (
 select 
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt ,tw.noofcalls,t.TenderID
,convert(varchar,t.topneddt,105) TOpnedDT,convert(varchar,t.topnedbdt,105) topnedbdt ,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas
,datediff(dd,isnull(t.topneddt,t.topnedbdt),getdate()) daysSinceOpen
,dis.DISTRICT_ID as ID,dis.DBStart_Name_En as Name
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join
(
select p.ppid,p.Work_id  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
where p.NewProgress='Y' 
and wpp.PPID in (22)
) pl on pl.Work_id=tw.work_id
where  1=1 and w.IsDeleted is null and t.IsZonal is null " + whereClause + @" ) a 
group by ID, Name
order by count(work_id) ";
            }

            return await _context.EvaluationDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }

        [HttpGet("TobeTender")]
        //Grid progress wise rigt side on dashboard
        public async Task<ActionResult<IEnumerable<ToBeTenderSummaryDTO>>> TobeTender(string RPType, string divisionid, string districtid, string mainschemeid)
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


            //whereClause += $" and t.rejid is null and t.topnedpricedt is not null  ";

            string query = "";
            if (RPType == "GTotal")
            {
                query = @" select 1 as ID,'' as Name, count(Work_id) as nosWorks,cast(round(sum(ValueWorks)/100,2) as decimal(18,2)) as TValue, '' as NosValue


from 
(
select p.ppid,p.Work_id
,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName
,case when isnull(w.TSAmount,0)>0 then cast(w.TSAmount as decimal(18,2)) else  isnull(cast(w.AaAmt as decimal(18,2)),0) end as  ValueWorks
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)

inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where 1=1 and w.isdeleted  is null and w.MainSchemeID not in (121)  and  NewProgress='Y' and dash.did =1001 " + whereClause + @"
 ) b ";
            }

            if (RPType == "Progress")
            {
                query = @" select ppid as ID,parentprogress as Name,count(Work_id) as nosWorks,

cast(round(sum(ValueWorks)/100,2) as decimal(18,2)) as TValue
,cast(count(Work_id) as varchar)+'/'+ cast(cast(round(sum(ValueWorks)/100,2) as decimal(18,2))as varchar) as NosValue

from 
(
select p.ppid,p.Work_id
,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName,wpp.parentprogress
,case when isnull(w.TSAmount,0)>0 then cast(w.TSAmount as decimal(18,2)) else  isnull(cast(w.AaAmt as decimal(18,2)),0) end as  ValueWorks
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)

inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where 1=1

and  w.isdeleted  is null 

and w.MainSchemeID not in (121) and NewProgress='Y' and dash.did =1001  " + whereClause + @"
 ) b group by ppid,parentprogress
 order by ppid ";
            }



            return await _context.ToBeTenderSummaryDbSet
         .FromSqlRaw(query)
         .ToListAsync();
        }

        [HttpGet("TobeTenderDetailsAS1")]
        public async Task<ActionResult<IEnumerable<TobeTenderDetailsASDTO>>> TobeTenderDetailsAS1(string divisionId, string mainSchemeId, string distid)
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

            whereClause += $" and p.PPID= 1 ";
            string query = @" select msc.Name as Head, dv.divname_en as Division, dis.DBStart_Name_En as District, b.Block_Name_En, hc.DETAILS_ENG, d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname
,w.letterno,convert(varchar, AADate,105) as ASDate,  isnull(cast(w.AaAmt as decimal(18,2)),0) as  ValueWorks
,wpp.parentprogress

,p.Work_id
,p.ppid,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName
, '' as wocancelletterno 
,''as PDate ,'' as WOCancelProposalLetterNo 
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID= w.worklocationdist_id
inner join agencydivisionmaster agd on agd.DivisionID= w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  dhrsHealthCenter d on cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint)
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
left outer join SWDetails s on s.SWId= w.work_description_id
inner join WorkLevelParent wpp on wpp.ppid= p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID= wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did= wpg.did
 left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
where 1=1 and w.isdeleted  is null and w.MainSchemeID not in (121)
 and NewProgress = 'Y' and dash.did = 1001 " + whereClause + @"

order by dv.divname_en, dis.DBStart_Name_En ";
            return await _context.TobeTenderDetailsASDbSet
               .FromSqlRaw(query)
               .ToListAsync();
        }


        [HttpGet("TobeTenderDetailsWOCancelled1934")]
        public async Task<ActionResult<IEnumerable<TobeTenderDetailsASDTO>>> TobeTenderDetailsWOCancelled1934(string divisionId, string mainSchemeId, string distid, string ppid)
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

            if (ppid == "19")
            {
                whereClause += $" and p.PPID= 19 ";
            }
            if (ppid == "34")
            {
                whereClause += $" and p.PPID= 34 ";
            }
            else
            {
                whereClause += $" and p.PPID in (34,19) ";
            }
            string query = @" select msc.Name as Head,dv.divname_en as Division,dis.DBStart_Name_En as District,b.Block_Name_En,hc.DETAILS_ENG,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname
,w.letterno,convert(varchar,AADate,105) as ASDate,case when isnull(w.TSAmount,0)>0 then cast(w.TSAmount as decimal(18,2)) else  isnull(cast(w.AaAmt as decimal(18,2)),0) end as  ValueWorks
,wpp.parentprogress
,p.wocancelletterno,convert(varchar,p.ProgressDT,105) as PDate,
p.WOCancelProposalLetterNo
,p.Work_id
,p.ppid,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName

from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
 left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
where 1=1 and w.isdeleted  is null  and w.MainSchemeID not in (121)
 and NewProgress='Y' and dash.did =1001
 " + whereClause + @"
order by dv.divname_en,dis.DBStart_Name_En ";
            return await _context.TobeTenderDetailsASDbSet
              .FromSqlRaw(query)
              .ToListAsync();
        }



        [HttpGet("TobeTenderAppliedZonalPermission25")]
        public async Task<ActionResult<IEnumerable<TobeTenderZonalAppliedDTO>>> TobeTenderDetailsWOCancelled1934(string divisionId, string mainSchemeId, string distid)
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


            string query = @" select msc.Name as Head,dv.divname_en as Division,dis.DBStart_Name_En as District,
b.Block_Name_En,hc.DETAILS_ENG,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname
,w.letterno,convert(varchar,AADate,105) as ASDate,cast(tw.appliedvalue as decimal(18,2)) as value
,t.ZonalType 
,t.tenderno as NITNo
,pl.LProgress,convert(varchar,pl.ProgressDT,105) as ProgressDT
,t.TenderID,pl.ppid,tw.apild,tw.work_id
from MasZonalWorkApply tw
inner join
(
select p.ppid,p.WorkLevel as level_id ,isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,r.Remarks as mremarks  from  WorkPhysicalProgress p
left outer join WorkLevelParent wpp on wpp.ppid=p.ppid
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y' and p.ppid in (25) 
group by p.Work_id,wpp.ParentProgress,ProgressDT, p.Remarks,p.ppid,p.WorkLevel,r.Remarks
) pl on pl.Work_id=tw.work_id
inner join MasTender t on t.TenderID=tw.ZTenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
left outer join SWDetails s on s.SWId=w.work_description_id 
left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID

where   1=1 and w.IsDeleted is null and t.IsZonal ='Y' and tw.IsAppliedCancel is null  and w.MainSchemeID not in (121)
" + whereClause + @"
and tw.ZDistpatchNo is not null  and  tw.ishoalloted is null order by pl.ProgressDT desc ";
            return await _context.TobeTenderZonalAppliedDbSet
              .FromSqlRaw(query)
              .ToListAsync();
        }

        [HttpGet("TobeTenderRejection23")]
        public async Task<ActionResult<IEnumerable<TobeTenderRejDetailsDTO>>> TobeTenderRejection23(string divisionId, string mainSchemeId, string distid)
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


            string query = @" select msc.Name as Head,dv.divname_en as Division,dis.DBStart_Name_En as District,b.Block_Name_En,hc.DETAILS_ENG,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname
,w.letterno,convert(varchar,AADate,105) as ASDate,case when isnull(w.TSAmount,0)>0 then cast(w.TSAmount as decimal(18,2)) else  isnull(cast(w.AaAmt as decimal(18,2)),0) end as  ValueWorks
,wpp.parentprogress

,p.wocancelletterno,convert(varchar,p.ProgressDT,105) as PDate,
p.WOCancelProposalLetterNo
,p.Work_id
,p.ppid,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName
,t.TenderNo as LastNIT,t.eProcNo as LastEprocno,rj.RejReason,convert(varchar,t.RejEntryDT,105) as RejectedDT
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
 left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
 left outer join
 (
 select max(twid) twid,Work_id from MasTenderWorks tw
 where tw.rejid is not null 
 group by Work_id
 ) ltw on ltw.Work_id=w.work_id
 left outer join MasTenderWorks Tw on tw.TWID=ltw.twid
 left outer join MasTenderRejReason rj on rj.RejID=tw.RejID
left outer join MasTender t on t.TenderID= Tw.tenderid
where 1=1 and NewProgress='Y' and dash.did =1001
and p.PPID=23  and w.isdeleted  is null 
 and w.MainSchemeID not in (121)
" + whereClause + @"
order by Tw.RejEntryDT desc ";
            return await _context.TobeTenderRejDetailsDbSet
              .FromSqlRaw(query)
              .ToListAsync();
        }


        //gyan test for github
        //https://localhost:7247/api/EMS/GetTenderStatus
        [HttpGet("GetTenderStatus")]
        public async Task<ActionResult<IEnumerable<GetTenderStatusDTO>>> GetTenderStatus(string NormalZonal)
        {
            string whclausez = "  ";

            if (NormalZonal == "Z")
            {
                whclausez = " and t.IsZonal is not null ";
            }
            if (NormalZonal == "N")
            {
                whclausez = " and t.IsZonal is null ";
            }
            else
            {

            }

            string query = $@" select PGroupID ,PPID, TenderStatus,nosTender,nosWorks,TotalValuecr from (
select PGroupID,PPID,TenderStatus,
 count(distinct TenderID) as nosTender,count(work_id) as nosWorks,
isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr

from 
(
 
 select  PGroupID,pl.ParentProgress as TenderStatus,
                        tw.work_id,
                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddate ,tw.noofcalls,t.TenderID
,convert(varchar,tw.TOpnedDT,103) TOpnedDate,cast(TSAmount as decimal(18,2)) as TSAmt
,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas,
t.enddt ,t.rejid,t.topneddt,t.topnedbdt, t.topnedpricedt
,case when  (enddt>=getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null ) then 1 else 0 end as Live
,case when  (enddt<getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null ) then 1 else 0 end as LiveTimeOver
from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join  
(
select p.ppid,p.Work_id,wpp.ParentProgress,wg.PGroupID  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wg on wg.PGroupID=wpp.PGroupID
where p.NewProgress='Y' 
 and wg.PGroupID in (3,4,6)
) pl on pl.Work_id=tw.work_id

where  1=1 and t.rejid is null and w.IsDeleted is null  " + whclausez + @"  
) a group by TenderStatus,PGroupID,PPID 

union

select 1001 as PGroupID, 1001 as PPID,'To Be Tender' as TenderStatus,0 as nosTender ,count(distinct Work_id) nosWorks,isnull(cast( sum(ValueWorks)/100 as decimal(18,2)),0)  as TotalValuecr from (

select msc.MainSchemeID, msc.Name as Head,dv.divname_en as Division,dis.DBStart_Name_En as District,b.Block_Name_En,hc.DETAILS_ENG,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname
,w.letterno,convert(varchar,AADate,105) as ASDate
,  isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt, isnull(w.TSAmount,0) as TSAmount
,case when isnull(w.TSAmount,0)>0 then cast(w.TSAmount as decimal(18,2)) else  isnull(cast(w.AaAmt as decimal(18,2)),0) end as  ValueWorks
,wpp.parentprogress,wpp.ppid

,p.wocancelletterno,convert(varchar,p.ProgressDT,105) as PDate,
p.WOCancelProposalLetterNo
,p.Work_id
,p.ppid as OldPPID,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName
,t.TenderNo as LastNIT,t.eProcNo as LastEprocno,rj.RejReason,convert(varchar,t.RejEntryDT,105) as RejectedDT
,t.iszonal,t.zonaltype
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
 left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
 left outer join
 (
 select max(twid) twid,Work_id from MasTenderWorks tw
 where tw.rejid is not null 
 group by Work_id
 ) ltw on ltw.Work_id=w.work_id
 left outer join MasTenderWorks Tw on tw.TWID=ltw.twid
 left outer join MasTenderRejReason rj on rj.RejID=tw.RejID
left outer join MasTender t on t.TenderID= Tw.tenderid
where 1=1
 and NewProgress='Y' 
and dash.did =1001
and t.IsZonal is null
 and w.isdeleted  is null 
 and w.MainSchemeID not in (121,137,140,142)
 and wpp.PPID not in (25)
 and isnull(cast(w.AaAmt as decimal(18,2)),0)>20

)tb


)x order by PGroupID,PPID ";

            //            string query = $@" select PGroupID,PPID,TenderStatus,
            // count(distinct TenderID) as nosTender,count(work_id) as nosWorks,
            //isnull(cast(sum(Valueworksinlas)/100 as decimal(18,2)),0)  TotalValuecr

            //from 
            //(

            // select  PGroupID,pl.ParentProgress as TenderStatus,
            //                        tw.work_id,
            //                        t.tenderno,t.eprocno,cast(AaAmt as decimal(18,2)) as ASAmt,
            //                        convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddate ,tw.noofcalls,t.TenderID
            //,convert(varchar,tw.TOpnedDT,103) TOpnedDate,cast(TSAmount as decimal(18,2)) as TSAmt
            //,pl.PPID,case when isnull(TSAmount,0) >0 then TSAmount else AaAmt end as Valueworksinlas,
            //t.enddt ,t.rejid,t.topneddt,t.topnedbdt, t.topnedpricedt
            //,case when  (enddt>=getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null ) then 1 else 0 end as Live
            //,case when  (enddt<getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null ) then 1 else 0 end as LiveTimeOver
            //from MasTenderWorks tw
            //inner join MasTender t on t.TenderID=tw.tenderid
            //inner join WorkMaster w on w.work_id=tw.work_id
            //inner join  
            //(
            //select p.ppid,p.Work_id,wpp.ParentProgress,wg.PGroupID  from  WorkPhysicalProgress p
            //inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
            //inner join WorkLevelParentGroup wg on wg.PGroupID=wpp.PGroupID
            //where p.NewProgress='Y' 
            // and wg.PGroupID in (3,4,6)
            //) pl on pl.Work_id=tw.work_id

            //where  1=1 and t.rejid is null and w.IsDeleted is null " + whclausez + @" 
            //) a group by TenderStatus,PGroupID,PPID order by PGroupID,PPID  ";



            var result = await _context.GetTenderStatusDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/EMS/GetTenderStatusDetail
        [HttpGet("GetTenderStatusDetail")]
        public async Task<ActionResult<IEnumerable<GetTenderStatusDetailDTO>>> GetTenderStatusDetail(Int32 pGroupId, Int32 ppid)
        {
            string whPgroupId = "";

            string query = $@" ";

            if (pGroupId == 0) //All
            {
                query = $@"  select  isnull(tsr.tenderstatus, pl.ParentProgress) as TenderStatus,
tw.work_id,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,

t.tenderno,t.eprocno,
cast(AaAmt as decimal(18,2)) as ASAmt,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddate ,tw.noofcalls
,convert(varchar,tw.TOpnedDT,103) CoverADT,convert(varchar,t.topnedbdt,103) as CoverBDT, convert(varchar,t.topnedpricedt,103) as CoverCDT 

,(case when pl.PGroupID=3 then ( case when  (enddt>=getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null ) then 'Live' else 'Tnder Closed,Pending to Open' end) else  pl.ParentProgress end) as Tstatus

,PGroupID,t.TenderID,t.rejid,tsr.tenderremark,tsr.entrydate from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join  
(
select p.ppid,p.Work_id,wpp.ParentProgress,wg.PGroupID  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wg on wg.PGroupID=wpp.PGroupID
where p.NewProgress='Y' 
 and wg.PGroupID in (3,4,6)
) pl on pl.Work_id=tw.work_id 

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=t.TenderID 
where  1=1 and t.rejid is null and w.IsDeleted is null and t.IsZonal is null
order by t.startdt  ";
            }

            if (pGroupId == 3 && ppid == 3) //Live
            {
                query = $@"  
 select  isnull(tsr.tenderstatus, pl.ParentProgress)  as TenderStatus,
tw.work_id,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,

t.tenderno,t.eprocno,
cast(AaAmt as decimal(18,2)) as ASAmt,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddate ,tw.noofcalls
,convert(varchar,tw.TOpnedDT,103) CoverADT,convert(varchar,t.topnedbdt,103) as CoverBDT, convert(varchar,t.topnedpricedt,103) as CoverCDT 
,case when  (enddt>=getdate() and t.rejid is null and t.topneddt is null and t.topnedbdt is null  and t.topnedpricedt is null ) then 'Live' else 'Tnder Closed,Pending to Open' end  as Tstatus

,PGroupID,t.TenderID,t.rejid,tsr.tenderremark,tsr.entrydate from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join  
(
select p.ppid,p.Work_id,wpp.ParentProgress,wg.PGroupID  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wg on wg.PGroupID=wpp.PGroupID
where p.NewProgress='Y' 
 and wg.PGroupID in (3)
) pl on pl.Work_id=tw.work_id

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=t.TenderID 


where  1=1 and t.rejid is null and w.IsDeleted is null and t.IsZonal is null order by t.startdt ";
            }


            if (pGroupId == 4)
            {
                string whPpid = "";

                if (ppid == 21 || ppid == 24 || ppid == 22) // 21 - Cover A Evolution, 24 - Cover B, 22- Cover C
                {
                    whPpid = " and wpp.PPID=" + ppid + "  ";
                }




                query = $@"  select  isnull(tsr.tenderstatus, pl.ParentProgress)  as TenderStatus,
tw.work_id,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,

t.tenderno,t.eprocno,
cast(AaAmt as decimal(18,2)) as ASAmt,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddate ,tw.noofcalls
,convert(varchar,tw.TOpnedDT,103) CoverADT,convert(varchar,t.topnedbdt,103) as CoverBDT, convert(varchar,t.topnedpricedt,103) as CoverCDT 
,pl.ParentProgress  as Tstatus

,PGroupID,t.TenderID,t.rejid,tsr.tenderremark,tsr.entrydate from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join  
(
select p.ppid,p.Work_id,wpp.ParentProgress,wg.PGroupID  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wg on wg.PGroupID=wpp.PGroupID
where p.NewProgress='Y' 
 and wg.PGroupID in (4) " + whPpid + @"
) pl on pl.Work_id=tw.work_id 

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=t.TenderID 

where  1=1 and t.rejid is null and w.IsDeleted is null and t.IsZonal is null order by t.startdt ";
            }

            if (pGroupId == 6)
            {
                string whPpid = "";

                if (ppid == 4) //Acceptance
                {
                    whPpid = " and wpp.PPID=" + ppid + "  ";
                }

                query = $@"  select  isnull(tsr.tenderstatus, pl.ParentProgress)  as TenderStatus,
tw.work_id,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname,

t.tenderno,t.eprocno,
cast(AaAmt as decimal(18,2)) as ASAmt,cast(TSAmount as decimal(18,2)) as TSAmount,convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddate ,tw.noofcalls
,convert(varchar,tw.TOpnedDT,103) CoverADT,convert(varchar,t.topnedbdt,103) as CoverBDT, convert(varchar,t.topnedpricedt,103) as CoverCDT 
,'LOI Generated' as Tstatus,entrydate,tenderremark

,PGroupID,t.TenderID,t.rejid from MasTenderWorks tw
inner join MasTender t on t.TenderID=tw.tenderid
inner join WorkMaster w on w.work_id=tw.work_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join  
(
select p.ppid,p.Work_id,wpp.ParentProgress,wg.PGroupID  from  WorkPhysicalProgress p
inner join  WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wg on wg.PGroupID=wpp.PGroupID
where p.NewProgress='Y' 
 and wg.PGroupID in (6) " + whPpid + @"
) pl on pl.Work_id=tw.work_id

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=t.TenderID 


where  1=1 and t.rejid is null and w.IsDeleted is null and t.IsZonal is null order by t.startdt  ";
            }




            var result = await _context.GetTenderStatusDetailDbSet
        .FromSqlRaw(query)
        .ToListAsync();

            return Ok(result);
        }


        [HttpGet("ZonalTenderStatus")]
        public async Task<ActionResult<IEnumerable<ZonalTenderStatusDTO>>> ZonalTenderStatus()
        {


            string query = $@"   select mt.TID,mt.CovName as TenderStatus,isnull(CntTender,0) as CntTender, CAST(ISNULL(TenderValue, 0) AS VARCHAR) AS TenderValue
  from masTenderStatus mt
 
 left outer join 
(

select TID,CovName,count( distinct x.TenderID) as CntTender, sum(Capacity) as TenderValue  from  
(

select t.TenderID,t.TenderNo,t.eProcNo,t.Discription,convert(varchar,t.startDT,103) as startDT 
,convert(varchar,t.endDT,103) as  endDT,cast(t.Capacity as bigint) as Capacity 
,t.ZonalType ,d.DBStart_Name_En district,

case when t.ZonalType='Block' then  b.Block_Name_En 
else case when t.ZonalType='NagarPalikaParishad' then ps.NagarPalika 
else  case when t.ZonalType='NagarNigam' then nn.NagarNigam else '' end end end as block

,d.District_ID as DistrictID,NagarNigam 


,isnull(t.calls,1) as calls  --,t2.TenderNo
, ISNULL(tsr.tenderstatus,CovName) as tenderstatus,tsr.tenderremark,tsr.entrydate,mt.CovName,mt.TID

,convert(varchar,t.TOpnedDT,103) CoverA,
 convert(varchar,t.topnedpricedt,103) as CoverC
from MasTender t 
left outer join Districts d on d.District_ID = t.DistrictID
left outer join BlocksMaster b on b.Block_ID = (case when LEN(Blockid)=1 then '0'+ convert(varchar, Blockid) else Blockid end) and b.District_ID = d.District_ID
left outer join MasNagarPalika ps on cast(ps.npid as int) = cast(t.NPID as int) and ps.DistrictID = d.District_ID
left outer join MasNagarNigam nn on nn.NNID = t.NNID and nn.DistrictID = d.District_ID
left outer join ContractMaster c on c.Contractorid = t.ContractorID
left outer join MasTender t2 on t2.tenderid=t.TRefID

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID,tr.TID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=t.TenderID 
left outer join masTenderStatus mt on  mt.TID=tsr.TID


where t.IsZonal='Y' and isnull(t.IsAccept,'N') ='N' and isnull(t.IsReject,'No') ='No'
 and t.IsCompleteWorkorder  is null 
 --and mt.TID=1

 )x group by CovName,TID
 
 )TS on   TS.TID=mt.TID
where 1=1 and mt.tid not in (6)
 order By mt.TID
 ";

            var result = await _context.ZonalTenderStatusDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("ZonalTenderStatusDetail")]
        public async Task<ActionResult<IEnumerable<ZonalTenderStatusDetailDTO>>> ZonalTenderStatusDetail(Int32 tid)
        {
            string whTid = "";

            if (tid != 0)
            {
                whTid = @" and mt.TID= " + tid + @" ";
            }

            string query = $@" SELECT 
    CAST(t.TenderID AS VARCHAR) AS TenderID,
    CAST(t.TenderNo AS VARCHAR) AS TenderNo,
    CAST(t.eProcNo AS VARCHAR) AS eProcNo,    
    t.Discription,
    CONVERT(VARCHAR, t.startDT, 103) AS startDT,
    CONVERT(VARCHAR, t.endDT, 103) AS endDT,
    CAST(t.Capacity AS VARCHAR) AS Capacity,
    t.ZonalType,
    d.DBStart_Name_En AS district,

    CASE 
        WHEN t.ZonalType = 'Block' THEN b.Block_Name_En 
        WHEN t.ZonalType = 'NagarPalikaParishad' THEN ps.NagarPalika 
        WHEN t.ZonalType = 'NagarNigam' THEN nn.NagarNigam 
        ELSE '' 
    END AS block,

    CAST(d.District_ID AS VARCHAR) AS DistrictID,
    NagarNigam,

    CAST(ISNULL(t.calls, 1) AS VARCHAR) AS calls,
    ISNULL(tsr.tenderstatus, CovName) AS tenderstatus,
    tsr.tenderremark,
    tsr.entrydate,

    CONVERT(VARCHAR, t.TOpnedDT, 103) AS CoverA,
    CONVERT(VARCHAR, t.topnedpricedt, 103) AS CoverC
from MasTender t 
left outer join Districts d on d.District_ID = t.DistrictID
left outer join BlocksMaster b on b.Block_ID = (case when LEN(Blockid)=1 then '0'+ convert(varchar, Blockid) else Blockid end) and b.District_ID = d.District_ID
left outer join MasNagarPalika ps on cast(ps.npid as int) = cast(t.NPID as int) and ps.DistrictID = d.District_ID
left outer join MasNagarNigam nn on nn.NNID = t.NNID and nn.DistrictID = d.District_ID
left outer join ContractMaster c on c.Contractorid = t.ContractorID
left outer join MasTender t2 on t2.tenderid=t.TRefID

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID,tr.TID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=t.TenderID 
left outer join masTenderStatus mt on  mt.TID=tsr.TID


where t.IsZonal='Y' and isnull(t.IsAccept,'N') ='N' and isnull(t.IsReject,'No') ='No'
 and t.IsCompleteWorkorder  is null 
 " + whTid + @"  ";

            var result = await _context.ZonalTenderStatusDetailDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetToBeTender")]
        public async Task<ActionResult<IEnumerable<GetToBeTenderDTO>>> GetToBeTender()
        {


            string query = $@"  select Head,Division,District,Work_id,workname, letterno as ASLetterNO,ASDate,ASAmt,TSAmount,ValueWorks,DashName as WorkStatus
 from (
select msc.MainSchemeID, msc.Name as Head,dv.divname_en as Division,dis.DBStart_Name_En as District,b.Block_Name_En,hc.DETAILS_ENG,d.NAME_ENG+' - '+isnull(s.SWName,'-')  as workname
,w.letterno,convert(varchar,AADate,105) as ASDate
,  isnull(cast(w.AaAmt as decimal(18,2)),0) as ASAmt, isnull(w.TSAmount,0) as TSAmount
,case when isnull(w.TSAmount,0)>0 then cast(w.TSAmount as decimal(18,2)) else  isnull(cast(w.AaAmt as decimal(18,2)),0) end as  ValueWorks
,wpp.parentprogress,wpp.ppid

,p.wocancelletterno,convert(varchar,p.ProgressDT,105) as PDate,
p.WOCancelProposalLetterNo
,p.Work_id
,p.ppid as OldPPID,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName
,t.TenderNo as LastNIT,t.eProcNo as LastEprocno,rj.RejReason,convert(varchar,t.RejEntryDT,105) as RejectedDT
,t.iszonal,t.zonaltype
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
inner join dhrsHealthCentreType hc on hc.Type_ID=d.type
left outer join SWDetails s on s.SWId=w.work_description_id 
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
 left outer join BlocksMaster b on cast(b.Block_ID as int) = cast(d.BLOCK_ID as int)  and b.District_ID =dis.District_ID
 left outer join
 (
 select max(twid) twid,Work_id from MasTenderWorks tw
 where tw.rejid is not null 
 group by Work_id
 ) ltw on ltw.Work_id=w.work_id
 left outer join MasTenderWorks Tw on tw.TWID=ltw.twid
 left outer join MasTenderRejReason rj on rj.RejID=tw.RejID
left outer join MasTender t on t.TenderID= Tw.tenderid
where 1=1
 and NewProgress='Y' 
and dash.did =1001
and t.IsZonal is null
 and w.isdeleted  is null 
 and w.MainSchemeID not in (121,137,140,142)
 and wpp.PPID not in (25)
 and isnull(cast(w.AaAmt as decimal(18,2)),0)>20
 )x  ";

            var result = await _context.GetToBeTenderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


    }
}

