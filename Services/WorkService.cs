using Newtonsoft.Json;
using SyncSharpServer.Common.ExceptionHandling;
using SyncSharpServer.Entities;
using SyncSharpServer.Interfaces;
using SyncSharpServer.Models;
using SyncSharpServer.Models.RequestModels;
using SyncSharpServer.ResponseDTOs;

namespace SyncSharpServer.Services
{
    public class WorkService : IWorkService
    {
        private readonly IWorkRepository _workRepository;
        private readonly ILogger _logger;

        public WorkService(IWorkRepository workRepository, ILogger<WorkService> logger)
        {
            _workRepository = workRepository;
            _logger = logger;
        }

        public async Task<GeneralResponse<WorkDTO>> GetWorkByID(Guid WorkID, CancellationToken cancellationToken)
        {
            try
            {
                var work = await _workRepository.GetWorkByID(WorkID, cancellationToken);
                if (work == null)
                {
                    _logger.LogError("Work item not found for the specified WorkID. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
                    throw new BadRequestException(["Work item not found for the specified WorkID"]);
                }
                return CreateSuccessResponse<WorkDTO>(work);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.GetWorkByID. Input parameters: {InputParams}", WorkID);
                throw;
            }
        }

        public async Task<GeneralResponse<List<WorkDTO>>> GetUserWorks(Guid UserID, CancellationToken cancellationToken)
        {
            try
            {
                var works = await _workRepository.GetUserWorks(UserID, cancellationToken);

                if (works == null || works.Count == 0)
                {
                    _logger.LogError("Work item not found for the specified UserID. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { UserID }));
                    throw new BadRequestException(["Work item not found for the specified UserID"]);
                }
                return CreateSuccessResponse<List<WorkDTO>>(works);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.GetUserWorks. Input parameters: {InputParams}", UserID);
                throw;
            }
        }

        public async Task<GeneralResponse<Guid>> CreateWork(CreateWorkRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var workID = await _workRepository.CreateWork(request, cancellationToken);
                if (workID == Guid.Empty)
                {
                    _logger.LogError("Failed to create work item. Returned WorkID is empty. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                    throw new BadRequestException(["Failed to create work item"]);
                }
                return CreateSuccessResponse<Guid>(workID);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.CreateWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                throw;
            }
        }

        public async Task<GeneralResponse<bool>> UpdateWork(UpdateWorkRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var workUpdated = await _workRepository.UpdateWork(request, cancellationToken);
                if (!workUpdated)
                {
                    _logger.LogError("Failed to update work. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                    throw new BadRequestException(["Failed to update work."]);
                }
                return CreateSuccessResponse<bool>(workUpdated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.UpdateWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                throw;
            }
        }

        public async Task<GeneralResponse<bool>> DeleteWork(Guid WorkID, CancellationToken cancellationToken)
        {
            try
            {
                var workDeleted = await _workRepository.DeleteWork(WorkID, cancellationToken);
                if (!workDeleted)
                {
                    _logger.LogError("Failed to delete work. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
                    throw new BadRequestException(["Failed to delete work."]);
                }
                return CreateSuccessResponse<bool>(workDeleted);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.DeleteWork. Input parameters: {InputParams}", JsonConvert.SerializeObject(new { WorkID }));
                throw;
            }
        }

        public async Task<GeneralResponse<bool>> AddWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var memberAdded = await _workRepository.AddWorkMember(request, cancellationToken);
                if (!memberAdded)
                {
                    _logger.LogError("Failed to add member. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                    throw new BadRequestException(["Failed to add member."]);
                }
                return CreateSuccessResponse<bool>(memberAdded);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.AddWorkMember. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                throw;
            }
        }

        public async Task<GeneralResponse<bool>> DeleteWorkMember(AddDeleteWorkMemberRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                var memberDeleted = await _workRepository.DeleteWorkMember(request, cancellationToken);
                if (!memberDeleted)
                {
                    _logger.LogError("Failed to delete member. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                    throw new BadRequestException(["Failed to delete member."]);
                }
                return CreateSuccessResponse<bool>(memberDeleted);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error thrown in WorkService.DeleteWorkMember. Input parameters: {InputParams}", JsonConvert.SerializeObject(request));
                throw;
            }
        }

        GeneralResponse<T> CreateSuccessResponse<T>(T data)
        {
            return new GeneralResponse<T>
            {
                Data = data,
                Success = true,
                RequestId = Guid.NewGuid().ToString(),
                ResponseMessage = "Request processed successfully"
            };
        }
    }
}
