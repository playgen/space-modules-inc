using System.Diagnostics;
using GameWork.Core.States;

public class GameState : TickableSequenceState
{
    private readonly GameStateInterface _interface;
    private ScenarioController _scenarioController;
    private ModulesController _modulesController;

    public const string StateName = "GameState";

    public GameState(ScenarioController scenarioController, ModulesController modulesController, GameStateInterface @interface)
    {
        _interface = @interface;
        _scenarioController = scenarioController;
        _modulesController = modulesController;
    }

    public override void Initialize()
    {
        _interface.Initialize();
    }

    public override void Terminate()
    {
        _interface.Terminate();
    }

    public override void Enter()
    {
        _interface.ShowCharacter(_scenarioController.CurrentCharacter);
        _scenarioController.GetPlayerDialogueSuccessEvent += _interface.UpdatePlayerDialogue;
        _scenarioController.GetCharacterDialogueSuccessEvent += _interface.UpdateCharacterDialogue;
        _scenarioController.GetCharacterStrongestEmotionSuccessEvent += _interface.UpdateCharacterExpression;
        _interface.Enter();
    }

    public override void Exit()
    {
        _scenarioController.GetCharacterStrongestEmotionSuccessEvent -= _interface.UpdateCharacterExpression;
        _scenarioController.GetCharacterDialogueSuccessEvent -= _interface.UpdateCharacterDialogue;
        _scenarioController.GetPlayerDialogueSuccessEvent -= _interface.UpdatePlayerDialogue;
        _interface.Exit();
    }

    public override string Name
    {
        get { return StateName; }
    }

    public override void NextState()
    {
        throw new System.NotImplementedException();
    }

    public override void PreviousState()
    {
        ChangeState(LevelState.StateName);
    }

    public override void Tick(float deltaTime)
    {
        if (_interface.HasCommands)
        {
            var command = _interface.TakeFirstCommand();

            var refreshPlayerDialogueCommand = command as RefreshPlayerDialogueCommand;
            if (refreshPlayerDialogueCommand != null)
            {
                refreshPlayerDialogueCommand.Execute(_scenarioController);
            }

            var setPlayerActionCommand = command as SetPlayerActionCommand;
            if (setPlayerActionCommand != null)
            {
                setPlayerActionCommand.Execute(_scenarioController);
            }

            var refreshCharacterResponseCommand = command as RefreshCharacterResponseCommand;
            if (refreshCharacterResponseCommand != null)
            {
                refreshCharacterResponseCommand.Execute(_scenarioController);
            }

            var toggleModulesCommand = command as ToggleModulesCommand;
            if (toggleModulesCommand != null)
            {
                toggleModulesCommand.Execute(_modulesController);
            }

            var commandResolver = new StateCommandResolver();
            commandResolver.HandleSequenceStates(command, this);
        }
    }
}
