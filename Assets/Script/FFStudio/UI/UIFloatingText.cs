/* Created by and for usage of FF Studios (2021). */

using UnityEngine;
using FFStudio;
using DG.Tweening;

public class UIFloatingText : UIText
{
#region Fields
	public UIFloatingTextStack floatingTextStack;
	
	[ HideInInspector ] public Color textColor;
	[ HideInInspector ] public float targetCofactor;
#endregion

#region Unity API
#endregion

#region API
	public override Tween GoToTargetPosition()
	{
		textRenderer.DOFade( 0, GameSettings.Instance.ui_Entity_FloatingMove_TweenDuration )
					.SetEase( Ease.InExpo );
		return uiTransform.DOMove( uiTransform.position + Vector3.up * targetCofactor, GameSettings.Instance.ui_Entity_FloatingMove_TweenDuration ).OnComplete( OnGoTargetComplete );
	}
#endregion


#region Implementation
	private void OnGoTargetComplete()
	{
		floatingTextStack.Stack.Push( this );
	}
#endregion
}
