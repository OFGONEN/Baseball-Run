/* Created by and for usage of FF Studios (2021). */

using System.Collections.Generic;
using UnityEngine;
using FFStudio;

[ CreateAssetMenu( fileName = "UIFloatingTextStack", menuName = "FF/Data/Sets/UIFloatingTextStack" ) ]
public class UIFloatingTextStack : RunTimeStack< UIFloatingText >
{

    public UIFloatingText poolEntity;

#region API
     	public void InitPool( Transform parent, bool active )
		{
			stack = new Stack< UIFloatingText >( stackSize );

			for( var i = 0; i < stackSize; i++ )
			{
				var entity = GameObject.Instantiate( poolEntity );
				entity.transform.SetParent( parent );
				entity.gameObject.SetActive( active );
				stack.Push( entity );
			}
		}

		public UIFloatingText GiveEntity( Transform parent, bool active )
		{
			UIFloatingText entity;

			if( stack.Count > 0 )
				entity = stack.Pop();
			else 
			{
				entity = GameObject.Instantiate( poolEntity );
				entity.transform.SetParent( parent );
			}

			entity.gameObject.SetActive( active );
			return entity;
		}
#endregion

}
