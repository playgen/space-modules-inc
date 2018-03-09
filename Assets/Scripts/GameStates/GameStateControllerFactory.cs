using GameWork.Core.States.Tick;

public class GameStateControllerFactory
{
	private readonly ScenarioController _scenarioController;
	private readonly ModulesController _modulesController;

	public GameStateControllerFactory(ScenarioController scenarioController, ModulesController modulesController)
	{
		_scenarioController = scenarioController;
		_modulesController = modulesController;
	}

	public TickStateController Create()
	{
		var loadingState = CreateLoadingState(_scenarioController);
		var menuState = CreateMenuState();
		var settingsState = CreateSettingsState();
		var levelState = CreateLevelState(_scenarioController);
		var callState = CreateCallState();
		var gameState = CreateGameState(_scenarioController, _modulesController);
		var reviewState = CreateReviewState(_scenarioController);
		var scoreState = CreateScoreState(_scenarioController);
		var questionnaireState = CreateQuestionnaireState();

		var stateController = new TickStateController(
			loadingState,
			menuState,
			settingsState,
			levelState,
			callState,
			gameState,
			reviewState,
			scoreState,
			questionnaireState
		);

		stateController.Initialize();

		return stateController;
	}

	private LoadingState CreateLoadingState(ScenarioController scenarioController)
	{
		var input = new LoadingStateInput();
		var state = new LoadingState(input, scenarioController);

		var menuTransition = new EventTransition(MenuState.StateName);
		input.LoggedInEvent += menuTransition.ChangeState;
		input.OfflineClickedEvent += menuTransition.ChangeState;

		state.AddTransitions(menuTransition);

		return state;
	}

	private MenuState CreateMenuState()
	{
		var input = new MenuStateInput();
		var state = new MenuState(input);

		var settingsTransition = new EventTransition(SettingsState.StateName);
		input.SettingsClickedEvent += settingsTransition.ChangeState;

		var levelTransition = new EventTransition(LevelState.StateName);
		input.PlayClickedEvent += levelTransition.ChangeState;

		state.AddTransitions(settingsTransition, levelTransition);

		return state;
	}

	private SettingsState CreateSettingsState()
	{
		var input = new SettingsStateInput();
		var state = new SettingsState(input);

		var menuTransistion = new EventTransition(MenuState.StateName);
		input.BackClickedEvent += menuTransistion.ChangeState;

		state.AddTransitions(menuTransistion);

		return state;
	}

	private LevelState CreateLevelState(ScenarioController scenarioController)
	{
		var input = new LevelStateInput();
		var state = new LevelState(input, scenarioController);

		var callTransition = new EventTransition(CallState.StateName);
		state.CompletedEvent += callTransition.ChangeState;

		state.AddTransitions(callTransition);

		return state;
	}

	private CallState CreateCallState()
	{
		var input = new CallStateInput();
		var state = new CallState(input);

		// To Test questionnaire
		//var questionnaireTransition = new EventTransition(QuestionnaireState.StateName);
		//input.AnswerClickedEvent+= questionnaireTransition.ChangeState;

		//state.AddTransitions(questionnaireTransition);

		var gameTransition = new EventTransition(GameState.StateName);
		input.AnswerClickedEvent += gameTransition.ChangeState;

		state.AddTransitions(gameTransition);

		return state;
	}

	private GameState CreateGameState(ScenarioController scenarioController, ModulesController modulesController)
	{
		var input = new GameStateInput(scenarioController);
		var state = new GameState(input, scenarioController, modulesController);

		var reviewTransition = new EventTransition(ReviewState.StateName);
		input.HandleFinalStateEvent += reviewTransition.ChangeState;

		state.AddTransitions(reviewTransition);

		return state;
	}

	private ReviewState CreateReviewState(ScenarioController scenarioController)
	{
		var input = new ReviewStateInput(scenarioController);
		var state = new ReviewState(input, scenarioController);

		var scoreTransition = new EventTransition(ScoreState.StateName);
		input.NextClickedEvent += scoreTransition.ChangeState;

		state.AddTransitions(scoreTransition);

		return state;
	}

	private ScoreState CreateScoreState(ScenarioController scenarioController)
	{
		var input = new ScoreStateInput(scenarioController);
		var state = new ScoreState(input, scenarioController);

		var menuTransition = new FalseEventTransition(MenuState.StateName);
		var quitTransition = new QuitOnTrueEventTransition();

		state.NextEvent += menuTransition.ChangeState;
		state.NextEvent += quitTransition.Quit;

		var questionnaireTransition = new EventTransition(QuestionnaireState.StateName);
		state.InGameQuestionnaire += questionnaireTransition.ChangeState;

		state.AddTransitions(menuTransition, quitTransition, questionnaireTransition);

		return state;
	}

	private QuestionnaireState CreateQuestionnaireState()
	{
		var input = new QuestionnaireStateInput();
		var state = new QuestionnaireState(input);

		var menuTransition = new EventTransition(MenuState.StateName);
		input.FinishClickedEvent += menuTransition.ChangeState;

		state.AddTransitions(menuTransition);

		return state;
	}
}