namespace Game
{
    public class PlayerCamera : PlayerBehaviour
    {
        public void Start()
        {
            gameObject.SetActive(Player.NetworkPlayer.IsSelf);
        }
    }
}