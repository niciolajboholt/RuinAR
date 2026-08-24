using System.IO;
using UnityEngine;

namespace RuinAR.Core
{
    public static class OfflineRuinPackageStore
    {
        private const string FileName = "kalo-slotsruin.json";

        public static string PackagePath => Path.Combine(Application.persistentDataPath, FileName);

        public static void Save(RuinSiteData site)
        {
            var json = JsonUtility.ToJson(site, true);
            File.WriteAllText(PackagePath, json);
        }

        public static RuinSiteData LoadOrCreate()
        {
            if (!File.Exists(PackagePath))
            {
                var prototype = RuinSiteData.CreatePrototype();
                Save(prototype);
                return prototype;
            }

            var json = File.ReadAllText(PackagePath);
            return JsonUtility.FromJson<RuinSiteData>(json) ?? RuinSiteData.CreatePrototype();
        }
    }
}

