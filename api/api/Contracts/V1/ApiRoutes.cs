namespace api.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = Base + "/posts";
            public const string Get = Base + "/posts/{postId}";
            public const string Create = Base + "/posts";
            public const string Update = Base + "/posts/{postId}";
            public const string Delete = Base + "/posts/{postId}";
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
        }
    }
}