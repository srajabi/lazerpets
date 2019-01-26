using UnityEngine;

namespace Game
{
    public class Damager
    {
        public GameObject GameObject { get; private set; }
        private Player player;

        public Damager(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public Damager(Player player) : this(player.gameObject)
        {
            this.player = player;
        }

        public string GetName()
        {
            if (player != null) return player.PlayerName;

            return GameObject.name;
        }
    }
}