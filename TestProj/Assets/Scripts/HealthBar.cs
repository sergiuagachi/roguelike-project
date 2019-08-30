using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class HealthBar : MonoBehaviour {
    
    private Vector3 _localScale;
    private LivingObject _parent;
    private int _maxHealth;

    private void Start() {
        _localScale = transform.localScale;
        _parent = transform.parent.GetComponent<LivingObject>();
        _maxHealth = _parent.GetHealth();
    }

    private void Update() {
        _localScale.x = (float)_parent.GetHealth() / _maxHealth;
        transform.localScale = _localScale;
    }
}
