using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class FilteredResutsDto<T>
    {
        public IEnumerable<T> Results { get; set; }

        public int AllResults { get; set; }
    }
}