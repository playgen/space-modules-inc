using System.Collections.Specialized;
using GameWork.Core.Commands.States;
using GameWork.Core.Interfacing;
using UnityEngine;
using UnityEngine.UI;

public class ReviewStateInterface : StateInterface
{
    private Image _characterMood;
    private GameObject _reviewContent;
    private GameObject _clientChatPrefab;
    private GameObject _playerChatPrefab;

    public override void Initialize()
    {
        _characterMood = GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/StatusBar/MoodPanel/FillBar").GetComponent<Image>();
        _reviewContent = GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/Scroll View");
        _clientChatPrefab = Resources.Load("Prefabs/ClientChatItem") as GameObject;
        _playerChatPrefab = Resources.Load("Prefabs/PlayerChatItem") as GameObject;

        var nextButton =
                    GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/NextButton");
        nextButton.GetComponent<Button>().onClick.AddListener(delegate
        {
            EnqueueCommand(new NextStateCommand());
        });
    }

    public override void Enter()
    {
        EnqueueCommand(new GetReviewDataCommand());
        GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
    }

    public void BuildReviewData(OrderedDictionary history, float mood)
    {
        _characterMood.fillAmount = (mood + 10) / 20;
        ClearList();

        string[] dialogues = new string[history.Count];
        history.Keys.CopyTo(dialogues,0);
        for (var i = 0; i < dialogues.Length; i++)
        {
            Transform chatObject = null;

            var entryKey = history[i];
            if (entryKey == "Client")
            {
                chatObject = GameObject.Instantiate(_clientChatPrefab).transform;
            }
            else if (entryKey == "Player")
            {
                chatObject = GameObject.Instantiate(_playerChatPrefab).transform;
            }

            if (chatObject != null)
            {
                chatObject.FindChild("Panel").GetChild(0).GetComponent<Text>().text = dialogues[i];
                chatObject.transform.SetParent(_reviewContent.GetComponent<ScrollRect>().content, false);
            }
               

        }
    }

    private void ClearList()
    {
        foreach (RectTransform child in _reviewContent.GetComponent<ScrollRect>().content)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
