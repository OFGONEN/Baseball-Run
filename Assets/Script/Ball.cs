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


    [ BoxGroup( "Setup" ) ] public Transform ball;
    [ BoxGroup( "Setup" ) ] public Transform ballSpawnPoint;


    [ BoxGroup( "Shared Variables" ) ]
    public SharedVector3 ball_Strike_TargetPoint;
    [ BoxGroup( "Shared Variables" ) ]
    public SharedVector3 ball_Fly_TargetPoint;


    private GameSettings Settings => GameSettings.Instance;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		ballStrikeEvent.OnEnable();
	}

    private void OnDisable()
    {
		ballStrikeEvent.OnDisable();
    }

    private void Awake()
    {
		ballStrikeEvent.response = BallStrikeResponse;
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
		
        var tween_strike = ball.DOMove( ball_Strike_TargetPoint.sharedValue, Settings.ball_duration_strike_point );
		tween_strike.SetEase( Settings.ball_curve_strike_point );

		var tween_fly = ball.DOMove( ball_Fly_TargetPoint.sharedValue, Settings.ball_duration_fly_point );
		tween_fly.SetEase( Settings.ball_curve_fly_point );

		sequence.Append( tween_strike );
		sequence.Append( tween_fly );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}