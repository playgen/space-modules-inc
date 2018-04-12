using System;
using UnityEngine.EventSystems;

public class ContentSizeFitterHelper : UIBehaviour
{
	public Action Action;

	protected override void OnRectTransformDimensionsChange()
	{
		Action();
	}
}
