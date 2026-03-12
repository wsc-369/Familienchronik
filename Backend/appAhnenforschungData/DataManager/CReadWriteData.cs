using appAhnenforschungBackEnd.Filters;
using appAhnenforschungData.Models.App;
using appAhnenforschungData.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Transactions;

namespace appAhnenforschungData.DataManager
{
  public class CReadWriteData
  {
    private wsc_chronikContext db = new wsc_chronikContext();
    private readonly List<TPerson> _persons;

    //public List<TPerson> Get_persons()
    //{
    //  return _persons;
    //}

    //public async void Initialize(List<TPerson> _persons)
    //{
    //  if (_persons == null)
    //  {
    //    _persons = await db.TPersons.ToListAsync();
    //  }
    //}

    // ------------------------------------------------------------------------------------------------------------------------------
    //  XML Serialization
    //  Using XML Serialization with C# and SQL Server
    //  https://www.c-sharpcorner.com/UploadFile/a5470d/using-xml-serialization-with-C-Sharp-and-sql-server/

    public CPerson GetFamilyPersonFromXml(CPerson oPerson)
    {
      try
      {

        CPerson operson = null;
        if (db.TFamilies.Any(t => t.StrPersonId == oPerson.PersonID))
        {
          TFamily tfamily = db.TFamilies.Single(t => t.StrPersonId == oPerson.PersonID);
          operson = DeserializationXMLToPersonObject(DecompressString(tfamily.StrTree));
        }

        return operson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Die Relationen einer Person speichern
    /// </summary>
    /// <param name="oPerson"></param>
    public void ModifiedFamilyPerson(CPerson oPerson)
    {
      try
      {
        if (!db.TFamilies.Any(t => t.StrPersonId == oPerson.PersonID))
        {
          TFamily tfamily = new TFamily();
          tfamily.StrPersonId = oPerson.PersonID;
          tfamily.StrTree = SerializationPersonObjectToXMLGeneric(oPerson);
          tfamily.DtCreate = DateTime.Now;
          tfamily.NActive = oPerson.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
          db.TFamilies.Attach(tfamily);
          db.Entry(tfamily).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();
        }
        else
        {
          TFamily tfamily = db.TFamilies.Single(t => t.StrPersonId == oPerson.PersonID);
          tfamily.StrTree = SerializationPersonObjectToXMLGeneric(oPerson);
          tfamily.DtUpdate = DateTime.Now;
          tfamily.NActive = oPerson.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
          db.TFamilies.Attach(tfamily);
          db.Entry(tfamily).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Compresses the string.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns></returns>
    private string CompressString(string text)
    {
      byte[] buffer = Encoding.UTF8.GetBytes(text);
      var memoryStream = new MemoryStream();
      using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
      {
        gZipStream.Write(buffer, 0, buffer.Length);
      }

      memoryStream.Position = 0;

      var compressedData = new byte[memoryStream.Length];
      memoryStream.Read(compressedData, 0, compressedData.Length);

      var gZipBuffer = new byte[compressedData.Length + 4];
      Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
      Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
      return Convert.ToBase64String(gZipBuffer);
    }


    /// <summary>
    /// Decompresses the string.
    /// Implemtierung fuer .net 6
    /// https://stackoverflow.com/questions/70933327/net-6-failing-at-decompress-large-gzip-text
    /// </summary>
    /// <param name="compressedText">The compressed text.</param>
    /// <returns></returns>
    private string DecompressString(string compressedText)
    {
      var gZipBuffer = Convert.FromBase64String(compressedText);

      using var memoryStream = new MemoryStream();
      int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
      memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

      var buffer = new byte[dataLength];
      memoryStream.Position = 0;

      using var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress);

      int totalRead = 0;
      while (totalRead < buffer.Length)
      {
        int bytesRead = gZipStream.Read(buffer, totalRead, buffer.Length - totalRead);
        if (bytesRead == 0) break;
        totalRead += bytesRead;
      }

      return Encoding.UTF8.GetString(buffer);
    }

    /// <summary>
    /// Get XML Serialization: Serialization
    /// </summary>
    /// <param name="oPerson"></param>
    /// <returns></returns>
    private string SerializationPersonObjectToXMLGeneric(CPerson oPerson)
    {
      try
      {
        string P = CompressString(ObjectToXMLGeneric(oPerson));
        return P;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    ///  Get XML Serialization: Deserialization
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    private CPerson DeserializationXMLToPersonObject(string xml)
    {
      try
      {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<CPerson>(xml);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Convert CPerson Object to XML: (Serialization)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="filter"></param>
    /// <returns></returns>
    private String ObjectToXMLGeneric(CPerson filter)
    {
      return Newtonsoft.Json.JsonConvert.SerializeObject(filter, Formatting.Indented);
    }

    /// <summary>
    /// Convert XML String to CPerson Object (Deserialization)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xml"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    private CPerson XMLToObject(string xml)
    {

      var serializer = new System.Xml.Serialization.XmlSerializer(typeof(CPerson));

      using (var textReader = new StringReader(xml))
      {
        using (var xmlReader = System.Xml.XmlReader.Create(textReader))
        {
          return (CPerson)serializer.Deserialize(xmlReader);
        }
      }
    }

    // ------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Eine Person mit der Entity ID (int)
    /// </summary>
    /// <param name="idPerson">int</param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public CPerson GetPersonByEntityID(int idPerson, CSettings i_oSettings)
    {
      try
      {

        CPerson oPerson = new CPerson();

        TPerson tperson = db.TPersons.FirstOrDefault(t => t.NPersonId == idPerson);
        if (tperson != null)
        {

          MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
        }
        return oPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Eine Person mit der PersonId (string)
    /// </summary>
    /// <param name="idPerson">string</param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public CPerson GetPersonByID(string idPerson, CSettings i_oSettings)
    {
      try
      {
        CPerson oPerson = new CPerson();

        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (tperson != null)
        {

          MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
        }
        return oPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CPerson GetPersonByID(string idPerson,List<TPerson> tPersons, CSettings i_oSettings)
    {
      try
      {
        CPerson oPerson = new CPerson();

        TPerson tperson = tPersons.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (tperson != null)
        {

          MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
        }
        return oPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Eine Person mit der PersonId (int)
    /// </summary>
    /// <param name="idPerson"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public CPerson GetPersonByID(int idPerson, CSettings i_oSettings)
    {
      try
      {
        CPerson oPerson = new CPerson();

        TPerson tperson = db.TPersons.FirstOrDefault(t => t.NPersonId == idPerson);
        if (tperson != null)
        {
          MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
        }
        return oPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CUser GetUserByEmail(string email)
    {
      try
      {
        CUser oUser = null;
        TUser tuser = db.TUsers.FirstOrDefault(t => t.StrEmail == email);
        if (tuser != null)
        {
          oUser = new CUser();
          MappUserEntityToModel(ref oUser, tuser);
        }
        return oUser;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Personen gemaess Filter
    /// </summary>
    /// <param name="strFilter"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public List<CPerson> GetPersonWildcardFilterByPersonId(string strPersonId, CSettings i_oSettings)
    {
      try
      {
        List<CPerson> arlPerson = new List<CPerson>();
        foreach (TPerson tperson in db.TPersons.Where(t => t.StrPersonId == strPersonId))
        {
          CPerson oPerson = new CPerson();
          MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
          arlPerson.Add(oPerson);
        }
        return arlPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public List<CPerson> GetPersonWildcardFilter(FilterPersons oFilterPersons, CSettings i_oSettings)
    {
      try
      {
        List<CPerson> arlPerson = new List<CPerson>();
        if (oFilterPersons.personID != null && oFilterPersons.personID.Length >= 1)
        {
          oFilterPersons.personID = oFilterPersons.personID.Replace("*", "");
          foreach (TPerson tperson in db.TPersons.Where(t => t.StrPersonId == oFilterPersons.personID))
          {
            CPerson oPerson = new CPerson();
            MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
            if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
            {
              arlPerson.Add(oPerson);
            }
          }
        }
        else if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1
           && oFilterPersons.familyName != null && oFilterPersons.familyName.Length >= 1)
        {
          // Ends
          if (oFilterPersons.firstName.EndsWith("*") && oFilterPersons.familyName.EndsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");

            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.StartsWith(oFilterPersons.familyName) && t.StrPreName.StartsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else if (oFilterPersons.firstName.StartsWith("*") && oFilterPersons.familyName.StartsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");

            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.EndsWith(oFilterPersons.familyName) && t.StrPreName.EndsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else if (oFilterPersons.firstName.EndsWith("*") && oFilterPersons.familyName.StartsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");

            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.StartsWith(oFilterPersons.familyName) && t.StrPreName.EndsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else if (oFilterPersons.firstName.StartsWith("*") && oFilterPersons.familyName.EndsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");

            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.EndsWith(oFilterPersons.familyName) && t.StrPreName.StartsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else if (oFilterPersons.firstName.StartsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.familyName && t.StrPreName.EndsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }

          }
          else if (oFilterPersons.firstName.EndsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.familyName && t.StrPreName.StartsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }

          }
          else if (oFilterPersons.familyName.StartsWith("*"))
          {
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.EndsWith(oFilterPersons.familyName) && t.StrPreName == oFilterPersons.firstName))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else if (oFilterPersons.familyName.EndsWith("*"))
          {
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.StartsWith(oFilterPersons.familyName) && t.StrPreName == oFilterPersons.firstName))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.familyName && t.StrPreName == oFilterPersons.firstName))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
        }
        else if (oFilterPersons.firstName != null && oFilterPersons.firstName.Length >= 1 && oFilterPersons.familyName == null)
        {
          if (oFilterPersons.firstName.StartsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.familyName && t.StrPreName.EndsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }

          }
          else if (oFilterPersons.firstName.EndsWith("*"))
          {
            oFilterPersons.firstName = oFilterPersons.firstName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrPreName.StartsWith(oFilterPersons.firstName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrPreName == oFilterPersons.firstName))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
        }
        else if (oFilterPersons.familyName != null && oFilterPersons.familyName.Length >= 1 && oFilterPersons.firstName == null)
        {
          if (oFilterPersons.familyName.StartsWith("*"))
          {
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.EndsWith(oFilterPersons.familyName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else if (oFilterPersons.familyName.EndsWith("*"))
          {
            oFilterPersons.familyName = oFilterPersons.familyName.Replace("*", "");
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName.StartsWith(oFilterPersons.familyName)))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrName == oFilterPersons.familyName))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
        }

        bool OnlyFilterDate = false;
        // Exist filters
        if (oFilterPersons.familyName == null && oFilterPersons.firstName == null)
        {
          OnlyFilterDate = true;
        }
        else if (oFilterPersons.familyName == null && oFilterPersons.firstName != null)
        {
          if (oFilterPersons.firstName.Length == 0)
          {
            OnlyFilterDate = true;
          }
        }
        else if (oFilterPersons.familyName != null && oFilterPersons.firstName == null)
        {
          if (oFilterPersons.familyName.Length == 0)
          {
            OnlyFilterDate = true;
          }
        }

        if (oFilterPersons.birthDate != null && oFilterPersons.birthDate != "undefined")
        {
          DateTime dtBirt = DateTime.Parse(oFilterPersons.birthDate);
          if (arlPerson.Count == 0 && OnlyFilterDate == true)
          {
            var tPersons = db.TPersons.Where(t => t.NBirthMonth == dtBirt.Month && t.NBirthDay == dtBirt.Day).ToList();
            var personIds = arlPerson.Select(p => p.PersonID).ToHashSet();

            foreach (var person in tPersons)
            {
              var oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, person, tPersons, i_oSettings);

              if (!personIds.Contains(oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
                personIds.Add(oPerson.PersonID); // Hinzufügen des neuen PersonIDs
              }
            }
          }
          else
          {
            arlPerson = arlPerson.Where(t => t.BirthDate.Month == dtBirt.Month && t.BirthDate.Day == dtBirt.Day).ToList();
          }

        }
        if (oFilterPersons.deathDate != null && oFilterPersons.deathDate != "undefined")
        {
          DateTime dtDeath = DateTime.Parse(oFilterPersons.deathDate);
          if (arlPerson.Count == 0 && OnlyFilterDate == true)
          {
            var tPersons = db.TPersons.Where(t => t.NDeathMonth == dtDeath.Month && t.NDeathDay == dtDeath.Day).ToList();
            foreach (TPerson tperson in tPersons)
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else
          {
            arlPerson = arlPerson.Where(t => t.DeathDate.Month == dtDeath.Month && t.DeathDate.Day == dtDeath.Day).ToList();
          }
        }


        if (oFilterPersons.birthYear != null && oFilterPersons.birthYear != "undefined" && Convert.ToInt32(oFilterPersons.birthYear) > 0)
        {
          if (arlPerson.Count == 0 && OnlyFilterDate == true)
          {
            var tPersons = db.TPersons.Where(t => t.NBirthYear == Convert.ToInt32(oFilterPersons.birthYear)).ToList();
            foreach (TPerson tperson in tPersons)
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else
          {
            arlPerson = arlPerson.Where(t => t.BirthDate.Year == Convert.ToInt32(oFilterPersons.birthYear)).ToList();
          }
        }


        if (oFilterPersons.older != null && oFilterPersons.older != "undefined" && Convert.ToInt32(oFilterPersons.older) > 0)
        {
          DateTime dtBirthYear = DateTime.Now.AddYears(-Convert.ToInt32(oFilterPersons.older));
          if (arlPerson.Count == 0 && OnlyFilterDate == true)
          {
            var tPersons = db.TPersons.Where(t => t.NBirthYear == dtBirthYear.Year && t.TkDeath == "0").ToList();
            foreach (TPerson tperson in tPersons)
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
              if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
              {
                arlPerson.Add(oPerson);
              }
            }
          }
          else
          {
            arlPerson = arlPerson.Where(t => t.Older == Convert.ToInt32(oFilterPersons.older)).ToList();
          }
        }


        if (oFilterPersons.dateFrom != null && oFilterPersons.dateFrom != "undefined" && oFilterPersons.dateUntil != null && oFilterPersons.dateUntil != "undefined")
        {
          DateTime dateFrom = DateTime.Parse(oFilterPersons.dateFrom);
          DateTime dateUntil = DateTime.Parse(oFilterPersons.dateUntil);

          var tPersons = db.TPersons.Where(t => Convert.ToInt64(t.TikBirth) >= Convert.ToInt64(dateFrom.Ticks)
                                                          && Convert.ToInt64(t.TikBirth) <= Convert.ToInt64(dateUntil.Ticks)).ToList();
          foreach (TPerson tperson in tPersons)
          {
            CPerson oPerson = new CPerson();
            MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
            if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
            {
              arlPerson.Add(oPerson);
            }
          }
        }


        else if (oFilterPersons.wildCardText != null && oFilterPersons.wildCardText.Length >= 1)
        {
          var tPersons = db.TPersons.ToList();
          var tRemark = db.TRemarks.Where(t => t.StrRemarks.Contains(oFilterPersons.wildCardText) || t.StrRemarksClean.Contains(oFilterPersons.wildCardText)).ToList();
          foreach (TRemark tremark in tRemark)
          {
            CPerson oPerson = new CPerson();
            TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == tremark.StrPersonId);
            MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
            if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
            {
              arlPerson.Add(oPerson);
            }
          }

          foreach (TObituary tobituary in db.TObituaries.Where(t => t.StrObituary.Contains(oFilterPersons.wildCardText)))
          {
            CPerson oPerson = new CPerson();
            TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == tobituary.StrPersonId);
            MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
            if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
            {
              arlPerson.Add(oPerson);
            }
          }

          foreach (TPersonPortrait tpersonPortrait in db.TPersonPortraits.Where(t => t.StrRemarks.Contains(oFilterPersons.wildCardText) || t.StrTitle.Contains(oFilterPersons.wildCardText)))
          {
            CPerson oPerson = new CPerson();
            TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == tpersonPortrait.StrPersonId);
            MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
            if (!arlPerson.Any(element => element.PersonID == oPerson.PersonID))
            {
              arlPerson.Add(oPerson);
            }
          }
        }

        return arlPerson;
      }
      catch (Exception)
      {
        throw;
      }
    }


    public (List<CPerson>, List<TPerson> tPersons) GetPersonsIncludedTable(CSettings i_oSettings)
    {
      try
      {
        List<CPerson> arlPersons = new List<CPerson>();

        CPerson oPerson;
        var tPersons = db.TPersons.ToList();
        foreach (TPerson tperson in tPersons.Where(x=> x.NBirthYear>1960))
        {
          oPerson = new CPerson();
          MappPersonEntityToModel(ref oPerson, tperson, tPersons, i_oSettings);
          arlPersons.Add(oPerson);
        }

        return (arlPersons, tPersons);

      }
      catch (Exception)
      {
        throw;
      }
    }

    public List<CPerson> GetPersons(CSettings i_oSettings)
    {
      try
      {
        List<CPerson> arlPersons = new List<CPerson>();

        CPerson oPerson;
        foreach (TPerson tperson in db.TPersons)
        {
          oPerson = new CPerson();
          MappPersonEntityToModel(ref oPerson, tperson, i_oSettings);
          arlPersons.Add(oPerson);
        }

        return arlPersons;

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Alle Namen gruppiert
    /// </summary>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public List<string> GetPersonNamesGrouped(CSettings i_oSettings)
    {
      try
      {
        List<string> arlNames = new List<string>();

        IEnumerable<TPerson> groups = db.TPersons.Select(data => data.StrName)
                                 .Distinct()
                                 .Select(name => new TPerson()
                                 {
                                   StrName = name
                                 });

        foreach (var group in groups.Where(x => x.StrName.Length > 0).OrderBy(x => x.StrName))
        {
          arlNames.Add(group.StrName);
        }
        return arlNames;

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Alle Voranem gruppiert
    /// </summary>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public List<string> GetPersonPreNamesGrouped(CSettings i_oSettings)
    {
      try
      {
        List<string> arlNames = new List<string>();

        IEnumerable<TPerson> groups = db.TPersons.Select(data => data.StrPreName)
                                 .Distinct()
                                 .Select(name => new TPerson()
                                 {
                                   StrPreName = name
                                 });

        foreach (var group in groups.Where(x => x.StrPreName.Length > 0).OrderBy(x => x.StrPreName))
        {
          arlNames.Add(group.StrPreName);
        }
        return arlNames;

      }
      catch (Exception)
      {
        throw;
      }
    }



    /// <summary>
    /// Einen User auslesen
    /// </summary>
    /// <param name="idUser"></param>
    /// <returns></returns>
    public List<CUser> GetUsers()
    {
      try
      {
        List<CUser> arlUser = new List<CUser>();
        CUser oUser;
        foreach (TUser tuser in db.TUsers)
        {
          oUser = new CUser();
          MappUserEntityToModel(ref oUser, tuser);
          arlUser.Add(oUser);
        }

        return arlUser;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CUser GetUserByID(int idUser)
    {
      try
      {
        CUser oUser = new CUser();

        TUser tuser = db.TUsers.FirstOrDefault(t => t.NUserId == idUser);
        if (tuser != null)
        {
          MappUserEntityToModel(ref oUser, tuser);
        }
        return oUser;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CUser AddUser(CUser user)
    {
      try
      {
        TUser tuser = new TUser();
        MappUserModelEntity(ref tuser, user);

        db.TUsers.Attach(tuser);
        db.Entry(tuser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        int returnValue = db.SaveChanges();
        var id = tuser.NUserId;

        return GetUserByID(tuser.NUserId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CUser UpdateUser(CUser user)
    {
      try
      {
        TUser tuser = new TUser();
        MappUserModelEntity(ref tuser, user);

        db.TUsers.Attach(tuser);
        db.Entry(tuser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        int returnValue = db.SaveChanges();
        var id = tuser.NUserId;

        return GetUserByID(tuser.NUserId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CUser UpdateUserPassword(CUser user)
    {
      try
      {
        TUser tuser = db.TUsers.FirstOrDefault(t => t.NUserId == user.UserId);
        tuser.StrPassword = user.Password;

        db.TUsers.Attach(tuser);
        db.Entry(tuser).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        int returnValue = db.SaveChanges();
        var id = tuser.NUserId;

        return GetUserByID(tuser.NUserId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    public void DeleteFamilyList()
    {
      try
      {
        db.TFamilies.RemoveRange(db.TFamilies);
        db.SaveChanges();
      }
      catch (Exception)
      {
        throw;
      }
    }


    public void DeleteKinshipConnection()
    {
      try
      {
        db.TKinshipConnections.RemoveRange(db.TKinshipConnections);
        db.SaveChanges();
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Verwandtschaftverbindungen
    /// </summary>
    public void RefreshKinshipConnection()
    {
      try
      {
        CSettings oSettings = new CSettings();
        List<CPerson> arlPersons = new List<CPerson>();
        List<CPerson> arlPersonsClone = new List<CPerson>();
        List<CPerson> arlChildren = new List<CPerson>();
        List<CKinshipConnection> arlCKinshipConnections = new List<CKinshipConnection>();
        List<CKinshipConnection> arlCKinshipConnectionsCopy = new List<CKinshipConnection>();
        CPerson oParent = new CPerson();

        arlPersons = GetPersons(oSettings);
        arlPersonsClone = arlPersons;
        foreach (CPerson p in arlPersons)
        {
          oParent.Childs = new List<CPerson>();
          if (p.FatherID != null && p.FatherID != "-1")
          {
            CKinshipConnection konship = new CKinshipConnection();
            konship.ParentId = p.FatherID;
            konship.PersonId = p.PersonID;
            if (!arlCKinshipConnectionsCopy.Any(x => x.ParentId == konship.ParentId && x.PersonId == konship.PersonId))
            {
              arlCKinshipConnections.Add(konship);
              arlCKinshipConnectionsCopy.Add(konship);
            }
            foreach (CPerson person in arlPersonsClone.Where(x => x.FatherID == p.PersonID).OrderBy(x => x.tikBirth))
            {
              oParent.Childs.Add(person);
              person.Childs = new List<CPerson>();
              person.Partners = new List<CPartner>();
              IterationRelationDown(arlCKinshipConnections, arlCKinshipConnectionsCopy, arlPersonsClone, p, person);
            }
          }
          else
          {
            CKinshipConnection konship = new CKinshipConnection();
            konship.ParentId = "-1";
            konship.PersonId = p.PersonID;
            if (!arlCKinshipConnectionsCopy.Any(x => x.ParentId == konship.ParentId && x.PersonId == konship.PersonId))
            {
              arlCKinshipConnections.Add(konship);
              arlCKinshipConnectionsCopy.Add(konship);
            }
          }

          if (p.MotherID != null && p.MotherID != "-1")
          {
            CKinshipConnection konship = new CKinshipConnection();
            konship.ParentId = p.MotherID;
            konship.PersonId = p.PersonID;
            if (!arlCKinshipConnectionsCopy.Any(x => x.ParentId == konship.ParentId && x.PersonId == konship.PersonId))
            {
              arlCKinshipConnections.Add(konship);
              arlCKinshipConnectionsCopy.Add(konship);
            }
            foreach (CPerson person in arlPersonsClone.Where(x => x.MotherID == p.PersonID).OrderBy(x => x.tikBirth))
            {
              oParent.Childs.Add(person);
              person.Childs = new List<CPerson>();
              person.Partners = new List<CPartner>();
              IterationRelationDown(arlCKinshipConnections, arlCKinshipConnectionsCopy, arlPersonsClone, p, person);
            }
          }
          else
          {
            CKinshipConnection konship = new CKinshipConnection();
            konship.ParentId = "-1";
            konship.PersonId = p.PersonID;
            if (!arlCKinshipConnectionsCopy.Any(x => x.ParentId == konship.ParentId && x.PersonId == konship.PersonId))
            {
              arlCKinshipConnections.Add(konship);
              arlCKinshipConnectionsCopy.Add(konship);
            }
          }
          UpdateAddedKinshipConnection(arlCKinshipConnections);
          arlCKinshipConnections.Clear();
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    private void IterationRelationDown(List<CKinshipConnection> arlCKinshipConnections, List<CKinshipConnection> arlCKinshipConnectionsCopy, List<CPerson> arlPersonsClone, CPerson oFirstParent, CPerson oParent)
    {
      foreach (CPerson child in GetChildrenSmallByPersonID(arlPersonsClone, oParent.PersonID).OrderBy(x => x.tikBirth))
      {
        CKinshipConnection konship = new CKinshipConnection();
        konship.ParentId = oFirstParent.PersonID;
        konship.PersonId = child.PersonID;

        //System.Diagnostics.Debug.WriteLine(child.PersonID + " " + child.Fullname);
        oParent.Childs.Add(child);
        child.Childs = new List<CPerson>();

        //child.Partners = new List<CPartner>();
        //foreach (CPartner partner in GetPartnersByPersonID(child.PersonID, oSettings))
        //{
        //  System.Diagnostics.Debug.WriteLine(partner.PartnerID + " " + partner.Person.Fullname);
        //  child.Partners.Add(partner);

        //}
        //CallAddKinshipConnection(konship);
        if (!arlCKinshipConnectionsCopy.Any(x => x.ParentId == konship.ParentId && x.PersonId == konship.PersonId))
        {
          arlCKinshipConnections.Add(konship);
          arlCKinshipConnectionsCopy.Add(konship);
        }
        IterationRelationDown(arlCKinshipConnections, arlCKinshipConnectionsCopy, arlPersonsClone, oFirstParent, child);
      }
    }


    /// <summary>
    /// Ein Partner mit der PersonId (string)
    /// </summary>
    /// <param name="idPerson">string</param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public CPartner GetPartnerByID(string idPerson, CSettings i_oSettings)
    {
      try
      {
        CPartner partner = new CPartner();
        TPartner tperson = db.TPartners.FirstOrDefault(x => x.StrPartnerId == idPerson);
        {

          partner.PartnerID = Convert.ToString(tperson.StrPartnerId);
          partner.PersonID = Convert.ToString(tperson.StrPersonId);

          // Geheiratet am
          if (tperson.TikMarriageDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikMarriageDate = Convert.ToUInt64(tperson.TikMarriageDate);
            partner.MarriageDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate));
          }
          else
          {
            partner.tikMarriageDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.MarriageDateTime = CGlobal.CliensSideEmptyDate();
          }

          // Geschieden am
          if (tperson.TikDivorceDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikDivorceDate = Convert.ToUInt64(tperson.TikDivorceDate);
            partner.DivorceDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate));
          }
          else
          {
            partner.tikDivorceDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.DivorceDateTime = CGlobal.CliensSideEmptyDate();
          }

          partner.MarriageDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)).ToShortDateString();
          partner.DivorceDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)).ToShortDateString();
          partner.IsMarriageDate = (tperson.TikMarriageDate != "0");
          partner.IsDivorceDate = (tperson.TikDivorceDate != "0");

          //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
          //tpartner.nPersonID = partner.ID; // nPersonID
          partner.Person = GetPersonByID(tperson.StrPersonId, i_oSettings);
        }

        return partner;

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Get Partner mit der PersonId (int)
    /// </summary>
    /// <param name="nPersonId"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public CPartner GetPartnerByID(int nPersonId, CSettings i_oSettings)
    {
      try
      {
        CPartner partner = new CPartner();
        TPartner tperson = db.TPartners.FirstOrDefault(x => x.NPartnerId == nPersonId);
        {

          partner.PartnerID = Convert.ToString(tperson.StrPartnerId);

          // Geheiratet am
          if (tperson.TikMarriageDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikMarriageDate = Convert.ToUInt64(tperson.TikMarriageDate);
            partner.MarriageDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate));
          }
          else
          {
            partner.tikMarriageDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.MarriageDateTime = CGlobal.CliensSideEmptyDate();
          }

          // Geschieden am
          if (tperson.TikDivorceDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikDivorceDate = Convert.ToUInt64(tperson.TikDivorceDate);
            partner.DivorceDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate));
          }
          else
          {
            partner.tikDivorceDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.DivorceDateTime = CGlobal.CliensSideEmptyDate();
          }

          partner.MarriageDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)).ToShortDateString();
          partner.DivorceDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)).ToShortDateString();
          partner.IsMarriageDate = (tperson.TikMarriageDate != "0");
          partner.IsDivorceDate = (tperson.TikDivorceDate != "0");

          //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
          //tpartner.nPersonID = partner.ID; // nPersonID
          partner.Person = GetPersonByID(tperson.StrPersonId, i_oSettings);
        }

        return partner;

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Alle Partner zu einer Person
    /// </summary>
    /// <param name="idPerson"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public List<CPartner> GetPartnersByPersonID(string idPerson, CSettings i_oSettings)
    {
      try
      {

        List<CPartner> arlPartners = new List<CPartner>();

        foreach (TPartner tperson in db.TPartners.Where(x => x.StrPartnerId == idPerson))
        {
          CPartner partner = new CPartner();
          partner.PartnerID = Convert.ToString(tperson.StrPartnerId);
          partner.PersonID = Convert.ToString(tperson.StrPersonId);

          // Geheiratet am
          if (tperson.TikMarriageDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikMarriageDate = Convert.ToUInt64(tperson.TikMarriageDate);
            partner.MarriageDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate));
          }
          else
          {
            partner.tikMarriageDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.MarriageDateTime = CGlobal.CliensSideEmptyDate();
          }

          // Geschieden am
          if (tperson.TikDivorceDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikDivorceDate = Convert.ToUInt64(tperson.TikDivorceDate);
            partner.DivorceDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate));
          }
          else
          {
            partner.tikDivorceDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.DivorceDateTime = CGlobal.CliensSideEmptyDate();
          }

          partner.MarriageDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)) == CGlobal.CliensSideEmptyDate() ? "" : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)).ToShortDateString();
          partner.DivorceDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)) == CGlobal.CliensSideEmptyDate() ? "" : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)).ToShortDateString();
          partner.IsMarriageDate = (tperson.TikMarriageDate != "0");
          partner.IsDivorceDate = (tperson.TikDivorceDate != "0");

          //partner.LiveDate = "* " + oPartner.BirthDisplay + " † " + oPartner.DeathDisplay;
          //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
          //partner.PersonID = partner.ID; // nPersonID
          partner.Person = GetPersonByID(tperson.StrPersonId, i_oSettings);

          arlPartners.Add(partner);

        }

        return arlPartners;

      }
      catch (Exception)
      {
        throw;
      }
    }


    public List<CPartner> GetPartnersByPersonID(string idPerson, List<TPerson> tPersons, CSettings i_oSettings)
    {
      try
      {

        List<CPartner> arlPartners = new List<CPartner>();

        foreach (TPartner tperson in db.TPartners.Where(x => x.StrPartnerId == idPerson))
        {
          CPartner partner = new CPartner();
          partner.PartnerID = Convert.ToString(tperson.StrPartnerId);
          partner.PersonID = Convert.ToString(tperson.StrPersonId);

          // Geheiratet am
          if (tperson.TikMarriageDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikMarriageDate = Convert.ToUInt64(tperson.TikMarriageDate);
            partner.MarriageDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate));
          }
          else
          {
            partner.tikMarriageDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.MarriageDateTime = CGlobal.CliensSideEmptyDate();
          }

          // Geschieden am
          if (tperson.TikDivorceDate != null && CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)) != CGlobal.CliensSideEmptyDate())
          {
            partner.tikDivorceDate = Convert.ToUInt64(tperson.TikDivorceDate);
            partner.DivorceDateTime = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate));
          }
          else
          {
            partner.tikDivorceDate = Convert.ToUInt64(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
            partner.DivorceDateTime = CGlobal.CliensSideEmptyDate();
          }

          partner.MarriageDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)) == CGlobal.CliensSideEmptyDate() ? "" : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikMarriageDate)).ToShortDateString();
          partner.DivorceDateDisplay = CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)) == CGlobal.CliensSideEmptyDate() ? "" : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikDivorceDate)).ToShortDateString();
          partner.IsMarriageDate = (tperson.TikMarriageDate != "0");
          partner.IsDivorceDate = (tperson.TikDivorceDate != "0");

          //partner.LiveDate = "* " + oPartner.BirthDisplay + " † " + oPartner.DeathDisplay;
          //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
          //partner.PersonID = partner.ID; // nPersonID
          partner.Person = GetPersonByID(tperson.StrPersonId, tPersons, i_oSettings);

          arlPartners.Add(partner);

        }

        return arlPartners;

      }
      catch (Exception)
      {
        throw;
      }
    }


    public List<CPerson> GetChildrenSmallByPersonID(List<CPerson> arlPersonsClone, string idPerson)
    {
      try
      {

        List<CPerson> arlchildrens = new List<CPerson>();

        CPerson operson = arlPersonsClone.FirstOrDefault(t => t.PersonID == idPerson);
        if (operson != null)
        {
          if (operson.Sex == "M")
          {
            foreach (CPerson oPerson in arlPersonsClone.Where(t => t.FatherID == operson.PersonID))
            {
              arlchildrens.Add(oPerson);
            }

          }
          else if (operson.Sex == "F")
          {
            foreach (CPerson oPerson in arlPersonsClone.Where(t => t.MotherID == operson.PersonID))
            {
              arlchildrens.Add(oPerson);
            }
          }
        }

        return arlchildrens;
      }
      catch (Exception)
      {
        throw;
      }
    }


    /// <summary>
    /// Alle Kinder zu einer Person
    /// </summary>
    /// <param name="idPerson"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public List<CPerson> GetChildrenByPersonID(string idPerson, CSettings oSettings)
    {
      try
      {

        List<CPerson> arlchildrens = new List<CPerson>();

        TPerson operson = db.TPersons.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (operson != null)
        {
          if (operson.StrSex == "M")
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrFatherId == operson.StrPersonId))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, oSettings);
              arlchildrens.Add(oPerson);
            }

          }
          else if (operson.StrSex == "F")
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrMotherId == operson.StrPersonId))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, oSettings);
              arlchildrens.Add(oPerson);
            }
          }
        }

        return arlchildrens;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Alle Kinder von einem Paar
    /// </summary>
    /// <param name="idPerson"> Father or Mother</param>
    /// <param name="idPartner"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public List<CPerson> GetChildrensByParents(string idPerson, string idPartner, CSettings oSettings)
    {
      try
      {

        List<CPerson> arlchildrens = new List<CPerson>();

        TPerson operson = db.TPersons.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (operson != null)
        {
          if (operson.StrSex == "M")
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrFatherId == operson.StrPersonId && t.StrMotherId == idPartner))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, oSettings);
              arlchildrens.Add(oPerson);
            }

          }
          else if (operson.StrSex == "F")
          {
            foreach (TPerson tperson in db.TPersons.Where(t => t.StrMotherId == operson.StrPersonId && t.StrFatherId == idPartner))
            {
              CPerson oPerson = new CPerson();
              MappPersonEntityToModel(ref oPerson, tperson, oSettings);
              arlchildrens.Add(oPerson);
            }
          }
        }

        return arlchildrens;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Die Bereibung zur Person
    /// </summary>
    /// <param name="idPerson"></param>
    /// <returns></returns>
    public CRemark GetRemarkByPersonID(string idPerson)
    {
      try
      {
        CRemark remark = new CRemark();
        TRemark tremark = db.TRemarks.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (tremark != null)
        {
          remark.PersonID = tremark.StrPersonId;
          remark.Remarks = tremark.StrRemarks;
          if (tremark.StrRemarksClean == null || tremark.StrRemarksClean.Trim().Length == 0)
          {
            remark.RemarksClean = tremark.StrRemarks;
          }
          else
          {
            remark.RemarksClean = tremark.StrRemarksClean;
          }
          remark.Active = tremark.BActiv.Value; // = true ? true : false;

        }
        return remark;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Den Nachruf zur Person
    /// </summary>
    /// <param name="idPerson"></param>
    /// <param name="oSettings"></param>
    /// <returns></returns>
    public CObituary GetObituaryByPersonID(string idPerson)
    {
      try
      {
        CObituary obituary = new CObituary();
        TObituary tobituary = db.TObituaries.FirstOrDefault(t => t.StrPersonId == idPerson);
        if (tobituary != null)
        {
          //remark.ID=tremark.nRemarkID;
          obituary.PersonID = tobituary.StrPersonId;
          obituary.Obituary = tobituary.StrObituary;
          obituary.Active = tobituary.BActiv;
        }
        return obituary;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Portait
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CPersonPortrait GetPersonPortraitByID(int id)
    {
      try
      {
        CPersonPortrait portrait = new CPersonPortrait();
        TPersonPortrait tpPersonPortrait = db.TPersonPortraits.FirstOrDefault(t => t.NPersonPortraitId == id);
        if (tpPersonPortrait != null)
        {
          MappPersonPortraitEntityToModel(ref portrait, tpPersonPortrait);
        }
        return portrait;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Portait zur Person
    /// </summary>
    /// <param name="idPerson"></param>
    /// <returns></returns>
    public List<CPersonPortrait> GetPersonPortraitsByPersonID(string idPerson)
    {
      try
      {

        List<CPersonPortrait> portraits = new List<CPersonPortrait>();
        foreach (TPersonPortrait tpPersonPortrait in db.TPersonPortraits.Where(t => t.StrPersonId == idPerson))
        {
          CPersonPortrait portrait = new CPersonPortrait();
          MappPersonPortraitEntityToModel(ref portrait, tpPersonPortrait);
          portraits.Add(portrait);
        }

        return portraits;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Wurzel der Verwandtschaftverbindungen ermittlen
    /// Es sind aktuell nur zwei Personen möglich
    /// </summary>
    /// <param name="param"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public List<CPerson> GetKinshipConnections(string param, CSettings i_oSettings)
    {
      try
      {
        List<CKinshipConnection> arlKinshipConnections = new List<CKinshipConnection>();
        List<CPerson> arlPerson = new List<CPerson>();

        param = param.Replace(";", ",");
        param = param.Remove(param.Length - 1, 1);

        var _params = param.Split(",");

        switch (_params.Length)
        {
          case 1:
            Console.WriteLine("Case 1");
            break;
          case 2:
            foreach (TKinshipConnection _table in db.TKinshipConnections.Where(p => p.StrPersonId == _params[0] || p.StrPersonId == _params[1]))
            {
              CKinshipConnection oKinshipConnection = new CKinshipConnection();
              MappKinshipConnectionEntityToModel(ref oKinshipConnection, _table);
              arlKinshipConnections.Add(oKinshipConnection);
            }
            break;
          case 3:
            return GetKinshipConnectionsTree(_params, i_oSettings);
          // break;
          default:
            Console.WriteLine("Default case");
            break;
        }

        //foreach (TKinshipConnections _table in db.TKinshipConnections.Where(p => p.StrPersonId == _params[0] || p.StrPersonId == _params[1]))
        //  {
        //  CKinshipConnection oKinshipConnection = new CKinshipConnection();
        //  MappKinshipConnectionEntityToModel(ref oKinshipConnection, _table);
        //  arlKinshipConnections.Add(oKinshipConnection);
        //}

        List<CKinshipConnection> arlKinshipConnectionsGrouped = new List<CKinshipConnection>();
        IEnumerable<IGrouping<string, CKinshipConnection>> groups = (IEnumerable<IGrouping<string, CKinshipConnection>>)arlKinshipConnections.GroupBy(keySelector: g => g.ParentId);
        IEnumerable<CKinshipConnection> smths = groups.SelectMany(group => group);

        foreach (CKinshipConnection _item in smths.OrderBy(o => o.ParentId).ThenBy(o => o.PersonId))
        {
          if (_item.PersonId == _params[0])
          {
            foreach (CKinshipConnection _item1 in smths.Where(p => p.ParentId == _item.ParentId && p.PersonId == _params[1]))
            {
              var person = GetPersonByID(_item1.ParentId, i_oSettings);
              if (!arlPerson.Any(x => x.PersonID == person.PersonID))
              {
                arlPerson.Add(person);
              }
            }
          }
          else if (_item.PersonId == _params[1])
          {
            foreach (CKinshipConnection _item1 in smths.Where(p => p.ParentId == _item.ParentId && p.PersonId == _params[0]))
            {
              var person = GetPersonByID(_item1.ParentId, i_oSettings);
              if (!arlPerson.Any(x => x.PersonID == person.PersonID))
              {
                arlPerson.Add(person);
              }
            }
          }
        }

        // Die Verbindung mit dem jüngsten Geburtstag
        return arlPerson.OrderByDescending(x => x.tikBirth.ToString()).ToList();
      }
      catch (Exception)
      {
        throw;
      }
    }

    public List<CPerson> GetKinshipConnectionsTree(string[] param, CSettings i_oSettings)
    {
      try
      {
        List<CKinshipConnection> arlKinshipConnections = new List<CKinshipConnection>();
        List<CPerson> arlPerson = new List<CPerson>();

        //param = param.Replace(";", ",");
        //param = param.Remove(param.Length - 1, 1);

        //var _params = param.Split(",");



        foreach (TKinshipConnection _table in db.TKinshipConnections.Where(p => p.StrPersonId == param[0] || p.StrPersonId == param[1]))
        {
          CKinshipConnection oKinshipConnection = new CKinshipConnection();
          MappKinshipConnectionEntityToModel(ref oKinshipConnection, _table);
          arlKinshipConnections.Add(oKinshipConnection);
        }

        List<CKinshipConnection> arlKinshipConnectionsGrouped = new List<CKinshipConnection>();
        IEnumerable<IGrouping<string, CKinshipConnection>> groups = (IEnumerable<IGrouping<string, CKinshipConnection>>)arlKinshipConnections.GroupBy(keySelector: g => g.ParentId);
        IEnumerable<CKinshipConnection> smths = groups.SelectMany(group => group);

        foreach (CKinshipConnection _item in smths.OrderBy(o => o.ParentId).ThenBy(o => o.PersonId))
        {
          if (_item.PersonId == param[0])
          {
            foreach (CKinshipConnection _item1 in smths.Where(p => p.ParentId == _item.ParentId && p.PersonId == param[1]))
            {
              var person = GetPersonByID(_item1.ParentId, i_oSettings);
              if (!arlPerson.Any(x => x.PersonID == person.PersonID))
              {
                arlPerson.Add(person);
              }
            }
          }
          else if (_item.PersonId == param[1])
          {
            foreach (CKinshipConnection _item1 in smths.Where(p => p.ParentId == _item.ParentId && p.PersonId == param[0]))
            {
              var person = GetPersonByID(_item1.ParentId, i_oSettings);
              if (!arlPerson.Any(x => x.PersonID == person.PersonID))
              {
                arlPerson.Add(person);
              }
            }
          }
        }

        // Die Verbindung mit dem jüngsten Geburtstag
        return arlPerson.OrderByDescending(x => x.tikBirth.ToString()).ToList();
      }
      catch (Exception)
      {
        throw;
      }
    }


    /// <summary>
    ///  Die Adresse zur Person
    /// </summary>
    /// <param name="strPersonId"></param>
    /// <returns></returns>
    public CAddress GetAddressByID(int nPersonId)
    {
      try
      {
        CAddress address = new CAddress();
        TAdress taddress = db.TAdresses.FirstOrDefault(x => x.NAdressId == nPersonId);
        {
          address.AddressId = taddress.NAdressId;
          address.PersonId = Convert.ToString(taddress.StrPersonId);
          address.HouseNr = Convert.ToString(taddress.StrHouseNr).Trim();
          address.Adresse = Convert.ToString(taddress.StrAdresse);
          address.Zip = Convert.ToString(taddress.StrZip);
          address.Town = Convert.ToString(taddress.StrTown);
          address.Country = Convert.ToString(taddress.StrCountry);
          address.CreateCreate = taddress.DtCreate;
          address.UpdateDate = taddress.DtUpdate;
          address.OrderNr = taddress.NOrderNr;
          address.Active = taddress.BActive;
        }
        return address;

      }
      catch (Exception)
      {
        throw;
      }
    }

    public List<CAddress> GetAddressesByPersonID(string strPersonId)
    {
      try
      {
        List<CAddress> addresses = new List<CAddress>();

        foreach (TAdress taddress in db.TAdresses.Where(x => x.StrPersonId == strPersonId).OrderBy(x => x.NOrderNr))
        {
          CAddress address = new CAddress();
          address.AddressId = taddress.NAdressId;
          address.PersonId = Convert.ToString(taddress.StrPersonId);
          address.HouseNr = Convert.ToString(taddress.StrHouseNr).Trim();
          address.Adresse = Convert.ToString(taddress.StrAdresse);
          address.Zip = Convert.ToString(taddress.StrZip);
          address.Town = Convert.ToString(taddress.StrTown);
          address.Country = Convert.ToString(taddress.StrCountry);
          address.CreateCreate = taddress.DtCreate;
          address.UpdateDate = taddress.DtUpdate;
          address.OrderNr = taddress.NOrderNr;
          address.Active = taddress.BActive;
          addresses.Add(address);
        }
        return addresses;

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Letzte OrderNr aller Adressen einer Person
    /// </summary>
    /// <param name="strPersonId"></param>
    /// <returns></returns>
    private int GetLastAddressOrderNumber(string strPersonId)
    {
      try
      {
        CAddress address = new CAddress();
        foreach (TAdress taddress in db.TAdresses.Where(x => x.StrPersonId == strPersonId).OrderByDescending(x => x.NOrderNr))
        {
          address.AddressId = taddress.NAdressId;
          address.PersonId = Convert.ToString(taddress.StrPersonId);
          address.HouseNr = Convert.ToString(taddress.StrHouseNr);
          address.Adresse = Convert.ToString(taddress.StrAdresse);
          address.Zip = Convert.ToString(taddress.StrZip);
          address.Town = Convert.ToString(taddress.StrTown);
          address.Country = Convert.ToString(taddress.StrCountry);
          address.CreateCreate = taddress.DtCreate;
          address.UpdateDate = taddress.DtUpdate;
          address.OrderNr = taddress.NOrderNr;
          address.Active = taddress.BActive;
          break;
        }
        return address.OrderNr.Value;

      }
      catch (Exception)
      {
        throw;
      }
    }

    public CAddress GetAddressLastByPersonId(string strPersonId)
    {
      try
      {
        CAddress address = new CAddress();
        TAdress taddress = db.TAdresses.LastOrDefault(x => x.StrPersonId == strPersonId);
        {
          address.AddressId = taddress.NAdressId;
          address.PersonId = Convert.ToString(taddress.StrPersonId);
          address.HouseNr = Convert.ToString(taddress.StrHouseNr);
          address.Adresse = Convert.ToString(taddress.StrAdresse);
          address.Zip = Convert.ToString(taddress.StrZip);
          address.Town = Convert.ToString(taddress.StrTown);
          address.Country = Convert.ToString(taddress.StrCountry);
          address.CreateCreate = taddress.DtCreate;
          address.UpdateDate = taddress.DtUpdate;
          address.OrderNr = taddress.NOrderNr;
          address.Active = taddress.BActive;
        }
        return address;

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Add new address and generate new order nr. 
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public CAddress AddAddress(CAddress address)
    {
      try
      {
        TAdress taddress = new TAdress();
        if (address != null)
        {
          taddress.StrPersonId = Convert.ToString(address.PersonId);
          taddress.StrHouseNr = Convert.ToString(address.HouseNr);
          taddress.StrAdresse = Convert.ToString(address.Adresse);
          taddress.StrZip = Convert.ToString(address.Zip);
          taddress.StrTown = Convert.ToString(address.Town);
          taddress.StrCountry = Convert.ToString(address.Country);
          taddress.DtCreate = DateTime.Now;
          taddress.DtUpdate = DateTime.Now;
          taddress.NOrderNr = GetLastAddressOrderNumber(address.PersonId) + 1;
          taddress.BActive = address.Active;

          db.TAdresses.Attach(taddress);
          db.Entry(taddress).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          var newAddress = GetAddressLastByPersonId(address.PersonId);

          return GetAddressByID(newAddress.AddressId);
        }
        return null;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Update modified address
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public CAddress UpdateAddress(CAddress address)
    {
      try
      {
        TAdress taddress = db.TAdresses.FirstOrDefault(x => x.NAdressId == address.AddressId);
        {
          taddress.StrPersonId = Convert.ToString(address.PersonId);
          taddress.StrHouseNr = Convert.ToString(address.HouseNr);
          taddress.StrAdresse = Convert.ToString(address.Adresse);
          taddress.StrZip = Convert.ToString(address.Zip);
          taddress.StrTown = Convert.ToString(address.Town);
          taddress.StrCountry = Convert.ToString(address.Country);
          taddress.DtUpdate = DateTime.Now;
          taddress.NOrderNr = address.OrderNr;
          taddress.BActive = address.Active;

          db.TAdresses.Attach(taddress);
          db.Entry(taddress).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          int returnValue = db.SaveChanges();
          var id = taddress.NAdressId;

          return GetAddressByID(id);
        }
      }
      catch (Exception)
      {
        throw;
      }

    }

    /// <summary>
    /// Delete address
    /// </summary>
    /// <param name="address"></param>
    public void DeleteAddress(CAddress address)
    {
      try
      {
        TAdress taddress = db.TAdresses.FirstOrDefault(x => x.NAdressId == address.AddressId);
        if (taddress != null)
        {
          db.TAdresses.Attach(taddress);
          db.Entry(taddress).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Die Settings um die Bilder anzuzeigen.
    /// </summary>
    /// <param name="i_oPerson"></param>
    /// <param name="i_tPerson"></param>
    /// <param name="i_oSettings"></param>
    private void SetImagePath(CPerson i_oPerson, TPerson i_tPerson, CSettings i_oSettings)
    {
      try
      {
        i_oPerson.ImageName = i_oPerson.PersonID.Replace("I", "") + "_" + i_oPerson.FirstName + "_" + i_oPerson.FamilyName + "_PE_01.jpg";
        string strImagePathSmall = "/48x60/" + i_oPerson.ImageName;
        string strImageFileNormal = "/80x100/" + i_oPerson.ImageName;
        string strImagePathBig = "/160x200/" + i_oPerson.ImageName;

        string strPhysicalPathImagePathSmall = strImagePathSmall.Replace("/", "\\");
        string strPhysicalPathImageFileNormal = strImageFileNormal.Replace("/", "\\");
        string strPhysicalPathImagePathBig = strImagePathBig.Replace("/", "\\");

        // Bilder nicht aus dem Cache laden
        i_oPerson.ImagePathSmall = i_oSettings.UrlImagePath + strImagePathSmall + "?" + DateTime.Now;
        i_oPerson.ImagePath = i_oSettings.UrlImagePath + strImageFileNormal + "?" + DateTime.Now;
        i_oPerson.ImagePathBig = i_oSettings.UrlImagePath + strImagePathBig + "?" + DateTime.Now;

        if (!System.IO.File.Exists(i_oSettings.PhysicalImagePath + strPhysicalPathImagePathSmall))
        {
          i_oPerson.ImagePathSmall = i_oSettings.UrlImagePath + "/48x60/Empty.jpg";
        }
        if (!System.IO.File.Exists(i_oSettings.PhysicalImagePath + strPhysicalPathImageFileNormal))
        {
          i_oPerson.ImagePath = i_oSettings.UrlImagePath + "/80x100/Empty.jpg";
        }

        if (!System.IO.File.Exists(i_oSettings.PhysicalImagePath + strPhysicalPathImagePathBig))
        {
          i_oPerson.ImagePathBig = i_oSettings.UrlImagePath + "/160x200/Empty.jpg";
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Geschlecht als Wort Mann\Frau
    /// </summary>
    /// <param name="i_oPerson"></param>
    /// <returns></returns>
    public string SexDisplay(CPerson i_oPerson)
    {

      string sex = "";
      if (i_oPerson.Sex == "M")
      {
        sex = "Mann";
      }
      else if (i_oPerson.Sex == "F")
      {
        sex = "Frau";
      }
      return sex; ;

    }


    /// <summary>
    /// Person und Kinder speichen, Bildnamen umbenennen
    /// </summary>
    /// <param name="Person"></param>
    /// <param name="oSettings"></param>
    public CPerson UpdatePerson(CPerson Person, CSettings oSettings)
    {
      try
      {
        //CPerson oPerson = new CPerson();
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == Person.PersonID);
        if (tperson != null)
        {
          MappPersonModelToEntity(ref tperson, Person, oSettings);

          db.TPersons.Attach(tperson);
          db.Entry(tperson).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          int returnValue = db.SaveChanges();

          ////CreateOrUpdateChildren(Person);

          ChangeImagePersonNameByChangedName(Person, oSettings);

          var id = tperson.NPersonId;

          return GetPersonByID(tperson.StrPersonId, oSettings);

        }
        throw new Exception("Person konnte nicht zugefügt werden");
      }
      catch (Exception)
      {
        throw;
      }

    }

    /// <summary>
    /// Weitere Informationen zur Person
    /// </summary>
    /// <param name="Remark"></param>
    /// <returns></returns>
    public CRemark UpdateRemark(CRemark Remark)
    {
      try
      {
        TRemark tremark = db.TRemarks.FirstOrDefault(t => t.StrPersonId == Remark.PersonID);
        if (tremark != null)
        {
          tremark.StrRemarks = Remark.Remarks;
          tremark.StrRemarksClean = Remark.RemarksClean;
          db.TRemarks.Attach(tremark);
          db.Entry(tremark).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          int returnValue = db.SaveChanges();

          // var id = tremark.NRemarkId;

          return GetRemarkByPersonID(Remark.PersonID);

        }
        else
        {
          return this.AddRemark(Remark);
        }
        throw new Exception("Remark konnte nicht geändert werden");
      }
      catch (Exception)
      {
        throw;
      }

    }

    /// <summary>
    /// Weitere Informationen zur Person
    /// </summary>
    /// <param name="Remark"></param>
    /// <returns></returns>
    public CRemark AddRemark(CRemark Remark)
    {
      try
      {
        TRemark tremark = new TRemark();
        if (Remark != null)
        {
          tremark.StrPersonId = Remark.PersonID;
          tremark.StrRemarks = Remark.Remarks;
          tremark.StrRemarksClean = Remark.RemarksClean;
          tremark.BActiv = true;

          db.TRemarks.Attach(tremark);
          db.Entry(tremark).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          int returnValue = db.SaveChanges();
          var id = tremark.StrPersonId;

          return GetRemarkByPersonID(id);
        }
        throw new Exception("Remark konnte nicht zugefügt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    #region CONTENT TEMPLATES
    /// <summary>
    /// Eine Liste aller Templates mit deren Relationen
    /// </summary>
    /// <returns></returns>
    public List<CContentTemplate> GetContendTemplates()
    {
      try
      {

        List<CContentTemplate> templates = new List<CContentTemplate>();
        foreach (TContentTemplate tableTemplate in db.TContentTemplates)
        {
          CContentTemplate template = new CContentTemplate();
          MappContentTemplateEntityToModel(ref template, tableTemplate);
          template.ContentTemplateLinks = new List<CContentTemplateLink>();
          foreach (TContentTemplateLink tableLink in db.TContentTemplateLinks.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
          {
            CContentTemplateLink link = new CContentTemplateLink();
            MappContentTemplateLinkEntityToModel(ref link, tableLink);
            template.ContentTemplateLinks.Add(link);
          }

          template.ContentTemplateImages = new List<CContentTemplateImage>();
          foreach (TContentTemplateImage tableImage in db.TContentTemplateImages.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
          {
            CContentTemplateImage image = new CContentTemplateImage();
            MappContentTemplateImageEntityToModel(ref image, tableImage);
            template.ContentTemplateImages.Add(image);
          }
          templates.Add(template);
        }

        return templates;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public List<CContentTemplate> GetContendTemplatesByType(int type)
    {
      try
      {

        List<CContentTemplate> templates = new List<CContentTemplate>();
        foreach (TContentTemplate tableTemplate in db.TContentTemplates.Where(x => x.NType == type))
        {
          CContentTemplate template = new CContentTemplate();
          MappContentTemplateEntityToModel(ref template, tableTemplate);
          template.ContentTemplateLinks = new List<CContentTemplateLink>();
          foreach (TContentTemplateLink tableLink in db.TContentTemplateLinks.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
          {
            CContentTemplateLink link = new CContentTemplateLink();
            MappContentTemplateLinkEntityToModel(ref link, tableLink);
            template.ContentTemplateLinks.Add(link);
          }

          template.ContentTemplateImages = new List<CContentTemplateImage>();
          foreach (TContentTemplateImage tableImage in db.TContentTemplateImages.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
          {
            CContentTemplateImage image = new CContentTemplateImage();
            MappContentTemplateImageEntityToModel(ref image, tableImage);
            template.ContentTemplateImages.Add(image);
          }
          templates.Add(template);
        }

        return templates;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Template mit allen Relationen
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CContentTemplate GetContendTemplatesById(int id)
    {
      try
      {

        CContentTemplate template = new CContentTemplate();
        foreach (TContentTemplate tableTemplate in db.TContentTemplates.Where(x => x.NContentTemplateId == id))
        {
          MappContentTemplateEntityToModel(ref template, tableTemplate);
          template.ContentTemplateLinks = new List<CContentTemplateLink>();
          foreach (TContentTemplateLink tableLink in db.TContentTemplateLinks.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
          {
            CContentTemplateLink link = new CContentTemplateLink();
            MappContentTemplateLinkEntityToModel(ref link, tableLink);
            template.ContentTemplateLinks.Add(link);
          }

          template.ContentTemplateImages = new List<CContentTemplateImage>();
          foreach (TContentTemplateImage tableImage in db.TContentTemplateImages.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
          {
            CContentTemplateImage image = new CContentTemplateImage();
            MappContentTemplateImageEntityToModel(ref image, tableImage);
            template.ContentTemplateImages.Add(image);
          }
        }

        return template;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public List<CContentTemplateImage> GetContendTemplateImagesByType(CContentTemplate.ETemplateTypes type, bool onlyActice)
    {
      try
      {
        List<CContentTemplateImage> arlImages = new List<CContentTemplateImage>();

        if (onlyActice)
        {
          TContentTemplate tableTemplate = db.TContentTemplates.FirstOrDefault(x => x.NType == (int)type && x.NActive == CGlobal.DBTRUE);
          if (tableTemplate != null)
          {
            foreach (TContentTemplateImage tableImage in db.TContentTemplateImages.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId && x.NActive == CGlobal.DBTRUE))
            {
              CContentTemplateImage image = new CContentTemplateImage();
              MappContentTemplateImageEntityToModel(ref image, tableImage);
              arlImages.Add(image);
            }
          }
        }
        else
        {
          TContentTemplate tableTemplate = db.TContentTemplates.FirstOrDefault(x => x.NType == (int)type);
          if (tableTemplate != null)
          {
            foreach (TContentTemplateImage tableImage in db.TContentTemplateImages.Where(x => x.NContentTemplateId == tableTemplate.NContentTemplateId))
            {
              CContentTemplateImage image = new CContentTemplateImage();
              MappContentTemplateImageEntityToModel(ref image, tableImage);
              arlImages.Add(image);
            }
          }
        }


        return arlImages;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein content link auslesen
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CContentTemplateLink GetContendTemplateLinkById(int id)
    {
      try
      {
        CContentTemplateLink model = new CContentTemplateLink();
        TContentTemplateLink table = db.TContentTemplateLinks.FirstOrDefault(x => x.NContentTemplateLinkId == id);
        MappContentTemplateLinkEntityToModel(ref model, table);

        return model;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein content image auslesen
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CContentTemplateImage GetContendTemplateImageById(int id)
    {
      try
      {
        CContentTemplateImage model = new CContentTemplateImage();
        TContentTemplateImage table = db.TContentTemplateImages.FirstOrDefault(x => x.NContentTemplateImageId == id);
        MappContentTemplateImageEntityToModel(ref model, table);

        return model;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Content template speichern
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public CContentTemplate UpdateContentTemplate(CContentTemplate model)
    {
      try
      {
        TContentTemplate table = db.TContentTemplates.FirstOrDefault(t => t.NContentTemplateId == model.ContentTemplateId);
        if (table != null)
        {
          MappContentTemplateModelEntity(ref table, model);

          db.TContentTemplates.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
          return GetContendTemplatesById(model.ContentTemplateId);
        }
        throw new Exception("UpdateContentTemplate konnte nicht ausgeführt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Content template link speichern
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public CContentTemplateLink UpdateContentTemplateLink(CContentTemplateLink model)
    {
      try
      {
        TContentTemplateLink table = db.TContentTemplateLinks.FirstOrDefault(t => t.NContentTemplateLinkId == model.ContentTemplateLinkId);
        if (table != null)
        {
          MappContentTemplateLinkModelEntity(ref table, model);

          db.TContentTemplateLinks.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
          return GetContendTemplateLinkById(model.ContentTemplateLinkId);
        }
        throw new Exception("UpdateContentTemplateLink konnte nicht ausgeführt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Contetn template image speichern
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public CContentTemplateImage UpdateContentTemplateImage(CContentTemplateImage model)
    {
      try
      {
        TContentTemplateImage table = db.TContentTemplateImages.FirstOrDefault(t => t.NContentTemplateImageId == model.ContentTemplateImageId);
        if (table != null)
        {
          MappContentTemplateImageModelEntity(ref table, model);

          db.TContentTemplateImages.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
          return GetContendTemplateImageById(model.ContentTemplateImageId);
        }
        throw new Exception("UpdateContentTemplateImage konnte nicht ausgeführt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Add content template
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public CContentTemplate AddContentTemplate(CContentTemplate model)
    {
      try
      {
        TContentTemplate table = new TContentTemplate();
        if (table != null)
        {
          MappContentTemplateModelEntity(ref table, model);

          db.TContentTemplates.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          var id = table.NContentTemplateId;
          return GetContendTemplatesById(id);
        }
        throw new Exception("AddContentTemplate konnte nicht abgeschlossen werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Add content template link
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public CContentTemplateLink AddContentTemplateLink(CContentTemplateLink model)
    {
      try
      {
        TContentTemplateLink table = new TContentTemplateLink();
        if (table != null)
        {
          MappContentTemplateLinkModelEntity(ref table, model);

          db.TContentTemplateLinks.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          var id = table.NContentTemplateLinkId;
          return GetContendTemplateLinkById(id);
        }
        throw new Exception("AddContentTemplateLink konnte nicht abgeschlossen werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Add content template image
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public CContentTemplateImage AddContentTemplateImage(CContentTemplateImage model)
    {
      try
      {
        TContentTemplateImage table = new TContentTemplateImage();
        if (table != null)
        {
          MappContentTemplateImageModelEntity(ref table, model);

          db.TContentTemplateImages.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          var id = table.NContentTemplateImageId;
          return GetContendTemplateImageById(id);
        }
        throw new Exception("AddContentTemplateImage konnte nicht abgeschlossen werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Delete content template
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool DeleteContentTemplate(CContentTemplate model)
    {
      bool isDeleted = false;
      try
      {
        CContentTemplate template = GetContendTemplatesById(model.ContentTemplateId);

        foreach (CContentTemplateLink link in template.ContentTemplateLinks)
        {
          DeleteContentTemplateLink(link);
        }
        foreach (CContentTemplateImage image in template.ContentTemplateImages)
        {
          DeleteContentTemplateImage(image);
        }
        TContentTemplate table = db.TContentTemplates.FirstOrDefault(t => t.NContentTemplateId == model.ContentTemplateId);
        if (table != null)
        {
          db.TContentTemplates.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        isDeleted = true;
      }
      catch (Exception)
      {
        return isDeleted;
      }
      return isDeleted;
    }

    /// <summary>
    /// Delete content template link
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool DeleteContentTemplateLink(CContentTemplateLink model)
    {
      bool isDeleted = false;
      try
      {
        TContentTemplateLink table = db.TContentTemplateLinks.FirstOrDefault(t => t.NContentTemplateLinkId == model.ContentTemplateLinkId);
        if (table != null)
        {
          // TODOD: Delete PDF
          db.TContentTemplateLinks.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        isDeleted = true;
      }
      catch (Exception)
      {
        return isDeleted;
      }
      return isDeleted;
    }

    /// <summary>
    /// Delete content template image
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public bool DeleteContentTemplateImage(CContentTemplateImage model)
    {
      bool isDeleted = false;
      try
      {
        TContentTemplateImage table = db.TContentTemplateImages.FirstOrDefault(t => t.NContentTemplateImageId == model.ContentTemplateImageId);
        if (table != null)
        {
          // TODO: Delete Image
          db.TContentTemplateImages.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        isDeleted = true;
      }
      catch (Exception)
      {
        return isDeleted;
      }
      return isDeleted;
    }

    #endregion

    #region VERWNDTSCHAFTSVERBUNDUNGEN

    public CKinshipConnection GetKinshipConnectionById(int id)
    {
      try
      {
        CKinshipConnection model = new CKinshipConnection();
        TKinshipConnection table = db.TKinshipConnections.FirstOrDefault(x => x.NKinshipConnectionId == id);
        MappKinshipConnectionEntityToModel(ref model, table);

        return model;
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CKinshipConnection AddKinshipConnection(CKinshipConnection model)
    {
      try
      {
        TKinshipConnection table = new TKinshipConnection();
        if (table != null)
        {
          MappKinshipConnectionModelEntity(ref table, model);

          db.TKinshipConnections.Attach(table);
          db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          var id = table.NKinshipConnectionId;
          return GetKinshipConnectionById(id);
        }
        throw new Exception("AddContentTemplate konnte nicht abgeschlossen werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    //public void CallAddKinshipConnection(CKinshipConnection model)
    //{
    //  try
    //  {
    //    TKinshipConnections table = new TKinshipConnections();
    //    if (table != null)
    //    {
    //      MappKinshipConnectionModelEntity(ref table, model);

    //      db.TKinshipConnections.Attach(table);
    //      db.Entry(table).State = Microsoft.EntityFrameworkCore.EntityState.Added;
    //      //db.SaveChanges();
    //    }

    //  }
    //  catch (Exception ex)
    //  {
    //    throw ex;
    //  }
    //}

    public void UpdateAddedKinshipConnection(List<CKinshipConnection> model)
    {
      try
      {
        List<TKinshipConnection> table = new List<TKinshipConnection>();
        foreach (CKinshipConnection _model in model)
        {
          TKinshipConnection _table = new TKinshipConnection();
          MappKinshipConnectionModelEntity(ref _table, _model);
          table.Add(_table);
        }
        db.TKinshipConnections.AddRange(table);
        db.SaveChanges();
      }
      catch (Exception)
      {
        throw;
      }
    }

    #endregion

    /// <summary>
    /// Update Nachruf zur Person
    /// </summary>
    /// <param name="Obituary"></param>
    /// <returns></returns>
    public CObituary UpdateObituary(CObituary Obituary)
    {
      try
      {
        TObituary tobituary = db.TObituaries.FirstOrDefault(t => t.StrPersonId == Obituary.PersonID);
        if (tobituary != null)
        {
          tobituary.StrObituary = Obituary.Obituary;
          db.TObituaries.Attach(tobituary);
          db.Entry(tobituary).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          int returnValue = db.SaveChanges();

          var id = tobituary.NObituaryId;

          return GetObituaryByPersonID(Obituary.PersonID);

        }
        throw new Exception("Nachruf konnte nicht geändert werden");
      }
      catch (Exception)
      {
        throw;
      }

    }

    public CPersonPortrait UpdatePersonPortrait(CPersonPortrait PersonPortrait)
    {
      try
      {
        TPersonPortrait tpersonPortrait = db.TPersonPortraits.FirstOrDefault(t => t.NPersonPortraitId == PersonPortrait.PersonPortraitID);
        if (PersonPortrait != null)
        {
          MappPersonPortraitModelEntity(ref tpersonPortrait, PersonPortrait);

          db.TPersonPortraits.Attach(tpersonPortrait);
          db.Entry(tpersonPortrait).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
          return GetPersonPortraitByID(PersonPortrait.PersonPortraitID);
        }
        throw new Exception("Remark konnte nicht zugefügt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }


    public CPersonPortrait AddPersonPortrait(CPersonPortrait PersonPortrait)
    {
      try
      {
        TPersonPortrait tpersonPortrait = new TPersonPortrait();
        if (PersonPortrait != null)
        {
          MappPersonPortraitModelEntity(ref tpersonPortrait, PersonPortrait);

          db.TPersonPortraits.Attach(tpersonPortrait);
          db.Entry(tpersonPortrait).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          var id = tpersonPortrait.NPersonPortraitId;
          return GetPersonPortraitByID(id);
        }
        throw new Exception("Remark konnte nicht zugefügt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    public bool DeletePersonPortrait(CPersonPortrait PersonPortrait)
    {
      bool isDeleted = false;
      try
      {
        TPersonPortrait tpersonPortrait = db.TPersonPortraits.FirstOrDefault(t => t.StrPersonId == PersonPortrait.PersonID);
        if (tpersonPortrait != null)
        {
          // TODOD: Delete PDF
          db.TPersonPortraits.Attach(tpersonPortrait);
          db.Entry(tpersonPortrait).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        isDeleted = true;
      }
      catch (Exception)
      {
        return isDeleted;
      }
      return isDeleted;
    }

    /// <summary>
    /// Nachruf zur Person
    /// </summary>
    /// <param name="Obituary"></param>
    /// <returns></returns>
    public CObituary AddObituary(CObituary Obituary)
    {
      try
      {
        TObituary tobituary = new TObituary();
        if (Obituary != null)
        {
          tobituary.StrPersonId = Obituary.PersonID;
          tobituary.StrObituary = Obituary.Obituary;
          tobituary.BActiv = true;

          db.TObituaries.Attach(tobituary);
          db.Entry(tobituary).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          int returnValue = db.SaveChanges();
          var id = tobituary.StrPersonId;

          return GetObituaryByPersonID(id);
        }
        throw new Exception("Nachruf konnte nicht zugefügt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }


    /// <summary>
    /// Eine neu angelegte Person zufügen
    /// </summary>
    /// <param name="Person"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public int InsertPerson(CPerson Person, CSettings oSettings)
    {
      try
      {
        int max = db.TPersons.Max(t => t.NPersonId) + 1;
        Person.PersonID = Convert.ToString("I" + max);
        TPerson tperson = new TPerson();
        if (Person != null)
        {
          MappPersonModelToEntity(ref tperson, Person, oSettings);


          db.TPersons.Attach(tperson);
          db.Entry(tperson).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          db.SaveChanges();

          // CreateOrUpdateChildren(Person);
        }
        return max;
      }
      catch (Exception)
      {
        return -1;
      }
    }

    /// <summary>
    /// Neue Person zufügen
    /// </summary>
    /// <param name="Person"></param>
    /// <param name="oSettings"></param>
    /// <returns></returns>
    public CPerson AddPerson(CPerson Person, CSettings oSettings)
    {
      try
      {

        int max = db.TPersons.Max(t => t.NPersonId) + 1;
        Person.PersonID = Convert.ToString("I" + max);
        TPerson tperson = new TPerson();
        if (Person != null)
        {
          tperson.StrPersonId = Person.PersonID;
          MappPersonModelToEntity(ref tperson, Person, oSettings);


          db.TPersons.Attach(tperson);
          db.Entry(tperson).State = Microsoft.EntityFrameworkCore.EntityState.Added;
          int returnValue = db.SaveChanges();
          var id = tperson.NPersonId;

          return GetPersonByID(tperson.NPersonId, oSettings);

          // CreateOrUpdateChildren(Person);
        }
        throw new Exception("Person konnte nicht zugefügt werden");
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein leeres Personen Objekt erzeugen
    /// </summary>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public CPerson CreateEmptyPerson()
    {
      CPerson oPerson = new CPerson();
      oPerson.tikBirth = 0;
      oPerson.tikDeath = 0;
      oPerson.tikBur = 0;

      oPerson.ID = -1;
      oPerson.PersonID = "-1";
      oPerson.FirstName = "";
      oPerson.FamilyName = "";
      oPerson.Fullname = "";
      oPerson.Bur = "";
      oPerson.Sex = "M";
      oPerson.FatherID = "-1";
      oPerson.MotherID = "-1";

      oPerson.HasParents = false; // ExistParents(oPerson) == CGlobal.DBTRUE ? true : false; // 0; // Sind die Eltern bekannt?
      oPerson.HasSpouse = false; // ExistSibling(oPerson) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister

      oPerson.BirthPlace = ""; // Convert.ToString(tperson.strBi);
      oPerson.DeathPlace = ""; // Convert.ToString(tperson.strBurPlace);

      oPerson.BurPlace = ""; // Convert.ToString(tperson.strBurPlace);
      oPerson.Race = ""; // Convert.ToString(tperson.strRace);
      oPerson.Work = ""; // Convert.ToString(tperson.strWork);

      oPerson.NameMerges = ""; // Convert.ToString(tperson.strMarriedName);
      oPerson.Nickname = ""; // Convert.ToString(tperson.strNick);

      oPerson.Active = true; // tperson.nActive == CGlobal.DBTRUE ? true : false;
      oPerson.Address = "";//  Convert.ToString(tperson.strAdress);

      oPerson.Older = 0; // CGlobal.CalcualteOlder(oPerson.tikBirth, oPerson.tikDeath, oPerson.IsDeath);

      oPerson.BirthDate = CGlobal.CliensSideEmptyDate();
      oPerson.DeathDate = CGlobal.CliensSideEmptyDate();
      oPerson.BurDate = CGlobal.CliensSideEmptyDate();

      oPerson.BirthDisplay = oPerson.BirthDate.ToShortDateString();
      oPerson.DeathDisplay = oPerson.DeathDate.ToShortDateString();
      oPerson.BurDisplay = oPerson.BurDate.ToShortDateString();

      oPerson.IsDeath = false; // tperson.nIsLiving == CGlobal.DBFALSE ? true : false;
      oPerson.SexDisplay = SexDisplay(oPerson);

      //SetImagePath(oPerson, tperson, oSettings);
      return oPerson;
    }

    /// <summary>
    /// Eine Person mit allen Relationen löschen. 
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public bool DeletePerson(CPerson person)
    {

      bool isDeleted = false;

      try
      {

        TChildren tchildren = db.TChildrens.FirstOrDefault(t => t.StrPersonId == person.PersonID);
        if (tchildren != null)
        {
          db.TChildrens.Attach(tchildren);
          db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == person.PersonID);
        if (tperson != null)
        {
          db.TPersons.Attach(tperson);
          db.Entry(tperson).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        TPartner tpartner = db.TPartners.FirstOrDefault(t => t.StrPersonId == person.PersonID);
        if (tpartner != null)
        {
          db.TPartners.Attach(tpartner);
          db.Entry(tpartner).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        TRemark tremark = db.TRemarks.FirstOrDefault(t => t.StrPersonId == person.PersonID);
        if (tremark != null)
        {
          db.TRemarks.Attach(tremark);
          db.Entry(tremark).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }
        TAdress taddress = db.TAdresses.FirstOrDefault(t => t.StrPersonId == person.PersonID);
        if (taddress != null)
        {
          db.TAdresses.Attach(taddress);
          db.Entry(taddress).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }


        isDeleted = true;
      }

      catch (Exception)
      {
        return isDeleted;
      }
      return isDeleted;
    }

    /// <summary>
    /// Update Partner (nur Datum Heirat u. Geshieden)
    /// </summary>
    /// <param name="partner"></param>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public void UpdatePartnerDate(CPartner partner)
    {
      try
      {

        foreach (TPartner tpartner in db.TPartners.Where(t => t.StrPartnerId == partner.PartnerID && t.StrPersonId == partner.Person.PersonID || t.StrPartnerId == partner.Person.PersonID && t.StrPersonId == partner.PartnerID))
        {
          // Geheiratet am
          if (partner.MarriageDateTime != CGlobal.CliensSideEmptyDate())
          {
            tpartner.TikMarriageDate = Convert.ToDateTime(partner.MarriageDateTime).Ticks.ToString();
            tpartner.DtMarriageDate = partner.MarriageDateTime;
          }
          else
          {
            tpartner.TikMarriageDate = Convert.ToString(0);
            tpartner.DtMarriageDate = CGlobal.CliensSideEmptyDate();
          }

          // Geschieden am
          if (partner.DivorceDateTime != CGlobal.CliensSideEmptyDate())
          {
            tpartner.TikDivorceDate = Convert.ToDateTime(partner.DivorceDateTime).Ticks.ToString();
            tpartner.DtDivorceDate = partner.DivorceDateTime;
          }
          else
          {
            tpartner.TikDivorceDate = Convert.ToString(0);
            tpartner.DtDivorceDate = CGlobal.CliensSideEmptyDate();
          }

          //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
          //tpartner.nPersonID = partner.ID; // nPersonID

          tpartner.StrFullName = ""; // partner.m_strFullname;
          tpartner.StrCurrentFullName = ""; // partner.m_strFullname;

          db.TPartners.Attach(tpartner);
          db.Entry(tpartner).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        }
        db.SaveChanges();
      }

      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Add partner to person
    /// </summary>
    /// <param name="partner"></param>
    public CPartner AddPartner(CPartner partner, CSettings i_oSettings)
    {
      try
      {
        //int returnValuePerson = -1;
        //int returnValuePartner = -1;
        TPartner tpartnerPerson = new TPartner();
        TPartner tpartnerPartner = new TPartner();
        TPerson tperson = new TPerson();
        TPerson tpersonPartner = new TPerson();

        using (var transaction = db.Database.BeginTransaction())
        {
          try
          {
            //                                           Walter I5008                         Rita I5656
            if (!db.TPartners.Any(t => t.StrPartnerId == partner.PartnerID && t.StrPersonId == partner.PersonID &&
                                    t.StrPartnerId == partner.PersonID && t.StrPersonId == partner.PartnerID))
            {
              tperson = db.TPersons.FirstOrDefault(x => x.StrPersonId == partner.PersonID);
              tpersonPartner = db.TPersons.FirstOrDefault(x => x.StrPersonId == partner.PartnerID);
              if (tperson != null && tpersonPartner != null)
              {

                tpartnerPerson.NPersonId = tperson.NPersonId;
                tpartnerPerson.StrPersonId = Convert.ToString(partner.PersonID);
                tpartnerPerson.StrPartnerId = Convert.ToString(partner.PartnerID);

                tpartnerPartner.NPersonId = tpersonPartner.NPersonId;
                tpartnerPartner.StrPersonId = Convert.ToString(partner.PartnerID);
                tpartnerPartner.StrPartnerId = Convert.ToString(partner.PersonID);

                // Geheiratet am
                //if (partner.MarriageDateTime != null && partner.MarriageDateTime != CGlobal.CliensSideEmptyDate())
                //{
                //  tpartnerPerson.TikMarriageDate = Convert.ToDateTime(partner.MarriageDateTime).Ticks.ToString();
                //  tpartnerPerson.DtMarriageDate = partner.MarriageDateTime;
                //  tpartnerPartner.TikMarriageDate = Convert.ToDateTime(partner.MarriageDateTime).Ticks.ToString();
                //  tpartnerPartner.DtMarriageDate = partner.MarriageDateTime;
                //}
                //else
                //{
                tpartnerPerson.TikMarriageDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPerson.DtMarriageDate = CGlobal.CliensSideEmptyDate();
                tpartnerPartner.TikMarriageDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPartner.DtMarriageDate = CGlobal.CliensSideEmptyDate();
                //}

                //// Geschieden am
                //if (partner.DivorceDateTime != null && partner.DivorceDateTime != CGlobal.CliensSideEmptyDate())
                //{
                //  tpartnerPerson.TikDivorceDate = Convert.ToDateTime(partner.DivorceDateTime).Ticks.ToString();
                //  tpartnerPerson.DtDivorceDate = partner.DivorceDateTime;
                //  tpartnerPartner.TikDivorceDate = Convert.ToDateTime(partner.DivorceDateTime).Ticks.ToString();
                //  tpartnerPartner.DtDivorceDate = partner.DivorceDateTime;
                //}
                //else
                //{
                tpartnerPerson.TikDivorceDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPerson.DtDivorceDate = CGlobal.CliensSideEmptyDate();
                tpartnerPartner.TikDivorceDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPartner.DtDivorceDate = CGlobal.CliensSideEmptyDate();
                //}

                //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
                //tpartner.nPersonID = partner.ID; // nPersonID

                tpartnerPerson.StrFullName = ""; // partner.m_strFullname;
                tpartnerPerson.StrCurrentFullName = ""; // partner.m_strFullname;
                tpartnerPartner.StrFullName = ""; // partner.m_strFullname;
                tpartnerPartner.StrCurrentFullName = ""; // partner.m_strFullname;

                tpartnerPerson.NCurrent = 0;
                tpartnerPartner.NCurrent = 0;

                db.TPartners.Attach(tpartnerPerson);
                db.Entry(tpartnerPerson).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                //returnValuePerson = db.SaveChanges(false);

                // throw new Exception("Error by add partner");

                db.TPartners.Attach(tpartnerPartner);
                db.Entry(tpartnerPartner).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                //returnValuePartner = db.SaveChanges(false);

                db.SaveChanges();

                transaction.Commit();

                return GetPartnerByID(partner.PartnerID, i_oSettings);
              }
            }
            //int returnValue = db.SaveChanges();

            throw new Exception("Partner konnte nicht zugefügt werden");

          }
          catch (Exception)
          {
            transaction.Rollback();
            // TODO: Handle failure
            throw;
          }
          //throw new Exception("Error by add partner");
        }
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Update Partner
    /// </summary>
    /// <param name="partner"></param>
    /// <param name="i_oSettings"></param>
    /// <returns></returns>
    public CPartner UpdatePartner(CPartner partner, CSettings i_oSettings)
    {
      try
      {

        TPartner tpartnerPerson = new TPartner();
        TPartner tpartnerPartner = new TPartner();
        TPerson tperson = new TPerson();
        TPerson tpersonPartner = new TPerson();

        using (TransactionScope scope = new TransactionScope())
        {
          // Gibt es den Partner
          if (db.TPartners.Any(t => t.StrPartnerId == partner.PartnerID && t.StrPersonId == partner.PersonID &&
                                   t.StrPartnerId == partner.PersonID && t.StrPersonId == partner.PartnerID))
          {
            tperson = db.TPersons.FirstOrDefault(x => x.StrPersonId == partner.Person.PersonID);
            tpersonPartner = db.TPersons.FirstOrDefault(x => x.StrPersonId == partner.PartnerID);

            if (tperson != null && tpersonPartner != null)
            {

              tpartnerPerson.NPersonId = tperson.NPersonId;
              tpartnerPerson.StrPersonId = Convert.ToString(partner.Person.PersonID);
              tpartnerPerson.StrPartnerId = Convert.ToString(partner.PartnerID);

              tpartnerPartner.NPersonId = tpersonPartner.NPersonId;
              tpartnerPartner.StrPersonId = Convert.ToString(partner.PartnerID);
              tpartnerPartner.StrPartnerId = Convert.ToString(partner.Person.PersonID);

              // Geheiratet am
              if (partner.MarriageDateTime != CGlobal.CliensSideEmptyDate())
              {
                tpartnerPerson.TikMarriageDate = Convert.ToDateTime(partner.MarriageDateTime).Ticks.ToString();
                tpartnerPerson.DtMarriageDate = partner.MarriageDateTime;
                tpartnerPartner.TikMarriageDate = Convert.ToDateTime(partner.MarriageDateTime).Ticks.ToString();
                tpartnerPartner.DtMarriageDate = partner.MarriageDateTime;
              }
              else
              {
                tpartnerPerson.TikMarriageDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPerson.DtMarriageDate = CGlobal.CliensSideEmptyDate();
                tpartnerPartner.TikMarriageDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPartner.DtMarriageDate = CGlobal.CliensSideEmptyDate();
              }

              // Geschieden am
              if (partner.DivorceDateTime != CGlobal.CliensSideEmptyDate())
              {
                tpartnerPerson.TikDivorceDate = Convert.ToDateTime(partner.DivorceDateTime).Ticks.ToString();
                tpartnerPerson.DtDivorceDate = partner.DivorceDateTime;
                tpartnerPartner.TikDivorceDate = Convert.ToDateTime(partner.DivorceDateTime).Ticks.ToString();
                tpartnerPartner.DtDivorceDate = partner.DivorceDateTime;
              }
              else
              {
                tpartnerPerson.TikDivorceDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPerson.DtDivorceDate = CGlobal.CliensSideEmptyDate();
                tpartnerPartner.TikDivorceDate = Convert.ToString(0); //Convert.ToDateTime(new DateTime(1900, 1, 1)).Ticks.ToString();
                tpartnerPartner.DtDivorceDate = CGlobal.CliensSideEmptyDate();
              }

              //tpartner.nCurrent = Convert.ToInt32(partner.IsCurrent);
              //tpartner.nPersonID = partner.ID; // nPersonID

              tpartnerPerson.StrFullName = ""; // partner.m_strFullname;
              tpartnerPerson.StrCurrentFullName = ""; // partner.m_strFullname;
              tpartnerPartner.StrFullName = ""; // partner.m_strFullname;
              tpartnerPartner.StrCurrentFullName = ""; // partner.m_strFullname;

              db.TPartners.Attach(tpartnerPerson);
              db.Entry(tpartnerPerson).State = Microsoft.EntityFrameworkCore.EntityState.Added;

              db.TPartners.Attach(tpartnerPartner);
              db.Entry(tpartnerPartner).State = Microsoft.EntityFrameworkCore.EntityState.Added;

            }

            db.SaveChanges();
            scope.Complete();

            return GetPartnerByID(partner.PartnerID, i_oSettings);
          }

          throw new Exception("Partner konnte nicht geändert werden");
        }
        //return GetPartnerByID("", i_oSettings);
      }
      catch (Exception)
      {
        throw;
      }
    }

    public CPartner UpdatePartnerLink(CPartner partner, CSettings i_oSettings)
    {
      try
      {
        CPartner returnPartner;
        //using (TransactionScope scope = new TransactionScope())
        //{

        foreach (TPartner p in db.TPartners.Where(
                                  t => t.StrPartnerId == partner.PartnerID && t.StrPersonId == partner.PersonID
                                  ||
                                  t.StrPartnerId == partner.PersonID && t.StrPersonId == partner.PartnerID
                                  ))
        {
          p.TikMarriageDate = Convert.ToDateTime(partner.MarriageDateTime).Ticks.ToString();
          p.DtMarriageDate = partner.MarriageDateTime;
          p.TikDivorceDate = Convert.ToDateTime(partner.DivorceDateTime).Ticks.ToString();
          p.DtDivorceDate = partner.DivorceDateTime;


        }
        db.SaveChanges();

        returnPartner = GetPartnerByID(partner.PartnerID, i_oSettings);
        //scope.Complete();

        return returnPartner;

        //}
        //return GetPartnerByID("-1", i_oSettings);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Remove link partner to person
    /// </summary>
    /// <param name="partner"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    public Boolean RemoveLinkPartnerFromPerson(CPartner partner)
    {
      try
      {
        using (var transaction = db.Database.BeginTransaction())
        {
          try
          {
            foreach (TPartner tpartner in db.TPartners.Where(t => (t.StrPartnerId == partner.PartnerID && t.StrPersonId == partner.PersonID)
                  || t.StrPartnerId == partner.PersonID && t.StrPersonId == partner.PartnerID))
            {
              db.TPartners.Attach(tpartner);
              db.Entry(tpartner).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            }
            // Kinder bleiben zugewiesen
            // RemoveLinkParentFromChildren(partner);
            db.SaveChanges();

            transaction.Commit();

            return true;
          }
          catch (Exception)
          {
            transaction.Rollback();
            throw;
          }
        }
      }
      catch (Exception)
      {
        throw;
      }
    }


    [ObsoleteAttribute("This property is obsolete.", true)]
    private void RemoveLinkParentFromChildren(CPartner partner)
    {
      try
      {
        if (partner.Person.Sex == "M")
        {
          foreach (TPerson tperson in db.TPersons.Where(t => (t.StrFatherId == partner.PartnerID && t.StrMotherId == partner.PersonID)))
          {
            tperson.StrMotherId = "-1";
            db.TPersons.Attach(tperson);
            db.Entry(tperson).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          }
        }
        else if (partner.Person.Sex == "F")
        {
          foreach (TPerson tperson in db.TPersons.Where(t => (t.StrMotherId == partner.PartnerID && t.StrFatherId == partner.PersonID)))
          {
            tperson.StrFatherId = "-1";
            db.TPersons.Attach(tperson);
            db.Entry(tperson).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          }
        }

        db.SaveChanges();

      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Ein Partner löschen. 
    /// </summary>
    /// <param name="partner"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public bool DeletePartner(CPartner partner)
    {

      bool isDeleted = false;

      try
      {

        TPartner tpartners = db.TPartners.FirstOrDefault(t => t.StrPartnerId == partner.PartnerID && t.StrPersonId == partner.Person.PersonID);
        if (tpartners != null)
        {
          db.TPartners.Attach(tpartners);
          db.Entry(tpartners).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }

        isDeleted = true;

      }

      catch (Exception)
      {

        return isDeleted;
      }

      return isDeleted;
    }

    /// <summary>
    /// Einen Partner einer Person zuweisen
    /// </summary>
    /// <param name="person"></param>
    /// <param name="partner"></param>
    /// <returns></returns>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    public bool AddPartnerToPerson(CPerson person, CPerson partner)
    {
      try
      {
        //     [ValidateAntiForgeryToken()]
        //  bool isDeleted = false;


        if (!db.TPartners.Any(t => t.StrPartnerId == partner.PersonID && t.StrPersonId == person.PersonID || t.StrPersonId == partner.PersonID))
        {
          TPartner tpartner = new TPartner();
          tpartner.NPersonId = person.ID;
          tpartner.StrPersonId = person.PersonID;
          tpartner.StrPartnerId = partner.PersonID;
          tpartner.TikMarriageDate = Convert.ToString(0);
          tpartner.DtMarriageDate = CGlobal.CliensSideEmptyDate();

          tpartner.TikDivorceDate = Convert.ToString(0);
          tpartner.DtDivorceDate = CGlobal.CliensSideEmptyDate(); ;

          tpartner.NCurrent = 0;// Convert.ToInt32(partner.IsCurrent);

          tpartner.StrFullName = ""; // partner.m_strFullname;
          tpartner.StrCurrentFullName = ""; // partner.m_strFullname;

          db.TPartners.Attach(tpartner);
          db.Entry(tpartner).State = Microsoft.EntityFrameworkCore.EntityState.Added;

          TPartner tpartner1 = new TPartner();
          tpartner1.NPersonId = partner.ID;
          tpartner1.StrPersonId = partner.PersonID;
          tpartner1.StrPartnerId = person.PersonID;
          tpartner1.TikMarriageDate = Convert.ToString(0);
          tpartner1.DtMarriageDate = CGlobal.CliensSideEmptyDate(); ;

          tpartner1.TikDivorceDate = Convert.ToString(0);
          tpartner1.DtDivorceDate = CGlobal.CliensSideEmptyDate(); ;

          tpartner1.NCurrent = 0;// Convert.ToInt32(partner.IsCurrent);

          tpartner1.StrFullName = ""; // partner.m_strFullname;
          tpartner1.StrCurrentFullName = ""; // partner.m_strFullname;

          db.TPartners.Attach(tpartner1);
          db.Entry(tpartner1).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        }

        db.SaveChanges();

        return true;
      }

      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Kinder zufuegen oder äendern
    /// </summary>
    /// <param name="person"></param>
    [ObsoleteAttribute("This property is obsolete. Use NewProperty instead.", true)]
    private void CreateOrUpdateChildren(CPerson person)
    {

      TChildren tchildren = db.TChildrens.FirstOrDefault(t => t.StrPersonId == person.PersonID);

      if (tchildren != null)
      {
        // Es sind keine Eltern zugewiesen
        if (person.FatherID == "" && person.MotherID == "")
        {
          db.TChildrens.Attach(tchildren);
          db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
          db.SaveChanges();
        }

        // Mutter hat sich geändert
        else if (person.FatherID == tchildren.StrFatherId && person.MotherID != tchildren.StrMotherId)
        {
          tchildren.StrMotherId = person.MotherID;
          tchildren.StrFullName = person.FamilyName + " " + person.FirstName;
          db.TChildrens.Attach(tchildren);
          db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
        }
        // Vater hat sich geändert.
        else if (person.FatherID != tchildren.StrFatherId && person.MotherID == tchildren.StrMotherId)
        {
          tchildren.StrFatherId = person.FatherID;
          tchildren.StrFullName = person.FamilyName + " " + person.FirstName;
          db.TChildrens.Attach(tchildren);
          db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
        }
        // Vater und Mutter haben sich geändert
        else if (person.FatherID != tchildren.StrFatherId && person.MotherID != tchildren.StrMotherId)
        {
          tchildren.StrFatherId = person.FatherID;
          tchildren.StrMotherId = person.MotherID;
          tchildren.StrFullName = person.FamilyName + " " + person.FirstName;
          db.TChildrens.Attach(tchildren);
          db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
        }
        // Vater und Mutter haben sich geändert
        else if (person.FatherID != tchildren.StrFatherId || person.MotherID != tchildren.StrMotherId)
        {
          tchildren.StrFatherId = person.FatherID;
          tchildren.StrMotherId = person.MotherID;
          tchildren.StrFullName = person.FamilyName + " " + person.FirstName;
          db.TChildrens.Attach(tchildren);
          db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
          db.SaveChanges();
        }
      }
      else
      {
        tchildren = new TChildren();
        tchildren.StrPersonId = person.PersonID;
        tchildren.StrFatherId = person.FatherID;
        tchildren.StrMotherId = person.MotherID;
        tchildren.StrFullName = person.FamilyName + " " + person.FirstName;
        db.TChildrens.Attach(tchildren);
        db.Entry(tchildren).State = Microsoft.EntityFrameworkCore.EntityState.Added;
        db.SaveChanges();
      }
    }

    #region Personenbilder




    public List<CImagePerson> GetPublicImagePersons()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1).OrderByDescending(x => x.AddDate).OrderByDescending(x => x.InProgress))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          arl.Add(pti);
        }

        return arl;
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    /// <summary>
    /// Alle Fotos die der normale Benutzer sechen soll
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetPublicImagePersonsOverviewLimited()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        // in Progress
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.InProgress == true && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }


        // Archiviert
        //foreach (tImagePersons t in db.tImagePersons.Where(x => x.Id >= -1 && x.IsArchivated == true && x.IsExported == false && x.Active == true).OrderByDescending(x => x.Add_Date))
        //{
        //    AhnenforschungModel.CImagePersons pti = new AhnenforschungModel.CImagePersons
        //    {
        //        ImagePersonsPositions = new List<AhnenforschungModel.CImagePersonPosition>()
        //    };
        //    MappImagePersonEntityToModel(ref pti, t);
        //    if (arl.Find(x => x.Id == pti.Id) == null)
        //    {
        //        arl.Add(pti);
        //    }
        //}

        // Exportiert
        //foreach (tImagePersons t in db.tImagePersons.Where(x => x.Id >= -1 && x.IsExported == true && x.Active == true).OrderByDescending(x => x.Add_Date))
        //{
        //    AhnenforschungModel.CImagePersons pti = new AhnenforschungModel.CImagePersons
        //    {
        //        ImagePersonsPositions = new List<AhnenforschungModel.CImagePersonPosition>()
        //    };
        //    MappImagePersonEntityToModel(ref pti, t);
        //    if (arl.Find(x => x.Id == pti.Id) == null)
        //    {
        //        arl.Add(pti);
        //    }
        //}

        // Akive
        //foreach (tImagePersons t in db.tImagePersons.Where(x => x.Id >= -1 && x.InProgress == false && x.IsArchivated == false && x.IsExported == false && x.Active == true).OrderByDescending(x => x.Add_Date))
        //{
        //    AhnenforschungModel.CImagePersons pti = new AhnenforschungModel.CImagePersons
        //    {
        //        ImagePersonsPositions = new List<AhnenforschungModel.CImagePersonPosition>()
        //    };
        //    MappImagePersonEntityToModel(ref pti, t);
        //    if (arl.Find(x => x.Id == pti.Id) == null)
        //    {
        //        arl.Add(pti);
        //    }
        //}
        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }


    /// <summary>
    /// Alle Fotos die der normale Benutzer sechen soll
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetPublicImagePersonsOverview()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        // in Progress
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.InProgress == true && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }


        // Archiviert
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.IsArchivated == true && x.IsExported == false && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }

        // Exportiert
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.IsExported == true && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }

        // Akive
        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.InProgress == false && x.IsArchivated == false && x.IsExported == false && x.Active == true).OrderByDescending(x => x.AddDate))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          if (arl.Find(x => x.Id == pti.Id) == null)
          {
            arl.Add(pti);
          }
        }
        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    /// <summary>
    /// all images listed and sorted.
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetAllImagePersons()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1).OrderByDescending(x => x.AddDate).OrderByDescending(x => x.InProgress))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          arl.Add(pti);
        }

        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    /// <summary>
    /// all active images listed and sorted.
    /// </summary>
    /// <returns></returns>
    public List<CImagePerson> GetActiveImagePersons()
    {
      try
      {
        List<CImagePerson> arl = new List<CImagePerson>();

        foreach (TImagePerson t in db.TImagePersons.Where(x => x.Id >= -1 && x.Active == true).OrderByDescending(x => x.AddDate).OrderByDescending(x => x.InProgress))
        {
          CImagePerson pti = new CImagePerson
          {
            ImagePersonsPositions = new List<CImagePersonPosition>()
          };
          MappImagePersonEntityToModel(ref pti, t);
          arl.Add(pti);
        }

        return arl.OrderByDescending(x => x.InProgress).OrderBy(x => x.IsArchivated).ToList();
      }
      catch (Exception)
      {
        return new List<CImagePerson>();
      }
    }

    private List<CImagePersonPosition> GetImagePersonsPositionByImageId(int idImage)
    {
      try
      {
        List<CImagePersonPosition> arl = new List<CImagePersonPosition>();

        foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.IdImagePerson == idImage))
        {
          CImagePersonPosition pos = new CImagePersonPosition();
          MappImagePersonPositionEntityToModel(ref pos, t);
          arl.Add(pos);
        }
        return arl;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Insert on position
    /// </summary>
    /// <param name="IdImage"></param>
    /// <returns>New CImagePersonPosition</returns>
    public CImagePersonPosition InsertImagePosition(int IdImage)
    {
      try
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {

            CImagePersonPosition imagePosition = null;
            CImagePerson image = null;
            int pos = 1;
            int newID = -1;

            if (db.TImagePersonsPositions.Any(x => x.IdImagePerson == IdImage))
            {
              pos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == IdImage).Max(x => x.Pos);
              pos = pos + 1;
            }

            TImagePersonsPosition t1 = CreateEmptyImagePersonPositionEntity(IdImage);
            t1.Pos = pos;
            t1.Active = true;
            db.TImagePersonsPositions.Add(t1);
            db.SaveChanges();
            newID = t1.Id;
            imagePosition = GetImagePersonsPositionById(newID);
            image = GetImagePersonsById(imagePosition.IdImagePerson, withPositions: true);
            image.PositionsCount = image.PositionsCount + 1;
            UpdatePositionCount(image, transaction);

            transaction.Commit();

            return imagePosition;
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Leerstruktur
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    private CImagePersonPosition CreateEmptyImagePersonPositionModel(int imageId)
    {
      CImagePersonPosition oPosition = new CImagePersonPosition();
      oPosition.IdPersonPosition = -1;
      oPosition.IdImagePerson = imageId;
      oPosition.Pos = 0;
      oPosition.PersonName = string.Empty;
      oPosition.PersonPreName = string.Empty;
      oPosition.PersonAddress = string.Empty;
      oPosition.PersonHouseNo = string.Empty;
      oPosition.PersonZip = string.Empty;
      oPosition.PersonCountry = string.Empty;
      oPosition.PersonTown = string.Empty;
      oPosition.PersonBirthYear = 0;
      oPosition.ReferencePersonId = string.Empty;
      oPosition.PersonDescription = string.Empty;
      oPosition.EditContactData = string.Empty;
      oPosition.EditEmail = string.Empty;
      oPosition.Person_Add_Date = DateTime.Now;
      oPosition.Person_Upd_Date = DateTime.Now;
      oPosition.PersonFinish = false;
      oPosition.PersonActive = false;

      return oPosition;
    }

    /// <summary>
    /// Leerstruktur
    /// </summary>
    /// <param name="imageId"></param>
    /// <returns></returns>
    private TImagePersonsPosition CreateEmptyImagePersonPositionEntity(int imageId)
    {
      TImagePersonsPosition tEmpty = new TImagePersonsPosition();
      tEmpty.IdImagePerson = imageId;
      tEmpty.Pos = 0;
      tEmpty.Name = string.Empty;
      tEmpty.PreName = string.Empty;
      tEmpty.Address = string.Empty;
      tEmpty.HouseNo = string.Empty;
      tEmpty.Zip = string.Empty;
      tEmpty.Country = string.Empty;
      tEmpty.Town = string.Empty;
      tEmpty.BirthYear = 0;
      tEmpty.ReferencePersonId = string.Empty;
      tEmpty.Description = string.Empty;
      tEmpty.EditContactData = string.Empty;
      tEmpty.EditEmail = string.Empty;
      tEmpty.AddDate = DateTime.Now;
      tEmpty.UpdDate = DateTime.Now;
      tEmpty.Finish = false;
      tEmpty.Active = false;

      return tEmpty;
    }

    public CImagePerson CreateEmptyImagePersons(CImagePerson oImage)
    {
      oImage.Id = -1;
      oImage.FileName = "";
      oImage.OriginalFileName = "?";
      oImage.ImagePath = "http://placehold.it/750x550";
      oImage.Title = "";
      oImage.Description = "";
      oImage.PositionsCount = 0;
      oImage.Add_Date = DateTime.Now;
      oImage.Active = false;
      oImage.InProgress = false;
      oImage.IsArchivated = false;

      oImage.IsExported = false;
      oImage.SourceDescription = "";
      oImage.SourceImageFileName = "";

      oImage.ImagePersonsPositions = new List<CImagePersonPosition>();
      CImagePersonPosition oPosition;
      for (int i = 0; i < oImage.PositionsCount; i++)
      {
        oPosition = CreateEmptyImagePersonPositionModel(oImage.Id);
        oPosition.Pos = i + 1;
        oPosition.PersonActive = true;

        oImage.ImagePersonsPositions.Add(oPosition);
      }
      return oImage;
    }

    private CImagePersonPosition CreateEmptyCreateOrUpdateImagePersonsPosition(int idImage, int maxPosition, bool withUpdate, IDbContextTransaction transaction)
    {
      try
      {
        TImagePersonsPosition tPosAdd = CreateEmptyImagePersonPositionEntity(idImage);
        CImagePersonPosition pos = new CImagePersonPosition();
        tPosAdd.Pos = maxPosition;
        tPosAdd.Active = true;
        MappImagePersonPositionEntityToModel(ref pos, tPosAdd);
        if (withUpdate)
        {
          db.TImagePersonsPositions.Attach(tPosAdd);
          db.Entry(tPosAdd).State = EntityState.Added;
          db.SaveChanges();
        }

        return pos;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Die Positinen für ein Bild erstellen
    /// </summary>
    /// <param name="idImage"></param>
    public void InsertPositionsToImagePostion(int idImage)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          TImagePerson tImg = db.TImagePersons.FirstOrDefault(x => x.Id == idImage);
          int countPos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == idImage).Count();
          if (tImg.PositionsCount > countPos)
          {
            if (tImg != null)
            {
              if (countPos == 0)
              {
                for (int i = 1; i <= tImg.PositionsCount; i++)
                {
                  CreateEmptyCreateOrUpdateImagePersonsPosition(tImg.Id, i, withUpdate: true, transaction: transaction);
                }
              }
              else
              {
                // nur wenn noch keine Position bearbeitet wurde.
                if (!db.TImagePersonsPositions.Any(x => x.Finish == true))
                {
                  int maxPos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == tImg.Id).Max(x => x.Pos);
                  if (maxPos <= countPos)
                  {
                    maxPos = maxPos + 1; ;
                    for (int i = maxPos; i <= tImg.PositionsCount; i++)
                    {
                      CreateEmptyCreateOrUpdateImagePersonsPosition(tImg.Id, i, withUpdate: true, transaction: transaction);
                    }
                  }
                }
              }
            }
          }
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }

    /// <summary>
    /// Bearbeitungsfortschritt
    /// </summary>
    /// <param name="onlyActive"></param>
    /// <returns></returns>
    public List<CImagePersonProgress> CalcWorkProgress(bool onlyActive)
    {
      try
      {
        List<CImagePersonProgress> arlProgress = new List<CImagePersonProgress>();
        List<CImagePerson> oImages = null;
        CImagePersonProgress oProgress = null;

        if (onlyActive)
        {
          oImages = GetActiveImagePersons();
        }
        else
        {
          oImages = GetAllImagePersons();
        }

        foreach (CImagePerson image in oImages)
        {
          oProgress = new CImagePersonProgress();
          Int32 count = GetImagePersonsPositionByImageId(image.Id).Count(x => x.PersonName != string.Empty && x.PersonPreName != string.Empty);
          oProgress.Id = image.Id;
          oProgress.QuantityReady = count;
          oProgress.Quantity = image.PositionsCount;
          oProgress.Percent = (100.00 / oProgress.Quantity) * oProgress.QuantityReady;
          if (oProgress.Percent > 100)
          {
            oProgress.Percent = 100;
          }
          oProgress.PercentDisplay = Math.Ceiling(oProgress.Percent) + "%";
          oProgress.ProgressClass = "progress-bar progress-bar bg-warning";
          Int32 countFinish = GetImagePersonsPositionByImageId(image.Id).Count(x => x.ReferencePersonId != string.Empty);
          if (oProgress.Quantity > 0 && oProgress.Quantity == oProgress.QuantityReady && oProgress.QuantityReady == countFinish)
          {
            oProgress.ProgressClass = "progress-bar progress-bar-success";
          }

          arlProgress.Add(oProgress);
        }

        return arlProgress;
      }
      catch (Exception ex)
      {
        throw ex;
      }

    }

    /// <summary>
    /// exist changes
    /// </summary>
    /// <param name="oClientImage"></param>
    /// <returns></returns>
    public bool IsModifiedImagePersonsOrPositions(CImagePerson oClientImage)
    {
      try
      {
        bool modified = false;
        CImagePerson oServerImage = GetImagePersonsById(oClientImage.Id, withPositions: true);

        CImagePersonPosition posClient;
        foreach (CImagePersonPosition posServer in oClientImage.ImagePersonsPositions)
        {
          posClient = oServerImage.ImagePersonsPositions.FirstOrDefault(x => x.IdPersonPosition == posServer.IdPersonPosition);
          if (posClient != null)
          {
            if (!posClient.Equals(posServer))
            {
              modified = true;
              break;
            }
          }
        }

        if (!modified)
        {
          if (oServerImage != null)
          {
            if (!oClientImage.Equals(oServerImage))
            {
              modified = true;
            }
          }
        }
        return modified;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    ///  Get imge with or non positions
    /// </summary>
    /// <param name="idImage"></param>
    /// <param name="withPositions"></param>
    /// <returns></returns>
    public CImagePerson GetImagePersonsById(int idImage, bool withPositions)
    {
      try
      {
        CImagePerson oImage = new CImagePerson();
        if (idImage > 0)
        {
          TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == idImage);

          oImage.ImagePersonsPositions = new List<CImagePersonPosition>();
          MappImagePersonEntityToModel(ref oImage, tImage);
          if (withPositions)
          {
            oImage.ImagePersonsPositions = GetImagePersonsPositionByImageId(oImage.Id);
          }
        }
        return oImage;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// All images for in person
    /// </summary>
    /// <param name="person"></param>
    /// <returns></returns>
    public List<CImagePerson> GetImagePositionWithPositionsByPerson(string person)
    {
      try
      {
        List<CImagePerson> oImages = new List<CImagePerson>();
        var persons = new List<int>();
        {
          foreach (TImagePersonsPosition pos in db.TImagePersonsPositions.Where(x => x.ReferencePersonId == person && x.Active == true && x.Finish == true))
          {
            if (!persons.Contains(pos.IdImagePerson))
            {
              persons.Add(pos.IdImagePerson);
            }

          }

          foreach (TImagePerson image in db.TImagePersons.Where(x => x.Active == true && persons.Contains(x.Id)))
          {
            var img = GetImagePersonsById(image.Id, withPositions: true);
            oImages.Add(img);
          }
          return oImages;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// On image wirth all pos
    /// </summary>
    /// <param name="imagePersonId"></param>
    /// <returns></returns>
    public CImagePerson GetImagePersonWithPositionsById(int imagePersonId)
    {
      try
      {
        CImagePerson oImages = new CImagePerson();
        oImages = GetImagePersonsById(imagePersonId, withPositions: true);
        return oImages;

      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
        throw;
      }
    }


    public CImagePersonPosition GetImagePersonsPositionById(int idPosition)
    {
      try
      {
        TImagePersonsPosition tPos = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == idPosition);
        CImagePersonPosition oPos = new CImagePersonPosition();

        if (tPos != null)
        {
          MappImagePersonPositionEntityToModel(ref oPos, tPos);
        }
        return oPos;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Default Adresse
    /// </summary>
    /// <param name="imagePersonPosition"></param>
    /// <returns></returns>
    public CImagePersonPosition UseImagePersonPositionDefaultValues(CImagePersonPosition imagePersonPosition)
    {
      try
      {
        string country = "Liechtenstein";
        if (imagePersonPosition.PersonZip != null && imagePersonPosition.PersonZip.Trim().Length == 4)
        {
          switch (imagePersonPosition.PersonZip)
          {
            case "9485":
              imagePersonPosition.PersonTown = "Nendeln";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9486":
              imagePersonPosition.PersonTown = "Schaanwald";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9487":
              imagePersonPosition.PersonTown = "Bendern";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9488":
              imagePersonPosition.PersonTown = "Schellenberg";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9489":
              imagePersonPosition.PersonTown = "Vaduz";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9490":
              imagePersonPosition.PersonTown = "Vaduz";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9491":
              imagePersonPosition.PersonTown = "Ruggell";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9492":
              imagePersonPosition.PersonTown = "Eschen";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9493":
              imagePersonPosition.PersonTown = "Mauren";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9494":
              imagePersonPosition.PersonTown = "Schaan";
              imagePersonPosition.PersonCountry = "Liechtenstein";
              break;
            case "9495":
              imagePersonPosition.PersonTown = "Triesen";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9496":
              imagePersonPosition.PersonTown = "Balzers";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9497":
              imagePersonPosition.PersonTown = "Triesenberg";
              imagePersonPosition.PersonCountry = country;
              break;
            case "9498":
              imagePersonPosition.PersonTown = "Planken";
              imagePersonPosition.PersonCountry = country;
              break;
          }
        }
        else if (imagePersonPosition.PersonTown != null)
        {
          switch (imagePersonPosition.PersonTown)
          {
            case "Nendeln":
              imagePersonPosition.PersonZip = "9485";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Schaanwald":
              imagePersonPosition.PersonZip = "9486";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Bendern":
              imagePersonPosition.PersonZip = "9487";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Schellenberg":
              imagePersonPosition.PersonZip = "9488";
              imagePersonPosition.PersonCountry = country;
              break;
            //case "Vaduz":
            //  imagePersonPosition.PersonZip = "9489";
            //  imagePersonPosition.PersonCountry = country;
            //  break;
            case "Vaduz":
              imagePersonPosition.PersonZip = "9490";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Ruggell":
              imagePersonPosition.PersonZip = "9491";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Eschen":
              imagePersonPosition.PersonZip = "9492";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Mauren":
              imagePersonPosition.PersonZip = "9493";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Schaan":
              imagePersonPosition.PersonZip = "9494";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Triesen":
              imagePersonPosition.PersonZip = "9495";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Balzers":
              imagePersonPosition.PersonZip = "9496";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Triesenberg":
              imagePersonPosition.PersonZip = "9497";
              imagePersonPosition.PersonCountry = country;
              break;
            case "Planken":
              imagePersonPosition.PersonZip = "9498";
              imagePersonPosition.PersonCountry = country;
              break;
          }
        }
        return imagePersonPosition;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public CImagePerson CreateOrUpdateImagePersons(CImagePerson oImage)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == oImage.Id);
        CImagePerson oResultImage;

        if (t == null)
        {
          int newID = -1;
          t = new TImagePerson();
          MappImagePersonModelToEntity(oImage, ref t);
          //using (Context.AhnenforschungEntityDataModel entities = new Context.AhnenforschungEntityDataModel())
          //{
          using (IDbContextTransaction transaction = db.Database.BeginTransaction())
          {
            try
            {
              db.TImagePersons.Add(t);
              db.Entry(t).State = EntityState.Added;
              db.SaveChanges();
              newID = t.Id;

              CImagePerson oImageAdded = new CImagePerson
              {
                ImagePersonsPositions = new List<CImagePersonPosition>()
              };
              t = db.TImagePersons.FirstOrDefault(x => x.Id == newID);
              MappImagePersonEntityToModel(ref oImageAdded, t);
              oImageAdded.ImagePersonsPositions = oImage.ImagePersonsPositions;
              CreateOrUpdateImagePersonPositions(oImageAdded.ImagePersonsPositions, transaction);
              oResultImage = oImageAdded;

              transaction.Commit();
            }
            catch (Exception ex)
            {
              transaction.Rollback();
              throw ex;
            }

          }
          // }
        }
        else
        {
          using (IDbContextTransaction transaction = db.Database.BeginTransaction())
          {
            try
            {
              MappImagePersonModelToEntity(oImage, ref t);
              db.TImagePersons.Attach(t);
              db.Entry(t).State = EntityState.Modified;
              db.SaveChanges();
              CreateOrUpdateImagePersonPositions(oImage.ImagePersonsPositions, transaction);
              oResultImage = GetImagePersonsById(oImage.Id, withPositions: true);
              transaction.Commit();
            }
            catch (Exception ex)
            {
              transaction.Rollback();
              throw ex;
            }
          }

        }
        return oResultImage;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public void UpdateImagePersonsDescription(CImagePerson oImage)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == oImage.Id);
        t.Description = oImage.Description;
        db.TImagePersons.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    public void UpdateImagePersonPositionDescription(CImagePersonPosition oPos)
    {
      try
      {
        TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == oPos.IdPersonPosition);
        t.Description = oPos.PersonDescription;
        db.TImagePersonsPositions.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private void UpdatePositionCount(CImagePerson oImage, IDbContextTransaction transaction)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == oImage.Id);
        t.PositionsCount = oImage.PositionsCount;
        db.TImagePersons.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    public CImagePersonPosition SetImagePersonPositionToFinish(int id)
    {
      try
      {
        TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == id);
        t.Finish = !t.Finish;
        t.UpdDate = DateTime.Now;
        db.TImagePersonsPositions.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
        return GetImagePersonsPositionById(id);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Status Bild ist exportiert 
    /// </summary>
    /// <param name="idImpage"></param>
    /// <param name="withPositions"></param>
    /// <returns>Changed Person image with or no positions</returns>
    public CImagePerson SetStateExported(int idImpage, bool withPositions)
    {
      try
      {
        TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == idImpage);
        t.IsExported = true;
        db.TImagePersons.Attach(t);
        db.Entry(t).State = EntityState.Modified;
        db.SaveChanges();
        return GetImagePersonsById(idImpage, withPositions);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Eine Position speichern
    /// </summary>
    /// <param name="modifiedPosition"></param>
    public CImagePersonPosition UpdateImagePersonPosition(CImagePersonPosition modifiedPosition)
    {
      try
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {
            CReadWriteData oRwd = new CReadWriteData();
            if (modifiedPosition != null)
            {
              TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == modifiedPosition.IdPersonPosition);
              if (t != null)
              {
                t.UpdDate = DateTime.Now;
                modifiedPosition = UseImagePersonPositionDefaultValues(modifiedPosition);
                oRwd.MappImagePersonPositionModelToEntity(modifiedPosition, ref t);
                modifiedPosition.Person_Upd_Date = DateTime.Now;
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                InsertImagePersonPositionHistory(null, modifiedPosition, transaction);
                transaction.Commit();
              }
            }
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }

        return GetImagePersonsPositionById(modifiedPosition.IdPersonPosition);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    private void CreateOrUpdateImagePersonPositions(List<CImagePersonPosition> Positions, IDbContextTransaction transaction)
    {
      try
      {
        CReadWriteData oRwd = new CReadWriteData();
        if (Positions != null)
        {
          foreach (CImagePersonPosition ptiPos in Positions)
          {
            TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == ptiPos.IdPersonPosition);
            if (t != null)
            {
              t.UpdDate = DateTime.Now;
              oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
              db.TImagePersonsPositions.Attach(t);
              db.Entry(t).State = EntityState.Modified;
              db.SaveChanges();
              InsertImagePersonPositionHistory(null, ptiPos, transaction);
            }
            else
            {
              t = new TImagePersonsPosition();
              oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
              db.TImagePersonsPositions.Attach(t);
              db.Entry(t).State = EntityState.Added;
              db.SaveChanges();
              InsertImagePersonPositionHistory(null, ptiPos, transaction);
            }
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    public void UpdateModifiedImagePersonPositions(CImagePerson ModifiedImagePersons, CImagePerson SessionImagePersons)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          TImagePersonsPosition tPos = null;
          CImagePersonPosition posSession;
          foreach (CImagePersonPosition posModified in ModifiedImagePersons.ImagePersonsPositions)
          {
            posSession = SessionImagePersons.ImagePersonsPositions.FirstOrDefault(x => x.IdPersonPosition == posModified.IdPersonPosition);
            if (posSession != null)
            {
              if (!posSession.Equals(posModified))
              {
                tPos = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == posModified.IdPersonPosition);
                if (tPos != null)
                {
                  MappImagePersonPositionModelToEntity(posModified, ref tPos);
                  tPos.UpdDate = DateTime.Now;
                  db.TImagePersonsPositions.Attach(tPos);
                  db.Entry(tPos).State = EntityState.Modified;
                  db.SaveChanges();
                }
              }
            }
          }

          if (!SessionImagePersons.Equals(ModifiedImagePersons))
          {
            TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == ModifiedImagePersons.Id);
            if (tImage != null)
            {
              MappImagePersonModelToEntity(ModifiedImagePersons, ref tImage);
              db.TImagePersons.Attach(tImage);
              db.Entry(tImage).State = EntityState.Modified;
              db.SaveChanges();
            }
          }
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }

    /// <summary>
    ///  Eine Liste von Positionen speichern
    /// </summary>
    /// <param name="Positions"></param>
    public void CreateOrUpdateImagePersonPositions(List<CImagePersonPosition> positions)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          CReadWriteData oRwd = new CReadWriteData();
          if (positions != null)
          {
            foreach (CImagePersonPosition ptiPos in positions)
            {
              TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == ptiPos.IdPersonPosition);
              if (t != null)
              {
                t.UpdDate = DateTime.Now;
                oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                InsertImagePersonPositionHistory(null, ptiPos, transaction);
              }
              else
              {
                t = new TImagePersonsPosition();
                oRwd.MappImagePersonPositionModelToEntity(ptiPos, ref t);
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Added;
                db.SaveChanges();
              }
            }
          }
          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }


    /// <summary>
    /// Loggen
    /// </summary>
    /// <param name="oImage"></param>
    /// <param name="oPosition"></param>
    /// <param name="transaction"></param>
    private void InsertImagePersonPositionHistory(CImagePerson oImage, CImagePersonPosition oPosition, IDbContextTransaction transaction)
    {
      try
      {
        TImagePersonsPositionsHistory t = new TImagePersonsPositionsHistory();
        t.IdImagePersonPosition = oPosition.IdPersonPosition;
        t.IdImagePerson = oPosition.IdImagePerson;
        t.Pos = oPosition.Pos;
        t.Name = CGlobal.ConvertNullToString(oPosition.PersonName);
        t.PreName = CGlobal.ConvertNullToString(oPosition.PersonPreName);
        t.Address = CGlobal.ConvertNullToString(oPosition.PersonAddress);
        t.HouseNo = CGlobal.ConvertNullToString(oPosition.PersonHouseNo);
        t.Zip = CGlobal.ConvertNullToString(oPosition.PersonZip);
        t.Country = CGlobal.ConvertNullToString(oPosition.PersonCountry);
        t.Town = CGlobal.ConvertNullToString(oPosition.PersonTown);
        t.BirthYear = oPosition.PersonBirthYear;
        t.ReferencePersonId = CGlobal.ConvertNullToString(oPosition.ReferencePersonId);
        if (oImage != null)
        {
          t.ImageDescription = CGlobal.ConvertNullToString(oImage.Description);
        }
        else
        {
          t.ImageDescription = string.Empty;
        }

        t.PersonDescription = CGlobal.ConvertNullToString(oPosition.PersonDescription);
        t.EditContactData = CGlobal.ConvertNullToString(oPosition.EditContactData);
        t.EditEmail = CGlobal.ConvertNullToString(oPosition.EditEmail);
        t.AddDate = oPosition.Person_Add_Date;
        t.UpdDate = oPosition.Person_Upd_Date;
        t.Finish = oPosition.PersonFinish;
        t.Active = oPosition.PersonActive;
        t.AddHistoryDate = DateTime.Now;

        db.TImagePersonsPositionsHistories.Attach(t);
        db.Entry(t).State = EntityState.Added;
        db.SaveChanges();
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Delete Image with all positions
    /// </summary>
    /// <param name="id"></param>
    public void DeleteImagePersonsByImageId(int id)
    {
      using (IDbContextTransaction transaction = db.Database.BeginTransaction())
      {
        try
        {
          TImagePerson t = db.TImagePersons.FirstOrDefault(x => x.Id == id);
          db.TImagePersons.Attach(t);
          db.Entry(t).State = EntityState.Deleted;
          db.SaveChanges();

          DeleteImagePersonPositionsByImageId(id, transaction);

          transaction.Commit();
        }
        catch (Exception ex)
        {
          transaction.Rollback();
          throw ex;
        }
      }
    }

    /// <summary>
    /// Delete all positions
    /// </summary>
    /// <param name="id"></param>
    /// <param name="tx"></param>
    public void DeleteImagePersonPositionsByImageId(int id, IDbContextTransaction tx)
    {
      try
      {
        if (id > 0)
        {
          foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.IdImagePerson == id))
          {
            db.TImagePersonsPositions.Attach(t);
            db.Entry(t).State = EntityState.Deleted;
            db.SaveChanges();
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Delete all positions
    /// </summary>
    /// <param name="id"></param>
    public void DeleteImagePersonPositionsByImageId(int id)
    {
      if (id > 0)
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {
            foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.IdImagePerson == id))
            {
              db.TImagePersonsPositions.Attach(t);
              db.Entry(t).State = EntityState.Deleted;
              db.SaveChanges();
            }
            transaction.Commit();
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }
      }
    }

    /// <summary>
    /// Is CreateOrUpdateImagePersons by id active
    /// </summary>
    /// <param name="idImage"></param>
    /// <returns></returns>
    public bool IsImagePersonActive(int idImage)
    {
      try
      {
        bool isActive = false;

        if (idImage > 0)
        {
          TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == idImage);
          if (tImage != null)
          {
            isActive = tImage.Active.Value;
          }
        }

        return isActive;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Is CreateOrUpdateImagePersons by position active
    /// </summary>
    /// <param name="idPosition"></param>
    /// <returns>true or false</returns>
    public bool IsImagePersonsActiveByPositionsById(int idPosition)
    {
      try
      {
        bool isActive = false;

        if (idPosition > 0)
        {
          TImagePersonsPosition tPos = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == idPosition);
          if (tPos != null)
          {
            TImagePerson tImage = db.TImagePersons.FirstOrDefault(x => x.Id == tPos.IdImagePerson);
            isActive = tImage.Active.Value;
          }
        }

        return isActive;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }


    /// <summary>
    /// Delete on single position.
    /// </summary>
    /// <param name="id"></param>
    public int DeleteImagePersonPosition(int id)
    {
      try
      {
        using (IDbContextTransaction transaction = db.Database.BeginTransaction())
        {
          try
          {
            int personImageId = -1;
            if (id > 0)
            {
              TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == id);
              if (!t.Finish)
              {
                personImageId = t.IdImagePerson;
                RecalcImagePersonPositions(t.Pos, transaction); // Positionen neu berechnen
                db.TImagePersonsPositions.Attach(t);
                db.Entry(t).State = EntityState.Deleted;
                db.SaveChanges();
                int countPos = db.TImagePersonsPositions.Where(x => x.IdImagePerson == personImageId).Count();
                var image = GetImagePersonsById(personImageId, withPositions: false);
                image.PositionsCount = countPos;
                UpdatePositionCount(image, transaction);

              }
            }

            transaction.Commit();

            return personImageId;
          }
          catch (Exception ex)
          {
            transaction.Rollback();
            throw ex;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Datensatz als gelöscht markieren.
    /// </summary>
    /// <param name="idPosition"></param>
    public void RemoveDeleteImagePersonPosition(CImagePersonPosition oPosition)
    {
      try
      {
        if (oPosition != null)
        {
          TImagePersonsPosition t = db.TImagePersonsPositions.FirstOrDefault(x => x.Id == oPosition.IdPersonPosition);
          if (!t.Finish)
          {
            //oPosition.PersonName = "gelöscht";
            //oPosition.PersonPreName = "gelöscht";
            //oPosition.PersonBirthYear = 0;
            //oPosition.PersonAddress = "gelöscht";
            //oPosition.PersonCountry = "gelöscht"; ;
            //oPosition.PersonDescription = "gelöscht";
            //oPosition.EditContactData = "gelöscht";
            //oPosition.EditEmail = "gelöscht";
            //oPosition.PersonHouseNo = "gelöscht";
            //oPosition.PersonTown = "gelöscht";
            //oPosition.PersonZip = "0000";

            oPosition.PersonName = null;
            oPosition.PersonPreName = null;
            oPosition.PersonBirthYear = 0;
            oPosition.PersonAddress = null;
            oPosition.PersonCountry = null;
            oPosition.PersonDescription = null;
            oPosition.EditContactData = null;
            oPosition.EditEmail = null;
            oPosition.PersonHouseNo = null;
            oPosition.PersonTown = null;
            oPosition.PersonZip = null;


            UpdateImagePersonPosition(oPosition);
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Recalc after deleted position
    /// </summary>
    /// <param name="StartPosition"></param>
    private void RecalcImagePersonPositions(int StartPosition, IDbContextTransaction transaction)
    {
      try
      {
        if (StartPosition > 0)
        {
          int i = StartPosition;
          foreach (TImagePersonsPosition t in db.TImagePersonsPositions.Where(x => x.Pos > StartPosition).OrderBy(x => x.Pos))
          {
            t.Pos = StartPosition;
            db.TImagePersonsPositions.Attach(t);
            db.Entry(t).State = EntityState.Deleted;
            db.SaveChanges();
            StartPosition += 1;
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }











    #endregion
    #region MAPPING ENTITY TO MODEL AND MODEL TO ENTITY

    private void MappPersonEntityToSmallModel(ref CPerson oPerson, TPerson tperson, CSettings oSettings)
    {

      oPerson.tikBirth = Convert.ToUInt64(tperson.TikBirth);
      oPerson.tikDeath = Convert.ToUInt64(tperson.TkDeath);
      oPerson.tikBur = Convert.ToUInt64(tperson.TikBurDate);

      oPerson.ID = tperson.NPersonId;
      oPerson.PersonID = Convert.ToString(tperson.StrPersonId);
      oPerson.FirstName = Convert.ToString(tperson.StrPreName);
      oPerson.FamilyName = Convert.ToString(tperson.StrName);
      oPerson.Fullname = Convert.ToString(tperson.StrFullname);
      oPerson.Bur = Convert.ToString(tperson.StrBurPlace);
      oPerson.Sex = Convert.ToString(tperson.StrSex);
      oPerson.FatherID = Convert.ToString(tperson.StrFatherId);
      oPerson.MotherID = Convert.ToString(tperson.StrMotherId);

      //oPerson.HasParents = ExistParents(oPerson) == CGlobal.DBTRUE ? true : false; // 0; // Sind die Eltern bekannt?
      //oPerson.HasSpouse = ExistSibling(oPerson) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister
      //oPerson.HasChildrens = ExistChildrens(oPerson) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister

      oPerson.HasParents = false;
      oPerson.HasSpouse = false;
      oPerson.HasChildrens = false;

      oPerson.BirthPlace = ""; // Convert.ToString(tperson.strBi);
      oPerson.DeathPlace = ""; Convert.ToString(tperson.StrBurPlace);

      oPerson.BurPlace = Convert.ToString(tperson.StrBurPlace);
      oPerson.Race = Convert.ToString(tperson.StrRace);
      oPerson.Work = Convert.ToString(tperson.StrWork);

      oPerson.NameMerges = Convert.ToString(tperson.StrMarriedName);
      oPerson.Nickname = Convert.ToString(tperson.StrNick);

      oPerson.Active = tperson.NActive == CGlobal.DBTRUE ? true : false;
      oPerson.Address = Convert.ToString(tperson.StrAdress);

      //oPerson.IsDeath = tperson.NIsLiving == CGlobal.DBFALSE ? true : false;
      oPerson.IsDeath = tperson.NDeathYear > 1000; // tperson.NDeathYear > 0 && tperson.NDeathMonth > 0 && tperson.NDeathDay > 0; // == CGlobal.DBFALSE ? true : false;
      oPerson.SexDisplay = SexDisplay(oPerson);

      oPerson.Older = CGlobal.CalcualteOlder(oPerson.tikBirth, oPerson.tikDeath, oPerson.IsDeath);

      MapDateTimeEntityToModel(tperson, ref oPerson);


      oPerson.ImagePathSmall = string.Empty;
      oPerson.ImagePath = string.Empty;
      oPerson.ImagePathBig = string.Empty;

    }


    /// <summary>
    /// Die Entity ins Model uebertragen
    /// </summary>
    /// <param name="oPerson"></param>
    /// <param name="tperson"></param>
    /// <param name="oSettings"></param>
    private void MappPersonEntityToModel(ref CPerson oPerson, TPerson tperson, CSettings oSettings)
    {

      oPerson.tikBirth = Convert.ToUInt64(tperson.TikBirth);
      oPerson.tikDeath = Convert.ToUInt64(tperson.TkDeath);
      oPerson.tikBur = Convert.ToUInt64(tperson.TikBurDate);

      oPerson.ID = tperson.NPersonId;
      oPerson.PersonID = Convert.ToString(tperson.StrPersonId);
      oPerson.FirstName = Convert.ToString(tperson.StrPreName);
      oPerson.FamilyName = Convert.ToString(tperson.StrName);
      oPerson.Fullname = Convert.ToString(tperson.StrFullname);
      oPerson.Bur = Convert.ToString(tperson.StrBurPlace);
      oPerson.Sex = Convert.ToString(tperson.StrSex);
      oPerson.FatherID = Convert.ToString(tperson.StrFatherId);
      oPerson.MotherID = Convert.ToString(tperson.StrMotherId);

      oPerson.HasParents = ExistParents(oPerson) == CGlobal.DBTRUE ? true : false; // 0; // Sind die Eltern bekannt?
      oPerson.HasSpouse = ExistSibling(oPerson) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister
      oPerson.HasChildrens = ExistChildrens(oPerson) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister

      oPerson.BirthPlace = ""; // Convert.ToString(tperson.strBi);
      oPerson.DeathPlace = ""; Convert.ToString(tperson.StrBurPlace);

      oPerson.BurPlace = Convert.ToString(tperson.StrBurPlace);
      oPerson.Race = Convert.ToString(tperson.StrRace);
      oPerson.Work = Convert.ToString(tperson.StrWork);

      oPerson.NameMerges = Convert.ToString(tperson.StrMarriedName);
      oPerson.Nickname = Convert.ToString(tperson.StrNick);

      oPerson.Active = tperson.NActive == CGlobal.DBTRUE ? true : false;
      oPerson.Address = Convert.ToString(tperson.StrAdress);

      //oPerson.IsDeath = tperson.NIsLiving == CGlobal.DBFALSE ? true : false;
      oPerson.IsDeath = tperson.NDeathYear > 1000; // tperson.NDeathYear > 0 && tperson.NDeathMonth > 0 && tperson.NDeathDay > 0; // == CGlobal.DBFALSE ? true : false;
      oPerson.SexDisplay = SexDisplay(oPerson);

      oPerson.Older = CGlobal.CalcualteOlder(oPerson.tikBirth, oPerson.tikDeath, oPerson.IsDeath);

      MapDateTimeEntityToModel(tperson, ref oPerson);



      SetImagePath(oPerson, tperson, oSettings);
    }


    private void MappPersonEntityToModel(ref CPerson oPerson, TPerson tperson, List<TPerson> tpersons, CSettings oSettings)
    {

      oPerson.tikBirth = Convert.ToUInt64(tperson.TikBirth);
      oPerson.tikDeath = Convert.ToUInt64(tperson.TkDeath);
      oPerson.tikBur = Convert.ToUInt64(tperson.TikBurDate);

      oPerson.ID = tperson.NPersonId;
      oPerson.PersonID = Convert.ToString(tperson.StrPersonId);
      oPerson.FirstName = Convert.ToString(tperson.StrPreName);
      oPerson.FamilyName = Convert.ToString(tperson.StrName);
      oPerson.Fullname = Convert.ToString(tperson.StrFullname);
      oPerson.Bur = Convert.ToString(tperson.StrBurPlace);
      oPerson.Sex = Convert.ToString(tperson.StrSex);
      oPerson.FatherID = Convert.ToString(tperson.StrFatherId);
      oPerson.MotherID = Convert.ToString(tperson.StrMotherId);

      oPerson.HasParents = ExistParents(oPerson, tpersons) == CGlobal.DBTRUE ? true : false; // 0; // Sind die Eltern bekannt?
      oPerson.HasSpouse = ExistSibling(oPerson, tpersons) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister
      oPerson.HasChildrens = ExistChildrens(oPerson, tpersons) == CGlobal.DBTRUE ? true : false; //0; // Gibt es Geschwister

      oPerson.BirthPlace = ""; // Convert.ToString(tperson.strBi);
      oPerson.DeathPlace = ""; Convert.ToString(tperson.StrBurPlace);

      oPerson.BurPlace = Convert.ToString(tperson.StrBurPlace);
      oPerson.Race = Convert.ToString(tperson.StrRace);
      oPerson.Work = Convert.ToString(tperson.StrWork);

      oPerson.NameMerges = Convert.ToString(tperson.StrMarriedName);
      oPerson.Nickname = Convert.ToString(tperson.StrNick);

      oPerson.Active = tperson.NActive == CGlobal.DBTRUE ? true : false;
      oPerson.Address = Convert.ToString(tperson.StrAdress);

      //oPerson.IsDeath = tperson.NIsLiving == CGlobal.DBFALSE ? true : false;
      oPerson.IsDeath = tperson.NDeathYear > 1000; // tperson.NDeathYear > 0 && tperson.NDeathMonth > 0 && tperson.NDeathDay > 0; // == CGlobal.DBFALSE ? true : false;
      oPerson.SexDisplay = SexDisplay(oPerson);

      oPerson.Older = CGlobal.CalcualteOlder(oPerson.tikBirth, oPerson.tikDeath, oPerson.IsDeath);

      MapDateTimeEntityToModel(tperson, ref oPerson);



      SetImagePath(oPerson, tperson, oSettings);
    }

    /// <summary>
    /// Public: Die Entity ins Model uebertragen
    /// </summary>
    /// <param name="oPerson"></param>
    /// <param name="tperson"></param>
    /// <param name="oSettings"></param>
    public void MappPersonEntityToModelChildrenCache(ref CPerson oPerson, TPerson tperson, CSettings oSettings)
    {
      MappPersonEntityToModel(ref oPerson, tperson, oSettings);
    }

    /// <summary>
    /// Datum zum in die Entity Class schreiben.
    /// </summary>
    /// <param name="date"></param>
    /// <returns>Model to entiy</returns>
    private void MapDateTimeModelToEntity(CPerson oPerson, ref TPerson tperson)
    {
      if (oPerson.BirthDate < CGlobal.CliensSideEmptyDate())
      {
        oPerson.BirthDate = CGlobal.CliensSideEmptyDate();
      }

      if (oPerson.DeathDate < CGlobal.CliensSideEmptyDate())
      {
        oPerson.DeathDate = CGlobal.CliensSideEmptyDate();
      }

      // Eingebuergert am
      if (oPerson.BurDate < CGlobal.CliensSideEmptyDate())
      {
        oPerson.BurDate = CGlobal.CliensSideEmptyDate();
      }


      tperson.TikBirth = (oPerson.BirthDate == CGlobal.CliensSideEmptyDate()) ? Convert.ToString(0) : oPerson.BirthDate.Ticks.ToString();
      tperson.TkDeath = (oPerson.DeathDate == CGlobal.CliensSideEmptyDate()) ? Convert.ToString(0) : oPerson.DeathDate.Ticks.ToString();
      tperson.TikBurDate = (oPerson.BurDate == CGlobal.CliensSideEmptyDate()) ? Convert.ToString(0) : oPerson.BurDate.Ticks.ToString();

      tperson.NBirthDay = (oPerson.BirthDate == CGlobal.CliensSideEmptyDate()) ? 0 : oPerson.BirthDate.Day;
      tperson.NBirthMonth = (oPerson.BirthDate == CGlobal.CliensSideEmptyDate()) ? 0 : oPerson.BirthDate.Month;
      tperson.NBirthYear = (oPerson.BirthDate == CGlobal.CliensSideEmptyDate()) ? 0 : oPerson.BirthDate.Year;

      tperson.NDeathDay = (oPerson.DeathDate == CGlobal.CliensSideEmptyDate()) ? 0 : oPerson.DeathDate.Day;
      tperson.NDeathMonth = (oPerson.DeathDate == CGlobal.CliensSideEmptyDate()) ? 0 : oPerson.DeathDate.Month;
      tperson.NDeathYear = (oPerson.DeathDate == CGlobal.CliensSideEmptyDate()) ? 0 : oPerson.DeathDate.Year;

      tperson.NIsLiving = (oPerson.DeathDate > CGlobal.CliensSideEmptyDate()) ? CGlobal.DBFALSE : CGlobal.DBTRUE;
    }

    /// <summary>
    /// Datum dem Model uebergeben
    /// </summary>
    /// <param name="dateticks"></param>
    /// <returns>Entity to Model</returns>
    public void MapDateTimeEntityToModel(TPerson tperson, ref CPerson oPerson)
    {
      oPerson.BirthDate = (Convert.ToInt64(tperson.TikBirth) == 0) ? CGlobal.CliensSideEmptyDate() : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikBirth));
      oPerson.DeathDate = (Convert.ToInt64(tperson.TkDeath) == 0) ? CGlobal.CliensSideEmptyDate() : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TkDeath));
      oPerson.BurDate = (Convert.ToInt64(tperson.TikBurDate) == 0) ? CGlobal.CliensSideEmptyDate() : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikBurDate));

      oPerson.BirthDisplay = (Convert.ToInt64(tperson.TikBirth) == 0) ? "" : oPerson.BirthDate.ToShortDateString();
      oPerson.DeathDisplay = (Convert.ToInt64(tperson.TkDeath) == 0) ? "" : oPerson.DeathDate.ToShortDateString();
      oPerson.BurDisplay = (Convert.ToInt64(tperson.TikBurDate) == 0) ? "" : oPerson.BurDate.ToShortDateString();
    }

    public static void MapDateTimeEntityToModelStatic(TPerson tperson, ref CPerson oPerson)
    {
      oPerson.BirthDate = (Convert.ToInt64(tperson.TikBirth) == 0) ? CGlobal.CliensSideEmptyDate() : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikBirth));
      oPerson.DeathDate = (Convert.ToInt64(tperson.TkDeath) == 0) ? CGlobal.CliensSideEmptyDate() : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TkDeath));
      oPerson.BurDate = (Convert.ToInt64(tperson.TikBurDate) == 0) ? CGlobal.CliensSideEmptyDate() : CGlobal.ConvertTicksToDateTime(Convert.ToInt64(tperson.TikBurDate));

      oPerson.BirthDisplay = (Convert.ToInt64(tperson.TikBirth) == 0) ? "" : oPerson.BirthDate.ToShortDateString();
      oPerson.DeathDisplay = (Convert.ToInt64(tperson.TkDeath) == 0) ? "" : oPerson.DeathDate.ToShortDateString();
      oPerson.BurDisplay = (Convert.ToInt64(tperson.TikBurDate) == 0) ? "" : oPerson.BurDate.ToShortDateString();
    }

    /// <summary>
    /// Model in die Entity schrieben
    /// </summary>
    /// <param name="Person"></param>
    /// <param name="tperson"></param>
    /// <param name="oSettings"></param>
    public void MappPersonModelToEntity(ref TPerson tperson, CPerson Person, CSettings oSettings)
    {
      //tperson.StrPersonId = Convert.ToString(Person.PersonID);
      tperson.StrName = CGlobal.ConvertNullToString(Convert.ToString(Person.FamilyName));
      tperson.StrPreName = CGlobal.ConvertNullToString(Convert.ToString(Person.FirstName));
      tperson.StrFullname = CGlobal.ConvertNullToString(Person.FirstName) + " " + CGlobal.ConvertNullToString(Person.FamilyName);

      MapDateTimeModelToEntity(Person, ref tperson);

      //tperson.strBirthPlace = Person.BirthPlace;
      tperson.StrBurPlace = CGlobal.ConvertNullToString(Person.BurPlace);

      tperson.StrSex = CGlobal.ConvertNullToString(Person.Sex);
      tperson.StrFatherId = CGlobal.ConvertNullToString(Person.FatherID); // Convert.ToString(GetFatherID(i_oPerson));
      tperson.StrMotherId = CGlobal.ConvertNullToString(Person.MotherID); //Convert.ToString(GetMotherID(i_oPerson));

      tperson.NHasParents = ExistParents(Person); // 0; // Sind die Eltern bekannt?
      tperson.NHasSpouse = ExistSibling(Person);//0; // Gibt es Geschwister

      tperson.StrWork = CGlobal.ConvertNullToString(Convert.ToString(Person.Work));
      tperson.StrRace = CGlobal.ConvertNullToString(Convert.ToString(Person.Race));
      tperson.StrNick = CGlobal.ConvertNullToString(Convert.ToString(Person.Nickname));

      tperson.NActive = Person.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;

      tperson.StrAdress = CGlobal.ConvertNullToString(Convert.ToString(Person.Address));

      tperson.StrEheName = CGlobal.ConvertNullToString(Convert.ToString(Person.NameMerges));

      tperson.StrMarriedName = CGlobal.ConvertNullToString(Convert.ToString(Person.NameMerges));

      //tperson.StrAdress = CGlobal.ConvertNullToString(Person.FullAdress);

    }

    /// <summary>
    /// Model in die Entity schreiben
    /// </summary>
    /// <param name="tuser"></param>
    /// <param name="User"></param>
    /// <param name="oSettings"></param>
    public void MappUserModelEntity(ref TUser tuser, CUser User)
    {

      // TODO: Datum mappen 
      tuser.NUserId = User.UserId;
      tuser.StrPersonId = CGlobal.ConvertNullToString(User.PersonId);
      tuser.StrSalutation = CGlobal.ConvertNullToString(User.Salutation);
      tuser.StrLetter = CGlobal.ConvertNullToString(User.Letter);
      tuser.StrName = CGlobal.ConvertNullToString(User.PreName);
      tuser.StrPreName = CGlobal.ConvertNullToString(User.FirstName);
      tuser.StrAdress = CGlobal.ConvertNullToString(User.Adress);
      tuser.NZip = User.Zip;
      tuser.StrTown = CGlobal.ConvertNullToString(User.Town);
      tuser.StrCountry = CGlobal.ConvertNullToString(User.Country);
      tuser.StrEmail = CGlobal.ConvertNullToString(User.Email);
      tuser.StrTel = CGlobal.ConvertNullToString(User.Tel);
      tuser.StrRemarks = CGlobal.ConvertNullToString(User.Remarks);
      tuser.NRole = User.Role;

      tuser.DtAdmissionDate = User.AdmissionDate;
      tuser.DtCheckOutDate = User.CheckOutDate;
      tuser.StrLoginName = CGlobal.ConvertNullToString(User.LoginName);
      tuser.StrPassword = CGlobal.ConvertNullToString(User.Password);
      tuser.BHasPaid = User.HasPaid;
      tuser.DtPaid = User.PaidDate;
      tuser.StrPersonAccessList = User.PersonAccessList;
      tuser.BActive = User.Active;
      tuser.BMustNotPaid = User.MustNotPaid;
    }

    /// <summary>
    /// Entity ins Model schreiben
    /// </summary>
    /// <param name="tuser"></param>
    /// <param name="User"></param>
    /// <param name="oSettings"></param>
    public void MappUserEntityToModel(ref CUser User, TUser tuser)
    {
      User.UserId = tuser.NUserId;
      User.PersonId = CGlobal.ConvertNullToString(tuser.StrPersonId);
      User.Salutation = CGlobal.ConvertNullToString(tuser.StrSalutation);
      User.Letter = CGlobal.ConvertNullToString(tuser.StrLetter);
      User.FirstName = CGlobal.ConvertNullToString(tuser.StrPreName);
      User.PreName = CGlobal.ConvertNullToString(tuser.StrName);
      User.Adress = CGlobal.ConvertNullToString(tuser.StrAdress);
      User.Zip = tuser.NZip;
      User.Town = CGlobal.ConvertNullToString(tuser.StrTown);
      User.Country = CGlobal.ConvertNullToString(tuser.StrCountry);
      User.Email = CGlobal.ConvertNullToString(tuser.StrEmail);
      User.Tel = CGlobal.ConvertNullToString(tuser.StrTel);
      User.Remarks = CGlobal.ConvertNullToString(tuser.StrRemarks);

      if (tuser.NRole < (int)CGlobal.ERole.Mitglied)
      {
        User.Role = (int)CGlobal.ERole.Mitglied;
      }
      else
      {
        User.Role = tuser.NRole;
      }


      if (tuser.DtAdmissionDate == null || tuser.DtAdmissionDate < new DateTime(1902, 1, 1))
      {
        User.AdmissionDate = new DateTime(1900, 1, 1);
      }
      else
      {
        User.AdmissionDate = new DateTime(tuser.DtAdmissionDate.Value.Year, tuser.DtAdmissionDate.Value.Month, tuser.DtAdmissionDate.Value.Day);
      }

      if (tuser.DtCheckOutDate == null || tuser.DtCheckOutDate < new DateTime(1902, 1, 1))
      {
        User.CheckOutDate = new DateTime(1900, 1, 1);
      }
      else
      {
        User.CheckOutDate = new DateTime(tuser.DtCheckOutDate.Value.Year, tuser.DtCheckOutDate.Value.Month, tuser.DtCheckOutDate.Value.Day);
      }


      //User.AdmissionDateDispaly = tuser.DtAdmissionDate == defaultTime ? "" : User.AdmissionDate.Value.ToShortDateString(); 
      //User.CheckOutDateDispaly = tuser.DtCheckOutDate == defaultTime ? "" : User.CheckOutDate.Value.ToShortDateString();

      User.LoginName = CGlobal.ConvertNullToString(tuser.StrLoginName);
      User.Password = CGlobal.ConvertNullToString(tuser.StrPassword);
      User.HasPaid = tuser.BHasPaid;
      //User.PaidDate=tuser.DtPaid;

      if (tuser.DtPaid == null || tuser.DtPaid < new DateTime(1902, 1, 1))
      {
        User.PaidDate = new DateTime(1900, 1, 1);
      }
      else
      {
        User.PaidDate = tuser.DtPaid;
      }

      User.PersonAccessList = tuser.StrPersonAccessList;
      User.Active = tuser.BActive;
      User.MustNotPaid = tuser.BMustNotPaid;
    }

    public void MappPersonPortraitEntityToModel(ref CPersonPortrait Portrait, TPersonPortrait tportrait)
    {
      Portrait.PersonPortraitID = tportrait.NPersonPortraitId;
      Portrait.PersonID = CGlobal.ConvertNullToString(tportrait.StrPersonId);
      Portrait.Title = CGlobal.ConvertNullToString(tportrait.StrTitle);
      Portrait.PdfName = CGlobal.ConvertNullToString(tportrait.StrPdfName);
      Portrait.Remarks = CGlobal.ConvertNullToString(tportrait.StrRemarks);
      Portrait.Update = tportrait.DtUpdate;
      Portrait.Create = tportrait.DtCreate;
      Portrait.Active = tportrait.NActive == CGlobal.DBTRUE ? true : false;
    }

    public void MappPersonPortraitModelEntity(ref TPersonPortrait tportrait, CPersonPortrait Portrait)
    {
      tportrait.StrPersonId = CGlobal.ConvertNullToString(Portrait.PersonID);
      tportrait.StrTitle = CGlobal.ConvertNullToString(Portrait.Title);
      tportrait.StrPdfName = CGlobal.ConvertNullToString(Portrait.PdfName);
      tportrait.StrRemarks = CGlobal.ConvertNullToString(Portrait.Remarks);
      tportrait.DtUpdate = Portrait.Update;
      tportrait.DtCreate = tportrait.DtCreate;
      tportrait.NActive = Portrait.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
    }

    public void MappContentTemplateModelEntity(ref TContentTemplate table, CContentTemplate model)
    {
      table.StrTitle = CGlobal.ConvertNullToString(model.Title);
      table.StrSubTitle = CGlobal.ConvertNullToString(model.SubTitle);
      table.StrContent = CGlobal.ConvertNullToString(model.Content);
      table.NActive = model.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
      table.NType = (int)model.Type;
      table.NSortNo = model.SortNo;
    }

    public void MappContentTemplateEntityToModel(ref CContentTemplate model, TContentTemplate table)
    {
      model.ContentTemplateId = table.NContentTemplateId;
      model.Title = CGlobal.ConvertNullToString(table.StrTitle);
      model.SubTitle = CGlobal.ConvertNullToString(table.StrSubTitle);
      model.Content = CGlobal.ConvertNullToString(table.StrContent);
      model.Active = table.NActive == CGlobal.DBTRUE ? true : false;
      model.Type = (int)table.NType;
      model.SortNo = table.NSortNo == null ? 0 : table.NSortNo.Value;
    }

    /// <summary>
    /// Mapp ContentTemplate entity with links and images to model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="table"></param>
    /// <param name="tableLinks"></param>
    /// <param name="tableImages"></param>
    public void MappContentTemplateEntityToModel(ref CContentTemplate model, TContentTemplate table, List<TContentTemplateLink> tableLinks, List<TContentTemplateImage> tableImages)
    {
      model.ContentTemplateId = table.NContentTemplateId;
      model.Title = CGlobal.ConvertNullToString(table.StrTitle);
      model.SubTitle = CGlobal.ConvertNullToString(table.StrSubTitle);
      model.Content = CGlobal.ConvertNullToString(table.StrContent);
      model.Active = table.NActive == CGlobal.DBTRUE ? true : false;
      model.Type = (int)table.NType;
      model.SortNo = table.NSortNo == null ? 0 : table.NSortNo.Value;

      model.ContentTemplateLinks = new List<CContentTemplateLink>();
      foreach (TContentTemplateLink link in tableLinks)
      {
        CContentTemplateLink _link = new CContentTemplateLink();
        MappContentTemplateLinkEntityToModel(ref _link, link);
        model.ContentTemplateLinks.Add(_link);
      }

      model.ContentTemplateImages = new List<CContentTemplateImage>();
      foreach (TContentTemplateImage image in tableImages)
      {
        CContentTemplateImage _image = new CContentTemplateImage();
        MappContentTemplateImageEntityToModel(ref _image, image);
        model.ContentTemplateImages.Add(_image);
      }
    }

    public void MappContentTemplateLinkModelEntity(ref TContentTemplateLink table, CContentTemplateLink model)
    {
      table.NContentTemplateId = model.ContentTemplateId;
      table.StrTitle = CGlobal.ConvertNullToString(model.Title);
      table.StrSubTitle = CGlobal.ConvertNullToString(model.SubTitle);
      table.StrDescription = CGlobal.ConvertNullToString(model.Description);
      table.StrPersonId = CGlobal.ConvertNullToString(model.PersonId);
      table.ExternalLink = model.ExternalLink == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
      table.StrNavigationTo = CGlobal.ConvertNullToString(model.NavigationTo);
      table.NActive = model.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
      table.NSortNo = model.SortNo;
    }

    public void MappContentTemplateLinkEntityToModel(ref CContentTemplateLink model, TContentTemplateLink table)
    {
      model.ContentTemplateLinkId = table.NContentTemplateLinkId;
      model.ContentTemplateId = table.NContentTemplateId;
      model.Title = CGlobal.ConvertNullToString(table.StrTitle);
      model.SubTitle = CGlobal.ConvertNullToString(table.StrSubTitle);
      model.Description = CGlobal.ConvertNullToString(table.StrDescription);
      model.PersonId = CGlobal.ConvertNullToString(table.StrPersonId);
      model.ExternalLink = table.ExternalLink == CGlobal.DBTRUE ? true : false;
      model.NavigationTo = CGlobal.ConvertNullToString(table.StrNavigationTo);
      model.Active = table.NActive == CGlobal.DBTRUE ? true : false;
      model.SortNo = table.NSortNo == null ? 0 : table.NSortNo.Value;
    }

    public void MappContentTemplateImageModelEntity(ref TContentTemplateImage table, CContentTemplateImage model)
    {
      table.NContentTemplateId = model.ContentTemplateId;
      table.StrTitle = CGlobal.ConvertNullToString(model.Title);
      table.StrSubTitle = CGlobal.ConvertNullToString(model.SubTitle);
      table.StrDescription = CGlobal.ConvertNullToString(model.Description);
      table.StrImageName = CGlobal.ConvertNullToString(model.ImageName);
      table.StrImageOriginalName = CGlobal.ConvertNullToString(model.ImageOriginalName);
      table.NActive = model.Active == true ? CGlobal.DBTRUE : CGlobal.DBFALSE;
      table.NSortNo = model.SortNo;
    }

    public void MappContentTemplateImageEntityToModel(ref CContentTemplateImage model, TContentTemplateImage table)
    {
      model.ContentTemplateImageId = table.NContentTemplateImageId;
      model.ContentTemplateId = table.NContentTemplateId;
      model.Title = CGlobal.ConvertNullToString(table.StrTitle);
      model.SubTitle = CGlobal.ConvertNullToString(table.StrSubTitle);
      model.Description = CGlobal.ConvertNullToString(table.StrDescription);
      model.ImageOriginalName = CGlobal.ConvertNullToString(table.StrImageOriginalName);
      model.ImageName = CGlobal.ConvertNullToString(table.StrImageName);
      model.Active = table.NActive == CGlobal.DBTRUE ? true : false;
      model.SortNo = table.NSortNo == null ? 0 : table.NSortNo.Value;

    }


    /// <summary>
    /// Verwandtschaftsgrad Model to Entity
    /// </summary>
    /// <param name="table"></param>
    /// <param name="model"></param>
    public void MappKinshipConnectionModelEntity(ref TKinshipConnection table, CKinshipConnection model)
    {
      table.StrParentId = CGlobal.ConvertNullToString(model.ParentId);
      table.StrPersonId = CGlobal.ConvertNullToString(model.PersonId);
    }

    /// <summary>
    /// Verwandtschaftsgrad Entity to Model
    /// </summary>
    /// <param name="model"></param>
    /// <param name="table"></param>
    public void MappKinshipConnectionEntityToModel(ref CKinshipConnection model, TKinshipConnection table)
    {
      model.KinshipConnectionId = table.NKinshipConnectionId;
      model.ParentId = CGlobal.ConvertNullToString(table.StrParentId);
      model.PersonId = CGlobal.ConvertNullToString(table.StrPersonId);
    }

    /// <summary>
    /// Personengruppenbilder Entity to Model 
    /// </summary>
    /// <param name="oImage"></param>
    /// <param name="t"></param>
    private void MappImagePersonEntityToModel(ref CImagePerson oImage, TImagePerson t)
    {
      oImage.Id = t.Id;
      oImage.ImagePath = CGlobal.ConvertNullToString(t.ImagePath);
      oImage.FileName = CGlobal.ConvertNullToString(t.FileName);
      oImage.OriginalFileName = CGlobal.ConvertNullToString(t.OriginalFileName);
      oImage.Title = CGlobal.ConvertNullToString(t.Title);
      oImage.Description = CGlobal.ConvertNullToString(t.Description);
      oImage.PositionsCount = t.PositionsCount;
      oImage.Add_Date = t.AddDate.Value;
      oImage.Active = t.Active.Value;
      oImage.InProgress = t.InProgress.Value;
      oImage.IsArchivated = t.IsArchivated.Value;
      oImage.IsExported = t.IsExported.Value;
      oImage.SourceDescription = t.SourceDescription;
      oImage.SourceImageFileName = t.SourceImageFileName;
    }

    /// <summary>
    /// Personengruppenbilder Model to Entity
    /// </summary>
    /// <param name="oImage"></param>
    /// <param name="t"></param>
    private void MappImagePersonModelToEntity(CImagePerson oImage, ref TImagePerson t)
    {
      t.ImagePath = CGlobal.ConvertNullToString(Convert.ToString(oImage.ImagePath));
      t.FileName = CGlobal.ConvertNullToString(Convert.ToString(oImage.FileName));
      t.OriginalFileName = CGlobal.ConvertNullToString(Convert.ToString(oImage.OriginalFileName));
      t.Title = CGlobal.ConvertNullToString(Convert.ToString(oImage.Title));
      t.Description = CGlobal.ConvertNullToString(Convert.ToString(oImage.Description));
      t.PositionsCount = oImage.PositionsCount;
      t.AddDate = oImage.Add_Date;
      t.Active = oImage.Active;
      t.InProgress = oImage.InProgress;
      t.IsArchivated = oImage.IsArchivated;
      t.IsExported = oImage.IsExported;
      t.SourceDescription = CGlobal.ConvertNullToString(Convert.ToString(oImage.SourceDescription));
      t.SourceImageFileName = CGlobal.ConvertNullToString(Convert.ToString(oImage.SourceImageFileName));
    }

    /// <summary>
    /// Personengruppenbildpositionen Entity to Model
    /// </summary>
    /// <param name="oPosition"></param>
    /// <param name="t"></param>
    private void MappImagePersonPositionEntityToModel(ref CImagePersonPosition oPosition, TImagePersonsPosition t)
    {
      oPosition.IdPersonPosition = t.Id;
      oPosition.IdImagePerson = t.IdImagePerson;
      oPosition.Pos = t.Pos;
      oPosition.PersonName = CGlobal.ConvertNullToString(t.Name);
      oPosition.PersonPreName = CGlobal.ConvertNullToString(t.PreName);
      oPosition.PersonAddress = CGlobal.ConvertNullToString(t.Address);
      oPosition.PersonHouseNo = CGlobal.ConvertNullToString(t.HouseNo);
      oPosition.PersonZip = CGlobal.ConvertNullToString(t.Zip);
      oPosition.PersonCountry = CGlobal.ConvertNullToString(t.Country);
      oPosition.PersonTown = CGlobal.ConvertNullToString(t.Town);
      oPosition.PersonBirthYear = t.BirthYear;
      oPosition.ReferencePersonId = CGlobal.ConvertNullToString(t.ReferencePersonId);
      oPosition.PersonDescription = CGlobal.ConvertNullToString(t.Description);
      oPosition.EditContactData = CGlobal.ConvertNullToString(t.EditContactData);
      oPosition.EditEmail = CGlobal.ConvertNullToString(t.EditEmail);
      oPosition.Person_Add_Date = t.AddDate;
      oPosition.Person_Upd_Date = t.UpdDate;
      oPosition.PersonFinish = t.Finish;
      oPosition.PersonActive = t.Active;
      oPosition.PersonComplete = (oPosition.PersonPreName.Length > 0) ? (oPosition.PersonPreName) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonName.Length > 0) ? (" | " + oPosition.PersonName) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonBirthYear > 0) ? (" | Jg. " + oPosition.PersonBirthYear) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonAddress.Length > 0) ? (" | " + oPosition.PersonAddress) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonHouseNo.Length > 0) ? (" | Nr. " + oPosition.PersonHouseNo) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonZip.Length > 0) ? (" | " + oPosition.PersonZip) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonTown.Length > 0) ? (" | " + oPosition.PersonTown) : (string.Empty);
      oPosition.PersonComplete += (oPosition.PersonCountry.Length > 0) ? (" | " + oPosition.PersonCountry) : (string.Empty);
    }

    /// <summary>
    /// Personengruppenbildpositionen Model to Entity
    /// </summary>
    /// <param name="oPosition"></param>
    /// <param name="t"></param>
    private void MappImagePersonPositionModelToEntity(CImagePersonPosition oPosition, ref TImagePersonsPosition t)
    {
      t.Id = oPosition.IdPersonPosition;
      t.IdImagePerson = oPosition.IdImagePerson;
      t.Pos = oPosition.Pos;
      t.Name = CGlobal.ConvertNullToString(oPosition.PersonName);
      t.PreName = CGlobal.ConvertNullToString(oPosition.PersonPreName);
      t.Address = CGlobal.ConvertNullToString(oPosition.PersonAddress);
      t.HouseNo = CGlobal.ConvertNullToString(oPosition.PersonHouseNo);
      t.Zip = CGlobal.ConvertNullToString(oPosition.PersonZip);
      t.Country = CGlobal.ConvertNullToString(oPosition.PersonCountry);
      t.Town = CGlobal.ConvertNullToString(oPosition.PersonTown);
      t.BirthYear = oPosition.PersonBirthYear;
      t.ReferencePersonId = CGlobal.ConvertNullToString(oPosition.ReferencePersonId);
      t.Description = CGlobal.ConvertNullToString(oPosition.PersonDescription);
      t.EditContactData = CGlobal.ConvertNullToString(oPosition.EditContactData);
      t.EditEmail = CGlobal.ConvertNullToString(oPosition.EditEmail);
      t.AddDate = oPosition.Person_Add_Date;
      t.UpdDate = oPosition.Person_Upd_Date;
      t.Finish = oPosition.PersonFinish;
      t.Active = oPosition.PersonActive;
    }


    #endregion

    /// <summary>
    /// Bildnamen nachziehen, falls sich der Namen geaendert hat.
    /// </summary>
    /// <param name="person"></param>
    private void ChangeImagePersonNameByChangedName(CPerson person, CSettings oSettings)
    {
      string imageDir = string.Empty;
      string imageGroupType = string.Empty;
      string imageFilter = string.Empty;
      string fileName = string.Empty;
      string path = string.Empty;
      DirectoryInfo di;
      FileInfo[] TXTFiles;

      imageGroupType = "_PE_01.jpg";
      imageFilter = "*.jpg";

      imageDir = "48x60";
      fileName = person.PersonID.Replace("I", "") + "_" + person.FirstName + "_" + person.FamilyName + imageGroupType;
      path = Path.Combine(Path.Combine(Path.Combine(oSettings.PhysicalImagePath), imageDir), fileName);
      di = new DirectoryInfo(Path.Combine(oSettings.PhysicalImagePath, imageDir));
      TXTFiles = di.GetFiles(person.PersonID.Replace("I", "") + imageFilter);
      if (TXTFiles.Length > 0)
      {
        System.IO.File.Move(Path.Combine(Path.Combine(oSettings.PhysicalImagePath, imageDir), TXTFiles[0].Name), path);
      }

      imageDir = "80x100";
      fileName = person.PersonID.Replace("I", "") + "_" + person.FirstName + "_" + person.FamilyName + imageGroupType;
      path = Path.Combine(Path.Combine(Path.Combine(oSettings.PhysicalImagePath), imageDir), fileName);
      di = new DirectoryInfo(Path.Combine(oSettings.PhysicalImagePath, imageDir));
      TXTFiles = di.GetFiles(person.PersonID.Replace("I", "") + imageFilter);
      if (TXTFiles.Length > 0)
      {
        System.IO.File.Move(Path.Combine(Path.Combine(oSettings.PhysicalImagePath, imageDir), TXTFiles[0].Name), path);
      }

      imageDir = "160x200";
      fileName = person.PersonID.Replace("I", "") + "_" + person.FirstName + "_" + person.FamilyName + imageGroupType;
      path = Path.Combine(Path.Combine(Path.Combine(oSettings.PhysicalImagePath), imageDir), fileName);
      di = new DirectoryInfo(Path.Combine(oSettings.PhysicalImagePath, imageDir));
      TXTFiles = di.GetFiles(person.PersonID.Replace("I", "") + imageFilter);
      if (TXTFiles.Length > 0)
      {
        System.IO.File.Move(Path.Combine(Path.Combine(oSettings.PhysicalImagePath, imageDir), TXTFiles[0].Name), path);
      }
    }
    /// <summary>
    /// Ist mindestens ein Elternteil bekannt
    /// </summary>
    /// <param name="Person"></param>
    /// <returns></returns>
    private Int32 ExistParents(CPerson Person)
    {

      Int32 nExist = CGlobal.DBFALSE;

      if (Person.FatherID != "-1")
      {
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == Person.FatherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      else if (Person.MotherID != "-1")
      {
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrPersonId == Person.MotherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      return nExist;
    }

    private Int32 ExistParents(CPerson Person, List<TPerson> tpersons)
    {

      Int32 nExist = CGlobal.DBFALSE;

      if (Person.FatherID != "-1")
      {
        TPerson tperson = tpersons.FirstOrDefault(t => t.StrPersonId == Person.FatherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      else if (Person.MotherID != "-1")
      {
        TPerson tperson = tpersons.FirstOrDefault(t => t.StrPersonId == Person.MotherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      return nExist;
    }

    

    /// <summary>
    /// Gibt es Geschwister
    /// </summary>
    /// <param name="Person"></param>
    /// <returns></returns>
    private Int32 ExistSibling(CPerson Person)
    {

      Int32 nExist = CGlobal.DBFALSE;

      if (Person.FatherID != "-1")
      {
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrFatherId == Person.FatherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      else if (Person.MotherID != "-1")
      {
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrMotherId == Person.MotherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      return nExist;
    }

    private Int32 ExistSibling(CPerson Person, List<TPerson> tpersons)
    {

      Int32 nExist = CGlobal.DBFALSE;

      if (Person.FatherID != "-1")
      {
        TPerson tperson = tpersons.FirstOrDefault(t => t.StrFatherId == Person.FatherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      else if (Person.MotherID != "-1")
      {
        TPerson tperson = tpersons.FirstOrDefault(t => t.StrMotherId == Person.MotherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      return nExist;
    }

    /// <summary>
    /// Gibt es Kinder
    /// </summary>
    /// <param name="Person"></param>
    /// <returns></returns>
    private Int32 ExistChildrens(CPerson Person)
    {

      Int32 nExist = CGlobal.DBFALSE;

      if (Person.Sex == "M")
      {
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrFatherId == Person.PersonID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      else if (Person.Sex == "F")
      {
        TPerson tperson = db.TPersons.FirstOrDefault(t => t.StrMotherId == Person.MotherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      return nExist;
    }

    private Int32 ExistChildrens(CPerson Person, List<TPerson> tpersons)
    {

      Int32 nExist = CGlobal.DBFALSE;

      if (Person.Sex == "M")
      {
        TPerson tperson = tpersons.FirstOrDefault(t => t.StrFatherId == Person.PersonID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      else if (Person.Sex == "F")
      {
        TPerson tperson = tpersons.FirstOrDefault(t => t.StrMotherId == Person.MotherID);
        if (tperson != null)
        {
          nExist = CGlobal.DBTRUE;
        }
      }
      return nExist;
    }
  }
}