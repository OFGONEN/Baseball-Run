/* Created by and for usage of FF Studios (2021). */

using UnityEngine;

namespace FFStudio
{
	public class UIHorizontalProgressIndicator : UIProgressIndicator
	{
#region Fields
#endregion

#region Unity API
#endregion

#region API
#endregion

#region Implementation
        protected override void OnProgressChange()
        {
			var position             = indicator_BasePosition;
			    position.x           = Mathf.Lerp( indicator_BasePosition.x, indicator_EndPosition.x, indicatorProgress.sharedValue );
			    uiTransform.localPosition = position;
		}
		
		protected override void GetIndicatorPositions()
        {
            indicator_BasePosition = ( indicatingParentWorldPos[ 0 ] + indicatingParentWorldPos[ 1 ] ) / 2;
            indicator_EndPosition  = ( indicatingParentWorldPos[ 2 ] + indicatingParentWorldPos[ 3 ] ) / 2;

			indicator_BasePosition = indicatingParent.InverseTransformPoint( indicator_BasePosition );
			indicator_EndPosition  = indicatingParent.InverseTransformPoint( indicator_EndPosition );

			var width            = indicator_EndPosition.x - indicator_BasePosition.x;
			var horizontalOffset = width * offsetPercentage;

			indicator_BasePosition.x += horizontalOffset;
			indicator_EndPosition.x  -= horizontalOffset;
		}
#endregion
	}
}