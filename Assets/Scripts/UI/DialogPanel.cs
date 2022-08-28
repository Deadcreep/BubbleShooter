using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanel : MonoBehaviour
{
	private static DialogPanel _instance;
	[SerializeField] private TextMeshProUGUI _messageField;
	[SerializeField] private Button _yesButton;
	[SerializeField] private Button _noButton;
	private Action _yesClick;
	private Action _noClick;

	private void Awake()
	{
		if (!_instance)
		{
			_instance = this;
			DontDestroyOnLoad(gameObject);
			_yesButton.onClick.AddListener(InvokeYes);
			_noButton.onClick.AddListener(InvokeNo);
			_instance.gameObject.SetActive(false);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	public static void ShowDialog(string message, Action yesCallback, Action noCallback = null)
	{
		_instance._messageField.text = message;
		_instance.gameObject.SetActive(true);
		_instance._yesClick = yesCallback;
		_instance._noClick = noCallback;
	}

	private void InvokeYes()
	{
		_yesClick?.Invoke();
		_yesClick = null;
		gameObject.SetActive(false);
	}

	private void InvokeNo()
	{
		_noClick?.Invoke();
		_noClick = null;
		gameObject.SetActive(false);
	}
}