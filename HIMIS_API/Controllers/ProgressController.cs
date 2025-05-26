using HIMIS_API.Models.DTOs;
using HIMIS_API.Services.ProgressServices.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace HIMIS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly IMainSchemeService _mainSchemeService;
        public ProgressController(IMainSchemeService mainSchemeService)
        {
            _mainSchemeService = mainSchemeService;
        }

        [HttpGet("GetValuePara")]
        public async Task<ActionResult<IEnumerable<MasValueParaDTO>>> GetValuePara()
        {
            var myList = await _mainSchemeService.GetValuePara();
            return myList.ToList();
        }

        [HttpGet("getMainScheme")]
        public async Task<ActionResult<IEnumerable<MainSchemeDTO>>> getMainScheme(bool isall)
        {
            var myList = await _mainSchemeService.GetMainSchemeAsync(isall);
            return myList.ToList();
        }

        [HttpGet("getDivision")]
        public async Task<ActionResult<IEnumerable<DivisionNameDTO>>> GetDivision(bool isall,string divisionId)
        {
            var divisions = await _mainSchemeService.GetDivisionsAsync(isall,divisionId);
            return divisions.ToList();
        }

        [HttpGet("GetDistrict")]
        public async Task<ActionResult<IEnumerable<DistrictNameDTO>>> GetDistrict(bool isall,string divisionId)
        {
            var districts = await _mainSchemeService.GetDistrictsAsync(isall,divisionId);
            return districts.ToList();
        }

        [HttpGet("GetPProgressLevel")]
        public async Task<ActionResult<IEnumerable<ProgressLevelDTO>>> GetPProgressLevel(bool isall,string ppid)
        {
            var progressLevels = await _mainSchemeService.GetProgressLevelsAsync(isall,ppid);
            return progressLevels.ToList();
        }

        [HttpGet("GetHealthCentre")]
        public async Task<ActionResult<IEnumerable<HealthCentreDTO>>> GetHealthCentre(string distId, string divisionId)
        {
            var healthCentres = await _mainSchemeService.GetHealthCentresAsync(distId, divisionId);
            return healthCentres.ToList();
        }

        [HttpGet("GetWorkDetail")]
        public async Task<ActionResult<IEnumerable<WorkDetailDTO>>> GetWorkDetail(string divisionId, string mainSchemeId, string workName, string compareStatus,string ppid, string wopending)
        {
            var workDetails = await _mainSchemeService.GetWorkDetailAsync(divisionId, mainSchemeId, workName, compareStatus,ppid, wopending);
            return workDetails.ToList();
        }

        [HttpGet("GetWorkOrderPending")]
        public async Task<ActionResult<IEnumerable<WOPendingDivisionDTO>>> GetWorkOrderPending(string divisionId)
        {
            var healthCentres = await _mainSchemeService.GetWorkOrderPending(divisionId);
            return healthCentres.ToList();
        }

        [HttpGet("GetWorkOrderPendingDetails")]
        public async Task<ActionResult<IEnumerable<WOPendingDetailDTO>>> getWorkOrderPendingDetails(string divisionId, string mainSchemeId, string isvaluePara)
        {
            var healthCentres = await _mainSchemeService.getWorkOrderPendingDetails(divisionId, mainSchemeId, isvaluePara);
            return healthCentres.ToList();
        }

        [HttpGet("GetWorkOrderHead")]
        public async Task<ActionResult<IEnumerable<MainSchemeDTO>>> GetWorkOrderHead(string Type, string divisionid)
        {
            var healthCentres = await _mainSchemeService.GetWorkOrderHead(Type, divisionid);
            return healthCentres.ToList();
        }

        [HttpGet("GetMasContractor")]
        public async Task<ActionResult<IEnumerable<MasContractorDTO>>> GetMasContractor()
        {
            var healthCentres = await _mainSchemeService.MasContractor();
            return healthCentres.ToList();
        }

        [HttpGet("GetContractorWorks")]
        public async Task<ActionResult<IEnumerable<WorkContractorDTO>>> GetContractorWorks(string divisionId, string mainSchemeId, string cid, string rpending)
        {
            var healthCentres = await _mainSchemeService.GetContractorWorks(divisionId, mainSchemeId, cid, rpending);
            return healthCentres.ToList();
        }

        [HttpGet("GetDivWiseProgress")]
        public async Task<ActionResult<IEnumerable<DivPerformanceDTO>>>GetDivWiseProgress(string divisionId,string mainSchemeId,string cid, string rpending)
        {
            var healthCentres = await _mainSchemeService.DivWiseProgress(divisionId, mainSchemeId, cid, rpending);
            return healthCentres.ToList();
        }


        [HttpGet("DashProgressCount")]
        public async Task<ActionResult<IEnumerable<ProgressCountDTO>>> DashProgressCount(string divisionId, string mainSchemeId, string distid, string ASAmount, string GrantID, string ASID)
        {
            var healthCentres = await _mainSchemeService.ProgressCount(divisionId, mainSchemeId, distid,  ASAmount,  GrantID,  ASID);
            return healthCentres.ToList();
        }

        [HttpGet("DashProgressDistCount")]
        public async Task<ActionResult<IEnumerable<DistProgressCountDTO>>> DashProgressDistCount(string divisionId, string mainSchemeId, string dashID)
        {
            var healthCentres = await _mainSchemeService.ProgressDistCount(divisionId, mainSchemeId, dashID);
            return healthCentres.ToList();
        }
       


    }
}
