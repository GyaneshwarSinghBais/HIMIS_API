using HIMIS_API.Data;
using HIMIS_API.Models.DTOs;
using HIMIS_API.Models.WebCGMSC;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebCgmscController : ControllerBase
    {
        private readonly DbContextWeb _context;

        public WebCgmscController(DbContextWeb context)
        {
            _context = context;
        }



        //[HttpGet("TestAPI")]

        //public async Task<ActionResult<IEnumerable<AdminLoginDTO>>> TestAPI()
        //{
        //    string query = "";

        //    query = $@" SELECT username, password FROM AdminLogin ";

        //    return await _context.AdminLoginDbSet
        //    .FromSqlRaw(query)
        //    .ToListAsync();

        //}

        //https://localhost:7247/api/WebCgmsc/GetDrugTenderList
        [HttpGet("GetDrugTenderList")]
        public async Task<ActionResult<IEnumerable<DrugTenderDTO>>> GetDrugTenderList(int n = 0)
        {
            string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@"
        SELECT {topClause}
            '~/Home/AttachmentList.aspx?a=' + CAST(c.Content_Registration_Id AS VARCHAR) AS Url,
            c.Content_Registration_Id,
            Attachment_Id,
            c.caption,
            n.Content_Discription,
            n.Content_Subject AS Subject,
            '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
            n.Content_Publising_Date,
            n.Expiry_Date_of,
            n.Expiry_DateOnNotice_Board,
            CASE 
                WHEN CAST(c.EntryDT AS DATE) < CAST(GETDATE() AS DATE) THEN 'N' 
                ELSE 'Y' 
            END AS DisplayNew
        FROM ContentAttachment c
        INNER JOIN NewContent_Create n ON n.Content_Registration_Id = c.Content_Registration_Id
        INNER JOIN Master_ContentCategory m ON m.ContentCategoryCode = n.ContentCategoryCode 
        WHERE 
            c.TenderFileStatus = 'Publish'  
            AND m.Dept = 'Technical' 
            AND m.ContentCategoryCode IN ('3','10')
            AND CAST(DATEADD(DAY, 7, c.EntryDT) AS DATE) >= CAST(GETDATE() AS DATE)
            AND CAST(n.Content_Publising_Date AS DATE) <= CAST(GETDATE() AS DATE)
        ORDER BY c.EntryDT DESC";

            var result = await _context.DrugTenderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetEquipmentList?n=2
        [HttpGet("GetEquipmentList")]
        public async Task<ActionResult<IEnumerable<EquipmentDTO>>> GetEquipmentList(int n = 0)
        {
            string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@"
        SELECT {topClause}
            '~/Home/AttachmentList.aspx?a=' + CAST(c.Content_Registration_Id AS VARCHAR) AS Url,
            c.Content_Registration_Id,
            Attachment_Id,
            c.caption,
            n.Content_Discription,
            n.Content_Subject AS Subject,
            '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
            n.Content_Publising_Date,
            n.Expiry_Date_of,
            n.Expiry_DateOnNotice_Board,
            CASE 
                WHEN CAST(c.EntryDT AS DATE) < CAST(GETDATE() AS DATE) THEN 'N' 
                ELSE 'Y' 
            END AS DisplayNew
        FROM ContentAttachment c
        INNER JOIN NewContent_Create n ON n.Content_Registration_Id = c.Content_Registration_Id
        INNER JOIN Master_ContentCategory m ON m.ContentCategoryCode = n.ContentCategoryCode 
          WHERE 
             c.TenderFileStatus = 'Publish'  
             AND m.Dept = 'Technical' 
             AND m.ContentCategoryCode = '6'
             AND CAST(DATEADD(DAY, 7, c.EntryDT) AS DATE) >= CAST(GETDATE() AS DATE)
             AND CAST(n.Content_Publising_Date AS DATE) <= CAST(GETDATE() AS DATE)
         ORDER BY c.EntryDT DESC";

            var result = await _context.EquipmentDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetCivilTenderList?n=2
        [HttpGet("GetCivilTenderList")]
        public async Task<ActionResult<IEnumerable<CivilTenderDTO>>> GetCivilTenderList(int n = 0)
        {
            string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@"
        SELECT {topClause}
            '~/Home/AttachmentList.aspx?a=' + CAST(c.Content_Registration_Id AS VARCHAR) AS Url,
            c.Content_Registration_Id,
            Attachment_Id,
            c.caption,
            n.Content_Discription,
            n.Content_Subject AS Subject,
            '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
            n.Content_Publising_Date,
            n.Expiry_Date_of,
            n.Expiry_DateOnNotice_Board,
            CASE 
                WHEN CAST(c.EntryDT AS DATE) < CAST(GETDATE() AS DATE) THEN 'N' 
                ELSE 'Y' 
            END AS DisplayNew
        FROM ContentAttachment c
        INNER JOIN NewContent_Create n ON n.Content_Registration_Id = c.Content_Registration_Id
        INNER JOIN Master_ContentCategory m ON m.ContentCategoryCode = n.ContentCategoryCode 
          WHERE 
             c.TenderFileStatus = 'Publish'  
             AND m.Dept = 'Civil' 
             AND m.ContentCategoryCode = '5'
             AND CAST(n.Expiry_DateOnNotice_Board AS DATE) >= CAST(GETDATE() AS DATE)
             AND CAST(n.Content_Publising_Date AS DATE) <= CAST(GETDATE() AS DATE)
         ORDER BY c.EntryDT DESC";

            var result = await _context.CivilTenderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetOtherTenderList?n=2

        [HttpGet("GetOtherTenderList")]
        public async Task<ActionResult<IEnumerable<OtherTenderDTO>>> GetOtherTenderList(int n = 0)
        {
            string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@"
        SELECT {topClause}
            '~/Home/AttachmentList.aspx?a=' + CAST(c.Content_Registration_Id AS VARCHAR) AS Url,
            c.Content_Registration_Id,
            Attachment_Id,
            c.caption,
            n.Content_Discription,
            n.Content_Subject AS Subject,
            '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
            n.Content_Publising_Date,
            n.Expiry_Date_of,
            n.Expiry_DateOnNotice_Board,
            CASE 
                WHEN CAST(c.EntryDT AS DATE) < CAST(GETDATE() AS DATE) THEN 'N' 
                ELSE 'Y' 
            END AS DisplayNew
        FROM ContentAttachment c
        INNER JOIN NewContent_Create n ON n.Content_Registration_Id = c.Content_Registration_Id
        INNER JOIN Master_ContentCategory m ON m.ContentCategoryCode = n.ContentCategoryCode 
        WHERE 
            c.TenderFileStatus = 'Publish'
            AND m.ContentCategoryCode IN ('7', '12', '8')
            AND CAST(DATEADD(DAY, 7, c.EntryDT) AS DATE) >= CAST(GETDATE() AS DATE)
            AND CAST(n.Content_Publising_Date AS DATE) <= CAST(GETDATE() AS DATE)
        ORDER BY c.EntryDT DESC";

            var result = await _context.OtherTenderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetMostVisitedContentList?n=2
        [HttpGet("GetMostVisitedContentList")]
        public async Task<ActionResult<IEnumerable<MostVisitedContentDTO>>> GetMostVisitedContentList(int n = 0)
        {
            string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@"
        SELECT {topClause}
            sub1.date_timestamp,
            sub1.Content_Registration_Id,
            sub1.Content_Subject,
            sub1.Content_Publising_Date,
            sub1.ContentCategoryName,
            sub1.Expiry_DateOnNotice_Board,
            '~/Home/AttachmentList.aspx?a=' + sub1.Content_Registration_Id AS Url
        FROM (
            SELECT 
                date_timestamp,
                nc.Content_Registration_Id,
                nc.Content_Subject,
                nc.Content_Publising_Date,
                nc.Expiry_DateOnNotice_Board,
                COUNT(ca.FilePath) AS Expr1,
                mc.ContentCategoryName
            FROM 
                NewContent_Create nc
            INNER JOIN 
                ContentAttachment ca ON nc.Content_Registration_Id = ca.Content_Registration_Id
            INNER JOIN 
                Master_ContentCategory mc ON nc.ContentCategoryCode = mc.ContentCategoryCode
            WHERE 
                nc.Status = 'Publish'
                AND nc.ContentCategoryCode IN ('1', '2', '4', '8', '9', '10')
                AND CAST(nc.Expiry_DateOnNotice_Board AS DATE) >= CAST(GETDATE() AS DATE)
                AND CAST(nc.Content_Publising_Date AS DATE) <= CAST(GETDATE() AS DATE)
            GROUP BY 
                date_timestamp, 
                nc.Content_Registration_Id, 
                nc.Content_Subject, 
                nc.Content_Publising_Date, 
                nc.Expiry_DateOnNotice_Board, 
                mc.ContentCategoryName
        ) AS sub1
        INNER JOIN ClickCount cc ON sub1.Content_Registration_Id = cc.Content_Registration_Id
        GROUP BY 
            sub1.Content_Registration_Id, 
            sub1.Content_Subject, 
            sub1.Content_Publising_Date, 
            sub1.ContentCategoryName, 
            sub1.Expiry_DateOnNotice_Board, 
            sub1.date_timestamp
        ORDER BY sub1.date_timestamp DESC";

            var result = await _context.MostVisitedContentDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }




        //https://localhost:7247/api/WebCgmsc/GetContentHeader?contentRegId=201408000001
        [HttpGet("GetContentHeader")]
        public async Task<ActionResult<IEnumerable<GetContentHeaderDTO>>> GetContentHeader(string contentRegId)
        {
            string whContentRegId = "";
            if (contentRegId != "0")
            {
                whContentRegId = " and Content_Registration_Id='" + contentRegId + "'  ";
            }

            string query = $@"  Select Content_Discription,Content_Subject,m.ContentCategoryName,Content_Registration_Id
            from NewContent_Create  inner join Master_ContentCategory m  on m.ContentCategoryCode=NewContent_Create.ContentCategoryCode   where 1=1 " + whContentRegId + @" ";

            var result = await _context.GetContentHeaderDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetContentAttachment?contentRegId=201408000001
        [HttpGet("GetContentAttachment")]
        public async Task<ActionResult<IEnumerable<GetContentAttachmentDTO>>> GetContentAttachment(string contentRegId)
        {
            string whContentRegId = "";
            if (contentRegId != "0")
            {
                whContentRegId = " and Content_Registration_Id='" + contentRegId + "'  ";
            }

            string query = $@"  
                            select FileName,FilePath,Caption,case when ( DATEDIFF (dd , EntryDT , GETDATE()))>7 then 'N' else 'Y' end as DisplayNew, EntryDT
                            from ContentAttachment where 1=1 " + whContentRegId + " order by EntryDT desc ";

            var result = await _context.GetContentAttachmentDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetTenderRef
        [HttpGet("GetTenderRef")]
        public async Task<ActionResult<IEnumerable<GetTenderRefDTO>>> GetTenderRef()
        {

            string query = $@" select  Content_Registration_Id ,Content_Subject  
            from NewContent_Create 
            where CreaterUserName='Technical' and  ContentCategoryCode='3' order by Content_Publising_Date desc ";

            var result = await _context.GetTenderRefDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }



        //https://localhost:7247/api/WebCgmsc/GetDrugTenderListAll
        [HttpGet("GetDrugTenderListAll")]
        public async Task<ActionResult<IEnumerable<GetDrugTenderListAllDTO>>> GetDrugTenderListAll()
        {
            //string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@" SELECT  
    '~/Home/AttachmentList.aspx?a=' + c.Content_Registration_Id AS url,
    c.Content_Registration_Id,
    Attachment_Id,
    caption,
    n.Content_Discription,
    n.Content_Subject AS Subject,
    '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
    CONVERT(varchar, c.EntryDT, 103) AS Content_Publising_Date,
    n.Expiry_Date_of,
    n.Expiry_DateOnNotice_Board,
    CASE 
        WHEN DATEDIFF(dd, EntryDT, GETDATE()) > 30 THEN 'N'
        ELSE 'Y'
    END AS DisplayNew
FROM ContentAttachment c
INNER JOIN NewContent_Create n ON n.Content_Registration_Id = c.Content_Registration_Id
INNER JOIN Master_ContentCategory m ON m.ContentCategoryCode = n.ContentCategoryCode 
-- WHERE  
   -- m.Dept = 'Technical' 
   -- AND m.ContentCategoryCode = '3' 
   -- AND c.TenderFileStatus = 'Publish'  
   -- AND c.Content_Registration_Id = 201408000001
ORDER BY c.EntryDT DESC;
 ";

            var result = await _context.GetDrugTenderListAllDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetEquipmentListAll
        [HttpGet("GetEquipmentListAll")]
        public async Task<ActionResult<IEnumerable<GetDrugTenderListAllDTO>>> GetEquipmentListAll()
        {
            //string topClause = n > 0 ? $"TOP ({n})" : "";

            string query = $@" SELECT  
    '~/Home/AttachmentList.aspx?a=' + c.Content_Registration_Id AS url,
    c.Content_Registration_Id,
    Attachment_Id,
    caption,
    n.Content_Discription,
    n.Content_Subject AS Subject,
    '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
    CONVERT(varchar, c.EntryDT, 103) AS Content_Publising_Date,
    n.Expiry_Date_of,
    n.Expiry_DateOnNotice_Board,
    CASE 
        WHEN DATEDIFF(dd, EntryDT, GETDATE()) > 30 THEN 'N'
        ELSE 'Y'
    END AS DisplayNew
FROM ContentAttachment c
INNER JOIN NewContent_Create n 
    ON n.Content_Registration_Id = c.Content_Registration_Id
INNER JOIN Master_ContentCategory m 
    ON m.ContentCategoryCode = n.ContentCategoryCode 
WHERE  
    m.Dept = 'Technical'
    AND m.ContentCategoryCode = '6'
    AND c.TenderFileStatus = 'Publish'
    AND CAST(n.Content_Publising_Date AS date) <= CAST(GETDATE() AS date)
ORDER BY  
    c.EntryDT DESC

 ";

            var result = await _context.GetDrugTenderListAllDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetCivilTenderListAll
        [HttpGet("GetCivilTenderListAll")]
        public async Task<ActionResult<IEnumerable<CivilTenderAllDTO>>> GetCivilTenderListAll()
        {


            string query = $@"  SELECT  
    '~/Home/AttachmentList.aspx?a=' + c.Content_Registration_Id AS url,
    c.Content_Registration_Id,
    Attachment_Id,
    caption,
    n.Content_Discription,
    n.Content_Subject AS Subject,
    '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
    CONVERT(varchar, c.EntryDT, 103) AS Content_Publising_Date,
    n.Expiry_Date_of,
    n.Expiry_DateOnNotice_Board,
    CASE 
        WHEN DATEDIFF(dd, EntryDT, GETDATE()) > 30 THEN 'N'
        ELSE 'Y'
    END AS DisplayNew
FROM ContentAttachment c
INNER JOIN NewContent_Create n 
    ON n.Content_Registration_Id = c.Content_Registration_Id
INNER JOIN Master_ContentCategory m 
    ON m.ContentCategoryCode = n.ContentCategoryCode 
WHERE  
    m.Dept = 'Civil'
    AND m.ContentCategoryCode = '5'
    AND c.TenderFileStatus = 'Publish'
ORDER BY  
    c.EntryDT DESC;
 ";

            var result = await _context.CivilTenderAllDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetOtherTenderListAll
        [HttpGet("GetOtherTenderListAll")]
        public async Task<ActionResult<IEnumerable<CivilTenderAllDTO>>> GetOtherTenderListAll()
        {


            string query = $@" SELECT  
    '~/Home/AttachmentList.aspx?a=' + c.Content_Registration_Id AS url,
    c.Content_Registration_Id,
    Attachment_Id,
    caption,
    n.Content_Discription,
    n.Content_Subject AS Subject,
    '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
    CONVERT(varchar, c.EntryDT, 103) AS Content_Publising_Date,
    n.Expiry_Date_of,
    n.Expiry_DateOnNotice_Board,
    CASE 
        WHEN DATEDIFF(dd, EntryDT, GETDATE()) > 30 THEN 'N'
        ELSE 'Y'
    END AS DisplayNew
FROM ContentAttachment c
INNER JOIN NewContent_Create n 
    ON n.Content_Registration_Id = c.Content_Registration_Id
INNER JOIN Master_ContentCategory m 
    ON m.ContentCategoryCode = n.ContentCategoryCode 
WHERE  
    m.ContentCategoryCode IN ('7', '12')
    AND c.TenderFileStatus = 'Publish'
ORDER BY  
    c.EntryDT DESC;

 ";

            var result = await _context.CivilTenderAllDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetNoticCircular
        [HttpGet("GetNoticCircular")]
        public async Task<ActionResult<IEnumerable<GetNoticCircularDTO>>> GetNoticCircular()
        {


            string query = $@" SELECT 
    dbo.getAttCount(dbo.NewContent_Create.Content_Registration_Id) AS countATT,
    dbo.Master_ContentCategory.ContentCategoryCode AS Expr1,
    dbo.NewContent_Create.Content_Registration_Id,
    dbo.NewContent_Create.Content_Subject,
    CASE 
        WHEN (CAST(dbo.NewContent_Create.Expiry_DateOnNotice_Board AS DATE) < CAST(GETDATE() AS DATE)) 
        THEN 'N' 
        ELSE 'Y' 
    END AS DisplayNew,
    dbo.NewContent_Create.CreaterUserName AS Department,
    dbo.NewContent_Create.Content_Discription,
    CONVERT(VARCHAR, dbo.NewContent_Create.Content_Publising_Date, 103) AS Content_Publising_Date,
    CONVERT(VARCHAR, dbo.NewContent_Create.Expiry_DateOnNotice_Board, 103) AS Expiry_DateOnNotice_Board,
    dbo.NewContent_Create.Expiry_Date_of AS Expiry_DateOnDepartment_Board,
    dbo.NewContent_Create.Status,
    dbo.NewContent_Create.Date_TimeStamp,
    dbo.NewContent_Create.Ip,
    dbo.NewContent_Create.SystemInfo,
    dbo.NewContent_Create.CreaterUserName,
    dbo.NewContent_Create.ContentCategoryCode,
    dbo.Master_ContentCategory.ContentCategoryName
FROM 
    dbo.NewContent_Create
INNER JOIN 
    dbo.Master_ContentCategory 
    ON dbo.NewContent_Create.ContentCategoryCode = dbo.Master_ContentCategory.ContentCategoryCode
WHERE 
    dbo.NewContent_Create.Status = 'Publish'
    AND dbo.NewContent_Create.ContentCategoryCode IN ('8','9','10','11')
ORDER BY 
    dbo.NewContent_Create.Content_Publising_Date DESC


 ";

            var result = await _context.GetNoticCircularDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetDept
        [HttpGet("GetDept")]
        public async Task<ActionResult<IEnumerable<GetDeptDTO>>> GetDept()
        {


            string query = $@" select distinct CoreDept from CGMSC_Team ";

            var result = await _context.GetDeptDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetEmployee?coreDept=Technical-Drug
        [HttpGet("GetEmployee")]
        public async Task<ActionResult<IEnumerable<GetEmployeeDTO>>> GetEmployee(string coreDept)
        {
            string whCoreDept = "";

            if (coreDept != "0")
            {
                whCoreDept = "  and CoreDept='" + coreDept + "' ";
            }


            string query = $@" select Initial +' '+ Name as FullName,Designation,Department,EmailId,ContactNo from CGMSC_Team  where 1=1 " + whCoreDept + " and (IsResigned is null or isResigned='') ORDER BY OrderNo ";

            var result = await _context.GetEmployeeDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetProductBlacklisted
        [HttpGet("GetProductBlacklisted")]
        public async Task<ActionResult<IEnumerable<GetProductBlacklistedDTO>>> GetProductBlacklisted()
        {


            string query = $@" select NameofProduct,NameOfFirm,Address,convert(varchar(19),fromdate,105)Fromdate,convert(varchar(19),upto,105)upto, ReasonOfBlacklisting,isnull(Spremarks,'-') as Spremarks 
from firmblacklisted
 where NameofProduct!=''  and  isactiveproduct = 1 and upto>=getdate() order by fromdate ";

            var result = await _context.GetProductBlacklistedDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetFirmBlacklisted
        [HttpGet("GetFirmBlacklisted")]
        public async Task<ActionResult<IEnumerable<GetFirmBlacklistedDTO>>> GetFirmBlacklisted()
        {


            string query = $@"  select NameOfFirm,Address,convert(varchar(19),fromdate,105)fromdate,convert(varchar(19),upto,105)upto, ReasonOfBlacklisting,isnull(Spremarks,'-') as Spremarks  
 from firmblacklisted 
 where isactive = 1 and NameofProduct is null or NameofProduct='' and upto>=getdate() order by upto asc ";

            var result = await _context.GetFirmBlacklistedDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        //https://localhost:7247/api/WebCgmsc/GetEqpProductBlacklisted
        [HttpGet("GetEqpProductBlacklisted")]
        public async Task<ActionResult<IEnumerable<GetEqpProductBlacklistedDTO>>> GetEqpProductBlacklisted()
        {


            string query = $@"  select NameofProduct,NameOfFirm,Address,convert(varchar(19),fromdate,105)Fromdate,convert(varchar(19),upto,105)upto, ReasonOfBlacklisting 
 from firmblacklisted_Equipment 
 where NameofProduct!=''  and  isactiveproduct = 1 order by upto desc ";

            var result = await _context.GetEqpProductBlacklistedDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }


        //https://localhost:7247/api/WebCgmsc/GetEqpBlacklistedFirms
        [HttpGet("GetEqpBlacklistedFirms")]
        public async Task<ActionResult<IEnumerable<GetEqpBlacklistedFirmsDTO>>> GetEqpBlacklistedFirms()
        {


            string query = $@"   select NameOfFirm,Address,convert(varchar(19),fromdate,105)fromdate,convert(varchar(19),upto,105)upto, ReasonOfBlacklisting 
 from firmblacklisted_Equipment 
 where isactive = 1 and NameofProduct is null or NameofProduct='' order by upto asc ";

            var result = await _context.GetEqpBlacklistedFirmsDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }



        [HttpGet("GetContentCategory")]
        public async Task<ActionResult<IEnumerable<ContentCategoryDTO>>> GetContentCategory(string s1)
        {
            string query = "";

            if (s1 == "DMAdmin")
            {
                query = @"SELECT ContentCategoryCode, ContentCategoryName 
                  FROM Master_ContentCategory 
                  WHERE Dept = 'HRA'";
            }
            else if (s1 == "SE")
            {
                query = @"SELECT ContentCategoryCode, ContentCategoryName 
                  FROM Master_ContentCategory 
                  WHERE Dept = 'Civil'";
            }
            else if (s1 == "GMFinance")
            {
                query = @"SELECT ContentCategoryCode, ContentCategoryName 
                  FROM Master_ContentCategory 
                  WHERE Dept = 'FIN'";
            }
            else if (s1 == "GMTechnical")
            {
                query = @"SELECT ContentCategoryCode, ContentCategoryName 
                  FROM Master_ContentCategory 
                  WHERE Dept = 'Technical'";
            }
            else if (s1 == "Dept" || s1 == "Archive")
            {
                query = @"SELECT ContentCategoryCode, ContentCategoryName 
                  FROM Master_ContentCategory 
                  WHERE Dept = 'HRA' 
                  AND ContentCategoryCode NOT IN ('7','8')";
            }
            else
            {
                query = @$"SELECT ContentCategoryCode, ContentCategoryName 
                   FROM Master_ContentCategory 
                   WHERE Dept = '{s1}'";
            }

            var result = await _context.ContentCategoryDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }




        //https://localhost:7247/api/WebCgmsc/GetHRarchiveParticular
        [HttpGet("GetHRarchiveParticular")]
        public async Task<ActionResult<IEnumerable<GetHRarchiveParticularDTO>>> GetHRarchiveParticular(Int32 contentCategoryCode)
        {
            string whcontentCategoryCode = "";
            if (contentCategoryCode != 0)
            {
                whcontentCategoryCode = @" and dbo.NewContent_Create.ContentCategoryCode=	" + contentCategoryCode + @"  ";
            }

            string query = $@" SELECT 
                CAST(dbo.getAttCount(dbo.NewContent_Create.Content_Registration_Id) AS VARCHAR) AS CountATT,
                CAST(dbo.Master_ContentCategory.ContentCategoryCode AS VARCHAR) AS CategoryCodeMaster,
                CAST(dbo.NewContent_Create.ContentCategoryCode AS VARCHAR) AS CategoryCodeContent,
                CAST(dbo.NewContent_Create.Content_Registration_Id AS VARCHAR) AS ContentRegistrationId,
                dbo.NewContent_Create.Content_Subject AS ContentSubject,
                CASE 
                    WHEN (CAST(dbo.NewContent_Create.Expiry_DateOnNotice_Board AS DATE) < CAST(GETDATE() AS DATE))
                    THEN 'N' 
                    ELSE 'Y' 
                END AS DisplayNew,
                CAST(YEAR(dbo.NewContent_Create.Expiry_DateOnNotice_Board) AS VARCHAR) AS ExpiryYear,
                CAST(YEAR(GETDATE()) AS VARCHAR) AS CurrentYear,
                dbo.NewContent_Create.Department AS Department,
                dbo.NewContent_Create.Content_Discription AS ContentDescription,
                CONVERT(VARCHAR, dbo.NewContent_Create.Content_Publising_Date, 103) AS ContentPublishingDate,
                CONVERT(VARCHAR, dbo.NewContent_Create.Expiry_DateOnNotice_Board, 103) AS ExpiryDateOnNoticeBoard,
                CONVERT(VARCHAR, dbo.NewContent_Create.Expiry_Date_of, 103) AS ExpiryDateOnDepartmentBoard,
                dbo.NewContent_Create.Status AS Status,
                CONVERT(VARCHAR, dbo.NewContent_Create.Date_TimeStamp, 120) AS DateTimeStamp,
                dbo.NewContent_Create.Ip AS Ip,
                dbo.NewContent_Create.SystemInfo AS SystemInfo,
                dbo.NewContent_Create.CreaterUserName AS CreaterUserName,
                dbo.Master_ContentCategory.ContentCategoryName AS ContentCategoryName
            FROM 
                dbo.NewContent_Create
            INNER JOIN 
                dbo.Master_ContentCategory 
                ON dbo.NewContent_Create.ContentCategoryCode = dbo.Master_ContentCategory.ContentCategoryCode
           WHERE     (dbo.NewContent_Create.Status = 'Publish') and CreaterUserName='HRA' " + whcontentCategoryCode + @" and len(dbo.NewContent_Create.RecruitmentDept)!=1 
                       and  cast(dbo.NewContent_Create.Expiry_DateOnNotice_Board as Date) < cast(GETDATE()as Date)
            ORDER BY 
                dbo.NewContent_Create.Date_TimeStamp DESC;

             ";


            //            string query = $@" SELECT 
            //    dbo.getAttCount(dbo.NewContent_Create.Content_Registration_Id) AS countATT, 
            //    dbo.Master_ContentCategory.ContentCategoryCode AS Expr1, 
            //    dbo.NewContent_Create.Content_Registration_Id, 
            //    dbo.NewContent_Create.Content_Subject, 
            //    dbo.NewContent_Create.Department, 
            //    dbo.NewContent_Create.Content_Discription,
            //    CONVERT(varchar, dbo.NewContent_Create.Content_Publising_Date, 103) AS Content_Publising_Date,
            //    dbo.NewContent_Create.Expiry_DateOnNotice_Board, 
            //    dbo.NewContent_Create.Expiry_Date_of AS Expiry_DateOnDepartment_Board, 
            //    dbo.NewContent_Create.Status, 
            //    dbo.NewContent_Create.Date_TimeStamp, 
            //    dbo.NewContent_Create.Ip, 
            //    dbo.NewContent_Create.SystemInfo, 
            //    dbo.NewContent_Create.CreaterUserName, 
            //    dbo.NewContent_Create.ContentCategoryCode,
            //    dbo.Master_ContentCategory.ContentCategoryName 
            //FROM dbo.NewContent_Create
            //INNER JOIN dbo.Master_ContentCategory 
            //    ON dbo.NewContent_Create.ContentCategoryCode = dbo.Master_ContentCategory.ContentCategoryCode
            //WHERE 
            //    dbo.NewContent_Create.Status = 'Publish' 
            //    AND CreaterUserName = 'HRA'
            //    AND LEN(dbo.NewContent_Create.RecruitmentDept) != 1 
            //    AND dbo.NewContent_Create.ContentCategoryCode NOT IN ('7', '8')
            //    AND CAST(dbo.NewContent_Create.Expiry_DateOnNotice_Board AS DATE) < CAST(GETDATE() AS DATE)
            //ORDER BY 
            //    dbo.NewContent_Create.Content_Publising_Date DESC;
            // ";




            var result = await _context.GetHRarchiveParticularDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetRecruitmentDepartments")]
        public async Task<ActionResult<IEnumerable<RecruitmentDeptDTO>>> GetRecruitmentDepartments()
        {
            string query = @"SELECT RecruitmentId, RecruitmentScheme FROM RecruitmentDept";

            var result = await _context.RecruitmentDeptDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetHRPartDeptCatParticular")]
        public async Task<ActionResult<IEnumerable<HRContentDeptCatDTO>>> GetHRPartDeptCatParticular(string s1, string s2)
        {

            string whS1 = "";
            string whS2 = "";

            if (s1 != "0")
            {
                whS1 = "  AND dbo.NewContent_Create.RecruitmentDept = "+ s1 +"  ";
            }

            if (s2 != "0")
            {
                whS2 = " AND dbo.NewContent_Create.ContentCategoryCode = "+ s2 + "  ";
            }

            var query = @"
        SELECT  
            dbo.getAttCount(dbo.NewContent_Create.Content_Registration_Id) AS CountATT,
            dbo.Master_ContentCategory.ContentCategoryCode AS Expr1,
            dbo.NewContent_Create.Content_Registration_Id,
            dbo.NewContent_Create.Content_Subject,
            CASE 
                WHEN CAST(dbo.NewContent_Create.Expiry_DateOnNotice_Board AS DATE) < CAST(GETDATE() AS DATE) 
                THEN 'N' ELSE 'Y' 
            END AS DisplayNew,
            dbo.NewContent_Create.Department,
            dbo.NewContent_Create.Content_Discription,
            CONVERT(VARCHAR, dbo.NewContent_Create.Content_Publising_Date, 103) AS Content_Publising_Date,
            dbo.NewContent_Create.Expiry_DateOnNotice_Board,
            dbo.NewContent_Create.Expiry_Date_of AS Expiry_DateOnDepartment_Board,
            dbo.NewContent_Create.Status,
            dbo.NewContent_Create.Date_TimeStamp,
            dbo.NewContent_Create.CreaterUserName,
            dbo.NewContent_Create.ContentCategoryCode,
            CASE 
                WHEN LEN(Expiry_Date_of) = 10 THEN Expiry_Date_of 
                ELSE 'N.A.' 
            END AS Expiry_Date_of,
            RecruitmentDept.RecruitmentScheme,
            dbo.Master_ContentCategory.ContentCategoryName 
        FROM dbo.NewContent_Create 
        INNER JOIN dbo.Master_ContentCategory 
            ON dbo.NewContent_Create.ContentCategoryCode = dbo.Master_ContentCategory.ContentCategoryCode
        INNER JOIN RecruitmentDept 
            ON dbo.NewContent_Create.RecruitmentDept = RecruitmentDept.Recruitmentid
        WHERE dbo.NewContent_Create.Status = 'Publish' 
            AND CreaterUserName = 'HRA' 
            AND LEN(RecruitmentDept) < 4 
            "+ whS1 + @" 
            "+ whS2 + @"
        ORDER BY dbo.NewContent_Create.Content_Publising_Date DESC";

            var result = await _context.HRContentDeptCatDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetQCTenderAttachments")]
        public async Task<ActionResult<IEnumerable<QCTenderAttachmentDTO>>> GetQCTenderAttachments(string registrationId)
        {

            string whregistrationId = "";

            if (registrationId != "0")
            {
                whregistrationId = "  AND c.Content_Registration_Id = "+ registrationId + "  ";
            }

            string query = $@"
        SELECT 
            '~/Home/AttachmentList.aspx?a=' + c.Content_Registration_Id AS Url,
            c.Content_Registration_Id,
            Attachment_Id,
            caption,
            n.Content_Discription,
            n.Content_Subject AS Subject,
            '(' + c.caption + ') :- ' + n.Content_Subject AS Content_Subject,
            CONVERT(varchar, n.Content_Publising_Date, 103) AS Content_Publising_Date,
            n.Expiry_Date_of,
            n.Expiry_DateOnNotice_Board,
            CASE 
                WHEN DATEDIFF(dd, EntryDT, GETDATE()) > 30 THEN 'N' 
                ELSE 'Y' 
            END AS DisplayNew
        FROM ContentAttachment c
        INNER JOIN NewContent_Create n ON n.Content_Registration_Id = c.Content_Registration_Id
        INNER JOIN Master_ContentCategory m ON m.ContentCategoryCode = n.ContentCategoryCode
        WHERE 
            m.Dept = 'Technical'
            AND m.ContentCategoryCode = '12'
            AND c.TenderFileStatus = 'Publish'
            "+ whregistrationId + @"
        ORDER BY c.EntryDT DESC";

          

            var result = await _context.QCTenderAttachmentDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("GetNewsLightBoxItems")]
        public async Task<ActionResult<IEnumerable<DynamicLightBoxDTO>>> GetNewsLightBoxItems(String type)  //news, civil
        {

            string whType = "";

            if (type != "0")
            {
                whType = "  and Type = '" + type +"'  ";
            }

            string query = @"SELECT id, Description, ImageName, Type FROM tblDynamicLightBox WHERE 1=1 "+ whType +"";

            var result = await _context.DynamicLightBoxDbSet
                .FromSqlRaw(query)
                .ToListAsync();

            return Ok(result);
        }
    }
}
