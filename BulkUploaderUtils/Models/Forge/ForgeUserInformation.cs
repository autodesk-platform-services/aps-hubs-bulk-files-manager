namespace Data.Models.Forge
{

    public class ForgeUserInformation
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string emailId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public bool emailVerified { get; set; }
        public bool _2FaEnabled { get; set; }
        public string countryCode { get; set; }
        public string language { get; set; }
        public bool optin { get; set; }
        public string lastModified { get; set; }
        public Profileimages profileImages { get; set; }
        public Ldapinfo ldapInfo { get; set; }
        public object[] socialUserInfoList { get; set; }
        public string createdDate { get; set; }
        public Userprofessionalinfo userProfessionalInfo { get; set; }
        public string lastLoginDate { get; set; }
        public string eidmGuid { get; set; }
    }

    public class Profileimages
    {
        public string sizeX20 { get; set; }
        public string sizeX40 { get; set; }
        public string sizeX50 { get; set; }
        public string sizeX58 { get; set; }
        public string sizeX80 { get; set; }
        public string sizeX120 { get; set; }
        public string sizeX160 { get; set; }
        public string sizeX176 { get; set; }
        public string sizeX240 { get; set; }
        public string sizeX360 { get; set; }
    }

    public class Ldapinfo
    {
        public bool ldapEnabled { get; set; }
        public string domainName { get; set; }
    }

    public class Userprofessionalinfo
    {
        public string industry { get; set; }
        public string industryCode { get; set; }
    }

}