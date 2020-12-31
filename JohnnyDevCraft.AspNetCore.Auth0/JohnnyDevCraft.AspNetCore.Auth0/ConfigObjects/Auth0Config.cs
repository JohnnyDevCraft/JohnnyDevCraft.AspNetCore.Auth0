namespace JohnnyDevCraft.AspNetCore.Auth0.ConfigObjects
{
    public class Auth0Config
    {
        /// <summary>
        /// The authority URL taken from the setup for your application
        /// in Auth0.
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// The Audience specified in your Auth0 Configuration.
        /// </summary>
        public string Audience { get; set; }
    }
}