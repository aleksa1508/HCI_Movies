namespace Filmovi.Models
{
    [Serializable]
    public class Users
    {
        public UserRole Role { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;

        public Users(UserRole role, string username, string password)
        {
            Role = role;
            this.username = username;
            this.password = password;
        }

        public Users()
        {
        }
    }
}
