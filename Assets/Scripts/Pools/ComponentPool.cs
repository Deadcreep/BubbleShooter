using UnityEngine;

public class ComponentPool<T> : UnityObjectPool<T> where T : Component
{

	public ComponentPool(T origin) : base(origin)
	{
	}

	protected override void BeforeGet(T item)
	{
		item.gameObject.SetActive(true);
	}

	protected override void BeforeReturn(T item)
	{
		item.gameObject.SetActive(false);
	}

	protected override T Create()
	{
		return GameObject.Instantiate(origin);
	}
}