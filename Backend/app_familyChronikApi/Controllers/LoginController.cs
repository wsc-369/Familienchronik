using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using appAhnenforschungBackEnd.Comunication;
using Microsoft.AspNetCore.Authorization;
using ValueObject;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LoginController : ControllerBase
  {

    private readonly ILogger<LoginController> _logger;
    private readonly CReadWriteData _context;

    public LoginController(ILogger<LoginController> logger)
    {
      _logger = logger;
      _context = new CReadWriteData();
    }

    // GET: api/PersonFilter/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAuthenticationUser([FromRoute] string id)
    {

      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      CLAuthentication oAuthentication = new CLAuthentication();
      IList<CAuthenticationUser> resultAuthenticationUsers;

      resultAuthenticationUsers = oAuthentication.AuthenticationUserByEMail(id);

      if (resultAuthenticationUsers == null)
      {
        return NotFound();
      }

      resultAuthenticationUsers[0].Password = string.Empty;
      return Ok(resultAuthenticationUsers);
      
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
      if (request == null || string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
      {
        return BadRequest("Invalid client request");
      }

      CLAuthentication oAuthentication = new CLAuthentication();
      CAuthenticationUser resultAuthenticationUsers;

      resultAuthenticationUsers = oAuthentication.AuthenticationByLogin(request.UserName, request.Password);

      if (resultAuthenticationUsers != null && resultAuthenticationUsers.LoginName == request.UserName && resultAuthenticationUsers.Password == request.Password)
      {
        var secretKey = CreateSecretKey();
        var signinCredentials = CreateSigningCredentials(secretKey);
        var claims = CreateClaims(resultAuthenticationUsers);
        var tokenOptions = GetTokeOptions(claims, signinCredentials);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        var response = new LoginResponse
        {
          AccessToken = tokenString,
          ExpiresIn = 120 * 60,
          User = new AuthUser
          {
            Id = resultAuthenticationUsers.UserId,
            UserName = resultAuthenticationUsers.LoginName,
            Email = resultAuthenticationUsers.Email,
            Roles = GetRolesFromAuthUser(resultAuthenticationUsers)
          }
        };

        return Ok(response);
      }
      else
      {
        return Unauthorized();
      }
    }

    [HttpPost("LoginNameAndPassword")]
    public IActionResult LoginNameAndPassword([FromBody] CAuthenticationUser user)
    {
      if (user == null)
      {
        return BadRequest("Invalid client request");
      }

      CLAuthentication oAuthentication = new CLAuthentication();
      CAuthenticationUser resultAuthenticationUsers;

      resultAuthenticationUsers = oAuthentication.AuthenticationByLogin(user.LoginName, user.Password);

      if (resultAuthenticationUsers != null && resultAuthenticationUsers.LoginName == user.LoginName && resultAuthenticationUsers.Password == user.Password)
      {
        var secretKey = CreateSecretKey();
        var signinCredentials = CreateSigningCredentials(secretKey);
        var claims = CreateClaims(resultAuthenticationUsers);
        var tokeOptions = GetTokeOptions(claims, signinCredentials);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

        resultAuthenticationUsers.Token = tokenString;
        resultAuthenticationUsers.Password = string.Empty;

        return Ok(resultAuthenticationUsers);
      }
      else
      {
        return Unauthorized();
      }
    }

    [HttpPost("forgot-password")]
    public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
      if (request == null || string.IsNullOrEmpty(request.Email))
      {
        return BadRequest("Invalid request");
      }

      try
      {
        var oUser = _context.GetUserByEmail(request.Email);
        if (oUser != null)
        {
          CSendMailHelper oSendMail = new CSendMailHelper();
          oUser.Password = oSendMail.CreatePassword(8);
          _context.UpdateUserPassword(oUser);
          bool valid = oSendMail.SendMailByForgotPassword(oUser);

          if (valid)
          {
            return Ok();
          }
        }
        
        return Ok();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in ForgotPassword");
        return StatusCode(500, "An error occurred while processing your request");
      }
    }

    [HttpPost("change-password")]
    [Authorize]
    public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
    {
      if (request == null)
      {
        return BadRequest("Invalid request");
      }

      if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
      {
        return BadRequest("Current password and new password are required");
      }

      try
      {
        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        var userName = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userName))
        {
          return Unauthorized();
        }

        CLAuthentication oAuthentication = new CLAuthentication();
        CAuthenticationUser currentUser = oAuthentication.AuthenticationByLogin(userName, request.CurrentPassword);

        if (currentUser == null)
        {
          return Unauthorized("Current password is incorrect");
        }

        var oUser = _context.GetUserByEmail(currentUser.Email);
        if (oUser != null)
        {
          oUser.Password = request.NewPassword;
          _context.UpdateUserPassword(oUser);
          return Ok();
        }

        return NotFound("User not found");
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error in ChangePassword");
        return StatusCode(500, "An error occurred while processing your request");
      }
    }

    private SymmetricSecurityKey CreateSecretKey()
    {
      return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345_1111111111111"));
    }
    
    private SigningCredentials CreateSigningCredentials(SymmetricSecurityKey secretKey)
    {
      return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    }

    private List<string> GetRolesFromAuthUser(CAuthenticationUser user)
    {
      var roles = new List<string>();
      
      switch (user.Role)
      {
        case 1:
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied);
          break;
        case 2:
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied);
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress);
          break;
        case 3:
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied);
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress);
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditMainPage);
          break;
        case 4:
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied);
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress);
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditMainPage);
          roles.Add(appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleAdmin);
          break;
      }
      
      return roles;
    }

    /// <summary>
    /// Benutzerrollen
    /// role1 = 'Mitglied',
    ///  role2 = 'Adressen bearbeiten',
    ///  role3 = 'Startseite bearbeiten',
    ///  role4 = 'Administrator',
    /// </summary>
    /// <param name="resultAuthenticationUsers"></param>
    /// <returns></returns>
    private List<Claim> CreateClaims(CAuthenticationUser resultAuthenticationUsers)
    {
      var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, resultAuthenticationUsers.LoginName),
            new Claim(ClaimTypes.Email, resultAuthenticationUsers.FirstName)
        };

      switch (resultAuthenticationUsers.Role)
      {
        case 1:
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          break;
        case 2:
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress));
          break;
        case 3:
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditMainPage));
          break;
        case 4:
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditMainPage));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleAdmin));
          break;
      }
      return claims;
    }

    private JwtSecurityToken GetTokeOptions(List<Claim> claims, SigningCredentials signinCredentials)
    {
      const int DURATION = 120;
      return new JwtSecurityToken(
        issuer: ReadSettings.UrlTickenValidation(),
        audience: ReadSettings.UrlTickenValidation(),
        claims: claims,
        expires: DateTime.Now.AddMinutes(DURATION),
        signingCredentials: signinCredentials);
    }
  }
}
