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
    public EventListenerDelegateResponse ballStrikeEvent;
    public EventListenerDelegateResponse ballCatchEvent;


    [ BoxGroup( "Setup" ) ] public Transform ball;
    [ BoxGroup( "Setup" ) ] public Transform ballSpawnPoint;


    [ BoxGroup( "Shared Variables" ) ]
    public SharedVector3 ball_initial_TargetPoint;
    [ BoxGroup( "Shared Variables" ) ]
    public SharedVector3 ball_secondary_TargetPoint;


    private GameSettings Settings => GameSettings.Instance;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		ballStrikeEvent.OnEnable();
		ballCatchEvent.OnEnable();
	}

    private void OnDisable()
    {
		ballStrikeEvent.OnDisable();
		ballCatchEvent.OnDisable();
    }

    private void Awake()
    {
		ballStrikeEvent.response = BallStrikeResponse;
		ballCatchEvent.response  = BallCatchResponse;
	}
#endregion

#region API
#endregion

#region Implementation
    private void BallStrikeResponse()
    {
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
	}

	private void BallCatchResponse()
	{
		ball.position = ball_initial_TargetPoint.sharedValue;
		ball.gameObject.SetActive( true );

		var tween_catch = ball.DOMove( ball_secondary_TargetPoint.sharedValue, Settings.ball_duration_catch_point );
		tween_catch.SetEase( Settings.ball_curve_catch_point );
		tween_catch.OnComplete( OnBallCatchComplete );
	}

	private void OnBallStrikeComplete()
	{
		ballStrikeEvent.response = ExtensionMethods.EmptyMethod;

		ball.gameObject.SetActive( false );
	}

	private void OnBallCatchComplete()
	{
		ballCatchEvent.response = ExtensionMethods.EmptyMethod;
		ball.gameObject.SetActive( false );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}