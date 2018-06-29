using UnityEngine;

[RequireComponent(typeof(Camera))]
[DisallowMultipleComponent]
public class ForcePortraitCamera : MonoBehaviour
{
	private void Awake ()
	{
		var cam = GetComponent<Camera>();
		if (cam && cam.aspect > 1)
		{
			var portrait = 1 / (cam.aspect * cam.aspect);
			var x = (1 - portrait) / 2;
			var y = 0;
			var w = portrait;
			var h = 1;
			cam.rect = new Rect(new Vector2(x, y), new Vector2(w, h));
		}
	}
}
