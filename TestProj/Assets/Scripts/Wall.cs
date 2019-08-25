public class Wall : PhysicsObject
{
    DefaultParameters _parameters = new DefaultParameters();
    
    protected override void Start() {
        _parameters.Health = 6;
        
        base.Start();
    }

    public override void TakeDamage(int loss) {
        throw new System.NotImplementedException();
    }
}
