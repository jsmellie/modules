/* --------------------------
 *
 * LoggerData.cs
 *
 * Description: Data class that holds all the current info about active categories and other important info
 *
 * Author: Jeremy Smellie
 *
 * Editors:
 *
 * 7/5/2016 - Starvoxel
 *
 * All rights reserved.
 *
 * -------------------------- */

#region Includes
#region Unity Includes
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

#region System Includes
using System.Collections;
#endregion

#region Other Includes
using Starvoxel.EditorUtilities;
#endregion
#endregion

 namespace Starvoxel.Core
{
    [InitializeOnLoad]
	public class LoggerData : CustomScriptableObject
	{
		#region Fields & Properties
		//const
	
		//public
	
		//protected
	
		//private
	
		//properties
		#endregion
	
		#region Public Methods
        /// <summary>
        /// Static constructor that is used on Unity launch to look at the project and decide if a logger data needs to be created
        /// </summary>
        static LoggerData()
        {
            LoggerData[] datas = Resources.FindObjectsOfTypeAll<LoggerData>();

            if (datas == null || datas.Length == 0)
            {
                //TODO: create a logger data file at a pre-defined path
                Debug.Log("No logger data found.");
            }
        }
		#endregion
	
		#region Protected Methods
		#endregion
	
		#region Private Methods
		#endregion
	
		#region Editor Functions
		#if UNITY_EDITOR
		[MenuItem("Assets/Create/ScriptableObjects/LoggerData", false, 5000)]
		public static void CreateAsset()
		{
			ScriptableObjectUtility.CreateAsset<LoggerData>();
		}
		#endif
		#endregion
	}
	
}