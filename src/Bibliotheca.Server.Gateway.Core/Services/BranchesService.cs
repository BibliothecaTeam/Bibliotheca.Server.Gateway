using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using YamlDotNet.Serialization;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class BranchesService : IBranchesService
    {
        private readonly IBranchesClient _branchesClient;

        public BranchesService(IBranchesClient branchesClient)
        {
            _branchesClient = branchesClient;
        }

        public async Task<IList<ExtendedBranchDto>> GetBranchesAsync(string projectId)
        {
            var branches = await _branchesClient.Get(projectId);
            return branches.Select(x => CreateExtendedBranchDto(x)).ToList();
        }

        public async Task<ExtendedBranchDto> GetBranchAsync(string projectId, string branchName)
        {
            var branch = await _branchesClient.Get(projectId, branchName);
            return CreateExtendedBranchDto(branch);
        }

        public async Task CreateBranchAsync(string projectId, BranchDto branch)
        {
            await _branchesClient.Post(projectId, branch);
        }

        public async Task UpdateBranchAsync(string projectId, string branchName, BranchDto branch)
        {
            await _branchesClient.Put(projectId, branchName, branch);
        }
        
        public async Task DeleteBranchAsync(string projectId, string branchName)
        {
            await _branchesClient.Delete(projectId, branchName);
        }

        private ExtendedBranchDto CreateExtendedBranchDto(BranchDto branchDto)
        {
            var extendedBranchDto = new ExtendedBranchDto(branchDto);
            var mkDocsConfiguration = ReadMkDocsConfiguration(branchDto.MkDocsYaml);

            if (mkDocsConfiguration.ContainsKey("docs_dir"))
            {
                extendedBranchDto.DocsDir = mkDocsConfiguration["docs_dir"].ToString();
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