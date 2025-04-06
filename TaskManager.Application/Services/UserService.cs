

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Application.DTOs;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public UserService(IUnitOfWork unitOfWork,IConfiguration configuration )
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = (await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email)).FirstOrDefault();

            if (user == null || !VerifyPasswordHash(dto.Password, user.PasswordHash, user.PasswordSalt))
                throw new Exception("Invalid email or password.");

            // Генерирай JWT токен
            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        public async Task<UserResponseDto> RegisterAsync(RegisterUserDto dto)
        {
            // Check if the email is already in use
            var existing = await _unitOfWork.Users.FindAsync(u => u.Email == dto.Email);
            if (existing.Any())
                throw new Exception("Email already in use.");

            // Hash the password
            CreatePasswordHash(dto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = UserRole.User
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:ExpiresMinutes"]!)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        // Method to hash the password
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using var hmac = new HMACSHA512(salt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(hash);
        }

    }
}
