using System;
using System.Collections.Generic;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using IntegratedAuthoringTool.DTOs;

using PlayGen.Unity.Utilities.Extensions;
using PlayGen.Unity.Utilities.Text;
using PlayGen.Unity.Utilities.Localization;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;

public class GameStateInput : TickStateInput
{
	private readonly string _panelRoute = "GameContainer/GamePanelContainer";
	private readonly ScenarioController _scenarioController;
	private readonly List<GameObject> _feedbackElements = new List<GameObject>();

	public event Action HandleFinalStateEvent;

	private GameObject _panel;
	private GameObject _background;
	private CharacterFaceController _characterFemalePrefab;
	private CharacterFaceController _characterMalePrefab;
	private CharacterFaceController _characterController;
	private Transform _characterPanel;
	private Transform _dialoguePanel;
	private GameObject _feedbackPanel;
	private GameObject _feedbackElementPrefab;
	private GameObject _choiceItemPrefab;
	private ScrollRect _listChoicePrefab;
	private Text _npcDialoguePanel;
	private Image _characterMood;

	public GameStateInput(ScenarioController scenarioController)
	{
		_scenarioController = scenarioController;
	}

	protected override void OnInitialize()
	{
		_panel = GameObjectUtilities.FindGameObject(_panelRoute);
		_background = GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage");
		_characterFemalePrefab = Resources.Load<CharacterFaceController>("Prefabs/Characters/Female");
		_characterMalePrefab = Resources.Load<CharacterFaceController>("Prefabs/Characters/Male");
		_characterPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/CharacterPanel").transform;
		_choiceItemPrefab = Resources.Load<GameObject>("Prefabs/DialogueItemScroll");
		_listChoicePrefab = Resources.Load<ScrollRect>("Prefabs/ListChoiceGroup");
		_dialoguePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/BottomPanel/DialogueOptionPanel").transform;
		_npcDialoguePanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/BottomPanel/NPCTextHolder/NPCText").GetComponent<Text>();
		_characterMood = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/TopBarPanel/StatusBar/Image").GetComponent<Image>();
		_feedbackPanel = GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/FeedbackPanel/IconHolder");
		_feedbackElementPrefab = Resources.Load<GameObject>("Prefabs/FeedbackElement");
		GameObjectUtilities.FindGameObject(_panelRoute + "/GameUI/TopBarPanel/ModulesButton").GetComponent<Button>().onClick.AddListener(() => CommandQueue.AddCommand(new ToggleModulesCommand()));
	}

	protected override void OnEnter()
	{
		CommandQueue.AddCommand(new CloseModulesCommand());
		ResetFeedbackAnim();

		_scenarioController.GetPlayerDialogueSuccessEvent += UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent += UpdateCharacterDialogue;
		_scenarioController.GetCharacterStrongestEmotionSuccessEvent += UpdateCharacterExpression;
		_scenarioController.FinalStateEvent += HandleFinalState;
		_scenarioController.StopTalkAnimationEvent += StopCharacterTalkAnimation;
		_scenarioController.GetFeedbackEvent += UpdateFeedbackForChoice;

		TrackerEventSender.SendEvaluationEvent(TrackerEvalautionEvent.GameFlow, new Dictionary<TrackerEvaluationKey, string>
		{
			{ TrackerEvaluationKey.PieceType, "GameState" },
			{ TrackerEvaluationKey.PieceId, "0" },
			{ TrackerEvaluationKey.PieceCompleted, "success" }
		});
		ShowCharacter(_scenarioController.CurrentCharacter);
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
		CommandQueue.AddCommand(new RefreshCharacterResponseCommand());
		_panel.SetActive(true);
		_background.SetActive(true);
		_npcDialoguePanel.text = string.Empty;
	}

	protected override void OnExit()
	{
		UnityEngine.Object.Destroy(_characterController.gameObject);
		_panel.SetActive(false);
		_background.SetActive(false);

		_scenarioController.GetPlayerDialogueSuccessEvent -= UpdatePlayerDialogue;
		_scenarioController.GetCharacterDialogueSuccessEvent -= UpdateCharacterDialogue;
		_scenarioController.GetCharacterStrongestEmotionSuccessEvent -= UpdateCharacterExpression;
		_scenarioController.FinalStateEvent -= HandleFinalState;
		_scenarioController.StopTalkAnimationEvent -= StopCharacterTalkAnimation;
		_scenarioController.GetFeedbackEvent -= UpdateFeedbackForChoice;
	}

	public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
	{
		_characterController = UnityEngine.Object.Instantiate(currentCharacter.BodyName == "Female" ? _characterFemalePrefab : _characterMalePrefab, _characterPanel, false);
		_characterController.CharacterId = "01";
		_characterController.Gender = currentCharacter.BodyName;
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
		ResetFeedbackAnim();

		if (feedback.Count > 0 && feedbackMode >= ScenarioController.FeedbackMode.InGame)
		{
			var rect = _feedbackPanel.RectTransform().rect;
			var width = (rect.width / feedback.Count) - 10;
			var height = Mathf.Min(rect.height, width / 3);

			foreach (var i in feedback)
			{
				var element = UnityEngine.Object.Instantiate(_feedbackElementPrefab, _feedbackPanel.transform, false);
				element.RectTransform().sizeDelta = new Vector2(width, height);
				element.transform.FindText("Title").text = Localization.Get("POINTS_" + i.Key.ToUpper());
				element.transform.FindText("Value").text = i.Value > 0 ? "+" + i.Value : i.Value.ToString();
				_feedbackElements.Add(element);
			}

			_feedbackPanel.BestFit();
			_feedbackPanel.GetComponent<Animation>().Play();
		}
	}

	private void ResetFeedbackAnim()
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
	}

	public void StopCharacterTalkAnimation()
	{
		_characterController.StopTalkAnimation();
	}

	public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
	{
		foreach (Transform child in _dialoguePanel)
		{
			UnityEngine.Object.Destroy(child.gameObject);
		}
		var rnd = new System.Random();
		var randomDialogueActions = dialogueActions.OrderBy(dto => rnd.Next()).ToArray();
		var dialogueObject = UnityEngine.Object.Instantiate(_listChoicePrefab, _dialoguePanel, false);
		var contentTotalHeight = 0f;

		for (var i = 0; i < randomDialogueActions.Length; i++)
		{
			var dialogueAction = randomDialogueActions[i];
			var choiceItem = UnityEngine.Object.Instantiate(_choiceItemPrefab, dialogueObject.content, false);
			choiceItem.GetComponentInChildren<Text>().text = Localization.GetAndFormat(dialogueAction.FileName, false, _scenarioController.ScenarioCode);
			contentTotalHeight += choiceItem.GetComponent<RectTransform>().rect.height;
			choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * choiceItem.GetComponent<RectTransform>().rect.height);
			choiceItem.GetComponent<Button>().onClick.AddListener(() => CommandQueue.AddCommand(new SetPlayerActionCommand(dialogueAction.Id)));
		}
		dialogueObject.content.sizeDelta = new Vector2(0, contentTotalHeight);
		dialogueObject.content.BestFit();
	}

	public void UpdateCharacterDialogue(string text)
	{
		_npcDialoguePanel.text = text;
		CommandQueue.AddCommand(new RefreshPlayerDialogueCommand());
	}

	public void HandleFinalState()
	{
		HandleFinalStateEvent?.Invoke();
	}
}