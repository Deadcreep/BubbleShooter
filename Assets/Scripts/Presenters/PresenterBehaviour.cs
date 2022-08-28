using UnityEditor.Experimental;
using UnityEngine;

public class PresenterBehaviour<T> : MonoBehaviour, IPresenter<T>
{
	public T Model { get; private set; }

	public void Inject(T model)
	{
		Model = model;
		OnInject();
	}

	protected virtual void OnInject()
	{
	}

	public void RemoveModel()
	{
		OnRemove();
		Model = default;
	}

	protected virtual void OnRemove()
	{
	}

	private void OnDestroy()
	{
		if (Model != null)
			RemoveModel();
	}
}