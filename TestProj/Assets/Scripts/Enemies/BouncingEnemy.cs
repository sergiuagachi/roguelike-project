using UnityEngine;

public class BouncingEnemy : Enemy {

    private Vector3 _moveVector;
    public float moveSpeed;

    public string bounceDirection;
    private bool _xBounce;
    private bool _yBounce;

    protected override void Start() {
        _defaultParameters.Health = 1f;
        PlayerDamage = 20;

        if (bounceDirection.Equals("x")) {
            _xBounce = true;
        }
        else if (bounceDirection.Equals("y")) {
            _yBounce = true;
        }
        
        _moveVector = new Vector3(_xBounce ? 1 : 0, _yBounce ? 1 : 0, 0 );

        base.Start();
    }
    
    // Update is called once per frame
    void Update() {
        transform.Translate(moveSpeed * Time.deltaTime * _moveVector);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        _moveVector *= -1;
        
        if(_xBounce)
            SpriteRenderer.flipX = !SpriteRenderer.flipX;

        if (other.gameObject.name.Equals("Player")) { 
            OnCantMove(GameObject.Find("Player").GetComponent<PhysicsObject>());
        }
    }

    protected override void OnCantMove(PhysicsObject component) {
        base.OnCantMove(component);
        gameObject.SetActive(false);
        enabled = false;
    }

    public override void Move() {}
}
