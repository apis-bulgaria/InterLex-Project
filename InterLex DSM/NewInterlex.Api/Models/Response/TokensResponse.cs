namespace NewInterlex.Api.Models.Response
{
    using Core.Dto;

    public class TokensResponse
    {
        public AccessToken AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string UserName { get; set; }

        public TokensResponse(AccessToken accessToken, string refreshToken, string userName)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
            this.UserName = userName;
        }
    }
}