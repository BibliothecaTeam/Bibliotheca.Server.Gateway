using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bibliotheca.Server.Gateway.Core.Policies;
using System;
using Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers;

namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class GraphQLQuery : ObjectGraphType
    {
        public GraphQLQuery(
            IProjectsResolver projectsResolver, 
            IBranchesResolver branchesResolver,
            IDocumentsResolver documentsResolver,
            ITagsResolver tagsResolver,
            IGroupsResolver groupsResolver,
            IChaptersResolver chaptersResolver)
        {
            projectsResolver.Resolve(this);
            branchesResolver.Resolve(this);
            documentsResolver.Resolve(this);
            tagsResolver.Resolve(this);
            groupsResolver.Resolve(this);
            chaptersResolver.Resolve(this);
        }
    }
}