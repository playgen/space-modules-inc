using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using IntegratedAuthoringTool.DTOs;
using PlayGen.Unity.Utilities.BestFit;

using RAGE.Analytics;

using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;

public class GameStateInput : TickStateInput
{
	public event Action HandleFinalStateEvent;

	private readonly ScenarioController _scenarioController;

	private GameObject _characterFemalePrefab;
	private GameObject _characterMalePrefab;

	private CharacterFaceController _characterController;
	private GameObject _characterPanel;
	private GameObject _dialoguePanel;

	private GameObject _feedbackPanel;
	private GameObject _feedbackElementPrefab;
	private List<GameObject> _feedbackElements = new List<GameObject>();

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
		_characterPanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/CharacterPanel");
		_listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
		_dialoguePanel =
			GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/DialogueOptionPanel");
		_npcDialoguePanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/NPCTextHolder/NPCText");
		var modulesButton = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/ModulesButton");
		_characterMood = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/StatusBar/Image").GetComponent<Image>();

		_feedbackPanel =
			GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/FeedbackPanel/IconHolder");
		_feedbackElementPrefab = Resources.Load("Prefabs/FeedbackElement") as GameObject;
		modulesButton.GetComponent<Button>().onClick.AddListener(delegate { CommandQueue.AddCommand(new ToggleModulesCommand()); });


		_scenarioController.GetPlayerDialogueSuccessEvent += UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent += UpdateCharacterDialogue;
		_scenarioController.GetCharacterStrongestEmotionSuccessEvent += UpdateCharacterExpression;
		_scenarioController.FinalStateEvent += HandleFinalState;
		_scenarioController.StopTalkAnimationEvent += StopCharacterTalkAnimation;
		_scenarioController.GetFeedbackEvent += UpdateFeedbackForChoice;
	}

	protected override void OnTerminate()
	{
		_scenarioController.GetCharacterStrongestEmotionSuccessEvent -= UpdateCharacterExpression;
		_scenarioController.GetCharacterDialogueSuccessEvent -= UpdateCharacterDialogue;
		_scenarioController.GetPlayerDialogueSuccessEvent -= UpdatePlayerDialogue;
		_scenarioController.FinalStateEvent -= HandleFinalState;
		_scenarioController.StopTalkAnimationEvent -= StopCharacterTalkAnimation;
		_scenarioController.GetFeedbackEvent -= UpdateFeedbackForChoice;
	}

	protected override void OnEnter()
	{
		Tracker.T.Accessible.Accessed("GameState", AccessibleTracker.Accessible.Screen);

		ShowCharacter(_scenarioController.CurrentCharacter);
		RefreshPlayerDialogueOptions();
		RefreshCharacterDialogueText();
		GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
		_npcDialoguePanel.GetComponent<Text>().text = "";
	}

	protected override void OnExit()
	{
		UnityEngine.Object.Destroy(_characterObject);
		GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
	}

	public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
	{
		_characterObject = UnityEngine.Object.Instantiate(currentCharacter.BodyName == "Female" ? _characterFemalePrefab : _characterMalePrefab);
		_characterController = _characterObject.GetComponent<CharacterFaceController>();
		_characterController.CharacterId = "01";//currentCharacter.BodyName; 
		_characterController.Gender = currentCharacter.BodyName; //currentCharacter.GetBeliefValue("Gender");

		_characterObject.transform.SetParent(_characterPanel.transform, false);
		_characterObject.GetComponent<RectTransform>().offsetMax = Vector2.one;
		_characterObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
	}

	public void ResetGameStateUi()
	{

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
			UnityEngine.Object.DestroyImmediate(element.gameObject);
		}

		if (feedback.Count > 0 && (int)feedbackMode >= 2)
		{
			var rect = _feedbackPanel.GetComponent<RectTransform>().rect;
			var width = rect.width / feedback.Count;
			var height = rect.height;
			width = Mathf.Min(width, _feedbackElementPrefab.GetComponent<RectTransform>().rect.width);

			foreach (var i in feedback)
			{
				var element = UnityEngine.Object.Instantiate(_feedbackElementPrefab);
				element.transform.SetParent(_feedbackPanel.transform, false);

				element.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

				var iconPath = "Prefabs/Icons/" + i.Key;
				var icon = Resources.Load<Sprite>(iconPath);
				element.GetComponentInChildren<Image>().sprite = icon;
				element.GetComponentInChildren<Text>().text = i.Value > 0 ? "+" + i.Value : i.Value.ToString();
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

	public void RefreshPlayerDialogueOptions()
	{
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
	}

	public void RefreshCharacterDialogueText()
	{
		CommandQueue.AddCommand(new RefreshCharacterResponseCommand());
	}

	public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
	{

		if (_dialoguePanel.GetComponentInChildren<ScrollRect>())
		{
			foreach (var child in _dialoguePanel.GetComponentInChildren<ScrollRect>().content)
			{
				var childObj = child as Transform;
				if (childObj != null) UnityEngine.Object.Destroy(childObj.gameObject);
			}


		}
		foreach (var child in _dialoguePanel.transform)
		{
			var childGameObject = child as Transform;
			if (childGameObject != null) UnityEngine.Object.Destroy(childGameObject.gameObject);
		}


		// TODO: Refactor
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
			var optionText = dialogueAction.Utterance;
			var choiceItem = UnityEngine.Object.Instantiate(choiceItemPrefab);
			choiceItem.transform.GetChild(0).GetComponent<Text>().text = optionText;
			var offset = i * choiceItem.GetComponent<RectTransform>().rect.height;
			contentTotalHeight += choiceItem.GetComponent<RectTransform>().rect.height;
			choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
			choiceItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnDialogueOptionClick(dialogueAction);
			});
			choiceItem.transform.SetParent(scrollRect.content, false);
		}
		var rectContent = scrollRect.content;
		rectContent.sizeDelta = new Vector2(0, contentTotalHeight);
		//CommandQueue.AddCommand(new UpdateDialogueFontSizeCommand(dialogueObject));
		ResizeOptions(dialogueObject);
	}

	public void ResizeOptions(GameObject dialogueObject)
	{
		dialogueObject.GetComponent<ScrollRect>().content.BestFit();
	}

	private void OnDialogueOptionClick(DialogueStateActionDTO dialogueAction)
	{
		CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueAction.Id));
	}

	public void UpdateCharacterDialogue(string text)
	{
		_npcDialoguePanel.GetComponent<Text>().text = text;
		RefreshPlayerDialogueOptions();
	}

	public void HandleFinalState()
	{
		if (HandleFinalStateEvent != null) HandleFinalStateEvent();
	}
}