using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HartPR.Entities;
using HartPR.Helpers;

namespace HartPR.Services
{
    public interface IHartPRRepository
    {
        PagedList<Player> GetPlayers(PlayersResourceParameters playersResourceParameters);
        Player GetPlayer(Guid playerId);
        IEnumerable<Player> GetPlayers(IEnumerable<Guid> playerIds);
        void AddPlayer(Player player);
        //void DeletePlayer(Player player);
        //void UpdatePlayer(Player player);
        //bool PlayerExists(Guid playerId);
        bool Save();




    }
}
