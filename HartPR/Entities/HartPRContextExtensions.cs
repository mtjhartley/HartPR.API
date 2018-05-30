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
            context.Tournaments.RemoveRange(context.Tournaments);
            context.Sets.RemoveRange(context.Sets);
            context.Users.RemoveRange(context.Users);
            context.PlayerTournaments.RemoveRange(context.PlayerTournaments);
            //context.SaveChanges();

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
                     //Trueskill = 3264.96,
                     SggPlayerId = 13045,
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
                     //Trueskill = 4200.69,
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
                     //Trueskill = 2345.76,
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
                     //Trueskill = 1111.11,
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
                     //Trueskill = 833.21,
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
                     //Trueskill = 5000.00,
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
                     //Trueskill = 3321.76,
                     SggPlayerId = 252,
                     CreatedAt = new DateTimeOffset(new DateTime(2010, 7, 21)),
                     UpdatedAt = new DateTimeOffset(DateTime.Now)
                }
            };

            var tournaments = new List<Tournament>()
            {
                new Tournament()
                {
                    Id = new Guid("977974e7-1e4a-4305-8ace-70e8268f4164"),
                    Name = "Tony Town 2: Swag Me Out",
                    Website = "smashgg",
                    URL = "tony-town-2",
                    Date = new DateTimeOffset(new DateTime(2007, 4, 20)),
                    SggTournamentId = 201,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                    //todo refactor sets to be FLAT
                },
                new Tournament()
                {
                    Id = new Guid("d791d482-b0fb-4598-966a-4abf802253ee"),
                    Name = "The Dawg Pound",
                    Website = "smashgg",
                    URL = "the-dawg-pound",
                    Date = new DateTimeOffset(new DateTime(2010, 3, 21)),
                    SggTournamentId = 202,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
                new Tournament()
                {
                    Id = new Guid("02007fa8-2e7e-405e-b25b-a6ef4e65c7f1"),
                    Name = "Smash For Smiles",
                    Website = "smashgg",
                    URL = "smash-for-smiles",
                    Date = new DateTimeOffset(new DateTime(2015, 1, 05)),
                    SggTournamentId = 203,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now)
                },
                new Tournament()
                {
                    Id = new Guid("a09ee685-1917-4251-be85-09b76b563861"),
                    Name = "Shrekfest 3: I Came In Like A Shreking Ball",
                    Website = "smashgg",
                    URL = "shrekfest-3",
                    Date = new DateTimeOffset(new DateTime(2014, 11, 14)),
                    SggTournamentId = 204,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now)
                }
            };

            var sets = new List<Set>()
            {
                new Set()
                {
                    Id = new Guid("fa7ef1f3-5236-4042-8b32-bf82b98ddb46"),
                    WinnerId = new Guid("1325360c-8253-473a-a20f-55c269c20407"), //Moze
                    LoserId = new Guid("a1da1d8e-1988-4634-b538-a01709477b77"), //leahboo
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                    TournamentId = new Guid("977974e7-1e4a-4305-8ace-70e8268f4164")
                },
                new Set()
                {
                    Id = new Guid("a5297b02-f14b-429c-aec4-ef092922e48b"),
                    WinnerId = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"), //PwrUp!
                    LoserId = new Guid("1325360c-8253-473a-a20f-55c269c20407"), //Moze
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                    TournamentId = new Guid("a09ee685-1917-4251-be85-09b76b563861")
                },
                new Set()
                {
                    Id = new Guid("5b906519-822a-4739-b1ae-0a16e0d95ba4"),
                    WinnerId = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"), //PwrUp!
                    LoserId = new Guid("a1da1d8e-1988-4634-b538-a01709477b77"), //leahboo
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                    TournamentId = new Guid("a09ee685-1917-4251-be85-09b76b563861")
                },
                new Set()
                {
                    Id = new Guid("5c32180e-aab1-4fe6-bdff-c16851cd3aee"),
                    WinnerId = new Guid("a1da1d8e-1988-4634-b538-a01709477b77"), //leahboo
                    LoserId = new Guid("578359b7-1967-41d6-8b87-64ab7605587e"), //PwrUp!
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                    TournamentId = new Guid("d791d482-b0fb-4598-966a-4abf802253ee")
                }
            };

            var users = new List<User>()
            {
                new User()
                {
                    Id = new Guid("730cc3c7-a286-4563-b113-1fbe2993bf4d"),
                    FirstName = "Michael",
                    LastName = "Hartley",
                    Email = "mtjhartley@gmail.com",
                    Password = "unhashed password lol",
                    IsAdmin = true,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
                },
               new User()
               {
                    Id = new Guid("c4e02165-94f5-422f-8c1e-9284c4625e01"),
                    FirstName = "Mitch",
                    LastName = "Dzugan",
                    Email = "mitchdzugan@gmail.com",
                    Password = "AQAAAAEAACcQAAAAEBCySMALfrT8FbzSfoQGJBnH2hM2Qi1g2W/k+gexflDG1NTxZjfY50mwYWLV7+dUaw==",
                    IsAdmin = true,
                    CreatedAt = new DateTimeOffset(DateTime.Now),
                    UpdatedAt = new DateTimeOffset(DateTime.Now),
               },
            };

            //context.Players.AddRange(players);
            //context.Tournaments.AddRange(tournaments);
            //context.Sets.AddRange(sets);
            context.Users.AddRange(users);

            context.SaveChanges();
        }
    }
}
