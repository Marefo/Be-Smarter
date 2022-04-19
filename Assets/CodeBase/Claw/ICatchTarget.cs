using UnityEngine;

namespace CodeBase.Claw
{
	public interface ICatchTarget
	{
		float SizeY { get; }
		Transform Transform { get; }

		void OnCatch();
	}
}