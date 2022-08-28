public static class ScoreSerializer
{
	private static IScoreSerializator _scoreSerializator;

	public static void Serialize(int[] scores)
	{
		if (_scoreSerializator == null)
		{
			_scoreSerializator = new PlayerPrefsScoreSerializator(scores.Length);
		}
		_scoreSerializator.Serialize(scores);
	}

	public static void Deserialize(int[] scores)
	{
		if (_scoreSerializator == null)
		{
			_scoreSerializator = new PlayerPrefsScoreSerializator(scores.Length);
		}
		_scoreSerializator.Deserialize(scores);
	}
}