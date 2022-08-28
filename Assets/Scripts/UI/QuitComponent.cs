using System;
using UnityEngine;

public class QuitComponent : MonoBehaviour
{
	public void Quit()
	{
#if UNITY_EDITOR
		Action action = () => UnityEditor.EditorApplication.isPlaying = false;
#else
		Action action = Application.Quit;
#endif
		DialogPanel.ShowDialog("Quit? ", action);
	}
}