/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

public class Ball : MonoBehaviour
{
#region Fields
    [ BoxGroup("Event Listeners" ) ]
	public EventListenerDelegateResponse levelStartEventListener;
    [ BoxGroup("Event Listeners" ) ]
    public EventListenerDelegateResponse ballStrikeEventListener;
    [ BoxGroup("Event Listeners" ) ]
    public EventListenerDelegateResponse ballCatchEventListener;

    [ BoxGroup( "Fired Events" ) ] public GameEvent ballCatchEvent;
    [ BoxGroup( "Fired Events" ) ] public ParticleSpawnEvent particleSpawnEvent;

    [ BoxGroup( "Setup" ) ] public Transform ball;
    [ BoxGroup( "Setup" ) ] public Transform ballSpawnPoint;

	[ BoxGroup( "Shared Variables" ) ] public SharedFloatProperty ballHeightProperty;
    [ BoxGroup( "Shared Variables" ) ] public SharedVector3 ball_initial_TargetPoint;
    [ BoxGroup( "Shared Variables" ) ] public SharedVector3 ball_secondary_TargetPoint;

    private GameSettings Settings => GameSettings.Instance;

	private UnityMessage updateMethod;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		levelStartEventListener.OnEnable();
		ballStrikeEventListener.OnEnable();
		ballCatchEventListener.OnEnable();
	}

    private void OnDisable()
    {
		levelStartEventListener.OnDisable();
		ballStrikeEventListener.OnDisable();
		ballCatchEventListener.OnDisable();
    }

    private void Awake()
    {
		levelStartEventListener.response = LevelStartEvent;
		ballStrikeEventListener.response = BallStrikeResponse;
		ballCatchEventListener.response  = BallCatchResponse;

		updateMethod = ExtensionMethods.EmptyMethod;
	}

	private void Update()
	{
		updateMethod();
	}
#endregion

#region API
#endregion

#region Implementation
	private void BallDepleteMethod()
	{
		var height = ballHeightProperty.sharedValue + -1f * Settings.ball_height_deplete_speed * Time.deltaTime;

		if( height <= 0 )
		{
			height = 0;
			ballCatchEvent.Raise();
		}

		ballHeightProperty.SetValue( height );
	}

    private void BallStrikeResponse()
    {
		FFLogger.Log( "Ball Strike" );

		var strikeEvent = ballStrikeEventListener.gameEvent as FloatGameEvent;
		ballHeightProperty.SetValue( strikeEvent.eventValue * Settings.ball_height_cofactor_strike );

		ball.gameObject.SetActive( true );
		ball.position = ballSpawnPoint.position;

		var sequence = DOTween.Sequence();
		
        var tween_strike = ball.DOMove( ball_initial_TargetPoint.sharedValue, Settings.ball_duration_strike_point );
		tween_strike.SetEase( Settings.ball_curve_strike_point );

		var tween_fly = ball.DOMove( ball_secondary_TargetPoint.sharedValue, Settings.ball_duration_fly_point );
		tween_fly.SetEase( Settings.ball_curve_fly_point );

		sequence.Append( tween_strike );
		sequence.Append( tween_fly );
		sequence.OnComplete( OnBallStrikeComplete );

		// Ball Catch particle effect
		particleSpawnEvent.Raise( "ball_catch", ball_secondary_TargetPoint.sharedValue );
	}

	private void BallCatchResponse()
	{
		// Level is ended there is no need to handle ball height anymore
		updateMethod = ExtensionMethods.EmptyMethod;

		ball.position = ball_initial_TargetPoint.sharedValue;
		ball.gameObject.SetActive( true );

		var tween_catch = ball.DOMove( ball_secondary_TargetPoint.sharedValue, Settings.ball_duration_catch_point );
		tween_catch.SetEase( Settings.ball_curve_catch_point );
		tween_catch.OnComplete( OnBallCatchComplete );
	}

	private void OnBallStrikeComplete()
	{
		ballStrikeEventListener.response = ExtensionMethods.EmptyMethod;

		ball.gameObject.SetActive( false );
	}

	private void OnBallCatchComplete()
	{
		ballCatchEventListener.response = ExtensionMethods.EmptyMethod;
		ball.gameObject.SetActive( false );
	}

	private void LevelStartEvent()
	{
		updateMethod = BallDepleteMethod;
		// Modify Event response set
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}