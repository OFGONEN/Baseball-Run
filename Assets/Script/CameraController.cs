/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor;

public class CameraController : MonoBehaviour
{
#region Fields
    [ Header( "Event Listeners" ) ] 
    public EventListenerDelegateResponse levelStartedListener;
    public MultipleEventListenerDelegateResponse levelCompleteListener;
    public EventListenerDelegateResponse catwalkEventListener;
    public EventListenerDelegateResponse ballCatchEventListener;

    [ HorizontalLine ]
    [ BoxGroup( "Setup" ), SerializeField ] private Transform target;
    [ BoxGroup( "Setup" ), SerializeField ] private Vector3 targetPosition;
    [ BoxGroup( "Setup" ), SerializeField ] private Vector3 targetRotation;

	// Private Fields \\
	private Camera mainCamera;

	private UnityMessage updateMethod;
    private Sequence levelStartSequence;
	private Sequence moveAndLookSequence;

	private float totalRotateAmount = 0f;
	private float rotateSign = 1f;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		levelStartedListener.OnEnable();
		levelCompleteListener.OnEnable();
		catwalkEventListener.OnEnable();
		ballCatchEventListener.OnEnable();
	}
    
    private void OnDisable()
    {
 		levelStartedListener.OnDisable();
		levelCompleteListener.OnDisable();
		catwalkEventListener.OnDisable();
		ballCatchEventListener.OnDisable();

		if( moveAndLookSequence != null )
		{
			moveAndLookSequence.Kill();
			moveAndLookSequence = null;
		}
    }

    private void Awake()
    {
		updateMethod                    = ExtensionMethods.EmptyMethod;
		levelStartedListener.response   = LevelStartedResponse;
		levelCompleteListener.response  = LevelCompleteResponse;
		catwalkEventListener.response   = CatwalkEventResponse;
		ballCatchEventListener.response = BallCatchEventResponse;
		mainCamera                      = GetComponent< Camera >();
	}

    private void Update()
    {
		updateMethod();
	}
#endregion

#region API
	public void MoveAndLook( Vector3 movePosition, Vector3 lookRotation )
	{
		updateMethod = ExtensionMethods.EmptyMethod;

		var duration = GameSettings.Instance.camera_duration_moveAndLook;

		moveAndLookSequence = DOTween.Sequence();
		moveAndLookSequence.Append( transform.DOLocalMove( movePosition, duration ) );
		moveAndLookSequence.Join( transform.DOLocalRotate( lookRotation, duration ) );
		moveAndLookSequence.OnComplete( OnMoveAndLookSequenceComplete );
	}

	public void ReturnDefault()
	{
		var duration = GameSettings.Instance.camera_duration_moveAndLook;

		levelStartSequence = DOTween.Sequence();
		levelStartSequence.Append( transform.DOLocalMove( targetPosition, duration ) );
		levelStartSequence.Join( transform.DOLocalRotate( targetRotation, duration ) );
		levelStartSequence.OnComplete( OnReturnDefaultComplete );
	}
#endregion

#region Implementation
    private void FollowTargetMethod()
    {
		var localPosition        = transform.localPosition;
		var target_localPosition = target.localPosition;
		var nextPosition         = Mathf.Lerp( localPosition.x, target_localPosition.x, Time.deltaTime * GameSettings.Instance.camera_speed_follow );

		localPosition.x         = nextPosition;
		transform.localPosition = localPosition;
	}

    private void RotateAroundTargetMethod()
    {
        var rotateAmount       = Time.deltaTime * rotateSign * GameSettings.Instance.camera_speed_LevelEndRotation;
            totalRotateAmount += rotateAmount;

		transform.RotateAround( target.position, Vector3.up, rotateAmount );

		if ( Mathf.Abs( totalRotateAmount ) >= GameSettings.Instance.camera_clamp_LevelEndRotation )
		    rotateSign *= -1f;
	}


	private void LevelStartedResponse()
    {
		levelStartSequence = DOTween.Sequence();
		levelStartSequence.Append( transform.DOLocalMove( targetPosition, GameSettings.Instance.camera_duration_movement ) );
		levelStartSequence.Join( transform.DOLocalRotate( targetRotation, GameSettings.Instance.camera_duration_movement ) );
		levelStartSequence.OnComplete( OnLevelStartSequenceComplete );
	}

    private void LevelCompleteResponse()
    {
		    updateMethod    = ExtensionMethods.EmptyMethod;
		var localPosition   = transform.localPosition;
		    localPosition.x = target.localPosition.x;

		transform.DOLocalMove( localPosition, GameSettings.Instance.camera_duration_movement ).OnComplete( () => updateMethod = RotateAroundTargetMethod );
	}

    private void OnLevelStartSequenceComplete()
    {
		levelStartSequence.Kill();
		levelStartSequence = null;

		updateMethod = FollowTargetMethod;
	}

	private void OnReturnDefaultComplete()
	{
		levelStartSequence.Kill();
		levelStartSequence = null;

		updateMethod = FollowTargetMethod;
	}

	private void OnMoveAndLookSequenceComplete()
	{
		moveAndLookSequence.Kill();
		moveAndLookSequence = null;
	}

	private void CatwalkEventResponse()
	{
		mainCamera.DOFieldOfView( GameSettings.Instance.camera_fieldOfView_catwalk, GameSettings.Instance.camera_fieldOfView_duration );
	}

	private void BallCatchEventResponse()
	{
		mainCamera.DOFieldOfView( GameSettings.Instance.camera_fieldOfView_normal, GameSettings.Instance.camera_fieldOfView_duration );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
		var final_cameraPosition = transform.position + ( targetPosition - transform.localPosition ) ;

		Handles.ArrowHandleCap( 0, final_cameraPosition, Quaternion.Euler( targetRotation ), 1f, EventType.Repaint );
		Handles.Label( final_cameraPosition.AddUp( 0.5f ), "Final Camera Position\n" + final_cameraPosition );
	}
#endif
#endregion
}
