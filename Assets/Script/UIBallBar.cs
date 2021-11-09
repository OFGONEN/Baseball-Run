/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class UIBallBar : UILoadingBar
{
#region Fields
    [ Header( "Event Listeners" ) ]
    public EventListenerDelegateResponse levelStartedListener;
    public EventListenerDelegateResponse ballCatchListener;
#endregion

#region Properties
#endregion

#region Unity API
    protected override void OnEnable()
    {
		base.OnEnable();

		levelStartedListener.OnEnable();
		ballCatchListener.OnEnable();
	}

    protected override void OnDisable()
    {
		base.OnDisable();

		levelStartedListener.OnDisable();
		ballCatchListener.OnDisable();
	}

    protected override void Awake()
    {
		base.Awake();

		levelStartedListener.response  = LevelStartedResponse;
		ballCatchListener.response     = BallCatchResponse;
	}

#endregion

#region API
#endregion

#region Implementation
	protected override void OnValueChange()
    {
		var ratio = progressProperty.sharedValue / GameSettings.Instance.ball_height_max;

		fillingImage.fillAmount = ratio;
	}

    private void LevelStartedResponse()
    {
		GoToTargetPosition();
	}

    private void BallCatchResponse() 
    {
		GoToStartPosition();
    }
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}