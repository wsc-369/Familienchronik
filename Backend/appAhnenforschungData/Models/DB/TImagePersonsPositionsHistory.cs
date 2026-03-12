using System;
using System.Collections.Generic;

#nullable disable

namespace appAhnenforschungData.Models.DB
{
    public partial class TImagePersonsPositionsHistory
    {
        public int Id { get; set; }
        public int IdImagePersonPosition { get; set; }
        public int IdImagePerson { get; set; }
        public int Pos { get; set; }
        public string Name { get; set; }
        public string PreName { get; set; }
        public string Address { get; set; }
        public string HouseNo { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public int BirthYear { get; set; }
        public string ReferencePersonId { get; set; }
        public string ImageDescription { get; set; }
        public string PersonDescription { get; set; }
        public string EditContactData { get; set; }
        public string EditEmail { get; set; }
        public bool Finish { get; set; }
        public DateTime AddDate { get; set; }
        public DateTime UpdDate { get; set; }
        public bool Active { get; set; }
        public DateTime AddHistoryDate { get; set; }
    }
}
