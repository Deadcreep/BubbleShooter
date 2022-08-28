using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Bubble : MonoBehaviour
{
	public Vector3 Direction { get; set; }
	public float Speed { get => _speed; private set => _speed = value; }
	public float GravityFactor { get => _gravityFactor; }
	public float ImpactForce { get => _impactForce; }
	public Color Color { get => _spriteRenderer.color; set => _spriteRenderer.color = value; }
	[SerializeField] private float _speed;
	[SerializeField] private float _gravityFactor;
	[SerializeField] private float _impactForce;
	private Vector3 _gravityVector;
	private SpriteRenderer _spriteRenderer;
	private CircleCollider2D _collider;
	private bool _isMoving;
	private WaitForFixedUpdate _waitForFixedUpdate;
	private IEnumerator moveCoroutine;

	public event Action<Collision2D> OnCollisioned;

	private void Awake()
	{
		moveCoroutine = ShootCoroutine();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_collider = GetComponent<CircleCollider2D>();
		_waitForFixedUpdate = new WaitForFixedUpdate();
		_gravityVector = GravityFactor * Time.fixedDeltaTime * Physics.gravity;
	}

	private void OnEnable()
	{
		_collider.enabled = true;
	}

	private void OnDisable()
	{
		_collider.enabled = false;
	}

	public void Shoot(Vector3 direction, float speed)
	{
		_collider.enabled = true;
		Direction = direction;
		Speed = speed;
		StopCoroutine(moveCoroutine);
		_isMoving = true;
		StartCoroutine(moveCoroutine);
	}

	public void Stop()
	{
		_isMoving = false;
		StopCoroutine(moveCoroutine);
	}

	public void MoveTo(Vector3 position, Action callback)
	{
		StopCoroutine(moveCoroutine);
		transform.DOMove(position, 0.2f).OnComplete(() => callback?.Invoke());
	}

	private IEnumerator ShootCoroutine()
	{
		while (_isMoving)
		{
			Direction += _gravityVector;
			transform.position += Direction * Speed * Time.fixedDeltaTime;
			yield return _waitForFixedUpdate;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.rigidbody)
		{
			collision.rigidbody.AddForce(Direction * ImpactForce);
		}
		OnCollisioned?.Invoke(collision);
	}
}