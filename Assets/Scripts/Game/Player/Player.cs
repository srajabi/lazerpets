using System;
using Networking;
using UnityEngine;
using NetworkPlayer = Networking.NetworkPlayer;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public NetworkPlayer NetworkPlayer { get; set; }
        public Health Health { get; private set; }
        public PlayerScore Score { get; private set; }
        public CritterController CritterController { get; private set; }
        public Effects Effects { get; private set; }

        public void Awake()
        {
            Health = GetComponentInChildren<Health>(true);
            Score = GetComponentInChildren<PlayerScore>(true);
        }

        internal void Initialize(NetworkPlayer netPlayer)
        {
            NetworkPlayer = netPlayer;
            name = string.Format("[Player] {0}", netPlayer.Name);
            netPlayer.Player = this;

            Debug.Log("Player Initialized: " + name + " isSelf" + netPlayer.IsSelf);

            GetComponent<CharacterInstantiator>().Create();
            CritterController = GetComponentInChildren<CritterController>(true);
            Effects = GetComponentInChildren<Effects>(true);
        }
    }
}