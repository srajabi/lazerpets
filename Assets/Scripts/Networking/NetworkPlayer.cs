using System;
using System.Linq;
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
            "Calcifer"
        };

        internal bool isServer;
        internal NetworkConnection Connection;

        public int ID;
        public string Name;

        public NetworkPlayer()
        {
            Name = NamePool.OrderBy(n => Guid.NewGuid()).First();
        }
    }

}