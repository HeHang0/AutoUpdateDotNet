﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdate.Core
{
    public class GithubChecker : IChecker
    {
        private string owner;
        private string repository;
        private string asset;
        private (int major, int minor, int point) version;
        private (int major, int minor, int point) remoteVersion;
        private string remoteDownloadUrl;
        private string localFilePath;
        private string ListReleaseUrl => $"https://api.github.com/repos/{owner}/{repository}/releases?per_page=1";

        public GithubChecker(string owner, string repository, string asset, string version)
        { 
            this.owner = owner;
            this.repository = repository;
            this.asset = asset;
            this.version = ParseVersion(version);
        }

        public async Task<(bool, string)> CheckUpdate()
        {
            remoteDownloadUrl = string.Empty;
            var releases = await Utils.HttpGet(ListReleaseUrl);
            var release = releases?.ArrayFirst();
            if(release == null) return (false, string.Empty);
            var tagName = release?.GetProperty("tag_name").GetString();
            if (tagName == null) return (false, string.Empty);
            (int major, int minor, int point) newVersion = ParseVersion(tagName.ToString());
            if (!CompareVersion(version, newVersion)) return (false, string.Empty);
            var assets = release?.GetProperty("assets");
            if (assets == null) return (false, string.Empty);
            foreach (var assetItem in assets?.EnumerateArray())
            {
                var name = assetItem.GetProperty("name").GetString();
                if(name == asset)
                {
                    var browserDownloadUrl = assetItem.GetProperty("browser_download_url").GetString();
                    if (browserDownloadUrl == null) return (false, string.Empty);
                    remoteDownloadUrl = browserDownloadUrl;
                    remoteVersion = newVersion;
                    return (true, $"{newVersion.major}.{newVersion.minor}.{newVersion.point}");
                }
            }
            return (false, string.Empty);
        }

        public bool CanUpdate()
        {
            return !string.IsNullOrWhiteSpace(remoteDownloadUrl);
        }

        public async Task<bool> DownloadPackage(CancellationToken? token,IProgress<int> progress = null)
        {
            var name = Path.GetFileNameWithoutExtension(asset);
            var extension = Path.GetExtension(asset);
            localFilePath = Path.Combine(Path.GetTempPath(), 
                $"{name}v{remoteVersion.major}.{remoteVersion.minor}.{remoteVersion.point}{extension}");
            return await Utils.DownloadFile(remoteDownloadUrl, localFilePath, token, progress);
        }

        public string GetPackagePath()
        {
            return localFilePath;
        }

        private bool CompareVersion((int major, int minor, int point) oldVersion, (int major, int minor, int point) newVersion)
        {
            return oldVersion.CompareTo(newVersion) < 0;
            //if (oldVersion.major < newVersion.major)
            //{
            //    return true;
            //}
            //if(oldVersion.major == newVersion.major && oldVersion.minor < newVersion.minor)
            //{
            //    return true;
            //}
            //if (oldVersion.major == newVersion.major && oldVersion.minor == newVersion.minor && oldVersion.point < newVersion.point)
            //{
            //    return true;
            //}
            //return false;
        }

        private (int major, int minor, int point) ParseVersion(string version)
        {
            int major = 0, minor = 0, point = 0;
            version = Regex.Replace(version, @"[^\.0-9]", "", RegexOptions.IgnoreCase).Trim().Trim('.');
            var versions = version.Split('.');
            int length = versions.Length > 3 ? 3 : versions.Length;
            switch (length)
            {
                case 3:
                    int.TryParse(versions[2], out point);
                    goto case 2;
                case 2:
                    int.TryParse(versions[1], out minor);
                    goto case 1;
                case 1:
                    int.TryParse(versions[0], out major);
                    break;
            }
            return (major, minor, point);
        }
    }
}
