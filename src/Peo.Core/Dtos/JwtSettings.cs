namespace Peo.Core.Dtos
{
    public class JwtSettings
    {
        public const string Position = "Jwt";

        public string Key { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}