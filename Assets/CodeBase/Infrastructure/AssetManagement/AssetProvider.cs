using UnityEngine;

namespace CodeBase.Infrastructure.AssetManagement
{
	public class AssetProvider
	{
		public AudioClip LoadClip(string path) => Resources.Load<AudioClip>(path);
	}
}