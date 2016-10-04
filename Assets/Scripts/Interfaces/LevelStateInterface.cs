using GameWork.Commands.States;
using GameWork.Interfacing;
using UnityEngine.UI;

public class LevelStateInterface : StateInterface
{
    public override void Initialize()
    {

        // TODO: For loop to iterate all of these and attach appropriate listeners.
        GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer/LevelPanel/LevelItem").GetComponent<Button>().onClick.AddListener(LoadLevel);
    }

    public override void Enter()
    {
        GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer").SetActive(true);
    }

    public override void Exit()
    {
        GameObjectUtilities.FindGameObject("LevelContainer/LevelPanelContainer").SetActive(false);
        GameObjectUtilities.FindGameObject("BackgroundContainer/MenuBackgroundImage").SetActive(false);
    }

    private void LoadLevel()
    {
        EnqueueCommand(new NextStateCommand());
    }
}
