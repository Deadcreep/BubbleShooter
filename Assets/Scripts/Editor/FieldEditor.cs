using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class FieldEditor : EditorWindow
	{
		private static FieldEditor _instance;
		private TextAsset _file;
		private Palette _palette;
		private List<List<NamedColor>> _field = new List<List<NamedColor>>();
		private Vector2 _scrollPosition;
		private Color _currentColor;

		[MenuItem("Field/Edit field")]
		public static void GetWindow()
		{
			if (_instance)
			{
				_instance.Focus();
			}
			else
			{
				_instance = GetWindow<FieldEditor>();
				_instance.Show();
				var assets = AssetDatabase.FindAssets($"t:{typeof(Palette)}");
				if (assets.Length > 0)
				{
					_instance._palette = AssetDatabase.LoadAssetAtPath<Palette>(AssetDatabase.GUIDToAssetPath(assets[0]));
					_instance._currentColor = _instance._palette.Colors[0].Color;
				}
			}
		}

		private void OnGUI()
		{
			_file = EditorGUILayout.ObjectField(_file, typeof(TextAsset), false) as TextAsset;
			_palette = EditorGUILayout.ObjectField(_palette, typeof(Palette), false) as Palette;
			if (!_file)
			{
				EditorGUILayout.HelpBox("File empty", MessageType.Error);
			}
			if (!_palette)
			{
				EditorGUILayout.HelpBox("Pallete empty", MessageType.Error);
			}
			else
			{
				EditorGUILayout.ColorField("Current color", _currentColor, GUILayout.Width(100));
				_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.Height(40));
				for (int i = 0; i < _palette.Colors.Count; i++)
				{
					NamedColor item = _palette.Colors[i];
					var rect = new Rect(new Vector2(i * 35, 0), new Vector2(30, 30));
					EditorGUI.DrawRect(rect, item.Color);
					if (Event.current.type == EventType.MouseDown)
					{
						var pos = Event.current.mousePosition;
						if (rect.Contains(pos))
						{
							_currentColor = item.Color;
						}
					}
				}
				EditorGUILayout.EndScrollView();
			}
			var minus = EditorGUIUtility.IconContent("Toolbar Minus");
			minus.tooltip = "Remove row";
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Deserialize", GUILayout.Width(90)))
			{
				Deserialize();
			}
			if (GUILayout.Button("Serialize", GUILayout.Width(90)))
			{
				Serialize();
			}
			EditorGUILayout.EndHorizontal();
			var lastRect = GUILayoutUtility.GetLastRect();
			if (_field.Count > 0)
			{
				GUILayoutUtility.GetRect(_field[0].Count * 35, _field.Count * 35 + EditorGUIUtility.singleLineHeight);
			}
			else
			{
				GUILayoutUtility.GetRect(30, 35);
			}
			for (int y = 0; y < _field.Count; y++)
			{
				var yCoord = lastRect.yMax + EditorGUIUtility.singleLineHeight + (y * 35);
				for (int x = 0; x < _field[y].Count; x++)
				{
					var rect = new Rect(new Vector2(y % 2 == 0 ? x * 35 : x * 35 + 17.5f, yCoord), new Vector2(30, 30));
					EditorGUI.DrawRect(rect, _field[y][x].Color);
					if (Event.current.type == EventType.MouseDown)
					{
						var pos = Event.current.mousePosition;
						if (rect.Contains(pos))
						{
							_field[y][x] = new NamedColor()
							{
								Color = _currentColor,
								Name = _palette.GetNameByColor(_currentColor)
							};
						}
					}
				}
				var buttonRect = new Rect(y % 2 == 0 ? _field[y].Count * 35 : _field[y].Count * 35 + 17.5f, yCoord, 30, 30);
				if (GUI.Button(buttonRect, minus))
				{
					_field.RemoveAt(y);
				}
			}
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add row", GUILayout.Width(100)))
			{
				var list = new List<NamedColor>();
				for (int i = 0; i < _field[0].Count; i++)
				{
					list.Add(new NamedColor() { Color = Color.black });
				}
				_field.Add(list);
			}
			if (GUILayout.Button("Add column", GUILayout.Width(100)))
			{
				foreach (var item in _field)
				{
					item.Add(new NamedColor() { Color = Color.black });
				}
			}
			GUILayout.EndHorizontal();
		}

		private void Deserialize()
		{
			if (_file)
			{
				var str = _file.text;
				var rows = str.Split("\r\n");
				_field.Clear();
				for (int y = 0; y < rows.Length; y++)
				{
					var row = rows[y].Split(' ');
					_field.Add(new List<NamedColor>());
					for (int x = 0; x < row.Length; x++)
					{
						_field[y].Add(new NamedColor()
						{
							Color = _palette.GetColorByName(row[x]),
							Name = row[x]
						});
					}
				}
			}
		}

		private void Serialize()
		{
			StringBuilder sb = new StringBuilder();
			for (int y = 0; y < _field.Count; y++)
			{
				//for (int x = 0; x < _field[y].Count; x++)
				//{
				//	sb.Append(_field[y][x].Name + ' ');
				//}
				sb.Append(string.Join(' ', _field[y].Select(x => x.Name)));
				if (y < _field.Count - 1)
					sb.Append(Environment.NewLine);
			}
			File.WriteAllText(AssetDatabase.GetAssetPath(_file), sb.ToString());
			EditorUtility.SetDirty(_file);
			AssetDatabase.SaveAssetIfDirty(_file);
			AssetDatabase.Refresh();
		}
	}
}