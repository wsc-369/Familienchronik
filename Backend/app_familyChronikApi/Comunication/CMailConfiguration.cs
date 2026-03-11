using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace appAhnenforschungBackEnd.Comunication
{
  public class CMailConfiguration
  {
    public string m_strClient;
    public string m_strNETWORK_CREDENTIAL_USER;
    public string m_strNETWORK_CREDENTIAL_PW;

    public string Client { get { return m_strClient; } set { m_strClient = value; } }
    public string NETWORK_CREDENTIAL_USER { get { return m_strNETWORK_CREDENTIAL_USER; } set { m_strNETWORK_CREDENTIAL_USER = value; } }
    public string NETWORK_CREDENTIAL_PW { get { return m_strNETWORK_CREDENTIAL_PW; } set { m_strNETWORK_CREDENTIAL_PW = value; } }
  }
}
