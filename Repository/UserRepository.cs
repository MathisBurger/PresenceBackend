using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PresenceBackend.Models.Database;
using PresenceBackend.Models.Request;
using PresenceBackend.Shared;

namespace PresenceBackend.Repository;

public class UserRepository : IRepository<User>
{
    private readonly IContext ctx;
    private readonly IPasswordHasher<User> hasher;

    public UserRepository(IContext ctx, IPasswordHasher<User> hasher)
    {
        this.ctx = ctx;
        this.hasher = hasher;
    }
    
    public async Task<User> RegisterUser(RegisterRequest request)
    {
        var user = new User();
        user.Username = request.Username;
        user.Password = hasher.HashPassword(user, request.Password);
        ctx.Add(user);
        await ctx.SaveChangesAsync();
        return user;
    }

    public async Task<bool> LoginUser(LoginRequest request)
    {
        var user = await FindUserByUsername(request.Username);
        if (user == null)
        {
            return false;
        }

        return hasher.VerifyHashedPassword(user, user.Password, request.Password) == PasswordVerificationResult.Success;
    }

    public async Task<User?> FindUserByUsername(string username)
    {
        return await ctx.Users
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> FindOneById(Guid id)
    {
        return await ctx.Users.FindAsync(id);
    }
}