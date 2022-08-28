using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class BubbleDropper
{
	private float _bottomY;
	private float _speed;

	public BubbleDropper(float bottomY, float speed)
	{
		_bottomY = bottomY;
		_speed = speed;
	}

	public void DropBubbles(ICollection<PlacedBubble> bubbles)
	{
		Vector3 endValue = new(0, _bottomY, 0);
		foreach (var item in bubbles)
		{
			endValue.x = item.transform.position.x + Random.Range(-1.5f, 1.5f);
			item.enabled = false;
			item.transform.DOMove(endValue, (item.transform.position.y - _bottomY) / _speed).OnComplete(() => item.Destroy(false));
		}
	}
}