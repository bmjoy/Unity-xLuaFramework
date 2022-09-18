using System.IO;
using UnityEngine;

namespace Framework.Util
{
    public class FileUtil
    {
        /// <summary>
        ///     检测文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool Exists(string path)
        {
            var file = new FileInfo(path);
            return file.Exists;
        }

        /// <summary>
        ///     写入文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="data"></param>
        public static void WriteFile(string path, byte[] data)
        {
            path = PathUtil.GetStandardPath(path); // 获取标准路径
            var dir = path.Substring(0, path.LastIndexOf("/")); // 文件夹路径
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var file = new FileInfo(path);
            if (file.Exists) file.Delete();

            try
            {
                using (var file_stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file_stream.Write(data, 0, data.Length);
                    file_stream.Close();
                }
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}