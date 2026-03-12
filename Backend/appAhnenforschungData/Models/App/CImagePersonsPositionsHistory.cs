using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace appAhnenforschungData.Models.App
{
    [Serializable()]
    public class CImagePersonsPositionsHistory
    {
        public int Id { get; set; }
        public int IdPersonPosition { get; set; }
        public int IdImagePerson { get; set; }
        [Display(Name = "Pos.")]
        public int Pos { get; set; }
        [Display(Name = "Name")]
        public string PersonName { get; set; }
        [Display(Name = "Vorname")]
        public string PersonPreName { get; set; }
        [Display(Name = "Adresse")]
        public string PersonAddress { get; set; }
        [Display(Name = "Haus Nr.")]
        public string PersonHouseNo { get; set; }
        [Display(Name = "PLZ")]
        public string PersonZip { get; set; }
        [Display(Name = "Ort")]
        public string PersonCountry { get; set; }
        [Display(Name = "Land")]
        public string PersonTown { get; set; }
        [Display(Name = "Jahrgang")]
        public int PersonBirthYear { get; set; }
        [Display(Name = "Chronik Ident")]
        public string ReferencePersonId { get; set; }
        [Display(Name = "Berschreibung")]
        public string PersonDescription { get; set; }
        public string PersonComplete { get; set; }
        [Display(Name = "Kontaktdaten")]
        public string EditContactData { get; set; }
        [Display(Name = "eMail")]
        public string EditEmail { get; set; }
        [Display(Name = "Erstellt")]
        public DateTime Person_Add_Date { get; set; }
        [Display(Name = "Geändert")]
        public DateTime Person_Upd_Date { get; set; }
        [Display(Name = "Fix")]
        public bool PersonFinish { get; set; }
        [Display(Name = "Aktiv")]
        public bool PersonActive { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as CImagePersonPosition;

            if (other == null)
                return false;

            if (other.PersonName == null)
                other.PersonName = "";

            if (other.PersonPreName == null)
                other.PersonPreName = "";

            if (other.PersonAddress == null)
                other.PersonAddress = "";

            if (other.PersonHouseNo == null)
                other.PersonHouseNo = "";

            if (other.PersonZip == null)
                other.PersonZip = "";

            if (other.PersonCountry == null)
                other.PersonCountry = "";

            if (other.PersonTown == null)
                other.PersonTown = "";

            //if (other.PersonBirthYear == null)
            //  other.PersonBirthYear = 0;

            if (other.ReferencePersonId == null)
                other.ReferencePersonId = "";

            if (other.PersonDescription == null)
                other.PersonDescription = "";

            if (other.EditContactData == null)
                other.EditContactData = "";

            if (other.EditEmail == null)
                other.EditEmail = "";


            if (PersonName != other.PersonName
              || PersonPreName != other.PersonPreName
              || PersonAddress != other.PersonAddress
              || PersonHouseNo != other.PersonHouseNo
              || PersonZip != other.PersonZip
              || PersonCountry != other.PersonCountry
              || PersonTown != other.PersonTown
              || PersonBirthYear != other.PersonBirthYear
              || ReferencePersonId != other.ReferencePersonId
              || PersonDescription != other.PersonDescription
              || EditContactData != other.EditContactData
              || EditEmail != other.EditEmail
              || Person_Add_Date != other.Person_Add_Date
              //|| Person_Upd_Date != other.Person_Upd_Date 
              || PersonFinish != other.PersonFinish
              || PersonActive != other.PersonActive)
                return false;

            return true;
        }

        public override int GetHashCode() { return 0; }
    }
}
