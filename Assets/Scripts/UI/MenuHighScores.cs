using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuHighScores : MonoBehaviour
{
	[SerializeField] private int _scorePlaces = 10;	
	[SerializeField] private List<TextMeshProUGUI> _fields;

	private void Awake()
	{
		var scores = new int[_scorePlaces];
		ScoreSerializer.Deserialize(scores);
		for (int i = 0; i < scores.Length; i++)
		{
			_fields[i].text = $"{i + 1}. {scores[i]}";
		}
	}

	private void SetupFields()
	{
		
		for (int i = 0; i < _fields.Count; i++)
		{
			var rect = _fields[i].rectTransform;
			
		}
	}

}