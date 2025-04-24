using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger) : Controller
{
    private readonly IProjectService _projectService = projectService;
    private readonly ILogger<ProjectsController> _logger = logger;

    [Route("projects")]
 
    public async Task<IActionResult> Projects()
    {
        try
        {
            var projects = await _projectService.GetAllProjectsAsync(); 
            return View(projects);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading projects");
            return StatusCode(500, "Internal server error");
        }
    }


    //public async Task<IActionResult> Projects(string filter = "all")
    //{
    //    var model = await _projectService.GetAllProjectsAsync();
    //    model.ActiveFilter = filter;
    //    return View(model);
    //}

    //public async Task<IActionResult> Details(int id)
    //{
    //    var project = await _projectService.GetProjectByIdAsync(id);

    //    if (project == null)
    //        return NotFound();

    //    return View(project);
    //}

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        var model = new AddProjectViewModel
        {
            AvailableMembers = await _projectService.GetAvailableMembersAsync(),
            ProjectMembers = new List<MemberViewModel>(),
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableMembers = await _projectService.GetAvailableMembersAsync();
           
            if (model.SelectedMemberIds != null && model.SelectedMemberIds.Any())
            {
                model.ProjectMembers = await _projectService.GetMembersByIdsAsync(model.SelectedMemberIds);
            }
            return View(model);
        }

        await _projectService.CreateProjectAsync(model);
        return RedirectToAction("Projects");
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();
        
        project.AvailableMembers = await _projectService.GetAvailableMembersAsync();

        return View(project);
    }

    [HttpPost]
    public async Task<IActionResult> Update(EditProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableMembers = await _projectService.GetAvailableMembersAsync();
         
            if (model.SelectedMemberIds != null && model.SelectedMemberIds.Any())
            {
                model.ProjectMembers = await _projectService.GetMembersByIdsAsync(model.SelectedMemberIds);
            }
            return View(model);
        }

        await _projectService.UpdateProjectAsync(model);
        return RedirectToAction("Projects");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();
        return View(project);
    }
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _projectService.DeleteProjectAsync(id);
        return RedirectToAction("Projects");
    }
}

