using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections.Generic;



namespace TrollArmorRework
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Name = "TrollArmorRework";
        public const string Guid = "StrykeDev." + Name;
        public const string Version = "1.3.0.0";

        public static ConfigEntry<bool> mud;
        public static ConfigEntry<int> stars;
        public static ConfigEntry<int> nexusID;

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            mud = Config.Bind("Appearance", "Mud", false, "Whether or not to be covered in mud.");
            stars = Config.Bind("Appearance", "Stars", 0, "Which troll used to make this armor? (0 for blue, 1 for green, 2 for purple, 3 for the rare black, 4 for the mythical white)");
            nexusID = Config.Bind<int>("Nexus", "NexusID", 175, "Nexus mod ID for updates");
            Logger = base.Logger;
            SceneManager.sceneLoaded += TextureReplacement.OnSceneLoaded;
        }
    }

    internal static class TextureReplacement
    {
        private static readonly Dictionary<string, Dictionary<string, Texture2D>> newMaterials = new Dictionary<string, Dictionary<string, Texture2D>>();

        static TextureReplacement()
        {
            string color;
            switch (Plugin.stars.Value)
            {
                case 1:
                    color = "_green.png";
                    break;
                case 2:
                    color = "_purple.png";
                    break;
                case 3:
                    color = "_black.png";
                    break;
                case 4:
                    color = "_white.png";
                    break;
                default:
                    color = "_blue.png";
                    break;
            }

            newMaterials.Add("helmet_trollleather", new Dictionary<string, Texture2D>()
            {
                { "_MainTex", CreateTexture2D("helmet_trollleather_d" + color) }
            });
            newMaterials.Add("CapeTrollHide", new Dictionary<string, Texture2D>()
            {
                { "_MainTex", CreateTexture2D("troll_diffuse" + color) },
                { "_BumpMap", CreateTexture2D("troll_n.png",TextureFormat.BC5) }
            });
            newMaterials.Add("TrollLeatherChest", new Dictionary<string, Texture2D>()
            {
                { "_ChestTex", CreateTexture2D(Plugin.mud.Value ? "TrollLeatherArmorChest_mud_d" + color : "TrollLeatherArmorChest_d" + color) },
                { "_ChestBumpMap", CreateTexture2D("TrollLeatherArmorChest_n.png",TextureFormat.BC5) },
            });
            newMaterials.Add("TrollLeatherPants", new Dictionary<string, Texture2D>()
            {
                { "_LegsTex", CreateTexture2D("TrollLeatherArmorLegs_d" + color) }
            });
        }

        private static Texture2D CreateTexture2D(string filename, TextureFormat format = TextureFormat.DXT5)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Plugin.Name + ".Assets." + filename);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            Texture2D tex2d = new Texture2D(2, 2, format, true, false);
            tex2d.LoadImage(buffer);
            tex2d.filterMode = FilterMode.Point;
            return tex2d;
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
                string matName = material.name;
                if (newMaterials.ContainsKey(matName))
                {
                    foreach (var texture in newMaterials[matName])
                    {
                        material.SetTexture(texture.Key, texture.Value);
                    }
                    material.SetColor("_Color", Color.white);
                }
            }
        }
    }
}
