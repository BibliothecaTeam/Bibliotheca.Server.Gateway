using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IGroupsService
    {
        Task<IList<GroupDto>> GetGroupsAsync();

        Task<GroupDto> GetGroupAsync(string groupName);

        Task CreateGroupAsync(GroupDto group);

        Task UpdateGroupAsync(string groupName, GroupDto group);

        Task DeleteGroupAsync(string groupName);
    }
}