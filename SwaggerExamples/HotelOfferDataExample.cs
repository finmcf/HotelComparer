using HotelComparer.Models;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

public class HotelOfferDataExample : IExamplesProvider<HotelOfferData>
{
    public HotelOfferData GetExamples()
    {
        return new HotelOfferData
        {
            Hotel = new HotelInfo
            {
                HotelId = "MCLONMAR",
                Name = "LONDON MARRIOTT MARBLE ARCH",
                ChainCode = "MC",
                CityCode = "LON",
                Latitude = 51.51587,
                Longitude = -0.16413
            },
            Offers = new List<HotelOffer>
            {
                new HotelOffer
                {
                    Id = "SEO1QJGD7Y",
                    CheckInDate = DateTime.Parse("2023-10-27T00:00:00"),
                    CheckOutDate = DateTime.Parse("2023-10-28T00:00:00"),
                    RateCode = "S9R",
                    Room = new HotelRoom
                    {
                        Type = "XMI",
                        TypeEstimated = new TypeEstimated
                        {
                            Category = "DELUXE_ROOM",
                            Beds = 1,
                            BedType = "KING"
                        },
                        Description = new RoomDescription
                        {
                            Text = "Marriott Senior Discount, 62 years and older valid ID required\nDeluxe Room, 1 King, 22sqm/237sqft,\nLiving/sitting area, Wireless internet, for a",
                            Lang = "EN"
                        }
                    },
                    Guests = new GuestInfo
                    {
                        Adults = 1
                    },
                    Price = new HotelPrice
                    {
                        Currency = "GBP",
                        Base = "338.641272",
                        Total = "338.641272",
                        Variations = new PriceVariations
                        {
                            Average = new AveragePrice { Base = "338.641272" },
                            Changes = new List<PriceChange>
                            {
                                new PriceChange
                                {
                                    StartDate = DateTime.Parse("2023-10-27T00:00:00"),
                                    EndDate = DateTime.Parse("2023-10-28T00:00:00"),
                                    Total = "338.641272"
                                }
                            }
                        }
                    },
                    Policies = new PolicyInfo
                    {
                        Cancellations = new List<CancellationPolicy>
                        {
                            new CancellationPolicy
                            {
                                Amount = "338.641272",
                                Deadline = DateTime.Parse("2023-10-26T23:59:00+01:00")
                            }
                        }
                    },
                    Self = "https://test.api.amadeus.com/v3/shopping/hotel-offers/SEO1QJGD7Y?lang=EN"
                },
                new HotelOffer
{
    Id = "LMR3NV198N",
    CheckInDate = DateTime.Parse("2023-10-27T00:00:00"),
    CheckOutDate = DateTime.Parse("2023-10-29T00:00:00"),
    RateCode = "S9R",
    Room = new HotelRoom
    {
        Type = "XMI",
        TypeEstimated = new TypeEstimated
        {
            Category = "DELUXE_ROOM",
            Beds = 1,
            BedType = "KING"
        },
        Description = new RoomDescription
        {
            Text = "Marriott Senior Discount, 62 years and older valid ID required\nDeluxe Room, 1 King, 22sqm/237sqft,\nLiving/sitting area, Wireless internet, for a",
            Lang = "EN"
        }
    },
    Guests = new GuestInfo
    {
        Adults = 1
    },
    Price = new HotelPrice
    {
        Currency = "GBP",
        Base = "648.152112",
        Total = "648.152112",
        Variations = new PriceVariations
        {
            Average = new AveragePrice { Base = "324.076056" },
            Changes = new List<PriceChange>
            {
                new PriceChange
                {
                    StartDate = DateTime.Parse("2023-10-27T00:00:00"),
                    EndDate = DateTime.Parse("2023-10-28T00:00:00"),
                    Total = "338.641272"
                },
                new PriceChange
                {
                    StartDate = DateTime.Parse("2023-10-28T00:00:00"),
                    EndDate = DateTime.Parse("2023-10-29T00:00:00"),
                    Total = "309.51084"
                }
            }
        }
    },
    Policies = new PolicyInfo
    {
        Cancellations = new List<CancellationPolicy>
        {
            new CancellationPolicy
            {
                Amount = "338.641272",
                Deadline = DateTime.Parse("2023-10-26T23:59:00+01:00")
            }
        }
    },
    Self = "https://test.api.amadeus.com/v3/shopping/hotel-offers/LMR3NV198N?lang=EN"
},
new HotelOffer
{
    Id = "91F0JT7DN1",
    CheckInDate = DateTime.Parse("2023-10-28T00:00:00"),
    CheckOutDate = DateTime.Parse("2023-10-29T00:00:00"),
    RateCode = "S9R",
    Room = new HotelRoom
    {
        Type = "XMI",
        TypeEstimated = new TypeEstimated
        {
            Category = "DELUXE_ROOM",
            Beds = 1,
            BedType = "KING"
        },
        Description = new RoomDescription
        {
            Text = "Marriott Senior Discount, 62 years and older valid ID required\nDeluxe Room, 1 King, 22sqm/237sqft,\nLiving/sitting area, Wireless internet, for a",
            Lang = "EN"
        }
    },
    Guests = new GuestInfo
    {
        Adults = 1
    },
    Price = new HotelPrice
    {
        Currency = "GBP",
        Base = "309.51084",
        Total = "309.51084",
        Variations = new PriceVariations
        {
            Average = new AveragePrice { Base = "309.51084" },
            Changes = new List<PriceChange>
            {
                new PriceChange
                {
                    StartDate = DateTime.Parse("2023-10-28T00:00:00"),
                    EndDate = DateTime.Parse("2023-10-29T00:00:00"),
                    Total = "309.51084"
                }
            }
        }
    },
    Policies = new PolicyInfo
    {
        Cancellations = new List<CancellationPolicy>
        {
            new CancellationPolicy
            {
                Amount = "309.51084",
                Deadline = DateTime.Parse("2023-10-27T23:59:00+01:00")
            }
        }
    },
    Self = "https://test.api.amadeus.com/v3/shopping/hotel-offers/91F0JT7DN1?lang=EN"
}

            },
            CheapestCombination = 648.152112,
            CheapestOfferIds = new List<string> { "LMR3NV198N" },

            Self = "https://test.api.amadeus.com/v3/shopping/hotel-offers?hotelIds=MCLONMAR&adults=1&boardType=ROOM_ONLY&checkInDate=2023-10-27&checkOutDate=2023-10-28&countryOfResidence=US&currency=USD&lang=EN&paymentPolicy=NONE&roomQuantity=1"
        };
    }
}