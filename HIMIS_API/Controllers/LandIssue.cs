using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LandIssue : Controller
    {
        private readonly DbContextData _context;
        public LandIssue(DbContextData context)
        {
            _context = context;
        }
        [HttpGet("LIPendingTotal")]
        public async Task<ActionResult<IEnumerable<LandIssueSummaryDTO>>> LIPendingTotal(string RPType,string divisionid,string districtid,string MainSchemeID)
        {
            string? whereClause = "";
            if (MainSchemeID != "0")
            {

                whereClause += $" and msc.MainSchemeID  = '{MainSchemeID}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }

            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            string query = "";
            if (RPType == "Total")
            {
                query = $@" select DivisionID as ID,DivName_En as Name,count(work_id) as TotalWorks,sum(WOIssued) as WOIssued, cast(SUM(v)/100 as decimal(18,2)) as valuecr,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
,sum(Month2Above) as Month2Above

from 
(

select w.work_id,dv.DivName_En,agd.DivisionID,case when Datediff(day,ProgressDT,GETDATE() ) >60 then 1 else 0 end as Month2Above
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,case when a.TotalAmountOfContract is not null then
(case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end)
else 
case when a.TotalAmountOfContract is null and  w.tsamount is not null then w.tsamount 
else  w.aaamt end end  v
, case when  a.wrokorderdt is not null and a.workorderrefnogovt is not null then 1 else 0 end as WOIssued
from  WorkMaster w
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
left outer join  AgreementDetails a on a.work_id=w.work_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'  and r.RID in (1,15)
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID=pw.remarkid
where  w.isdeleted is null  and w.MainSchemeID not in (121)
and r.RID in (1,15)  " + whereClause + @"
) b
group by DivName_En,DivisionID
order by  DivName_En  ";
            }
            if (RPType == "Scheme")
            {
                //Scheme
                query = $@" select cast(MainSchemeID as varchar) as ID,name  as Name,
count(work_id) as TotalWorks,
sum(WOIssued) as WOIssued,cast(SUM(v)/100 as decimal(18,2)) as valuecr, cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
,sum(Month2Above) as Month2Above


from 
(

select w.work_id,msc.name,w.MainSchemeID ,case when Datediff(day,ProgressDT,GETDATE() ) >60 then 1 else 0 end as Month2Above
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,case when a.TotalAmountOfContract is not null then
(case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end)
else 
case when a.TotalAmountOfContract is null and  w.tsamount is not null then w.tsamount 
else  w.aaamt end end  v
, case when  a.wrokorderdt is not null and a.workorderrefnogovt is not null then 1 else 0 end as WOIssued
from  WorkMaster w
left outer join  AgreementDetails a on a.work_id=w.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'  and r.RID in (1,15) 
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID=pw.remarkid
where  w.isdeleted is null  and w.MainSchemeID not in (121)
and r.RID in (1,15)   " + whereClause + @"
) b
group by name,MainSchemeID
order by  count(work_id) desc ";
            }
            

            if (RPType == "District")
            {
               //districtwise
                query = $@" select cast(District_ID as varchar) as ID,dbstart_name_en as Name,
count(work_id) as TotalWorks,sum(WOIssued) as WOIssued,cast(SUM(v)/100 as decimal(18,2)) as valuecr
, cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
,sum(Month2Above) as Month2Above
from 
(

select w.work_id,d.dbstart_name_en,d.District_ID,case when Datediff(day,ProgressDT,GETDATE() ) >60 then 1 else 0 end as Month2Above
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,case when a.TotalAmountOfContract is not null then
(case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end)
else 
case when a.TotalAmountOfContract is null and  w.tsamount is not null then w.tsamount 
else  w.aaamt end end  v
, case when  a.wrokorderdt is not null and a.workorderrefnogovt is not null then 1 else 0 end as WOIssued
from  WorkMaster w
left outer join  AgreementDetails a on a.work_id=w.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'  and r.RID in (1,15)  
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID=pw.remarkid
where  w.isdeleted is null  and w.MainSchemeID not in (121)
and r.RID in (1,15)   " + whereClause + @"
) b
group by District_ID,dbstart_name_en
order by  count(work_id) desc ";
            }

            return await _context.LandIssueSummaryDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getLandIssueDetails")]

        public async Task<ActionResult<IEnumerable<WorkorderpendingdetailsDTO>>> getLandIssueDetails(string divisionId, string mainSchemeId, string distid)
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
          
            string query = "";

            query = $@" select w.LetterNo, w.work_id, msc.Name as Head,ap.login_name as Approver,w.GrantNo as grantNo,convert(varchar,n.WrokOrderDT,103) as WrokOrderDT

 ,isnull(totalpaid,0) as Totalpaid
 ,isnull(GrossUnpaid,0) as Totalunpaid,
 dv.DivName_En, rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname
,rtrim(ltrim(isnull(ae.Name,'Not Alloted'))) as AEName,n.AgreementRefNo,n.WorkorderRefNoGovt,



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
,GroupName,PGroupID,pl.LProgress,convert(varchar, pl.ProgressDT,103) as Pdate,pl.PRemarks,pl.Remarks

,pl.RID, n.Divisionid
,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference,
pl.DelayReason DelayReason
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
left outer join AgreementDetails  n on n.work_id= w.work_id
 left outer join MasTender t on t.tenderid= n.tenderid

left outer join  ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))

inner join 
(
select p.ppid, p.WorkLevel as level_id ,
isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,
p.Work_id
,r.RID,p.Remarkid,r.Remarks
,wpg.GroupName,wpg.PGroupID,p.DelayReason
from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid

inner join WorkLevelParentGroup wpg on wpg.PGroupID= wpp.PGroupID
inner join  WorkProgressLevel_Remarks r on r.RID= p.remarkid

where NewProgress = 'Y' and r.RID in (1,15)
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID,p.DelayReason
) pl on pl.Work_id=w.work_id

left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id

 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode

left outer join 
(
select work_code,sum(cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2))) GrossUnpaid
 from BillPayment 
where isnull(Ispaid,'N')='N' and isnull(ispass,'N') ='N' and ChequeNo is  null 
group by work_code
) btu on btu.work_code=w.work_id


where 1=1 and w.isdeleted is null  and w.MainSchemeID not in (121)    " + whereClause + @"
 order by pl.LProgress desc ";
            
            return await _context.WorkorderpendingdetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }




        }
}

