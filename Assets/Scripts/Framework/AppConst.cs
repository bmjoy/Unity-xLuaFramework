namespace Framework
{
    public enum GameMode
    {
        EditorMode,
        PackageBundle,
        UpdateMode
    }
    public class AppConst
    {
        public const string BundleExtension = ".ab";
        public const string FileListName = "filelist.txt";
        public static GameMode GameMode = GameMode.EditorMode;
    }
}