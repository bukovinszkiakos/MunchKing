namespace MunchKing.DTOs
{
    public class ProfileDto
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? MobileNumber { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }

        public ProfileDto(string email, string username, string fullName, string? profileImageUrl, string? mobileNumber, string? address, string? postalCode)
        {
            Email = email;
            Username = username;
            FullName = fullName;
            ProfileImageUrl = profileImageUrl;
            MobileNumber = mobileNumber;
            Address = address;
            PostalCode = postalCode;
        }
    }


}
