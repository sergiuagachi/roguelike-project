using System.Collections;
using UnityEngine;

public class FastWeakEnemy : Enemy {

    private bool _canMoveTwice;
    
    protected override void Start () {
        _defaultParameters.Health = 20f;
        PlayerDamage = 10;
        _canMoveTwice = true;
		
        base.Start ();
    }

    public override void Move() {
        StartCoroutine(MoveTwice());
    }

    private IEnumerator MoveTwice() {
        base.Move();

        if (_canMoveTwice) {
            yield return new WaitForSeconds(moveTime + 0.1f);
            base.Move();

            _canMoveTwice = false;
        }
        else {
            _canMoveTwice = true;
        }
    }
}