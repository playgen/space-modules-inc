using System.Linq;

using PlayGen.SUGAR.Unity;

using UnityEngine;
using UnityEngine.UI;

using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

public class AchievementListInterface : BaseEvaluationListInterface
{
	/// <summary>
	/// An array of the AchievementItemInterfaces on this GameObject, set in the Inspector.
	/// </summary>
	[Tooltip("An array of the AchievementItemInterfaces on this GameObject, set in the Inspector.")]
	[SerializeField]
	private AchievementItemInterface[] _achievementItems;

	/// <summary>
	/// Trigger DoBestFit method and add event listeners for when resolution and language changes.
	/// </summary>
	private void OnEnable()
	{
		DoBestFit();
		BestFit.ResolutionChange += DoBestFit;
		Localization.LanguageChange += OnLanguageChange;
	}

	/// <summary>
	/// Remove event listeners.
	/// </summary>
	private void OnDisable()
	{
		BestFit.ResolutionChange -= DoBestFit;
		Localization.LanguageChange -= OnLanguageChange;
	}

	protected override void PreDisplay()
	{
	}

	/// <summary>
	/// Adjust AchievementItemInterface pool to display a page of achievements.
	/// </summary>
	protected override void Draw()
	{
		var achievementList = SUGARManager.Evaluation.Progress.Take(_achievementItems.Length).ToList();
		for (int i = 0; i < _achievementItems.Length; i++)
		{
			if (i >= achievementList.Count)
			{
				_achievementItems[i].Disable();
			}
			else
			{
				_achievementItems[i].SetText(achievementList[i].Name, Mathf.Approximately(achievementList[i].Progress, 1.0f));
			}
		}
		_achievementItems.Select(t => t.gameObject).BestFit();
	}

	/// <summary>
	/// If a user signs in via this panel, refresh the current page (which should be page 1).
	/// </summary>
	protected override void OnSignIn()
	{
		Show(true);
	}

	/// <summary>
	/// Set the text of all buttons and all achievements to be as big as possible and the same size within the same grouping.
	/// </summary>
	private void DoBestFit()
	{
		_achievementItems.Select(t => t.gameObject).BestFit();
		GetComponentsInChildren<Button>(true).Select(t => t.gameObject).BestFit();
	}

	/// <summary>
	/// Refresh the current page to ensure any text set in code is also translated.
	/// </summary>
	private void OnLanguageChange()
	{
		Show(true);
	}
}
