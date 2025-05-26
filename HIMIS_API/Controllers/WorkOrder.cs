using System.IO;
using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkOrder : Controller
    {
        private readonly DbContextData _context;
        public WorkOrder(DbContextData context)
        {
            _context = context;
        }
        [HttpGet("WorkOrderGenerated")]
        public async Task<ActionResult<IEnumerable<WOrderGeneratedDTO>>> WorkOrderGenerated(string RPType, string divisionid, string districtid, string fromdt, string todt, string MainSchemeID)
        {
            string? whereClause = "";

            if (MainSchemeID != "0")
            {

                whereClause += $" and msc.MainSchemeID  = '{MainSchemeID}'";

            }

            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and dis.District_ID  = '{districtid}'";

            }
            if (fromdt != "0")
            {
                if (todt != "0")
                {
                    whereClause += $" and a.WrokOrderDT between   '{fromdt}' and '{todt}' ";

                }
                else
                {
                    whereClause += $" and a.WrokOrderDT between   '{fromdt}' and getdate() ";
                }


            }
            string query = "";
            if (RPType == "GTotal")
            {
                //Gtotal
                query = $@" select cast(1 as varchar) as ID,'Total' as Name,
cast(count(work_id) as varchar) as TotalWorks,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as TotalTVC
 ,round(sum(daytaken)/count(work_id),0) as AVGDaysSinceAcceptance
 ,sum(ZonalWorks) as ZonalWorks
 ,cast(SUM(TVCZonal)/100 as decimal(18,2))  as TotalZonalTVC
  ,sum(TenderWorks) as TenderWorks
  ,cast(SUM(TVCNormal)/100 as decimal(18,2))  as TotalNormalTVC
from 
(
select agd.DivisionID,dv.divname_en , a.work_id,a.acceptletterdt,a.WrokOrderDT,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

, case when isnull(t.iszonal,'N')='Y' then datediff(d,a.HOAllotedDT,a.WrokOrderDT) else datediff(d,a.acceptletterdt,a.WrokOrderDT) end daytaken,
a.tenderid,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType
,HOAllotedDT,t.iszonal
, case when isnull(t.iszonal,'N')='Y' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCZonal
, case when isnull(t.iszonal,'N')='N' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCNormal,
case when isnull(t.iszonal,'N')='Y' then 1 else 0 end as ZonalWorks,
case when isnull(t.iszonal,'N')='N' then 1 else 0 end as TenderWorks
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID=a.tenderid
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd
on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and wpp.PPID=6 
group by Work_id 
) p on p.Work_id=w.work_id


where a.AcceptLetterDT is not null
and w.IsDeleted is null and a.WrokOrderDT is not null and a.workorderrefnogovt is not null
and  w.MainSchemeID not in (121)  " + whereClause + @"
) a ";
            }
            if (RPType == "Total")
            {
                //divisinwise
                query = $@" select cast(DivisionID as varchar) as ID,divname_en as Name,
cast(count(work_id) as varchar) as TotalWorks,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as TotalTVC
 ,round(sum(daytaken)/count(work_id),0) as AVGDaysSinceAcceptance
 ,sum(ZonalWorks) as ZonalWorks
 ,cast(SUM(TVCZonal)/100 as decimal(18,2))  as TotalZonalTVC
  ,sum(TenderWorks) as TenderWorks
  ,cast(SUM(TVCNormal)/100 as decimal(18,2))  as TotalNormalTVC

from 
(
select agd.DivisionID,dv.divname_en , a.work_id,a.acceptletterdt,a.WrokOrderDT,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

, case when isnull(t.iszonal,'N')='Y' then datediff(d,a.HOAllotedDT,a.WrokOrderDT) else datediff(d,a.acceptletterdt,a.WrokOrderDT) end daytaken,
a.tenderid,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType
,HOAllotedDT,t.iszonal
, case when isnull(t.iszonal,'N')='Y' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCZonal
, case when isnull(t.iszonal,'N')='N' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCNormal,
case when isnull(t.iszonal,'N')='Y' then 1 else 0 end as ZonalWorks,
case when isnull(t.iszonal,'N')='N' then 1 else 0 end as TenderWorks
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID=a.tenderid
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd
on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and wpp.PPID=6 
group by Work_id 
) p on p.Work_id=w.work_id

where a.AcceptLetterDT is not null
and w.IsDeleted is null and a.WrokOrderDT is not null and a.workorderrefnogovt is not null
and  w.MainSchemeID not in (121)  " + whereClause + @"
) a
group by  DivisionID,divname_en
order by count(work_id) desc ";
            }
            if (RPType == "Scheme")
            {
                //Scheme

                query = $@" select cast(MainSchemeID as varchar) as ID,name as Name,
cast(count(work_id) as varchar) as TotalWorks,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as TotalTVC
 ,round(sum(daytaken)/count(work_id),0) as AVGDaysSinceAcceptance
 ,sum(ZonalWorks) as ZonalWorks
 ,cast(SUM(TVCZonal)/100 as decimal(18,2))  as TotalZonalTVC
  ,sum(TenderWorks) as TenderWorks
  ,cast(SUM(TVCNormal)/100 as decimal(18,2))  as TotalNormalTVC

from 
(
select msc.name,w.MainSchemeID , a.work_id,a.acceptletterdt,a.WrokOrderDT,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

, case when isnull(t.iszonal,'N')='Y' then datediff(d,a.HOAllotedDT,a.WrokOrderDT) else datediff(d,a.acceptletterdt,a.WrokOrderDT) end daytaken,
a.tenderid,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType
,HOAllotedDT,t.iszonal
, case when isnull(t.iszonal,'N')='Y' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCZonal
, case when isnull(t.iszonal,'N')='N' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCNormal,
case when isnull(t.iszonal,'N')='Y' then 1 else 0 end as ZonalWorks,
case when isnull(t.iszonal,'N')='N' then 1 else 0 end as TenderWorks
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID=a.tenderid
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd
on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and wpp.PPID=6 
group by Work_id 
) p on p.Work_id=w.work_id

where a.AcceptLetterDT is not null
and w.IsDeleted is null and a.WrokOrderDT is not null and a.workorderrefnogovt is not null
and  w.MainSchemeID not in (121)  " + whereClause + @"
) a
group by  MainSchemeID,name
order by count(work_id) desc ";
            }
            if (RPType == "District")
            {
                //Districtwise
                query = $@" select cast(District_ID as varchar) as ID,dbstart_name_en as Name,
cast(count(work_id) as varchar) as TotalWorks,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as TotalTVC
 ,round(sum(daytaken)/count(work_id),0) as AVGDaysSinceAcceptance
 ,sum(ZonalWorks) as ZonalWorks
 ,cast(SUM(TVCZonal)/100 as decimal(18,2))  as TotalZonalTVC
  ,sum(TenderWorks) as TenderWorks
  ,cast(SUM(TVCNormal)/100 as decimal(18,2))  as TotalNormalTVC

from 
(
select dis.dbstart_name_en,dis.District_ID , a.work_id,a.acceptletterdt,a.WrokOrderDT,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

, case when isnull(t.iszonal,'N')='Y' then datediff(d,a.HOAllotedDT,a.WrokOrderDT) else datediff(d,a.acceptletterdt,a.WrokOrderDT) end daytaken,
a.tenderid,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType
,HOAllotedDT,t.iszonal
, case when isnull(t.iszonal,'N')='Y' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCZonal
, case when isnull(t.iszonal,'N')='N' then (case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end)
else 0  end TVCNormal,
case when isnull(t.iszonal,'N')='Y' then 1 else 0 end as ZonalWorks,
case when isnull(t.iszonal,'N')='N' then 1 else 0 end as TenderWorks
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID=a.tenderid
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd
on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and wpp.PPID=6 
group by Work_id 
) p on p.Work_id=w.work_id

where a.AcceptLetterDT is not null
and w.IsDeleted is null and a.WrokOrderDT is not null and a.workorderrefnogovt is not null
and  w.MainSchemeID not in (121)  " + whereClause + @"
) a
group by  District_ID,dbstart_name_en
order by count(work_id) desc ";
            }
          return await _context.WOrderGeneratedDbSet
        .FromSqlRaw(query)
        .ToListAsync();
        }


            [HttpGet("WOPendingTotal")]
        public async Task<ActionResult<IEnumerable<WOPendingTotalDTO>>> WOPendingTotal(string RPType,string divisionid,string districtid,string MainSchemeID)
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

            if (MainSchemeID != "0")
            {

                whereClause += $" and msc.MainSchemeID  = '{MainSchemeID}'";

            }

            string query = "";
            if (RPType == "Total")
            {
                query = $@" select cast(DivisionID as varchar) as ID,divname_en as Name,cast(count(work_id) as varchar) as PendingWork,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as ContrctValuecr
 ,cast(sum(NoofWorksGreater7Days) as varchar) as NoofWorksGreater7Days

from 
(
select agd.DivisionID,dv.divname_en , a.work_id,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract
,case when datediff(d,a.acceptletterdt,GETDATE())>7 then 1 else 0 end as NoofWorksGreater7Days
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'  and dash.did =3001 and wpp.PPID=4 
group by Work_id 
) p on p.Work_id=w.work_id

where a.AcceptLetterDT>'01-01-2022'
and w.IsDeleted is null and a.WrokOrderDT is null and a.workorderrefnogovt is  null
and  w.MainSchemeID not in (121)  " + whereClause + @"
) a
group by  DivisionID,divname_en
order by count(work_id) desc ";
            }
            if (RPType == "Scheme")
            {
                //Scheme
                query = $@" select cast(MainSchemeID as varchar) as ID,schemename as Name,cast(count(work_id) as varchar) as PendingWork,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as ContrctValuecr
  ,cast(sum(NoofWorksGreater7Days) as varchar) as NoofWorksGreater7Days
from 
(
select w.MainSchemeID,msc.name as schemename , a.work_id,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract
,case when datediff(d,a.acceptletterdt,GETDATE())>7 then 1 else 0 end as NoofWorksGreater7Days
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'  and dash.did =3001 and wpp.PPID=4 
group by Work_id 
) p on p.Work_id=w.work_id


where a.AcceptLetterDT>'01-01-2022'
and w.IsDeleted is null and a.WrokOrderDT is null and a.workorderrefnogovt is  null
and  w.MainSchemeID not in (121) " + whereClause + @"
) a
group by  MainSchemeID,schemename
order by count(work_id) desc ";
            }
            if (RPType == "Contractor")
            {
                //Contractor
                query = $@" 
select cast(contractorid as varchar) as ID,Contractor as Name,MobNo,cast(count(work_id) as varchar) as PendingWork,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as ContrctValuecr
   ,cast(sum(NoofWorksGreater7Days) as varchar) as NoofWorksGreater7Days
from 
(
select a.contractorid,c.englishname as Contractor,c.MobNo, a.work_id,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract
,case when datediff(d,a.acceptletterdt,GETDATE())>7 then 1 else 0 end as NoofWorksGreater7Days
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
inner join  ContractMaster c on c.Contractorid=a.ContractorID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'  and dash.did =3001 and wpp.PPID=4 
group by Work_id 
) p on p.Work_id=w.work_id

where a.AcceptLetterDT>'01-01-2022'
and w.IsDeleted is null and a.WrokOrderDT is null and a.workorderrefnogovt is  null
and  w.MainSchemeID not in (121) " + whereClause + @"
) a
group by  contractorid,Contractor,MobNo
order by count(work_id) desc ";
            }

            if (RPType == "District")
            {
               //districtwise
                query = $@" select cast(DISTRICT_ID as varchar) as  ID,District as  Name,cast(count(work_id) as varchar) as PendingWork,
 cast(SUM(TotalAmountOfContract)/100 as decimal(18,2))  as ContrctValuecr
 ,cast(sum(NoofWorksGreater7Days) as varchar) as NoofWorksGreater7Days

from 
(
select dis.DISTRICT_ID, dis.DBStart_Name_En as District , a.work_id,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract
,case when datediff(d,a.acceptletterdt,GETDATE())>7 then 1 else 0 end as NoofWorksGreater7Days
from  WorkMaster  w 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join Division dv on dv.Div_Id=dis.Div_Id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'  and dash.did =3001 and wpp.PPID=4 
group by Work_id 
) p on p.Work_id=w.work_id

where a.AcceptLetterDT>'01-01-2022'
and w.IsDeleted is null and a.WrokOrderDT is null and a.workorderrefnogovt is  null
and  w.MainSchemeID not in (121)  " + whereClause + @"
) a
group by   DISTRICT_ID,District
order by count(work_id) desc ";
            }

            return await _context.WOPendingTotalDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getWorkOrderPendingDetailsNew")]

        public async Task<ActionResult<IEnumerable<WorkorderpendingdetailsDTO>>> getWorkOrderPendingDetailsNew(string divisionId, string mainSchemeId, string distid, string contractid)
        {
            string? whereClause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and di.District_ID= '{distid}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            if (contractid != "0")
            {

                whereClause += $" and rtrim(ltrim(cnt.Contractorid)) = '{contractid}'";
            }
            string query = "";

            query = $@"select w.LetterNo, w.work_id, msc.Name as Head,ap.login_name as Approver,
 wt.type_name ,
  di.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  d.NAME_ENG+' - '+s.SWName  as work,

  cast(w.AaAmt as decimal(18,2)) as AAAMT
 ,convert(varchar, w.AADate,103) as AADate,
   cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,103) as TSDate 
 ,n.AcceptanceLetterRefNo ,convert(varchar, n.AcceptLetterDT,103) as AcceptLetterDT ,
    cast((n.PAC) as decimal(18,2)) as PAC


,case when n.formt='B' then Round(cast(Round(n.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(n.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

    ,cast(n.SanctionRate as decimal(18,2)) as SanctionRate
,n.SanctionDetail,
 cast(n.TimeAllowed as bigint) as TimeAllowed,
 n.RainySeason  ,convert(varchar, n.DateOfSanction,103) as DateOfSanction,
 convert(varchar, n.DateOfIssueNIT,103) as DateOfIssueNIT,     
cnt.Contractorid as CID,isnull(cnt.Title,'')+cnt.EnglishName as ContractorNAme,cnt.RegType,cnt.Class,cnt.EnglishAddress,cnt.MobNo 
,w.ASPath,w.ASLetter 
,GroupName,PGroupID,pl.LProgress,convert(varchar, pl.ProgressDT,103) as Pdate,pl.PRemarks,pl.Remarks,pl.RID, n.Divisionid
,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference
,pl.DelayReason DelayReason
from WorkMaster w

inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
    inner join Districts di on di.DISTRICT_ID= w.worklocationdist_id
    left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=di.District_ID
  inner join  dhrsHealthCenter d on d.HC_ID= w.worklocation_id
inner join Division dv on dv.Div_Id=di.Div_Id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')  
 and agd.DivisionID not in ('D1032')
inner join Login ap on ap.Login_id= w.approved_by
inner join WorkType wt on wt.type_id= w.work_type

inner join SWDetails s on s.SWId= w.work_description_id
inner join AgreementDetails  n on n.work_id= w.work_id
 left outer join MasTender t on t.tenderid= n.tenderid

inner join ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))

inner join 
(
select p.ppid, p.WorkLevel as level_id ,
isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,
p.Work_id
,r.RID,p.Remarkid,r.Remarks,p.DelayReason
,wpg.GroupName,wpg.PGroupID
from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid

inner join WorkLevelParentGroup wpg on wpg.PGroupID= wpp.PGroupID
left outer join WorkProgressLevel_Remarks r on r.RID= p.remarkid
where NewProgress = 'Y'   and wpp.PPID=4 
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID,p.DelayReason
) pl on pl.Work_id=w.work_id
where 1=1 and w.isdeleted is null  and w.MainSchemeID not in (121)   and n.AcceptLetterDT is not null
and n.AcceptLetterDT>'01-01-2022'
 and n.WrokOrderDT is null and n.workorderrefnogovt is  null " + whereClause + @"
 order by   n.AcceptLetterDT  desc ";
            
            return await _context.WorkorderpendingdetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }


        [HttpGet("getWorkGenDetails")]

        public async Task<ActionResult<IEnumerable<WOGeneratedDetailsDTO>>> getWorkGenDetails(string divisionId, string mainSchemeId, string distid, string work_id, string fromdt, string todt)
        {
            string? whereClause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and di.District_ID= '{distid}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            if (work_id != "0")
            {

                whereClause += $" and w.work_id = '{work_id}'";
            }

            if (fromdt != "0")
            {
                if (todt != "0")
                {
                    whereClause += $" and n.WrokOrderDT between   '{fromdt}' and '{todt}' ";

                }
                else
                {
                    whereClause += $" and n.WrokOrderDT between   '{fromdt}' and getdate() ";
                }


            }

            string query = "";

            query = $@"select w.LetterNo, w.work_id, msc.Name as Head,ap.login_name as Approver,
 wt.type_name ,
  di.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  d.NAME_ENG+' - '+s.SWName  as work,

  cast(w.AaAmt as decimal(18,2)) as AAAMT
 ,convert(varchar, w.AADate,103) as AADate,
   cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,103) as TSDate 
 ,n.AcceptanceLetterRefNo ,convert(varchar, n.AcceptLetterDT,103) as AcceptLetterDT ,
    cast((n.PAC) as decimal(18,2)) as PAC


,case when n.formt='B' then Round(cast(Round(n.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(n.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

    ,cast(n.SanctionRate as decimal(18,2)) as SanctionRate
,n.SanctionDetail,
 cast(n.TimeAllowed as bigint) as TimeAllowed,
 n.RainySeason  ,convert(varchar, n.DateOfSanction,103) as DateOfSanction,
 convert(varchar, n.DateOfIssueNIT,103) as DateOfIssueNIT,     
cnt.Contractorid as CID,isnull(cnt.Title,'')+cnt.EnglishName as ContractorNAme,cnt.RegType,cnt.Class,cnt.EnglishAddress,cnt.MobNo 
,w.ASPath,w.ASLetter 
,GroupName,PGroupID,pl.LProgress,convert(varchar, pl.ProgressDT,103) as Pdate,pl.PRemarks,pl.Remarks,pl.RID, n.Divisionid
,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference
,pl.DelayReason DelayReason
,convert(varchar,n.WrokOrderDT,103) as WrokOrderDT ,case when n.HOAllotedDT is null then '-' else convert(varchar,n.HOAllotedDT,103) end as HOAllotedDT ,n.AgreementRefNo
 ,isnull(n.WorkorderRefNoGovt,'') as WorkorderRefNoGovt,CONVERT(varchar,case when n.TimeAllowed>6 then DATEADD(Month, n.TimeAllowed+1, n.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, n.TimeAllowed, n.WrokOrderDT))  end,105) DueDTTimePerAdded

from WorkMaster w

inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
    inner join Districts di on di.DISTRICT_ID= w.worklocationdist_id
    left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=di.District_ID
  inner join  dhrsHealthCenter d on d.HC_ID= w.worklocation_id
inner join Division dv on dv.Div_Id=di.Div_Id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')  
 and agd.DivisionID not in ('D1032')
inner join Login ap on ap.Login_id= w.approved_by
inner join WorkType wt on wt.type_id= w.work_type

inner join SWDetails s on s.SWId= w.work_description_id
inner join AgreementDetails  n on n.work_id= w.work_id
 left outer join MasTender t on t.tenderid= n.tenderid

inner join ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))

inner join 
(
select p.ppid, p.WorkLevel as level_id ,
isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,
p.Work_id
,r.RID,p.Remarkid,r.Remarks,p.DelayReason
,wpg.GroupName,wpg.PGroupID
from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid

inner join WorkLevelParentGroup wpg on wpg.PGroupID= wpp.PGroupID
left outer join WorkProgressLevel_Remarks r on r.RID= p.remarkid
where NewProgress = 'Y' and wpp.ppid=6
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID,p.DelayReason
) pl on pl.Work_id=w.work_id
where 1=1 and w.isdeleted is null  and w.MainSchemeID not in (121)   and n.AcceptLetterDT is not null
 and n.WrokOrderDT is not null and n.workorderrefnogovt is not  null " + whereClause + @"
 order by   n.AcceptLetterDT  desc ";

            return await _context.WOGeneratedDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }

  



    }
}

