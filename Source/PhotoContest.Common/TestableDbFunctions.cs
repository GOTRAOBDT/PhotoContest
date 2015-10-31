namespace PhotoContest.Common
{
    using System;

    public class TestableDbFunctions
    {
        [System.Data.Entity.DbFunction("Edm", "DiffMinutes")]
        public static int? DiffMinutes(DateTime? dateValue1, DateTime? dateValue2)
        {
            if (!dateValue1.HasValue || !dateValue2.HasValue)
            {
                return null;
            }

            return (int)(dateValue2.Value - dateValue1.Value).TotalMinutes;
        }
    }
}