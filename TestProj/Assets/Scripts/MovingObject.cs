using UnityEngine;
using System.Collections;

public abstract class MovingObject : LivingObject {
	
	protected float MoveTime;			//Time it will take object to move, in seconds.
	private LayerMask _blockingLayer;			//Layer on which collision will be checked.
	
	private float _inverseMoveTime;			//Used to make movement more efficient.

	protected bool HitOuterWall;
	
	//Protected, virtual functions can be overridden by inheriting classes.
	protected override void Start () {
		_inverseMoveTime = 1f / MoveTime;
		_blockingLayer =  LayerMask.GetMask("BlockingLayer");
		
		base.Start();
	}

	protected virtual void AttemptMove(int xDir, int yDir) {

		// reset the flag
		HitOuterWall = false;
		
		switch (xDir) {
			case -1:
				SpriteRenderer.flipX = true;
				break;
			case 1:
				SpriteRenderer.flipX = false;
				break;
		}

		//Hit will store whatever our linecast hits when Move is called.
		RaycastHit2D hit;

		//Set canMove to true if Move was successful, false if failed.
		bool canMove = Move(xDir, yDir, out hit);

		//Check if nothing was hit by linecast
		if (hit.transform == null)
			//If nothing was hit, return and don't execute further code.
			return;

		//Get a component reference to the component of type T attached to the object that was hit
		MonoBehaviour hitComponent = hit.transform.GetComponent<MonoBehaviour>();

		//If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
		if (!canMove && hitComponent != null) {

			//Call the OnCantMove function and pass it hitComponent as a parameter.
			OnCantMove(hitComponent);
		}
		else if (!canMove && hitComponent == null) {
			HitOuterWall = true;
		}
	}

	private IEnumerator SmoothMovement (Vector3 end) {

		//Calculate the remaining distance to move based on the square magnitude of the difference between current position and end parameter. 
		//Square magnitude is used instead of magnitude because it's computationally cheaper.
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		
		//While that distance is greater than a very small amount (Epsilon, almost zero):
		while(sqrRemainingDistance > float.Epsilon)
		{
			//Find a new position proportionally closer to the end, based on the moveTime
			Vector3 newPostion = Vector3.MoveTowards(Rb2D.position, end, _inverseMoveTime * Time.deltaTime);
			
			//Call MovePosition on attached Rigidbody2D and move it to the calculated position.
			Rb2D.MovePosition (newPostion);
			
			//Recalculate the remaining distance after moving.
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			//Return and loop until sqrRemainingDistance is close enough to zero to end the function
			yield return null;
		}

		if (this is Player) {
			yield return new WaitForSeconds(0.2f);
			GameManager.Instance.playerCanMove = true;
		}
	}

	private bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		//Store start position to move from, based on objects current transform position.
		Vector2 start = transform.position;
		
		// Calculate end position based on the direction parameters passed in when calling Move.
		Vector2 end = start + new Vector2 (xDir, yDir);
		
		//Cast a line from start point to end point checking collision on blockingLayer.
		hit = Physics2D.Linecast (start, end, _blockingLayer);
		
		//Check if anything was hit
		if(hit.transform == null)
		{
			//If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
			StartCoroutine (SmoothMovement (end));
			
			//Return true to say that Move was successful
			return true;
		}
		
		//If something was hit, return false, Move was unsuccesful.
		return false;
	}

	protected abstract void OnCantMove(MonoBehaviour component);
}
