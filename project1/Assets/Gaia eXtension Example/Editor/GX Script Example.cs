#if GAIA_2_PRESENT && UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Text;

//Insert a reference to your company here
namespace Gaia.GX.ExampleCompany
{
    //<summary>
    //AQUAS setup for Gaia
    //</summary>
    public class GXScriptExample     
    {
        #region Generic informational methods
        /// <summary>
        /// Returns the publisher name if provided. 
        /// This will override the publisher name in the namespace ie Gaia.GX.PublisherName
        /// </summary>
        /// <returns>Publisher name</returns>
        public static string GetPublisherName()
        {
            return "Example Company";
        }

        /// <summary>
        /// Returns the package name if provided
        /// This will override the package name in the class name ie public class PackageName.
        /// </summary>
        /// <returns>Package name</returns>
        public static string GetPackageName()
        {
            return "Gaia Extension Example";
        }
        #endregion

        //<summary>
        //Example Button that gives some info on about your extension
        //</summary>
        public static void GX_About()
        {
            EditorUtility.DisplayDialog("About this Example Extension", "This is an example to demonstrate the creation and usage of Gaia Extensions.", "OK");
        }

        //<summary>
        //Example Button that selects the Gaia Terrain Object
        //</summary>
        public static void GX_SelectTerrainObject()
        {
            Selection.activeObject = GaiaUtils.GetTerrainObject();
        }

        //<summary>
        //Example Button that selects the Gaia Terrain Object
        //</summary>
        public static void GX_SceneInfoExample()
        {
            GaiaSceneInfo info = GaiaSceneInfo.GetSceneInfo();
            Bounds sceneBounds = info.m_sceneBounds;
            Vector3 centrePointOnTerrain = info.m_centrePointOnTerrain;
            float seaLevel = info.m_seaLevel;

            StringBuilder sb = new StringBuilder();
            sb.Append("This is an example to demonstrate the Scene Info object.");
            sb.AppendLine();
            sb.AppendLine();
            sb.Append("Info provided by the Scene Info object:");
            sb.AppendLine();
            sb.AppendFormat("Scene Bounds: X {0} Y {1} Z {2}, Extents:  X {3} Y {4} Z {5}" ,sceneBounds.center.x, sceneBounds.center.y, sceneBounds.center.z, sceneBounds.extents.x, sceneBounds.extents.y, sceneBounds.extents.z);
            sb.AppendLine();
            sb.AppendFormat("Center Point on Terain: X {0} Y {1} Z {2}", centrePointOnTerrain.x, centrePointOnTerrain.y, centrePointOnTerrain.z);
            sb.AppendLine();
            sb.AppendFormat("Sea Level: {0}", seaLevel);

            EditorUtility.DisplayDialog("Scene Info Example", sb.ToString() , "OK");
        }

        //<summary>
        //Adds the example spawner that comes with this package to the scene
        //</summary>
        public static void GX_AddExampleSpawner()
        {
            //Find the example spawner settings file - we use "FindAssets" here, in your own asset 
            //you can of course use more sophisticated methods to get a reference to the settings file
            string[] assets = AssetDatabase.FindAssets("GX Example Spawner", null);
            for (int idx = 0; idx < assets.Length; idx++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assets[idx]);
                try
                {
                    SpawnerSettings spawnerSettings = (SpawnerSettings)AssetDatabase.LoadAssetAtPath(assetPath, typeof(SpawnerSettings));
                    Spawner spawner = spawnerSettings.CreateSpawner();
                    //This is optional, but better UX: We select the spawner we just created so the user can find it easier
                    if (spawner != null)
                    {
                        Selection.activeObject = spawner.gameObject;
                    }
                    else
                    {
                        Debug.LogError("The GX example spawner could not be created in the scene!");
                    }
                }
                catch(Exception ex)
                {
                    Debug.LogError(String.Format("Error while loading the GX Example Spawner. Error Message: {0}", ex.Message));
                }
                
            }
        }
    }
}
#endif
