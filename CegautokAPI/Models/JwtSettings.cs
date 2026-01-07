namespace CegautokAPI.Models
{
    public class Jwtsettings
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double ExpirityMinutes { get; set; }

    }
}
