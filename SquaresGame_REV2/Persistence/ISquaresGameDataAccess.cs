using System;
using System.Threading.Tasks;

namespace SquaresGame.Persistence
{
    public interface ISquaresGameDataAccess
    {
        Task SaveGameAsync(GameStateWrapper state, String path);
        Task<GameStateWrapper> LoadGameAsync(String path);
    }
}
