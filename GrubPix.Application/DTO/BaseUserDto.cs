namespace GrubPix.Application.DTO
{
    // DTO for creating a user
    public class BaseUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
