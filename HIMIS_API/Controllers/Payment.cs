using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.Payment;
using HIMIS_API.Models.TS;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Payment : Controller
    {
        private readonly DbContextData _context;
        public Payment(DbContextData context)
        {
            _context = context;
        }
    
        [HttpGet("PaidSummary")]
        public async Task<ActionResult<IEnumerable<PaidSummaryDTO>>> PaidSummary(string RPType,string divisionid,string districtid,string mainschemeid, string fromdt, string todt)
        {
            string? whereClause = "";
            string whereClauseDivison = "";
            if (divisionid != "0")
            {

                whereClause += $" and agd.DivisionID  = '{divisionid}'";

            }


            if (districtid != "0")
            {

                whereClause += $" and d.District_ID  = '{districtid}'";

            }


            if (RPType == "Division")
            {
                if (mainschemeid != "0")
                {

                    whereClauseDivison += $" and w.MainSchemeID  = '{mainschemeid}'";

                }

            }
            else
            {

                if (mainschemeid != "0")
                {

                    whereClause += $" and w.MainSchemeID  = '{mainschemeid}'";

                }
            }
           
            if (fromdt != "0")
            {

                if (RPType == "Division")
                {
                    if (todt != "0")
                    {
                        whereClauseDivison += $" and b.ChequeDT between   '{fromdt}' and '{todt}' ";

                    }
                    else
                    {
                        whereClauseDivison += $" and b.ChequeDT between   '{fromdt}' and getdate() ";
                    }


                    
                        if (mainschemeid != "0")
                        {

                        whereClauseDivison += $" and w.MainSchemeID  = '{mainschemeid}'";

                        }

                    
                }
                else
                {
                    if (todt != "0")
                    {
                        whereClause += $" and b.ChequeDT between   '{fromdt}' and '{todt}' ";

                    }
                    else
                    {
                        whereClause += $" and b.ChequeDT between   '{fromdt}' and getdate() ";
                    }
                }

            }

            string query = "";
            if (RPType == "GTotal")
            {
                //divisionwise
                //                query = $@" 
                //select cast(1 as varchar) as ID,'Total' as Name,count(distinct work_code) as NoofWorks,cast((isnull(sum(totalExp),0)+isnull(sum(gstcharge),0))/10000000 as decimal (18,2)) as GrossPaidcr
                //,round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaysSinceMeasurement
                //from 
                //(
                //select work_code , cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) as totalExp

                //,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)) gstcharge
                //,datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
                //from BillPayment b
                //where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
                //" + whereClause + @" 	
                //group by b.work_code,b.MesurementDT,b.ChequeDT
                //) b  ";

                query = $@" 
select cast(1 as varchar) as ID,'Total' as Name,count(distinct work_code) as NoofWorks,cast((isnull(sum(totalExp),0)+isnull(sum(gstcharge),0))/10000000 as decimal (18,2)) as GrossPaidcr
,round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaysSinceMeasurement
from 
(
select work_code , cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) as totalExp

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)) gstcharge
,datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
from BillPayment b
inner join  WorkMaster w on w.work_id=b.work_code
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
" + whereClause + @" 	
group by b.work_code,b.MesurementDT,b.ChequeDT
) b  ";
            }

            if (RPType == "Division")
            {
                //divisionwise
                query = $@" select ID,Name,count(distinct work_code) as NoofWorks,cast(isnull(sum(GrossPaidcr),0)/10000000 as decimal (18,2)) as GrossPaidcr
,round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaysSinceMeasurement
from 
(
select agd.DivisionID as ID,dv.DivName_En as Name,work_code,
cast((isnull(totalExp,0)+isnull(gstcharge,0)) as decimal (18,2)) as GrossPaidcr
,daysSinceMeasurement
 from agencydivisionmaster  agd 
 inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)

left outer join
(
select b.divisionid,work_code , cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) as totalExp

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)) gstcharge
,datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
from BillPayment b
inner join  WorkMaster w on w.work_id=b.work_code
where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
  	" + whereClauseDivison + @"
group by b.divisionid,b.work_code,b.MesurementDT,b.ChequeDT
) ex on ex.divisionid=agd.DivisionID
where  agd.DivisionID not in ('D1032')
"+ whereClause + @"
) b
group by ID,Name
order by sum(GrossPaidcr) desc ";
            }
            if (RPType == "Scheme")
            {
                //Scheme
                //                query = $@"  select ID,Name,count(distinct work_code) as NoofWorks,cast(isnull(sum(totalExp),0)/10000000 as decimal (18,2)) as GrossPaidcr
                //,round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaysSinceMeasurement
                //from 
                //(
                //select cast(msc.MainSchemeID as varchar) ID,msc.Name,b.work_code,
                // cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))as decimal (18,2))+
                //(cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2))) totalExp
                //,datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
                //from BillPayment b
                //inner join WorkMaster w on w.work_id=b.work_code
                //inner join MainSchemes msc on msc.MainSchemeID=w.MainSchemeID
                //where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
                //"+ whereClause + @" 	
                //group by msc.MainSchemeID,msc.Name,b.work_code,b.MesurementDT,b.ChequeDT
                //) b
                //group by ID,Name
                //order by sum(totalExp)desc ";


                query = $@"  select ID,Name,count(distinct work_code) as NoofWorks,cast(isnull(sum(totalExp),0)/10000000 as decimal (18,2)) as GrossPaidcr
,round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaysSinceMeasurement
from 
(
select cast(msc.MainSchemeID as varchar) ID,msc.Name,b.work_code,
 cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))as decimal (18,2))+
(cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2))) totalExp
,datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
from BillPayment b
inner join  WorkMaster w on w.work_id=b.work_code
inner join Districts d on d.District_ID  =w.worklocationdist_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join division dv on dv.div_id=d.div_id
inner join agencydivisionmaster  agd on cast(agd.divisionname as  bigint)=cast(dv.div_id as bigint)  and agd.DivisionID not in ('D1032')
where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
" + whereClause + @" 	
group by msc.MainSchemeID,msc.Name,b.work_code,b.MesurementDT,b.ChequeDT
) b
group by ID,Name
order by sum(totalExp)desc ";

            }
            return await _context.PaidSummaryDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("UnPaidSummary")]
        public async Task<ActionResult<IEnumerable<UnPaidSummary>>> UnPaidSummary(string RPType, string divisionid, string districtid, string mainschemeid)
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
            if (RPType == "GTotal")
            {
                //Total
                query = $@" select cast(1 as varchar) as ID,'Total' as Name,count(distinct work_code) as  NoofWorks,cast(sum(GrossAmtNew)/10000000 as decimal(18,2)) as Unpaidcr,
round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaySinceM
from 
(
select b.work_code , cast(PaidGrossAmount as decimal(18,2)) as PaidGross,EEGrossAmount,AEGrossAmount,GrossAmount,SEGrossAmount
,datediff(dd,b.MesurementDT,getdate() ) daysSinceMeasurement,b.WorkStatus
,case when left(b.WorkStatus,1)='D' then 'EE' else cg.Name end as EngName
,cg.Designation,case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end GrossAmtNew

from BillPayment b
inner join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=b.work_code
inner join WorkMaster w on w.work_id=b.work_code
inner join MainSchemes msc on msc.MainSchemeID=w.MainSchemeID
inner join AgencyDivisionMaster ag on ag.DivisionID=b.WorkStatus
inner join AgencyDivisionMaster agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join CGMSC_Team cg on cg.Emp_Code=ag.Empcode
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0 " + whereClause+@"
) b ";
            }

            if (RPType == "Division")
            {
                //divisionwise
                query = $@" select ID,Name,count(distinct work_code) as  NoofWorks,cast(sum(GrossAmtNew)/10000000 as decimal(18,2)) as Unpaidcr,
round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaySinceM
from 
(
select agd.DivisionID as ID,dv.DivName_En as Name,b.work_code , cast(PaidGrossAmount as decimal(18,2)) as PaidGross,EEGrossAmount,AEGrossAmount,GrossAmount,SEGrossAmount
,datediff(dd,b.MesurementDT,getdate() ) daysSinceMeasurement,b.WorkStatus
,case when left(b.WorkStatus,1)='D' then 'EE' else cg.Name end as EngName
,cg.Designation,case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end GrossAmtNew

from BillPayment b
inner join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=b.work_code
inner join WorkMaster w on w.work_id=b.work_code
inner join MainSchemes msc on msc.MainSchemeID=w.MainSchemeID
inner join AgencyDivisionMaster ag on ag.DivisionID=b.WorkStatus
inner join AgencyDivisionMaster agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
 inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join CGMSC_Team cg on cg.Emp_Code=ag.Empcode
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
" + whereClause+@"
) b
group by ID,Name
order by sum(GrossAmtNew) desc 
 ";
            }

            if (RPType == "Designation")
            {
                //--Designationwise
                query = $@"  
select Designation as ID,Designation as Name,count(distinct work_code) as  NoofWorks,cast(sum(GrossAmtNew)/10000000 as decimal(18,2)) as Unpaidcr,
round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaySinceM
from 
(
select agd.DivisionID as ID,dv.DivName_En as Name,b.work_code , cast(PaidGrossAmount as decimal(18,2)) as PaidGross,EEGrossAmount,AEGrossAmount,GrossAmount,SEGrossAmount
,datediff(dd,b.MesurementDT,getdate() ) daysSinceMeasurement,b.WorkStatus
,case when left(b.WorkStatus,1)='D' then 'EE' else cg.Name end as EngName
,case when left(b.WorkStatus,1)='D' then 'Executive Engineer' else cg.Designation end as Designation ,case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end GrossAmtNew

from BillPayment b
inner join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=b.work_code
inner join WorkMaster w on w.work_id=b.work_code
inner join MainSchemes msc on msc.MainSchemeID=w.MainSchemeID
inner join AgencyDivisionMaster ag on ag.DivisionID=b.WorkStatus
inner join AgencyDivisionMaster agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
 inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join CGMSC_Team cg on cg.Emp_Code=ag.Empcode
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
" + whereClause+@"
) b
group by Designation
order by sum(GrossAmtNew) desc ";

            }

            if (RPType == "Scheme")
            {
                //Scheme
                query = $@"  select ID,Name,count(distinct work_code) as  NoofWorks,cast(sum(GrossAmtNew)/10000000 as decimal(18,2)) as Unpaidcr,
round(sum(daysSinceMeasurement)/count(distinct work_code),0) as AvgDaySinceM
from 
(
select cast(msc.MainSchemeID as varchar) ID,msc.Name,work_code , cast(PaidGrossAmount as decimal(18,2)) as PaidGross,EEGrossAmount,AEGrossAmount,GrossAmount,SEGrossAmount
,datediff(dd,b.MesurementDT,getdate() ) daysSinceMeasurement,b.WorkStatus
,case when left(b.WorkStatus,1)='D' then 'EE' else cg.Name end as EngName
,cg.Designation,case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end GrossAmtNew

from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
inner join MainSchemes msc on msc.MainSchemeID=w.MainSchemeID
inner join AgencyDivisionMaster ag on ag.DivisionID=b.WorkStatus
inner join AgencyDivisionMaster agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
 inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join CGMSC_Team cg on cg.Emp_Code=ag.Empcode
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0 " + whereClause+@"
) b
group by ID,Name
order by sum(GrossAmtNew) desc ";

            }
            return await _context.UnPaidSummaryDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }


        [HttpGet("PaidDetails")]

        public async Task<ActionResult<IEnumerable<PaidDetailsDTO>>> PaidDetails(string divisionId, string mainSchemeId, string distid,string fromdt, string todt)
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
            if (fromdt != "0")
            {
                if (todt != "0")
                {
                    whereClause += $" and b.ChequeDT between   '{fromdt}' and '{todt}' ";

                }
                else
                {
                    whereClause += $" and b.ChequeDT between   '{fromdt}' and getdate() ";
                }


            }

            string query = "";

            query = $@" select w.work_id,msc.name as Head, dv.divname_en as Division,dis.DBStart_Name_En as District, d.NAME_ENG+' - '+s.SWName as workname,
convert(varchar,a.WrokOrderDT,105) as WrokOrderDT,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract,
b.billno,b.agrbillstatus,
cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2)) GrossPaid
,convert(varchar,b.MesurementDT,105) as MesurementDT,convert(varchar,b.billdate,105) as billdate ,convert(varchar,b.ChequeDT,105) as ChequeDT ,
datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
,isnull(totalpaid,0) as TotalPaidTillinlac
from BillPayment b
inner join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=b.work_code
inner join WorkMaster w on w.work_id=b.work_code
inner join SWDetails s on s.SWId=w.work_description_id 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
  inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  AgreementDetails a on a.work_id= w.work_id
where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
"+ whereClause + @"
order by b.ChequeDT ";
            
            return await _context.PaidDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }


        [HttpGet("UnPaidDetails")]

        public async Task<ActionResult<IEnumerable<UnPaidDetailsDTO>>> UnPaidDetails(string divisionId, string mainSchemeId, string distid,string designame,string OfficerID)
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
            if (OfficerID != "0")
            {

                whereClause += $" and  ag.DivisionID = '{OfficerID}'";
            }
            if (designame != "0")
            {

                whereClause += $" and  rtrim(ltrim(cg.Designation)) = '{designame}'";
            }


            //if (fromdt != "0")
            //{
            //    if (todt != "0")
            //    {
            //        whereClause += $" and b.ChequeDT between   '{fromdt}' and '{todt}' ";

            //    }
            //    else
            //    {
            //        whereClause += $" and b.ChequeDT between   '{fromdt}' and getdate() ";
            //    }


            //}

            string query = "";

            query = $@" select w.work_id,msc.name as Head, dv.divname_en as Division,dis.DBStart_Name_En as District, d.NAME_ENG+' - '+s.SWName as workname,
convert(varchar,a.WrokOrderDT,105) as WrokOrderDT,
case when a.formt='B' then Round(cast(Round(a.TotalAmountOfContract/100000,2) as decimal(18,2)),2) else    Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) end TotalAmountOfContract,
b.billno,b.agrbillstatus ,convert(varchar,b.MesurementDT,105) as MesurementDT,convert(varchar,b.billdate,105) as billdate ,convert(varchar,b.ChequeDT,105) as ChequeDT 
,datediff(dd,b.MesurementDT,getdate() ) daysSinceMeasurement,b.WorkStatus
,case when left(b.WorkStatus,1)='D' then 'EE' else rtrim(ltrim(cg.Name)) end as EngName,rtrim(ltrim(cg.Designation)) as Designation

,cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossAmtNew


 ,isnull(totalpaid,0) as TotalPaidTillinlac
from BillPayment b
left outer join 
(
select work_code,sum(cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2))) as totalpaid from BillPayment 
where Ispaid='Y' and isnull(ispass,'N') ='P' and ChequeNo is not null 
group by work_code
) bt on bt.work_code=b.work_code

inner join WorkMaster w on w.work_id=b.work_code

inner join SWDetails s on s.SWId=w.work_description_id 
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
  inner join agencydivisionmaster  agd on agd.DivisionID=w.AllotedDivisionID and agd.DivisionID not in ('D1032')
inner join division dv on cast(dv.div_id as bigint) =cast(agd.divisionname as  bigint)
inner join  AgreementDetails a on a.work_id= w.work_id
inner join AgencyDivisionMaster ag on ag.DivisionID=b.WorkStatus
inner join CGMSC_Team cg on cg.Emp_Code=ag.Empcode
where 1=1 and b.BillDate>'01-Apr-2022' and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0

" + whereClause + @"
order by b.MesurementDT desc ";

            return await _context.UnPaidDetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }




        [HttpGet("WorkBillStatus")]

        public async Task<ActionResult<IEnumerable<WorkBillStatusDTO>>> WorkBillStatus(string workid)
        {
           

            string query = @" select w.work_id,b.billno,b.agrbillstatus,convert(varchar,b.MesurementDT,105) as MesurementDT,convert(varchar,b.billdate,105) as billdate,
cast(Round((cast(((cast(PaidGrossAmount as decimal(18,2)))+Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)) as decimal (18,2)) +cast(((Round((cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100) as decimal (18,2)))/100000,2)as decimal(18,2)) GrossPaid
,convert(varchar,b.ChequeDT,105) as ChequeDT ,
datediff(dd,b.MesurementDT,b.ChequeDT ) daysSinceMeasurement
, rtrim(ltrim(b.mbno)) mbno ,rtrim(ltrim(b.billmbno)) as billmbno,'Paid' as BillStatus
,b.ChequeNo
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
inner join  AgreementDetails a on a.work_id= w.work_id
where b.Ispaid='Y' and isnull(ispass,'N') ='P' and b.ChequeNo is not null 
and w.work_id='" + workid + @"'

union all

select w.work_id,b.billno,b.agrbillstatus ,convert(varchar,b.MesurementDT,105) as MesurementDT,convert(varchar,b.billdate,105) as billdate ,

cast(round((case when PaidGrossAmount is null 
then case when EEGrossAmount is null
 then case when AEGrossAmount is null 
 then GrossAmount else AEGrossAmount end 
 else EEGrossAmount end 
 else PaidGrossAmount end)/100000,2) as decimal(18,2)) GrossPending,
 'UnPaid' as ChequeDT 
,datediff(dd,b.MesurementDT,getdate() ) daysSinceMeasurement
, rtrim(ltrim(b.mbno)) mbno ,rtrim(ltrim(b.billmbno)) as billmbno
,case when left(b.WorkStatus,1)='D' then 'EE'  else case when a.AgencyID is not  null then a.AgencyName else    rtrim(ltrim(cg.Name)) end end as EngName
,isnull(b.ChequeNo,'NA') as ChequeNo
from BillPayment b
inner join WorkMaster w on w.work_id=b.work_code
left outer  join AgencyDivisionMaster ag on ag.DivisionID=b.WorkStatus
left outer join AgencyMaster a on a.AgencyID=b.WorkStatus
left outer  join CGMSC_Team cg on cg.Emp_Code=ag.Empcode
where 1=1 and b.isoldPaid is null   and isnull(b.Ispaid,'N')='N' 
and b.ChequeNo is  null  and w.IsDeleted is null and GrossAmount>0
and w.work_id='" + workid + @"'
order by b.billno";
            
                  return await _context.WorkBillStatusDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }





     }
}

