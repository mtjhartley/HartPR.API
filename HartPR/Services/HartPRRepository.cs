using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Entities;
using HartPR.Helpers;
using HartPR.Models;

namespace HartPR.Services
{
    public class HartPRRepository : IHartPRRepository
    {
        private HartPRContext _context;
        private IPropertyMappingService _propertyMappingService;

        public HartPRRepository(HartPRContext context,
            IPropertyMappingService propertyMappingService)
        {
            _context = context;
            _propertyMappingService = propertyMappingService;
        }

        #region Players
        public Player GetPlayer(Guid playerId)
        {
            return _context.Players.FirstOrDefault(a => a.Id == playerId);
        }
        public PagedList<Player> GetPlayers(PlayersResourceParameters playersResourceParameters)
        {
            //var collectionBeforePaging = _context.Players
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();
            IQueryable<Player> collectionBeforePaging =
                        _context.Players.ApplySort(playersResourceParameters.OrderBy,
                        _propertyMappingService.GetPropertyMapping<PlayerDto, Player>());


            if (!string.IsNullOrEmpty(playersResourceParameters.State))
            {
                // trim & ignore casing
                var stateForWhereClause = playersResourceParameters.State
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.State.ToLowerInvariant() == stateForWhereClause);
            }

            if (!string.IsNullOrEmpty(playersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = playersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.State.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Tag.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Player>.Create(collectionBeforePaging,
                playersResourceParameters.PageNumber,
                playersResourceParameters.PageSize);
        }

        public IEnumerable<Player> GetPlayers(IEnumerable<Guid> playerIds)
        {
            return _context.Players.Where(p => playerIds.Contains(p.Id))
                .OrderBy(p => p.FirstName)
                .OrderBy(p => p.LastName)
                .ToList();
        }

        public void AddPlayer(Player player)
        {
            player.Id = Guid.NewGuid();
            _context.Players.Add(player);
            // the repository fills the id (instead of using identity columns)
        }

        public void AddPlayer(Player player, Guid playerId)
        {
            //uses the Upserted Id.
            _context.Players.Add(player);
        }


        public void DeletePlayer(Player player)
        {
            _context.Players.Remove(player);
        }

        public void UpdatePlayer(Player player)
        {
            //no code in this implementation
        }

        public bool PlayerExists(Guid playerId)
        {
            return _context.Players.Any(p => p.Id == playerId);
        }

        public IEnumerable<Tournament> GetTournamentsForPlayer(Guid playerId, int gameNum)
        {
            var tournaments = (from s in _context.Sets
                               join tournament in _context.Tournaments on s.TournamentId equals tournament.Id
                               join games in _context.Games on tournament.GameId equals games.Id
                               where (s.WinnerId == playerId || s.LoserId == playerId)
                               && games.Enum == gameNum
                               select tournament)
                              .Distinct()
                              .OrderByDescending(t => t.Date)
                              .ToList();
            return tournaments;
        }

        #endregion

        #region Tournaments 
        public PagedList<Tournament> GetTournaments(
            TournamentsResourceParameters tournamentResourceParameters)
        {
            //var collectionBeforePaging = _context.Players
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();

            var collectionBeforePaging =
                _context.Tournaments.ApplySort(tournamentResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<TournamentDto, Tournament>());

            //if (!string.IsNullOrEmpty(tournamentResourceParameters.State))
            //{
            //    // trim & ignore casing
            //    var stateForWhereClause = playersResourceParameters.State
            //        .Trim().ToLowerInvariant();
            //    collectionBeforePaging = collectionBeforePaging
            //        .Where(a => a.State.ToLowerInvariant() == stateForWhereClause);
            //}

            if (!string.IsNullOrEmpty(tournamentResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = tournamentResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Tournament>.Create(collectionBeforePaging,
                tournamentResourceParameters.PageNumber,
                tournamentResourceParameters.PageSize);
        }

        public PagedList<Tournament> GetTournamentsForGame(TournamentsResourceParameters tournamentResourceParameters, int gameNum)
        {
            //var collectionBeforePaging = _context.Players
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();

            var collectionBeforePaging =
                _context.Tournaments.ApplySort(tournamentResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<TournamentDto, Tournament>());

            //if (!string.IsNullOrEmpty(tournamentResourceParameters.State))
            //{
            //    // trim & ignore casing
            //    var stateForWhereClause = playersResourceParameters.State
            //        .Trim().ToLowerInvariant();
            //    collectionBeforePaging = collectionBeforePaging
            //        .Where(a => a.State.ToLowerInvariant() == stateForWhereClause);
            //}

            if (!string.IsNullOrEmpty(tournamentResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = tournamentResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.Name.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            //Filtering the tournaments by the game they belong to
            var gameFromDb = _context.Games.FirstOrDefault(g => g.Enum == gameNum);

            collectionBeforePaging = collectionBeforePaging
                .Where(t => t.GameId == gameFromDb.Id);

            return PagedList<Tournament>.Create(collectionBeforePaging,
                tournamentResourceParameters.PageNumber,
                tournamentResourceParameters.PageSize);
        }

        public Tournament GetTournament(Guid tournamentId)
        {
            return _context.Tournaments.FirstOrDefault(t => t.Id == tournamentId);
        }

        public void AddTournament(Tournament tournament)
        {
            tournament.Id = Guid.NewGuid();
            _context.Tournaments.Add(tournament);
        }

        public void DeleteTournament(Tournament tournament)
        {
            _context.Tournaments.Remove(tournament);
        }

        public void UpdateTournament(Tournament tournament)
        {
            //no code in this implementation
        }

        public bool TournamentExists(Guid tournamentId)
        {
            return _context.Tournaments.Any(t => t.Id == tournamentId);
        }

        public IEnumerable<Player> GetPlayersForTournament(Guid tournamentId)
        {
            var playerIds = (from s in _context.Sets
                             where s.TournamentId == tournamentId
                             select s.WinnerId)
                           .Union(from s in _context.Sets
                                  where s.TournamentId == tournamentId
                                  select s.LoserId)
                           .ToList();
            var players = _context.Players.Where(p => playerIds.Contains(p.Id))
                .OrderBy(p => p.Tag)
                .ToList();

            return players;

        }

        #endregion

        #region sets
        //TODO: Consider changing this to search by SGGPlayerId as per discussion with Mitch.
        public IEnumerable<SetDtoForPlayer> GetSetsForPlayer(Guid playerId, int gameNum)
        {
            var playerSets = (from set in _context.Sets
                              join winner in _context.Players on set.WinnerId equals winner.Id
                              join loser in _context.Players on set.LoserId equals loser.Id
                              join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                              join games in _context.Games on tournament.GameId equals games.Id
                              where (set.WinnerId == playerId || set.LoserId == playerId)
                              && set.LoserScore != -1 && games.Enum == gameNum

                              select new SetDtoForPlayer()
                              {
                                  Opponent = playerId == set.WinnerId ? loser.Tag : winner.Tag, 
                                  OpponentId = playerId == set.WinnerId ? set.LoserId : set.WinnerId,
                                  isWin = playerId == set.WinnerId ? true : false,
                                  WinnerScore = set.WinnerScore,
                                  LoserScore = set.LoserScore,
                                  Tournament = tournament.Name,
                                  TournamentId = tournament.Id,
                                  Date = tournament.Date
                              }
                             )
                             .OrderByDescending(s => s.Date)
                             .ToList();


            return playerSets;
        }

        public IEnumerable<SetDtoForTournament> GetSetsForTournament(Guid tournamentId)
        {
            var tournamentSets = (from set in _context.Sets
                                  join winner in _context.Players on set.WinnerId equals winner.Id
                                  join loser in _context.Players on set.LoserId equals loser.Id
                                  join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                                  where set.TournamentId == tournamentId && set.LoserScore != -1

                                  select new SetDtoForTournament()
                                  {
                                      Winner = winner.Tag,
                                      Loser = loser.Tag,
                                      WinnerScore = set.WinnerScore,
                                      LoserScore = set.LoserScore,
                                      WinnerId = set.WinnerId,
                                      LoserId = set.LoserId
                                  }
                                  ).ToList();

            return tournamentSets;
        }

        public IEnumerable<SetDtoForHead2Head> GetSetsBetweenPlayers(Guid player1Id, Guid player2Id, int gameNum)
        {
            var head2HeadSets = (from set in _context.Sets
                                 join winner in _context.Players on set.WinnerId equals winner.Id
                                 join loser in _context.Players on set.LoserId equals loser.Id
                                 join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                                 join games in _context.Games on tournament.GameId equals games.Id
                                 where (((set.WinnerId == player1Id && set.LoserId == player2Id) ||
                                    (set.WinnerId == player2Id && set.LoserId == player1Id)) && set.LoserScore != -1)
                                    && games.Enum == gameNum

                                 select new SetDtoForHead2Head()
                                 {
                                     Winner = winner.Tag,
                                     Loser = loser.Tag,
                                     WinnerScore = set.WinnerScore,
                                     LoserScore = set.LoserScore,
                                     WinnerId = set.WinnerId,
                                     LoserId = set.LoserId,
                                     Tournament = tournament.Name,
                                     TournamentId = tournament.Id,
                                     Date = tournament.Date
                                 }
                                )
                                .OrderByDescending(s => s.Date)
                                .ToList();

            return head2HeadSets;
        }

        public Set GetSet(Guid setId)
        {
            return _context.Sets.FirstOrDefault(s => s.Id == setId);
        }

        public SetDtoForDisplay GetSetForDisplay(Guid setId)
        {
            var set = (from s in _context.Sets
                       join winner in _context.Players on s.WinnerId equals winner.Id
                       join loser in _context.Players on s.LoserId equals loser.Id
                       join tournament in _context.Tournaments on s.TournamentId equals tournament.Id
                       where s.Id == setId

                       select new SetDtoForDisplay()
                       {
                           Winner = winner.Tag,
                           Loser = loser.Tag,
                           WinnerId = s.WinnerId,
                           LoserId = s.LoserId,
                           WinnerScore = s.WinnerScore,
                           LoserScore = s.LoserScore,
                           Tournament = tournament.Name,
                           TournamentId = tournament.Id,
                       }
                       ).FirstOrDefault();

            return set;
        }

        public void AddSet(Set set)
        {
            set.Id = Guid.NewGuid();
            _context.Sets.Add(set);
            // the repository fills the id (instead of using identity columns)
        }

        public void DeleteSet(Set set)
        {
            _context.Sets.Remove(set);
        }

        public void UpdateSet(Set set)
        {
            //no code in this implementation
        }

        #endregion

        #region User

        public User GetUserByEmail(string email)
        {
            return _context.Users.SingleOrDefault(user => user.Email == email);
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.ToList();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
        }

        #endregion

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }

        public IEnumerable<TrueskillHistoryDto> GetTrueskillHistoryForPlayer(Guid playerId, int gameNum)
        {
            var trueskillHistories = (from history in _context.TrueskillHistories
                                      join tourney in _context.Tournaments on history.TournamentId equals tourney.Id
                                      join players in _context.Players on history.PlayerId equals players.Id
                                      join games in _context.Games on tourney.GameId equals games.Id
                                      where history.PlayerId == playerId && games.Enum == gameNum

                                      select new TrueskillHistoryDto()
                                      {
                                          Trueskill = history.Trueskill,
                                          TournamentName = tourney.Name,
                                          TournamentDate = tourney.Date,
                                          PlayerName = players.Tag
                                      }
                                      )
                                      .OrderByDescending(t => t.TournamentDate)
                                      .ThenBy(p => p.PlayerName)
                                      .ToList();
            return trueskillHistories;
        }

        public TrueskillHistoryDto GetMostRecentTrueskillForPlayer(Guid playerId, int gameNum)
        {
            var trueskillHistories = (from history in _context.TrueskillHistories
                                      join tourney in _context.Tournaments on history.TournamentId equals tourney.Id
                                      join players in _context.Players on history.PlayerId equals players.Id
                                      join games in _context.Games on tourney.GameId equals games.Id
                                      where history.PlayerId == playerId && games.Enum == gameNum

                                      select new TrueskillHistoryDto()
                                      {
                                          Trueskill = history.Trueskill,
                                          TournamentName = tourney.Name,
                                          TournamentDate = tourney.Date,
                                          PlayerName = players.Tag
                                      }
                                      )
                                      .OrderByDescending(t => t.TournamentDate)
                                      .FirstOrDefault();
            return trueskillHistories;

            //TODO: Evaluate if it is necessary to make a custom linq query to grab Tournament Name? shouldn't be too hard.
            //Just some joins
        }

        public Player GetPlayerFromTrueskillHistory(Guid playerId, int gameNum)
        {
            //TODO: Refactor to take in GAME ID, for now let's hard code it in. 
            //TODO: What happens if a player has never played that game? Let's make a blank response for that perhaps. 
            //Guid gameId = Guid.Parse("1F52BB15-DFEF-4FD3-9C0A-E3F8260F9A1C"); //s4
            //Guid gameId = Guid.Parse("8FA6C1F8-B06F-4020-A154-3A88260515A4"); //melee

            var playerForGame = (from history in _context.TrueskillHistories
                                      join tourney in _context.Tournaments on history.TournamentId equals tourney.Id
                                      join players in _context.Players on history.PlayerId equals players.Id
                                      join games in _context.Games on tourney.GameId equals games.Id
                                      where history.PlayerId == playerId && games.Enum == gameNum

                                      select new Player()
                                      {
                                          Trueskill = history.Trueskill,
                                          Id = players.Id,
                                          Tag = players.Tag,
                                          State = players.State,
                                          FirstName = players.FirstName,
                                          LastName = players.LastName,
                                          SggPlayerId = players.SggPlayerId,
                                          LastActive = tourney.Date
                                      }
                                      )
                                      .OrderByDescending(t => t.LastActive)
                                      .FirstOrDefault();

            if (playerForGame == null)
            {
                //This can occur when the player has been added by an admin and doesn't have any set history.
                //In this case, we use the older method of returning a player
                return _context.Players.FirstOrDefault(p => p.Id == playerId);
            }
            return playerForGame;

            //TODO: Evaluate if it is necessary to make a custom linq query to grab Tournament Name? shouldn't be too hard.
            //Just some joins
        }

        public PagedList<Player> GetPlayersFromTrueskillHistory(PlayersResourceParameters playersResourceParameters, int gameNum)
        {
            //var gameTournament1 = _context.Tournaments
            //var playersTrueSkillHistory = _context.TrueskillHistories.Where(tsh => tsh.PlayerId == playerId);


            //var gameTournament = _context.Tournaments
            //    .Where(t => t.GameId == Guid.Parse("1F52BB15-DFEF-4FD3-9C0A-E3F8260F9A1C"));


            //Guid gameId = Guid.Parse("1F52BB15-DFEF-4FD3-9C0A-E3F8260F9A1C"); //s4
            Guid gameId = Guid.Parse("8FA6C1F8-B06F-4020-A154-3A88260515A4"); //melee

            //playersTrueSkillHistory.Where(p => p.TournamentId == )

            var allPlayersForGame = (from history in _context.TrueskillHistories
                                     join tourney in _context.Tournaments on history.TournamentId equals tourney.Id
                                     join players in _context.Players on history.PlayerId equals players.Id
                                     join games in _context.Games on tourney.GameId equals games.Id
                                     where games.Enum == gameNum

                                     select new Player()
                                     {
                                         Trueskill = history.Trueskill,
                                         Id = players.Id,
                                         Tag = players.Tag,
                                         State = players.State,
                                         FirstName = players.FirstName,
                                         LastName = players.LastName,
                                         SggPlayerId = players.SggPlayerId,
                                         LastActive = tourney.Date
                                     }
                                     )
                                    .OrderByDescending(t => t.LastActive)
                                      .ThenBy(p => p.Trueskill)
                                      .GroupBy(p => p.Id)
                                      .Select(p => p.First());
            //TODO look at group by, look at max with max of tournament date.
            //.ToList();
            //return allPlayersForGame.OrderByDescending(p => p.Trueskill);

            IQueryable<Player> collectionBeforePaging = allPlayersForGame.ApplySort(playersResourceParameters.OrderBy,
                _propertyMappingService.GetPropertyMapping<PlayerDto, Player>());

            if (!string.IsNullOrEmpty(playersResourceParameters.State))
            {
                // trim & ignore casing
                var stateForWhereClause = playersResourceParameters.State
                    .Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.State.ToLowerInvariant() == stateForWhereClause);
            }

            if (!string.IsNullOrEmpty(playersResourceParameters.SearchQuery))
            {
                // trim & ignore casing
                var searchQueryForWhereClause = playersResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging
                    .Where(a => a.State.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.FirstName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.LastName.ToLowerInvariant().Contains(searchQueryForWhereClause)
                    || a.Tag.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            return PagedList<Player>.Create(collectionBeforePaging, playersResourceParameters.PageNumber, playersResourceParameters.PageSize);
        }


    }
}
