using System;
using System.Linq;
using USPGH_planning_lourd.classes;
using Microsoft.EntityFrameworkCore;

namespace USPGH_planning_lourd.services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService()
        {
            _context = new AppDbContext();
        }

        public User CreateUser(string firstName, string lastName, string email, string password, string roleName)
        {
            // Check if user already exists
            if (_context.Users.Any(u => u.email == email))
            {
                throw new InvalidOperationException("User with this email already exists.");
            }

            // Create user
            var user = new User
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                // Use the BCrypt.Net package directly
                password = BCrypt.Net.BCrypt.HashPassword(password),
                email_verified_at = DateTime.Now,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Assign role
            AssignRoleToUser(user.Id, roleName);

            return user;
        }

        public void AssignRoleToUser(int userId, string roleName)
        {
            var role = _context.Roles
                .FirstOrDefault(r => r.Name == roleName);

            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleName}' not found.");
            }

            // Check if the role is already assigned
            var existingAssignment = _context.UserRoles
                .FirstOrDefault(ar => ar.EntityId == userId && ar.RoleId == role.Id && ar.EntityType == "App\\Models\\User");

            if (existingAssignment != null)
            {
                throw new InvalidOperationException($"User already has the role '{roleName}'.");
            }

            // Create the new role assignment
            var userRole = new UserRole
            {
                RoleId = role.Id,
                EntityId = userId,
                EntityType = "App\\Models\\User" // This must match Laravel's namespace
            };

            _context.UserRoles.Add(userRole);
            _context.SaveChanges();
        }

        public void RemoveRoleFromUser(int userId, string roleName)
        {
            var role = _context.Roles
                .FirstOrDefault(r => r.Name == roleName);

            if (role == null)
            {
                throw new InvalidOperationException($"Role '{roleName}' not found.");
            }

            var userRole = _context.UserRoles
                .FirstOrDefault(ar => ar.EntityId == userId && ar.RoleId == role.Id && ar.EntityType == "App\\Models\\User");

            if (userRole == null)
            {
                throw new InvalidOperationException($"User does not have the role '{roleName}'.");
            }

            _context.UserRoles.Remove(userRole);
            _context.SaveChanges();
        }
    }
}