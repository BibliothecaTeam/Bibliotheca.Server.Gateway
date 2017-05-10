using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;
using YamlDotNet.Serialization;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class BranchesService : IBranchesService
    {
        private readonly IBranchesClient _branchesClient;

        private readonly ICacheService _cacheService;

        public BranchesService(IBranchesClient branchesClient, ICacheService cacheService)
        {
            _branchesClient = branchesClient;
            _cacheService = cacheService;
        }

        public async Task<IList<ExtendedBranchDto>> GetBranchesAsync(string projectId)
        {
            IList<ExtendedBranchDto> branches = null;
            if(!_cacheService.TryGetBranches(projectId, out branches))
            {
                var branchesDto = await _branchesClient.Get(projectId);
                if(branchesDto == null)
                {
                    return null;
                }

                branches = branchesDto.Select(x => CreateExtendedBranchDto(x)).ToList();
                _cacheService.AddBranches(projectId, branches);
            }
            
            return branches;
        }

        public async Task<ExtendedBranchDto> GetBranchAsync(string projectId, string branchName)
        {
            var branch = await _branchesClient.Get(projectId, branchName);
            if(branch == null)
            {
                return null;
            }

            return CreateExtendedBranchDto(branch);
        }

        public async Task CreateBranchAsync(string projectId, BranchDto branch)
        {
            var result = await _branchesClient.Post(projectId, branch);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new CreateBranchException("During creating the new branch error occurs: " + content);
            }

            _cacheService.ClearBranchesCache(projectId);
        }

        public async Task UpdateBranchAsync(string projectId, string branchName, BranchDto branch)
        {
            var result = await _branchesClient.Put(projectId, branchName, branch);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateBranchException("During updating the branch error occurs: " + content);
            }

            _cacheService.ClearBranchesCache(projectId);
            _cacheService.ClearTableOfContentsCache(projectId, branchName);
        }
        
        public async Task DeleteBranchAsync(string projectId, string branchName)
        {
            var result = await _branchesClient.Delete(projectId, branchName);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteBranchException("During deleteing the branch error occurs: " + content);
            }

            _cacheService.ClearBranchesCache(projectId);
            _cacheService.ClearTableOfContentsCache(projectId, branchName);
        }

        private ExtendedBranchDto CreateExtendedBranchDto(BranchDto branchDto)
        {
            var extendedBranchDto = new ExtendedBranchDto(branchDto);
            var mkDocsConfiguration = ReadMkDocsConfiguration(branchDto.MkDocsYaml);

            if (mkDocsConfiguration.ContainsKey("docs_dir"))
            {
                extendedBranchDto.DocsDir = mkDocsConfiguration["docs_dir"].ToString();
            }
            else
            {
                extendedBranchDto.DocsDir = "docs";
            }

            if (mkDocsConfiguration.ContainsKey("site_name"))
            {
                extendedBranchDto.SiteName = mkDocsConfiguration["site_name"].ToString();
            }

            return extendedBranchDto;
        }

        private Dictionary<object, object> ReadMkDocsConfiguration(string yamlFileContent)
        {
            using (var reader = new StringReader(yamlFileContent))
            {
                var deserializer = new Deserializer();

                var banchConfiguration = deserializer.Deserialize(reader) as Dictionary<object, object>;
                return banchConfiguration;
            }
        }
    }
}