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

        internal void Initialize(NetworkPlayer netPlayer, IInputGrabber localInputGrabber, bool isServer)
        {
            NetworkPlayer = netPlayer;
            name = string.Format("[Player] {0}", netPlayer.Name);
            netPlayer.Player = this;

            GetComponent<CharacterInstantiator>().Create();
            CritterController = GetComponentInChildren<CritterController>(true);
            CritterController.IsServer = isServer;
            CritterController.OnCritterStatePacket += netPlayer.ForwardCritterStatePacket;
            CritterController.localInputGrabber = (netPlayer.IsSelf) ? localInputGrabber : null;

            NetworkPlayer.OnIncommingCritterStatePacket += CritterController.UpdateViaCritterStatePacket;


            if (!isServer && netPlayer.IsSelf)
            {
                CritterController.OnCritterInputPacket += NetworkPlayer.PostCritterInputPacket;
            }

            Debug.Log("Player Initialized: " + name + " isSelf" + netPlayer.IsSelf);

            Effects = GetComponentInChildren<Effects>(true);
        }

        internal void SetInputPacket(CritterInputPacket critterInputPacket)
        {
            CritterController.InputPacketOveride = critterInputPacket;
        }
    }
}