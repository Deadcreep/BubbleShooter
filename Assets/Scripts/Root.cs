using TextParsers;
using UnityEngine;

public class Root : MonoBehaviour
{
	[Header("Borders")]
	[SerializeField] private Camera _camera;
	[SerializeField] private BoxCollider2D _leftWall;
	[SerializeField] private BoxCollider2D _rightWall;
	[SerializeField] private BoxCollider2D _upperDeadzone;
	[SerializeField] private BoxCollider2D _bottomDeadzone;
	[SerializeField] private float _minDistanceFromSlingshotToField;
	[SerializeField] private Transform _leftRubber;
	[SerializeField] private Transform _rightRubber;

	[Space]
	[SerializeField] private int _shotsCount = 10;
	[SerializeField] private float _bubblesDropSpeed = 10;
	[SerializeField] private ScoreManager _scoreManager;
	[SerializeField] private Slingshot _slingshot;
	[SerializeField] private Bubble _bubble;

	[Space]
	[SerializeField] private PlacedBubble _placedBallPrefab;
	[SerializeField] private string _dataFileName;
	[SerializeField] private Palette _palette;

	[Header("Presenters")]
	[SerializeField] private GamePresenter _gamePresenter;
	[SerializeField] private SlingshotPathPresenter _slingshotPathPresenter;
	[SerializeField] private SlingshotRubberPresenter _slingshotRubberPresenter;
	[SerializeField] private CurrentScorePresenter _currentScorePresenter;
	[SerializeField] private GameEndPresenter _gameEndPresenter;

	[Header("Masks")]
	[SerializeField] private LayerMask _wallMask;
	[SerializeField] private LayerMask _deadZoneMask;
	[SerializeField] private LayerMask _placedBallMask;

	private float _leftBorder, _rightBorder;
	private Vector2 _startPos;
	private Game _game;
	private GameField _gameField;
	private CollisionProcessor _collisionProcessor;
	private BubbleDropper _bubbleDropper;

	private void Awake()
	{
		SetupField();
		_slingshot.Init(_bubble, _leftBorder, _rightBorder);
		_collisionProcessor = new CollisionProcessor(_bubble, _wallMask, _deadZoneMask, _placedBallMask);
		_game = new Game(_shotsCount, _bubble, _slingshot, _gameField, _palette);
		_bubbleDropper = new BubbleDropper(_bottomDeadzone.transform.position.y - 1, _bubblesDropSpeed);

		_gamePresenter.Inject(_game);
		_slingshotPathPresenter.Inject(_slingshot);
		_slingshotRubberPresenter.Inject(_slingshot);
		_currentScorePresenter.Inject(_scoreManager);
		_gameEndPresenter.Inject(_scoreManager);

		_bubble.OnCollisioned += _collisionProcessor.ProcessCollision;
		_collisionProcessor.OnBubbleShouldReturn += _game.ReturnBubble;
		_collisionProcessor.OnCollisionWithBubble += _game.ProcessBubblesColision;
		_gameField.OnBubblesFalling += _bubbleDropper.DropBubbles;
		_gameField.OnBubblesDestroyed += _scoreManager.AddScores;
		_game.OnGameEnded += _gameEndPresenter.HandleGameEnded;
	}

	private void SetupField()
	{
		_gameField = new GameField();
		var colors = SimpleTextParser.Parse(_dataFileName, _palette);
		SetupBordersAndCamera(colors.Length, colors[0].Length);

		ComponentPool<PlacedBubble> pool = new PlacedBubblePool(_placedBallPrefab);
		pool.Preload(colors.Length * colors[0].Length + 10);
		_gameField.Init(colors, pool, _startPos);
	}

	private void SetupBordersAndCamera(float height, float fieldWidth)
	{
		_camera.orthographicSize = height / 2 + _slingshot.MaxStretchDistance * 2 + 1;
		_startPos.x = 0;
		_startPos.y = (int)_camera.orthographicSize - 1;

		var halfWidth = _camera.orthographicSize * _camera.aspect;

		_camera.transform.position = new Vector3(fieldWidth / 2 - 0.25f, 0, -10);
		_slingshot.transform.position = new Vector3(_camera.transform.position.x, -_camera.orthographicSize + _slingshot.MaxStretchDistance + 2, 0);

		_leftWall.transform.localScale = new Vector3(1, _camera.orthographicSize * 2);
		_leftWall.transform.position = new Vector3(_camera.transform.position.x - (fieldWidth / 2) - (_leftWall.size.x / 2) - 0.5f, 0, 0);

		_rightWall.transform.localScale = new Vector3(1, _camera.orthographicSize * 2);
		_rightWall.transform.position = new Vector3(_camera.transform.position.x + (fieldWidth / 2) + (_rightWall.size.x / 2) + 0.5f, 0, 0);

		_upperDeadzone.transform.position = new Vector3(halfWidth, _camera.orthographicSize, 0);
		_upperDeadzone.transform.localScale = new Vector3(halfWidth * 2, 1, 1);

		_bottomDeadzone.transform.position = new Vector3(halfWidth, -_camera.orthographicSize, 0);
		_bottomDeadzone.transform.localScale = new Vector3(halfWidth * 2, 1, 1);

		_leftBorder = _leftWall.transform.position.x + (_leftWall.size.x / 2);
		_rightBorder = _rightWall.transform.position.x - (_rightWall.size.x / 2);

		_leftRubber.position = new Vector3(_leftBorder + 0.5f, _slingshot.transform.position.y + 0.5f);
		_rightRubber.position = new Vector3(_rightBorder - 0.5f, _slingshot.transform.position.y + 0.5f);
	}

	private void OnDestroy()
	{
		_bubble.OnCollisioned -= _collisionProcessor.ProcessCollision;
		_collisionProcessor.OnBubbleShouldReturn -= _game.ReturnBubble;
		_collisionProcessor.OnCollisionWithBubble -= _game.ProcessBubblesColision;
		_gameField.OnBubblesFalling -= _bubbleDropper.DropBubbles;
		_gameField.OnBubblesDestroyed -= _scoreManager.AddScores;
		_game.OnGameEnded -= _gameEndPresenter.HandleGameEnded;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (_gameField != null)
			_gameField.DrawGizmos();
	}
#endif
}