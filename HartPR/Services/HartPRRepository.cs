﻿using System;
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
