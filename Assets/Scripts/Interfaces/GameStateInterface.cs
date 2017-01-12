using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using IntegratedAuthoringTool.DTOs;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.UI;

public class GameStateInterface : StateInterface
{
    private GameObject _characterPrefab;
    private CharacterFaceController _characterController;
    private GameObject _characterPanel;
    private GameObject _dialoguePanel;

    private GameObject _listChoicePrefab;
    private GameObject _multipleChoicePrefab;
    private GameObject _npcDialoguePanel;
    private Image _characterMood;

    /// <summary>
    /// 
    /// </summary>
    public override void Initialize()
    {
        _characterPrefab = Resources.Load("Prefabs/Characters/Female") as GameObject;
        _characterPanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/CharacterPanel");
        _listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
        _multipleChoicePrefab = Resources.Load("Prefabs/MultipleChoiceGroup") as GameObject;
        _dialoguePanel =
            GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/DialogueOptionPanel");
        _npcDialoguePanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/NPCText");
        var modulesButton = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/ModulesButton");
        _characterMood = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/TopBarPanel/StatusBar/Image").GetComponent<Image>();

        modulesButton.GetComponent<Button>().onClick.AddListener(delegate { EnqueueCommand(new ToggleModulesCommand()); });
    }

    public override void Enter()
    {
		RefreshPlayerDialogueOptions();
		//RefreshCharacterDialogueText();
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
		_npcDialoguePanel.GetComponent<Text>().text = "";
	}

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
    }


    public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
    {
        var characterObject = GameObject.Instantiate(_characterPrefab);
        _characterController = characterObject.GetComponent<CharacterFaceController>();
        _characterController.CharacterId = "01";//currentCharacter.BodyName; 
        _characterController.Gender = "Female";//currentCharacter.GetBeliefValue("Gender");

        characterObject.transform.SetParent(_characterPanel.transform, false);
        characterObject.GetComponent<RectTransform>().offsetMax = Vector2.one;
        characterObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
    }

    public void UpdateCharacterExpression(string emotion, float mood)
    {
        Debug.Log("EMOTION: " + emotion + ". MOOD: " + mood );
        _characterMood.fillAmount = (mood + 10)/20;
        if (emotion != null)
        {
            _characterController.SetEmotion(emotion);
        }
    }

    public void RefreshPlayerDialogueOptions()
    {
        EnqueueCommand(new RefreshPlayerDialogueCommand());
    }

    public void RefreshCharacterDialogueText()
    {
        EnqueueCommand(new RefreshCharacterResponseCommand());
    }

    public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
    {
        foreach (var child in _dialoguePanel.transform)
        {
            var childGameObject = child as Transform;
            GameObject.Destroy(childGameObject.gameObject);
        }


        // TODO: Refactor
        GameObject dialogueObject;
        if (dialogueActions.Length == 4)
        {
            dialogueObject = GameObject.Instantiate(_multipleChoicePrefab);
            for (var i = 0; i < dialogueActions.Length; i++)
            {
                var dialogueAction = dialogueActions[i];
                var optionText = dialogueAction.Utterance;
                var optionObject = dialogueObject.transform.GetChild(i);
                optionObject.GetChild(0).GetComponent<Text>().text = optionText;//
                optionObject.GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnDialogueOptionClick(dialogueAction);
                });
            }
			dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
		}
		else
        {
            dialogueObject = GameObject.Instantiate(_listChoicePrefab);
	        var scrollRect = dialogueObject.GetComponent<ScrollRect>();
            var choiceItemPrefab = Resources.Load("Prefabs/DialogueItemScroll") as GameObject;
	        var contentTotalHeight = 0f;
			dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
			for (var i = 0; i < dialogueActions.Length; i++)
            {
                var dialogueAction = dialogueActions[i];
                var optionText = dialogueAction.Utterance;
                var choiceItem = GameObject.Instantiate(choiceItemPrefab);
                choiceItem.transform.GetChild(0).GetComponent<Text>().text = optionText;
                //choiceItem.GetComponent<RectTransform>().rect = dialogueObject.GetComponent<Rect>().height;
                var offset = i*choiceItem.GetComponent<RectTransform>().rect.height;
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
        }
    }

    private void OnDialogueOptionClick(DialogueStateActionDTO dialogueAction)
    {
        EnqueueCommand(new SetPlayerActionCommand(dialogueAction.Id));
    }

    public void UpdateCharacterDialogue(string text)
    {
        _npcDialoguePanel.GetComponent<Text>().text = text;
        RefreshPlayerDialogueOptions();
    }

    public void HandleFinalState()
    {
        // put in a delay here (e.g. delayLastState command)
        EnqueueCommand(new NextStateCommand());
    }
}
