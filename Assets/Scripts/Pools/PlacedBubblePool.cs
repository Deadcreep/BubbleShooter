public class PlacedBubblePool : ComponentPool<PlacedBubble>
{
	public PlacedBubblePool(PlacedBubble origin) : base(origin)
	{
	}

	protected override PlacedBubble Create()
	{
		var bubble = base.Create();
		bubble.Pool = this;
		return bubble;
	}
}