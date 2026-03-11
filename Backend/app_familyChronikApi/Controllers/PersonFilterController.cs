using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using appAhnenforschungBackEnd.Filters;
using appAhnenforschungData.DataManager;
using appAhnenforschungData.Models.App;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace appAhnenforschungBackEnd.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class PersonFilterController : ControllerBase
  {

    private readonly ILogger<PersonFilterController> _logger;

    public PersonFilterController(ILogger<PersonFilterController> logger)
    {
      _logger = logger;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{filterParam}", Name = "Get")]
    public IEnumerable<CPerson> GetFiltered([FromRoute] string filterParam)
    {
      CReadWriteData oReadWriteData = new CReadWriteData();
      FilterPersons oFilterPersons = new FilterPersons();
      List<CPerson> arlPersons = new List<CPerson>();
      List<CPerson> arlPersonsPreName = new List<CPerson>();
      List<CPerson> arlPersonsFirstName = new List<CPerson>();
     
      string[] filters = filterParam.Split(";");
      for (int i = 0; i < filters.Length; i++)
      {
        string[] filter = filters[i].Split("_");
        switch (i)
        {
          case 0:
            if (filter[0] == "personID" && filter[1] != "undefined")
            {
              oFilterPersons.personID = filter[1].ToString();
            }
            break;
          case 1:
            if (filter[0] == "firstName" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.firstName = filter[1].ToString();
            }
            break;
          case 2:
            if (filter[0] == "familyName" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.familyName = filter[1].ToString();
            }
            break;
         
          case 3:
            if (filter[0] == "birthDate" && filter[1].ToString() != "undefined")
            {
              // DD.MM.YYYY
              string[] filterbirthDate = filter[1].ToString().Split(".");
              oFilterPersons.birthDate = new DateTime(Convert.ToInt32(filterbirthDate[2]), Convert.ToInt32(filterbirthDate[1]), Convert.ToInt32(filterbirthDate[0])).ToShortDateString();
            }
            break;
          case 4:
            if (filter[0] == "older" && filter[1].ToString() != "undefined")
            {
              oFilterPersons.older = filter[1].ToString();
            }
            break;
                    
                    //case 5:
                    //    if (filter[0] == "dateFrom" && filter[1].ToString() != "undefined")
                    //    {
                    //        // DD.MM.YYYY
                    //        string[] filterbirthDate = filter[1].ToString().Split(".");
                    //        oFilterPersons.birthDate = new DateTime(Convert.ToInt32(filterbirthDate[2]), Convert.ToInt32(filterbirthDate[1]), Convert.ToInt32(filterbirthDate[0])).ToShortDateString();
                    //    }
                    //    break;

                    //case 6:
                    //    if (filter[0] == "dateUntil" && filter[1].ToString() != "undefined")
                    //    {
                    //        // DD.MM.YYYY
                    //        string[] filterbirthDate = filter[1].ToString().Split(".");
                    //        oFilterPersons.birthDate = new DateTime(Convert.ToInt32(filterbirthDate[2]), Convert.ToInt32(filterbirthDate[1]), Convert.ToInt32(filterbirthDate[0])).ToShortDateString();
                    //    }
                    //    break;


                    case 5:
            //i = filters.Length;
            break;
          default:
            Console.WriteLine("Default case");
            arlPersons = oReadWriteData.GetPersonWildcardFilterByPersonId("I-100", new CSettings());
            i = filters.Length;
            break;
        }
      }
      arlPersons = oReadWriteData.GetPersonWildcardFilter(oFilterPersons, new CSettings());
      return arlPersons.ToList();
    }
  }
}
