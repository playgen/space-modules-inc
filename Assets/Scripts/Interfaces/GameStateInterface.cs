using GameWork.Interfacing;
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

    public override void Initialize()
    {
        _characterPrefab = Resources.Load("Prefabs/Characters/Female") as GameObject;
        _characterController = _characterPrefab.GetComponent<CharacterFaceController>();
        _characterPanel = GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/CharacterPanel");

        _listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
        _multipleChoicePrefab = Resources.Load("Prefabs/MultipleChoiceGroup") as GameObject;
        _dialoguePanel =
            GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer/GameUI/BottomPanel/DialogueOptionPanel");


    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("GameContainer/GamePanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
    }


    public void ShowCharacter(RolePlayCharacterAsset currentCharacter)
    {
        _characterController.CharacterId = "01";//currentCharacter.BodyName; 
        _characterController.Gender = "Female";//currentCharacter.GetBeliefValue("Gender");
        var characterObject = GameObject.Instantiate(_characterPrefab);
        characterObject.transform.SetParent(_characterPanel.transform, false);
        characterObject.GetComponent<RectTransform>().offsetMax = Vector2.one;
        characterObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;

        RefreshPlayerDialogueOptions();
    }

    public void UpdateCharacterExpression(string emotion)
    {
        _characterController.SetEmotion(emotion);
    }

    public void RefreshPlayerDialogueOptions()
    {
        EnqueueCommand(new RefreshPlayerDialogueCommand());
    }

    public void UpdatePlayerDialogue(DialogueStateActionDTO[] dialogueActions)
    {
        foreach (var child in _dialoguePanel.transform)
        {
            var childGameObject = child as GameObject;
            GameObject.Destroy(childGameObject);
        }


        // TODO: Refactor
        GameObject dialogueObject;
        if (dialogueActions.Length == 3)
        {
            dialogueObject = GameObject.Instantiate(_multipleChoicePrefab);
            for (var i = 0; i < dialogueActions.Length; i++)
            {
                var dialogueAction = dialogueActions[i];
                var optionText = dialogueAction.Utterance;
                var optionObject = dialogueObject.transform.GetChild(i).GetChild(0);
                optionObject.GetComponent<Text>().text = optionText;
            }
        }
        else
        {
            dialogueObject = GameObject.Instantiate(_listChoicePrefab);
            var choiceItemPrefab = Resources.Load("Prefabs/DialogueItemScroll") as GameObject;

            for (var i = 0; i < dialogueActions.Length; i++)
            {
                var dialogueAction = dialogueActions[i];
                var optionText = dialogueAction.Utterance;
                var choiceItem = GameObject.Instantiate(choiceItemPrefab);
                choiceItem.transform.GetChild(0).GetComponent<Text>().text = optionText;
                var offset = i*choiceItem.GetComponent<RectTransform>().rect.height;
                choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);
                choiceItem.GetComponent<Button>().onClick.AddListener(delegate
                {
                    OnDialogueOptionClick(dialogueAction);
                });
                choiceItem.transform.SetParent(dialogueObject.GetComponent<ScrollRect>().content, false);
            }

        }
        dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
    }

    private void OnDialogueOptionClick(DialogueStateActionDTO dialogueAction)
    {
        EnqueueCommand(new SetPlayerActionCommand(dialogueAction.Id));
        foreach (var s in dialogueAction.Style)
        {
            Debug.Log(s);

        }
    }
}
