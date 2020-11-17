using System.Collections.Generic;

namespace api.Contracts.V1.Responses
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}