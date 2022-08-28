using TMPro;
using UnityEngine;

public class CurrentScorePresenter : PresenterBehaviour<ScoreManager>
{
	[SerializeField] private TextMeshProUGUI _scoreField;

	protected override void OnInject()
	{
		Model.OnScoreIncreased += UpdateView;
		_scoreField.text = "Score: " + Model.CurrentScore.ToString();
	}

	protected override void OnRemove()
	{
		Model.OnScoreIncreased -= UpdateView;
	}

	private void UpdateView(int score)
	{
		_scoreField.text = "Score: " + score.ToString();
	}
}