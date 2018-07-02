using System;
using System.Collections.Generic;
using GameWork.Core.States.Tick.Input;

using IntegratedAuthoringTool.DTOs;

using PlayGen.Unity.Utilities.Extensions;

using UnityEngine.UI;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;

using UnityEngine;

public class QuestionnaireStateInput : TickStateInput
{
	private readonly string _panelRoute = "QuestionnaireContainer/QuestionnairePanelContainer";
	private readonly ScenarioController _scenarioController;

	public event Action FinishClickedEvent;

	private GameObject _panel;
	private GameObject _background;
	private GameObject _characterPrefab;
	private Transform _characterPanel;
	private RectTransform _characterObject;
	private GameObject _choiceItemPrefab;
	private GameObject _listChoicePrefab;
	private Transform _dialoguePanel;
	private Text _questionText;

	public QuestionnaireStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage");

		_characterPrefab = Resources.Load<GameObject>("Prefabs/Characters/Generic");
		_characterPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/CharacterPanel").transform;

		_choiceItemPrefab = Resources.Load<GameObject>("Prefabs/DialogueItemScroll");
		_listChoicePrefab = Resources.Load<GameObject>("Prefabs/ListChoiceGroup");
		_dialoguePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/QuestionnaireUI/QuestionPanel/AnswerPanel").transform;

		_questionText = GameObjectUtilities.FindGameObject(_panelRoute + "/QuestionnaireUI/QuestionPanel/QuestionHolder/Question").GetComponent<Text>();
	}

	protected override void OnEnter()
	{
		_scenarioController.GetPlayerDialogueSuccessEvent += UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent += UpdateCharacterDialogue;
		_scenarioController.FinalStateEvent += HandleFinalState;

		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "QuestionnaireState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});
		_panel.SetActive(true);
		_background.SetActive(true);
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
		CommandQueue.AddCommand(new RefreshCharacterResponseCommand());
		_questionText.text = string.Empty;
		ShowCharacter();
	}

	protected override void OnExit()
	{
		UnityEngine.Object.Destroy(_characterObject.gameObject);
		_panel.SetActive(false);
		_background.SetActive(false);

		_scenarioController.GetPlayerDialogueSuccessEvent -= UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent -= UpdateCharacterDialogue;
		_scenarioController.FinalStateEvent -= HandleFinalState;
	}

	public void ShowCharacter()
	{
		_characterObject = UnityEngine.Object.Instantiate(_characterPrefab, _characterPanel, false).RectTransform();
		_characterObject.offsetMax = Vector2.one;
		_characterObject.offsetMin = Vector2.zero;
	}

	public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
	{
		foreach (Transform child in _dialoguePanel)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		//skip dialogue where the player is only presented with one option
		if (dialogueActions.Length == 1)
		{
			CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueActions[0].Id));
		}
		else
		{
			var dialogueObject = UnityEngine.Object.Instantiate(_listChoicePrefab, _dialoguePanel, false);
			var scrollRect = dialogueObject.GetComponent<ScrollRect>();
			var contentTotalHeight = 0f;
			for (var i = 0; i < dialogueActions.Length; i++)
			{
				var dialogueAction = dialogueActions[i];
				var choiceItem = UnityEngine.Object.Instantiate(_choiceItemPrefab, scrollRect.content, false);
				choiceItem.GetComponentInChildren<Text>().text = Localization.GetAndFormat(dialogueAction.FileName, false, _scenarioController.ScenarioCode);
				contentTotalHeight += choiceItem.RectTransform().rect.height;
				choiceItem.RectTransform().anchoredPosition = new Vector2(0, -i * choiceItem.GetComponent<RectTransform>().rect.height);
				choiceItem.GetComponent<Button>().onClick.AddListener(() => CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueAction.Id)));
			}
			scrollRect.content.sizeDelta = new Vector2(0, contentTotalHeight);
			dialogueObject.GetComponent<ScrollRect>().content.BestFit();
		}
	}

	public void UpdateCharacterDialogue(string text)
	{
		_questionText.text = text;
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
	}

	public void HandleFinalState()
	{
		FinishClickedEvent?.Invoke();
	}
}
