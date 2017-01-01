using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class FilteredResuts<T>
    {
        public IEnumerable<T> Results { get; set; }
        
        public int AllResults { get; set; }
    }
}