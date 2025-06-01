using GarageMasterBE.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

namespace GarageMasterBE.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<MongoDBSettings> mongoSettings, IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        // Tạo hash mật khẩu (dùng SHA256 làm ví dụ đơn giản)
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hashBytes);
        }

        // Kiểm tra mật khẩu so với hash lưu trong DB
        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == passwordHash;
        }

        // Tìm user theo email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Tạo user mới (chưa xác thực email)
        public async Task<User> CreateUserAsync(string email, string password)
        {
            var user = new User
            {
                Email = email,
                PasswordHash = HashPassword(password),
                EmailConfirmed = false,
                EmailConfirmationCode = GenerateConfirmationCode(),
                EmailConfirmationCodeExpiry = DateTime.UtcNow.AddMinutes(10) // Mã hiệu lực 10 phút
            };

            await _users.InsertOneAsync(user);
            return user;
        }

        // Sinh mã xác nhận 6 chữ số
        private string GenerateConfirmationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        // Xác thực email
        public async Task<bool> ConfirmEmailAsync(string email, string code)
        {
            var user = await GetByEmailAsync(email);
            if (user == null)
                return false;

            if (user.EmailConfirmed)
                return true; // Đã xác thực rồi

            if (user.EmailConfirmationCode == code && user.EmailConfirmationCodeExpiry > DateTime.UtcNow)
            {
                var update = Builders<User>.Update
                    .Set(u => u.EmailConfirmed, true)
                    .Unset(u => u.EmailConfirmationCode)
                    .Unset(u => u.EmailConfirmationCodeExpiry);

                await _users.UpdateOneAsync(u => u.Id == user.Id, update);
                return true;
            }

            return false;
        }

        // Kiểm tra đăng nhập, trả về user nếu đúng
        public async Task<User?> ValidateUserAsync(string email, string password)
        {
            var user = await GetByEmailAsync(email);
            if (user == null) return null;

            if (!user.EmailConfirmed) return null;

            if (VerifyPassword(password, user.PasswordHash))
                return user;

            return null;
        }
    }
}
