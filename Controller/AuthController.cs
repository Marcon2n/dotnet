using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("/")]
public class AuthController : ControllerBase
{
    private readonly UserRepository _userRepository;

    public AuthController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        var users = _userRepository.GetUsers();

        foreach (var user in users)
        {
            Console.WriteLine($"Id: {user.Id}, Username: {user.Username}, PasswordHash: {user.PasswordHash}");
        }

        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Chưa có thông tin tài khoản hoặc mật khẩu" });
        }

        if (!_userRepository.IsUsernameAvailable(request.Username))
        {
            return BadRequest(new { message = "Tài khoản đã tồn tại" });
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var newUser = new User
        {
            Id = Guid.NewGuid().ToString(),
            Username = request.Username,
            PasswordHash = passwordHash
        };
        _userRepository.AddUser(newUser);

        return Ok(new { message = "Đăng ký tài khoản thành công" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Chưa có thông tin tài khoản hoặc mật khẩu" });
        }

        var user = _userRepository.GetUserByUsername(request.Username);

        if (user == null)
        {
            return Unauthorized(new { message = "Tài khoản không tồn tại" });
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Mật khẩu sai" });
        }

        return Ok(new { message = "Đăng nhập thành công", userId = user.Id, username = user.Username });
    }
}
