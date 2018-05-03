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
        public PagedList<Player> GetPlayers(
            PlayersResourceParameters playersResourceParameters)
        {
            //var collectionBeforePaging = _context.Players
            //    .OrderBy(a => a.FirstName)
            //    .ThenBy(a => a.LastName).AsQueryable();

            var collectionBeforePaging =
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

        #endregion

        #region sets
        //TODO: Consider changing this to search by SGGPlayerId as per discussion with Mitch.
        public IEnumerable<SetDtoForPlayer> GetSetsForPlayer(Guid playerId)
        {
            var playerSets = (from set in _context.Sets
                              join winner in _context.Players on set.WinnerId equals winner.Id
                              join loser in _context.Players on set.LoserId equals loser.Id
                              join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                              where set.WinnerId == playerId || set.LoserId == playerId

                              select new SetDtoForPlayer()
                              {
                                  Winner = winner.Tag,
                                  Loser = loser.Tag,
                                  WinnerId = set.WinnerId,
                                  LoserId = set.LoserId,
                                  Tournament = tournament.Name,
                                  Date = tournament.Date
                              }
                             ).ToList();

            return playerSets;
        }

        public IEnumerable<SetDtoForTournament> GetSetsForTournament(Guid tournamentId)
        {
            var tournamentSets = (from set in _context.Sets
                                  join winner in _context.Players on set.WinnerId equals winner.Id
                                  join loser in _context.Players on set.LoserId equals loser.Id
                                  join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                                  where set.TournamentId == tournamentId

                                  select new SetDtoForTournament()
                                  {
                                      Winner = winner.Tag,
                                      Loser = loser.Tag,
                                      WinnerId = set.WinnerId,
                                      LoserId = set.LoserId
                                  }
                                  ).ToList();

            return tournamentSets;
        }

        public IEnumerable<SetDtoForHead2Head> GetSetsBetweenPlayers(Guid player1Id, Guid player2Id)
        {
            var head2HeadSets = (from set in _context.Sets
                                 join winner in _context.Players on set.WinnerId equals winner.Id
                                 join loser in _context.Players on set.LoserId equals loser.Id
                                 join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                                 where (set.Entrant1Id == player1Id && set.Entrant2Id == player2Id) ||
                                    (set.Entrant1Id == player2Id && set.Entrant2Id == player1Id)

                                 select new SetDtoForHead2Head()
                                 {
                                     Winner = winner.Tag,
                                     Loser = loser.Tag,
                                     WinnerId = set.WinnerId,
                                     LoserId = set.LoserId,
                                     Tournament = tournament.Name,
                                     Date = tournament.Date
                                 }
                                ).ToList();

            return head2HeadSets;
        }

        #endregion  

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }


    }
}
