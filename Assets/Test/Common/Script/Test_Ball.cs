/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using DG.Tweening;
using NaughtyAttributes;

public class Test_Ball : MonoBehaviour
{
#region Fields
    [ BoxGroup( "SharedVariables" ) ] public SharedVector3 shared_Initial_Point;
    [ BoxGroup( "SharedVariables" ) ] public SharedVector3 shared_Secondary_Point;

    [ BoxGroup( "Fired Events" ) ] public IntGameEvent tapInput;
    [ BoxGroup( "Fired Events" ) ] public GameEvent levelRevealed;
    [ BoxGroup( "Fired Events" ) ] public GameEvent levelStarted;
    [ BoxGroup( "Fired Events" ) ] public GameEvent catchEvent;
    [ BoxGroup( "Fired Events" ) ] public FloatGameEvent strikeEvent;

    [ BoxGroup( "Setup" ) ] public Transform strikePoint;
    [ BoxGroup( "Setup" ) ] public Transform flyPoint;
    [ BoxGroup( "Setup" ) ] public Transform catch_Spawn_Point;
    [ BoxGroup( "Setup" ) ] public Transform catch_Target_Point;

#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void StrikeTest()
    {
		shared_Initial_Point.sharedValue   = strikePoint.position;
		shared_Secondary_Point.sharedValue = flyPoint.position;

		// strikeEvent.eventValue = 1f;
		strikeEvent.Raise();
	}

    [ Button() ]
    public void CatchTest()
    {
		shared_Initial_Point.sharedValue   = catch_Spawn_Point.position;
		shared_Secondary_Point.sharedValue = catch_Target_Point.position;

		catchEvent.Raise();
	}

	[ Button() ]
	public void LevelStartTest()
	{
		var sequence = DOTween.Sequence();

		shared_Initial_Point.sharedValue   = strikePoint.position;
		shared_Secondary_Point.sharedValue = flyPoint.position;

		sequence.AppendCallback( levelRevealed.Raise );
		sequence.AppendInterval( 1.5f );
		sequence.AppendCallback( tapInput.Raise );
		sequence.AppendInterval( 2f );
		sequence.AppendCallback( levelStarted.Raise );
		sequence.OnComplete( () =>
		{
			shared_Initial_Point.sharedValue   = catch_Spawn_Point.position;
			shared_Secondary_Point.sharedValue = catch_Target_Point.position;
		} );
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}