/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;

public class UIStrikeBar : UIEntity
{
#region Fields
    [ Header( "EventListener" ) ]
    public EventListenerDelegateResponse levelRevealedListener;
    public EventListenerDelegateResponse tapInputListener;

    [ Header( "Fired Events" ) ]
    public FloatGameEvent strikeEvent;

    [ Header( "SharedVariables" ) ]
    public SharedFloatProperty indicatorValue;

    // Private Fields \\
    private Sequence indicatorSequence; 
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		levelRevealedListener.OnEnable();
		tapInputListener.OnEnable();
	}

    private void OnDisable()
    {
 		levelRevealedListener.OnDisable();
		tapInputListener.OnDisable();
    }

    private void Awake()
    {
		levelRevealedListener.response = LevelRevealedResponse;
		tapInputListener.response      = ExtensionMethods.EmptyMethod;
	}
#endregion

#region API
#endregion

#region Implementation
    private void LevelRevealedResponse()
    {
		indicatorValue.SetValue( 0 );

		GoToTargetPosition().OnComplete( OnGoTargetMoveDone );
	}

    private void TapInputResponse()
    {
		tapInputListener.response = ExtensionMethods.EmptyMethod;

		indicatorSequence.Kill();
		indicatorSequence = null;

		GoToStartPosition().SetDelay( 0.25f ).OnComplete( OnGoStartMoveDone );
	}

    private void OnGoTargetMoveDone()
    {
		indicatorSequence = DOTween.Sequence();

		indicatorSequence.Append( DOTween.To( () => indicatorValue.sharedValue, x => indicatorValue.SetValue( x ), 1f, GameSettings.Instance.ui_world_indicator_speed ) );
		indicatorSequence.Append( DOTween.To( () => indicatorValue.sharedValue, x => indicatorValue.SetValue( x ), 0, GameSettings.Instance.ui_world_indicator_speed ) );

		indicatorSequence.SetLoops( -1, LoopType.Restart );

		tapInputListener.response = TapInputResponse;
	}

	private void OnGoStartMoveDone()
	{
		if( indicatorValue.sharedValue <= 0.125f || indicatorValue.sharedValue >= 0.875f )
			strikeEvent.eventValue = GameSettings.Instance.ui_world_indicator_red;
		else if( indicatorValue.sharedValue >= 0.25f || indicatorValue.sharedValue <= 0.75f )
			strikeEvent.eventValue = GameSettings.Instance.ui_world_indicator_green;
		else
			strikeEvent.eventValue = GameSettings.Instance.ui_world_indicator_yellow;

		strikeEvent.Raise();
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}