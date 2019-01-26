using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public CharacterTypes CharacterType;
        public string PlayerName = null;
        public Health Health { get; private set; }
        public PlayerScore Score { get; private set; }
        public CritterController CritterController { get; private set; }

        public void Awake()
        {
            Health = GetComponentInChildren<Health>(true);
            Score = GetComponentInChildren<PlayerScore>(true);
            CritterController = GetComponentInChildren<CritterController>(true);
        }
    }
}