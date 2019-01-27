using UnityEngine;

namespace Game
{
    public class CritterModel : PlayerBehaviour
    {
        private CritterController controller;

        public void Start()
        {
            gameObject.SetActive(!Player.NetworkPlayer.IsSelf);
            controller = GetComponentInParent<CritterController>();
        }

        public void Update()
        {
            Vector3 eulerAngles = controller.Mover.Head.transform.eulerAngles;
            eulerAngles = new Vector3(0, eulerAngles.y, 0);
            transform.rotation = Quaternion.Euler(eulerAngles);
        }
    }
}