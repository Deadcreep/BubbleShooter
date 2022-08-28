using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
	public int CurrentScore
	{
		get => _currentScore;
		private set
		{
			if (_currentScore != value)
			{
				_currentScore = value;
				OnScoreIncreased?.Invoke(_currentScore);
				CheckHighScores();
			}
		}
	}

	[SerializeField] private int _currentScore;
	[SerializeField] private int _scorePlaces = 10;

	[Space, SerializeField] private float _scorePerBall = 100;
	[SerializeField] private float _chainFactor = 1.2f;

	private int[] _scores;

	public event Action<int> OnScoreIncreased;

	private void Awake()
	{
		_scores = new int[_scorePlaces];
	}

	public void AddScores(int bubblesCount)
	{
		CurrentScore += (int)(_scorePerBall * bubblesCount * _chainFactor);
	}

	public void CheckHighScores()
	{
		for (int i = 0; i < _scores.Length; i++)
		{
			if (CurrentScore > _scores[i])
			{
				if (i < _scores.Length - 1)
				{
					for (int j = _scores.Length - 1; j > i; j--)
					{
						_scores[j] = _scores[j - 1];
					}
					_scores[i] = CurrentScore;
					break;
				}
			}
		}
	}

	private void OnDestroy()
	{
		ScoreSerializer.Serialize(_scores);
	}
}