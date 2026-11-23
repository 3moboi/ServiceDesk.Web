namespace ServiceDesk.Web.Models.Api;

public class ReportDto
{
    public List<CategoryReportItem> ByCategory { get; set; } = new();
    public List<AssigneeReportItem> ByAssignee { get; set; } = new();
    public double OnTimePercentage { get; set; }
    public int TotalClosed { get; set; }
}

public class CategoryReportItem
{
    public string CategoryName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AssigneeReportItem
{
    public string AssigneeName { get; set; } = string.Empty;
    public int Count { get; set; }
}