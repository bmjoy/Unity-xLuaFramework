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

        // 热更资源的地址
        public const string ResourcesUrl = "http://127.0.0.1/AssetBundles";
        public static GameMode GameMode = GameMode.EditorMode;
    }
}