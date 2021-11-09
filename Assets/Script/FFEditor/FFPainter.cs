/* Created by and for usage of FF Studios (2021). */

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using FFStudio;
using NaughtyAttributes;

[ ExecuteInEditMode ]
public class FFPainter : MonoBehaviour
{
#region Fields
    public GameObject prefab;
	public Transform parentToSpawn;
	public float heightToSpawn;
	public float distanceToSpawn;

	private Vector3 lastSpawnedPosition;
	private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
	}
#endregion

#region API
    [ Button() ]
    public void StartSpawing()
    {
		lastSpawnedPosition = Vector3.zero;
		updateMethod        = OnUpdate_Spawn;
	}

    [ Button() ]
    public void StopSpawning()
    {
		updateMethod = ExtensionMethods.EmptyMethod;
	}

    [ Button() ]
    public void DeleteSpawned()
    {
		for( var i = parentToSpawn.childCount - 1; i >= 0 ; i-- )
		{
			DestroyImmediate( parentToSpawn.GetChild( i ) );
		}
	}

    private void Update()
    {
		updateMethod();
	}

    public void Spawn()
    {
		var gameObject                    = PrefabUtility.InstantiatePrefab( prefab ) as GameObject;
		    gameObject.transform.position = transform.position.SetY( heightToSpawn );

		gameObject.transform.SetParent( parentToSpawn );

		lastSpawnedPosition = transform.position;
	}
#endregion

#region Implementation
    private void OnUpdate_Spawn()
    {
        if( Vector3.Distance( transform.position, lastSpawnedPosition ) >= distanceToSpawn )
			Spawn();
    }
#endregion

#region Editor Only
#endregion
}
#endif