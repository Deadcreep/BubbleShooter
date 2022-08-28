using System.Collections.Generic;
using UnityEngine;

public class UnityObjectPool<T> where T : Object
{
	protected Stack<T> stack = new Stack<T>();
	protected T origin;

	protected UnityObjectPool(T origin)
	{
		this.origin = origin;
	}

	public virtual void Preload(int count)
	{
		List<T> items = new List<T>();
		for (int i = 0; i < count; i++)
		{
			items.Add(Get());
		}
		foreach (var item in items)
		{
			Return(item);
		}
	}

	public T Get()
	{
		T item;
		if (stack.Count == 0)
		{
			item = Create();
		}
		else
		{
			item = stack.Pop();
		}
		BeforeGet(item);		
		return item;
	}

	public void Return(T item)
	{
		BeforeReturn(item);
		stack.Push(item);
	}

	protected virtual T Create()
	{
		return GameObject.Instantiate<T>(origin);
	}

	protected virtual void BeforeGet(T item)
	{
	}

	protected virtual void BeforeReturn(T item)
	{
	}
}