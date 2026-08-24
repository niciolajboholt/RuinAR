using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RuinAR.Editor
{
    [InitializeOnLoad]
    public static class RuinARProjectBootstrapper
    {
        private const string SceneDirectory = "Assets/RuinAR/Scenes";
        private const string ScenePath = SceneDirectory + "/RuinARPrototype.unity";

        static RuinARProjectBootstrapper()
        {
            EditorApplication.delayCall += EnsureProjectSetup;
        }

        [MenuItem("RuinAR/Configure prototype project")]
        public static void EnsureProjectSetup()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            PlayerSettings.companyName = "RuinAR";
            PlayerSettings.productName = "RuinAR Prototype";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "dk.ruinar.prototype");
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.iOS, "dk.ruinar.prototype");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;

            if (!Directory.Exists(SceneDirectory))
                Directory.CreateDirectory(SceneDirectory);

            if (!File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
                Debug.Log("RuinAR: Prototype scene created.");
            }

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };

            AssetDatabase.SaveAssets();
        }
    }
}

