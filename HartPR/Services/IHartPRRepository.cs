using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Entities;
using HartPR.Helpers;
using HartPR.Models;

namespace HartPR.Services
{
    public interface IHartPRRepository
    {
        //players
        PagedList<Player> GetPlayers(PlayersResourceParameters playersResourceParameters);
        Player GetPlayer(Guid playerId);
        IEnumerable<Player> GetPlayers(IEnumerable<Guid> playerIds);
        void AddPlayer(Player player);
        void AddPlayer(Player player, Guid playerId);
        void DeletePlayer(Player player);
        void UpdatePlayer(Player player);
        bool PlayerExists(Guid playerId);
        IEnumerable<Tournament> GetTournamentsForPlayer(Guid playerId);
        //tournaments
        PagedList<Tournament> GetTournaments(TournamentsResourceParameters tournamentsResourceParameters);
        Tournament GetTournament(Guid tournamentId);
        void AddTournament(Tournament tournament);
        void DeleteTournament(Tournament tournament);
        void UpdateTournament(Tournament tournament);
        bool TournamentExists(Guid tournamentId);
        IEnumerable<Player> GetPlayersForTournament(Guid tournamentId);
        //sets
        IEnumerable<SetDtoForPlayer> GetSetsForPlayer(Guid playerId);
        IEnumerable<SetDtoForTournament> GetSetsForTournament(Guid tournamentId);
        IEnumerable<SetDtoForHead2Head> GetSetsBetweenPlayers(Guid player1Id, Guid player2Id);
        IEnumerable<TrueskillHistoryDto> GetTrueskillHistoryForPlayer(Guid playerId);
        TrueskillHistoryDto GetMostRecentTrueskillForPlayer(Guid playerId);
        IEnumerable<Player> GetPlayersFromTrueskillHistory(Guid playerId);
        Set GetSet(Guid setId);
        SetDtoForDisplay GetSetForDisplay(Guid setId);
        void AddSet(Set set);
        void DeleteSet(Set set);
        void UpdateSet(Set set);
        bool Save();
        //Users
        User GetUserByEmail(string email);
        IEnumerable<User> GetUsers();
        void AddUser(User newUser);
    }
}
