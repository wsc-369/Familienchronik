
export interface Address {
    id?: string;
    person: Person;
    personRefId?: string | undefined;
    street?: string | undefined;
    houseNr?: string | undefined;
    town?: string | undefined;
    zip?: string | undefined;
    country?: string | undefined;
    fullAddress?: string | undefined;
    orderNr?: number;
    description?: string | undefined;
    active?: boolean;
}

/* export interface LoginRequest {
    username?: string;
    password?: string;
}

export interface LoginResponse {
    accessToken: string;
    refreshToken: string;
    expiresIn: number;
    user: AuthUser;
}

export interface AuthUser {
    id: number;
    userName: string;
    email: string;
    roles: string[];
} */

export interface ForgotPasswordRequest {
    email: string;
}

export interface ContentTemplate {
    id: string;
    refContentTemplateId?: number;
    title: string;
    subTitle: string;
    content: string;
    sortNo: number;
    type: number;
    active: boolean;
    contentTemplateLinks: ContentTemplateLink[] ;
    contentTemplateImages: ContentTemplateImage[];
}

export interface ContentTemplateImage {
    id: string;
    contentTemplateId: string;
    title: string;
    subTitle: string;
    imageName: string;
    imageOriginalName: string;
    description: string;
    sortNo: number;
    active: boolean;
}

export interface ContentTemplateLink {
    id: string;
    contentTemplateId: string;
    title: string; 
    subTitle: string;
    isExternalLink: boolean;
    navigationTo: string;
    personId: string | null;
    description: string;
    sortNo: number;
    active: boolean;
    mediaLibraryDocuments: MediaLibraryDocument[];
}

export interface DialectWord {
    id: string;
    personId: string;
    title: string;
    description: string;
    voice: string;
    personFamilyName: string;
    personFirstname: string;
    active: boolean;
}

export interface MediaLibraryDocument {
    id: string;
    title: string;
    description: string;
    filePath: string;
    contentType: string;
    //uploadDate: Date;
    extractedText: string;
    keywords: string;
    keywordsJson: string;
    summary: string;
    formatedHtml: string;
    active: boolean;
    contentTemplateLink: ContentTemplateLink;
    //documentTopics: DocumentTopic[];
}

export interface Partner {
    personId: string;
    partnerId: string;
    person: Person;
    partnerPerson: Person;
    marriageDateTime: Date;
    divorceDateTime: Date;
    connectionRole: number;
    isCurrent: boolean;
    active: boolean;
}

export interface Person {
    Id: string;
    personId: string; // Referenz auf die Person, z.B. für die Verknüpfung mit einem Partner oder Elternteil 
    fatherId?: string; // Referenz auf den Vater, optional
    motherId?: string; // Referenz auf die Mutter, optional
    familyName?: string; // Nachname
    firstName?: string; // Vorname

    fullname?: string; // Vollständiger Name, z.B. "Nachname Vorname"
    bur?: string; // Einbürgerung     
    sex?: string; // Geschlecht

    //  Male = 1, // Männlich 
    // Female = 2, // Weiblich 
    // Divers = 3, // Für Personen, die sich nicht als männlich oder weiblich identifizieren
    // Intersex = 4, // für Personen, deren Geschlechtschromosomen oder anatomische Merkmale nicht dem traditionellen Geschlechtermodell entsprechen

    genderStatus: number // Geschlechtsstatus
    genderStatusName: string; // Geschlechtsstatus
    birthPlace?: string; // Geburtsort
    deathPlace?: string; // Sterbeort
    burPlace?: string; // Einbürgerungsort
    race?: string; // Stamm
    work?: string; // Beruf
    nameMerges?: string; // Name nach Heirat
    nickname?: string; // Rufname
    birthDate?: Date; // Geburtsdatum
    deathDate?: Date; // Sterbedatum
    burDate?: Date; // Einbürgerungsdatum
    active: boolean; // Aktivitätsstatus
}

export interface ParentChild {
    id: string;
    active: boolean;
    child: Person;
    parent: Person;
    parentRole: number;
}

export interface PersonPortrait {
    id: string;
    person: Person;
    title?: string;
    pdfName?: string;
    remarks?: string;
    createDate: Date;
    updateDate: Date;
    active: boolean;
}

export interface SearchResult {
    results: MediaLibraryDocument[];
    totalCount: number;
    pageIndex: number;
    pageSize: number;
}

export interface TypeheadPerson {
    familyName: string;
    firstName: string;
    birthDate: Date;
}

export interface Images {
    index: number;
    urlOriginal: string;
    urlLarge: string;
    urlSmall: string;
    urlThumb: string;
    title: string;
    subTitle: string;
    description: string;
    type: number;
    imageName: string;
    sortNo: number;
}

export interface Family {
    id: string;
    personId: string;
    tree: string;
    active: boolean;
}

export interface PersonRelation {
    basePersonId: string;
    persion: Person;
}

