/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;
using NaughtyAttributes;

public class Test_Ball : MonoBehaviour
{
#region Fields
    [ BoxGroup( "SharedVariables" ) ] public SharedVector3 shared_Strike_Point;
    [ BoxGroup( "SharedVariables" ) ] public SharedVector3 shared_Fly_Point;

    [ BoxGroup( "Fired Events" ) ] public FloatGameEvent strikeEvent;

    [ BoxGroup( "Setup" ) ] public Transform strikePoint;
    [ BoxGroup( "Setup" ) ] public Transform flyPoint;

#endregion

#region Properties
#endregion

#region Unity API
#endregion

#region API
    [ Button() ]
    public void StrikeTest()
    {
		shared_Strike_Point.sharedValue = strikePoint.position;
		shared_Fly_Point.sharedValue    = flyPoint.position;

		strikeEvent.eventValue = 1f;
		strikeEvent.Raise();
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}