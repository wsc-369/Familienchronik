export  interface User {
  userId: number;
  personId: string;
  salutation: string;
  letter: string;
  firstName: string;
  preName: string;
  adress: string;
  zip: number;
    flag: string; // 👈 neues Feld für Flaggen
  town: string;
  country: string;
  email: string;
  tel: string;
  remarks: string;
  role: number;
  admissionDate: Date;
  checkOutDate: Date;
  admissionDateDispaly: string;
  checkOutDateDispaly: string;
  loginName: string;
  password: string;
  hasPaid: boolean;
  paidDate: Date;
  personAccessList: string;
  active: boolean;
  mustNotPaid: boolean;
  authdata: string;
  token: string;
}
