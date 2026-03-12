using System;
using System.Collections.Generic;
using System.Text;

namespace appAhnenforschungData.Models.App
{
  [Serializable()]
  public class CAuthenticationUser
  {
    public int UserId { get; set; }
    public string PersonId { get; set; }
    public string FirstName { get; set; }
    public string PreName { get; set; }
    public string Email { get; set; }
    public int Role { get; set; }
    public string LoginName { get; set; }
    public string Password { get; set; }
    public bool Active { get; set; }
    public string Token { get; set; }

    }
}
