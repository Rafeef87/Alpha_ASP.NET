namespace Domain.Models;

public class ProjectListViewModel
{
    public List<ProjectViewModel> AllProjects { get; set; } = new List<ProjectViewModel>();
    public List<ProjectViewModel> StartedProjects { get; set; } = new List<ProjectViewModel>();
    public List<ProjectViewModel> CompletedProjects { get; set; } = new List<ProjectViewModel>();
    public string ActiveFilter { get; set; } = "all";

}