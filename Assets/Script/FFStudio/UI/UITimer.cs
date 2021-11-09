/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;

public class UITimer : UIText
{
#region Fields
    [ Header( "Setup" ) ]
    public EventListenerDelegateResponse levelStartListener;
	public MultipleEventListenerDelegateResponse levelFinishedListener;
	public EventListenerDelegateResponse modifyEventListener;
	public SharedFloatProperty ballHeightProperty;
	public UIFloatingTextStack uiFloatingTextStack;
	public RectTransform floatingTextSpawnPoint;

	// Private Fields \\
	private Tween scalePunchTween;
#endregion

#region Properties
#endregion

#region Unity API
    private void OnEnable()
    {
		levelStartListener.OnEnable();
		levelFinishedListener.OnEnable();
		modifyEventListener.OnEnable();
	}

    private void OnDisable()
    {
		levelStartListener.OnDisable();
		levelFinishedListener.OnDisable();
		modifyEventListener.OnDisable();

		ballHeightProperty.changeEvent -= OnBallHeightChange;
	}

    private void Awake()
    {
		levelStartListener.response    = LevelStartedResponse;
		levelFinishedListener.response = LevelFinishedResponse;
		modifyEventListener.response   = ModifyEventResponse;
		textRenderer.text              = "00.00";

		uiFloatingTextStack.InitPool( uiTransform.parent, false );
	}
#endregion

#region API
#endregion

#region Implementation
    private void LevelStartedResponse()
    {
		OnBallHeightChange();
		GoToTargetPosition().onComplete = OnGoTargetComplete;
	}

	private void LevelFinishedResponse()
	{
		ballHeightProperty.changeEvent -= OnBallHeightChange;
		GoToStartPosition();
	}

	private void ModifyEventResponse()
	{
		var modifyValue = ( modifyEventListener.gameEvent as FloatGameEvent ).eventValue;
		if( scalePunchTween != null )
		{
			scalePunchTween.Kill();
			uiTransform.localScale = Vector3.one;
		}

		var floatingText = uiFloatingTextStack.GiveEntity( uiTransform, false );

		if( modifyValue > 0 )
		{
			floatingText.textRenderer.color = Color.green;
			floatingText.textRenderer.text = "+" + modifyValue * GameSettings.Instance.ball_modify_cofactor_positive;
		}
		else 
		{
			floatingText.textRenderer.color = Color.red;
			floatingText.textRenderer.text = ( modifyValue * GameSettings.Instance.ball_modify_cofactor_negative ).ToString();
		}

		var targetCofactor = GameSettings.Instance.ui_Entity_FloatingMove_Cofactor;

		floatingText.uiTransform.position = floatingTextSpawnPoint.position.AddXY( Random.insideUnitCircle * GameSettings.Instance.ui_Entity_FloatingMove_Spawn_Cofactor );
		floatingText.targetCofactor = Random.Range( 0.8f * targetCofactor, 1.2f * targetCofactor );
		floatingText.gameObject.SetActive( true );
		floatingText.GoToTargetPosition();

		scalePunchTween = uiTransform.DOPunchScale( Vector3.one * GameSettings.Instance.ui_Entity_Scale_PunchSize, GameSettings.Instance.ui_Entity_Scale_PunchSize_Duration );
	}

    private void OnGoTargetComplete()
    {
		OnBallHeightChange();

		ballHeightProperty.changeEvent += OnBallHeightChange;
	}

    private void OnBallHeightChange()
    {
		var ballHeightMax = GameSettings.Instance.ball_height_max * 0.9f;
		var timer         = ballHeightProperty.sharedValue / GameSettings.Instance.ball_height_deplete_speed;
		var ratio         = ballHeightProperty.sharedValue / ballHeightMax;
		var color         = Color.Lerp( Color.red, Color.green, ratio );

		textRenderer.color = color;
		textRenderer.text  = timer.ToString( "00.00" );
	}

	private void OnPunchScaleDone()
	{
		scalePunchTween = null;
	}
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}
