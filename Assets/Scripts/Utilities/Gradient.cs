﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
	public Color32 topColor = Color.white;
	public Color32 bottomColor = Color.black;

	public override void ModifyMesh(VertexHelper helper)
	{
		if (!IsActive() || helper.currentVertCount == 0)
			return;

		var vertices = new List<UIVertex>();
		helper.GetUIVertexStream(vertices);

		var bottomY = vertices[0].position.y;
		var topY = vertices[0].position.y;

		for (var i = 1; i < vertices.Count; i++)
		{
			var y = vertices[i].position.y;
			if (y > topY)
			{
				topY = y;
			}
			else if (y < bottomY)
			{
				bottomY = y;
			}
		}

		var uiElementHeight = topY - bottomY;

		var v = new UIVertex();

		for (var i = 0; i < helper.currentVertCount; i++)
		{
			helper.PopulateUIVertex(ref v, i);
			v.color = Color32.Lerp(bottomColor, topColor, (v.position.y - bottomY) / uiElementHeight);
			helper.SetUIVertex(v, i);
		}
	}
}