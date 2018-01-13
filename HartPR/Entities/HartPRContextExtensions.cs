using System;
using System.Collections.Generic;

namespace HartPR.Entities
{
    public static class HartPRContextExtensions
    {
        public static void EnsureSeedDataForContext(this HartPRContext context)
        {
            // first, clear the database.  This ensures we can always start 
            // fresh with each demo.  Not advised for production environments, obviously :-)

            context.Players.RemoveRange(context.Players);
            context.SaveChanges();

            // init seed data
            var players = new List<Player>()
            {
                new Player()
                {
                     Id = new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"),
                     Tag = "Dempsey",
                     FirstName = "Michael",
                     LastName = "Hartley",
                     State = "WA",
                     Trueskill = 3264.96,
                     SggPlayerId = 200,
                     CreatedAt = new DateTimeOffset(new DateTime(2012, 9, 21)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
                new Player()
                {
                     Id = new Guid("76053df4-6687-4353-8937-b45556748abe"),
                     Tag = "Dz",
                     FirstName = "Mitch",
                     LastName = "Dzugan",
                     State = "CA",
                     Trueskill = 4200.69,
                     SggPlayerId = 98,
                     CreatedAt = new DateTimeOffset(new DateTime(2007, 6, 13)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)

                },
                new Player()
                {
                     Id = new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"),
                     Tag = "MILK$",
                     FirstName = "Gary",
                     LastName = "Mai",
                     State = "WA",
                     Trueskill = 2345.76,
                     SggPlayerId = 214,
                     CreatedAt = new DateTimeOffset(new DateTime(2007, 6, 14)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
                new Player()
                {
                     Id = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"),
                     Tag = "PwrUp!",
                     FirstName = "Alex",
                     LastName = "Wallin",
                     State = "WA",
                     Trueskill = 1111.11,
                     SggPlayerId = 229,
                     CreatedAt = new DateTimeOffset(new DateTime(2003, 6, 14)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
                new Player()
                {
                     Id = new Guid("f74d6899-9ed2-4137-9876-66b070553f8f"),
                     Tag = "DavidCanFly",
                     FirstName = "David",
                     LastName = "Tze",
                     State = "WA",
                     Trueskill = 833.21,
                     SggPlayerId = 194,
                     CreatedAt = new DateTimeOffset(new DateTime(2006, 2, 01)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)

                },
                new Player()
                {
                     Id = new Guid("a1da1d8e-1988-4634-b538-a01709477b77"),
                     Tag = "leahboo",
                     FirstName = "Leah",
                     LastName = "Doh",
                     State = "CA",
                     Trueskill = 5000.00,
                     SggPlayerId = 168,
                     CreatedAt = new DateTimeOffset(new DateTime(2013, 4, 01)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
                new Player()
                {
                     Id = new Guid("1325360c-8253-473a-a20f-55c269c20407"),
                     Tag = "Moze",
                     FirstName = "Tony",
                     LastName = "Salatino",
                     State = "AZ",
                     Trueskill = 3321.76,
                     SggPlayerId = 252,
                     CreatedAt = new DateTimeOffset(new DateTime(2010, 7, 21)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)
                }
            };

            context.Players.AddRange(players);
            context.SaveChanges();
        }
    }
}
