using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;



namespace SilverShieldFix
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Name = "SilverShieldFix";
        public const string Guid = "StrykeDev." + Name;
        public const string Version = "1.0.1.0";

        public static ConfigEntry<int> nexusID;

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            nexusID = Config.Bind<int>("Nexus", "NexusID", 126, "Nexus mod ID for updates");
            Logger = base.Logger;
            SceneManager.sceneLoaded += TextureReplacement.OnSceneLoaded;
        }
    }

    internal static class TextureReplacement
    {
        private static readonly Texture2D tex2d = new Texture2D(2, 2, TextureFormat.RGBA32, true, false);

        static TextureReplacement()
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("SilverShieldFix.Assets.Shield_Designs_Decals.png");
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            tex2d.LoadImage(buffer);
            tex2d.filterMode = FilterMode.Point;
        }

        internal static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            HandleTextures();
        }

        internal static void HandleTextures()
        {
            Material[] materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (Material material in materials)
            {
                string[] propertyNames = material.GetTexturePropertyNames();
                foreach (string propertyName in propertyNames)
                {
                    Texture texture = material.GetTexture(propertyName);

                    if (texture != null)
                    {
                        if (texture.name == "Shield_Designs_Decals")
                        {
                            material.SetTexture(propertyName, tex2d);
                        }
                    }
                }
            }
        }
    }
}
