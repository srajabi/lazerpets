using Game;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameHUD : MonoBehaviour
    {
        [SerializeField]
        private Text HealthText;
        [SerializeField]
        private Slider HealthBar;
        [SerializeField]
        private Text KillsText;
        [SerializeField]
        private Text DeathsText;

        private GameManager gameManager;

        public void Start()
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        public void Update()
        {
            var player = gameManager.Players.First(p => p.NetworkPlayer.IsSelf);
            if (player == null)
                return;

            var health = player.Health;
            var score = player.Score;

            HealthText.text = string.Format("Health {0}/{1}", health.Current, health.Default);
            HealthBar.value = health.Current / health.Default;

            KillsText.text = string.Format("Kills: {0}", score.Kills);
            DeathsText.text = string.Format("Deaths: {0}", score.Deaths);
        }
    }
}