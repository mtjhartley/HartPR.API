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
            //return _context.Sets.Where(s => s.Entrant1Id == playerId
            //    || s.Entrant2Id == playerId).Join()
            //    ToList();

            //return _context.Players
            //    .Join(_context.Sets,
            //        p => p.Id,
            //        s => s.Entrant1Id || s.Entrant2Id,

            //var playerSets = from p in _context.Players
            //                 join s in _context.Sets on p.Id equals s.Entrant1Id || p.Id equals s.Entrant2Id

            //var setPlayers = from s in _context.Sets
            //                 join p in _context.Players on s.Entrant1Id equals p.Id

            //var linqQuery1 = from s in _context.Sets
            //                   from w in _context.Players
            //                   from l in _context.Players
            //                   where s.Entrant1Id == w.Id || s.Entrant2Id == w.Id || s.Entrant1Id == l.Id || s.Entrant2Id == l.Id
            //                   select new { w.Tag, l.Tag, }

            //var linqQuery2 = from p in _context.Players
            //                 join ws in _context.Sets on p.Id equals ws.WinnerId
            //                 join ls in _context.Sets on p.Id equals ls.LoserId
            //                 select new
            //                     {
            //                        ws.p
            //                     }
            var linqQuery3 = (from set in _context.Sets
                              join winner in _context.Players on set.WinnerId equals winner.Id
                              join loser in _context.Players on set.LoserId equals loser.Id
                              join tournament in _context.Tournaments on set.TournamentId equals tournament.Id
                              where set.WinnerId == winner.Id || set.LoserId == loser.Id

                              //change this to use a new front facing set DTO
                              select new SetDtoForPlayer()
                              {
                                  Winner = winner.Tag,
                                  Loser = loser.Tag,
                                  Tournament = tournament.Name
                              }
                             ).ToList();
            return linqQuery3;


        }

        public IEnumerable<Set> GetSetsForTournament(Guid tournamentId)
        {
            return _context.Sets.Where(s => s.TournamentId == tournamentId).ToList();
        }

        #endregion  

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }


    }
}
