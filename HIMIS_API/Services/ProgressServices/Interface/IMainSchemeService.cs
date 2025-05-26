using HIMIS_API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HIMIS_API.Services.ProgressServices.Interface
{
    public interface IMainSchemeService
    {
     
        Task<IEnumerable<MasValueParaDTO>> GetValuePara();
        Task <IEnumerable<MainSchemeDTO>> GetMainSchemeAsync(bool isall);

        Task<IEnumerable<MasContractorDTO>> MasContractor();

        Task<IEnumerable<MainSchemeDTO>> GetWorkOrderHead(string Type, string divisionid);
        Task<IEnumerable<DivisionNameDTO>> GetDivisionsAsync(bool isall,string divisionId);
        Task<IEnumerable<DistrictNameDTO>> GetDistrictsAsync(bool isall,string divisionId);
        Task<IEnumerable<ProgressLevelDTO>> GetProgressLevelsAsync(bool isall,string ppid);
        Task<IEnumerable<HealthCentreDTO>> GetHealthCentresAsync(string distId, string divisionId);
        Task<IEnumerable<WorkDetailDTO>> GetWorkDetailAsync(string divisionId, string mainSchemeId, string workName, string compareStatus,string ppid,string wopending);
        Task<IEnumerable<WOPendingDivisionDTO>> GetWorkOrderPending(string divisionId);

        Task<IEnumerable<WOPendingDetailDTO>> getWorkOrderPendingDetails(string divisionId, string mainSchemeId, string isvaluePara);
        Task<IEnumerable<WorkContractorDTO>> GetContractorWorks(string divisionId, string mainSchemeId, string cid, string rpending);
        Task<IEnumerable<DivPerformanceDTO>> DivWiseProgress(string divisionId, string mainSchemeId, string cid, string rpending);
        Task<IEnumerable<ProgressCountDTO>> ProgressCount(string divisionId, string mainSchemeId, string distid,string  ASAmount, string GrantID, string ASID);
        Task<IEnumerable<DistProgressCountDTO>> ProgressDistCount(string divisionId, string mainSchemeId, string dashID);
        
   
    }
}
