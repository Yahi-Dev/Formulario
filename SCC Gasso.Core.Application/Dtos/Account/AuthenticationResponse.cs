namespace SCC_Gasso.Core.Application.Dtos.Account
{
    public class AuthenticationResponse
    {
        //
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public bool? IsVerified { get; set; }
        public bool? UserStatus { get; set; }
        public string? Role { get; set; }



        public List<string>? Roles { get; set; }
        public List<int>? Permissions { get; set; }


        public string? IdCard { get; set; }

        public bool HasError { get; set; }
        public string? Error { get; set; }

        public string? LastLogin { get; set; }
    }
}
