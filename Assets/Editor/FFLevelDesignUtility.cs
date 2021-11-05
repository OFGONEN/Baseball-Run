/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;
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

		[ BoxGroup( "Gate Props" ) ] public string[] positive_names;
		[ BoxGroup( "Gate Props" ) ] public string[] negative_names;
		[ BoxGroup( "Gate Props" ) ] public GameObject[] positive_objects;
		[ BoxGroup( "Gate Props" ) ] public GameObject[] negative_objects;


        [ HorizontalLine ] public GameObject ballThrower;
			
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

				var rootObjects = activeScene.GetRootGameObjects();

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

        [ Button() ]
        public void PairWaypoints()
        {
			var scenes = EditorBuildSettings.scenes;

			for( var i = 1; i <= scenes.Length - 1 ; i++ )
            {
				var activeScene = EditorSceneManager.OpenScene( scenes[ i ].path );

				EditorSceneManager.MarkSceneDirty( activeScene );

				var waypointsParent = GameObject.FindGameObjectWithTag( "WaypointParent" ).transform;

                for( var a = 0; a < waypointsParent.childCount - 1; a++ )
                {
					var waypoint = waypointsParent.GetChild( a ).GetComponent< Waypoint >();
					var waypoint_Next = waypointsParent.GetChild( a + 1 ).GetComponent< Waypoint >();

					waypoint.Editor_SetNextWaypoint( waypoint_Next );

					PrefabUtility.RecordPrefabInstancePropertyModifications( waypoint );
				}

				EditorSceneManager.SaveScene( activeScene );
			}
        }

		[ Button() ]
		public void InsertGateProps()
		{
			var scenes = EditorBuildSettings.scenes;

			for( var i = 1; i <= scenes.Length - 1; i++ )
			{
				var activeScene = EditorSceneManager.OpenScene( scenes[ i ].path );

				EditorSceneManager.MarkSceneDirty( activeScene );

				var gates = GameObject.FindGameObjectsWithTag( "Gate" );

				for( var a = 0; a < gates.Length; a++ )
				{
					var gate_positive = gates[ a ].transform.GetChild( 0 );
					var gate_negative = gates[ a ].transform.GetChild( 1 );

					var random = Random.Range( 0, positive_names.Length );

					var positive_text = gate_positive.GetComponentInChildren< TextMeshProUGUI >();
					var negative_text = gate_negative.GetComponentInChildren< TextMeshProUGUI >();

					positive_text.text = positive_names[ random ];
					negative_text.text = negative_names[ random ];

					var positive_Prop = PrefabUtility.InstantiatePrefab( positive_objects[ random ] ) as GameObject;
					var negative_Prop = PrefabUtility.InstantiatePrefab( negative_objects[ random ] ) as GameObject;

					positive_Prop.transform.SetParent( gate_positive.transform );
					positive_Prop.transform.localPosition = Vector3.zero;

					negative_Prop.transform.SetParent( gate_negative.transform );
					negative_Prop.transform.localPosition = Vector3.zero;

					PrefabUtility.RecordPrefabInstancePropertyModifications( positive_text );
					PrefabUtility.RecordPrefabInstancePropertyModifications( negative_text );

					PrefabUtility.RecordPrefabInstancePropertyModifications( gate_positive );
					PrefabUtility.RecordPrefabInstancePropertyModifications( gate_negative );

					PrefabUtility.RecordPrefabInstancePropertyModifications( gates[ a ] );
				}

				EditorSceneManager.SaveScene( activeScene );
			}
		}

		[ Button() ]
		public void InsertCurrentGateProps()
		{
			EditorSceneManager.MarkAllScenesDirty();

			var gates = GameObject.FindGameObjectsWithTag( "Gate" );

			for( var a = 0; a < gates.Length; a++ )
			{
				var gate_positive = gates[ a ].transform.GetChild( 0 );
				var gate_negative = gates[ a ].transform.GetChild( 1 );

				var random = Random.Range( 0, positive_names.Length );

				var positive_text = gate_positive.GetComponentInChildren<TextMeshProUGUI>();
				var negative_text = gate_negative.GetComponentInChildren<TextMeshProUGUI>();

				positive_text.text = positive_names[ random ];
				negative_text.text = negative_names[ random ];

				var positive_Prop = PrefabUtility.InstantiatePrefab( positive_objects[ random ] ) as GameObject;
				var negative_Prop = PrefabUtility.InstantiatePrefab( negative_objects[ random ] ) as GameObject;

				positive_Prop.transform.SetParent( gate_positive.transform );
				positive_Prop.transform.localPosition = Vector3.zero;

				negative_Prop.transform.SetParent( gate_negative.transform );
				negative_Prop.transform.localPosition = Vector3.zero;

				PrefabUtility.RecordPrefabInstancePropertyModifications( positive_text );
				PrefabUtility.RecordPrefabInstancePropertyModifications( negative_text );

				PrefabUtility.RecordPrefabInstancePropertyModifications( gate_positive );
				PrefabUtility.RecordPrefabInstancePropertyModifications( gate_negative );

				PrefabUtility.RecordPrefabInstancePropertyModifications( gates[ a ] );
			}

			EditorSceneManager.SaveOpenScenes();
		}

		[ Button() ]
		public void InsertBallThrower()
		{
			var scenes = EditorBuildSettings.scenes;

			for( var i = 1; i <= scenes.Length - 1; i++ )
			{
				var activeScene = EditorSceneManager.OpenScene( scenes[ i ].path );

				EditorSceneManager.MarkSceneDirty( activeScene );

				var player = GameObject.Find( "player" ) as GameObject;
				player.transform.position = Vector3.forward * -8f;

				var ballThrower_Instance = PrefabUtility.InstantiatePrefab( ballThrower ) as GameObject;
				ballThrower_Instance.transform.position = Vector3.zero;
				ballThrower_Instance.transform.SetSiblingIndex( player.transform.GetSiblingIndex() + 1 );

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