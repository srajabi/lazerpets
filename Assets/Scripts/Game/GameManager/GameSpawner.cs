using System.Linq;

namespace Game
{
    public class GameSpawner
    {
        private readonly GameManager manager;

        public GameSpawner(GameManager manager)
        {
            this.manager = manager;
        }

        public void Spawn(Player player)
        {
            player.transform.position = SpawnPointManager.GetRandomVacantPoint(player.CharacterType).transform.position;
        }

        public void ReSpawn(Player player)
        {
            player.transform.position = SpawnPointManager.GetFurthestPoint(player.CharacterType, manager.Players.Where(p=>p!=player).Select(p=>p.transform).ToArray()).transform.position;
        }
    }
}