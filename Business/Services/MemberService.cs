
using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IMemberService
{
    Task<List<MemberViewModel>> GetAvailableMembersAsync(ProjectEntity project);
}

public class MemberService(AppDbContext context) : IMemberService
{
    private readonly AppDbContext _context = context;

    public async Task<List<MemberViewModel>> GetAvailableMembersAsync(ProjectEntity project)
    {
        var allUsers = await _context.Users.ToListAsync();
        var projectMembers = project.projectMembers.Select(pm => pm.UserId).ToList();
        var availableMembers = allUsers
            .Where(u => !projectMembers.Contains(u.Id))
            .Select(u => new MemberViewModel
            {
                Id = u.Id,
                Name = u.FirstName + u.LastName,
                Email = u.Email!,
                Image = u.ProfileImage!
            })
            .ToList();
        return availableMembers;
    }
}
