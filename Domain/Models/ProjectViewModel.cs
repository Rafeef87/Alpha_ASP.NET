using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Domain.Models;

public class ProjectViewModel
{
    public int Id { get; set; }

    [Display(Name = "Project Image", Prompt = "Select an image")]
    [DataType(DataType.Upload)]
    public IFormFile Image { get; set; } = null!;

    [Display(Name = "Project Name", Prompt = "Project Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Required")]
    public string ProjectName { get; set; } = null!;

    [Display(Name = "Client Name", Prompt = "Client Name")]
    [DataType(DataType.Text)]
    [Required(ErrorMessage = "Required")]
    public string ClientId { get; set; } = null!;

    [Display(Name = "Status")]
    public string Status { get; set; } = null!;

    [Display(Name = "Description", Prompt = "Type something")]
    [DataType(DataType.Text)]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Required")]
    [Display(Name = "Start Date", Prompt = "Start Date")]
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "Required")]
    [Display(Name = "End Date", Prompt = "End Date")]
    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Display(Name = "Budget", Prompt = "0")]
    [DataType(DataType.Currency)]
    public decimal? Budget { get; set; }

    [Display(Name = "Team Members")]
    public List<string> SelectedMemberIds { get; set; } = new List<string>();

    public List<MemberViewModel> AvailableMembers { get; set; } = new List<MemberViewModel>();

    public List<MemberViewModel> ProjectMembers { get; set; } = new List<MemberViewModel>();

    public int DaysLeft => (EndDate - DateTime.Now).Days;

    public bool IsOverdue => DaysLeft < 0;
}