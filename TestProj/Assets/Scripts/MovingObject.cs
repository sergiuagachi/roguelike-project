using UnityEngine;
using System.Collections;

public abstract class MovingObject : LivingObject {
	
	protected float MoveTime;			//Time it will take object to move, in seconds.
	private LayerMask _blockingLayer;			//Layer on which collision will be checked.
	
	private float _inverseMoveTime;			//Used to make movement more efficient.

	protected bool HitOuterWall;
	
	//Protected, virtual functions can be overridden by inheriting classes.
	protected override void Start () {
		base.Start();
		
		_inverseMoveTime = 1f / MoveTime;
		_blockingLayer =  LayerMask.GetMask("BlockingLayer");
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

		RaycastHit2D hit;

		var canMove = Move(xDir, yDir, out hit);

		if (hit.transform == null)
			return;
		
		var hitComponent = hit.transform.GetComponent<MonoBehaviour>();
		
		if (!canMove && hitComponent != null) {
			OnCantMove(hitComponent);
		}
		else if (!canMove && hitComponent == null) {
			HitOuterWall = true;
		}
	}

	private bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		
		hit = Physics2D.Linecast (start, end, _blockingLayer);

		if (hit.transform != null) return false;
		
		StartCoroutine (SmoothMovement (end));
		return true;
	}
	
	private IEnumerator SmoothMovement (Vector3 end) {
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		
		while(sqrRemainingDistance > float.Epsilon)
		{
			Vector3 newPostion = Vector3.MoveTowards(Rb2D.position, end, _inverseMoveTime * Time.deltaTime);
			
			Rb2D.MovePosition (newPostion);
			
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			
			yield return null;
		}

		if (this is Player) {
			yield return new WaitForSeconds(0.2f);
			GameManager.Instance.playerCanMove = true;
		}
	}

	protected abstract void OnCantMove(MonoBehaviour component);
}
