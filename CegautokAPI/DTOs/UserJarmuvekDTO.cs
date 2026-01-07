namespace CegautokAPI.DTOs
{
    public class UserJarmuvekDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Kezdes { get; set; }
        public string Rendszam { get; set; } = null!;
    }
}
