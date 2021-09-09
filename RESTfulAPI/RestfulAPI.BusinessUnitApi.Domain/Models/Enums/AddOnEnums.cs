namespace RestfulAPI.BusinessUnitApi.Domain.Models.Enums
{
    public class AddOnEnums
    {
        public enum EndType
        {
            IMMEDIATE = 1,
            BILLCYCLE = 2
        }

        public enum ServiceTypeCode
        {
            DEFAULT = 0,//this should be threated as invalid input
            VOICE = 1,
            SMS = 2,
            DATA = 3,
            QUOTA = 4
        }
    }
}
