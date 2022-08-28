using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class GameField
{
	private ComponentPool<PlacedBubble> _pool;
	private Dictionary<Vector2Int, PlacedBubble> _bubbles;
	private Dictionary<Color, int> _colors;
	private int _width;
	private int _rootY;
	private int _rootCountToWin;
	private int _rootRowBubblesCount;

	public event Action<ICollection<PlacedBubble>> OnBubblesFalling;

	public event Action<int> OnBubblesDestroyed;

	public void Init(Color[][] colors, ComponentPool<PlacedBubble> pool, Vector3 startPos)
	{
		_pool = pool;
		_colors = new Dictionary<Color, int>();
		_bubbles = new Dictionary<Vector2Int, PlacedBubble>();
		_width = colors[0].Length;
		_rootRowBubblesCount = colors[0].Length;
		_rootCountToWin = _rootRowBubblesCount / 3;
		for (int y = 0; y < colors.Length; y++)
		{
			var row = colors[y];
			for (int x = 0; x < row.Length; x++)
			{
				var bubble = pool.Get();
				if (!_colors.ContainsKey(row[x]))
				{
					_colors.Add(row[x], 1);
				}
				else
				{
					_colors[row[x]]++;
				}

				Vector3 pos;
				if (y % 2 == 0)
				{
					pos = new Vector3(startPos.x + x, startPos.y - y, 0);
				}
				else
				{
					pos = new Vector3(startPos.x + x + 0.5f, startPos.y - y, 0);
				}

				var index = MathUtils.RoundVector(pos);
				bubble.Setup(pos, row[x], index);
				bubble.OnDestroyed += RemoveBubble;
				bubble.name = index.ToString();
				_bubbles.Add(index, bubble);
			}
		}
		_rootY = _bubbles.Values.First().Index.y;
	}

	public Color GetRandomColor()
	{
		return _colors.Keys.ElementAt(UnityEngine.Random.Range(0, _colors.Count));
	}

	public bool CheckGameIsEnd()
	{
		if (_rootRowBubblesCount <= _rootCountToWin)
		{
			OnBubblesFalling?.Invoke(_bubbles.Values);
			return true;
		}
		return false;
	}

	public void StickBubble(Vector3 position, PlacedBubble contact, Color color)
	{
		if (contact.transform.position.y - position.y < 0.25f)
		{
			if (position.x - contact.transform.position.x < 0)
			{
				position.x = contact.transform.position.x - 1f;
			}
			else
			{
				position.x = contact.transform.position.x + 1f;
			}
			position.y = contact.transform.position.y;
		}
		else
		{
			if (position.x - contact.transform.position.x < 0)
			{
				position.x = contact.transform.position.x - 0.5f;
			}
			else
			{
				position.x = contact.transform.position.x + 0.5f;
			}
			position.y = contact.transform.position.y - 1;
		}

		if (contact.Index.y % 2 == 0)
		{
			position.x = Mathf.Clamp(position.x, 0.5f, _width + 0.5f); //в зависмости от y%2
		}
		else
		{
			position.x = Mathf.Clamp(position.x, 0, _width); //в зависмости от y%2
		}
		var index = MathUtils.RoundVector(position);
		if (!TryRemoveSameColored(color, index))
		{
			AddBubble(position, index, color);
		}
		{
			FindFalling();
		}
	}

	public void ReplaceBubble(PlacedBubble bubble, Color color)
	{
		if (!TryRemoveSameColored(color, bubble.Index))
		{
			bubble.Destroy();
			AddBubble(bubble.transform.position, bubble.Index, color);
		}
		else
		{
			if (bubble.Color != color)
			{
				bubble.Color = color;
				bubble.Destroy();
			}
			else
			{
				Debug.Log($"[GameField] not remove {color}", bubble);
			}
			FindFalling();
		}
	}

	public void RemoveBubble(PlacedBubble bubble)
	{
		if (bubble.Index.y == _rootY)
		{
			_rootRowBubblesCount--;
		}
		_bubbles.Remove(bubble.Index);
		if (_colors.ContainsKey(bubble.Color))
		{
			_colors[bubble.Color]--;
			if (_colors[bubble.Color] == 0)
			{
				_colors.Remove(bubble.Color);
			}
		}
		bubble.OnDestroyed -= RemoveBubble;
	}

	private void AddBubble(Vector3 position, Vector2Int index, Color color)
	{
		var bubble = _pool.Get();		
		bubble.name = index.ToString();
		bubble.Setup(position, color, index);
		bubble.OnDestroyed += RemoveBubble;
		_bubbles.Add(index, bubble);
		if (!_colors.ContainsKey(color))
		{
			_colors.Add(color, 0);
		}
		_colors[color]++;
		if (index.y == _rootY)
		{
			_rootY++;
		}
	}

	private bool TryRemoveSameColored(Color color, Vector2Int index)
	{
		var neighbors = new HashSet<PlacedBubble>();
		GetSameColoredNeighbors(color, index, neighbors);
		if (neighbors.Count > 0)
		{
			Debug.Log($"[GameField] destroy same colored {color} {neighbors.Count} {neighbors.Any(x => x.Index == index)}");
			foreach (var item in neighbors)
			{
				if (item.Index == index)
				{
					Debug.Log($"[GameField] destroy index {index}");
				}
				item.Destroy();
			}
			OnBubblesDestroyed?.Invoke(neighbors.Count);
			return true;
		}
		return false;
	}

	private void FindFalling()
	{
		List<HashSet<PlacedBubble>> islands = new List<HashSet<PlacedBubble>>();
		HashSet<Vector2Int> processedIndeces = new HashSet<Vector2Int>();
		List<Vector2Int> remainingIndeces = _bubbles.Keys.ToList();
		while (remainingIndeces.Count > 0)
		{
			var currentIndex = remainingIndeces[0];
			HashSet<PlacedBubble> island = new HashSet<PlacedBubble>();
			CreateIsland(currentIndex, island, remainingIndeces);
			islands.Add(island);
		}
		foreach (var island in islands)
		{
			if (!island.Any(x => x.Index.y == _rootY))
			{
				OnBubblesFalling?.Invoke(island);
				OnBubblesDestroyed?.Invoke(island.Count);
			}
		}
	}

	private void CreateIsland(Vector2Int currentIndex, HashSet<PlacedBubble> members, List<Vector2Int> remainingIndeces)
	{
		Vector2Int vector = Vector2Int.zero;
		int startIndex;
		int endIndex;
		if ((float)currentIndex.y % 2 == 0)
		{
			startIndex = currentIndex.x - 1;
			endIndex = currentIndex.x;
		}
		else
		{
			startIndex = currentIndex.x;
			endIndex = currentIndex.x + 1;
		}
		for (int y = currentIndex.y - 1; y <= currentIndex.y + 1; y += 2)
		{
			for (int x = startIndex; x <= endIndex; x++)
			{
				vector.x = x;
				vector.y = y;
				if (_bubbles.ContainsKey(vector))
				{
					if (members.Add(_bubbles[vector]))
					{
						remainingIndeces.Remove(vector);
						CreateIsland(vector, members, remainingIndeces);
					}
				}
			}
		}
		for (int x = currentIndex.x - 1; x <= currentIndex.x + 1; x++)
		{
			vector.x = x;
			vector.y = currentIndex.y;
			if (_bubbles.ContainsKey(vector))
			{
				if (members.Add(_bubbles[vector]))
				{
					remainingIndeces.Remove(vector);
					CreateIsland(vector, members, remainingIndeces);
				}
			}
		}
	}

	private void GetSameColoredNeighbors(Color color, Vector2Int index, HashSet<PlacedBubble> neighbors)
	{
		Vector2Int vector = Vector2Int.zero;
		int startIndex;
		int endIndex;
		if ((float)index.y % 2 == 0)
		{
			startIndex = index.x - 1;
			endIndex = index.x;
		}
		else
		{
			startIndex = index.x;
			endIndex = index.x + 1;
		}
		for (int y = index.y - 1; y <= index.y + 1; y += 2)
		{
			for (int x = startIndex; x <= endIndex; x++)
			{
				vector.x = x;
				vector.y = y;
				if (_bubbles.ContainsKey(vector) && _bubbles[vector].Color == color)
				{
					var neighbor = _bubbles[vector];
					if (neighbors.Add(neighbor))
					{
						//_bubbles.Remove(vector);
						GetSameColoredNeighbors(color, new Vector2Int(x, y), neighbors);
					}
				}
			}
		}
		for (int x = index.x - 1; x <= index.x + 1; x++)
		{
			vector.x = x;
			vector.y = index.y;
			if (_bubbles.ContainsKey(vector) && _bubbles[vector].Color == color)
			{
				var neighbor = _bubbles[vector];
				if (neighbors.Add(neighbor))
				{
					//_bubbles.Remove(vector);
					GetSameColoredNeighbors(color, new Vector2Int(x, index.y), neighbors);
				}
			}
		}
	}

#if UNITY_EDITOR

	public void DrawGizmos()
	{
		foreach (var item in _bubbles)
		{
			Gizmos.color = item.Value.Color;
			Gizmos.DrawCube(item.Value.transform.position, new Vector3(0.1f, 0.1f, 0));
			UnityEditor.Handles.Label(item.Value.transform.position, item.Key.ToString());
		}
	}

#endif
}