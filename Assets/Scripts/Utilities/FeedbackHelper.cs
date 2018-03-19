using System;
using System.Collections;
using System.Collections.Generic;
using PlayGen.Unity.Utilities.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackHelper : MonoBehaviour
{
	[Serializable]
	public struct MeasuredFeedback
	{
		/// <summary>
		/// Key should match the key used to save the points during play
		/// </summary>
		public string Key;
		public Text Value;
	}

	public List<MeasuredFeedback> FeedbackPoints;

	public Text GetTextComponent(string key)
	{
		return FeedbackPoints.Find(k => k.Key == key).Value;
	}
}
