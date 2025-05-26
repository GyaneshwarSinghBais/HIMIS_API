using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Services.ProgressServices.Interface;
using HIMIS_API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace HIMIS_API.Services.ProgressServices.Implementation
{
    public class MainSchemeService : IMainSchemeService
    {
        private readonly DbContextData _context;
        public MainSchemeService(DbContextData context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MasValueParaDTO>> GetValuePara()
        {
            string qry = @"select ID ,Valuepara  from MasWorkValuePara order by ID ";

            var myList = await _context.MasValueParaDbSet
                .FromSqlInterpolated(FormattableStringFactory.Create(qry))
                .ToListAsync();

            return myList;
        }


        public async Task<IEnumerable<MasContractorDTO>> MasContractor()
        {
            string qry = @"select rtrim(ltrim(ContractorID)) ContractorID , EnglishName+'('+cast(COUNT(work_id) AS  VARCHAR)+')-'+cast(cast(SUM(TOTALAMOUNTOFCONTRACT) as decimal(18,0))AS VARCHAR) as contractorname
        from
        (
select w.work_id, c.Contractorid, c.EnglishName,case when a.FormT= 'B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end as TOTALAMOUNTOFCONTRACT from AgreementDetails a
inner join WorkMaster w on w.work_id=a.work_id
inner join ContractMaster c on c.Contractorid= a.ContractorID
where 1=1 
and w.IsDeleted is null
and w.MainSchemeID not in (121)
) A
GROUP BY Contractorid, EnglishName
ORDER BY SUM(TOTALAMOUNTOFCONTRACT) DESC  ";
  var myList = await _context.MasContractorDbSet
                .FromSqlInterpolated(FormattableStringFactory.Create(qry))
                .ToListAsync();

            return myList;
        }






        public async Task<IEnumerable<MainSchemeDTO>> GetMainSchemeAsync(bool isall)
        {
            string qry = "";
            if (isall)
            {
                 qry = @" select MainSchemeID,Name
						  from 
						  (
						  select 0 as MainSchemeID,'All' as Name
						  union all

						  SELECT msc.MainSchemeID, msc.Name+'('+ISNULL(w.d,0)+')' as Name FROM MainSchemes msc
                       LEFT OUTER JOIN
                       (
                           SELECT CAST(COUNT(w.work_id) as varchar) as d, MainSchemeID
                           FROM WorkMaster w
                           WHERE 1=1 AND  w.MainSchemeID not in (121) and w.IsDeleted IS NULL
                           GROUP BY MainSchemeID
                       ) w ON w.MainSchemeID=msc.MainSchemeID
                       WHERE 1=1 AND ISNULL(w.d,0) > 0
					   )a ";
            }
            else
            {
                qry = @"SELECT msc.MainSchemeID, msc.Name+'('+ISNULL(w.d,0)+')' as Name FROM MainSchemes msc
                       LEFT OUTER JOIN
                       (
                           SELECT CAST(COUNT(w.work_id) as varchar) as d, MainSchemeID
                           FROM WorkMaster w
                           WHERE 1=1 AND w.IsDeleted IS NULL AND  w.MainSchemeID not in (121)
                           GROUP BY MainSchemeID
                       ) w ON w.MainSchemeID=msc.MainSchemeID
                       WHERE 1=1 AND ISNULL(w.d,0) > 0
                       ORDER BY w.d DESC";

            }

            var myList = await _context.MainSchemeDbSet
                .FromSqlInterpolated(FormattableStringFactory.Create(qry))
                .ToListAsync();

            return myList;
        }




        public async Task<IEnumerable<MainSchemeDTO>> GetWorkOrderHead(string Type, string divisionid)
        {
            string whereClause = "";
            if (Type == "WorkOrder")
            {
                whereClause += $" and a.WrokOrderDT is null  ";
            }
            if (divisionid != "0")
            {
                whereClause += $" and a.DivisionID = '{divisionid}'";
            }

            string query = $@"  select 0 as MainSchemeID,'All' as Name
 union all
 

select msc.MainSchemeID , msc.Name +'-'+cast(count(distinct a.work_id) as varchar) as  Name
from  WorkMaster w
inner join MainSchemes msc on msc.MainSchemeID=w.MainSchemeID
inner join  AgreementDetails a on a.work_id= w.work_id
where a.AcceptLetterDT>'01-01-2022' and w.IsDeleted is null 
and w.MainSchemeID not in (121)  {whereClause}
group by  msc.MainSchemeID,msc.Name
 order by  MainSchemeID ";
            var myList = await _context.MainSchemeDbSet
                   .FromSqlInterpolated(FormattableStringFactory.Create(query))
                   .ToListAsync();
            return myList;
        }

        public async Task<IEnumerable<DivisionNameDTO>> GetDivisionsAsync(bool isall,string divisionId)
        {
            string whereClause = divisionId == "0" ? "" : $" and a.DivisionID = '{divisionId}'";
            string query = "";
            if (isall)
            {
                 query = $@" 
		                 select 0 as Div_Id,'All' as DivName_En, '0' as DivisionID
						  union all
                          SELECT Div_Id, DivName_En, a.DivisionID FROM Division d
                          INNER JOIN AgencyDivisionMaster a ON a.DivisionName = d.Div_Id
                          WHERE 1 = 1 and a.DivisionId != 'D1032' {whereClause}
                          ORDER BY DivName_En";
            }
            else
            {
                 query = $@"SELECT Div_Id, DivName_En, a.DivisionID FROM Division d
                          INNER JOIN AgencyDivisionMaster a ON a.DivisionName = d.Div_Id
                          WHERE 1 = 1 and a.DivisionId != 'D1032' {whereClause}
                          ORDER BY DivName_En";

            }

            var divisions = await _context.DivisionNameDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return divisions;
        }

        public async Task<IEnumerable<DistrictNameDTO>> GetDistrictsAsync(bool isall,string divisionId)
        {
            FacOperation ob = new FacOperation(_context);

            string whereClause = divisionId != "0" ? $" and Div_Id = {ob.getDivID(divisionId)}" : "";
            string query = "";
            if (isall)
            {
                 query = $@" select 0 as District_ID,'All' as DistrictName, 0 as Div_Id
						  union all
                          SELECT District_ID, DBStart_Name_En as DistrictName, Div_Id FROM Districts
                          WHERE 1 = 1 {whereClause}
                          ORDER BY DistrictName ";
            }
            else
            {
                 query = $@"SELECT District_ID, DBStart_Name_En as DistrictName, Div_Id FROM Districts
                          WHERE 1 = 1 {whereClause}
                          ORDER BY DBStart_Name_En";
            }

                return await _context.DistrictNameDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProgressLevelDTO>> GetProgressLevelsAsync(bool isall,string ppid)
        {
            string whereClause = ppid != "0" ? $" and wl.PPID = {ppid}" : "";

            string query = "";
            if (isall)
            {
                 query = $@"
  select 0 as PPID,'All' as PPStatus,0 as PPOrderid
						  union all
SELECT PPID, parentprogress as PPStatus,wl.PPOrderid FROM WorkLevelParent wl
                          INNER JOIN WorkLevelStatus ws ON ws.wlstatusid = wl.SNHID
                          WHERE Isvisible = 'Y' {whereClause}
                         order by PPOrderid ";
            }
            else
            {
                 query = $@" SELECT PPID, parentprogress as PPStatus,wl.PPOrderid FROM WorkLevelParent wl
                          INNER JOIN WorkLevelStatus ws ON ws.wlstatusid = wl.SNHID
                          WHERE Isvisible = 'Y' { whereClause}
                order by PPOrderid ";
            }

            return await _context.ProgressLevelDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }

        public async Task<IEnumerable<HealthCentreDTO>> GetHealthCentresAsync(string distId, string divisionId)
        {
            string whereClause = "";
            if (divisionId != "0")
            {
                whereClause += $" and ag.DivisionID = '{divisionId}'";
            }
            if (distId != "0")
            {
                whereClause += $" and dis.District_ID = '{distId}'";
            }

            string query = $@"SELECT ty.Type_ID, ty.DETAILS_ENG, COUNT(w1.work_id) AS nosworks FROM dhrsHealthCentreType ty
                          INNER JOIN dhrsHealthCenter d ON ty.Type_ID = d.TYPE
                          INNER JOIN WorkMaster w1 ON w1.worklocation_id = d.HC_ID
                          INNER JOIN Districts dis ON dis.DISTRICT_ID = w1.worklocationdist_id
                          INNER JOIN Division dv ON dv.Div_Id = dis.Div_Id
                          INNER JOIN AgencyDivisionMaster ag ON ag.DivisionName = dv.Div_Id
                          WHERE 1 = 1 AND w1.IsDeleted IS NULL {whereClause}
                          GROUP BY ty.DETAILS_ENG, ty.Type_ID
                          ORDER BY ty.DETAILS_ENG";

            return await _context.HealthCentreDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }

        public async Task<IEnumerable<WOPendingDivisionDTO>>GetWorkOrderPending( string divisionId)
        {
            string whereClause = "";
            if (divisionId != "0")
            {
                whereClause += $" and a.DivisionID = '{divisionId}'";
            }
            string query = $@"  select '0' as Divisionid,'All' as details
 union all
select a.Divisionid, dv.DivName_En +'-'+cast(count(distinct a.work_id) as varchar) as details
from  WorkMaster w
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID= a.tenderid
inner join  ContractMaster c on c.Contractorid= a.ContractorID
inner join Districts dis on dis.DISTRICT_ID= w.worklocationdist_id
inner join Division dv on dv.Div_Id= dis.Div_Id
where a.AcceptLetterDT>'01-01-2022'
and w.IsDeleted is null and a.WrokOrderDT is null 
and w.MainSchemeID not in (121) {whereClause}
group by  a.Divisionid,dv.DivName_En ";
                   return await _context.WOPendingDivisionDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }







        public async Task<IEnumerable<WOPendingDetailDTO>> getWorkOrderPendingDetails(string divisionId, string mainSchemeId, string isvaluePara)
        {
            FacOperation f = new FacOperation(_context);
            string whereClause = "";
            if (divisionId != "0")
            {
                whereClause += $" and a.DivisionID = '{divisionId}'";
            }
            if (mainSchemeId != "0")
            {
                whereClause += $" and  msc.MainSchemeID  = {mainSchemeId}";
            }
            if (isvaluePara != "0")
            {
                whereClause += f.paraId(isvaluePara);
       
            }

            string query = $@" 
select a.work_id,msc.Name as Head,cast(w.AaAmt as decimal(18,2)) as ASAMT
,CONVERT(varchar,w.AADate, 105) as ASDATE,CONVERT(varchar,w.TSDate, 105) as TSDate,dis.DBStart_Name_En as DISTRICT,b.Block_Name_En as Block , d.NAME_ENG+' - '+s.SWName WORKNAME ,


Round(cast(a.PAC as float(2)),2) as PAC,Round(cast(a.SanctionRate as float(2)),2) as SANCTIONRATE,a.SanctionDetail,a.FormT
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end as TOTALAMOUNTOFCONTRACT

,cast(a.TimeAllowed as float(2)) as TIMEALLOWED ,a.AcceptanceLetterRefNo,convert(varchar,a.AcceptLetterDT,105) as ACCEPTLETTERDT ,
a.ContractorID,c.EnglishName as CNAME
,a.TenderReference,convert(varchar,a.DateOfSanction,105) as DateOfSanction,case when t.IsZonal='Y' then 'Zonal' else 'Works' end as tendertype
,convert(varchar,ProgressDT,105) as ProgressDT,LProgress,PRemarks,DelayReason
from  WorkMaster  w 
INNER JOIN Login l ON l.Login_id = w.approved_by
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  dhrsHealthCenter d on  d.HC_ID=w.worklocation_id 
inner join SWDetails s on s.SWId=w.work_description_id 
inner join dhrsHealthCentreType ty on ty.Type_ID=d.TYPE
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID=a.tenderid
inner join  ContractMaster c on c.Contractorid=a.ContractorID
left outer join BlocksMaster b on b.Block_ID=d.BLOCK_ID and b.District_ID=dis.District_ID
left outer join 
(
select wl.ppid, wl.level_id ,isnull(wl.level_name,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,

case when p.Remarkid is null then '' else  r.Remarks end as mremarks,p.DelayReason
from  WorkPhysicalProgress p
inner join WorklevelMaster wl on wl.level_id=WorkLevel
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'
group by p.Work_id,wl.level_name,ProgressDT, p.Remarks,wl.level_id,wl.ppid,r.Remarks,p.Remarkid,p.DelayReason
) pl on pl.Work_id=w.work_id
where a.AcceptLetterDT>'01-01-2022'
and w.IsDeleted is null and a.WrokOrderDT is null 
and  w.MainSchemeID not in (121) {whereClause}
order by a.AcceptLetterDT desc ";

            return await _context.WOPendingDetailDBset
                .FromSqlRaw(query)
                .ToListAsync();


        }
        





            public async Task<IEnumerable<WorkDetailDTO>> GetWorkDetailAsync(string divisionId, string mainSchemeId, string workName, string compareStatus,string ppid,string wopending)
        {
            FacOperation f = new FacOperation(_context);
            string? whereClause = "";
            string? whereClauseWork = "";
            string? whereClauseCompare = "";



            if (divisionId != "0")
            {
                if (wopending != "0")
                {
                    whereClause += $" and a.DivisionID = '{divisionId}'";
                }
                else
                {
                    whereClause += $" and dv.Div_Id = {divisionId}";
                }
            }
            if (mainSchemeId != "0")
            {
                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
            if (workName != "0")
            {
                whereClauseWork += $" and workname like '%{workName}%'";
            }
            if (ppid != "0")
            {
                whereClause += $" and pl.ppid = {ppid}";
            }
            if (compareStatus != "0")
            {
                whereClause += f.paraId(compareStatus);

            }
            string orderyb = " order by DivName_En,District,AADate";
            if (wopending != "0")
            {
                whereClause += $" and a.AcceptLetterDT>'01-01-2022' and  a.WrokOrderDT is null ";
                orderyb = " order by accdt   ";
            }


            string query = $@" select ROW_NUMBER() Over (Order by District) As SN, District,workname,DivName_En,convert(varchar,AADate,103)  as ASDate,ISNULL(ASAmt,0) as ASAmt,ISNULL(tsamt,0) as tsamt,tsdate,Head,Worktype,AAYEar
,AcceptLetterDT,NIT as NITNo,IsZonal,EnglishName as CName,mobno as CMobNo,WrokOrderDT,PAC,SanctionRate,SanctionDetail,TotalAmountOfContract,TimeAllowed,convert(varchar,newDueDT,103)  as DueDateofCompletion

,PStatus,status,LastProgressDT,mremarks,DelayReason,isnull(expCompDT ,convert(varchar,newDueDT,103)) as expCompDT
,FinalBill,isnull(cast(billno as varchar),'NILL') as billno ,totalexp+gstcharge as totalexp
,work_id
,fPStatus
,imagename, imagename2, imagename3, imagename4, ImageName5,IsMongo,SR,accdt,AADate
from 
(
select distinct  y.year  as AAYEar,
 msc.Name as Head,dis.DBStart_Name_En as District,
  d.NAME_ENG+' - '+s.SWName +' ('+w.work_id+') ' as workname
,cast(AaAmt as decimal(18,2)) as ASAmt,w.LetterNo,cast(TSAmount as decimal(18,2)) as tsamt,CONVERT(varchar,TSDate, 103) as tsdate
,CONVERT(varchar,AADate, 103) as AA_RAA_Date,isnull(totalExp,0) as totalexp,
wpp.ParentProgress as status,convert(varchar,ProgressDT,103) as  LastProgressDT
 ,isnull(fi.fbill,'No') as FinalBill
 ,dis.District_ID
 ,pl.level_id,
 pl.ppid
 ,dv.Div_Id
 ,msc.MainSchemeID 
 ,w.work_id
 ,dv.DivName_En
 ,l.login_name as WorkApprover
 ,case when work_type=1 then 'New'  else case when work_type=2 then 'Renovation' else 'Upgradation' end end as Worktype,
c.EnglishName,c.mobno
,isnull(convert(varchar,AcceptLetterDT,103) ,'-') as AcceptLetterDT, convert(varchar,a.WrokOrderDT,103)  as WrokOrderDT
,convert(varchar,a.DueDatecompletion,103) as DueDatecompletion,
 LProgress
 ,datediff(dd,ProgressDT,getdate()) daySincLastProgress
 ,WType.WLStatusID,WType.PStatus,mremarks,DProgress,DProgressDT,DProgressRemarks
,AADate
,fbill,case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end as newDueDT
,a.TimeAllowed,blt.billno, isnull(ex.gstcharge,0) as GSTcharge
,pl.RID,DP.DelayReason,convert(varchar,DP.expCompDT,103)  as expCompDT,Round(cast(a.PAC as decimal(18,2)),2) PAC,
Round(cast(a.SanctionRate as decimal(18,2)),2) SanctionRate ,a.SanctionDetail,Round(cast(a.TotalAmountOfContract as decimal(18,2)),2) TotalAmountOfContract
,isnull(fPStatus,'Unpaid')  fPStatus
,t.TenderNo as NITNo,case when t.IsZonal is not null then 'Zonal' else 'Non Zonal' end as IsZonal
,pl.imagename, pl.imagename2, pl.imagename3, pl.imagename4, pl.ImageName5,pl.IsMongo,pl.SR,AcceptLetterDT as accdt,
case when t.IsZonal is not null then t.TenderNo else a.TenderReference end as NIT
 from 
WorkMaster w
left outer join mas_financial_year y on y.yearval=w.approved_fin_year
INNER JOIN Login l ON l.Login_id = w.approved_by
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
left outer join AgreementDetails a on a.work_id=w.work_id 
left outer join MasTender t on t.TenderID=a.tenderid
left outer join ContractMaster c on c.Contractorid=a.ContractorId  
INNER JOIN  dhrsHealthCenter d on  cast(d.HC_ID as bigint)=cast(w.worklocation_id as bigint) 
INNER JOIN dhrsHealthCentreType ty on ty.Type_ID=d.TYPE
INNER JOIN SWDetails s on s.SWId=w.work_description_id 
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
left outer join BlocksMaster b on b.Block_ID =w.BlockID and b.District_ID =dis.District_ID
inner join  Division dv on dv.Div_Id=dis.Div_Id
INNER JOIN 
(
select wl.ppid, wl.level_id ,isnull(wl.level_name,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,

case when p.Remarkid is null then '' else  r.Remarks end as mremarks,r.RID
,p.imagename, p.imagename2, p.imagename3, p.imagename4, p.ImageName5,p.IsMongo,p.SR
from  WorkPhysicalProgress p
inner join WorklevelMaster wl on wl.level_id=WorkLevel
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'
group by p.Work_id,wl.level_name,ProgressDT, p.Remarks,wl.level_id,wl.ppid,r.Remarks,p.Remarkid,r.RID
,p.imagename, p.imagename2, p.imagename3, p.imagename4, p.ImageName5,p.IsMongo,p.SR
) pl on pl.Work_id=w.work_id

left outer join 
(
select isnull(wl.level_name,'Progress Not Entered' ) as DProgress,ProgressDT as DProgressDT,isnull(p.Remarks,'' ) as DProgressRemarks,p.Work_id
,p.DelayReason,p.expCompDT
from  WorkPhysicalProgress p
inner join WorklevelMaster wl on wl.level_id=p.WorkLevel
where NewProgress='Y'
group by p.Work_id,wl.level_name,ProgressDT, p.Remarks,wl.level_id,p.DelayReason,p.expCompDT
) DP on dp.Work_id=w.work_id

left outer join 
(
select  work_code,case when  bp.AgrBillStatus='Final' then 'Yes' else 'No' end as fbill  from BillPayment bp 
where bp.AgrBillStatus='Final'
group by work_code,bp.AgrBillStatus
) fi on fi.work_code=w.work_id


left outer join 
(
select  work_code,case when bp.ChequeNo is not null and isnull(bp.ispass,'N') ='P' and   bp.Ispaid='Y'  then 'Paid' else 'UnPaid' end as fPStatus from BillPayment bp 
where bp.AgrBillStatus='Final'  
) fPU on fPU.work_code=w.work_id


left outer join 
(
select  work_code,MAX(billno) as billno  from BillPayment bp 
where bp.AgrBillStatus='Running' and bp.ChequeNo is not null and bp.ispass='P'
group by work_code
) blt on blt.work_code=w.work_id



left outer join WorkLevelParent wpp on wpp.ppid=pl.ppid
left outer join WorkLevelStatus WType on WType.WLStatusID=wpp.SNHID

left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal(18,2)) as totalExp 

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal(18,2)) gstcharge
from BillPayment b
where b.Ispaid='Y' and  isnull(ispass,'N') ='P' and b.ChequeNo is not null
group by work_code
) ex on ex.work_code=w.work_id
where 1=1 and w.IsDeleted is null and w.MainSchemeID not in (121) {whereClause}
) b
where 1=1  {whereClauseWork}
{whereClauseCompare}
{orderyb} ";

            return await _context.WorkDetailDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }
  
        public async Task<IEnumerable<WorkContractorDTO>> GetContractorWorks(string divisionId, string mainSchemeId, string cid,string rpending)
        {
           
            string? whereClause = "";
     



            if (divisionId != "0")
            {
                //if (wopending != "0")
                //{
                whereClause += $" and a.DivisionID = '{divisionId}'";
                //}
                //else
                //{
                //    whereClause += $" and dv.Div_Id = {divisionId}";
                //}
            }
            if (mainSchemeId != "0")
            {
                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }
         
            if (cid != "0")
            {
                whereClause += $" and a.ContractorID= '{cid}'";
            }

            if (rpending == "Running")
            {
                whereClause += $" and WType.WLStatusID= 1";
            }
            if (rpending == "Handover")
            {
                whereClause += $" and WType.WLStatusID= 3";
            }
            if (rpending == "NotStarted")
            {
                whereClause += $" and WType.WLStatusID= 2";
            }


            string query = $@" select PPID,CNAME,rtrim(ltrim(ContractorID)) ContractorID,count(distinct work_id) nosworks,cast(round(sum(TOTALAMOUNTOFCONTRACT),2) as decimal(18,2)) as workvaluelc,cast(round(isnull(sum(totalexp),0),2) as decimal(18,2)) as totalexplac,WLStatusID,PStatus,ParentProgress
from 
(

select a.work_id,msc.Name as Head,cast(w.AaAmt as decimal(18,2)) as ASAMT
,CONVERT(varchar,w.AADate, 105) as ASDATE,CONVERT(varchar,w.TSDate, 105) as TSDate,dis.DBStart_Name_En as DISTRICT,b.Block_Name_En as Block , d.NAME_ENG+' - '+s.SWName WORKNAME ,


Round(cast(a.PAC as float(2)),2) as PAC,Round(cast(a.SanctionRate as float(2)),2) as SANCTIONRATE,a.SanctionDetail,a.FormT
,case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end as TOTALAMOUNTOFCONTRACT

,cast(a.TimeAllowed as float(2)) as TIMEALLOWED ,a.AcceptanceLetterRefNo,convert(varchar,a.AcceptLetterDT,105) as ACCEPTLETTERDT ,
a.ContractorID,c.EnglishName as CNAME
,a.TenderReference,convert(varchar,a.DateOfSanction,105) as DateOfSanction,case when t.IsZonal='Y' then 'Zonal' else 'Works' end as tendertype
,convert(varchar,ProgressDT,105) as ProgressDT,LProgress,PRemarks,DelayReason
,WType.PStatus,WType.WLStatusID,totalexp+gstcharge as totalexp,wpp.ParentProgress,wpp.PPID
from  WorkMaster  w 
left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal(18,2)) as totalExp 

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal(18,2)) gstcharge
from BillPayment b
where b.Ispaid='Y' and  isnull(ispass,'N') ='P' and b.ChequeNo is not null
group by work_code
) ex on ex.work_code=w.work_id

INNER JOIN Login l ON l.Login_id = w.approved_by
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  dhrsHealthCenter d on  d.HC_ID=w.worklocation_id 
inner join SWDetails s on s.SWId=w.work_description_id 
inner join dhrsHealthCentreType ty on ty.Type_ID=d.TYPE
inner join  AgreementDetails a on a.work_id= w.work_id
left outer join MasTender t on t.TenderID=a.tenderid
inner join  ContractMaster c on c.Contractorid=a.ContractorID
left outer join BlocksMaster b on b.Block_ID=d.BLOCK_ID and b.District_ID=dis.District_ID
left outer join 
(
select wl.ppid, wl.level_id ,isnull(wl.level_name,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,

case when p.Remarkid is null then '' else  r.Remarks end as mremarks,p.DelayReason
from  WorkPhysicalProgress p
inner join WorklevelMaster wl on wl.level_id=WorkLevel
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'
group by p.Work_id,wl.level_name,ProgressDT, p.Remarks,wl.level_id,wl.ppid,r.Remarks,p.Remarkid,p.DelayReason
) pl on pl.Work_id=w.work_id
inner join  WorkLevelParent wpp on wpp.ppid=pl.ppid
inner join   WorkLevelStatus WType on WType.WLStatusID=wpp.SNHID
where 1=1 
and w.IsDeleted is null
and  w.MainSchemeID not in (121)   {whereClause}
) a group by CNAME,ContractorID,PStatus,WLStatusID,ParentProgress,PPID
order by CNAME,WLStatusID,PPID ";

            return await _context.WorkContractorDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }


        public async Task<IEnumerable<DivPerformanceDTO>> DivWiseProgress(string divisionId, string mainSchemeId, string cid, string rpending)
        {

            string? whereClause = "";
            if (divisionId != "0")
            {
               
                whereClause += $" and a.DivisionID = '{divisionId}'";
               
            }
            if (mainSchemeId != "0")
            {
                whereClause += $" and  msc.MainSchemeID = {mainSchemeId}";
            }

            if (cid != "0")
            {
                whereClause += $" and a.ContractorID= '{cid}'";
            }

            string query = "";
            if (rpending == "Running")
            {
                query = $@" select  ROW_NUMBER() Over (Order by Division) As SN,Division,Divisionid,Round(sum(TOTALAMOUNTOFCONTRACT)/100,2) as tvccr,Round(sum(totalexp)/100,2) as expcr ,sum(Delaywork) as Delaywork ,sum(ontime) as ontime
from 
(
select w.work_id,c.Contractorid,c.EnglishName,convert(varchar,w.AADate,103)  as ASDate,ISNULL(w.AaAmt,0) as ASAmt,
case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end as TOTALAMOUNTOFCONTRACT,a.WrokOrderDT,
TimeAllowed,case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end as DueDT,extDT
,case when extDT>getdate() then  datediff(dd,extDT,getdate()) else   datediff(dd,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end),getdate()) end as dayApr


,wpp.ParentProgress,wpp.PPID,cast(isnull(totalexp,0)+isnull(gstcharge,0) as float(2)) as totalexp
,case when (case when extDT>getdate() then  datediff(dd,extDT,getdate()) else   datediff(dd,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end),getdate()) end)>=0
then 1 else 0 end as Delaywork
,case when (case when extDT>getdate() then  datediff(dd,extDT,getdate()) else   datediff(dd,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end),getdate()) end)>=0
then 0 else 1 end as ontime
,dv.DivName_En as Division,a.Divisionid
from AgreementDetails a
inner join WorkMaster w on w.work_id=a.work_id
inner join MainSchemes msc on msc.MainSchemeID = w.MainSchemeID
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  Division dv on dv.Div_Id=dis.Div_Id
inner join  ContractMaster c on c.Contractorid=a.ContractorID
INNER JOIN 
(
select wl.ppid, wl.level_id ,isnull(wl.level_name,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,
case when p.Remarkid is null then '' else  r.Remarks end as mremarks,r.RID
from  WorkPhysicalProgress p
inner join WorklevelMaster wl on wl.level_id=WorkLevel
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'
group by p.Work_id,wl.level_name,ProgressDT, p.Remarks,wl.level_id,wl.ppid,r.Remarks,p.Remarkid,r.RID,p.IsMongo,p.SR
) pl on pl.Work_id=a.work_id

left outer join 
(
 select workcode, max(DueDatecompletion) extDT from AgreementExtensionMaster 
 group by workcode
)  we on we.workcode=a.work_id

left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal(18,2)) as totalExp 

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal(18,2)) gstcharge
from BillPayment b
where b.Ispaid='Y' and  isnull(ispass,'N') ='P' and b.ChequeNo is not null
group by work_code
) ex on ex.work_code=w.work_id

inner join  WorkLevelParent wpp on wpp.ppid=pl.ppid
inner join WorkLevelStatus WType on WType.WLStatusID=wpp.SNHID
where 1=1 
and w.IsDeleted is null and  a.WrokOrderDT is not null 
and WType.WLStatusID= 1
and  w.MainSchemeID not in (121) {whereClause}
) b
group by Division,Divisionid order by Division ";
            }
            else if (rpending == "Handover")
            {

                //Handover
                query = $@" select  ROW_NUMBER() Over (Order by Division) As SN,Division,'0' as Divisionid,Round(sum(TOTALAMOUNTOFCONTRACT)/100,2) as TVCCr,Round(sum(totalexp)/100,2) as expcr ,sum(Delaywork) as Delaywork ,sum(ontime) as ontime

from 
(

select w.work_id,c.Contractorid,c.EnglishName,convert(varchar,w.AADate,103)  as ASDate,ISNULL(w.AaAmt,0) as ASAmt,
case when a.FormT='B' then round(a.TotalAmountOfContract/100000,2) else Round(cast(a.TotalAmountOfContract as float(2)),2)  end as TOTALAMOUNTOFCONTRACT,a.WrokOrderDT,
TimeAllowed,case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end as DueDT,extDT
,case when extDT>getdate() then  datediff(dd,extDT,getdate()) else   datediff(dd,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end),getdate()) end as dayApr


,wpp.ParentProgress,wpp.PPID,cast(isnull(totalexp,0)+isnull(gstcharge,0) as float(2)) as totalexp
,case when (case when extDT>ProgressDT then  datediff(dd,extDT,ProgressDT) else   datediff(dd,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end),ProgressDT) end)>=0
then 1 else 0 end as Delaywork
,case when (case when extDT>ProgressDT then  datediff(dd,extDT,ProgressDT) else   datediff(dd,(case when a.TimeAllowed>6 then DATEADD(Month, TimeAllowed+1, a.WrokOrderDT)  else DATEADD(day,15,DATEADD(Month, TimeAllowed, a.WrokOrderDT))  end),ProgressDT) end)>=0
then 0 else 1 end as ontime
,dv.DivName_En as Division,a.Divisionid,ProgressDT
from AgreementDetails a
inner join WorkMaster w on w.work_id=a.work_id
inner join Districts dis on dis.DISTRICT_ID=w.worklocationdist_id
inner join  Division dv on dv.Div_Id=dis.Div_Id
inner join  ContractMaster c on c.Contractorid=a.ContractorID
INNER JOIN 
(
select wl.ppid, wl.level_id ,isnull(wl.level_name,'Progress Not Entered' ) as LProgress,ProgressDT,isnull(p.Remarks,'' ) as PRemarks,p.Work_id,
case when p.Remarkid is null then '' else  r.Remarks end as mremarks,r.RID
from  WorkPhysicalProgress p
inner join WorklevelMaster wl on wl.level_id=WorkLevel
left outer join WorkProgressLevel_Remarks r on r.RID=p.remarkid
where NewProgress='Y'
group by p.Work_id,wl.level_name,ProgressDT, p.Remarks,wl.level_id,wl.ppid,r.Remarks,p.Remarkid,r.RID,p.IsMongo,p.SR
) pl on pl.Work_id=a.work_id

left outer join 
(
 select workcode, max(DueDatecompletion) extDT from AgreementExtensionMaster 
 group by workcode
)  we on we.workcode=a.work_id

left outer join
(
select work_code, cast((sum(cast(PaidGrossAmount as decimal(18,2)))+Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2))/100000 as decimal(18,2)) as totalExp 

,cast(((Round(sum(cast(PaidGrossAmount as decimal(18,2)))*5/100,2)*18)/100)/100000 as decimal(18,2)) gstcharge
from BillPayment b
where b.Ispaid='Y' and  isnull(ispass,'N') ='P' and b.ChequeNo is not null
group by work_code
) ex on ex.work_code=w.work_id

inner join  WorkLevelParent wpp on wpp.ppid=pl.ppid
inner join WorkLevelStatus WType on WType.WLStatusID=wpp.SNHID
where 1=1 
and w.IsDeleted is null 
and WType.WLStatusID= 3
and  w.MainSchemeID not in (121) {whereClause}
) b
group by Division order by Division ";
            }
            else if (rpending == "Handover")
            {
                query = "";
            }
            else
            {
                query = "";
            }

                return await _context.DivPerformanceDbSet
                .FromSqlRaw(query)
                .ToListAsync();
        }


        public async Task<IEnumerable<ProgressCountDTO>> ProgressCount(string divisionId, string mainSchemeId, string distid,string ASAmount,string GrantID,string ASID)
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

            if (distid != "0")
            {
                whereClause += $"  and d.District_ID= '{distid}'";
            
            }

            if (GrantID != "0")
            {
                whereClause += $"  and w.GrantNo= '{GrantID}'";

            }

            if (ASID != "0")
            {
                whereClause += $"  and w.ASID= '{ASID}'";

            }





            if (ASAmount!="0")
            {
                if(ASAmount=="1")
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
           
                query = $@"  select d.did,a.drid, d.dashname,
count(a.work_id)as nosworks from WorkLevelParentDash d

left outer join 
(
select msc.Name as Head,w.work_id,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,

GroupName,PGroupID,pl.LProgress,pl.ProgressDT,pl.PRemarks,pl.Remarks,pl.RID,

d.District_ID,agd.DivisionID,msc.MainSchemeID 
,did,DashName, case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end as drid,
case when pl.RID is not null  and pl.RID in (1,15) and w.ISRETURNDEP is null then 'Land Not Alloted/Land Dispute' else 
case when w.ISRETURNDEP ='Y' then 'Return to Department' else 
DashName end end as display

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

where   (case when w.isdeleted ='Y' and w.ISRETURNDEP is null then 0 else 1 end )=1  and w.MainSchemeID not in (121) " + whereClause + @"

) a  on a.drid=d.did
where  d.did not in (7001)
  group by a.drid, d.dashname,d.did
  order by d.did  ";
           
           

            return await _context.ProgressCountDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }

        public async Task<IEnumerable<DistProgressCountDTO>> ProgressDistCount(string divisionId, string mainSchemeId,string dashID)
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
            string hvclause="";
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

            query = $@" select d.District_ID,d.DBStart_Name_En as Districtname,cast(sum(ToBeTender1001) as int) as ToBeTender1001,
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
,DivisionID
from WorkLevelParentDash d

left outer join 
(
select msc.Name as Head,w.work_id,
CONVERT(varchar,w.AADate,105) as AADT,cast(AaAmt as decimal(18,2)) as ASAmt,

GroupName,PGroupID,pl.LProgress,pl.ProgressDT,pl.PRemarks,pl.Remarks,pl.RID,

d.District_ID,agd.DivisionID,msc.MainSchemeID 
,did,DashName, case when pl.RID is not null and pl.RID in (1,15) and w.ISRETURNDEP is null   then  6001  else case when w.ISRETURNDEP ='Y' then 8001 else  did   end end as drid,
case when pl.RID is not null  and pl.RID in (1,15) and w.ISRETURNDEP is null then 'Land Not Alloted/Land Dispute' else 
case when w.ISRETURNDEP ='Y' then 'Return to Department' else 
DashName end end as display

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

where  (case when w.isdeleted ='Y' and w.ISRETURNDEP is null then 0 else 1 end )=1  and w.MainSchemeID not in (121) 
 " + whereClause + @"
) a  on a.drid=d.did
where  d.did not in (7001) and DivisionID is not null
  group by a.drid, d.dashname,d.did,District_ID,DivisionID
  ) b on b.District_ID=d.District_ID
  where 1=1 and DivisionID is not null 
  group by d.District_ID,d.DBStart_Name_En,DivisionID
  " + hvclause + @"
  order by DivisionID,d.District_ID ";



            return await _context.DistProgressCountDbSet
            .FromSqlRaw(query)
            .ToListAsync();
        }

    }
}
