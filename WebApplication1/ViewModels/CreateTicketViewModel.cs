using System.ComponentModel.DataAnnotations;
using ServiceDesk.Web.Models.Api;

namespace ServiceDesk.Web.ViewModels;

public class CreateTicketViewModel
{
    [Required(ErrorMessage = "Укажите тему заявки")]
    [StringLength(200, ErrorMessage = "Тема не должна превышать 200 символов")]
    [Display(Name = "Тема")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите описание проблемы")]
    [StringLength(4000, ErrorMessage = "Описание не должно превышать 4000 символов")]
    [Display(Name = "Описание")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Выберите категорию")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите категорию")]
    [Display(Name = "Категория")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Выберите приоритет")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите приоритет")]
    [Display(Name = "Приоритет")]
    public int PriorityId { get; set; }

    public IReadOnlyList<CategoryDto> Categories { get; set; } = Array.Empty<CategoryDto>();
    public IReadOnlyList<PriorityDto> Priorities { get; set; } = Array.Empty<PriorityDto>();
}