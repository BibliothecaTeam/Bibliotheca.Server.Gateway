using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IGroupsClient
    {
        Task<IList<GroupDto>> Get();

        Task<GroupDto> Get(string groupName);

        Task<HttpResponseMessage> Post(GroupDto group);

        Task<HttpResponseMessage> Put(string groupName, GroupDto group);

        Task<HttpResponseMessage> Delete(string groupName);
    }
}