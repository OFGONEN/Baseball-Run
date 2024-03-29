/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using TMPro;
using NaughtyAttributes;

public class UIStatus : UILoadingBar
{
#region Fields
    [ BoxGroup( "Shared Variables" ) ] public Status_Property statusProperty;
	[ BoxGroup( "UI Elements" ) ] public TextMeshProUGUI statusText;

	public EventListenerDelegateResponse ballCatchListener;
#endregion

#region Properties
#endregion

#region Unity API
    protected override void OnEnable()
	{
		base.OnEnable();

		statusProperty.changeEvent += OnStatusChange;
		ballCatchListener.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();

		statusProperty.changeEvent -= OnStatusChange;
		ballCatchListener.OnDisable();
	}

	protected override void Awake()
	{
		base.Awake();

		ballCatchListener.response = BallCatchResponse;

		OnStatusChange();
	}
#endregion

#region API
#endregion

#region Implementation
	private void OnStatusChange()
	{
		statusText.text    = statusProperty.status_Name;
		statusText.color   = statusProperty.status_Color;
		fillingImage.color = statusProperty.status_Color;
	}

	private void BallCatchResponse()
	{
		gameObject.SetActive( false );
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
