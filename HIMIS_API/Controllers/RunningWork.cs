using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using HIMIS_API.Data;
using HIMIS_API.Models.DetailsProgress;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.RunningWork;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RunningWork : Controller
    {
        private readonly DbContextData _context;
        public RunningWork(DbContextData context)
        {
            _context = context;
        }

        [HttpGet("RunningWorkSummary")]
        public async Task<ActionResult<IEnumerable<RunningWorkDTOSummary>>> RunningWorkSummary(string RPType, string divisionid, string districtid, string mainSchemeId,string contractid)
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
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            if (contractid != "0")
            {

                whereClause += $" and rtrim(ltrim(c.Contractorid)) = '{contractid}'";
            }
           

            string query = "";
            if (RPType == "GTotal")
            {
                //Gtotal
                query = $@"  select cast(1 as varchar) as ID,'-' as  Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
from 
(
select w.work_id,dv.DivName_En,agd.DivisionID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121) " + whereClause + @"
 and dash.did =5001
) b ";
            }
            if (RPType == "Division")
            {
                //divisinwise
                query = $@" select DivisionID as ID,DivName_En as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
from 
(
select w.work_id,dv.DivName_En,agd.DivisionID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121) 
 and dash.did =5001 " + whereClause + @"
) b
group by DivName_En,DivisionID
order by  DivName_En ";
            }
            if (RPType == "Scheme")
            {
                //Scheme

                query = $@" select cast(MainSchemeID as varchar) as ID,name as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
from 
(
select w.work_id,msc.name,w.MainSchemeID 
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121)
 and dash.did =5001 " + whereClause + @"
) b
group by  MainSchemeID,name
order by   cast(SUM(TVC)/100 as decimal(18,2)) desc ";
            }
            if (RPType == "District")
            {
                //Districtwise
                query = $@" select cast(District_ID as varchar) as ID,dbstart_name_en as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
from 
(
select w.work_id,dis.dbstart_name_en,dis.District_ID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join division dv on dv.div_id=dis.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121)
 and dash.did =5001 " + whereClause + @"
) b
group by  District_ID,dbstart_name_en
order by   cast(SUM(TVC)/100 as decimal(18,2)) desc ";
            }
            if (RPType == "Contractor")
            {
                //Contractor above 4 cr
                query = $@" select cast(Contractorid as varchar) as ID,Contractor as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
from 
(
select w.work_id,rtrim(ltrim(c.englishname)) as Contractor,rtrim(ltrim(c.Contractorid)) Contractorid
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join  ContractMaster c on rtrim(ltrim(c.Contractorid))=rtrim(ltrim(a.ContractorID))
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121)
 and dash.did =5001 " + whereClause + @"
) b
where  1=1
group by  Contractorid,Contractor
having cast(SUM(TVC)/100 as decimal(18,2))>=4
order by   cast(SUM(TVC)/100 as decimal(18,2)) desc ";
           
            }
                return await _context.RunningWorkDTOSummaryDbSet
        .FromSqlRaw(query)
        .ToListAsync();
        }




        [HttpGet("RunningWorkSummaryDelay")]
        public async Task<ActionResult<IEnumerable<RunningWorkDelayDTO>>> RunningWorkSummaryDelay(string RPType, string divisionid, string districtid, string mainSchemeId, string contractid)
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
            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            if (contractid != "0")
            {

                whereClause += $" and rtrim(ltrim(c.Contractorid)) = '{contractid}'";
            }


            string query = "";
            if (RPType == "GTotal")
            {
                query = @" select cast(1 as varchar) as ID,'-' as  Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
    ,sum(MorethanSix) as MorethanSixMonth,sum(exce3_6Month) as D_91_180Days
	,sum(exce1_3Month) as D_1_90Days,sum(TimeValid) as TimeValid
	,(sum(MorethanSix)+sum(exce3_6Month)+sum(exce1_3Month)) as TotalDelayWork
from 
(
select w.work_id,dv.DivName_En,agd.DivisionID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
,convert(varchar,WrokOrderDT,105) as WorkorderDT,cast(a.TimeAllowed as int) as TimeAllowed
, CONVERT(varchar,case when a.TimeAllowed>6 then DATEADD(Month, a.TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, a.TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
,case when  (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=0
 then  1 else 0 end as TimeValid
 , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>180 
 then  1 else 0 end as MorethanSix
  , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>90
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=180 
 then  1 else 0 end as exce3_6Month
 ,case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>=1
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=90 
 then  1 else 0 end as exce1_3Month
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121) 
 and dash.did =5001 " + whereClause + @"
) b ";
            }
            if (RPType == "Division")
            {
                query = @" select DivisionID as ID,DivName_En as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
    ,sum(MorethanSix) as MorethanSixMonth,sum(exce3_6Month) as D_91_180Days
	,sum(exce1_3Month) as D_1_90Days,sum(TimeValid) as TimeValid
	,(sum(MorethanSix)+sum(exce3_6Month)+sum(exce1_3Month)) as TotalDelayWork
from 
(
select w.work_id,dv.DivName_En,agd.DivisionID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
,convert(varchar,WrokOrderDT,105) as WorkorderDT,cast(a.TimeAllowed as int) as TimeAllowed
, CONVERT(varchar,case when a.TimeAllowed>6 then DATEADD(Month, a.TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, a.TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
,case when  (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=0
 then  1 else 0 end as TimeValid
 , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>180 
 then  1 else 0 end as MorethanSix
  , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>90
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=180 
 then  1 else 0 end as exce3_6Month
 ,case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>=1
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=90 
 then  1 else 0 end as exce1_3Month
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null   and w.MainSchemeID not in (121) 
 and dash.did =5001 "+ whereClause + @"
) b
group by DivName_En,DivisionID
order by  (sum(MorethanSix)+sum(exce3_6Month)+sum(exce1_3Month)) desc ";
            }
            if (RPType == "Scheme")
            {
                //Scheme

                query = $@" select cast(MainSchemeID as varchar) as ID,name as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr
 , cast(SUM(PaidTillLacs)/100 as decimal(18,2)) as PaidTillcr 
  , cast(SUM(GrossPendinglacs)/100 as decimal(18,2)) as GrossPendingcr 
    ,sum(MorethanSix) as MorethanSixMonth,sum(exce3_6Month) as D_91_180Days
	,sum(exce1_3Month) as D_1_90Days,sum(TimeValid) as TimeValid
	,(sum(MorethanSix)+sum(exce3_6Month)+sum(exce1_3Month)) as TotalDelayWork
from 
(
select w.work_id,msc.Name,msc.MainSchemeID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,isnull(totalpaid,0) PaidTillLacs
,isnull(GrossPending,0) GrossPendinglacs
,convert(varchar,WrokOrderDT,105) as WorkorderDT,cast(a.TimeAllowed as int) as TimeAllowed
, CONVERT(varchar,case when a.TimeAllowed>6 then DATEADD(Month, a.TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, a.TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
,case when  (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=0
 then  1 else 0 end as TimeValid
 , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>180 
 then  1 else 0 end as MorethanSix
  , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>90
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=180 
 then  1 else 0 end as exce3_6Month
 ,case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>=1
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=90 
 then  1 else 0 end as exce1_3Month
from  WorkMaster w
inner join  AgreementDetails a on a.work_id=w.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

where  w.isdeleted is null  and w.MainSchemeID not in (121)
 and dash.did =5001 " + whereClause + @"
) b
group by  MainSchemeID,name
order by  (sum(MorethanSix)+sum(exce3_6Month)+sum(exce1_3Month)) desc ";
            }

            if (RPType == "Contractor")
            {
                //Contractor above 4 cr

                query = @" select cast(Contractorid as varchar) as ID,Contractor as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC) / 100 as decimal(18, 2)) as TVCValuecr
 , cast(SUM(PaidTillLacs) / 100 as decimal(18, 2)) as PaidTillcr
  , cast(SUM(GrossPendinglacs) / 100 as decimal(18, 2)) as GrossPendingcr
    ,sum(MorethanSix) as MorethanSixMonth,sum(exce3_6Month) as D_91_180Days
	,sum(exce1_3Month) as D_1_90Days,sum(TimeValid) as TimeValid
	,(sum(MorethanSix) + sum(exce3_6Month) + sum(exce1_3Month)) as TotalDelayWork
from
(
select w.work_id, rtrim(ltrim(c.EnglishName)) as Contractor, rtrim(ltrim(c.Contractorid)) as Contractorid
,case when a.TotalAmountOfContract is not null then(case when a.FormT = 'B' then round(a.TotalAmountOfContract/ 100000,2) else Round(cast(a.TotalAmountOfContract as float(2)), 2)  end) else 0 end as TVC
,isnull(totalpaid, 0) PaidTillLacs
,isnull(GrossPending, 0) GrossPendinglacs
,convert(varchar, WrokOrderDT, 105) as WorkorderDT,cast(a.TimeAllowed as int) as TimeAllowed
, CONVERT(varchar,case when a.TimeAllowed > 6 then DATEADD(Month, a.TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day, 15, DATEADD(Month, a.TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
,case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<= 0
 then  1 else 0 end as TimeValid
 , case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )> 180
 then  1 else 0 end as MorethanSix
  , case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )> 90
  and(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<= 180
 then  1 else 0 end as exce3_6Month
 ,case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>= 1
  and(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<= 90
 then  1 else 0 end as exce1_3Month
from WorkMaster w
inner
join AgreementDetails a on a.work_id = w.work_id
inner
join ContractMaster c on rtrim(ltrim(c.Contractorid)) = rtrim(ltrim(a.ContractorID))
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner
join
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid = p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID = wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did = wpg.did
where NewProgress = 'Y'   and dash.did = 5001
group by Work_id
) p on p.Work_id = w.work_id
inner join WorkPhysicalProgress pw on pw.SR = p.sr
inner join WorkLevelParent wpp on wpp.ppid = pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID = wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did = wpg.did
left outer join
(
select work_code, sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18, 2))) + Round((cast(PaidGrossAmount as decimal(18, 2))) * 5 / 100, 2)) as decimal (18, 2)) + cast(((Round((cast(PaidGrossAmount as decimal(18, 2))) * 5 / 100, 2) * 18) / 100) as decimal (18, 2))) / 100000, 2) as decimal(18, 2))) as totalpaid from BillPayment
where Ispaid = 'Y' and isnull(ispass, 'N') = 'P' and ChequeNo is not null
group by work_code
) bt on bt.work_code = w.work_id
left outer join
(
select w.work_id, cast(round((case when PaidGrossAmount is null
then case when EEGrossAmount is null
 then case when AEGrossAmount is null
 then GrossAmount else AEGrossAmount end
 else EEGrossAmount end
 else PaidGrossAmount end)/ 100000,2) as decimal(18, 2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id = b.work_code
where 1 = 1 and b.BillDate > '01-Apr-2022' and b.isoldPaid is null and isnull(b.Ispaid, 'N') = 'N'
and b.ChequeNo is null and w.IsDeleted is null and GrossAmount > 0
)   up on up.work_id = w.work_id

where w.isdeleted is null and w.MainSchemeID not in (121)
 and dash.did = 5001 " + whereClause + @"
) b
where  1 = 1
group by  Contractorid,Contractor
having(count(work_id)) > 10
order by(sum(MorethanSix)+sum(exce3_6Month) + sum(exce1_3Month)) desc ";

            }
            if (RPType == "District")
            {
                //Districtwise

                query = @" select cast(District_ID as varchar) as ID,dbstart_name_en as Name,count(work_id) as TotalWorks,
 cast(SUM(TVC) / 100 as decimal(18, 2)) as TVCValuecr
 , cast(SUM(PaidTillLacs) / 100 as decimal(18, 2)) as PaidTillcr
  , cast(SUM(GrossPendinglacs) / 100 as decimal(18, 2)) as GrossPendingcr
    ,sum(MorethanSix) as MorethanSixMonth,sum(exce3_6Month) as D_91_180Days
	,sum(exce1_3Month) as D_1_90Days,sum(TimeValid) as TimeValid
	,(sum(MorethanSix) + sum(exce3_6Month) + sum(exce1_3Month)) as TotalDelayWork
	,DivisionName
from
                (
                select w.work_id, dis.DBStart_Name_En, dis.District_ID, agd.DivisionName
,case when a.TotalAmountOfContract is not null then(case when a.FormT = 'B' then round(a.TotalAmountOfContract/ 100000,2) else Round(cast(a.TotalAmountOfContract as float(2)), 2)  end) else 0 end as TVC
,isnull(totalpaid, 0) PaidTillLacs
,isnull(GrossPending, 0) GrossPendinglacs
,convert(varchar, WrokOrderDT, 105) as WorkorderDT,cast(a.TimeAllowed as int) as TimeAllowed
, CONVERT(varchar,case when a.TimeAllowed > 6 then DATEADD(Month, a.TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day, 15, DATEADD(Month, a.TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
,case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<= 0
 then  1 else 0 end as TimeValid
 , case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )> 180
 then  1 else 0 end as MorethanSix
  , case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )> 90
  and(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<= 180
 then  1 else 0 end as exce3_6Month
 ,case when(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>= 1
  and(DATEDIFF(DAY, (case when a.TimeAllowed > 6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else DATEADD(day, 15, DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<= 90
 then  1 else 0 end as exce1_3Month
from WorkMaster w
inner
join AgreementDetails a on a.work_id = w.work_id
inner
join Districts dis on dis.DISTRICT_ID = w.worklocationdist_id
inner
join division dv on dv.div_id = dis.div_id
inner
join agencydivisionmaster agd on cast(agd.divisionname as bigint) = cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join
(
select max(SR) as sr,Work_id from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid = p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID = wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did = wpg.did
where NewProgress = 'Y'   and dash.did = 5001
group by Work_id
) p on p.Work_id = w.work_id
inner join WorkPhysicalProgress pw on pw.SR = p.sr
inner join WorkLevelParent wpp on wpp.ppid = pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID = wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did = wpg.did
left outer join
(
select work_code, sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18, 2))) + Round((cast(PaidGrossAmount as decimal(18, 2))) * 5 / 100, 2)) as decimal (18, 2)) + cast(((Round((cast(PaidGrossAmount as decimal(18, 2))) * 5 / 100, 2) * 18) / 100) as decimal (18, 2))) / 100000, 2) as decimal(18, 2))) as totalpaid from BillPayment
where Ispaid = 'Y' and isnull(ispass, 'N') = 'P' and ChequeNo is not null
group by work_code
) bt on bt.work_code = w.work_id
left outer join
(
select w.work_id, cast(round((case when PaidGrossAmount is null
then case when EEGrossAmount is null
 then case when AEGrossAmount is null
 then GrossAmount else AEGrossAmount end
 else EEGrossAmount end
 else PaidGrossAmount end)/ 100000,2) as decimal(18, 2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id = b.work_code
where 1 = 1 and b.BillDate > '01-Apr-2022' and b.isoldPaid is null and isnull(b.Ispaid, 'N') = 'N'
and b.ChequeNo is null and w.IsDeleted is null and GrossAmount > 0
)   up on up.work_id = w.work_id

where w.isdeleted is null and w.MainSchemeID not in (121)
 and dash.did = 5001 " + whereClause + @"
) b
group by District_ID, dbstart_name_en, DivisionName
order by  DivisionName, cast(SUM(TVC) / 100 as decimal(18, 2)) desc ";
            }

          return await _context.RunningWorkDelayDbSet
       .FromSqlRaw(query)
       .ToListAsync();
        }



        [HttpGet("RunningDelayWorksDetails")]
        public async Task<ActionResult<IEnumerable<RunningWorkDetailsDelnTimeDTO>>> RunningDelayWorksDetails(string delayTime,string parameter, string divisionid, string districtid, string mainschemeid, string contractorid)
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

            if (delayTime == "Delay")
            {
                //delay parameter
                if (parameter == "SixMonth")
                {
                    whereClause += @" and 
 (case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>180 
 then  1 else 0 end )=1 ";
                }
                if (parameter == "Between3_6")
                {
                    whereClause += @" and 
 (case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>90
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=180 
 then  1 else 0 end )=1 ";
                }
                if (parameter == "Between1_3")
                {
                    whereClause += @" and 
 (case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>=1
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=90 
 then  1 else 0 end )=1 ";
                }
             

                orderbyclause = "  order by (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) ) desc ";
            }
            else
            {
                if (parameter == "TimeValid")
                {
                    whereClause += @" and 
 (case when  (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=0
 then  1 else 0 end )=1 ";
                }

                orderbyclause = " order by a.WrokOrderDT  ";
            }




            string query = @" 
select 

 w.LetterNo, w.work_id, msc.Name as Head,ap.login_name as Approver,
 wt.type_name ,
  d.DBStart_Name_En as District,isnull(b.Block_Name_En,'') as blockname,
  h.NAME_ENG+' - '+s.SWName  as work,

  cast(w.AaAmt as decimal(18,2)) as AAAMT
 ,convert(varchar, w.AADate,103) as AADate,
   cast(w.TSAmount as decimal(18,2)) as TSAMT
 ,convert(varchar, w.TSDate,103) as TSDate 
 ,a.AcceptanceLetterRefNo ,convert(varchar, a.AcceptLetterDT,103) as AcceptLetterDT ,
dv.DivName_En,agd.DivisionID
,case when a.TotalAmountOfContract is not null then (case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end) else 0 end as TVC
,  cast(isnull(totalpaid,0) as decimal(18,2)) as PaidTillLacs
,cast(isnull(GrossPending,0) as decimal(18,2)) as GrossPendinglacs
,convert(varchar,a.WrokOrderDT,105) as WorkorderDT,
cast(a.TimeAllowed as int) as TimeAllowed
, CONVERT(varchar,case when a.TimeAllowed>6 then DATEADD(Month, a.TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15, DATEADD(Month, a.TimeAllowed, a.WrokOrderDT))  end,105) DueDTTimePerAdded
, cast((DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) ) as int) as DelayDays
,case when t.iszonal='Y' then 'Zonal' else 'Works' end as TType, a.TenderReference
,wpp.ParentProgress as LProgress,convert(varchar,pw.ProgressDT,105) as ProgressDT,isnull(pw.Remarks,'-')+' '+isnull(wpr.Remarks,'-') as Remarks ,
 rtrim(ltrim(isnull(e.Name,'Not Alloted'))) as subengname
,rtrim(ltrim(isnull(ae.Name,'Not Alloted'))) as AEName
,rtrim(ltrim(pldr.delayreason)) as delayreason
,case when plEC.expcompdt is not null then convert(varchar,plEC.expcompdt,105)  else '' end as expcompdt 
,cnt.Contractorid as CID,isnull(cnt.Title,'')+cnt.EnglishName as ContractorNAme,cnt.RegType,cnt.Class,cnt.EnglishAddress,cnt.MobNo 

,case when  (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=0
 then  1 else 0 end as TimeValid
 , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>180 
 then  1 else 0 end as MorethanSix
  , case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>90
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=180 
 then  1 else 0 end as exce3_6Month
 ,case when (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )>=1
  and (DATEDIFF(DAY,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT) else  DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT)) end ),GETDATE()) )<=90 
 then  1 else 0 end as exce1_3Month
,convert(varchar, a.DateOfSanction,103) as DateOfSanction,convert(varchar, a.DateOfIssueNIT,103) as DateOfIssueNIT
from  WorkMaster w
 left outer join CGMSC_Team  e on e.Emp_Code= w.SubeEmpcode
  left outer join CGMSC_Team  ae on ae.Emp_Code= w.AEEmpcode
left outer join SWDetails s on s.SWId= w.work_description_id
inner join Login ap on ap.Login_id= w.approved_by
inner join WorkType wt on wt.type_id= w.work_type
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID

inner join  AgreementDetails a on a.work_id=w.work_id
inner join  ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(a.Contractorid))
inner join Districts d on d.District_ID  =w.worklocationdist_id
 inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
 left outer join BlocksMaster b on cast(b.Block_ID as int)=cast(h.BLOCK_ID as int) and b.District_ID=d.District_ID
inner join division dv on dv.div_id=d.div_id
 left outer join MasTender t on t.tenderid= a.tenderid

inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint) and agd.DivisionID not in ('D1032')
inner join 
(
select max(SR) as sr,Work_id  from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress='Y'   and dash.did =5001
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
left outer join WorkProgressLevel_Remarks wpr on wpr.RID=pw.remarkid
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=w.work_id
left outer join 
(
select w.work_id, cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
)   up  on up.work_id=w.work_id

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

where  w.isdeleted is null  and w.MainSchemeID not in (121) 
 and dash.did =5001  " + whereClause + @"
"+orderbyclause+@"  ";


            return await _context.RunningWorkDetailsDelnTimeDbSet
           .FromSqlRaw(query)
           .ToListAsync();
        }



        }
}

