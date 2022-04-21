namespace CodeBase.Services
{
	public interface IProgressService
	{
		int SceneIndex { get; set; }
		int GetFirstLevelIndex();
	}
}