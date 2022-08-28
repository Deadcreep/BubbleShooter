using UnityEngine;

namespace Utils
{
	public static class LayerUtils
	{
		public static bool CheckLayer(this LayerMask mask, int objectLayer)
		{
			return (mask.value & (1 << objectLayer)) > 0;
		}
	}
}