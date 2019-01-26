using System;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class GameOutcomeManager : MonoBehaviour
    {
        public int KillsToWin = 10;

        public event EventHandler<GameOutcomeEventArgs> OnGameEnded;

        private GameManager GameManager { get; set; }

        public void Start()
        {
            GameManager = GetComponent<GameManager>();
            GameManager.OnInitialized += InitializeGame;
        }

        public void Update()
        {
            if (!GameManager.Initialized)
                return;
            
            CheckGameOutcome();
        }

        private void InitializeGame(object sender, EventArgs e)
        {
            foreach (var player in GameManager.Players)
            {
                InitializePlayer(player);
            }
        }

        private void InitializePlayer(Player player)
        {
            player.Health.OnModified += OnHealthModified;
            player.Health.OnDeath += OnDeath;
        }

        private void OnDeath(object sender, HealthEventArgs e)
        {
            Debug.LogErrorFormat("Player {0} died thanks to {1}.", e.Causee.GetName(), e.Causer.GetName());
        }

        private void OnHealthModified(object sender, HealthEventArgs e)
        {
            Debug.LogErrorFormat("Player {0} took some damage from {1}.", e.Causee.GetName(), e.Causer.GetName());
        }

        private void CheckGameOutcome()
        {
            foreach (var player in GameManager.Players)
            {
                var score = player.Score;
                if (score.Kills >= KillsToWin)
                {
                    OnGameEnded?.Invoke(this, new GameOutcomeEventArgs(new GameOutcome(player, GameManager.Players.Where(p => p != player).ToList())));
                }
            }
        }
    }
}