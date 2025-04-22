using Business.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

public class ProjectsController(IProjectService projectService) : Controller
{
    private readonly IProjectService _projectService = projectService;
   
    [Route("projects")]
    public async Task<IActionResult> Projects(string filter = "all")
    {
        var model = await _projectService.GetAllProjectsAsync();
        model.ActiveFilter = filter;
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);

        if (project == null)
            return NotFound();

        return View(project);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new ProjectViewModel
        {
            AvailableMembers = await _projectService.GetAvailableMembersAsync(),
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(30)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableMembers = await _projectService.GetAvailableMembersAsync();
            return View(model);
        }
        await _projectService.CreateProjectAsync(model);
        return RedirectToAction("Projects");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
            return NotFound();
        return View(project);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(ProjectViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableMembers = await _projectService.GetAvailableMembersAsync();
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

