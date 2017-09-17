using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Neutrino.Entities.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface ICacheService
    {
        bool TryGetTags(out IList<string> tags);
        void AddTags(IList<string> tags);
        void ClearTagsCache();

        bool TryGetBranches(string projectId, out IList<ExtendedBranchDto> branches);
        void AddBranches(string projectId, IList<ExtendedBranchDto> branches);
        void ClearBranchesCache(string projectId);

        bool TryGetDocument(string projectId, string branchName, string fileUri, out DocumentDto documentDto);
        void AddDocument(string projectId, string branchName, string fileUri, DocumentDto documentDto);
        void ClearDocumentCache(string projectId, string branchName, string fileUri);

        bool TryGetGroups(out IList<GroupDto> groups);
        void AddGroups(IList<GroupDto> groups);
        void ClearGroupsCache();

        bool TryGetProjects(out IList<ProjectDto> projects);
        void AddProjectsToCache(IList<ProjectDto> projects);
        void ClearProjectsCache();

        bool TryGetTableOfContents(string projectId, string branchName, out IList<ChapterItemDto> toc);
        void AddTableOfContentsToCache(string projectId, string branchName, IList<ChapterItemDto> toc);
        void ClearTableOfContentsCache(string projectId, string branchName);

        bool TryGetUsers(out IList<UserDto> users);
        void AddUsers(IList<UserDto> users);
        void ClearUsersCache();

        bool TryGetService(string serviceType, out Service service);
        void AddService(string serviceType, Service service);
    }
}