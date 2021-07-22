using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;



namespace BoarCape
{
    [BepInPlugin(Guid, Name, Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string Name = "BoarCape";
        public const string Guid = "StrykeDev." + Name;
        public const string Version = "1.0.0.0";

        internal static new ManualLogSource Logger;

        public void Awake()
        {
            Logger = base.Logger;
            SceneManager.sceneLoaded += MeshReplacement.OnSceneLoaded;
        }
    }

    internal static class MeshReplacement
    {
        private static readonly Dictionary<string, Mesh> beamsMeshes = new Dictionary<string, Mesh>();

        static MeshReplacement()
        {

        }

        private static Mesh LoadMesh(string filename)
        {
            Mesh mesh = new Mesh();
            return new Mesh();
        }

        internal static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            HandleMeshes();
        }

        internal static void HandleMeshes()
        {
            Mesh[] meshes = Resources.FindObjectsOfTypeAll<Mesh>();
            foreach (Mesh mesh in meshes)
            {
            }
        }
    }
}
