using TMPro;
using UnityEngine;

public class GameEndPresenter : PresenterBehaviour<ScoreManager>
{
	[SerializeField] private GameObject _panel;
	[SerializeField] private TextMeshProUGUI _textField;
	[SerializeField] private TextMeshProUGUI _scoreField;

	public void HandleGameEnded(bool result)
	{
		_textField.text = result ? "Game completed" : "Game over";
		_panel.SetActive(true);
		_scoreField.text = "Scores: " + Model.CurrentScore.ToString();
	}
}