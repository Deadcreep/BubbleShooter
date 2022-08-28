using System;
using UnityEngine;

public class Game
{
	public int RemainingShots { get; private set; }
	public Color NextColor { get; private set; }
	private Bubble _bubble;
	private Slingshot _slingshot;
	private GameField _field;
	private Palette _palette;

	public event Action OnBubbleReturned;

	public event Action<bool> OnGameEnded;

	public Game(int remainingShots, Bubble bubble, Slingshot slingshot, GameField field, Palette palette)
	{
		RemainingShots = remainingShots;
		_bubble = bubble;
		_slingshot = slingshot;
		_field = field;
		_palette = palette;
		_bubble.Color = _palette.GetRandomColor();
		NextColor = _palette.GetRandomColor();
	}

	public void ReturnBubble()
	{
		_bubble.Stop();
		_bubble.Color = NextColor;
		_slingshot.PrepareToShoot();
		NextColor = _field.GetRandomColor();
		RemainingShots--;
		OnBubbleReturned?.Invoke();
		if (RemainingShots == 0)
		{
			OnGameEnded?.Invoke(false);
		}
	}

	public void ProcessBubblesColision(Vector3 movingBubblePos, PlacedBubble contactedBubble)
	{
		if (_slingshot.IsFullSpeed)
		{
			_bubble.Stop();
			_bubble.MoveTo(contactedBubble.transform.position, () => OnBubbleMoved(contactedBubble));
		}
		else
		{
			_field.StickBubble(movingBubblePos, contactedBubble, _bubble.Color);
			ReturnBubble();
			if (_field.CheckGameIsEnd())
			{
				OnGameEnded?.Invoke(true);
			}
		}
	}

	private void OnBubbleMoved(PlacedBubble contact)
	{
		_field.ReplaceBubble(contact, _bubble.Color);
		ReturnBubble();
		if (_field.CheckGameIsEnd())
		{
			OnGameEnded?.Invoke(true);
		}
	}
}