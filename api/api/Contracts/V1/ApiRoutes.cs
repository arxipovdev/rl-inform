namespace api.Contracts.V1
{
    public static class ApiRoutes
    {
        private const string Root = "api";
        private const string Version = "v1";
        private const string Base = Root + "/" + Version;

        public static class Roles
        {
            public const string GetAll = Base + "/roles";
            public const string Get = Base + "/roles/{roleId}";
            public const string Create = Base + "/roles";
            public const string Update = Base + "/roles/{roleId}";
            public const string Delete = Base + "/roles/{roleId}";
        }
        
        public static class Users
        {
            public const string GetAll = Base + "/users";
            public const string Get = Base + "/users/{userId}";
            public const string Create = Base + "/users";
            public const string Update = Base + "/users/{userId}";
            public const string Delete = Base + "/users/{userId}";
        }

        public static class Account
        {
            public const string Login = Base + "/account/login";
            public const string Register = Base + "/account/register";
            public const string ChangePassword = Base + "/account/changepassword";
            public const string Refresh = Base + "/account/refresh";
            public const string User = Base + "/account/user";
        }
    }
}