using System;
using System.Linq;
using Game;
using UnityEngine.Networking;

namespace Networking
{
    public class NetworkPlayer
    {
        static readonly string[] NAMES_CATS = new string[] {
            "Fluffy",
            "Scuffy",
            "Scruffy",
            "Kitty", 
            "Frodo",
            "Calcifer",
        };

        static readonly string[] NAMES_DOGS = new string[] {
            "Jasper",
            "Spike",
            "Rex",
            "Boots",
            "Doggie",
            "Baloo",
            "Sydney",
        };
        
        static readonly string[] NAMES_BIRDS = new string[] {
            "Birdie",
            "Squawk",
            "BigBird",
            "Fly'y McFly",
        };

        internal bool isServer;
        internal NetworkConnection Connection;

        public int ID;
        public string Name;
        public CharacterTypes CharacterType;

        public NetworkPlayer()
        {
            CharacterType = (CharacterTypes)UnityEngine.Random.Range(1, 3);

            switch (CharacterType)
            {
                case CharacterTypes.Unassigned:
                case CharacterTypes.Cat:
                    Name = NAMES_CATS.OrderBy(n => Guid.NewGuid()).First();
                    break;
                case CharacterTypes.Dog:
                    Name = NAMES_DOGS.OrderBy(n => Guid.NewGuid()).First();
                    break;
                case CharacterTypes.Bird:
                    Name = NAMES_BIRDS.OrderBy(n => Guid.NewGuid()).First();
                    break;
            }
        }
    }

}