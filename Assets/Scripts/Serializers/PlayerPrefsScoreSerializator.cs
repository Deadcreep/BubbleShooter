using UnityEngine;

public class PlayerPrefsScoreSerializator : IScoreSerializator
{
	private int _scorePlaces;

	public PlayerPrefsScoreSerializator(int scorePlaces)
	{
		_scorePlaces = scorePlaces;
	}

	public void Serialize(int[] scores)
	{
		for (int i = 0; i < scores.Length; i++)
		{
			PlayerPrefs.SetInt($"Scores_{i}", scores[i]);
		}
	}

	public void Deserialize(int[] scores)
	{
		for (int i = 0; i < _scorePlaces; i++)
		{
			scores[i] = PlayerPrefs.GetInt($"Scores_{i}", 0);
		}
	}
}