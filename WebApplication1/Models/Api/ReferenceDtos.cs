namespace ServiceDesk.Web.Models.Api;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PriorityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class SlaRuleDto
{
    public int Id { get; set; }
    public string PriorityName { get; set; } = string.Empty;
    public int ResponseTimeMin { get; set; }
    public int ResolutionTimeMin { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateSlaRuleDto
{
    public int ResponseTimeMin { get; set; }
    public int ResolutionTimeMin { get; set; }
    public bool IsActive { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}