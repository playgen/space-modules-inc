﻿using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using IntegratedAuthoringTool.DTOs;
using PlayGen.Unity.Utilities.BestFit;
using PlayGen.Unity.Utilities.Localization;
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
		_characterPanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/CharacterPanel");
		_listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
		_dialoguePanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/DialogueOptionPanel");
		_npcDialoguePanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/NPCTextHolder/NPCText");
		_characterMood = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/StatusBar/Image").GetComponent<Image>();
		_feedbackPanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/FeedbackPanel/IconHolder");
		_feedbackElementPrefab = Resources.Load("Prefabs/FeedbackElement") as GameObject;
		GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/ModulesButton").GetComponent<Button>().onClick.AddListener(delegate { CommandQueue.AddCommand(new ToggleModulesCommand()); });

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
		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvents.GameFlow, new Dictionary<TrackerEvaluationKeys, string>
		{
			{ TrackerEvaluationKeys.Type, "GameState" },
			{ TrackerEvaluationKeys.Id, "0" },
			{ TrackerEvaluationKeys.Completed, "success" }
		});
		ShowCharacter(_scenarioController.CurrentCharacter);
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
		CommandQueue.AddCommand(new RefreshCharacterResponseCommand());
		GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
		_npcDialoguePanel.GetComponent<Text>().text = string.Empty;
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
			var width = rect.width / feedback.Count;
			var height = Mathf.Min(rect.height, width / 3);
			width = Mathf.Min(width, _feedbackElementPrefab.GetComponent<RectTransform>().rect.width);

			foreach (var i in feedback)
			{
				var element = UnityEngine.Object.Instantiate(_feedbackElementPrefab);
				element.transform.SetParent(_feedbackPanel.transform, false);
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
			choiceItem.transform.GetChild(0).GetComponent<Text>().text = dialogueAction.Utterance;
			contentTotalHeight += choiceItem.GetComponent<RectTransform>().rect.height;
			choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * choiceItem.GetComponent<RectTransform>().rect.height);
			choiceItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueAction.Id));
			});
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