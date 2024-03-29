/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Modifier : MonoBehaviour
{
#region Fields
    [ Header( "Fired Events" ) ]
    public ParticleSpawnEvent particleSpawnEvent;
	public FloatGameEvent modifier_Event;

	// Private \\
	[ BoxGroup( "Setup" ) ] public float modifier_Point;
    [ BoxGroup( "Setup" ) ] public string modifier_ParticleName;

    // Components 
    protected ColliderListener_EventRaiser colliderListener;
    protected Collider modiferCollider;

#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		colliderListener.triggerEnter += TriggerEnter;
	}

    private void OnDisable()
    {
		colliderListener.triggerEnter -= TriggerEnter;
	}

    private void Awake()
    {
		colliderListener = GetComponentInChildren< ColliderListener_EventRaiser >();
		modiferCollider = GetComponentInChildren< Collider >();
	}


#endregion

#region API
    public void DisableCollider()
    {
		modiferCollider.enabled = false;
	}
#endregion

#region Implementation
    protected virtual void TriggerEnter( Collider other )
    {
		particleSpawnEvent.Raise( modifier_ParticleName, transform.position );

        modifier_Event.eventValue = modifier_Point;
		modifier_Event.Raise();
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
