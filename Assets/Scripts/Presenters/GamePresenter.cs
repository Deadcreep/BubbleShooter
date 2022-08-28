using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePresenter : PresenterBehaviour<Game>
{
	[SerializeField] private Image _nextColorView;
	[SerializeField] private TextMeshProUGUI _remainingShotsView;

	protected override void OnInject()
	{
		Model.OnBubbleReturned += UpdateView;
		UpdateView();
	}
	protected override void OnRemove()
	{
		Model.OnBubbleReturned -= UpdateView;
	}

	private void UpdateView()
	{
		_nextColorView.color = Model.NextColor;
		_remainingShotsView.text = Model.RemainingShots.ToString();
	}
}
