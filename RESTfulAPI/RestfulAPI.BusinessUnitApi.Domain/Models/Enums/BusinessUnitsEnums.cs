namespace RestfulAPI.BusinessUnitApi.Domain.Models.Enums
{
    public class BusinessUnitsEnums
    {
        public enum PeriodCode
        {
            Month = 0
        }

        public enum TypeOfTreshold
        {
            BUNDLE = 1,
            QUOTA = 2,
            METERING = 3,
            FUPMETER = 4
        }

        public enum UnitType
        {
            MINUTES = 1,
            SMS,
            GB,
            MB,
            kB,
            USD,
            MONETARY
        }
    }
}
