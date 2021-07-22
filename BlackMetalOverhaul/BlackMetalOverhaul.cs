using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;
using System.Collections.Generic;



namespace BlackMetalOverhaul
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Name = "BlackMetalOverhaul";
        public const string Guid = "StrykeDev." + Name;
        public const string Version = "0.6.2.0";

        public static ConfigEntry<bool> eyes;
        public static ConfigEntry<bool> armor;
        public static ConfigEntry<bool> cape;
        public static ConfigEntry<bool> sword;
        public static ConfigEntry<bool> shields;
        public static ConfigEntry<bool> porcupine;
        public static ConfigEntry<int> nexusID;

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            eyes = Config.Bind("Appearance", "Eyes", true, "Recolor the eyes?");
            armor = Config.Bind("Appearance", "Armor", true, "Retexture the padded armor?");
            cape = Config.Bind("Appearance", "Cape", true, "Retexture the Lox cape?");
            shields = Config.Bind("Appearance", "Shields", true, "Retexture the black metal shields?");
            sword = Config.Bind("Appearance", "Sword", true, "Retexture the black metal sword?");
            porcupine = Config.Bind("Appearance", "Porcupine", true, "Retexture the Porcupine?");
            nexusID = Config.Bind<int>("Nexus", "NexusID", 434, "Nexus mod ID for updates");
            Logger = base.Logger;
            SceneManager.sceneLoaded += TextureReplacement.OnSceneLoaded;
        }
    }

    internal static class TextureReplacement
    {
        private static readonly Dictionary<string, Dictionary<string, Texture2D>> newMaterials = new Dictionary<string, Dictionary<string, Texture2D>>();
        private static readonly Dictionary<string, byte[]> newSprites = new Dictionary<string, byte[]>();

        static TextureReplacement()
        {
            // Armor
            if (Plugin.armor.Value)
            {
                newMaterials.Add("Padded_mat", new Dictionary<string, Texture2D>()
                {
                    { "_MainTex", CreateTexture2D("paddedarmortest_d.png") },
                    { "_BumpMap", CreateTexture2D("paddedarmortest_n.png", TextureFormat.BC5) },
                    { "_MetallicGlossMap", CreateTexture2D("paddedarmortest_m.png")}
                });
                newMaterials.Add("PaddedChest_mat", new Dictionary<string, Texture2D>()
                {
                    { "_ChestTex", CreateTexture2D(Plugin.eyes.Value ? "PaddedArmorChest_d_eyes.png" : "PaddedArmorChest_d.png") },
                    { "_ChestBumpMap", CreateTexture2D("PaddedArmorChest_n.png", TextureFormat.BC5) },
                    { "_ChestMetal", CreateTexture2D("PaddedArmorChest_m.png")}
                });
                newMaterials.Add("PaddedLegs_mat", new Dictionary<string, Texture2D>()
                {
                    { "_LegsTex", CreateTexture2D("Padded_Grieves_d.png") },
                    { "_LegsBumpMap", CreateTexture2D("Padded_Grieves_n.png", TextureFormat.BC5) },
                    { "_LegsMetal", CreateTexture2D("Padded_Grieves_m.png")}
                });
            }

            // Cape
            if (Plugin.cape.Value)
                newMaterials.Add("LoxCape_Mat", new Dictionary<string, Texture2D>()
                {
                    { "_MainTex", CreateTexture2D("LoxCape_D.png") },
                });

            // Shields
            if (Plugin.shields.Value)
                newMaterials.Add("BlackMetalRoundShields_mat", new Dictionary<string, Texture2D>()
                {
                    { "_MainTex", CreateTexture2D("BlackMetalShields_d.png") },
                    { "_BumpMap", CreateTexture2D("BlackMetalShields_n.png", TextureFormat.BC5) },
                    { "_MetallicGlossMap", CreateTexture2D("BlackMetalShields_m.png") }
                });

            // Sword
            if (Plugin.sword.Value)
                newMaterials.Add("blackmetalsword", new Dictionary<string, Texture2D>()
                {
                    { "_MainTex", CreateTexture2D("blackmetalsword_d.png") }
                });

            // Porcupine
            if (Plugin.porcupine.Value)
                newMaterials.Add("maceneedle", new Dictionary<string, Texture2D>()
                {
                    { "_MainTex", CreateTexture2D("maceneedle_d.png") }
                });


            newSprites.Add("HelmetPadded", CreateTextureBuffer("Sprite.HelmetPadded.png"));
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

        private static byte[] CreateTextureBuffer(string filename)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Plugin.Name + ".Assets." + filename);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }

        private static Sprite CreateSprite(string filename)
        {
            Texture2D tex = CreateTexture2D(filename);
            Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
            return sprite;
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
                    material.SetFloat("_Glossiness", .1f);
                    material.SetColor("_MetalColor", Color.white);
                    material.SetFloat("_MetalGloss", .7f);
                }
            }

            Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
            foreach (Sprite sprite in sprites)
            {
                if (newSprites.ContainsKey(sprite.name))
                {
                    Plugin.Logger.LogInfo($"patching {sprite.name}");

                    sprite.texture.LoadImage(newSprites[sprite.name]);
                }
            }
        }
    }
}

