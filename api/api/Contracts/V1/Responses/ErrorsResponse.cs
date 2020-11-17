using System.Collections.Generic;

namespace api.Contracts.V1.Responses
{
    public class ErrorsResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}