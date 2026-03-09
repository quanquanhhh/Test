using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using GamePlay;
using YooAsset.Editor;

namespace EditorTools
{
    public static class YooAssetBuildEditorTools
    {
        private const string BuiltinTag = "buildin";
        private const string LastBuildOutputPathKey = "YooAssetBuildEditorTools.LastBuildOutputPath";
        private const string LastBuildPackageNameKey = "YooAssetBuildEditorTools.LastBuildPackageName";

        [MenuItem("Tools/YooAssetBuild", priority = 100)]
        public static void YooAssetBuild()
        {
            try
            {
                string packageName = ResolvePackageName();
                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                string packageVersion = ResolvePackageVersion();

                if (string.IsNullOrEmpty(packageName))
                {
                    Debug.LogError("[YooAssetBuildTool] Build aborted. Package name is empty. Configure GlobalSetting.RuntimeConfig.PackageName first.");
                    return;
                }

                ScriptableBuildParameters buildParameters = CreateBuildParameters(packageName, packageVersion, buildTarget);
                Debug.Log($"[YooAssetBuildTool] Start build. Package:{packageName}, Target:{buildTarget}, BuildinTag:{BuiltinTag}");

                var pipeline = new ScriptableBuildPipeline();
                BuildResult buildResult = pipeline.Run(buildParameters, true);
                if (buildResult.Success == false)
                {
                    Debug.LogError($"[YooAssetBuildTool] Build failed. Package:{packageName}, Target:{buildTarget}, Error:{buildResult.ErrorInfo}");
                    return;
                }

                if (string.IsNullOrEmpty(buildResult.OutputPackageDirectory))
                {
                    Debug.LogError($"[YooAssetBuildTool] Build succeeded but output path is empty. Package:{packageName}, Target:{buildTarget}");
                    return;
                }

                SaveLastBuildInfo(packageName, buildResult.OutputPackageDirectory);

                string buildinRootDirectory = buildParameters.GetBuildinRootDirectory();
                int copiedFileCount = CountFiles(buildinRootDirectory);

                Debug.Log($"[YooAssetBuildTool] Build succeeded. Package:{packageName}, Target:{buildTarget}, OutputPath:{buildResult.OutputPackageDirectory}");
                Debug.Log($"[YooAssetBuildTool] Builtin prepared. Package:{packageName}, Target:{buildTarget}, Source:{buildResult.OutputPackageDirectory}, Destination:{buildinRootDirectory}, CopiedFileCount:{copiedFileCount}");

                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[YooAssetBuildTool] Unexpected failure. Reason:{ex}");
            }
        }

        [MenuItem("Tools/Bundle上传", priority = 101)]
        public static void UploadLastBuildBundlesToS3()
        {
            try
            {
                if (TryGetLastBuildInfo(out string packageName, out string outputPath) == false)
                {
                    Debug.LogError("[YooAssetBuildTool] Upload aborted. Not found last successful build output path. Run Tools/YooAssetBuild first.");
                    return;
                }

                if (Directory.Exists(outputPath) == false)
                {
                    Debug.LogError($"[YooAssetBuildTool] Upload aborted. Last build output path not found : {outputPath}");
                    return;
                }

                string bucket = Environment.GetEnvironmentVariable("YOOASSET_S3_BUCKET");
                string prefix = Environment.GetEnvironmentVariable("YOOASSET_S3_PREFIX");
                string region = Environment.GetEnvironmentVariable("AWS_REGION");
                string endpoint = Environment.GetEnvironmentVariable("YOOASSET_S3_ENDPOINT");

                if (string.IsNullOrEmpty(bucket))
                {
                    Debug.LogError("[YooAssetBuildTool] Upload aborted. Missing env YOOASSET_S3_BUCKET.");
                    return;
                }

                string s3Target = BuildS3Target(bucket, prefix);
                int fileCount = CountFiles(outputPath);
                Debug.Log($"[YooAssetBuildTool] Upload start. Package:{packageName}, Source:{outputPath}, FileCount:{fileCount}, Target:{s3Target}, Region:{region}, Endpoint:{endpoint}");

                string arguments = BuildAwsS3SyncArguments(outputPath, s3Target, region, endpoint);
                int exitCode = RunProcess("aws", arguments, out string stdOutput, out string stdError);

                if (exitCode != 0)
                {
                    Debug.LogError($"[YooAssetBuildTool] Upload failed. ExitCode:{exitCode}, Target:{s3Target}, Error:{stdError}\nOutput:{stdOutput}");
                    return;
                }

                Debug.Log($"[YooAssetBuildTool] Upload succeeded. Package:{packageName}, Source:{outputPath}, FileCount:{fileCount}, Target:{s3Target}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[YooAssetBuildTool] Upload unexpected failure. Reason:{ex}");
            }
        }

        private static ScriptableBuildParameters CreateBuildParameters(string packageName, string packageVersion, BuildTarget buildTarget)
        {
            return new ScriptableBuildParameters
            {
                BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(),
                BuildinFileRoot = AssetBundleBuilderHelper.GetStreamingAssetsRoot(),
                BuildPipeline = EBuildPipeline.ScriptableBuildPipeline.ToString(),
                BuildBundleType = (int)EBuildBundleType.AssetBundle,
                BuildTarget = buildTarget,
                PackageName = packageName,
                PackageVersion = packageVersion,
                EnableSharePackRule = true,
                VerifyBuildingResult = true,
                FileNameStyle = EFileNameStyle.HashName,
                BuildinFileCopyOption = EBuildinFileCopyOption.ClearAndCopyByTags,
                BuildinFileCopyParams = BuiltinTag,
                CompressOption = ECompressOption.LZ4,
                ClearBuildCacheFiles = false,
                UseAssetDependencyDB = true
            };
        }

        private static string BuildAwsS3SyncArguments(string outputPath, string s3Target, string region, string endpoint)
        {
            string args = $"s3 sync \"{outputPath}\" \"{s3Target}\"";
            if (string.IsNullOrEmpty(region) == false)
                args += $" --region \"{region}\"";
            if (string.IsNullOrEmpty(endpoint) == false)
                args += $" --endpoint-url \"{endpoint}\"";
            return args;
        }

        private static string BuildS3Target(string bucket, string prefix)
        {
            string target = $"s3://{bucket}";
            if (string.IsNullOrEmpty(prefix) == false)
            {
                target += "/" + prefix.Trim('/');
            }
            return target;
        }

        private static void SaveLastBuildInfo(string packageName, string outputPath)
        {
            EditorPrefs.SetString(LastBuildPackageNameKey, packageName ?? string.Empty);
            EditorPrefs.SetString(LastBuildOutputPathKey, outputPath ?? string.Empty);
            Debug.Log($"[YooAssetBuildTool] Last successful build recorded. Package:{packageName}, OutputPath:{outputPath}");
        }

        private static bool TryGetLastBuildInfo(out string packageName, out string outputPath)
        {
            packageName = EditorPrefs.GetString(LastBuildPackageNameKey, string.Empty);
            outputPath = EditorPrefs.GetString(LastBuildOutputPathKey, string.Empty);
            return string.IsNullOrEmpty(outputPath) == false;
        }

        private static int RunProcess(string fileName, string arguments, out string standardOutput, out string standardError)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            process.Start();
            standardOutput = process.StandardOutput.ReadToEnd();
            standardError = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode;
        }

        private static string ResolvePackageName()
        {
            var runtimeConfig = GlobalSetting.RuntimeConfig;
            if (runtimeConfig != null && string.IsNullOrEmpty(runtimeConfig.PackageName) == false)
                return runtimeConfig.PackageName;

            return string.Empty;
        }

        private static string ResolvePackageVersion()
        {
            string version = GlobalSetting.ResourceVersion;
            if (string.IsNullOrEmpty(version))
                version = DateTime.Now.ToString("yyyyMMddHHmmss");

            return version;
        }

        private static int CountFiles(string folderPath)
        {
            if (Directory.Exists(folderPath) == false)
                return 0;

            return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Length;
        }
    }
}
