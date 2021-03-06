﻿using GameWork.Core.States.Tick;

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
		var menuState = CreateMenuState(_scenarioController);
		var settingsState = CreateSettingsState(_scenarioController);
		var levelState = CreateLevelState(_scenarioController);
		var callState = CreateCallState();
		var gameState = CreateGameState(_scenarioController, _modulesController);
		var reviewState = CreateReviewState(_scenarioController);
		var scoreState = CreateScoreState(_scenarioController);
		var questionnaireState = CreateQuestionnaireState(_scenarioController);
		TrackerEventSender.SetScenarioController(_scenarioController);

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
		var state = new LoadingState(input);

		var menuTransition = new EventTransition(MenuState.StateName);
		input.LoggedInEvent += scenarioController.Initialize;
		input.LoggedInEvent += menuTransition.ChangeState;

		state.AddTransitions(menuTransition);

		return state;
	}

	private MenuState CreateMenuState(ScenarioController scenarioController)
	{
		var input = new MenuStateInput(scenarioController);
		var state = new MenuState(input);

		var settingsTransition = new EventTransition(SettingsState.StateName);
		input.SettingsClickedEvent += settingsTransition.ChangeState;

		// TODO uncomment the lines for the next state wanted when play is pressed
		
		// begin level select
		// var nextStateTransition = new EventTransition(LevelState.StateName);
		// end level select
		
		// begin straight to gema
		var nextStateTransition = new EventTransition(CallState.StateName);
		input.PlayClickedEvent += _scenarioController.NextLevel;
		// end straight to game
		input.PlayClickedEvent += nextStateTransition.ChangeState;
		state.AddTransitions(settingsTransition, nextStateTransition);

		return state;
	}

	private SettingsState CreateSettingsState(ScenarioController scenarioController)
	{
		var input = new SettingsStateInput(scenarioController);
		var state = new SettingsState(input);

		var menuTransistion = new EventTransition(MenuState.StateName);
		input.BackClickedEvent += menuTransistion.ChangeState;

		state.AddTransitions(menuTransistion);

		return state;
	}

	private LevelState CreateLevelState(ScenarioController scenarioController)
	{
		var input = new LevelStateInput(scenarioController);
		var state = new LevelState(input, scenarioController);

		var menuTransition = new EventTransition(MenuState.StateName);
		var callTransition = new EventTransition(CallState.StateName);

		input.BackClickedEvent += menuTransition.ChangeState;

		input.LoadLevelEvent += _scenarioController.NextLevel;
		input.LoadLevelEvent += callTransition.ChangeState;

		state.AddTransitions(callTransition, menuTransition);

		return state;
	}

	private CallState CreateCallState()
	{
		var input = new CallStateInput();
		var state = new CallState(input);

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

		var menuTransition = new EventTransition(MenuState.StateName);
		input.NextEvent += menuTransition.ChangeState;

		var questionnaireTransition = new EventTransition(QuestionnaireState.StateName);
		input.InGameQuestionnaire += scenarioController.NextQuestionnaire;
		input.InGameQuestionnaire += questionnaireTransition.ChangeState;

		state.AddTransitions(menuTransition, questionnaireTransition);

		return state;
	}

	private QuestionnaireState CreateQuestionnaireState(ScenarioController scenarioController)
	{
		var input = new QuestionnaireStateInput(scenarioController);
		var state = new QuestionnaireState(input, scenarioController);

		var menuTransition = new EventTransition(MenuState.StateName);
		input.FinishClickedEvent += menuTransition.ChangeState;

		state.AddTransitions(menuTransition);

		return state;
	}
}