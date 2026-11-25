namespace ServiceDesk.Api.DTOs
{
    public class UpdateSlaRuleDto
    {
        public int ResponseTimeMin { get; set; }
        public int ResolutionTimeMin { get; set; }
        public bool IsActive { get; set; }
    }
}
