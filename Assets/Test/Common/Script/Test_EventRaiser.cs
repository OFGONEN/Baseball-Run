/* Created by and for usage of FF Studios (2021). */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FFStudio;

public class Test_EventRaiser : MonoBehaviour
{
#region Fields
    public GameEvent[] gameEvents;

    private Dictionary< string, GameEvent > gameEventDictionary;
#endregion

#region Properties
#endregion

#region Unity API
    private void Awake()
    {
		gameEventDictionary = new Dictionary< string, GameEvent >( gameEvents.Length );

        foreach( var gameEvent in gameEvents )
        {
			gameEventDictionary.Add( gameEvent.name, gameEvent );
		}
	}
#endregion

#region API
    public void Raise( string eventName )
    {
		GameEvent gameEvent;

		gameEventDictionary.TryGetValue( eventName, out gameEvent );

		gameEvent?.Raise();
	}
#endregion

#region Implementation
#endregion

#region Editor Only
#if UNITY_EDITOR
#endif
#endregion
}