
using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services;

public interface IMemberService
{

}

public class MemberService(AppDbContext context) : IMemberService
{
    private readonly AppDbContext _context = context;

    
}
