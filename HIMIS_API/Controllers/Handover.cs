using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.Handover;
using HIMIS_API.Models.LandIssue;
using HIMIS_API.Models.WorkOrder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Handover : Controller
    {
        private readonly DbContextData _context;
        public Handover(DbContextData context)
        {
            _context = context;
        }
        [HttpGet("HandoverAbstract")]
        public async Task<ActionResult<IEnumerable<HandOverAbstractDTO>>> HandoverAbstract(string RPType,string dashid,string divisionid,string districtid,string SWId, string fromdt,string todt,string mainSchemeId)
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
            if (SWId != "0")
            {

                whereClause += $" and s.SWId  = '{SWId}'";

            }

            if (mainSchemeId != "0")
            {

                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }

            if (fromdt != "0")
            {
                if (todt != "0")
                {
                    whereClause += $" and ProgressDT between   '{fromdt}' and '{todt}' ";

                }
                else
                {
                    whereClause += $" and ProgressDT between   '{fromdt}' and getdate() ";
                }
              

            }
            string query = "";
            if (RPType == "Total")
            {
                query = $@" select DivisionID as ID,DivName_En as Name,count(work_id) as TotalWorks,
cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr ,
round((sum(daytakensinceworkorder)/count(work_id))/30 ,0) as AvgMonthTaken
from 
(

select w.work_id,dv.DivName_En,agd.DivisionID,ProgressDT,Datediff(day,WrokOrderDT,ProgressDT ) daytakensinceworkorder
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end TVC
from  WorkMaster w
inner join SWDetails s on s.SWId= w.work_description_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
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
where NewProgress='Y' and dash.did=" +dashid+@"
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where  w.isdeleted is null  and w.MainSchemeID not in (121) and dash.did= " +dashid+@" "+ whereClause + @"
) b
group by DivName_En,DivisionID
order by  count(work_id) desc  ";
            }
            if (RPType == "Scheme")
            {
                //Scheme
                query = $@" select cast(MainSchemeID as varchar) as ID,Name,count(work_id) as TotalWorks,
cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr,round((sum(daytakensinceworkorder)/count(work_id))/30 ,0) as AvgMonthTaken
from 
(

select w.work_id,msc.name,w.MainSchemeID,ProgressDT,Datediff(day,WrokOrderDT,ProgressDT ) daytakensinceworkorder
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end TVC
from  WorkMaster w
inner join SWDetails s on s.SWId= w.work_description_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
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
where NewProgress='Y' and dash.did=" +dashid+@"
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where  w.isdeleted is null  and w.MainSchemeID not in (121) and dash.did=" +dashid+@" " + whereClause + @"
) b
group by Name,MainSchemeID
order by  count(work_id) desc ";
            }
            

            if (RPType == "District")
            {
               //districtwise
                query = $@" select cast(District_ID as varchar) as ID,dbstart_name_en as Name,count(work_id) as TotalWorks,
cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr,round((sum(daytakensinceworkorder)/count(work_id))/30 ,0) as AvgMonthTaken
from 
(

select w.work_id,d.dbstart_name_en,d.District_ID,ProgressDT,Datediff(day,WrokOrderDT,ProgressDT ) daytakensinceworkorder
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end TVC
from  WorkMaster w
inner join SWDetails s on s.SWId= w.work_description_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
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
where NewProgress='Y' and dash.did=" +dashid+ @"
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where  w.isdeleted is null  and w.MainSchemeID not in (121) and dash.did="+dashid+@" " + whereClause + @"
) b
group by dbstart_name_en,District_ID
order by  count(work_id) desc ";
            }

            if (RPType == "WorkType")
            {
                //SwType
                query = $@" select cast(SWId as varchar) as  ID,SWName as Name,count(work_id) as TotalWorks,
cast(SUM(TVC)/100 as decimal(18,2)) as TVCValuecr,round((sum(daytakensinceworkorder)/count(work_id))/30 ,0) as AvgMonthTaken
from 
(

select w.work_id,ht.TYPE_id,ht.details_eng,ProgressDT,Datediff(day,WrokOrderDT,ProgressDT ) daytakensinceworkorder
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end TVC
,s.SWName,s.SWId
from  WorkMaster w
inner join WorkType wt on wt.type_id= w.work_type
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join SWDetails s on s.SWId= w.work_description_id
inner join dhrsHealthCenter h on h.HC_ID  = w.worklocation_id
inner join dhrsHealthCentreType ht on ht.TYPE_id=h.TYPE
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
where NewProgress='Y' and dash.did=" +dashid+ @"
group by Work_id 
) p on p.Work_id=w.work_id
inner join WorkPhysicalProgress pw on pw.SR=p.sr
inner join WorkLevelParent wpp on wpp.ppid=pw.ppid
inner join WorkLevelParentGroup wpg on wpg.PGroupID=wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where  w.isdeleted is null  and w.MainSchemeID not in (121) and dash.did="+dashid+@" " + whereClause + @"
--and wt.type_id=1

) b
group by SWId,SWName
order by  round(sum(TVC)/100,2) desc ";
            }

            return await _context.HandOverAbstractDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }

        [HttpGet("getHandoverDetails")]

        public async Task<ActionResult<IEnumerable<WorkorderpendingdetailsDTO>>> getHandoverDetails(string dashid,string divisionId, string mainSchemeId, string distid,string SWId,string fromdt,string todt)
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
            if (SWId != "0")
            {

                whereClause += $" and s.SWId  = '{SWId}'";

            }

            if (fromdt != "0")
            {
                if (todt != "0")
                {
                    whereClause += $" and ProgressDT between   '{fromdt}' and '{todt}' ";

                }
                else
                {
                    whereClause += $" and ProgressDT between   '{fromdt}' and getdate() ";
                }


            }

            string query = "";

            query = $@" select w.LetterNo, w.work_id, msc.Name as Head,ap.login_name as Approver,
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
inner join AgreementDetails  n on n.work_id= w.work_id
 left outer join MasTender t on t.tenderid= n.tenderid

inner join  ContractMaster cnt on rtrim(ltrim(cnt.Contractorid))=ltrim(rtrim(n.Contractorid))

inner join 
(
select p.ppid, p.WorkLevel as level_id ,
isnull(wpp.ParentProgress,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,r.RID,p.Remarkid,r.Remarks,wpg.GroupName,wpg.PGroupID,p.DelayReason
from  WorkPhysicalProgress p
inner join WorkLevelParent wpp on wpp.ppid=p.ppid
left outer join  WorkProgressLevel_Remarks r on r.RID= p.remarkid
inner join WorkLevelParentGroup wpg on wpg.PGroupID= wpp.PGroupID
inner join WorkLevelParentDash dash on dash.did=wpg.did
where NewProgress = 'Y' and dash.did=" +dashid+@"
group by p.Work_id,wpp.ParentProgress,ProgressDT,
 p.Remarks,p.WorkLevel,p.ppid,r.Remarks,p.Remarkid,r.RID,p.Remarkid,wpg.GroupName,wpg.PGroupID,p.DelayReason
) pl on pl.Work_id=w.work_id
where 1=1 and w.isdeleted is null  and w.MainSchemeID not in (121)    " + whereClause + @"
 order by pl.LProgress desc ";
            
            return await _context.WorkorderpendingdetailsDbSet
            .FromSqlRaw(query)
            .ToListAsync();

        }




        }
}

