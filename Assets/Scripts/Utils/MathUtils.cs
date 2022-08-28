using UnityEngine;

namespace Utils
{
	public static class MathUtils
	{
		public static float Normalize(float value, float min, float max)
		{
			return (value - min) / (max - min);
		}

		public static Vector2Int RoundVector(ref Vector3 vector)
		{
			var x = Mathf.Round(vector.x * 2) / 2;
			var y = Mathf.RoundToInt(vector.y * 2) / 2;
			Debug.Log($"[MathUtils] round {vector} {x} {y}");
			vector.x = x;
			vector.y = y;
			return new Vector2Int((int)x, y);
		}

		public static Vector2Int RoundVector(Vector3 vector)
		{
			var x = Mathf.Round(vector.x * 2) / 2;
			var y = Mathf.RoundToInt(vector.y);
			return new Vector2Int((int)x, y);
		}
	}
}