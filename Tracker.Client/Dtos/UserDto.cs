using System.Globalization;

namespace Tracker.Client.Dtos
{
    public class UserDto
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Avatar_url { get; set; }
        public string Bio { get; set; }
        public int PublicRepos { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
    }

}
