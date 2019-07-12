public class SlowPowerfulEnemy : Enemy {

    private const int TurnsToMove = 2;
    private int _turnUntilCanMove;

    protected override void Start () {
        _defaultParameters.Health = 80f;
        PlayerDamage = 40;
        _turnUntilCanMove = TurnsToMove;
        moveTime = 0.5f;
		
        base.Start ();
    }
	
    protected override void AttemptMove(int xDir, int yDir) {
        // to make the game playable and fair, the enemies can only move once every 2 turns
        if(_turnUntilCanMove == 0) {
            base.AttemptMove(xDir, yDir);
            _turnUntilCanMove = TurnsToMove;
        }
        _turnUntilCanMove--;
    }
}