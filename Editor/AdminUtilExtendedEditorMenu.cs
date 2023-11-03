using UnityEditor;
using UnityEngine;
using VRC.Udon;

namespace MagmaMc.AdminUtil.Extended.Editor
{
    public class AdminUtilExtendedEditorMenu: MonoBehaviour
    {
        [MenuItem("MagmaMc/AdminUtil/Extended/Create Instance")]
        static void CreateAdminUtilInstance()
        {
            AdminUtilExtendedEditorWindow window = EditorWindow.GetWindow<AdminUtilExtendedEditorWindow>("Create AdminUtil Instance");
            window.Show();
        }
    }

    public class AdminUtilExtendedEditorWindow: EditorWindow
    {
        private Vector3 BannedVec;
        private Material customMaterial;
        private string configURL = "https://magmamcnet.github.io/AdminUtil/PlayerManager.example.txt";

        private void OnGUI()
        {
            customMaterial = AssetDatabase.LoadAssetAtPath<Material>("Packages/net.magma-mc.adminutil.extended/Runtime/Textures/WatchList_Player.mat");
            EditorGUILayout.LabelField("Create AdminUtil Instance", EditorStyles.boldLabel);

            BannedVec = EditorGUILayout.Vector3Field("Banned Position", BannedVec);
            customMaterial = EditorGUILayout.ObjectField("WatchList Player Material", customMaterial, typeof(Material), true) as Material;
            configURL = EditorGUILayout.TextField("Config URL", configURL);

            if (GUILayout.Button("Create AdminUtil Instance"))
            {
                CreateAdminUtilInstance(BannedVec, configURL, customMaterial);
                Close();
            }
        }

        private void CreateAdminUtilInstance(Vector3 bannedObject, string configURL, Material mat)
        {
            GameObject adminUtilObject = new GameObject("AdminUtil");
            adminUtilObject.transform.parent = null;

            adminUtilObject.transform.position = Vector3.zero;
            adminUtilObject.transform.rotation = Quaternion.identity;
            adminUtilObject.transform.localScale = Vector3.one;

            GameObject BanPlayer = new GameObject("BanPlayer");
            BanPlayer.transform.position = bannedObject;
            BanPlayer.transform.parent = adminUtilObject.transform;
            BanPlayer.SetActive(false);
            BanPlayer.AddComponent<ForceStay>().MaxDistance = 50;

            AdminUtilExtended util = adminUtilObject.AddComponent<AdminUtilExtended>();
            UdonBehaviour udonb = adminUtilObject.AddComponent<UdonBehaviour>();
            string path = "Packages/net.magma-mc.adminutil/Runtime/StringDownloader.asset";
            udonb.programSource = AssetDatabase.LoadAssetAtPath<AbstractUdonProgramSource>(path);
            util.StringDownloader = udonb;
            util.BannedObject = BanPlayer; 
            util.ConfigURL = new VRC.SDKBase.VRCUrl(configURL);
            for (int i = 0; i < util.WatchListObjectPool.Length; i++)
            {
                GameObject WatchListObject = new GameObject($"WatchListObject {i}");
                MeshFilter meshFilter = WatchListObject.AddComponent<MeshFilter>();
                WatchListObject.AddComponent<MeshRenderer>().material = mat;

                Mesh mesh = new Mesh();
                mesh.vertices = new Vector3[]
                {
                    new Vector3(-5, 0, -5),
                    new Vector3(-5, 0, 5),
                    new Vector3(5, 0, -5),
                    new Vector3(5, 0, 5)
                };

                mesh.triangles = new int[] { 0, 1, 2, 2, 1, 3 };
                mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1) };
                meshFilter.mesh = mesh;

                WatchListObject.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                WatchList_Object obj = WatchListObject.AddComponent<WatchList_Object>();
                obj.root = util;
                WatchListObject.transform.parent = adminUtilObject.transform;
                util.WatchListObjectPool[i] = obj;
            }

            Selection.activeObject = adminUtilObject;
        }
    }
}
