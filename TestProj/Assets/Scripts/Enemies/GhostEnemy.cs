public class GhostEnemy : Enemy {
    protected override void Start() {
        _defaultParameters.Health = 20f;
        PlayerDamage = 20;

        base.Start();
    }

    public override void TakeDamage(int loss) {
        
        base.TakeDamage(loss);
    }
}
