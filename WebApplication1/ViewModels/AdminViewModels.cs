using ServiceDesk.Web.Models.Api;

namespace ServiceDesk.Web.ViewModels;

public class SlaRulesViewModel
{
    public IReadOnlyList<SlaRuleDto> Rules { get; set; } = Array.Empty<SlaRuleDto>();
}

public class UsersViewModel
{
    public IReadOnlyList<UserDto> Users { get; set; } = Array.Empty<UserDto>();
}