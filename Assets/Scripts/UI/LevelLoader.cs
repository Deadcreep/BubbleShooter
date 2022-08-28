using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
	[SerializeField] private int _sceneIndex;
	[SerializeField] private bool _askBefore;

	public void LoadLevel()
	{
		if (_askBefore)
		{
			DialogPanel.ShowDialog("Return?", () => SceneManager.LoadScene(_sceneIndex, LoadSceneMode.Single));
		}
		else
		{
			SceneManager.LoadScene(_sceneIndex, LoadSceneMode.Single);
		}
	}
}