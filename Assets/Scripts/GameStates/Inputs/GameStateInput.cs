using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using IntegratedAuthoringTool.DTOs;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;

public class GameStateInput : TickStateInput
{
	private readonly string _panelRoute = "GameContainer/GamePanelContainer";

	public event Action HandleFinalStateEvent;

	private readonly ScenarioController _scenarioController;

	private GameObject _characterFemalePrefab;
	private GameObject _characterMalePrefab;

	private CharacterFaceController _characterController;
	private GameObject _characterPanel;
	private GameObject _dialoguePanel;

	private GameObject _feedbackPanel;
	private GameObject _feedbackElementPrefab;
	private readonly List<GameObject> _feedbackElements = new List<GameObject>();

	private GameObject _listChoicePrefab;
	private GameObject _npcDialoguePanel;
	private GameObject _characterObject;
	private Image _characterMood;

	public GameStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_characterFemalePrefab = Resources.Load("Prefabs/Characters/Female") as GameObject;
		_characterMalePrefab = Resources.Load("Prefabs/Characters/Male") as GameObject;
		_characterPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/CharacterPanel");
		_listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
		_dialoguePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/BottomPanel/DialogueOptionPanel");
		_npcDialoguePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/BottomPanel/NPCTextHolder/NPCText");
		_characterMood = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/TopBarPanel/StatusBar/Image").GetComponent<Image>();
		_feedbackPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/FeedbackPanel/IconHolder");
		_feedbackElementPrefab = Resources.Load("Prefabs/FeedbackElement") as GameObject;
		GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/TopBarPanel/ModulesButton").GetComponent<Button>().onClick.AddListener(() => CommandQueue.AddCommand(new ToggleModulesCommand()));
	}

	protected override void OnEnter()
	{
		CommandQueue.AddCommand(new CloseModulesCommand());
		var anim = _feedbackPanel.GetComponent<Animation>();
		anim.Stop();
		anim[anim.clip.name].time = 0f;
		_feedbackPanel.GetComponent<CanvasGroup>().alpha = 0f;

		foreach (var element in _feedbackElements)
		{
			UnityEngine.Object.Destroy(element.gameObject);
		}

		_scenarioController.GetPlayerDialogueSuccessEvent += UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent += UpdateCharacterDialogue;
		_scenarioController.GetCharacterStrongestEmotionSuccessEvent += UpdateCharacterExpression;
		_scenarioController.FinalStateEvent += HandleFinalState;
		_scenarioController.StopTalkAnimationEvent += StopCharacterTalkAnimation;
		_scenarioController.GetFeedbackEvent += UpdateFeedbackForChoice;

		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "GameState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});
		ShowCharacter(_scenarioController.CurrentCharacter);
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
		CommandQueue.AddCommand(new RefreshCharacterResponseCommand());
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
		_npcDialoguePanel.GetComponent<Text>().text = string.Empty;
	}

	protected override void OnExit()
	{
		UnityEngine.Object.Destroy(_characterObject);
		GameObjectUtilities.FindGameObject(_panelRoute).SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);

		_scenarioController.GetPlayerDialogueSuccessEvent -= UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent -= UpdateCharacterDialogue;
		_scenarioController.GetCharacterStrongestEmotionSuccessEvent -= UpdateCharacterExpression;
		_scenarioController.FinalStateEvent -= HandleFinalState;
		_scenarioController.StopTalkAnimationEvent -= StopCharacterTalkAnimation;
		_scenarioController.GetFeedbackEvent -= UpdateFeedbackForChoice;
	}

	public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
	{
		_characterObject = UnityEngine.Object.Instantiate(currentCharacter.BodyName == "Female" ? _characterFemalePrefab : _characterMalePrefab);
		_characterController = _characterObject.GetComponent<CharacterFaceController>();
		_characterController.CharacterId = "01";
		_characterController.Gender = currentCharacter.BodyName;
		_characterObject.transform.SetParent(_characterPanel.transform, false);
	}

	public void UpdateCharacterExpression(string emotion, float mood)
	{
		_characterMood.fillAmount = (mood + 10) / 20;
		_characterController.SetEmotion(emotion ?? "Idle");
		if (_scenarioController.IsTalking)
		{
			_characterController.StartTalkAnimation();
		}
	}

	public void UpdateFeedbackForChoice(Dictionary<string, int> feedback, ScenarioController.FeedbackMode feedbackMode)
	{
		// Make sure the anim plays from the beginning
		var anim = _feedbackPanel.GetComponent<Animation>();
		anim.Stop();
		anim[anim.clip.name].time = 0f;
		_feedbackPanel.GetComponent<CanvasGroup>().alpha = 0f;

		foreach (var element in _feedbackElements)
		{
			UnityEngine.Object.Destroy(element.gameObject);
		}

		if (feedback.Count > 0 && (int)feedbackMode >= 2)
		{
			var rect = _feedbackPanel.GetComponent<RectTransform>().rect;
			var width = (rect.width / feedback.Count) - 10;
			var height = Mathf.Min(rect.height, width / 3);

			foreach (var i in feedback)
			{
				var element = UnityEngine.Object.Instantiate(_feedbackElementPrefab, _feedbackPanel.transform, false);
				element.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
				element.transform.Find("Title").GetComponent<Text>().text = Localization.Get("POINTS_" + i.Key.ToUpper());
				element.transform.Find("Value").GetComponent<Text>().text = i.Value > 0 ? "+" + i.Value : i.Value.ToString();
				_feedbackElements.Add(element);
			}

			_feedbackPanel.BestFit();
			anim.Play();
		}
	}

	public void StopCharacterTalkAnimation()
	{
		_characterController.StopTalkAnimation();
	}

	public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
	{
		foreach (Transform child in _dialoguePanel.transform)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		var rnd = new System.Random();
		var randomDialogueActions = dialogueActions.OrderBy(dto => rnd.Next()).ToArray();
		var dialogueObject = UnityEngine.Object.Instantiate(_listChoicePrefab);
		var scrollRect = dialogueObject.GetComponent<ScrollRect>();
		var choiceItemPrefab = Resources.Load("Prefabs/DialogueItemScroll") as GameObject;
		var contentTotalHeight = 0f;

		dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
		
		for (var i = 0; i < randomDialogueActions.Length; i++)
		{
			var dialogueAction = randomDialogueActions[i];
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

	public void UpdateCharacterDialogue(string text)
	{
		_npcDialoguePanel.GetComponent<Text>().text = text;
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
	}

	public void HandleFinalState()
	{
		HandleFinalStateEvent?.Invoke();
	}
}