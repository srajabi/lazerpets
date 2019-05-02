using UnityEngine;

namespace Game
{
    public class ModelPicker : PlayerBehaviour
    {
        [SerializeField]
        private GameObject[] Models;
        public void Start()
        {
            int index = Player.NetworkPlayer.ID % Models.Length;
            Models[index].SetActive(true);
        }
    }
}