using System;
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

        [MenuItem("Tools/YooAsset/Rebuild Full Package", priority = 100)]
        public static void RebuildFullPackage()
        {
            ExecuteBuild(copyBuiltinToStreamingAssets: false, forceCleanPackageRoot: true);
        }

        [MenuItem("Tools/YooAsset/Prepare Builtin To StreamingAssets", priority = 101)]
        public static void PrepareBuiltinToStreamingAssets()
        {
            ExecuteBuild(copyBuiltinToStreamingAssets: true, forceCleanPackageRoot: false);
        }

        private static void ExecuteBuild(bool copyBuiltinToStreamingAssets, bool forceCleanPackageRoot)
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

                ScriptableBuildParameters buildParameters = CreateBuildParameters(packageName, packageVersion, buildTarget, copyBuiltinToStreamingAssets);
                string packageRootDirectory = buildParameters.GetPackageRootDirectory();

                if (forceCleanPackageRoot)
                {
                    CleanPackageRootDirectory(packageRootDirectory);
                }

                Debug.Log($"[YooAssetBuildTool] Start build. Package:{packageName}, Target:{buildTarget}, OutputRoot:{buildParameters.BuildOutputRoot}, OutputPath:{buildParameters.GetPackageOutputDirectory()}, CopyBuiltin:{copyBuiltinToStreamingAssets}, BuildinTag:{BuiltinTag}");

                var pipeline = new ScriptableBuildPipeline();
                BuildResult buildResult = pipeline.Run(buildParameters, true);

                if (buildResult.Success == false)
                {
                    Debug.LogError($"[YooAssetBuildTool] Build failed. Package:{packageName}, Target:{buildTarget}, OutputPath:{buildParameters.GetPackageOutputDirectory()}, Error:{buildResult.ErrorInfo}");
                    return;
                }

                Debug.Log($"[YooAssetBuildTool] Build succeeded. Package:{packageName}, Target:{buildTarget}, OutputPath:{buildResult.OutputPackageDirectory}");

                if (copyBuiltinToStreamingAssets)
                {
                    string buildinRootDirectory = buildParameters.GetBuildinRootDirectory();
                    int copiedFileCount = CountFiles(buildinRootDirectory);
                    Debug.Log($"[YooAssetBuildTool] Builtin prepared. Package:{packageName}, Target:{buildTarget}, Source:{buildResult.OutputPackageDirectory}, Destination:{buildinRootDirectory}, CopiedFileCount:{copiedFileCount}");
                }

                AssetDatabase.Refresh();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[YooAssetBuildTool] Unexpected failure. Reason:{ex}");
            }
        }

        private static ScriptableBuildParameters CreateBuildParameters(string packageName, string packageVersion, BuildTarget buildTarget, bool copyBuiltinToStreamingAssets)
        {
            var buildParameters = new ScriptableBuildParameters
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
                BuildinFileCopyOption = copyBuiltinToStreamingAssets ? EBuildinFileCopyOption.ClearAndCopyByTags : EBuildinFileCopyOption.None,
                BuildinFileCopyParams = BuiltinTag,
                CompressOption = ECompressOption.LZ4,
                ClearBuildCacheFiles = false,
                UseAssetDependencyDB = true
            };

            return buildParameters;
        }

        private static void CleanPackageRootDirectory(string packageRootDirectory)
        {
            if (Directory.Exists(packageRootDirectory) == false)
            {
                return;
            }

            Directory.Delete(packageRootDirectory, true);
            Debug.Log($"[YooAssetBuildTool] Cleaned old package output directory : {packageRootDirectory}");
        }

        private static string ResolvePackageName()
        {
            var runtimeConfig = GlobalSetting.RuntimeConfig;
            if (runtimeConfig != null && string.IsNullOrEmpty(runtimeConfig.PackageName) == false)
            {
                return runtimeConfig.PackageName;
            }

            return string.Empty;
        }

        private static string ResolvePackageVersion()
        {
            string version = GlobalSetting.ResourceVersion;
            if (string.IsNullOrEmpty(version))
            {
                version = DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            return version;
        }

        private static int CountFiles(string folderPath)
        {
            if (Directory.Exists(folderPath) == false)
            {
                return 0;
            }

            return Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories).Length;
        }
    }
}
