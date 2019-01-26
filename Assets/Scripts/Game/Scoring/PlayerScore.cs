namespace Game
{
    public class PlayerScore : PlayerBehaviour
    {
        public int Deaths        { get; private set; }
        public int Kills         { get; private set; }
        public float DamageDealt { get; private set; }
        public float DamageTaken { get; private set; }

        public override void Awake()
        {
            var health = Player.GetComponent<Health>();
            health.OnDeath += HandleDeath;
            health.OnModified += HandleDamage;
        }

        private void HandleDamage(object sender, HealthEventArgs e)
        {
            OnDamageTaken(e.Modification);

            var causer = e.Causer.GetComponent<PlayerScore>();
            causer.OnDamageDealt(e.Modification);
        }

        private void HandleDeath(object sender, HealthEventArgs e)
        {
            OnDeath();

            var causer = e.Causer.GetComponent<PlayerScore>();
            causer.OnKill();
        }

        public void OnDeath()
        {
            Deaths++;
        }

        public void OnKill()
        {
            Kills++;
        }

        public void OnDamageTaken(float amount)
        {
            DamageTaken -= amount; //negative, since amount is a health modification
        }

        public void OnDamageDealt(float amount)
        {
            DamageTaken -= amount; //negative, since amount is a health modification
        }
    }
}