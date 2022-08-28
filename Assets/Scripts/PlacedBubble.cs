using DG.Tweening;
using System;
using UnityEngine;

public class PlacedBubble : MonoBehaviour
{
	public Vector2Int Index { get; private set; }
	public Color Color { get => _spriteRenderer.color; set => _spriteRenderer.color = value; }
	public ComponentPool<PlacedBubble> Pool { get; set; }
	[SerializeField] private Vector3 _punchScale;
	[SerializeField] private float _punchDuration;
	[SerializeField] private float _punchStrengh;
	[SerializeField] private int _punchVibrato;
	[SerializeField][Range(0, 1)] private float _punchElasticity;
	private SpriteRenderer _spriteRenderer;
	private SpringJoint2D _springJoint;
	private CircleCollider2D _collider;
	Tween _tween;
	public event Action<PlacedBubble> OnDestroyed;

	private void Awake()
	{
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_springJoint = GetComponent<SpringJoint2D>();
		_collider = GetComponent<CircleCollider2D>();
	}

	private void OnEnable()
	{
		_collider.enabled = true;
	}

	public void Setup(Vector3 position, Color color, Vector2Int index)
	{
		transform.position = position;
		_spriteRenderer.color = color;
		_springJoint.connectedAnchor = transform.position;
		Index = index;
	}

	public void Destroy(bool visualize = true)
	{
		if (_tween != null)
		{
			Debug.Log($"[PlacedBubble] tween not null {_tween.position}", this);
		}
		if (visualize)
		{
			_collider.enabled = false;
			_spriteRenderer.DOFade(0, _punchDuration);
			_tween = transform.DOPunchScale(_punchScale, _punchDuration, _punchVibrato, _punchElasticity).OnComplete(() =>
			 {
				 Pool.Return(this);
				 _tween = null;
			 });
		}
		else
		{
			Pool.Return(this);
		}
		OnDestroyed?.Invoke(this);
	}
}