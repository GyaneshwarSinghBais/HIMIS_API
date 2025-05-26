using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.Handover;
using HIMIS_API.Models.WorkOrder;
using HIMIS_API.Services.ProgressServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using System;
using System.Security.Cryptography;


namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        private readonly DbContextData _context;
        public WorkController(DbContextData context)
        {
            _context = context;
        }

        [HttpGet("getDashLoginDDL")]

        public async Task<ActionResult<IEnumerable<DashLoginDDLDTO>>> getDashLoginDDL()
        {
            string query = "";

            query = $@" select ID,Desig,Mobile,Rankid,Role
from 
(
select a.DivisionID as ID, d.DivName_En  Desig ,DivMobile as Mobile ,2 as Rankid  ,'Division' as Role
from  AgencyDivisionMaster a inner join Division d on d.Div_Id=a.DivisionName 
where left(divisionid,1)='D'  and a.DivisionID not in ('D1032')
union all
select cast(AgencyID as varchar) as ID,'Superintending Engineer' as Desig,Mobileno as Mobile,1 RankID,'SE' as Role from AgencyMaster where  AgencyID='1001'
) b 
order by b.Rankid";

            return await _context.DashLoginDDLDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }
        [HttpGet("DistrictNameDME")]

        public async Task<ActionResult<IEnumerable<DistrictNameDTO>>> DistrictNameDME( string divisionid,string districtid)
        {
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {
                whereClause += $"  and d.District_ID= '{districtid}'";

            }
            string query = @"  SELECT distinct District_ID, DBStart_Name_En as DistrictName, 0 as Div_Id FROM Districts d
 inner join WorkMaster w on w.worklocationdist_id=d.District_ID
inner join Division dv on dv.Div_Id=d.Div_Id		
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
           WHERE 1 = 1 and w.approved_by=16
	       " + whereClause + @"
           ORDER BY DBStart_Name_En";
            return await _context.DistrictNameDbSet
               .FromSqlRaw(query)
               .ToListAsync();
        }


        [HttpGet("DMEProgressSummary")]

        public async Task<ActionResult<IEnumerable<DMEProgressCountDTO>>> DMEProgressSummary(string divisionId, string mainSchemeId, string distid, string dashID)
        {
            string? whereClause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            string hvclause = "";
            if (dashID != "0")
            {
                if (dashID == "6001")
                {
                    hvclause += $"  having sum(LandIssue6001)>0";
                }
                if (dashID == "2001")
                {
                    hvclause += $"  having sum(TenderProcess2001)>0";
                }
                if (dashID == "3001")
                {
                    hvclause += $"  having sum(AccWorkOrder3001)>0";
                }
                if (dashID == "4001")
                {
                    hvclause += $"  having sum(Completed4001)>0";
                }
                if (dashID == "5001")
                {
                    hvclause += $"  having sum(Running5001)>0";
                }
                if (dashID == "8001")
                {
                    hvclause += $"  having sum(RetunDept8001)>0";
                }
                if (dashID == "1001")
                {
                    hvclause += $"  having sum(ToBeTender1001)>0";
                }

            }

            string query = "";

            query = $@" select hc_id,NAme_eng,d.District_ID,d.DBStart_Name_En as Districtname,cast(sum(ToBeTender1001) as int) as ToBeTender1001,
cast(sum(TenderProcess2001)as int)  as TenderProcess2001,cast(sum(AccWorkOrder3001)as int)  as AccWorkOrder3001,cast(sum(Completed4001)as int)  as Completed4001,cast(sum(Running5001)as int)  as Running5001,cast(sum(LandIssue6001)as int)  as LandIssue6001,cast(sum(RetunDept8001)as int)  as RetunDept8001
,cast((sum(ToBeTender1001)+sum(TenderProcess2001)+sum(AccWorkOrder3001)+sum(Completed4001)+sum(Running5001)+sum(LandIssue6001)+sum(RetunDept8001)) as int) as Total
,DivisionID
 from Districts d 
 left outer join 
(
select District_ID, d.did,a.drid, d.dashname,
count(a.work_id)as nosworks 
,case when d.did=1001 and count(a.work_id)>0 then count(a.work_id) else 0 end as ToBeTender1001
,case when d.did=2001 and count(a.work_id)>0 then count(a.work_id) else 0 end as TenderProcess2001
,case when d.did=3001 and count(a.work_id)>0 then count(a.work_id) else 0 end as AccWorkOrder3001
,case when d.did=4001 and count(a.work_id)>0 then count(a.work_id) else 0 end as Completed4001
,case when d.did=5001 and count(a.work_id)>0 then count(a.work_id) else 0 end as Running5001
,case when d.did=6001 and count(a.work_id)>0 then count(a.work_id) else 0 end as LandIssue6001
,case when d.did=8001 and count(a.work_id)>0 then count(a.work_id) else 0 end as RetunDept8001
,DivisionID,HC_ID,NAme_eng
from WorkLevelParentDash d

left outer join 
(
select msc.Name as Head,w.work_id, h.NAME_ENG+' - '+s.SWName  as workname,h.NAme_eng,h.HC_ID,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,

GroupName,PGroupID,pl.LProgress,pl.ProgressDT,pl.PRemarks,pl.Remarks,pl.RID,

d.District_ID,agd.DivisionID,msc.MainSchemeID 
,did,DashName, case when pl.RID is not null and pl.RID in (1,15)  then  6001  else did   end as drid,
case when pl.RID is not null  and pl.RID in (1,15)  then 'Land Not Alloted/Land Dispute' else DashName end as display

from  WorkMaster w
inner join SWDetails s on s.SWId=w.work_description_id 
inner join dhrsHealthCenter  h on  h.HC_ID  =w.worklocation_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

inner join 
(
select p.ppid,p.WorkLevel as level_id ,
isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,
p.Work_id
,r.RID,p.Remarkid,r.Remarks
,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName
from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid

inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y' and dash.did not in (7001)
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID
 ,dash.did,dash.DashName
) pl on pl.Work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121)  and w.approved_by=16
" + whereClause + @"
) a  on a.drid=d.did
where  d.did not in (7001) and DivisionID is not null
  group by a.drid, d.dashname,d.did,District_ID,DivisionID,HC_ID,NAme_eng
  ) b on b.District_ID=d.District_ID
  where 1=1 and DivisionID is not null 
  group by d.District_ID,d.DBStart_Name_En,DivisionID, hc_id,NAme_eng
 " + hvclause + @"
  order by DivisionID,d.District_ID,hc_id ";



            return await _context.DMEProgressCountDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("WorkFill")]

        public async Task<ActionResult<IEnumerable<FillWorkDTO>>> WorkFill(string searchtext, string workid, string divisionId, string distid,string mainSchemeId)
        {
         

           string  orderbywh = " order by w.Work_id desc";
            string? whereClause = "";
            string? downwhclause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";
                downwhclause+= $" and DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";
                downwhclause += $" and district_id  = '{distid}'";
            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
                downwhclause += $" and mainschemeid  = '{mainSchemeId}'";
            }


     
            if (searchtext != "0")
            {

                whereClause += $"  and (w.Work_id+''+msc.Name +'-'+d.DBStart_Name_En +'-'+ h.NAME_ENG+' - '+s.SWName+'-'+isnull(a.tenderreference,'') ) like  '%{searchtext}%'";
                downwhclause += $" and searchtext   like  '%{searchtext}%'";

            }
            if (workid != "0")
            {
                whereClause += $"  and w.Work_id= '{workid}'";
                downwhclause += $" and Work_id  = '{workid}'";

            }
            string query = "";

//            query1 = $@" select w.Work_id,msc.Name as Head,d.DBStart_Name_En as District,
//h.NAME_ENG+' - '+s.SWName  as workname,
//CONVERT(varchar,w.AADate,105) as AADT
//,h.NAME_ENG+' - '+isnull(s.SWName,'')+'-'+w.Work_id+'-'+msc.Name +'-'+d.DBStart_Name_En +'-'+CONVERT(varchar,w.AADate,105)+'-'+isnull(a.tenderreference,'')   as searchname

// from WorkMaster w

//left outer join  AgreementDetails a on a.work_id=w.work_id
//inner join SWDetails s on s.SWId=w.work_description_id 
//inner join dhrsHealthCenter  h on  h.HC_ID  =w.worklocation_id
//inner join Districts d on d.District_ID  =w.worklocationdist_id
//inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
//inner join division dv on dv.div_id=d.div_id
//inner join  agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
// where 1=1  and w.isdeleted is null  and w.MainSchemeID not in (121)    " + whereClause + @"  
//" + orderbywh;
    

                 query = @"  select w.Work_id
,h.NAME_ENG + ' - ' + isnull(s.SWName, '') + '-' + w.Work_id + '-' + msc.Name + '-' + d.DBStart_Name_En + '-' + CONVERT(varchar, w.AADate, 105) + '-' + isnull(a.tenderreference, '') as searchname
,w.MainSchemeID as mainschemeid,d.District_ID as district_id, agd.DivisionID as divisionid
 from WorkMaster w
left outer join  AgreementDetails a on a.work_id = w.work_id
inner join SWDetails s on s.SWId = w.work_description_id
inner join dhrsHealthCenter h on h.HC_ID = w.worklocation_id
inner join Districts d on d.District_ID = w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id = d.div_id
inner join  agencydivisionmaster agd on cast(agd.divisionname as bigint)= cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')

 where 1 = 1  and w.isdeleted is null and w.MainSchemeID not in (121)  " + whereClause + @"
and w.work_id not in (Select work_id from WorkMasterSearch)
 union all
 select work_id, searchtext as searchname, mainschemeid, district_id, divisionid from WorkMasterSearch
 where 1 = 1 " + downwhclause + @"

 order by w.Work_id ";

            return await _context.FillWorkDTODbSet
             .FromSqlRaw(query)
            .ToListAsync();
        }

        [HttpGet("GetProjectTimeline")]

        public async Task<ActionResult<IEnumerable<ProjectTimeDTO>>> GetProjectTimeline(string workid)
        {
           string  query = $@"  select 1 as PPId,'Administrative Sanction' as  Level, convert(varchar,w.AADate, 105) as Pdate ,1 SinceAS, 1 SinceLastProg from WorkMaster w  where work_id='"+ workid + @"'
union all
select 2 as PPId,'Technical Sanction' as  ASLevel, convert(varchar,w.TSDate, 105) as TSDate,datediff(dd,AADate,TSDate) as SinceAS,datediff(dd,AADate,TSDate) as SinceLastProg  from WorkMaster w  where work_id='"+ workid + @"'
union all
select 3 as PPId,'Tender Issued' as  Tender,convert(varchar,a.DateOfIssueNIT, 105) as TenderDate,datediff(dd,AADate,a.DateOfIssueNIT) as SinceAS,datediff(dd,TSDate,a.DateOfIssueNIT) as SinceLastProg   from AgreementDetails a 
inner join WorkMaster w on w.work_id=a.work_id
 where a.work_id='" + workid + @"'
union all
select 6 as PPId,'Tender Acceptance' as  Acceptance,convert(varchar,a.acceptletterdt, 105) as Acceptance,datediff(dd,AADate,a.acceptletterdt) as SinceAS,datediff(dd,a.DateOfIssueNIT,a.acceptletterdt) as SinceLastProg   from AgreementDetails a  
inner join WorkMaster w on w.work_id=a.work_id
 where a.work_id='"+ workid + @"'

union all
select 7 as PPId,'Work Order' as  workorder,convert(varchar,a.wrokorderdt, 105) as workorderDT,datediff(dd,AADate,a.wrokorderdt) as SinceAS,datediff(dd,a.acceptletterdt,a.wrokorderdt) as SinceLastProg   from AgreementDetails a
inner join WorkMaster w on w.work_id=a.work_id
 where a.work_id='"+ workid + @"'

union all
select p.ppid,wpp.parentprogress,convert(varchar,max(ProgressDT),105) as Pdate,datediff(dd,AADate,max(ProgressDT)) as SinceAS,datediff(dd,a.wrokorderdt,max(ProgressDT)) as SinceLastProg
 from WorkPhysicalProgress p
inner join AgreementDetails a on a.work_id=p.Work_id
inner join WorkMaster w on w.work_id=p.work_id
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
where 1=1 and wpg.PGroupID not in (1,2,3,4,5,6,7,8) and  p.work_id='"+ workid + @"'
group by p.ppid,wpp.parentprogress,w.AADate,a.wrokorderdt
order by ppid ";
            
                    return await _context.ProjectTimeDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }


        [HttpGet("GetProjectTimelineNew")]

        public async Task<ActionResult<IEnumerable<ProjectTimelinePyramidDTO>>> GetProjectTimelineNew(string workid)
        {
            string query = $@"  select PPId,Level,DateProgress
from 
(
 select 1 as PPId,'Admiministrative Santioned' as  Level, convert(varchar,w.AADate, 105) as DateProgress ,0 as sr,w.AADate as DT from WorkMaster w  where work_id='"+ workid + @"'
union all
select 2 as PPId,'Technical Sanction' as  Level, convert(varchar,w.TSDate, 105) as ProgressDT, 0 as sr,w.TSDate as DT from WorkMaster w  where work_id='"+ workid + @"'

union all
select 3 as PPId,'Tender Issued' as  Tender,convert(varchar,a.DateOfIssueNIT, 105) as TenderDate,datediff(dd,AADate,a.DateOfIssueNIT) as SinceAS,datediff(dd,TSDate,a.DateOfIssueNIT) as SinceLastProg   
from AgreementDetails a 
inner join WorkMaster w on w.work_id=a.work_id
 where a.work_id='"+ workid + @"'

union all
select 6 as PPId,'Tender Acceptance' as  Acceptance,convert(varchar,a.acceptletterdt, 105) as Acceptance,datediff(dd,AADate,a.acceptletterdt) as SinceAS,datediff(dd,a.DateOfIssueNIT,a.acceptletterdt) as SinceLastProg   from AgreementDetails a  
inner join WorkMaster w on w.work_id=a.work_id
 where a.work_id='"+ workid + @"'

union all
select 7 as PPId,'Work Order' as  workorder,convert(varchar,a.wrokorderdt, 105) as workorderDT,datediff(dd,AADate,a.wrokorderdt) as SinceAS,datediff(dd,a.acceptletterdt,a.wrokorderdt) as SinceLastProg   from AgreementDetails a
inner join WorkMaster w on w.work_id=a.work_id
 where a.work_id='"+ workid + @"'

union all
select p.PPID,p.ParentProgress,convert(varchar,wpt.ProgressDT,105) as ProgressDT  ,wpp.sr,wpt.ProgressDT as DT 
from WorkLevelParent  p
inner join
(
select max(sr) as sr, wpp.PPID,wpp.Work_id from WorkPhysicalProgress wpp
inner join  WorkLevelParent wp on wp.PPID=wpp.PPID
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where dash.did  in (5001)
group by wpp.PPID,wpp.Work_id
) wpp on wpp.PPID=p.PPID
inner join WorkPhysicalProgress wpt on wpt.SR=wpp.sr
inner join WorkMaster w on w.work_id=wpp.work_id
inner join WorkLevelParentGroup wpg on wpg.PGroupID=p.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where dash.did  in (5001)
and w.work_id='"+ workid + @"'
) a order by PPId desc ";

            return await _context.ProjectTimelinePyramidDbSet
    .FromSqlRaw(query)
    .ToListAsync();
        }





        [HttpGet("GetWorkInfo")]

        public async Task<ActionResult<IEnumerable<WorkInfoDTO>>> GetWorkInfo(string workid)
        {
            string query = "";

            query = $@" select w.LetterNo, w.work_id, msc.Name as Head,
 rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname,rtrim(ltrim(isnull(e.Designation,'Not Alloted'))) as desig,
 isnull(g.GName,'-') as grantname,ap.login_name as Approver,
 wt.type_name ,div.DivName_En,
  di.DBStart_Name_En,isnull(b.Block_Name_En,'') as blockname,
  d.NAME_ENG+' - '+s.SWName  as work ,cast(cast(w.AaAmt as decimal(18,2)) as varchar) as AAAMT,convert(varchar, w.AADate,103) as AADate,cast(cast(w.TSAmount as decimal(18,2)) as varchar) as TSAMT,convert(varchar, w.TSDate,103) as TSDate 
 ,n.AgreementNo,convert(varchar, n.WrokOrderDT,103) as WrokOrderDT ,n.YearofAgreement
 ,Convert(varchar, n.DueDatecompletion,105) as ActualDueDT
 ,CONVERT(varchar,case when n.TimeAllowed>6 then DATEADD(Month, n.TimeAllowed+1, n.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, n.TimeAllowed, n.WrokOrderDT))  end,105) DueDTTimePerAdded,
 n.AcceptanceLetterRefNo ,convert(varchar, n.AcceptLetterDT,103) as AcceptLetterDT 
 ,cast(n.PAC as decimal(18,2)) as PAC 

,case when n.formt='B' then Round(cast(Round(n.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(n.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract
,n.SanctionRate,n.SanctionDetail,
 cast(n.TimeAllowed as bigint) as TimeAllowed,n.AgreementRefNo
 ,isnull(n.WorkorderRefNoGovt,'') as WorkorderRefNoGovt,
 n.RainySeason ,n.SDRequired as PGReq ,convert(varchar, n.DateOfSanction,103) as DateOfSanction,
 convert(varchar, n.DateOfIssueNIT,103) as DateOfIssueNIT,     
cnt.Contractorid as CID,isnull(cnt.Title,'')+rtrim(ltrim(cnt.EnglishName)) as ContractorNAme,cnt.RegType,cnt.Class,cnt.EnglishAddress,cnt.MobNo 
,w.ASPath,w.ASLetter 
,GroupName,PGroupID,pl.LProgress,convert(varchar, pl.ProgressDT,103) as Pdate,pl.PRemarks,pl.Remarks,pl.RID, n.Divisionid
,did,DashName
,isnull(totalExp,0)+isnull(gstcharge,0) as Expd
,isnull(fbill,'No') as FBill
,isnull(grosspending,0)+isnull(gstpending,0) as ExpPending,
case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference
,pl.SR,ImageName,ImageName2,ImageName3,ImageName4,ImageName5,ismongo,ProgressEnterby,ProgressEntryTime,NonMongoImage
,rtrim(ltrim(plDR.delayreason)) as delayreason
,case when plEC.expcompdt is not null then convert(varchar,plEC.expcompdt,105)  else '' end as expCompDT ,w.GrantNo
from WorkMaster w
inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
    inner join Districts di on di.DISTRICT_ID= w.worklocationdist_id

    left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=di.District_ID
      left outer join  dhrsHealthCenter d on d.HC_ID= w.worklocation_id
    left outer join Schemes sc on sc.SchemeID= w.SchemeID
 inner join Division div on div.Div_Id= di.Div_Id
  left outer join Login ap on ap.Login_id= w.approved_by
 left outer join WorkType wt on wt.type_id= w.work_type
  left outer join GrantMaster g on g.GrantID= w.GrantID
 left outer join SWDetails s on s.SWId= w.work_description_id
 left outer join AgreementDetails  n on n.work_id= w.work_id
 left outer join MasTender t on t.tenderid= n.tenderid
 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
 left outer join ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))

 inner join
(
select p.ppid, p.WorkLevel as level_id ,
isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,
p.Work_id
,r.RID,p.Remarkid,r.Remarks
,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName,isnull(p.ismongo,'N') as ismongo
,DelayReason,Max(expCompDT) as expCompDT, isnull(p.Remarks,'' )  as PRemarks
,case when p.ismongo='Y' then p.ImageName else 'NA' end as ImageName
,case when p.ismongo='Y' then p.ImageName2 else 'NA' end as ImageName2
,case when p.ismongo='Y' then p.ImageName3 else 'NA' end as ImageName3
,case when p.ismongo='Y' then p.ImageName4 else 'NA' end as ImageName4
,case when p.ismongo='Y' then p.ImageName5 else 'NA' end as ImageName5
,p.SR,p.UpdateEmpName as ProgressEnterby,p.EntryDT as ProgressEntryTime,p.ImageData as NonMongoImage
from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid

inner join WorkLevelParentGroup wpg on wpg.PGroupID= wpp.PGroupID
left outer join WorkLevelParentDash dash on dash.did= wpg.did
left outer join WorkProgressLevel_Remarks r on r.RID= p.remarkid
where 1=1 and NewProgress = 'Y' and dash.did not in (7001)
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID
 ,dash.did,dash.DashName,p.ImageName,p.ImageName2,p.ImageName3,p.ImageName4,p.ImageName5,p.SR,p.IsMongo,p.UpdateEmpName,p.EntryDT,p.ImageData
,p.DelayReason
) pl on pl.Work_id=w.work_id
left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal (18,2)) as totalExp

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal (18,2)) gstcharge
from BillPayment b
where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
group by work_code
) ex on ex.work_code=w.work_id
left outer join
(
select work_code,case when bp.AgrBillStatus= 'Final' then 'Yes' else 'No' end as fbill from BillPayment bp
where bp.AgrBillStatus= 'Final' and isnull(bp.ispass,'N') ='P'
group by work_code,bp.AgrBillStatus
) fi on fi.work_code=w.work_id

left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal (18,2)) as grosspending 

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal (18,2)) gstpending
from BillPayment b
where isnull(ispass,'N') not in ('P')  and b.ChequeDT > '01-Apr-2021' 
group by work_code
) expen on expen.work_code=w.work_id

left outer join 
(
select drs.sr,drs.work_id,wpdrs.delayreason
from 
(
select max(sr) sr ,work_id from WorkPhysicalProgress
where delayreason is not null
group by work_id
) drs
inner join WorkPhysicalProgress wpdrs on wpdrs.sr=drs.sr
) plDR on plDR.Work_id=w.work_id

left outer join 
(
select ecd.sr,ecd.work_id,ecwp.expcompdt
from 
(
select max(sr) sr ,work_id from WorkPhysicalProgress
where expcompdt is not null
group by work_id
) ecd
inner join WorkPhysicalProgress ecwp on ecwp.sr=ecd.sr
) plEC on plEC.Work_id=w.work_id

where 1=1 and w.isdeleted is null  and w.MainSchemeID not in (121)    and w.work_id ='" + workid + @"'            ";
            return await _context.WorkInfoDBset
            .FromSqlRaw(query)
            .ToListAsync();
        }


        [HttpGet("getProgressCount")]

        public async Task<ActionResult<IEnumerable<ProgressCountDivisionDTO>>> getProgressCount(string did, string divisionId, string distid, string mainSchemeId)
        {
            string query = "";

            //5001 --> Running
            //4001 --> Handover

            string? whereClause = "";
            string didclause = "";
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and   w.MainSchemeID  = {mainSchemeId}";
            }
            if (did != "0")
            {

                didclause += $" and    dash.did    = {did}";
            }

            query = $@" select DivisionID,DivName_En as DivisionName,count(work_id) as nosworks,
sum(TotalToday) as TotalToday,
sum(Mobiletoday) as Mobiletoday,
sum(MobileInLast7Days) as TotalInLast7Days,
sum(MobileInLast7Days) as MobileInLast7Days,
sum(MobileInLast7Days) as TotalInLast15Days, 
sum(MobileLast15Days) as MobileLast15Days
,sum(TotalBefore15Days) as TotalBefore15Days
,sum(MobileBefore15Days) as MobileBefore15Days
from 
(
select w.work_id,lt.Device,case when lt.Device='Mobile' then 1 else 0 end as appProgress
,pw.EntryDT, case when w.proglat is not null then w.proglat else pw.Latitude end as Latitude
, case when w.ProgLong is not null then w.ProgLong else pw.Longitude end as Longitude
,case when convert(varchar,pw.EntryDT,103) = convert(varchar,GETDATE(),103)   then 1 else 0 end as TotalToday
,case when lt.Device='Mobile' and convert(varchar,pw.EntryDT,103) = convert(varchar,GETDATE(),103)   then 1 else 0 end as Mobiletoday
,case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())<=7
then 1 else 0 end as TotalInLast7Days
,case when lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())<=7
then 1 else 0 end as MobileInLast7Days,

case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>7 and datediff(d,pw.EntryDT,GETDATE())<=15
then 1 else 0 end as TotalInLast15Days

,case when lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>7 and datediff(d,pw.EntryDT,GETDATE())<=15
then 1 else 0 end as MobileLast15Days

,case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>15
then 1 else 0 end as TotalBefore15Days

,case when  lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>15
then 1 else 0 end as MobileBefore15Days
,dv.DivName_En,agd.DivisionID
from  WorkMaster w
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) 
and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y' " + didclause + @"
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid


left outer join 
(
select Work_id as LTWorkid,p.Latitude,p.Longitude,p.Device  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where  1=1  " + didclause + @"
and Device='Mobile' and NewProgress='Y'
) lt on lt.LTWorkid=w.work_id 
where  w.isdeleted is null  and w.MainSchemeID not in (121) " + whereClause + @"
) b group by DivName_En,DivisionID order by DivName_En";


            return await _context.ProgressCountDivisionDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getProgressDetailsLatLong")]

        public async Task<ActionResult<IEnumerable<ProgressCountDetailsDTO>>> getProgressDetailsLatLong(string did, string divisionId, string distid, string mainSchemeId, string workid, string TotMobile, string dayPara)
        {
            string? whereClause = "";
            string didclause = "";
            if (workid != "0")
            {

                whereClause += $" and w.work_id  = '{workid}'";

            }
            if (divisionId != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionId}'";

            }
            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            if (did != "0")
            {

                didclause += $" and    dash.did    = {did}";
            }
            string whdaypara = "";
            if (TotMobile == "Total")
            {
                if (dayPara == "1")
                {
                    whdaypara += " and (case when convert(varchar,pw.EntryDT,103) = convert(varchar,GETDATE(),103)   then 1 else 0 end )= 1";
                }
                else if (dayPara == "7")
                {
                    whdaypara += " and (case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103)  and datediff(d,pw.EntryDT,GETDATE())<=7 then 1 else 0 end )= 1";

                }
                else if (dayPara == "15")
                {
                    whdaypara += " and (case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103)  and datediff(d,pw.EntryDT,GETDATE())<=7 then 1 else 0 end )= 1";
                }
                else
                {
                    whdaypara += " and ( case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) and datediff(d,pw.EntryDT,GETDATE())>15 then 1 else 0 end )= 1";
                }



            }
            if (TotMobile == "Mobile")
            {

                if (dayPara == "1")
                {
                    whdaypara += " and (case when lt.Device = 'Mobile' and convert(varchar, pw.EntryDT,103) = convert(varchar, GETDATE(), 103)   then 1 else 0 end)= 1";
                }
                else if (dayPara == "7")
                {
                    whdaypara += " and (case when lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) and datediff(d,pw.EntryDT,GETDATE())<=7 then 1 else 0 end)= 1";

                }
                else if (dayPara == "15")
                {
                    whdaypara += " and (case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) and datediff(d,pw.EntryDT,GETDATE())>7 and datediff(d,pw.EntryDT,GETDATE())<=15 then 1 else 0 end )= 1";
                }
                else
                {
                    whdaypara += " and (case when  lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) and datediff(d,pw.EntryDT,GETDATE())>15 then 1 else 0 end)= 1";
                }

            }



            string query = @"  
select w.work_id,lt.Device,case when lt.Device='Mobile' then 1 else 0 end as appProgress
,pw.EntryDT, case when w.proglat is not null then w.proglat else pw.Latitude end as Latitude
, case when w.ProgLong is not null then w.ProgLong else pw.Longitude end as Longitude
,case when convert(varchar,pw.EntryDT,103) = convert(varchar,GETDATE(),103)   then 1 else 0 end as TotalToday
,case when lt.Device='Mobile' and convert(varchar,pw.EntryDT,103) = convert(varchar,GETDATE(),103)   then 1 else 0 end as Mobiletoday
,case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())<=7
then 1 else 0 end as TotalInLast7Days
,case when lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())<=7
then 1 else 0 end as MobileInLast7Days,

case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>7 and datediff(d,pw.EntryDT,GETDATE())<=15
then 1 else 0 end as TotalInLast15Days

,case when lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>7 and datediff(d,pw.EntryDT,GETDATE())<=15
then 1 else 0 end as MobileLast15Days

,case when CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>15
then 1 else 0 end as TotalBefore15Days

,case when  lt.Device='Mobile' and CONVERT(VARCHAR, pw.EntryDT , 103) !=CONVERT(VARCHAR, GETDATE(), 103) 
and datediff(d,pw.EntryDT,GETDATE())>15
then 1 else 0 end as MobileBefore15Days
,dv.DivName_En,agd.DivisionID

,d.District_ID,d.DBStart_Name_En as districtname
,b.Block_Name_En, h.NAME_ENG+' - '+s.SWName+'('+w.work_id+')'  as workname
,msc.Name as Head
,wpp.ParentProgress as PLevel,CONVERT(varchar,ProgressDT,105) as ProgressDT
,case when r.Remarks is not null then r.Remarks else isnull(pw.DelayReason,pw.Remarks) end as Premarks
,CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt
,Convert(varchar, a.WrokOrderDT,105) as WrokOrderDT,CONVERT(varchar,case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
,c.EnglishName as contrctorname
,case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract
,isnull(ex.totalExp,0)+isnull(ex.gstcharge,0)  as GrossExpPaid
,rtrim(ltrim(e.Name)) as subeng,rtrim(ltrim(e2.Name)) as AE,l.login_name as Approver
,pw.SR,pw.ImageName
from  WorkMaster w
inner join Login l on l.Login_id=w.approved_by
inner join CGMSC_Team  e on e.Emp_Code=w.SubeEmpcode
inner join CGMSC_Team  e2 on e2.Emp_Code=w.AEEmpcode
left outer join AgreementDetails a on a.work_id=w.work_id
left outer join ContractMaster c on rtrim(ltrim(c.Contractorid))=ltrim(rtrim(a.Contractorid))
inner join SWDetails s on s.SWId=w.work_description_id 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join dhrsHealthCenter  h on  h.HC_ID  =w.worklocation_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
left outer join  BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) 
and agd.DivisionID not in ('D1032')

inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y' " + didclause + @"
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
left outer join WorkProgressLevel_Remarks r on r.RID=pw.remarkid

left outer join 
(
select Work_id as LTWorkid,p.Latitude,p.Longitude,p.Device  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where  1=1 " + didclause + @"
and Device='Mobile' and NewProgress='Y'
) lt on lt.LTWorkid=w.work_id 

left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal(18,2)) as totalExp 

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal(18,2)) gstcharge
from BillPayment b
where b.Ispaid='Y' and  isnull(ispass,'N') ='P' and b.ChequeNo is not null
group by work_code
) ex on ex.work_code=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121) " + whereClause + @"
" + whdaypara + "  order by pw.EntryDT desc  ";


            return await _context.ProgressCountDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }


        [HttpGet("EngAllotedWorks")]
        public async Task<ActionResult<IEnumerable<EngAllotedDTO>>> EngAllotedWorks(string engtype, string divisionid, string distid)
        {
            string? whereClause = "";
            string? orderbycluase = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";
                orderbycluase = " order by DivName_En, count(work_id) desc";

            }
            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";

            }
            string empjoin = " rtrim(ltrim(w.SubeEmpcode)) ";
            if (engtype=="AE")
            {
                empjoin = " rtrim(ltrim(w.aeEmpcode)) ";
            }
            string query = "";
          
               //WO / Running works which is alloted to eng
                query = $@" select DivisionID as ID,DivName_En as Name,EMPID,EngName as EngName,count(work_id) as TotalWorks,
cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr ,
sum(WOACC) as WOIssue,sum(running) as running,sum(Land) as ladissue

from 
(

select w.work_id,rtrim(ltrim(e.Name)) as EngName,dv.DivName_En,agd.DivisionID,rtrim(ltrim(e.Emp_Code)) as EMPID ,case when wpg.PGroupID=7 then 1 else 0 end as WOIssue
,case when r.RID is null and dash.did=5001 then 1 else 0 end as running,
case when r.RID is null and dash.did=3001 then 1 else 0 end as WOACC
, case when r.RID is not null and r.RID in (1,15)  then  1  else 0  end as Land
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end TVC
from  WorkMaster w
inner join AgreementDetails a on a.work_id=w.work_id
inner join CGMSC_Team  e on rtrim(ltrim(e.Emp_Code)) =" + empjoin + @"
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y' and  dash.did in (5001,6001,3001) 
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join WorkProgressLevel_Remarks r on r.RID=pw.remarkid
where  w.isdeleted is null  and w.MainSchemeID not in (121)
and a.wrokorderdt is not null and a.workorderrefnogovt is not null
and  dash.did in (5001,6001,3001) " +whereClause+@"
) b
group by DivName_En,EngName,EMPID,DivisionID
having (sum(WOACC)+sum(running)+sum(Land))>0 " +orderbycluase+@" ";

        

            return await _context.EngAllotedDbSet
          .FromSqlRaw(query)
          .ToListAsync();
        }
        [HttpGet("DistrictEngAllotedWorks")]
        public async Task<ActionResult<IEnumerable<DistrictEngAllotedDTO>>> DistrictEngAllotedWorks(string engtype, string divisionid, string distid)
        {
            string? whereClause = "";
            string? orderbycluase = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";
                orderbycluase = " order by  Name,count(work_id) desc";

            }
            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";
                orderbycluase = " order by  count(work_id) desc";

            }
            string empjoin = " rtrim(ltrim(w.SubeEmpcode)) ";
            if (engtype == "AE")
            {
                empjoin = " rtrim(ltrim(w.aeEmpcode)) ";
            }
            string query = "";

            //WO / Running works which is alloted to eng
            query = $@"  select cast(District_ID as varchar)+cast(EMPID as varchar) as ID, District_ID as DistrictID,Name as DistrictName,EMPID,EngName as EngName,count(work_id) as TotalWorks,
cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr ,
sum(WOACC) as WOIssue,sum(running) as running,sum(Land) as ladissue

from 
(

select w.work_id,rtrim(ltrim(e.Name))  as EngName,dbstart_name_en as Name,d.District_ID,rtrim(ltrim(e.Emp_Code)) as EMPID ,case when wpg.PGroupID=7 then 1 else 0 end as WOIssue
,case when r.RID is null and dash.did=5001 then 1 else 0 end as running,
case when r.RID is null and dash.did=3001 then 1 else 0 end as WOACC
, case when r.RID is not null and r.RID in (1,15)  then  1  else 0  end as Land
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end TVC
from  WorkMaster w
inner join AgreementDetails a on a.work_id=w.work_id
inner join CGMSC_Team  e on rtrim(ltrim(e.Emp_Code)) =" + empjoin + @"
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y' and  dash.did in (5001,6001,3001) 
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join WorkProgressLevel_Remarks r on r.RID=pw.remarkid
where  w.isdeleted is null  and w.MainSchemeID not in (121)
and a.wrokorderdt is not null and a.workorderrefnogovt is not null
and  dash.did in (5001,6001,3001) " + whereClause+@"
) b
group by District_ID ,Name,EngName,EMPID
having (sum(WOACC)+sum(running)+sum(Land))>0 " + orderbycluase + @" ";



            return await _context.DistrictEngAllotedDbSet
          .FromSqlRaw(query)
          .ToListAsync();
        }

        [HttpGet("getWorkDetailsWithEng")]

        public async Task<ActionResult<IEnumerable<WorkorderpendingdetailsDTO>>> getWorkDetailsWithEng(string dahid,string divisionId, string mainSchemeId, string distid,string engtype, string empcode)
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
            if (empcode != "0")
            {

                whereClause += $" and rtrim(ltrim(e.Emp_Code))  = '{empcode.TrimEnd().TrimStart()}'";

            }
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            string whdashclaue = " and  dash.did in (5001,6001,3001)  ";
            if (dahid != "0")
            {

                whdashclaue += $" and  dash.did = {dahid}";
            }
            string empjoin = " rtrim(ltrim(w.SubeEmpcode)) ";
            if (engtype == "AE")
            {
                empjoin = " rtrim(ltrim(w.aeEmpcode)) ";
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
inner join CGMSC_Team  e on rtrim(ltrim(e.Emp_Code)) =" + empjoin + @"
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
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y' " + whdashclaue + @" 
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID,p.DelayReason
) pl on pl.Work_id=w.work_id
where 1=1 and w.isdeleted is null  and w.MainSchemeID not in (121)  
 and n.WrokOrderDT is not null and n.workorderrefnogovt is not null " + whereClause + @"
 order by   n.WrokOrderDT  desc ";

            return await _context.WorkorderpendingdetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }
    }
}
