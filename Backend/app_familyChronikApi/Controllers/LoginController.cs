using appAhnenforschungBackEnd.Configutation;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static appAhnenforschungBackEnd.DataManager.CGlobal;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class LoginController : ControllerBase
  {

    private readonly ILogger<LoginController> _logger;

    public LoginController(ILogger<LoginController> logger)
    {
      _logger = logger;
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

    // GET api/values
    [HttpPost]
    public IActionResult Login([FromBody]CAuthenticationUser user)
    {
      if (user == null)
      {
        return BadRequest("Invalid client request");
      }

      CLAuthentication oAuthentication = new CLAuthentication();
      CAuthenticationUser resultAuthenticationUsers;

      resultAuthenticationUsers = oAuthentication.AuthenticationUserByEMail(user.Email,user.Password);

      if (resultAuthenticationUsers != null && resultAuthenticationUsers.Email == user.Email && resultAuthenticationUsers.Password == user.Password)
      {
        var secretKey = CreateSecretKey();
        var signinCredentials = CreateSigningCredentials(secretKey);
        var claims = CreateClaims(resultAuthenticationUsers);
        var tokeOptions = GetTokeOptions(claims, signinCredentials);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

        resultAuthenticationUsers.Token = tokenString;
        resultAuthenticationUsers.Password = string.Empty;
        
        return Ok(resultAuthenticationUsers);
        
        //return Ok({
        //  id: resultAuthenticationUsers.PersonId, username: resultAuthenticationUsers.LoginName,
        //  role: resultAuthenticationUsers.Role.ToString(),
        //  token: tokenString
        //        });
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

      //if (user.Email == "walter.schaedler@wsc.li" && user.Password == "ck595-4")
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



    private SymmetricSecurityKey CreateSecretKey()
    {
      return new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345_1111111111111"));
    }
    private SigningCredentials CreateSigningCredentials(SymmetricSecurityKey secretKey)
    {
      return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
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

      //if (((AppUserRoles)resultAuthenticationUsers.Role).HasFlag(AppUserRoles.Administrator))
      //{
      //  claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
      //  claims.Add(new Claim(ClaimTypes.Role, "EditAdress"));
      //  claims.Add(new Claim(ClaimTypes.Role, "EditMainPage"));
      //  claims.Add(new Claim(ClaimTypes.Role, "Admin"));

      //  claims.Add(new Claim(ClaimTypes.Role, AppUserRoles.EditAdress.ToString()));
      //  claims.Add(new Claim(ClaimTypes.Role, AppUserRoles.EditDialect.ToString()));
      //  claims.Add(new Claim(ClaimTypes.Role, AppUserRoles.EditPerson.ToString()));
      //  claims.Add(new Claim(ClaimTypes.Role, AppUserRoles.EditPortrait.ToString()));
      //  claims.Add(new Claim(ClaimTypes.Role, AppUserRoles.EditAppUser.ToString()));
        


      //}
      //else if (((AppUserRoles)resultAuthenticationUsers.Role).HasFlag(AppUserRoles.EditMainPage))
      //{
      //  claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
      //  claims.Add(new Claim(ClaimTypes.Role, "EditAdress"));
      //  claims.Add(new Claim(ClaimTypes.Role, "EditMainPage"));
      //}
      //else if (((AppUserRoles)resultAuthenticationUsers.Role).HasFlag(AppUserRoles.EditAdress))
      //{
      //  claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
      //  claims.Add(new Claim(ClaimTypes.Role, "EditAdress"));
      //}
      //else
      //{
      //  claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
      //}

      switch (resultAuthenticationUsers.Role)
      {
        case 1: // Mitglied
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          break;
        case 2: // Adressen bearbeiten
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress));
          break;
        case 3: // Startseite bearbeiten
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleMitglied));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditAdress));
          claims.Add(new Claim(ClaimTypes.Role, appAhnenforschungBackEnd.DataManager.CGlobal.AppUserRoleEditMainPage));
          break;
        case 4: // Administrator
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
        claims: claims, // new List<Claim>(),
        expires: DateTime.Now.AddMinutes(DURATION),
        signingCredentials: signinCredentials);
    }
  }
}
