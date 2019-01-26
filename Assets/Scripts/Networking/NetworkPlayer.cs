using System;
using System.Linq;
using Game;
using UnityEngine.Networking;

namespace Networking
{
    public class NetworkPlayer
    {
        static string[] NamePool = new string[] {
            "Fluffy",
            "Jasper",
            "Spike",
            "Pet",
            "Scuffy",
            "Boots",
            "Doggie",
            "Birdie",
            "Kitty",
            "Baloo",
            "Frodo",
            "Calcifer",
            "Sydney" // dog
        };

        internal bool isServer;
        internal NetworkConnection Connection;

        public int ID;
        public string Name;
        internal Player Player;

        public NetworkPlayer()
        {
            Name = NamePool.OrderBy(n => Guid.NewGuid()).First();
        }
    }

}