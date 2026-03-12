using System;
using System.Collections.Generic;

namespace ValueObject
{
  public class LoginRequest
  {
    public string UserName { get; set; }
    public string Password { get; set; }
  }

  public class LoginResponse
  {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public int? ExpiresIn { get; set; }
    public AuthUser User { get; set; }
  }

  public class AuthUser
  {
    public int? Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
  }

  public class ForgotPasswordRequest
  {
    public string Email { get; set; }
  }

  public class ChangePasswordRequest
  {
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
  }
}
