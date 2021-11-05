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
	[ BoxGroup("Event Listeners" ) ] public EventListenerDelegateResponse levelStartEventListener;
    [ BoxGroup("Event Listeners" ) ] public EventListenerDelegateResponse ballStrikeEventListener;
    [ BoxGroup("Event Listeners" ) ] public EventListenerDelegateResponse ballCatchEventListener;
    [ BoxGroup("Event Listeners" ) ] public EventListenerDelegateResponse modifyEventListener;

    [ BoxGroup( "Fired Events" ) ] public GameEvent ballCatchEvent;

    [ BoxGroup( "Setup" ) ] public Transform ball;
    [ BoxGroup( "Setup" ) ] public Transform ballSpawnPoint;
    [ BoxGroup( "Setup" ) ] public ParticleSystem strikeParticle;
    [ BoxGroup( "Setup" ) ] public ParticleSystem catchParticle;

	[ BoxGroup( "Shared Variables" ) ] public SharedFloatProperty ballHeightProperty;
	[ BoxGroup( "Shared Variables" ) ] public SharedFloatProperty ballHeight_RatioProperty;
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
		modifyEventListener.OnEnable();
	}

    private void OnDisable()
    {
		levelStartEventListener.OnDisable();
		ballStrikeEventListener.OnDisable();
		ballCatchEventListener.OnDisable();
		modifyEventListener.OnDisable();
    }

    private void Awake()
    {
		levelStartEventListener.response = LevelStartEvent;
		ballStrikeEventListener.response = BallStrikeResponse;
		ballCatchEventListener.response  = BallCatchResponse;
		modifyEventListener.response     = ModifyEventResponse;

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
		ballHeight_RatioProperty.SetValue( height / Settings.ball_height_max );
	}

    private void BallStrikeResponse()
    {
		FFLogger.Log( "Ball Strike" );

		var strikeEvent = ballStrikeEventListener.gameEvent as FloatGameEvent;
		var height = strikeEvent.eventValue * Settings.ball_height_cofactor_strike;
		ballHeightProperty.SetValue( height );
		ballHeight_RatioProperty.SetValue( height / Settings.ball_height_max );

		// ball.gameObject.SetActive( true );
		ball.position = ballSpawnPoint.position;
		ball.LookAt( ball_initial_TargetPoint.sharedValue );


		var sequence = DOTween.Sequence();
		
        var tween_strike = ball.DOMove( ball_initial_TargetPoint.sharedValue, Settings.ball_duration_strike_point );
		tween_strike.SetEase( Settings.ball_curve_strike_point );

		var tween_fly = ball.DOMove( ball_secondary_TargetPoint.sharedValue, Settings.ball_duration_fly_point );
		tween_fly.SetEase( Settings.ball_curve_fly_point );

		sequence.AppendInterval( Settings.ball_delay_strike_point );
		sequence.AppendCallback( () => ball.gameObject.SetActive( true ) );
		sequence.Append( tween_strike );
		sequence.AppendCallback( OnBallStrike );
		sequence.Append( tween_fly );
		sequence.OnComplete( OnBallStrikeComplete );
	}

	private void BallCatchResponse()
	{
		// Level is ended there is no need to handle ball height anymore
		updateMethod = ExtensionMethods.EmptyMethod;

		var sequence = DOTween.Sequence();

		sequence.AppendInterval( Settings.ball_delay_catch_point );
		sequence.AppendCallback( () =>
		{
			ball.position = ball_initial_TargetPoint.sharedValue;
			ball.LookAt( ball_secondary_TargetPoint.sharedValue );
			ball.gameObject.SetActive( true );
		} );

		sequence.Append( ball.DOMove( ball_secondary_TargetPoint.sharedValue, Settings.ball_duration_catch_point ).SetEase( Settings.ball_curve_catch_point ) );
		sequence.OnComplete( OnBallCatchComplete );
	}

	private void ModifyEventResponse()
	{
		var modifyEvent = modifyEventListener.gameEvent as FloatGameEvent;

		float modify = 0;

		if( Mathf.Sign( modifyEvent.eventValue ) > 0 )
			modify = modifyEvent.eventValue * GameSettings.Instance.ball_modify_cofactor_positive;
		else 
			modify = modifyEvent.eventValue * GameSettings.Instance.ball_modify_cofactor_negative;

		var height = Mathf.Clamp( ballHeightProperty.sharedValue + modify, 0, GameSettings.Instance.ball_height_max);
		ballHeightProperty.SetValue( height );
		ballHeight_RatioProperty.SetValue( height / Settings.ball_height_max );
	}

	private void OnBallStrikeComplete()
	{
		ballStrikeEventListener.response = ExtensionMethods.EmptyMethod;

		ball.gameObject.SetActive( false );
	}

	private void OnBallCatchComplete()
	{
		catchParticle.transform.position = ball_secondary_TargetPoint.sharedValue;
		catchParticle.Play();

		ballCatchEventListener.response = ExtensionMethods.EmptyMethod;
		ball.gameObject.SetActive( false );
	}

	private void OnBallStrike()
	{
		strikeParticle.transform.position = ball_initial_TargetPoint.sharedValue;
		strikeParticle.Play();

		ball.LookAt( ball_secondary_TargetPoint.sharedValue );
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