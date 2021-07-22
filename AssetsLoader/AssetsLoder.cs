using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Reflection;
using System.Collections.Generic;



namespace AssetsLoader
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Name = "AssetsLoader";
        public const string Guid = "StrykeDev." + Name;
        public const string Version = "1.0.0.0";

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            Logger = base.Logger;
            SceneManager.sceneLoaded += LoadAssets.OnSceneLoaded;
        }
    }

    internal static class LoadAssets
    {
        private static readonly Dictionary<string, Material> newMaterials = new Dictionary<string, Material>();
        private static readonly Dictionary<string, Mesh> newMeshes = new Dictionary<string, Mesh>();
        private static readonly Mesh newMesh;
        static LoadAssets()
        {
            string assetsPath = Path.Combine(Path.GetDirectoryName(typeof(Plugin).Assembly.Location), "AssetsLoader");
            Dictionary<string, AssetBundle> assetbundles = new Dictionary<string, AssetBundle>();

            // Create a new AssetsLoader folder if there's none.
            if (!Directory.Exists(assetsPath))
            {
                Directory.CreateDirectory(assetsPath);
                Plugin.Logger.LogInfo("AssetsLoader folder created, Add assets and restart the game.");
                return;
            }

            // Verify the asset bundles.
            foreach (string file in Directory.GetFiles(assetsPath))
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
                if (assetBundle == null)
                {
                    Plugin.Logger.LogError("Failed to load " + Path.GetFileName(file));
                    continue;
                }
                assetbundles.Add(Path.GetFileName(file), assetBundle);
            }

            // Load materials
            //foreach (var assetbundle in assetbundles)
            //{
            //    Plugin.Logger.LogInfo(assetbundle.Key + " loaded!");
            //    foreach (Material asset in assetbundle.Value.LoadAllAssets<Material>())
            //    {
            //        newMaterials.Add(asset.name, asset);
            //        Plugin.Logger.LogInfo(asset.name);
            //    }
            //}

            //Load meshes
            foreach (var assetbundle in assetbundles)
            {
                Plugin.Logger.LogInfo(assetbundle.Key + " loaded!");
                foreach (GameObject asset in assetbundle.Value.LoadAllAssets<GameObject>())
                {
                    //newMeshes.Add(asset.name, asset);
                    //Plugin.Logger.LogInfo("Loading " + asset.name);
                    //newMesh = asset.GetComponent<SkinnedMeshRenderer>().sharedMesh;
                    //Plugin.Logger.LogInfo("loaded " + newMesh.name);
                }
            }
        }

        internal static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            HandleAssets();
        }

        internal static void HandleAssets()
        {
            //Renderer[] rendererss = Resources.FindObjectsOfTypeAll<Renderer>();
            //foreach (Renderer renderer in rendererss)
            //{
            //    string matName = renderer.material.name.Replace(" (Instance)", "");
            //    if (newMaterials.ContainsKey(matName))
            //    {
            //        renderer.material = newMaterials[matName];

            //        //foreach (string property in newMaterials[matName].GetTexturePropertyNames())
            //        //{
            //        //    if (property == "_BumpMap")
            //        //    {
            //        //        continue;
            //        //    }
            //        //    Plugin.Logger.LogDebug("replacing " + property);
            //        //    renderer.material.SetTexture(property, newMaterials[matName].GetTexture(property));
            //        //}
            //        //renderer.material.SetColor("_Color", newMaterials[matName].GetColor("_Color"));
            //    }
            //}

            GameObject[] meshes = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject mesh in meshes)
            {
                if (mesh.name == "CapeDeerHide")
                {
                    Plugin.Logger.LogInfo("found " + mesh.name);
                    foreach (SkinnedMeshRenderer item in mesh.GetComponents(typeof(SkinnedMeshRenderer)))
                    {
                        Plugin.Logger.LogInfo("item " + item.name);
                        Plugin.Logger.LogInfo("item " + item.sharedMesh);
                    }
                    
                    //mesh.sharedMesh = newMesh;
                }
            }
        }
    }
}
