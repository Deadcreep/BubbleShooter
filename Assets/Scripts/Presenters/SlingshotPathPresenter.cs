using UnityEngine;

public class SlingshotPathPresenter : PresenterBehaviour<Slingshot>
{
	[SerializeField] private int _pathLength = 20;
	[SerializeField] private float _pathFrame = 0.1f;
	[SerializeField] private float width = 0.5f;
	[SerializeField] private LineRenderer _mainLine;
	[SerializeField] private LineRenderer _spreadLineFirst;
	[SerializeField] private LineRenderer _spreadLineSecond;
	private Vector3[] _mainLineArray;
	private Vector3[] _spreadLineFirstArray;
	private Vector3[] _spreadLineSecondArray;

	private void Awake()
	{
		_mainLineArray = new Vector3[_pathLength];
		_spreadLineFirstArray = new Vector3[_pathLength];
		_spreadLineSecondArray = new Vector3[_pathLength];
		_mainLine.positionCount = _spreadLineFirst.positionCount = _spreadLineSecond.positionCount = _pathLength;
		_mainLine.startWidth = width;
		_spreadLineFirst.startWidth = width;
		_spreadLineSecond.startWidth = width;
	}

	protected override void OnInject()
	{
		Model.OnStateChanged += UpdateView;
		Model.OnSpeedChanged += UpdateSpreadView;
	}

	protected override void OnRemove()
	{
		Model.OnStateChanged -= UpdateView;
		Model.OnSpeedChanged -= UpdateSpreadView;
	}

	private void UpdateView()
	{
		if (Model.State != SlingshotState.Drag)
		{
			_mainLine.enabled = false;
			_spreadLineFirst.enabled = false;
			_spreadLineSecond.enabled = false;
		}
		else
		{
			_mainLine.enabled = true;
		}
	}

	private void UpdateSpreadView()
	{
		_spreadLineFirst.enabled = _spreadLineSecond.enabled = Model.IsFullSpeed;
	}

	private void Update()
	{
		if (Model.State == SlingshotState.Drag)
		{
			if (Model.IsFullSpeed)
			{
				Model.BuildPath(_mainLineArray, _spreadLineFirstArray, _spreadLineSecondArray, _pathFrame);
				_mainLine.SetPositions(_mainLineArray);
				_spreadLineFirst.SetPositions(_spreadLineFirstArray);
				_spreadLineSecond.SetPositions(_spreadLineSecondArray);
			}
			else
			{
				Model.BuildPath(_mainLineArray, _pathFrame);
				_mainLine.SetPositions(_mainLineArray);
			}
		}
	}
}