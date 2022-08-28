using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Pallete", menuName = "Data/Pallete", order = 1)]
public class Palette : ScriptableObject
{
	public List<NamedColor> Colors;

	public Color GetRandomColor()
	{
		return Colors[Random.Range(0, Colors.Count)].Color;
	}

	public string GetNameByColor(Color color)
	{
		return Colors.First(x => x.Color == color).Name;
	}

	public Color GetColorByName(string name)
	{
		for (int i = 0; i < Colors.Count; i++)
		{
			if (Colors[i].Name == name)
				return Colors[i].Color;
		}
		return Color.white;
	}
}

[System.Serializable]
public struct NamedColor
{
	public Color Color;
	public string Name;
}