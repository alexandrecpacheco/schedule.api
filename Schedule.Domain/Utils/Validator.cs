using System;

namespace Schedule.Domain.Utils
{
    public static class Validator
    {
        public static bool ValidateClosedHours(DateTime startAt, DateTime endAt)
        {
            const string zero = "00";
            var start = startAt.ToString("mm");
            var end = endAt.ToString("mm");

            return (start == zero && end == zero);
        }

        public static bool ValidateDates(DateTime startAt, DateTime endAt)
        {
            var now = DateTime.Now;
            return startAt < endAt && startAt > now && now < endAt;
        }
    }
}
