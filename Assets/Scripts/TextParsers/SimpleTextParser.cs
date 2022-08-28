using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace TextParsers
{
	public static class SimpleTextParser
	{
		public static Color[][] Parse(string fileName, Palette palette)
		{
			TextAsset text = Resources.Load<TextAsset>(fileName);
			var rows = Regex.Split(text.text, "\r\n");
			Color[][] result = new Color[rows.Length][];
			for (int y = 0; y < rows.Length; y++)
			{
				var row = rows[y].Split(' ');
				result[y] = new Color[row.Length];
				for (int x = 0; x < row.Length; x++)
				{
					result[y][x] = palette.GetColorByName(row[x]);
				}
			}
			return result;
		}
	}
}