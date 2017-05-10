using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class CacheService : ICacheService
    {
        private const string _tagsCacheKey = "TagsService";

        private const string _groupsCacheKey = "GroupsService";

        private const string _allProjectsInformationCacheKey = "ProjectsService";

        private const string _allUsersInformationCacheKey = "UsersService";

        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool TryGetTags(out IList<string> tags)
        {
            return _memoryCache.TryGetValue(_tagsCacheKey, out tags);
        }

        public void AddTags(IList<string> tags)
        {
            _memoryCache.Set(_tagsCacheKey, tags,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        public void ClearTagsCache()
        {
            _memoryCache.Remove(_tagsCacheKey);
        }

        public bool TryGetBranches(string projectId, out IList<ExtendedBranchDto> branches)
        {
            var cacheKey = GetBranchesCacheKey(projectId);
            return _memoryCache.TryGetValue(cacheKey, out branches);
        }

        public void AddBranches(string projectId, IList<ExtendedBranchDto> branches)
        {
            var cacheKey = GetBranchesCacheKey(projectId);
            _memoryCache.Set(cacheKey, branches, 
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        } 

        public void ClearBranchesCache(string projectId)
        {
            var cacheKey = GetBranchesCacheKey(projectId);
            _memoryCache.Remove(cacheKey);
        }

        private string GetBranchesCacheKey(string projectId) 
        {
            return $"BranchesService#{projectId}";
        }

        public bool TryGetDocument(string projectId, string branchName, string fileUri, out DocumentDto documentDto)
        {
            var cacheKey = GetDocumentCacheKey(projectId, branchName, fileUri);
            return _memoryCache.TryGetValue(cacheKey, out documentDto);
        }

        public void AddDocument(string projectId, string branchName, string fileUri, DocumentDto documentDto)
        {
            var cacheKey = GetDocumentCacheKey(projectId, branchName, fileUri);
            _memoryCache.Set(cacheKey, documentDto, 
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        public void ClearDocumentCache(string projectId, string branchName, string fileUri)
        {
            var cacheKey = GetDocumentCacheKey(projectId, branchName, fileUri);
            _memoryCache.Remove(cacheKey);
        }

        private string GetDocumentCacheKey(string projectId, string branchName, string fileUri)
        {
            return $"DocumentsService#{projectId}#{branchName}#{fileUri}";
        }

        public bool TryGetGroups(out IList<string> tags)
        {
            return _memoryCache.TryGetValue(_groupsCacheKey, out tags);
        }

        public void AddGroups(IList<string> tags)
        {
            _memoryCache.Set(_groupsCacheKey, tags,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        public void ClearGroupsCache()
        {
            _memoryCache.Remove(_groupsCacheKey);
        }

        public bool TryGetProjects(out IList<ProjectDto> projects)
        {
            return _memoryCache.TryGetValue(_allProjectsInformationCacheKey, out projects);
        }

        public void AddProjectsToCache(IList<ProjectDto> projects)
        {
            _memoryCache.Set(_allProjectsInformationCacheKey, projects,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        public void ClearProjectsCache()
        {
            _memoryCache.Remove(_allProjectsInformationCacheKey);
        }

        public bool TryGetTableOfContents(string projectId, string branchName, out IList<ChapterItemDto> toc)
        {
            var cacheKey = GetTableOfContentsCacheKey(projectId, branchName);
            return _memoryCache.TryGetValue(cacheKey, out toc);
        }

        public void AddTableOfContentsToCache(string projectId, string branchName, IList<ChapterItemDto> toc)
        {
            var cacheKey = GetTableOfContentsCacheKey(projectId, branchName);
            _memoryCache.Set(cacheKey, toc,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        public void ClearTableOfContentsCache(string projectId, string branchName)
        {
            var cacheKey = GetTableOfContentsCacheKey(projectId, branchName);
            _memoryCache.Remove(cacheKey);
        }

        private string GetTableOfContentsCacheKey(string projectId, string branchName)
        {
            return $"TableOfContentsService#{projectId}#{branchName}";
        }

        public bool TryGetUsers(out IList<UserDto> users)
        {
            return _memoryCache.TryGetValue(_allUsersInformationCacheKey, out users);
        }

        public void AddUsers(IList<UserDto> users)
        {
            _memoryCache.Set(_allUsersInformationCacheKey, users,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        public void ClearUsersCache()
        {
            _memoryCache.Remove(_allUsersInformationCacheKey);
        }
    }
}