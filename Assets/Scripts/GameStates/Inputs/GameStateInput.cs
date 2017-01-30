using System;
using System.Linq;
using GameWork.Core.States.Tick.Input;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Inputs
{
	public class GameStateInput : TickStateInput
	{
		public event Action HandleFinalStateEvent;

		private readonly ScenarioController _scenarioController;

		private GameObject _characterFemalePrefab;
		private GameObject _characterMalePrefab;

		private CharacterFaceController _characterController;
		private GameObject _characterPanel;
		private GameObject _dialoguePanel;

		private GameObject _listChoicePrefab;
		private GameObject _multipleChoicePrefab;
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
			_multipleChoicePrefab = Resources.Load("Prefabs/MultipleChoiceGroup") as GameObject;
			_dialoguePanel =
				GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/DialogueOptionPanel");
			_npcDialoguePanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/NPCTextHolder/NPCText");
			var modulesButton = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/ModulesButton");
			_characterMood = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/StatusBar/Image").GetComponent<Image>();

			modulesButton.GetComponent<Button>().onClick.AddListener(delegate { CommandQueue.AddCommand(new ToggleModulesCommand()); });


			_scenarioController.GetPlayerDialogueSuccessEvent += UpdatePlayerDialogue;
			_scenarioController.GetCharacterDialogueSuccessEvent += UpdateCharacterDialogue;
			_scenarioController.GetCharacterStrongestEmotionSuccessEvent += UpdateCharacterExpression;
			_scenarioController.FinalStateEvent += HandleFinalState;
			_scenarioController.StopTalkAnimationEvent += StopCharacterTalkAnimation;
		}

		protected override void OnTerminate()
		{
			_scenarioController.GetCharacterStrongestEmotionSuccessEvent -= UpdateCharacterExpression;
			_scenarioController.GetCharacterDialogueSuccessEvent -= UpdateCharacterDialogue;
			_scenarioController.GetPlayerDialogueSuccessEvent -= UpdatePlayerDialogue;
			_scenarioController.FinalStateEvent -= HandleFinalState;
			_scenarioController.StopTalkAnimationEvent -= StopCharacterTalkAnimation;
		}

		protected override void OnEnter()
		{
			Tracker.T.accessible.Accessed("GameState");

			ShowCharacter(_scenarioController.CurrentCharacter);
			RefreshPlayerDialogueOptions();
			RefreshCharacterDialogueText();
			GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
			GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
			_npcDialoguePanel.GetComponent<Text>().text = "";
		}

		protected override void OnExit()
		{
			GameObject.Destroy(_characterObject);
			GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(false);
			GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
		}

		public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
		{
			if (currentCharacter.BodyName == "Female")
			{
				_characterObject = GameObject.Instantiate(_characterFemalePrefab);
			}
			else
			{
				_characterObject = GameObject.Instantiate(_characterMalePrefab);

			}
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
			Debug.Log("EMOTION: " + emotion + ". MOOD: " + mood );
			_characterMood.fillAmount = (mood + 10)/20;
			if (emotion != null)
			{
				_characterController.SetEmotion(emotion);
			}
			else
			{
				_characterController.SetEmotion("Idle");
			}
			if (_scenarioController.IsTalking) 
			{
				_characterController.StartTalkAnimation();
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
					GameObject.Destroy(childObj.gameObject);
				}
			

			}
			foreach (var child in _dialoguePanel.transform)
			{
				var childGameObject = child as Transform;
				GameObject.Destroy(childGameObject.gameObject);
			}


			// TODO: Refactor
			GameObject dialogueObject;
			var rnd = new System.Random();
			var randomDialogueActions = dialogueActions.OrderBy(dto => rnd.Next()).ToArray(); 

			//	For Multiple Choice view
			//      if (randomDialogueActions.Length == 4)
			//      {
			//          dialogueObject = GameObject.Instantiate(_multipleChoicePrefab);
			//          for (var i = 0; i < randomDialogueActions.Length; i++)
			//          {
			//              var dialogueAction = randomDialogueActions[i];
			//              var optionText = dialogueAction.Utterance;
			//              var optionObject = dialogueObject.transform.GetChild(i);
			//              optionObject.GetChild(0).GetComponent<Text>().text = optionText;//
			//              optionObject.GetComponent<Button>().onClick.AddListener(delegate
			//              {
			//                  OnDialogueOptionClick(dialogueAction);
			//              });
			//          }
			//	dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
			//}

			dialogueObject = GameObject.Instantiate(_listChoicePrefab);
			var scrollRect = dialogueObject.GetComponent<ScrollRect>();
			var choiceItemPrefab = Resources.Load("Prefabs/DialogueItemScroll") as GameObject;
			var contentTotalHeight = 0f;
			dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
			for (var i = 0; i < randomDialogueActions.Length; i++)
			{
				var dialogueAction = randomDialogueActions[i];
				var optionText = dialogueAction.Utterance;
				var choiceItem = GameObject.Instantiate(choiceItemPrefab);
				choiceItem.transform.GetChild(0).GetComponent<Text>().text = optionText;
				var offset = i*choiceItem.GetComponent<RectTransform>().rect.height;
				contentTotalHeight += choiceItem.GetComponent<RectTransform>().rect.height;
				choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset * 0.85f);
				choiceItem.GetComponent<Button>().onClick.AddListener(delegate
				{
					OnDialogueOptionClick(dialogueAction);
				});
				choiceItem.transform.SetParent(scrollRect.content, false);
			}
			var rectContent = scrollRect.content;
			rectContent.sizeDelta = new Vector2(0, contentTotalHeight);
			CommandQueue.AddCommand(new UpdateDialogueFontSizeCommand(dialogueObject));
		}

		public void ResizeOptions(GameObject dialogueObject)
		{
			int smallestFontSize = 0;
			foreach (var obj in dialogueObject.GetComponent<ScrollRect>().content)
			{
				var textObj = obj as Transform;
				var text = textObj.GetComponentInChildren<Text>();
				text.resizeTextForBestFit = true;
				text.resizeTextMinSize = 1;
				text.resizeTextMaxSize = 100;
				text.fontSize = text.resizeTextMaxSize;
				text.cachedTextGenerator.Invalidate();
				text.cachedTextGenerator.Populate(text.text, text.GetGenerationSettings(text.rectTransform.rect.size));
				text.resizeTextForBestFit = false;
				var newSize = text.cachedTextGenerator.fontSizeUsedForBestFit;

				var newSizeRescale = text.rectTransform.rect.size.x / text.cachedTextGenerator.rectExtents.size.x;
				if (text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y < newSizeRescale)
				{
					newSizeRescale = text.rectTransform.rect.size.y / text.cachedTextGenerator.rectExtents.size.y;
				}
				newSize = Mathf.FloorToInt(newSize * newSizeRescale);
				if (newSize < smallestFontSize || smallestFontSize == 0)
				{
					smallestFontSize = newSize;
				}
			}
			foreach (var obj in dialogueObject.GetComponent<ScrollRect>().content)
			{
				var textObj = obj as Transform;
				var text = textObj.GetComponentInChildren<Text>();
				if (!text)
				{
					continue;
				}
				text.fontSize = smallestFontSize;
			}
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
			HandleFinalStateEvent();
		}
	}
}
