//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
using appAhnenforschungBackEnd.Comunication;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace appAhnenforschungBackEnd.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        CReadWriteData _context = new CReadWriteData();

        private readonly ILogger<ServiceController> _logger;

        public ServiceController(ILogger<ServiceController> logger)
        {
            _logger = logger;
        }

        [HttpPost("SendMailByForgotPassword")]
        public IActionResult SendMailByForgotPassword([FromBody] CUser user)
        {
            bool valid = false;
            if (user == null)
            {
                return BadRequest("Invalid client request");
            }
            var oUser = _context.GetUserByEmail(user.Email);
            if (oUser != null)
            {

                CSendMailHelper oSendMail = new CSendMailHelper();
                oUser.Password = oSendMail.CreatePassword(8);
                _context.UpdateUserPassword(oUser);
                valid = oSendMail.SendMailByForgotPassword(oUser);
            }
            else
            {
                oUser = new CUser();
            }

            oUser.Password = string.Empty;

            if (!valid)
            {
                oUser.Email = "";
            }
            return Ok(oUser);

        }

        //static ILogger _logger;
        //public ServiceController(Microsoft.Extensions.Logging.ILoggerFactory factory)
        //{
        //  if (_logger == null)
        //    _logger = factory.Create("Unhandled Error");
        //}

        //public IActionResult Error()
        //{
        //  var feature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        //  var error = feature?.Error;
        //  _logger.LogError("Oops!", error);
        //  return View("~/Views/Shared/Error.cshtml", error);
        //}
        //[HttpPost("LoginNameAndPassword")]
        //public IActionResult SendEmail([FromBody]CUser user)
        //{
        //  if (user == null)
        //  {
        //    return BadRequest("Invalid client request");
        //  }

        //  CLAuthentication oAuthentication = new CLAuthentication();
        //  CAuthenticationUser resultAuthenticationUsers;

        //  resultAuthenticationUsers = oAuthentication.AuthenticationByLogin(user.LoginName, user.Password);

        //  //if (user.Email == "walter.schaedler@wsc.li" && user.Password == "ck595-4")
        //  if (resultAuthenticationUsers != null && resultAuthenticationUsers.LoginName == user.LoginName && resultAuthenticationUsers.Password == user.Password)
        //  {
        //    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345"));
        //    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        //    var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Name, resultAuthenticationUsers.LoginName),
        //        new Claim(ClaimTypes.Email, resultAuthenticationUsers.FirstName)
        //    };

        //    //  role1 = 'Mitglied',
        //    //  role2 = 'Adressen bearbeiten',
        //    //  role3 = 'Startseite bearbeiten',
        //    //  role4 = 'Administrator',


        //    switch (resultAuthenticationUsers.Role)
        //    {
        //      case 1:
        //        claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
        //        break;
        //      case 2:
        //        claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
        //        claims.Add(new Claim(ClaimTypes.Role, "EditAdress"));
        //        break;
        //      case 3:
        //        claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
        //        claims.Add(new Claim(ClaimTypes.Role, "EditAdress"));
        //        claims.Add(new Claim(ClaimTypes.Role, "EditMainPage"));
        //        break;
        //      case 4:
        //        claims.Add(new Claim(ClaimTypes.Role, "Mitglied"));
        //        claims.Add(new Claim(ClaimTypes.Role, "EditAdress"));
        //        claims.Add(new Claim(ClaimTypes.Role, "EditMainPage"));
        //        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
        //        break;
        //    }

        //    var tokeOptions = new JwtSecurityToken(
        //        issuer: ReadSettings.UrlTickenValidation(),
        //        audience: ReadSettings.UrlTickenValidation(),
        //        claims: claims, // new List<Claim>(),
        //        expires: DateTime.Now.AddMinutes(5),
        //        signingCredentials: signinCredentials
        //    );

        //    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        //    // Fake
        //    // resultAuthenticationUsers.Role = 1;

        //    resultAuthenticationUsers.Token = tokenString;
        //    return Ok(resultAuthenticationUsers);
        //  }
        //  else
        //  {
        //    return Unauthorized();
        //  }
        //}

    }
}