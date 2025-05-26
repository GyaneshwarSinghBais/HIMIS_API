using HIMIS_API.Data;
using HIMIS_API.Models.DetailsProgress;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using System.Security.Cryptography;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetailProgress : Controller
    {
        private readonly DbContextData _context;
        public DetailProgress(DbContextData context)
        {
            _context = context;
        }

        [HttpGet("TotalWorksAbstract")]
        public async Task<ActionResult<IEnumerable<WORunningHandDetailsDTO>>> TotalWorksAbstract(string divisionid, string districtid, string mainschemeid, string contractorid,string ASAmount)
        {
            string query = "";
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }
            if (mainschemeid != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainschemeid}";
            }
            if (contractorid != "0")
            {

                whereClause += $" and  rtrim(ltrim(cnt.Contractorid)) = '{contractorid}'";
            }


            if (ASAmount != "0")
            {
                if (ASAmount == "1")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>= 200";

                }
                if (ASAmount == "2")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=50  and cast(AaAmt as decimal(18, 2))< 200";
                }

                if (ASAmount == "3")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=20  and cast(AaAmt as decimal(18, 2))< 50";

                }

                if (ASAmount == "4")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))< 20";

                }

            }

            query = $@" 

select dv.DivName_En, msc.Name as Head,w.work_id,w.LetterNo,ap.login_name as Approver,w.GrantNo,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,
 d.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  h.NAME_ENG+' - '+rtrim(ltrim(s.SWName))  as work,
  cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,105) as TSDate ,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference
  ,n.AcceptanceLetterRefNo ,convert(varchar, n.AcceptLetterDT,105) as AcceptLetterDT  ,cast(n.SanctionRate as decimal(18,2)) as SanctionRate
,n.SanctionDetail,convert(varchar, n.DateOfSanction,103) as DateOfSanction,convert(varchar, n.DateOfIssueNIT,103) as DateOfIssueNIT
  ,convert(varchar,n.WrokOrderDT,105) as WrokOrderDT ,case when n.HOAllotedDT is null then '-' else convert(varchar,n.HOAllotedDT,105) end as HOAllotedDT ,n.AgreementRefNo
   ,cast(n.TimeAllowed as bigint) as TimeAllowed
 ,isnull(n.WorkorderRefNoGovt,'') as WorkorderRefNoGovt,
 CONVERT(varchar,case when n.TimeAllowed>6 then DATEADD(Month, n.TimeAllowed+1, n.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, n.TimeAllowed, n.WrokOrderDT))  end,105) DueDTTimePerAdded
  ,case when n.formt='B' then Round(cast(Round(n.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(n.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

  ,isnull(totalpaid,0) as Totalpaid
  ,isnull(GrossUnpaid,0) as Totalunpaid



 ,cnt.Contractorid as CID,isnull(cnt.Title,'')+rtrim(ltrim(cnt.EnglishName)) as ContractorNAme,cnt.RegType,cnt.Class,cnt.EnglishAddress,cnt.MobNo ,
GroupName,PGroupID,pl.LProgress,convert(varchar,pl.ProgressDT,105) as ProgressDT,pl.PRemarks,pl.Remarks,pl.RID,

d.District_ID,agd.DivisionID,msc.MainSchemeID 
,did,DashName, case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end as drid,
case when pl.RID is not null  and pl.RID in (1,15) and w.ISRETURNDEP is null then 'Land Not Alloted/Land Dispute' else 
case when w.ISRETURNDEP ='Y' then 'Return to Department' else 
DashName end end as display
,w.asid,w.ASPath,w.ASLetter 
,rtrim(ltrim(s.SWName)) as descri
, wt.type_name,fmr.fmrcode,was.fmrid
,rtrim(ltrim(pldr.delayreason)) as delayreason
,case when plEC.expcompdt is not null then convert(varchar,plEC.expcompdt,105)  else '' end as expcompdt 
, rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname
,rtrim(ltrim(isnull(ae.Name,'Not Alloted'))) as AEName
from  WorkMaster w

left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id

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

 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode
left outer join WorkMasteras was on was.asid=w.asid
left outer join masfmrcode fmr on fmr.fmrid=was.fmrid
  left outer join Login ap on ap.Login_id= w.approved_by
   left outer join WorkType wt on wt.type_id= w.work_type
   left outer join AgreementDetails  n on n.work_id= w.work_id
   left outer join ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))
  left outer join SWDetails s on s.SWId=w.work_description_id 
inner join dhrsHealthCenter  h on  h.HC_ID  =w.worklocation_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
    left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
 left outer join MasTender t on t.tenderid= n.tenderid
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

where   (case when w.isdeleted ='Y' and w.ISRETURNDEP is null then 0 else 1 end )=1  and w.MainSchemeID not in (121) " + whereClause + @"
order by dv.DivName_En, d.DBStart_Name_En";


            return await _context.WORunningHandDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }

            [HttpGet("WORunningHandDetails")]
        public async Task<ActionResult<IEnumerable<WORunningHandDetailsDTO>>> WORunningHandDetails(string did,string divisionid,string districtid, string mainschemeid,string contractorid,string ASAmount)
        {
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }
            if (mainschemeid != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainschemeid}";
            }
            if (contractorid != "0")
            {

                whereClause += $" and  rtrim(ltrim(cnt.Contractorid)) = '{contractorid}'";
            }
            string orderbyclause = "";
            if (did == "3001")
            {
                //WOAcceptance

                orderbyclause = " order by n.AcceptLetterDT desc ";
            }
            if (did == "4001")
            {
                //Handover

                orderbyclause = " order by pl.ProgressDT desc ";
            }
            if (did == "5001")
            {
                //Running

                orderbyclause = " order by pl.ProgressDT desc ";
            }



            if (ASAmount != "0")
            {
                if (ASAmount == "1")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>= 200";

                }
                if (ASAmount == "2")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=50  and cast(AaAmt as decimal(18, 2))< 200";
                }

                if (ASAmount == "3")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=20  and cast(AaAmt as decimal(18, 2))< 50";

                }

                if (ASAmount == "4")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))< 20";

                }

            }



            string query = "";
           
                query = $@" 

select dv.DivName_En, msc.Name as Head,w.work_id,w.LetterNo,ap.login_name as Approver,w.GrantNo,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,
 d.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  h.NAME_ENG+' - '+rtrim(ltrim(s.SWName))  as work,
  cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,105) as TSDate ,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference
  ,n.AcceptanceLetterRefNo ,convert(varchar, n.AcceptLetterDT,105) as AcceptLetterDT  ,cast(n.SanctionRate as decimal(18,2)) as SanctionRate
,n.SanctionDetail,convert(varchar, n.DateOfSanction,103) as DateOfSanction,convert(varchar, n.DateOfIssueNIT,103) as DateOfIssueNIT
  ,convert(varchar,n.WrokOrderDT,105) as WrokOrderDT ,case when n.HOAllotedDT is null then '-' else convert(varchar,n.HOAllotedDT,105) end as HOAllotedDT ,n.AgreementRefNo
   ,cast(n.TimeAllowed as bigint) as TimeAllowed
 ,isnull(n.WorkorderRefNoGovt,'') as WorkorderRefNoGovt,
 CONVERT(varchar,case when n.TimeAllowed>6 then DATEADD(Month, n.TimeAllowed+1, n.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, n.TimeAllowed, n.WrokOrderDT))  end,105) DueDTTimePerAdded
  ,case when n.formt='B' then Round(cast(Round(n.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(n.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

  ,isnull(totalpaid,0) as Totalpaid
  ,isnull(GrossUnpaid,0) as Totalunpaid


 ,cnt.Contractorid as CID,isnull(cnt.Title,'')+rtrim(ltrim(cnt.EnglishName)) as ContractorNAme,cnt.RegType,cnt.Class,cnt.EnglishAddress,cnt.MobNo ,
GroupName,PGroupID,pl.LProgress,convert(varchar,pl.ProgressDT,105) as ProgressDT,pl.PRemarks,pl.Remarks,pl.RID,

d.District_ID,agd.DivisionID,msc.MainSchemeID 
,did,DashName, case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end as drid,
case when pl.RID is not null  and pl.RID in (1,15) and w.ISRETURNDEP is null then 'Land Not Alloted/Land Dispute' else 
case when w.ISRETURNDEP ='Y' then 'Return to Department' else 
DashName end end as display
,w.asid,w.ASPath,w.ASLetter 
,rtrim(ltrim(s.SWName)) as descri
, wt.type_name,fmr.fmrcode,was.fmrid
,rtrim(ltrim(pldr.delayreason)) as delayreason
,case when plEC.expcompdt is not null then convert(varchar,plEC.expcompdt,105)  else '' end as expcompdt 
, rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname
,rtrim(ltrim(isnull(ae.Name,'Not Alloted'))) as AEName
from  WorkMaster w

left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id

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

 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode
left outer join WorkMasteras was on was.asid=w.asid
left outer join masfmrcode fmr on fmr.fmrid=was.fmrid
  left outer join Login ap on ap.Login_id= w.approved_by
   left outer join WorkType wt on wt.type_id= w.work_type
inner join AgreementDetails  n on n.work_id= w.work_id
inner join ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))
  left outer join SWDetails s on s.SWId=w.work_description_id 
inner join dhrsHealthCenter  h on  h.HC_ID  =w.worklocation_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
    left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
 left outer join MasTender t on t.tenderid= n.tenderid
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

where   (case when w.isdeleted ='Y' and w.ISRETURNDEP is null then 0 else 1 end )=1  and w.MainSchemeID not in (121) " + whereClause + @"

and (case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end)=" + did + @"
"+ orderbyclause + "" ;
          

            return await _context.WORunningHandDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("LandIssue_RetToDeptDetatails")]

        public async Task<ActionResult<IEnumerable<WORunningHandDetailsDTO>>> LandIssue_RetToDeptDetatails(string did, string divisionid, string districtid, string mainschemeid,string ASAmount)
        {
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }
            if (mainschemeid != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainschemeid}";
            }
            string orderbyclause = "";
            if (did == "6001")
            {
                //Land Issue
                orderbyclause = " order by pl.ProgressDT desc ";
          
            }
            if (did == "8001")
            {
                //Return to department

                orderbyclause = " order by pl.ProgressDT desc ";
            }

            if (ASAmount != "0")
            {
                if (ASAmount == "1")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>= 200";

                }
                if (ASAmount == "2")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=50  and cast(AaAmt as decimal(18, 2))< 200";
                }

                if (ASAmount == "3")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=20  and cast(AaAmt as decimal(18, 2))< 50";

                }

                if (ASAmount == "4")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))< 20";

                }

            }


            string query = "";

            query = $@" 

select dv.DivName_En, msc.Name as Head,w.work_id,rtrim(ltrim(w.LetterNo)) as LetterNo ,ap.login_name as Approver,w.GrantNo,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,
 d.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  h.NAME_ENG+' - '+rtrim(ltrim(s.SWName))  as work,
  cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,105) as TSDate ,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, n.TenderReference
  ,n.AcceptanceLetterRefNo ,convert(varchar, n.AcceptLetterDT,105) as AcceptLetterDT  ,cast(n.SanctionRate as decimal(18,2)) as SanctionRate
,n.SanctionDetail,convert(varchar, n.DateOfSanction,103) as DateOfSanction,convert(varchar, n.DateOfIssueNIT,103) as DateOfIssueNIT
  ,convert(varchar,n.WrokOrderDT,105) as WrokOrderDT ,case when n.HOAllotedDT is null then '-' else convert(varchar,n.HOAllotedDT,105) end as HOAllotedDT ,n.AgreementRefNo
   ,cast(n.TimeAllowed as bigint) as TimeAllowed
 ,isnull(n.WorkorderRefNoGovt,'') as WorkorderRefNoGovt,
 CONVERT(varchar,case when n.TimeAllowed>6 then DATEADD(Month, n.TimeAllowed+1, n.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, n.TimeAllowed, n.WrokOrderDT))  end,105) DueDTTimePerAdded
  ,case when n.formt='B' then Round(cast(Round(n.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(n.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract

  ,isnull(totalpaid,0) as Totalpaid
  ,isnull(GrossUnpaid,0) as Totalunpaid


 ,cnt.Contractorid as CID,isnull(cnt.Title,'')+rtrim(ltrim(cnt.EnglishName)) as ContractorNAme,cnt.RegType,cnt.Class,rtrim(ltrim(cnt.EnglishAddress)) as EnglishAddress ,cnt.MobNo ,
GroupName,PGroupID,pl.LProgress,convert(varchar,pl.ProgressDT,105) as ProgressDT,rtrim(ltrim(pl.PRemarks)) as PRemarks ,rtrim(ltrim(pl.Remarks)) as Remarks ,pl.RID,

d.District_ID,agd.DivisionID,msc.MainSchemeID 
,did,DashName, case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end as drid,
case when pl.RID is not null  and pl.RID in (1,15) and w.ISRETURNDEP is null then 'Land Not Alloted/Land Dispute' else 
case when w.ISRETURNDEP ='Y' then 'Return to Department' else 
DashName end end as display
,w.asid,w.ASPath,w.ASLetter 
,rtrim(ltrim(s.SWName)) as descri
, wt.type_name,fmr.fmrcode,was.fmrid
,rtrim(ltrim(pldr.delayreason)) as delayreason
,case when plEC.expcompdt is not null then convert(varchar,plEC.expcompdt,105)  else '' end as expcompdt 
, rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname
,rtrim(ltrim(isnull(ae.Name,'Not Alloted'))) as AEName
from  WorkMaster w
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id

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

 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode
left outer join WorkMasteras was on was.asid=w.asid
left outer join masfmrcode fmr on fmr.fmrid=was.fmrid
  left outer join Login ap on ap.Login_id= w.approved_by
   left outer join WorkType wt on wt.type_id= w.work_type
  left outer join AgreementDetails  n on n.work_id= w.work_id
  left outer join ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))
  left outer join SWDetails s on s.SWId=w.work_description_id 
inner join dhrsHealthCenter  h on  h.HC_ID  =w.worklocation_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
    left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
 left outer join MasTender t on t.tenderid= n.tenderid
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

where   (case when w.isdeleted ='Y' and w.ISRETURNDEP is null then 0 else 1 end )=1  and w.MainSchemeID not in (121)
 " + whereClause + @"

and (case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end)=" + did + @"
" + orderbyclause + "";


            return await _context.WORunningHandDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }


        [HttpGet("TobeTenderAll")]

        public async Task<ActionResult<IEnumerable<TobeTenderDash>>> TobeTenderAll(string did, string divisionid, string districtid, string mainschemeid, string ASAmount)
        {
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }
            if (mainschemeid != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainschemeid}";
            }
            string orderbyclause = "";
            if (did == "1001")
            {
                //tobeTender
                orderbyclause = " order by w.AADate desc ";

            }


            if (ASAmount != "0")
            {
                if (ASAmount == "1")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>= 200";

                }
                if (ASAmount == "2")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=50  and cast(AaAmt as decimal(18, 2))< 200";
                }

                if (ASAmount == "3")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=20  and cast(AaAmt as decimal(18, 2))< 50";

                }

                if (ASAmount == "4")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))< 20";

                }

            }

            string query = "";

            query = $@" select dv.DivName_En, msc.Name as Head,w.work_id,w.LetterNo,ap.login_name as Approver,w.GrantNo,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,
 d.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  h.NAME_ENG+' - '+rtrim(ltrim(s.SWName))  as work, 
    cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,105) as TSDate,
	wpp.ParentProgress as   LProgress,convert(varchar,p.ProgressDT,105) as ProgressDT,case when r.Remarks is null then  p.Remarks else r.Remarks end as Remarks,
  cast(p.ppid as varchar) as ppid
,wpg.GroupName,cast(wpg.PGroupID as varchar) as PGroupID ,cast(dash.did as varchar) as did,dash.DashName

,cast(w.asid as varchar) as asid,w.ASPath,w.ASLetter 
,s.SWName as descri
, wt.type_name,fmr.fmrcode,cast(was.fmrid as varchar) as fmrid
,rtrim(ltrim(pldr.delayreason)) as delayreason
,case when plEC.expcompdt is not null then convert(varchar,plEC.expcompdt,105)  else '' end as expcompdt 
, rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname
,rtrim(ltrim(isnull(ae.Name,'Not Alloted'))) as AEName
,cast(t.TenderNo as varchar) as LastNIT,cast(t.eProcNo as varchar) as LastEprocno,rj.RejReason,convert(varchar,t.RejEntryDT,105) as RejectedDT
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id
 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode
left outer join WorkMasteras was on was.asid=w.asid
left outer join masfmrcode fmr on fmr.fmrid=was.fmrid
  left outer join Login ap on ap.Login_id= w.approved_by
   left outer join WorkType wt on wt.type_id= w.work_type

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
 left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
  inner join  dhrsHealthCenter dh on dh.HC_ID= w.worklocation_id
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
 left outer join SWDetails s on s.SWId= w.work_description_id
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

 left outer join
 (
 select max(twid) twid,Work_id from MasTenderWorks tw
 where tw.rejid is not null 
 group by Work_id
 ) ltw on ltw.Work_id=w.work_id

 left outer join MasTenderWorks Tw on tw.TWID=ltw.twid
 left outer join MasTenderRejReason rj on rj.RejID=tw.RejID
left outer join MasTender t on t.TenderID= Tw.tenderid
where 1=1 and w.isdeleted  is null and  NewProgress='Y'  and w.MainSchemeID not in (121)  " + whereClause + @"   and dash.did =1001 ";

           return await _context.TobeTenderDashDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }



        [HttpGet("TenderInProcess")]

        public async Task<ActionResult<IEnumerable<TenderinProcess>>> TenderInProcess(string did, string divisionid, string districtid, string mainschemeid, string ASAmount)
        {
            string? whereClause = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }
            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }
            if (mainschemeid != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainschemeid}";
            }
            string orderbyclause = "";
            if (did == "2001")
            {
                //tobeTender
                orderbyclause = " order by w.AADate desc ";

            }

            if (ASAmount != "0")
            {
                if (ASAmount == "1")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>= 200";

                }
                if (ASAmount == "2")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=50  and cast(AaAmt as decimal(18, 2))< 200";
                }

                if (ASAmount == "3")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))>=20  and cast(AaAmt as decimal(18, 2))< 50";

                }

                if (ASAmount == "4")
                {
                    whereClause += $" and cast(AaAmt as decimal(18, 2))< 20";

                }

            }


            string query = "";

            query = $@" select dv.DivName_En, msc.Name as Head,w.work_id,w.LetterNo,ap.login_name as Approver,w.GrantNo,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,
 d.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  h.NAME_ENG+' - '+s.SWName  as work, 
    cast(w.TSAmount as decimal(18,2)) as TSAMT,convert(varchar, w.TSDate,105) as TSDate,
	wpp.ParentProgress as   LProgress,convert(varchar,p.ProgressDT,105) as ProgressDT,case when r.Remarks is null then  p.Remarks else r.Remarks end as Remarks,
  p.ppid
,wpg.GroupName,wpg.PGroupID,dash.did,dash.DashName

,w.asid,w.ASPath,w.ASLetter 
,s.SWName as descri
, wt.type_name,fmr.fmrcode,was.fmrid
,convert(varchar,t.startdt,105) startdt,convert(varchar,t.enddt,105) as enddt,isnull(tw.Noofcalls,0) as Noofcalls
,datediff(dd,t.enddt,getdate()) as DaystoEnd,cast(t.tenderno as varchar) as tenderno ,cast(t.eprocno as varchar) eprocno
,case when t.topnedbdt is not null then convert(varchar,t.topnedbdt,105) else  convert(varchar,t.topneddt,105) end CovOpenedDT
,convert(varchar,t.topnedpricedt,105) topnedpricedt 
from  WorkPhysicalProgress p
inner join WorkMaster w on w.work_id=p.work_id
 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode
left outer join WorkMasteras was on was.asid=w.asid
left outer join masfmrcode fmr on fmr.fmrid=was.fmrid
  left outer join Login ap on ap.Login_id= w.approved_by
   left outer join WorkType wt on wt.type_id= w.work_type

inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.DISTRICT_ID=w.worklocationdist_id
inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
 left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
  inner join  dhrsHealthCenter dh on dh.HC_ID= w.worklocation_id
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
 left outer join SWDetails s on s.SWId= w.work_description_id


 left outer join
 (
 select max(twid) twid,Work_id from MasTenderWorks tw
 where tw.rejid is not null 
 group by Work_id
 ) ltw on ltw.Work_id=w.work_id

 left outer join MasTenderWorks Tw on tw.TWID=ltw.twid and tw.Work_id=w.work_id
 left outer join MasTenderRejReason rj on rj.RejID=tw.RejID
left outer join MasTender t on t.TenderID= Tw.tenderid
where 1=1  " + whereClause + @" and w.isdeleted  is null and  NewProgress='Y'  and w.MainSchemeID not in (121) and dash.did =2001 order by p.ppid  ";

            return await _context.TenderinProcessDbSet
             .FromSqlRaw(query)
             .ToListAsync();

        }




    }
}

