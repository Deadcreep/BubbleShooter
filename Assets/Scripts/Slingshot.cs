using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public enum SlingshotState
{
	Idle,
	Drag,
	Shoot
}

public class Slingshot : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
	public SlingshotState State
	{
		get => _state;
		set
		{
			if (_state != value)
			{
				_state = value;
				OnStateChanged?.Invoke();
			}
		}
	}

	public bool IsFullSpeed
	{
		get => _isFullSpeed;
		set
		{
			if (_isFullSpeed != value)
			{
				_isFullSpeed = value;
				OnSpeedChanged?.Invoke();
			}
		}
	}

	public float MaxStretchDistance { get => _maxStretchDistance; }
	[SerializeField] private SlingshotState _state;
	[SerializeField] private float _maxStretchDistance;
	[SerializeField] private float _maxSpeed;
	[SerializeField] private float _spreadingAngle = 5;

	private Bubble _ball;
	private bool _isFullSpeed;
	private Vector3 _defaultPos;
	private Vector3 _direction;
	private float _currentSpeed;
	private float _gravityFactor;
	private float _leftBorder;
	private float _rightBorder;
	private WaitForFixedUpdate _sleep;

	public event System.Action OnStateChanged;

	public event System.Action OnSpeedChanged;

	public void Init(Bubble ball, float leftBorder, float rightBorder)
	{
		_defaultPos = transform.position;
		_ball = ball;
		_gravityFactor = _ball.GravityFactor;
		_leftBorder = leftBorder;
		_rightBorder = rightBorder;
		_ball.transform.SetParent(transform);
	}

	private void Start()
	{
		PrepareToShoot();
	}

	public void PrepareToShoot()
	{
		State = SlingshotState.Idle;
		_ball.transform.position = transform.position;
	}

	public void BuildPath(Vector3[] mainLine, float pathFrame)
	{
		var direction = _direction;

		Vector3 gravityVector = (Physics.gravity * _gravityFactor * pathFrame);
		mainLine[0] = transform.position;

		for (int i = 1; i < mainLine.Length; i++)
		{
			direction += gravityVector;
			mainLine[i] = mainLine[i - 1] + (direction * _currentSpeed * pathFrame);
			ClampPoint(ref mainLine[i], ref direction);
		}
	}

	public void BuildPath(Vector3[] mainLine, Vector3[] spreadLineFirst, Vector3[] spreadLineSecond, float pathFrame)
	{
		var direction = _direction;
		var fsdir = Quaternion.Euler(0, 0, -_spreadingAngle) * direction;
		var ssdir = Quaternion.Euler(0, 0, _spreadingAngle) * direction;
		Vector3 gravityVector = (_gravityFactor * pathFrame * Physics.gravity);
		mainLine[0] = transform.position;
		spreadLineFirst[0] = transform.position;
		spreadLineSecond[0] = transform.position;

		for (int i = 1; i < mainLine.Length; i++)
		{
			direction += gravityVector;
			mainLine[i] = mainLine[i - 1] + (_currentSpeed * pathFrame * direction);

			fsdir += gravityVector;
			ssdir += gravityVector;
			spreadLineFirst[i] = spreadLineFirst[i - 1] + fsdir;
			spreadLineSecond[i] = spreadLineSecond[i - 1] + ssdir;

			ClampPoint(ref mainLine[i], ref direction);
			ClampPoint(ref spreadLineFirst[i], ref fsdir);
			ClampPoint(ref spreadLineSecond[i], ref ssdir);
		}
	}

	private void ClampPoint(ref Vector3 point, ref Vector3 direction)
	{
		if (point.x < _leftBorder)
		{
			point = FindPoint(direction, point, _leftBorder, -1);
			direction = Vector3.Reflect(direction, -Vector3.right);
		}
		else if (point.x > _rightBorder)
		{
			point = FindPoint(direction, point, _rightBorder, 1);
			direction = Vector3.Reflect(direction, Vector3.right);
		}
	}

	private static Vector3 FindPoint(Vector3 direction, Vector3 pos, float border, int sign)
	{
		var angle = Vector3.Angle(direction, Vector3.right * sign);
		var catet = sign > 0 ? border - pos.x : pos.x - border;
		var secCatet = catet * Mathf.Tan(angle * Mathf.Deg2Rad);
		var point = new Vector3(border, pos.y + secCatet, 0);
		return point;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (State == SlingshotState.Shoot)
			return;
		State = SlingshotState.Drag;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (State != SlingshotState.Drag)
		{
			return;
		}
		var pos = Camera.main.ScreenToWorldPoint(eventData.position);
		pos.z = 0;
		if (pos.y > _defaultPos.y)
		{
			return;
		}
		if (Vector3.Distance(pos, _defaultPos) > MaxStretchDistance)
		{
			var direction = pos - _defaultPos;
			pos = _defaultPos + (direction.normalized * MaxStretchDistance);
			IsFullSpeed = true;
			_currentSpeed = _maxSpeed;
		}
		else
		{
			IsFullSpeed = false;
			float distance = Vector3.Distance(transform.position, _defaultPos);
			_currentSpeed = _maxSpeed * Utils.MathUtils.Normalize(distance, 0, MaxStretchDistance);
		}
		_direction = (_defaultPos - transform.position).normalized;
		transform.position = pos;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (State == SlingshotState.Drag)
		{
			Shoot();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
	}

	private void Shoot()
	{
		State = SlingshotState.Shoot;
		if (IsFullSpeed)
		{
			var randomAngle = Random.Range(-_spreadingAngle, _spreadingAngle);
			_direction = Quaternion.Euler(0, 0, randomAngle) * _direction;
		}		
		var distance = Vector3.Distance(transform.position, _defaultPos);
		var time = distance / _currentSpeed;
		transform.DOMove(_defaultPos, _currentSpeed).SetSpeedBased(true).OnComplete(() => _ball.Shoot(_direction, _currentSpeed));
	}
}