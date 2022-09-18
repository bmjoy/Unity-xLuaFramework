using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Framework.Util;
using UnityEngine;
using UnityEngine.Networking;
using UObject = UnityEngine.Object;

namespace Framework
{
    public class HotUpdate : MonoBehaviour
    {
        private void Start()
        {
            if (IsFirstInstall())
                ReleaseResources();
            else
                CheckUpdate();
        }

        /// <summary>
        ///     下载单个文件
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IEnumerator DownloadFile(DownloadInfo info, Action<DownloadInfo> OnComplete)
        {
            var web_request = UnityWebRequest.Get(info.URL);
            yield return web_request.SendWebRequest();

            if (web_request.result is UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"Download error: {info.URL}");
                yield break;
                // TODO 重试
            }

            info.Handler = web_request.downloadHandler;
            OnComplete?.Invoke(info);
            web_request.Dispose();
        }

        /// <summary>
        ///     下载多个文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="OnComplete"></param>
        /// <returns></returns>
        private IEnumerator DownloadFiles(List<DownloadInfo> infos, Action<DownloadInfo> OnComplete, Action AllComplete)
        {
            foreach (var info in infos) yield return DownloadFile(info, OnComplete);

            AllComplete?.Invoke();
        }

        /// <summary>
        ///     获取文件信息
        /// </summary>
        /// <returns></returns>
        private List<DownloadInfo> GetFileList(string file_list_data, string path)
        {
            var content = file_list_data.Trim().Replace("\r", "");
            var files = content.Split("\n");
            var download_infos = new List<DownloadInfo>(files.Length);
            foreach (var file in files)
            {
                var info = file.Split("|");
                var download_info = new DownloadInfo
                {
                    Name = info[1],
                    URL = Path.Combine(path, info[1])
                };
                download_infos.Add(download_info);
            }

            return download_infos;
        }

        private bool IsFirstInstall()
        {
            // 判断只读目录是否存在版本文件
            var exists_in_read_path = FileUtil.Exists(Path.Combine(PathUtil.ReadPath, AppConst.FileListName));
            // 判断可读写目录是否存在版本文件
            var exists_in_read_write_path =
                FileUtil.Exists(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName));

            return exists_in_read_path && !exists_in_read_write_path;
        }

        private void ReleaseResources()
        {
            // 下载只读目录的 filelist
            var url = Path.Combine(PathUtil.ReadPath, AppConst.FileListName);
            
            // TODO 在 Mac 中测试
            url = url.Substring(url.IndexOf("Assets"));

            var file_list_info = new DownloadInfo
            {
                URL = url
            };

            // 下载文件
            StartCoroutine(DownloadFile(file_list_info, DownloadOtherFiles));

            void DownloadOtherFiles(DownloadInfo read_path_file_list_info)
            {
                var read_file_data = read_path_file_list_info.Handler.data;
                var file_infos = GetFileList(read_path_file_list_info.Handler.text, PathUtil.ReadPath);

                // TODO 在 Mac 中测试
                foreach (var file_info in file_infos)
                {
                    var url = file_info.URL;
                    file_info.URL = url.Substring(url.IndexOf("Assets"));
                }

                StartCoroutine(DownloadFiles(file_infos, WriteFile, WriteFileList));

                void WriteFile(DownloadInfo file_info)
                {
                    Debug.LogFormat("Releasing: writing file: {0}", file_info.URL);
                    var write_path = Path.Combine(PathUtil.ReadWritePath, file_info.Name);
                    FileUtil.WriteFile(write_path, file_info.Handler.data);
                }

                void WriteFileList()
                {
                    // 把 filelist 写入读写目录
                    FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName),
                        read_file_data);
                    CheckUpdate();
                }
            }
        }

        private void CheckUpdate()
        {
            // 下载远程的 filelist
            var file_list = new DownloadInfo
            {
                URL = Path.Combine(AppConst.ResourcesUrl, AppConst.FileListName)
            };

            StartCoroutine(DownloadFile(file_list, DownloadOtherFiles));

            void DownloadOtherFiles(DownloadInfo remote_file_list_info)
            {
                var remote_file_list_data = remote_file_list_info.Handler.data;
                var remote_file_infos = GetFileList(remote_file_list_info.Handler.text, AppConst.ResourcesUrl);

                List<DownloadInfo> required_files = new();
                foreach (var file_info in remote_file_infos)
                {
                    var local_file = Path.Combine(PathUtil.ReadWritePath, file_info.Name);
                    if (!FileUtil.Exists(local_file))
                    {
                        file_info.URL = Path.Combine(AppConst.ResourcesUrl, file_info.Name);
                        required_files.Add(file_info);
                    }
                }

                if (required_files.Count > 0)
                    StartCoroutine(DownloadFiles(required_files, WriteFile, WriteFileList));
                else
                    EnterGame();

                void WriteFile(DownloadInfo file_info)
                {
                    Debug.LogFormat("Updating: writing file: {0}", file_info.URL);

                    var write_path = Path.Combine(PathUtil.ReadWritePath, file_info.Name);
                    FileUtil.WriteFile(write_path, file_info.Handler.data);
                }

                void WriteFileList()
                {
                    FileUtil.WriteFile(Path.Combine(PathUtil.ReadWritePath, AppConst.FileListName),
                        remote_file_list_data);
                    EnterGame();
                }
            }
        }

        private void EnterGame()
        {
            Manager.Resource.ParseVersionFile();
            Manager.Resource.LoadUI("Login/LoginUI", SetGameObject);

            void SetGameObject(UObject obj)
            {
                var go = Instantiate(obj, transform) as GameObject;
                if (go == null) return;
                go.SetActive(true);
                go.transform.localPosition = Vector3.zero;
            }
        }

        internal class DownloadInfo
        {
            public DownloadHandler Handler;
            public string Name;
            public string URL;
        }
    }
}