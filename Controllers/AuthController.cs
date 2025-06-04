using GarageMasterBE.Models;
using GarageMasterBE.Services;
using Microsoft.AspNetCore.Mvc;

namespace GarageMasterBE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly EmailService _emailService;
        private readonly JwtService _jwtService;

        public AuthController(UserService userService, EmailService emailService, JwtService jwtService)
        {
            _userService = userService;
            _emailService = emailService;
            _jwtService = jwtService;
        }

      [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterRequest request)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var existingUser = await _userService.GetByEmailAsync(request.Email);
    if (existingUser != null)
        return BadRequest("Email đã được sử dụng.");

    var user = await _userService.CreateUserAsync(request.Email, request.Password);
    if (user == null)
        return StatusCode(500, "Đăng ký không thành công.");

    // Kiểm tra email và mã xác nhận trước khi gửi mail
    if (string.IsNullOrWhiteSpace(user.Email))
        return StatusCode(500, "Email của người dùng không hợp lệ.");

    if (string.IsNullOrWhiteSpace(user.EmailConfirmationCode))
        return StatusCode(500, "Mã xác nhận email không hợp lệ.");

    try
    {
        await _emailService.SendConfirmationEmailAsync(user.Email, user.EmailConfirmationCode);
    }
    catch (Exception ex)
    {
        // Log lỗi (nên có logger ở đây)
        return StatusCode(500, $"Gửi email xác nhận thất bại: {ex.Message}");
    }

    return Ok(new { message = "Đăng ký thành công. Vui lòng kiểm tra email để xác nhận tài khoản." });
}


        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ConfirmEmailAsync(request.Email, request.Code);
            if (!result)
                return BadRequest("Mã xác thực không đúng hoặc đã hết hạn.");

            return Ok(new { message = "Xác thực email thành công. Bạn có thể đăng nhập." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.ValidateUserAsync(request.Email, request.Password);
            if (user == null)
                return Unauthorized("Email hoặc mật khẩu không đúng, hoặc chưa xác thực email.");

            var token = _jwtService.GenerateToken(user.Id!, user.Email);

            return Ok(new
            {
                message = "Đăng nhập thành công",
                user = new
                {
                    user.Id,
                    user.Email
                }
            });
        }
    }
}
