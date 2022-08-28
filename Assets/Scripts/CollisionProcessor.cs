using System;
using UnityEngine;
using Utils;

public class CollisionProcessor
{
	private Bubble _bubble;
	private LayerMask _wallMask;
	private LayerMask _deadZoneMask;
	private LayerMask _placedBubbleMask;
	private int _lastContactFrame;

	public event Action OnBubbleShouldReturn;
	public event Action<Vector3, PlacedBubble> OnCollisionWithBubble;

	public CollisionProcessor(Bubble bubble, LayerMask wallMask, LayerMask deadZoneMask, LayerMask placedBubbleMask)
	{
		_bubble = bubble;
		_wallMask = wallMask;
		_deadZoneMask = deadZoneMask;
		_placedBubbleMask = placedBubbleMask;
	}

	public void ProcessCollision(Collision2D collision)
	{
		if (_wallMask.CheckLayer(collision.gameObject.layer))
		{
			_bubble.Direction = Vector3.Reflect(_bubble.Direction, Vector3.right);
		}
		else if (_deadZoneMask.CheckLayer(collision.gameObject.layer))
		{
			OnBubbleShouldReturn?.Invoke();
		}
		else if (_placedBubbleMask.CheckLayer(collision.gameObject.layer))
		{
			if (_lastContactFrame - Time.frameCount < -1)
			{
				var placedBall = collision.gameObject.GetComponent<PlacedBubble>();
				_bubble.enabled = false;
				OnCollisionWithBubble?.Invoke(_bubble.transform.position, placedBall);
				_lastContactFrame = Time.frameCount;
			}
		}
	}
}