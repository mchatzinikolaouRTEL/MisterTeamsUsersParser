using MisterControlHubApiDto.HybridDtos;

namespace MisterTeamsUsersParserParser.Models.Dtos
{
    public record AuthTokenResponseDTO
    {
        public string Token { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }
    }
}
