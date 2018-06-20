using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;

using IntegratedAuthoringTool.DTOs;

using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;

public class QuestionnaireStateInput : TickStateInput
{
	private readonly string _panelRoute = "QuestionnaireContainer/QuestionnairePanelContainer";

	private readonly ScenarioController _scenarioController;

	public QuestionnaireStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	public event Action FinishClickedEvent;

	private GameObject _characterPrefab;
	private GameObject _characterPanel;
	private GameObject _characterObject;
	private GameObject _listChoicePrefab;
	private GameObject _dialoguePanel;
	private Text _questionText;

	protected override void OnInitialize()
	{
		_characterPrefab = Resources.Load("Prefabs/Characters/Generic") as GameObject;
		_characterPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/CharacterPanel");

		_listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
		_dialoguePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/QuestionnaireUI/QuestionPanel/AnswerPanel");

		_questionText = GameObjectUtilities.FindGameObject(_panelRoute + "/QuestionnaireUI/QuestionPanel/QuestionHolder/Question").GetComponent<Text>();
	}

	protected override void OnEnter()
	{
		_scenarioController.GetPlayerDialogueSuccessEvent += UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent += UpdateCharacterDialogue;
		_scenarioController.FinalStateEvent += HandleFinalState;

		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "QuestionnaireState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
		CommandQueue.AddCommand(new RefreshCharacterResponseCommand());
		_questionText.GetComponent<Text>().text = string.Empty;
		ShowCharacter();
	}

	protected override void OnExit()
	{
		UnityEngine.Object.Destroy(_characterObject);
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);

		_scenarioController.GetPlayerDialogueSuccessEvent -= UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent -= UpdateCharacterDialogue;
		_scenarioController.FinalStateEvent -= HandleFinalState;
	}

	public void ShowCharacter()
	{
		_characterObject = UnityEngine.Object.Instantiate(_characterPrefab);

		_characterObject.transform.SetParent(_characterPanel.transform, false);
		_characterObject.GetComponent<RectTransform>().offsetMax = Vector2.one;
		_characterObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
	}

	public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
	{
		foreach (Transform child in _dialoguePanel.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		if (dialogueActions.Length == 1)
		{
			CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueActions[0].Id));
		}
		else
		{
			var dialogueObject = UnityEngine.Object.Instantiate(_listChoicePrefab);
			var scrollRect = dialogueObject.GetComponent<ScrollRect>();
			var choiceItemPrefab = Resources.Load("Prefabs/DialogueItemScroll") as GameObject;
			var contentTotalHeight = 0f;
			dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
			for (var i = 0; i < dialogueActions.Length; i++)
			{
				var dialogueAction = dialogueActions[i];
				var choiceItem = UnityEngine.Object.Instantiate(choiceItemPrefab);
				choiceItem.transform.GetChild(0).GetComponent<Text>().text = Localization.GetAndFormat(dialogueAction.FileName, false, _scenarioController.ScenarioCode);
				contentTotalHeight += choiceItem.GetComponent<RectTransform>().rect.height;
				choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * choiceItem.GetComponent<RectTransform>().rect.height);
				choiceItem.GetComponent<Button>().onClick.AddListener(() => CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueAction.Id)));
				choiceItem.transform.SetParent(scrollRect.content, false);
			}
			scrollRect.content.sizeDelta = new Vector2(0, contentTotalHeight);
			dialogueObject.GetComponent<ScrollRect>().content.BestFit();
		}
	}

	public void UpdateCharacterDialogue(string text)
	{
		_questionText.GetComponent<Text>().text = text;
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
	}

	public void HandleFinalState()
	{
		FinishClickedEvent?.Invoke();
	}
}
