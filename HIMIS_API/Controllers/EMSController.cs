using HIMIS_API.Data;
using HIMIS_API.Models.EMS;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EMSController : ControllerBase
    {
        private readonly DbContextEMS _context;

        public EMSController(DbContextEMS context)
        {
            _context = context;
        }


        //https://localhost:7247/api/EMS/GetEqpTender
        [HttpGet("GetEqpTender")]
        public async Task<ActionResult<IEnumerable<GetEqpTenderDTO>>> GetEqpTender()
        {


            string query = $@"   select distinct t.tender_no,t.tender_id
from  contract_items c
inner join  masitems m on m.item_id= c.item_id
inner join award_of_contract ac on ac.award_of_contract_id=c.award_of_contract_id
inner join massuppliers s on s.supplier_id=ac.supplier_id
inner join tenders t on t.tender_id=ac.tender_id
where GETDATE() between ac.contract_date and ac.contract_end_date
--and c.isfreezed is null ";

            var result = await _context.GetEqpTenderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/EMS/GetEqpRC
        [HttpGet("GetEqpRC")]
        public async Task<ActionResult<IEnumerable<GetEqpRCDTO>>> GetEqpRC(string tenderID)
        {


            string query = $@"   select distinct contract_item_id,item_codeE,item_nameE,CAST(basic_rate AS decimal(18,2)) as basic_rate,
CAST(percentage AS decimal(18,2)) as percentage,
CAST(single_unit_price AS decimal(18,2)) as single_unit_price,model,contract_date,contract_duration,case when contract_new_end_date is not null  then convert(varchar,a.contract_new_end_date,103) else  contract_end_date end  contract_end_date
,name,tender_no,tender_id,webSiteUploadID,file_name,upload_folder_name,item_id,is_extended,contract_new_end_date
from 
(
select c.contract_item_id, m.item_code_as_per_tender as item_codeE,m.item_name as item_nameE
,basic_rate,percentage,single_unit_price,model,convert(varchar,ac.contract_date,103) as contract_date,ac.contract_duration ,convert(varchar,ac.contract_end_date,103) as contract_end_date,
s.name,t.tender_no,t.tender_id,t.webSiteUploadID,mu.file_name,mu.upload_folder_name,m.item_id
,c.is_extended,c.contract_new_end_date
from  contract_items c
inner join  masitems m on m.item_id= c.item_id
left outer join  masitems_upload mu on mu.item_id=m.item_id
inner join award_of_contract ac on ac.award_of_contract_id=c.award_of_contract_id
inner join massuppliers s on s.supplier_id=ac.supplier_id
inner join tenders t on t.tender_id=ac.tender_id
where 1=1 
--and t.isgemtender is null 
--and  c.isfreezed is null 
and GETDATE() between ac.contract_date and (case when c.contract_new_end_date is not null then c.contract_new_end_date else  ac.contract_end_date  end)
and t.tender_id=" + tenderID + @"
 )a
order by a.contract_date ";

            var result = await _context.GetEqpRCDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/EMS/GetTotalTendersByStatus
        [HttpGet("GetTotalTendersByStatus")]
        public async Task<ActionResult<IEnumerable<GetTotalTendersByStatusDTO>>> GetTotalTendersByStatus()
        {


            string query = $@"   select st.csid ,st.CStatus ,count(distinct TENDER_ID) as NoofTender,sum(isnull(tValue,0)) as TenderValue
 from mascoverstatus st 
 
 left outer join  (


SELECT A.TENDER_ID,A.TENDER_NO,Convert(varchar(10),A.TENDER_DATE, 103) AS TENDER_DATE,A.TENDER_DESCRIPTION,
A.FLAG,A.financial_year_id,A.warranty_year,A.import_days,A.domestic_days --,A.flag
,Convert(varchar(10),A.cover_a, 103) AS cover_a,Convert(varchar(10),A.cover_b, 103) AS cover_b,Convert(varchar(10),A.cover_Demo, 103) AS cover_Demo
,Convert(varchar(10),A.cover_c, 103) AS cover_c
,s.cStatus ,s.csid 
,isnull(t.totali,0) as totali, isnull(fnd.found,0)  as found
,isnull(n.nosNotFound,0) as nosNotFound,isnull(p.PriceEntry,0) as PriceEntry,isnull(ac.accept,0) as accept,isnull(r.reject,0) as reject 
,isnull(nosBidder,0) nosBidder,isnull(nositems,0) as nositems,isnull(a.tValue,0) as tValue
,case when isnull(isGemTender,'N')='N' then 'e-Proc' else 'GeM' end tendertype
FROM TENDERS A 


left outer join 
(
select s.SCHEMEID,  count(distinct s.SUPPLIERID) as nosBidder,count(distinct sc.itemid) as nositems from masschemesstatusdetails s
left outer join schemestatusdetailschild sc on sc.SCHSTATUSDID=s.SCHSTATUSDID
group by s.SCHEMEID
) bd on bd.SCHEMEID=A.tender_id
left outer join 
(
select COUNT(*) nosNotFound,tender_id from tender_items where  priceflag='N'
and  rejectdate is null
group by tender_id
) n on n.tender_id=A.tender_id

left outer join 
(
select COUNT(distinct ti.item_id) found,ti.tender_id from tender_items ti
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
 where  ti.priceflag is null
group by ti.tender_id
) fnd on fnd.tender_id=A.tender_id

left outer join 
(
select count(distinct ti.item_id) as PriceEntry,t.tender_id   from tender_items ti 
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
where l.basicrate is not null
group by t.tender_id
) p on p.tender_id=A.tender_id

left outer join 
(
select count(distinct ti.item_id) as accept,t.tender_id   from tender_items ti 
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
where l.basicrate is not null and  l.isaccept='Y'
group by t.tender_id
) ac on ac.tender_id=A.tender_id

left outer join 
(
select COUNT(*) reject,tender_id from tender_items where rejectdate is not null
group by tender_id
) r on r.tender_id=A.tender_id

left outer join 
(
select COUNT(*) totali,tender_id from tender_items
group by tender_id
) t on t.tender_id=A.tender_id
left outer join mascoverstatus s on s.csid=a.csid 
  where 1=1  and A.financial_year_id >=18   
  and  s.csid is not null
and s.csid not in (6)

)ts on ts.CSID=st.CSID

where  st.CSID not in (6)

 group by st.csid ,st.CStatus order by st.csid;
 ";

            var result = await _context.GetTotalTendersByStatusDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }



        //https://localhost:7247/api/EMS/GetTenderDetail?csid=1
        [HttpGet("GetTenderDetail")]
        public async Task<ActionResult<IEnumerable<GetTenderDetailDTO>>> GetTenderDetail(Int32 csid)
        {
            string whCsid = "";

            if(csid != 0) 
            {
                whCsid = @"  and s.CSID=  "+ csid + "  ";
            }


            string query = $@" select x.TENDER_ID
,
TENDER_NO,eprocID,

TENDER_DATE,extenddt,ENDDate,tender_description,ISNULL(tsr.TENDERSTATUS,x.CStatus) as TENDERSTATUS,tenderremark,totali as NoOfItems,tValue as TenderValue
,cover_a,cover_b,cover_Demo,cover_c
from (

SELECT A.TENDER_ID,A.TENDER_NO,Convert(varchar(10),A.TENDER_DATE, 103) AS TENDER_DATE,Convert(varchar,A.ENDDate, 103) AS ENDDate
,Convert(varchar,A.extenddt, 103) AS extenddt
,A.TENDER_DESCRIPTION,
A.FLAG,A.financial_year_id,A.warranty_year,A.import_days,A.domestic_days --,A.flag
,Convert(varchar(10),A.cover_a, 103) AS cover_a,Convert(varchar(10),A.cover_b, 103) AS cover_b,Convert(varchar(10),A.cover_Demo, 103) AS cover_Demo
,Convert(varchar(10),A.cover_c, 103) AS cover_c
,s.cStatus ,s.csid 
,isnull(t.totali,0) as totali, isnull(fnd.found,0)  as found
,isnull(n.nosNotFound,0) as nosNotFound,isnull(p.PriceEntry,0) as PriceEntry,isnull(ac.accept,0) as accept,isnull(r.reject,0) as reject 
,isnull(nosBidder,0) nosBidder,isnull(nositems,0) as nositems,isnull(a.tValue,0) as tValue
,case when isnull(isGemTender,'N')='N' then 'e-Proc' else 'GeM' end tendertype
,A.eprocID
FROM TENDERS A 


left outer join 
(
select s.SCHEMEID,  count(distinct s.SUPPLIERID) as nosBidder,count(distinct sc.itemid) as nositems from masschemesstatusdetails s
left outer join schemestatusdetailschild sc on sc.SCHSTATUSDID=s.SCHSTATUSDID
group by s.SCHEMEID
) bd on bd.SCHEMEID=A.tender_id
left outer join 
(
select COUNT(*) nosNotFound,tender_id from tender_items where  priceflag='N'
and  rejectdate is null
group by tender_id
) n on n.tender_id=A.tender_id

left outer join 
(
select COUNT(distinct ti.item_id) found,ti.tender_id from tender_items ti
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
 where  ti.priceflag is null
group by ti.tender_id
) fnd on fnd.tender_id=A.tender_id

left outer join 
(
select count(distinct ti.item_id) as PriceEntry,t.tender_id   from tender_items ti 
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
where l.basicrate is not null
group by t.tender_id
) p on p.tender_id=A.tender_id

left outer join 
(
select count(distinct ti.item_id) as accept,t.tender_id   from tender_items ti 
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
where l.basicrate is not null and  l.isaccept='Y'
group by t.tender_id
) ac on ac.tender_id=A.tender_id

left outer join 
(
select COUNT(*) reject,tender_id from tender_items where rejectdate is not null
group by tender_id
) r on r.tender_id=A.tender_id

left outer join 
(
select COUNT(*) totali,tender_id from tender_items
group by tender_id
) t on t.tender_id=A.tender_id
left outer join mascoverstatus s on s.csid=a.csid 
  where 1=1  and A.financial_year_id >=18   
  and  s.csid is not null
and s.csid not in (6) "+ whCsid + @"


)x

left outer join
(

select tr.tender_id,  tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
,t.tenderstatus,tr.TSID 
from
TENDERSTATUSREMARK tr 
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where tr.ISNEW='Y'  

)tsr on  tsr.tender_id=x.tender_id  ";

//            string query = $@"   select x.TENDER_ID
//,
//TENDER_NO,

//TENDER_DATE,tender_description,ISNULL(tsr.TENDERSTATUS,x.CStatus) as TENDERSTATUS,tenderremark,totali as NoOfItems,tValue as TenderValue
//from (

//SELECT A.TENDER_ID,A.TENDER_NO,Convert(varchar(10),A.TENDER_DATE, 103) AS TENDER_DATE,A.TENDER_DESCRIPTION,
//A.FLAG,A.financial_year_id,A.warranty_year,A.import_days,A.domestic_days --,A.flag
//,Convert(varchar(10),A.cover_a, 103) AS cover_a,Convert(varchar(10),A.cover_b, 103) AS cover_b,Convert(varchar(10),A.cover_Demo, 103) AS cover_Demo
//,Convert(varchar(10),A.cover_c, 103) AS cover_c
//,s.cStatus ,s.csid 
//,isnull(t.totali,0) as totali, isnull(fnd.found,0)  as found
//,isnull(n.nosNotFound,0) as nosNotFound,isnull(p.PriceEntry,0) as PriceEntry,isnull(ac.accept,0) as accept,isnull(r.reject,0) as reject 
//,isnull(nosBidder,0) nosBidder,isnull(nositems,0) as nositems,isnull(a.tValue,0) as tValue
//,case when isnull(isGemTender,'N')='N' then 'e-Proc' else 'GeM' end tendertype
//FROM TENDERS A 


//left outer join 
//(
//select s.SCHEMEID,  count(distinct s.SUPPLIERID) as nosBidder,count(distinct sc.itemid) as nositems from masschemesstatusdetails s
//left outer join schemestatusdetailschild sc on sc.SCHSTATUSDID=s.SCHSTATUSDID
//group by s.SCHEMEID
//) bd on bd.SCHEMEID=A.tender_id
//left outer join 
//(
//select COUNT(*) nosNotFound,tender_id from tender_items where  priceflag='N'
//and  rejectdate is null
//group by tender_id
//) n on n.tender_id=A.tender_id

//left outer join 
//(
//select COUNT(distinct ti.item_id) found,ti.tender_id from tender_items ti
//inner join tenders t on t.tender_id=ti.tender_id
//inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
// where  ti.priceflag is null
//group by ti.tender_id
//) fnd on fnd.tender_id=A.tender_id

//left outer join 
//(
//select count(distinct ti.item_id) as PriceEntry,t.tender_id   from tender_items ti 
//inner join tenders t on t.tender_id=ti.tender_id
//inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
//where l.basicrate is not null
//group by t.tender_id
//) p on p.tender_id=A.tender_id

//left outer join 
//(
//select count(distinct ti.item_id) as accept,t.tender_id   from tender_items ti 
//inner join tenders t on t.tender_id=ti.tender_id
//inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
//where l.basicrate is not null and  l.isaccept='Y'
//group by t.tender_id
//) ac on ac.tender_id=A.tender_id

//left outer join 
//(
//select COUNT(*) reject,tender_id from tender_items where rejectdate is not null
//group by tender_id
//) r on r.tender_id=A.tender_id

//left outer join 
//(
//select COUNT(*) totali,tender_id from tender_items
//group by tender_id
//) t on t.tender_id=A.tender_id
//left outer join mascoverstatus s on s.csid=a.csid 
//  where 1=1  and A.financial_year_id >=18   
//  and  s.csid is not null
//and s.csid not in (6) "+ whCsid + @"

//)x

//left outer join
//(

//select tr.tender_id,  tr.tenderremark,
//convert(varchar, tr.entrydate,103) as entrydate
//,t.tenderstatus,tr.TSID 
//from
//TENDERSTATUSREMARK tr 
//inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
//where tr.ISNEW='Y'  

//)tsr on  tsr.tender_id=x.tender_id

// ";

            var result = await _context.GetTenderDetailDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/EMS/EqToBeTender
        [HttpGet("EqToBeTender")]
        public async Task<ActionResult<IEnumerable<EqToBeTenderDTO>>> EqToBeTender()
        {


            string query = $@"   select count( distinct item_id) CntItems,round(sum(IndentValue)/10000000,2)  as IndentValue  from (

SELECT A.INDENT_CONSOLIDATION_ID,b.item_id,b.indent_cons_items_id,A.description,A.USER_ID,A.DIRECTORATE_ID,A.FINANCIAL_YEAR_ID,SUM(B.PROPOSED_QTY) AS PROPOSED_QTY,A.indent_con_no,
CONVERT(VARCHAR(10),A.CONSOLIDATED_DATE,103) AS CONSOLIDATED_DATE,SUM(B.FINAL_QTY) AS FINAL_QTY
,(case when A.STATUS='I' then 'Incomplete' when A.STATUS='C' then 'Completed' else '' end ) EStatus,
(
select (case when FileName is null then 'Not Uploaded' else 'Uploaded' end) UStatus from INDENT_CONSOLIDATION where indent_consolidation_id = A.INDENT_CONSOLIDATION_ID
) as uploadStatus,A.CreatedOn,isnull(B.estimated_cost,0) IndentValue
 FROM INDENT_CONSOLIDATION A 
left outer join  INDENT_CONS_ITEMS B ON (B.INDENT_CONSOLIDATED_ID = A.INDENT_CONSOLIDATION_ID)			
WHERE  a.financial_year_id >=18  

and  b.item_id  is not null and B.item_id  not in (select ti.item_id from TENDERS t
inner join  tender_items ti on  ti.tender_id  =t.tender_id
where t.tender_id in (

select  TENDER_ID
 from   (
SELECT A.TENDER_ID,A.TENDER_NO,Convert(varchar(10),A.TENDER_DATE, 103) AS TENDER_DATE,A.TENDER_DESCRIPTION,
A.FLAG,A.financial_year_id,A.warranty_year,A.import_days,A.domestic_days --,A.flag
,Convert(varchar(10),A.cover_a, 103) AS cover_a,Convert(varchar(10),A.cover_b, 103) AS cover_b,Convert(varchar(10),A.cover_Demo, 103) AS cover_Demo
,Convert(varchar(10),A.cover_c, 103) AS cover_c
,s.cStatus ,s.csid 
,isnull(t.totali,0) as totali, isnull(fnd.found,0)  as found
,isnull(n.nosNotFound,0) as nosNotFound,isnull(p.PriceEntry,0) as PriceEntry,isnull(ac.accept,0) as accept,isnull(r.reject,0) as reject 
,isnull(nosBidder,0) nosBidder,isnull(nositems,0) as nositems,isnull(a.tValue,0) as tValue
,case when isnull(isGemTender,'N')='N' then 'e-Proc' else 'GeM' end tendertype
FROM TENDERS A 


left outer join 
(
select s.SCHEMEID,  count(distinct s.SUPPLIERID) as nosBidder,count(distinct sc.itemid) as nositems from masschemesstatusdetails s
left outer join schemestatusdetailschild sc on sc.SCHSTATUSDID=s.SCHSTATUSDID
group by s.SCHEMEID
) bd on bd.SCHEMEID=A.tender_id
left outer join 
(
select COUNT(*) nosNotFound,tender_id from tender_items where  priceflag='N'
and  rejectdate is null
group by tender_id
) n on n.tender_id=A.tender_id

left outer join 
(
select COUNT(distinct ti.item_id) found,ti.tender_id from tender_items ti
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
 where  ti.priceflag is null
group by ti.tender_id
) fnd on fnd.tender_id=A.tender_id

left outer join 
(
select count(distinct ti.item_id) as PriceEntry,t.tender_id   from tender_items ti 
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
where l.basicrate is not null
group by t.tender_id
) p on p.tender_id=A.tender_id

left outer join 
(
select count(distinct ti.item_id) as accept,t.tender_id   from tender_items ti 
inner join tenders t on t.tender_id=ti.tender_id
inner join live_tender_price l on l.tender_item_id=ti.tender_item_id
where l.basicrate is not null and  l.isaccept='Y'
group by t.tender_id
) ac on ac.tender_id=A.tender_id

left outer join 
(
select COUNT(*) reject,tender_id from tender_items where rejectdate is not null
group by tender_id
) r on r.tender_id=A.tender_id

left outer join 
(
select COUNT(*) totali,tender_id from tender_items
group by tender_id
) t on t.tender_id=A.tender_id
left outer join mascoverstatus s on s.csid=a.csid 
  where 1=1  and A.financial_year_id >=18   
  and  s.csid is not null
and s.csid not in (6)

)ts 

))  and A.STATUS='C'
GROUP BY  b.indent_cons_items_id,b.item_id,A.INDENT_CONSOLIDATION_ID,A.USER_ID,A.DIRECTORATE_ID,A.FINANCIAL_YEAR_ID,A.STATUS,A.CONSOLIDATED_DATE,A.indent_con_no,A.CreatedOn,A.description,B.estimated_cost
)x  ";

            var result = await _context.EqToBeTenderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }








    }
}
