public class DefaultEnemy : Enemy {
	protected override void Start () {
		_defaultParameters.Health = 50f;
		PlayerDamage = 20;
		
		base.Start ();
	}
}
