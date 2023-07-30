using System;

namespace HotelPriceComparer.Models
{
    public class DateCombination
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public DateCombination(DateTime checkInDate, DateTime checkOutDate)
        {
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }
    }
}
