
using System.Linq;
using Business.Helpers;
using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IProjectService
{
    Task CreateProjectAsync(AddProjectViewModel model);
    Task DeleteProjectAsync(int id);
    Task<ProjectListViewModel> GetAllProjectsAsync();
    Task<List<MemberViewModel>> GetAvailableMembersAsync();
    Task<List<MemberViewModel>> GetMembersByIdsAsync(List<string> memberIds);
    Task<ProjectViewModel> GetProjectByIdAsync(int id);
    Task UpdateProjectAsync(EditProjectViewModel model);
}

public class ProjectService(AppDbContext context, UserManager<UserEntity> userManager, ProjectMapper projectMapper) : IProjectService
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly ProjectMapper _projectMapper = projectMapper;


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
        projectViewModel.AvailableMembers = await GetAvailableMembersAsync();
        return projectViewModel;

    }

    public async Task CreateProjectAsync(AddProjectViewModel model)
    {
        // Save the uploaded image to a file path or convert it to a string representation
        string? projectImagePath = null;
        if (model.Image != null)
        {
            var uploadsFolder = Path.Combine("wwwroot", "images", "projects");
            Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }

            projectImagePath = "/images/projects/" + uniqueFileName; // Store relative path
        }

        // Create project
        var project = new ProjectEntity
        {
            ProjectName = model.ProjectName,
            ProjectImage = projectImagePath, // Assign the saved image path
            Description = model.Description,
            ClientName = model.ClientName,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Budget = model.Budget,
            Status = "started",
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Add project members
        if (model.SelectedMemberIds != null && model.SelectedMemberIds.Any())
        {
            foreach (var memberId in model.SelectedMemberIds)
            {
                _context.Members.Add(new MemberEntity
                {
                    ProjectId = project.Id,
                    UserId = memberId
                });
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateProjectAsync(EditProjectViewModel model)
    {
        // Save the uploaded image to a file path or convert it to a string representation
        string? projectImagePath = null;
        if (model.Image != null)
        {
            var uploadsFolder = Path.Combine("wwwroot", "images", "projects");
            Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Image.CopyToAsync(fileStream);
            }
            projectImagePath = "/images/projects/" + uniqueFileName; // Store relative path
        }
        var project = await _context.Projects
            .FindAsync(model.Id);
        if (project == null)
        {
            return;
        }
        // Update project entity
        project.ProjectName = model.ProjectName;
        project.ClientName = model.ClientName;
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


    public async Task<List<MemberViewModel>> GetAvailableMembersAsync()
    {

        var users = await _context.Users
             .Select(u => new MemberViewModel
             {
                 Id = u.Id,
                 Name = u.FirstName + u.LastName,
                 Email = u.Email!,
                 Image = u.ProfileImage!
             })
       .ToListAsync();

        return users;
    }

    public async Task<List<MemberViewModel>> GetMembersByIdsAsync(List<string> memberIds)
    {
        var members = await _context.Users
            .Where(u => memberIds.Contains(u.Id))
            .Select(u => new MemberViewModel
            {
                Id = u.Id,
                Name = u.FirstName + u.LastName,
                Email = u.Email!,
                Image = u.ProfileImage!
            })
            .ToListAsync();
        return members;
    }
}
