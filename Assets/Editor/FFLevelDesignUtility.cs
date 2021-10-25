/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using UnityEditor;
using UnityEditor.SceneManagement;
using NaughtyAttributes;

namespace FFEditor
{
	[ CreateAssetMenu( fileName = "FFLevelDesignUtility", menuName = "FFEditor/FFLevelDesignUtility" ) ]
	public class FFLevelDesignUtility : ScriptableObject
	{
#region Fields
        [ BoxGroup( "Setup" ) ] public int lastBuildIndex;
        [ BoxGroup( "Setup" ) ] public string[] unwantedObjects;
#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
        [ Button() ]
        public void RemoveUnwantedObjects()
        {
			var scenes = EditorBuildSettings.scenes;

			for( var i = 1; i <= scenes.Length - 1 ; i++ )
            {
				var activeScene = EditorSceneManager.OpenScene( scenes[ i ].path );

				EditorSceneManager.MarkSceneDirty( activeScene );

				var rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

                for( var a = rootObjects.Length - 1; a >= 0 ; a-- )
                {
					var gameobject = rootObjects[ a ];

					for( var b = 0; b < unwantedObjects.Length; b++ )
                    {
                        if( gameobject.name.Contains( unwantedObjects[b] ) )
                        {
                            if( PrefabUtility.IsAnyPrefabInstanceRoot( gameobject ) )
								PrefabUtility.UnpackPrefabInstance( gameobject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction );

							DestroyImmediate( gameobject );
							break;
						}
                    }
                }

				EditorSceneManager.SaveScene( activeScene );
			}
        }
#endregion

#region Implementation

#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
	}
}