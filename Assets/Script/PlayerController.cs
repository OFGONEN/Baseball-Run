/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor;

public class PlayerController : MonoBehaviour
{
#region Fields
    [ Header( "Event Listeners" ) ]
    public EventListenerDelegateResponse levelStartListener;
	public EventListenerDelegateResponse modifierEventListener;
	public EventListenerDelegateResponse catwalkEventListener;
	public EventListenerDelegateResponse playerStrikeListener;
	public EventListenerDelegateResponse ballCatchListener;

	[ Header( "Fired Events" ) ]
	public GameEvent ballCatchEvent;

	[ Header( "Shared Variables" ) ]
    public SharedFloatProperty inputDirectionProperty;
	public SharedReferenceProperty startWaypointReference;
	public SharedFloatProperty playerStatusRatioProperty;
	public Status_Property playerStatusProperty;

	[ BoxGroup( "Setup" ) ] public Transform modelTransform;
	[ BoxGroup( "Setup" ) ] public MeshRenderer model_baseball_bat;
    [ BoxGroup( "Setup" ) ] public AnimatorGroup animatorGroup;
    [ BoxGroup( "Setup" ) ] public ModelRenderer[] modelRenderers;
    [ BoxGroup( "Setup" ) ] public CameraController cameraController;
    [ BoxGroup( "Setup" ) ] public Status currentStatus;

	[ BoxGroup( "Setup" ) ] public ParticleSystem particleSystem_transformUp;
	[ BoxGroup( "Setup" ) ] public ParticleSystem particleSystem_transformDown;


    [ BoxGroup( "Target Points" ) ] public Transform target_point_strike;
    [ BoxGroup( "Target Points" ) ] public Transform target_point_fly;
    [ BoxGroup( "Target Points" ) ] public Transform target_point_spawn;
    [ BoxGroup( "Target Points" ) ] public Transform target_point_catch;
    [ BoxGroup( "Target Points" ) ] public SharedVector3 target_point_initial;
    [ BoxGroup( "Target Points" ) ] public SharedVector3 target_point_secondary;


	// Private Fields \\
	private Waypoint currentWaypoint;
	private float modelRotationAmount;
	private float vertical_speed;

	// Status releated
	private float statusPoint_Current;
	private float statusDepleteSpeed;
	private float statusPoint_Floor;
	private float statusPoint_Ceil;

	private bool transformAfterSequence = false;
	private bool catwalking = false;

	private Dictionary< string, ModelRenderer > modelRendererDictionary;

	// Delegates
	private UnityMessage updateMethod;
	private UnityMessage startApproachMethod;
	private Sequence obstacleInteractonSequence;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		levelStartListener.OnEnable();
		modifierEventListener.OnEnable();
		catwalkEventListener.OnEnable();
		playerStrikeListener.OnEnable();
		ballCatchListener.OnEnable();
	}

    private void OnDisable()
    {
		levelStartListener.OnDisable();
		modifierEventListener.OnDisable();
		catwalkEventListener.OnDisable();
		playerStrikeListener.OnDisable();
		ballCatchListener.OnDisable();
    }
    
    private void Awake()
    {
		// Set Delegates
		levelStartListener.response    = LevelStartResponse;
		modifierEventListener.response = ModifierEventResponse;
		updateMethod                   = ExtensionMethods.EmptyMethod;
		catwalkEventListener.response  = CatwalkEventResponse;
		playerStrikeListener.response  = PlayerStrikeResponse;
		ballCatchListener.response 	   = LevelComplete;

		vertical_speed = GameSettings.Instance.player_speed_vertical;

		modelRendererDictionary = new Dictionary< string, ModelRenderer >( modelRenderers.Length );

	}
	
	private void Start()
	{
        statusPoint_Current = CurrentLevelData.Instance.levelData.levelStartStatusPoint;
        playerStatusRatioProperty.SetValue( statusPoint_Current / GameSettings.Instance.status_maxPoint );
		playerStatusProperty.SetValue( currentStatus );

		// Cache renderers in a dictionary
		for( var i = 0; i < modelRenderers.Length; i++ )
		{
			modelRendererDictionary.Add( modelRenderers[ i ].rendererName, modelRenderers[ i ] );
			modelRenderers[ i ].ToggleRenderer( false );
		}

		// Toogle on the current status model renderer
		ToggleRenderer( currentStatus.status_Name, true );

target_point_initial.sharedValue   = target_point_strike.position;
		target_point_secondary.sharedValue = target_point_fly.position;
	}

    private void Update()
    {
		updateMethod();
	}
#endregion

#region API
    public void StartApproachWaypoint()
    {
		startApproachMethod = StartApproachWaypoint;

		if( currentWaypoint != null )
			updateMethod = ApproachWaypointMethod;
	}

    public void StartApproach_DepletingWaypoint()
    {
		startApproachMethod = StartApproach_DepletingWaypoint;

        if( currentWaypoint != null )
			updateMethod = Approach_DepletingWaypointMethod;
    }
#endregion

#region Implementation
	private void ToggleRenderer( string rendererName, bool value )
	{
		ModelRenderer renderer;

		modelRendererDictionary.TryGetValue( rendererName, out renderer );
		renderer.ToggleRenderer( value );
	}

	private void PlayerStrikeResponse()
	{
		animatorGroup.SetTrigger( "strike" );
	}

    private void LevelStartResponse()
    {
		model_baseball_bat.enabled = false;

		currentWaypoint = startWaypointReference.sharedValue as Waypoint;
		currentWaypoint.PlayerEntered( this );
		transform.forward = currentWaypoint.transform.forward;

        statusPoint_Floor = 0;
		statusPoint_Ceil = currentStatus.status_Point;
	}

    private void ModifierEventResponse()
    {
		var modifyAmount = ( modifierEventListener.gameEvent as FloatGameEvent ).eventValue;

		var transform = ModifyStatus( modifyAmount );

		if( statusPoint_Current < 0 )
			ballCatchEvent.Raise();
		else if ( transform ) 
		{
			if( modifyAmount > 0 )
				TransformUp();
			else
				TransformDown();
		}
	}

	private void CatwalkEventResponse()
	{
		catwalking = true;

		vertical_speed = GameSettings.Instance.player_speed_catwalking;

		animatorGroup.SetBool( "slide", true );
	}

    private void ApproachWaypointMethod()
    {
		var position = transform.position;

		var approachDistance = currentWaypoint.ApproachMethod( transform, vertical_speed );
        var playerIsPassed = transform.InverseTransformPoint( currentWaypoint.TargetPoint ).z < 0;

        if( Vector3.Distance( approachDistance, currentWaypoint.TargetPoint ) <= GameSettings.Instance.player_target_checkDistance || playerIsPassed )
        {
			currentWaypoint.PlayerExited( this );

            if( currentWaypoint.NextWaypoint != null )
            {

				currentWaypoint = currentWaypoint.NextWaypoint;
				currentWaypoint.PlayerEntered( this );
				transform.forward = currentWaypoint.transform.forward;
			}
            else
            {
				updateMethod = ExtensionMethods.EmptyMethod;

				if( catwalking )
					ballCatchEvent.Raise();

				return;
			}
		}
        else
			transform.position = approachDistance;

		// Move GFX Object

		var clampedInput = Mathf.Clamp( inputDirectionProperty.sharedValue, -1, 1 );
		var clampedSpeed = Mathf.Clamp( Mathf.Abs( inputDirectionProperty.sharedValue ), 0, GameSettings.Instance.input_horizontal_clamp );
		Vector3 horizontalMove = Vector3.right * clampedInput;

		modelRotationAmount = Mathf.Lerp( modelRotationAmount, 
                                clampedInput * GameSettings.Instance.player_clamp_rotation, 
                                Time.deltaTime * GameSettings.Instance.player_speed_turning );

        // Calculate new local position for model
		var modelPosition = modelTransform.localPosition;
		var model_NewPosition = Vector3.MoveTowards( modelPosition, modelPosition + horizontalMove, Time.deltaTime * clampedSpeed * GameSettings.Instance.player_speed_horizontal );

		model_NewPosition.x = Mathf.Clamp( model_NewPosition.x, -currentWaypoint.Wide / 2f, currentWaypoint.Wide / 2f );
		modelTransform.localPosition = model_NewPosition;

		// Calculate new local rotation for model
		modelTransform.localRotation = Quaternion.Euler( 0, modelRotationAmount, 0 );
	}

    private void Approach_DepletingWaypointMethod()
    {
		var lossStatus = Time.deltaTime * GameSettings.Instance.player_speed_statusDepleting;

		var transform = ModifyStatus( -lossStatus );

		if( statusPoint_Current < 0 )
		{
			ballCatchEvent.Raise();
		}
		else if ( !catwalking && transform ) 
		{
			TransformDown();
		}

		ApproachWaypointMethod();
	}

	private bool ModifyStatus( float modifyAmount )
	{
		bool transform = false;
		var newStatusPoint = statusPoint_Current + modifyAmount;

		if( newStatusPoint < statusPoint_Floor && currentStatus.prevStatus != null )
		{
			statusPoint_Ceil  = statusPoint_Floor;
			statusPoint_Floor = statusPoint_Floor - currentStatus.prevStatus.status_Point;
			currentStatus     = currentStatus.prevStatus;

			transform = true;
		}
		else if( newStatusPoint >= statusPoint_Ceil && currentStatus.nextStatus != null )
		{
			statusPoint_Floor = statusPoint_Ceil;
			statusPoint_Ceil  = statusPoint_Ceil + currentStatus.nextStatus.status_Point;
			currentStatus     = currentStatus.nextStatus;

			transform = true;
		}
		else if( newStatusPoint >= statusPoint_Ceil && currentStatus.nextStatus == null )
		{
			newStatusPoint = GameSettings.Instance.status_maxPoint;
		}

		statusPoint_Current = newStatusPoint;
		playerStatusRatioProperty.SetValue( statusPoint_Current / GameSettings.Instance.status_maxPoint );

		return transform;
	}

	private void TransformUp()
	{
		ToggleRenderer( currentStatus.prevStatus.status_Name, false ); // Previous model
		ToggleRenderer( currentStatus.status_Name, true ); // Current model

		//TODO:(ofg) spawn a transform particle

		playerStatusProperty.SetValue( currentStatus );

		//TODO:(ofg) We can player different animation when transforming UP
		// animatorGroup.SetBool( "walking", false );
		// animatorGroup.SetBool( "transform_positive", true);
		// animatorGroup.SetTrigger( "transform" );

		particleSystem_transformUp.Play();
	}

	private void TransformDown()
	{
		ToggleRenderer( currentStatus.nextStatus.status_Name, false ); // Previous model
		ToggleRenderer( currentStatus.status_Name, true ); // Current model

		//TODO:(ofg) spawn a transform particle

		playerStatusProperty.SetValue( currentStatus );

        //TODO:(ofg) We can player different animation when transforming DOWN
        // animatorGroup.SetBool( "walking", false );
		// animatorGroup.SetBool( "transform_positive", false);
		// animatorGroup.SetTrigger( "transform" );

		particleSystem_transformDown.Play();
	}

	private void LevelComplete()
	{
		updateMethod = ExtensionMethods.EmptyMethod;

		target_point_initial.sharedValue   = target_point_spawn.position;
		target_point_secondary.sharedValue = target_point_catch.position;

		animatorGroup.SetBool( "slide", false );
		animatorGroup.SetTrigger( "catch" );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR

	[ Button]
	public void StartPlayer()
	{
		LevelStartResponse();
	}
#endif
#endregion

	[ System.Serializable ]
	public class AnimatorGroup
	{
		public Animator[] animators;

		public void SetTrigger( string parameterName )
		{
			foreach( var animator in animators )
			{
				animator.SetTrigger( parameterName );
			}
		}
		public void SetBool( string parameterName, bool value )
		{
			foreach( var animator in animators )
			{
				animator.SetBool( parameterName, value );
			}
		}

		public void SetInteger( string parameterName, int value )
		{
			foreach( var animator in animators )
			{
				animator.SetInteger( parameterName, value );
			}
		}

		public void SetFloat( string parameterName, float value )
		{
			foreach( var animator in animators )
			{
				animator.SetFloat( parameterName, value );
			}
		}

		public void SetWeightOfLayer( int layerIndex, float weight )
		{
			foreach( var animator in animators )
			{
				animator.SetLayerWeight( layerIndex, weight );
			}		
		}
	}
}
