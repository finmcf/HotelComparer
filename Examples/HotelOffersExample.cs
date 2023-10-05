namespace HotelComparer.Examples
{
    public class HotelOffersExample
    {
        public void DisplayHotelOffers()
        {
            string jsonData = @"[
  {
    ""hotel"": {
      ""type"": null,
      ""hotelId"": ""MCLONGHM"",
      ""chainCode"": ""MC"",
      ""dupeId"": null,
      ""name"": ""JW Marriott Grosvenor House London"",
      ""cityCode"": ""LON"",
      ""latitude"": 51.50988,
      ""longitude"": -0.15509
    },
    ""offers"": [
      {
        ""id"": ""CU6CPZYL9S"",
        ""checkInDate"": ""2023-10-26T00:00:00"",
        ""checkOutDate"": ""2023-10-27T00:00:00"",
        ""rateCode"": ""RAC"",
        ""rateFamilyEstimated"": null,
        ""room"": {
          ""type"": ""AP7"",
          ""typeEstimated"": {
            ""category"": ""SUPERIOR_ROOM"",
            ""beds"": 1,
            ""bedType"": ""DOUBLE""
          },
          ""description"": {
            ""text"": ""Prepay Non-refundable Non-changeable, prepay in full\nSuperior King Room, 1 King,\n23sqm/247sqft-35sqm/377sqft, Wireless"",
            ""lang"": ""EN""
          }
        },
        ""guests"": {
          ""adults"": 1
        },
        ""price"": {
          ""currency"": ""GBP"",
          ""base"": ""558.93128"",
          ""total"": ""558.93128"",
          ""variations"": {
            ""average"": {
              ""base"": ""558.93128""
            },
            ""changes"": [
              {
                ""startDate"": ""2023-10-26T00:00:00"",
                ""endDate"": ""2023-10-27T00:00:00"",
                ""total"": ""558.93128""
              }
            ]
          }
        },
        ""policies"": {
          ""cancellations"": [
            {
              ""amount"": ""0"",
              ""deadline"": ""0001-01-01T00:00:00""
            }
          ],
          ""paymentType"": ""deposit""
        },
        ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers/CU6CPZYL9S?lang=EN""
      },
      {
        ""id"": ""O3CGUZYA4J"",
        ""checkInDate"": ""2023-10-26T00:00:00"",
        ""checkOutDate"": ""2023-10-28T00:00:00"",
        ""rateCode"": ""RAC"",
        ""rateFamilyEstimated"": null,
        ""room"": {
          ""type"": ""AP7"",
          ""typeEstimated"": {
            ""category"": ""DELUXE_ROOM"",
            ""beds"": 1,
            ""bedType"": ""DOUBLE""
          },
          ""description"": {
            ""text"": ""Prepay Non-refundable Non-changeable, prepay in full\nDeluxe Queen Room, 1 Queen(s),\n20sqm/215sqft-29sqm/312sqft, Wireless"",
            ""lang"": ""EN""
          }
        },
        ""guests"": {
          ""adults"": 1
        },
        ""price"": {
          ""currency"": ""GBP"",
          ""base"": ""952.6133120000001"",
          ""total"": ""952.6133120000001"",
          ""variations"": {
            ""average"": {
              ""base"": ""476.30665600000003""
            },
            ""changes"": [
              {
                ""startDate"": ""2023-10-26T00:00:00"",
                ""endDate"": ""2023-10-27T00:00:00"",
                ""total"": ""481.16692800000004""
              },
              {
                ""startDate"": ""2023-10-27T00:00:00"",
                ""endDate"": ""2023-10-28T00:00:00"",
                ""total"": ""471.446384""
              }
            ]
          }
        },
        ""policies"": {
          ""cancellations"": [
            {
              ""amount"": ""0"",
              ""deadline"": ""0001-01-01T00:00:00""
            }
          ],
          ""paymentType"": ""deposit""
        },
        ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers/O3CGUZYA4J?lang=EN""
      },
      {
        ""id"": ""MESGX5S1MG"",
        ""checkInDate"": ""2023-10-27T00:00:00"",
        ""checkOutDate"": ""2023-10-28T00:00:00"",
        ""rateCode"": ""RAC"",
        ""rateFamilyEstimated"": null,
        ""room"": {
          ""type"": ""AP7"",
          ""typeEstimated"": {
            ""category"": ""DELUXE_ROOM"",
            ""beds"": 1,
            ""bedType"": ""DOUBLE""
          },
          ""description"": {
            ""text"": ""Prepay Non-refundable Non-changeable, prepay in full\nDeluxe Queen Room, 1 Queen(s),\n20sqm/215sqft-29sqm/312sqft, Wireless"",
            ""lang"": ""EN""
          }
        },
        ""guests"": {
          ""adults"": 1
        },
        ""price"": {
          ""currency"": ""GBP"",
          ""base"": ""471.446384"",
          ""total"": ""471.446384"",
          ""variations"": {
            ""average"": {
              ""base"": ""471.446384""
            },
            ""changes"": [
              {
                ""startDate"": ""2023-10-27T00:00:00"",
                ""endDate"": ""2023-10-28T00:00:00"",
                ""total"": ""471.446384""
              }
            ]
          }
        },
        ""policies"": {
          ""cancellations"": [
            {
              ""amount"": ""0"",
              ""deadline"": ""0001-01-01T00:00:00""
            }
          ],
          ""paymentType"": ""deposit""
        },
        ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers/MESGX5S1MG?lang=EN""
      }
    ],
    ""cheapestCombination"": 952.6133120000001,
    ""cheapestOfferIds"": [
      ""O3CGUZYA4J""
    ],
    ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers?hotelIds=MCLONGHM&adults=1&boardType=ROOM_ONLY&checkInDate=2023-10-26&checkOutDate=2023-10-27&countryOfResidence=US&currency=USD&lang=EN&paymentPolicy=NONE&roomQuantity=1""
  },
  {
    ""hotel"": {
      ""type"": null,
      ""hotelId"": ""MCLONMAR"",
      ""chainCode"": ""MC"",
      ""dupeId"": null,
      ""name"": ""LONDON MARRIOTT MARBLE ARCH"",
      ""cityCode"": ""LON"",
      ""latitude"": 51.51587,
      ""longitude"": -0.16413
    },
    ""offers"": [
      {
        ""id"": ""DZF0V8NOY6"",
        ""checkInDate"": ""2023-10-26T00:00:00"",
        ""checkOutDate"": ""2023-10-27T00:00:00"",
        ""rateCode"": ""S9R"",
        ""rateFamilyEstimated"": null,
        ""room"": {
          ""type"": ""XMI"",
          ""typeEstimated"": {
            ""category"": ""DELUXE_ROOM"",
            ""beds"": 1,
            ""bedType"": ""KING""
          },
          ""description"": {
            ""text"": ""Marriott Senior Discount, 62 years and older valid ID required\nDeluxe Room, 1 King, 22sqm/237sqft,\nLiving/sitting area, Wireless internet, for a"",
            ""lang"": ""EN""
          }
        },
        ""guests"": {
          ""adults"": 1
        },
        ""price"": {
          ""currency"": ""GBP"",
          ""base"": ""407.04778"",
          ""total"": ""407.04778"",
          ""variations"": {
            ""average"": {
              ""base"": ""407.04778""
            },
            ""changes"": [
              {
                ""startDate"": ""2023-10-26T00:00:00"",
                ""endDate"": ""2023-10-27T00:00:00"",
                ""total"": ""407.04778""
              }
            ]
          }
        },
        ""policies"": {
          ""cancellations"": [
            {
              ""amount"": ""407.04778"",
              ""deadline"": ""2023-10-26T00:59:00+01:00""
            }
          ],
          ""paymentType"": null
        },
        ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers/DZF0V8NOY6?lang=EN""
      },
      {
        ""id"": ""L0UZ21262J"",
        ""checkInDate"": ""2023-10-26T00:00:00"",
        ""checkOutDate"": ""2023-10-28T00:00:00"",
        ""rateCode"": ""S9R"",
        ""rateFamilyEstimated"": null,
        ""room"": {
          ""type"": ""XMI"",
          ""typeEstimated"": {
            ""category"": ""SUPERIOR_ROOM"",
            ""beds"": 1,
            ""bedType"": ""KING""
          },
          ""description"": {
            ""text"": ""Marriott Senior Discount, 62 years and older valid ID required\nSuperior Room, 1 King, Sofa bed,\n28sqm/301sqft, Living/sitting area, Wireless"",
            ""lang"": ""EN""
          }
        },
        ""guests"": {
          ""adults"": 1
        },
        ""price"": {
          ""currency"": ""GBP"",
          ""base"": ""804.3750160000001"",
          ""total"": ""804.3750160000001"",
          ""variations"": {
            ""average"": {
              ""base"": ""402.18750800000004""
            },
            ""changes"": [
              {
                ""startDate"": ""2023-10-26T00:00:00"",
                ""endDate"": ""2023-10-27T00:00:00"",
                ""total"": ""465.37104400000004""
              },
              {
                ""startDate"": ""2023-10-27T00:00:00"",
                ""endDate"": ""2023-10-28T00:00:00"",
                ""total"": ""339.00397200000003""
              }
            ]
          }
        },
        ""policies"": {
          ""cancellations"": [
            {
              ""amount"": ""465.37104400000004"",
              ""deadline"": ""2023-10-26T00:59:00+01:00""
            }
          ],
          ""paymentType"": null
        },
        ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers/L0UZ21262J?lang=EN""
      },
      {
        ""id"": ""H0SH5M4CTR"",
        ""checkInDate"": ""2023-10-27T00:00:00"",
        ""checkOutDate"": ""2023-10-28T00:00:00"",
        ""rateCode"": ""S9R"",
        ""rateFamilyEstimated"": null,
        ""room"": {
          ""type"": ""XMI"",
          ""typeEstimated"": {
            ""category"": ""EXECUTIVE_ROOM"",
            ""beds"": 1,
            ""bedType"": ""KING""
          },
          ""description"": {
            ""text"": ""Marriott Senior Discount, 62 years and older valid ID required\nExecutive Room with Executive Benefits, 1\nKing, 22sqm/237sqft, Wireless internet,"",
            ""lang"": ""EN""
          }
        },
        ""guests"": {
          ""adults"": 1
        },
        ""price"": {
          ""currency"": ""GBP"",
          ""base"": ""402.18750800000004"",
          ""total"": ""402.18750800000004"",
          ""variations"": {
            ""average"": {
              ""base"": ""402.18750800000004""
            },
            ""changes"": [
              {
                ""startDate"": ""2023-10-27T00:00:00"",
                ""endDate"": ""2023-10-28T00:00:00"",
                ""total"": ""402.18750800000004""
              }
            ]
          }
        },
        ""policies"": {
          ""cancellations"": [
            {
              ""amount"": ""402.18750800000004"",
              ""deadline"": ""2023-10-27T00:59:00+01:00""
            }
          ],
          ""paymentType"": null
        },
        ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers/H0SH5M4CTR?lang=EN""
      }
    ],
    ""cheapestCombination"": 804.3750160000001,
    ""cheapestOfferIds"": [
      ""L0UZ21262J""
    ],
    ""self"": ""https://test.api.amadeus.com/v3/shopping/hotel-offers?hotelIds=MCLONMAR&adults=1&boardType=ROOM_ONLY&checkInDate=2023-10-26&checkOutDate=2023-10-27&countryOfResidence=US&currency=USD&lang=EN&paymentPolicy=NONE&roomQuantity=1""
  }
]";

            var hotelOffers = Newtonsoft.Json.Linq.JArray.Parse(jsonData);

            foreach (var offer in hotelOffers)
            {
                System.Console.WriteLine($"Hotel Name: {offer["hotel"]["name"]}");
                System.Console.WriteLine($"Price: {offer["offers"][0]["price"]["total"]}");
                System.Console.WriteLine("-----------------------------------");
            }
        }
    }
}
