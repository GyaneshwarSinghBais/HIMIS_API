using HIMIS_API.Data;
using HIMIS_API.Models.EMS;
using HIMIS_API.Models.StockMgm;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockMgmController : ControllerBase
    {
        private readonly DbContextStockMgm _context;
        public StockMgmController(DbContextStockMgm context)
        {
            _context = context;
        }

        //https://localhost:7247/api/StockMgm/CoverStatus
        [HttpGet("CoverStatus")]
        public async Task<ActionResult<IEnumerable<CoverStatusDTO>>> CoverStatus()
        {


            string query = $@"  select tcs.CSID,tcs.CStatus , count(tender_id) as CntTender,sum(tValue) as tValue 
 

from 
mascoverstatus tcs  
left outer join 
 (

select ts.tender_id,isGemTender,tender_no,tender_description,tender_date,ENDDate,cover_a,cover_b,CStatus
,count(distinct  item_Id) cntItems,sum(TenderValue) as TenderValue,sum(tValue) as tValue
, isnull(ts.TENDERSTATUS,x.CStatus) as TENDERSTATUS,ts.tenderremark,ts.entrydate
,x.CSID
 from 
(

select t.tender_id,ti.item_ID ,categoryName,m.item_code,m.item_Name,m.estimated_cost,ti.tender_quantity 
,m.estimated_cost*ti.tender_quantity  as TenderValue,
 case when t.isGemTender ='Y' then 'Gem-Tender' else 'Normal Tender'  end  as isGemTender,tender_no,tender_description
,convert(varchar,tender_date,105) as tender_date,
convert(varchar,t.ENDDate,105) as ENDDate,convert(varchar,t.cover_a,105) as cover_a,convert(varchar,t.cover_b,105)  as cover_b
,ct.CSID,ct.CStatus,t.tValue  
from tenders t
inner join tender_items ti on ti.tender_id=t.tender_id
inner join masitems m on m.item_id=ti.item_id
left outer join masCategory c on c.categoryId=m.categoryId
inner join mascoverstatus ct on ct.CSID=t.CSID
where ct.CSID not in (6)
)x
left outer join 
(
select A.TENDER_ID,
 tr.TSID, t.TENDERSTATUS, tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
from TENDERS A 
inner join  TENDERSTATUSREMARK tr on tr.tender_id=A.tender_id and ISNEW='Y'
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where ISNEW='Y'
)ts on ts.tender_id=x.tender_id
where isnull(ts.TENDERSTATUS,x.CStatus)='Tender Live'
group by ts.tender_id,isGemTender,tender_no,tender_description,tender_date,ENDDate,cover_a,cover_b,CStatus,ts.TENDERSTATUS,ts.tenderremark,ts.entrydate,x.CSID,x.CStatus
)td on td.CSID=tcs.CSID

where tcs.CSID not in (6,4,5)
group by tcs.CSID,tcs.CStatus
;  ";

            var result = await _context.CoverStatusDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/StockMgm/CoverStatusDetail
        [HttpGet("CoverStatusDetail")]
        public async Task<ActionResult<IEnumerable<CoverStatusDetailDTO>>> CoverStatusDetail(Int32 csid)
        {
            string whCsid = "";

            if(csid != 0) 
            {
                whCsid = " and CSID=1  ";
            }


            string query = $@" select CSID,ts.tender_id,isGemTender,tender_no,tender_description,tender_date,ENDDate,cover_a,cover_b,CStatus
,count(distinct  item_Id) cntItems,sum(tValue) as TenderValue
, isnull(ts.TENDERSTATUS,x.CStatus) as TENDERSTATUS,ts.tenderremark,ts.entrydate
 from 
(

select t.tender_id,ti.item_ID ,categoryName,m.item_code,m.item_Name,m.estimated_cost,ti.tender_quantity 
,m.estimated_cost*ti.tender_quantity  as TenderValue,
 case when t.isGemTender ='Y' then 'Gem-Tender' else 'Normal Tender'  end  as isGemTender,tender_no,tender_description
,convert(varchar,tender_date,105) as tender_date,
convert(varchar,t.ENDDate,105) as ENDDate,convert(varchar,t.cover_a,105) as cover_a,convert(varchar,t.cover_b,105)  as cover_b
,ct.CSID,ct.CStatus,t.tValue  
from tenders t
inner join tender_items ti on ti.tender_id=t.tender_id
inner join masitems m on m.item_id=ti.item_id
left outer join masCategory c on c.categoryId=m.categoryId
inner join mascoverstatus ct on ct.CSID=t.CSID
where ct.CSID not in (6) 
)x
left outer join 
(
select A.TENDER_ID,
 tr.TSID, t.TENDERSTATUS, tr.tenderremark,
convert(varchar, tr.entrydate,103) as entrydate
from TENDERS A 
inner join  TENDERSTATUSREMARK tr on tr.tender_id=A.tender_id and ISNEW='Y'
inner join TENDERSTATUSMASTER t  on t.tsid=tr.tsid
where ISNEW='Y'
)ts on ts.tender_id=x.tender_id
where 1=1"+ whCsid + @"
group by CSID,ts.tender_id,isGemTender,tender_no,tender_description,tender_date,ENDDate,cover_a,cover_b,CStatus,ts.TENDERSTATUS,ts.tenderremark,ts.entrydate  ";

            var result = await _context.CoverStatusDetailDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/StockMgm/CoverStatusTenderDetail?tenderId=1
        [HttpGet("CoverStatusTenderDetail")]
        public async Task<ActionResult<IEnumerable<CoverStatusTenderDetailDTO>>> CoverStatusTenderDetail(Int32 tenderId)
        {

            
            string whTenderId = "";

            

            if (tenderId != 0)
            {
                whTenderId = "and t.tender_id="+ tenderId + @"   ";
            }


            string query = $@" select t.tender_id,ti.item_ID ,categoryName,m.item_code,m.item_Name,m.estimated_cost,ti.tender_quantity 
,m.estimated_cost*ti.tender_quantity  as TenderValue
 
from tenders t
inner join tender_items ti on ti.tender_id=t.tender_id
inner join masitems m on m.item_id=ti.item_id
left outer join masCategory c on c.categoryId=m.categoryId
inner join mascoverstatus ct on ct.CSID=t.CSID
where 1=1 and ct.CSID not in (6) "+ whTenderId + @"  ";

            var result = await _context.CoverStatusTenderDetailDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }



    }
}
