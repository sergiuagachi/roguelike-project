using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : PhysicsObject
{
    protected override void Start() {
        _defaultParameters.Health = 6f;
        
        base.Start();
    }
}
