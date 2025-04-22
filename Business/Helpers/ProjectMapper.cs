
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.Helpers;

public class ProjectMapper
{
    private readonly UserManager<UserEntity> _userManager;

    public ProjectMapper(UserManager<UserEntity> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ProjectViewModel> MapToViewModel(ProjectEntity project)
    {
        var members = new List<MemberViewModel>();

        if (project.projectMembers != null)
        {
            foreach (var pm in project.projectMembers)
            {
                var user = pm.User ?? await _userManager.FindByIdAsync(pm.UserId);
                if (user != null)
                {
                    members.Add(new MemberViewModel
                    {
                        Id = user.Id,
                        Name = user.UserName!,
                        Email = user.Email!,
                        Image = user.ProfileImage!

                    });
                }
            }
        }

        return new ProjectViewModel
        {
            ProjectName = project.ProjectName,
            Description = project.Description,
            ClientId = project.ClientName!,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            ProjectMembers = members,
            SelectedMemberIds = members.Select(m => m.Id).ToList()
        };
    }

    public async Task<List<ProjectViewModel>> MapToViewModels(List<ProjectEntity> projects)
    {
        var viewModels = new List<ProjectViewModel>();
        foreach (var project in projects)
        {
            viewModels.Add(await MapToViewModel(project));
        }
        return viewModels;
    }

}
