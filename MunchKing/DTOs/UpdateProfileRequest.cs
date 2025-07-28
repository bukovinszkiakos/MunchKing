namespace MunchKing.DTOs
{
    public class UpdateProfileRequest
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }

}
