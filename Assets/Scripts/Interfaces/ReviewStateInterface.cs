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
        var nextButton =
            GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/NextButton");
        nextButton.AddComponent<Button>().onClick.AddListener(delegate
        {
            EnqueueCommand(new NextStateCommand());
        });
        _characterMood = GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/StatusBar/MoodPanel/FillBar").GetComponent<Image>();
        _reviewContent = GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer/ReviewPanel/ScrollView");
        _clientChatPrefab = Resources.Load("Prefabs/ClientChatItem") as GameObject;
        _playerChatPrefab = Resources.Load("Prefabs/PlayerChatItem") as GameObject;
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(true);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(true);
        //EnqueueCommand( new GetReviewDatas);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("ReviewContainer/ReviewPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/CallBackgroundImage").SetActive(false);
    }

    public void BuildChatHistory(OrderedDictionary history)
    {
        string[] entryKeys = new string[history.Count];
        history.Keys.CopyTo(entryKeys,0);
        for (var i = 0; i < entryKeys.Length; i++)
        {
            Transform chatObject = null;

            var entryKey = entryKeys[i];
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
                chatObject.FindChild("Panel").GetChild(0).GetComponent<Text>().text = history[i].ToString();
                chatObject.transform.SetParent(_reviewContent.GetComponent<ScrollRect>().content, false);
            }
               

        }
    }
}
