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
            var spawnPoint = SpawnPointManager.GetRandomVacantPoint(player.NetworkPlayer.CharacterType);
            player.CritterController.transform.position = spawnPoint.transform.position;
        }

        public void ReSpawn(Player player)
        {
            var otherPlayers = manager.Players
                .Where(p => p != player)
                .Select(p => p.transform)
                .ToArray();

            var spawnPoint = SpawnPointManager.GetFurthestPoint(player.NetworkPlayer.CharacterType, otherPlayers);
            player.CritterController.transform.position = spawnPoint.transform.position;
        }
    }
}