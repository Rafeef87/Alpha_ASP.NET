
using Business.Helpers;
using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IProjectService
{
    Task<ProjectViewModel> CreateProjectAsync(ProjectViewModel model);
    Task DeleteProjectAsync(int id);
    Task<ProjectListViewModel> GetAllProjectsAsync();
    Task<ProjectViewModel> GetProjectByIdAsync(int id);
    Task UpdateProjectAsync(ProjectViewModel model);
}

public class ProjectService(AppDbContext context, UserManager<UserEntity> userManager, ProjectMapper projectMapper, MemberService memberService) : IProjectService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly ProjectMapper _projectMapper = projectMapper;
    private readonly MemberService _memberService = memberService;

    public async Task<ProjectListViewModel> GetAllProjectsAsync()
    {
        var projects = await _context.Projects.Include(p => p.projectMembers)
            .ThenInclude(pm => pm.User)
            .ToListAsync();

        return new ProjectListViewModel
        {
            AllProjects = await _projectMapper.MapToViewModels(projects),
            StartedProjects = await _projectMapper.MapToViewModels(
               projects.Where(p => p.Status == "started").ToList()
           ),
            CompletedProjects = await _projectMapper.MapToViewModels(
               projects.Where(p => p.Status == "completed").ToList()
           )
        };

    }

    public async Task<ProjectViewModel> GetProjectByIdAsync(int id)
    {
        var project = await _context.Projects
            .Include(p => p.projectMembers)
            .ThenInclude(pm => pm.User)
            .FirstOrDefaultAsync(p => p.Id == id.ToString());
        if (project == null)
        {
            return null!;
        }
        var projectViewModel = await _projectMapper.MapToViewModel(project);
        projectViewModel.AvailableMembers = await _memberService.GetAvailableMembersAsync(project);
        return projectViewModel;

    }

    public async Task<ProjectViewModel> CreateProjectAsync(ProjectViewModel model)
    {
        // Create project entity
        var project = new ProjectEntity
        {
            ProjectName = model.ProjectName,
            ClientName = model.ClientId,
            Status = "started", // Default status
            Description = model.Description,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        await _context.Projects.AddAsync(project);
        await _context.SaveChangesAsync();


        // Add project members
        if (model.SelectedMemberIds != null && model.SelectedMemberIds.Count > 0)
        {
            foreach (var memberId in model.SelectedMemberIds)
            {
                var projectMember = new MemberEntity
                {
                    ProjectId = project.Id,
                    UserId = memberId
                };
                await _context.Members.AddAsync(projectMember);
            }
            await _context.SaveChangesAsync();
        }
        return model;


    }

    public async Task UpdateProjectAsync(ProjectViewModel model)
    {
        var project = await _context.Projects
            .FindAsync(model.Id);
        if (project == null)
        {
            return;
        }
        // Update project entity
        project.ProjectName = model.ProjectName;
        project.ClientName = model.ClientId;
        project.Status = model.Status;
        project.Description = model.Description;
        project.StartDate = model.StartDate;
        project.EndDate = model.EndDate;
        project.Budget = model.Budget;
        project.Updated = DateTime.Now;
        _context.Projects.Update(project);
        await _context.SaveChangesAsync();
        // Update project members
        var existingMemberIds = project.projectMembers.Select(pm => pm.UserId).ToList();
        var newMemberIds = model.SelectedMemberIds;
        // Remove members that are no longer selected
        foreach (var memberId in existingMemberIds.Except(newMemberIds))
        {
            var memberToRemove = await _context.Members
                .FirstOrDefaultAsync(pm => pm.ProjectId == project.Id && pm.UserId == memberId);
            if (memberToRemove != null)
            {
                _context.Members.Remove(memberToRemove);
            }
        }
        // Add new members
        foreach (var memberId in newMemberIds.Except(existingMemberIds))
        {
            var newProjectMember = new MemberEntity
            {
                ProjectId = project.Id,
                UserId = memberId
            };
            await _context.Members.AddAsync(newProjectMember);
        }
        await _context.SaveChangesAsync();

    }
    public async Task DeleteProjectAsync(int id)
    {
        var project = await _context.Projects
            .FindAsync(id);
        if (project == null)
            return;
        // Remove project members
        var projectMembers = await _context.Members
            .Where(pm => pm.ProjectId == project.Id)
            .ToListAsync();
        _context.Members.RemoveRange(projectMembers);

        // Remove project
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

    }



}
