﻿using System;
using System.Collections.Generic;
using System.Linq;

using GameWork.Core.States.Tick.Input;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Unity;
using UnityEngine.UI;
using PlayGen.Unity.Utilities.BestFit;
using RolePlayCharacter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuestionnaireStateInput : TickStateInput
{
	public event Action FinishClickedEvent;

	public class TempQuestions
	{
		public int Number;
		public string Question;
		public string[] Answers;
	}

	public class TempAnswers
	{
		public string Question;
		public string Answer;
	}
	
	private List<TempQuestions> _questions = new List<TempQuestions>
	{
		new TempQuestions
		{
			Number = 1,
			Question = "Hoe mentaal belastend waren de opdrachten in de game?",
			Answers = new[] { "1. Zeer laag belastend", "2", "3", "4", "5", "6",  "7. Zeer hoog belastend" }
		},
		new TempQuestions
		{
			Number = 2,
			Question = "Hoe fysiek belastend waren de opdrachten in de game?",
			Answers = new[] { "1. Zeer laag belastend", "2", "3", "4", "5", "6",  "7. Zeer hoog belastend" }
		},
		new TempQuestions
		{
			Number = 3,
			Question = "Hoe gehaast was het tempo van de opdrachten in de game?",
			Answers = new[] { "1. Zeer laag belastend", "2", "3", "4", "5", "6",  "7. Zeer hoog belastend" }
		},
		new TempQuestions
		{
			Number = 4,
			Question = "Hoe succesvol was je in het doen van de opdrachten in de game?",
			Answers = new[] { "1. Zeer laag belastend", "2", "3", "4", "5", "6",  "7. Zeer hoog belastend" }
		},
		new TempQuestions
		{
			Number = 5,
			Question = "Hoe hard moest je inspannen om de opdrachten in de game succesvol te kunnen doen?",
			Answers = new[] { "1. Zeer laag belastend", "2", "3", "4", "5", "6",  "7. Zeer hoog belastend" }
		},
		new TempQuestions
		{
			Number = 6,
			Question = "Hoeveel negatieve gevoelens had je tijdens de opdrachten? (negatieve gevoelens zijn bijvoorbeeld: onzeker, ontmoedigd, geïrriteerd, gestrest, geërgerd)",
			Answers = new[] { "1. Zeer laag belastend", "2", "3", "4", "5", "6",  "7. Zeer hoog belastend" }
		}
	};

	private GameObject _characterPrefab;
	private GameObject _characterPanel;
	private GameObject _characterObject;

	private GameObject _listChoicePrefab;
	private GameObject _dialoguePanel;

	private Text _questionTrackText;
	private Text _questionText;

	private List<TempAnswers> _tempAnswers = new List<TempAnswers>();


	protected override void OnInitialize()
	{
		_characterPrefab = Resources.Load("Prefabs/Characters/Generic") as GameObject;
		_characterPanel = GameObjectUtilities.FindGameObject("QuestionnaireContainer/QuestionnairePanelContainer/CharacterPanel");

		_listChoicePrefab = Resources.Load("Prefabs/ListChoiceGroup") as GameObject;
		_dialoguePanel = GameObjectUtilities.FindGameObject("QuestionnaireContainer/QuestionnairePanelContainer/QuestionnaireUI/QuestionPanel/AnswerPanel");

		_questionText = GameObjectUtilities.FindGameObject("QuestionnaireContainer/QuestionnairePanelContainer/QuestionnaireUI/QuestionPanel/QuestionHolder/Question").GetComponent<Text>();
		_questionTrackText = GameObjectUtilities.FindGameObject("QuestionnaireContainer/QuestionnairePanelContainer/QuestionnaireUI/QuestionTrackPanel/QuestionHolder/QuestionTrack").GetComponent<Text>();
	}

	protected override void OnEnter()
	{
		Tracker.T.accessible.Accessed("Questionnaire", AccessibleTracker.Accessible.Screen);

		GameObjectUtilities.FindGameObject("QuestionnaireContainer/QuestionnairePanelContainer").SetActive(true);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(true);

		ShowCharacter();
		NextQuestion();
	}

	protected override void OnExit()
	{
		UnityEngine.Object.Destroy(_characterObject);
		_tempAnswers.Clear();

		GameObjectUtilities.FindGameObject("QuestionnaireContainer/QuestionnairePanelContainer").SetActive(false);
		GameObjectUtilities.FindGameObject("BackgroundContainer/GameBackgroundImage").SetActive(false);
	}

	public void ShowCharacter()
	{
		_characterObject = UnityEngine.Object.Instantiate(_characterPrefab);

		_characterObject.transform.SetParent(_characterPanel.transform, false);
		_characterObject.GetComponent<RectTransform>().offsetMax = Vector2.one;
		_characterObject.GetComponent<RectTransform>().offsetMin = Vector2.zero;
	}

	public void UpdateQuestion(TempQuestions question)
	{
		// Clear current dialogue panel container
		if (_dialoguePanel.GetComponentInChildren<ScrollRect>())
		{
			foreach (var child in _dialoguePanel.GetComponentInChildren<ScrollRect>().content)
			{
				var childObj = child as Transform;
				if (childObj != null) UnityEngine.Object.Destroy(childObj.gameObject);
			}
		}
		foreach (var child in _dialoguePanel.transform)
		{
			var childGameObject = child as Transform;
			if (childGameObject != null) UnityEngine.Object.Destroy(childGameObject.gameObject);
		}

		// TODO Translate
		_questionTrackText.text = "Vraag " + question.Number;
		_questionText.text = question.Question;

		// Generate Answers
		var dialogueObject = UnityEngine.Object.Instantiate(_listChoicePrefab);
		var scrollRect = dialogueObject.GetComponent<ScrollRect>();
		var choiceItemPrefab = Resources.Load("Prefabs/DialogueItemScroll") as GameObject;
		var contentTotalHeight = 0f;
		dialogueObject.transform.SetParent(_dialoguePanel.transform, false);
		for (var i = 0; i < question.Answers.Length; i++)
		{
			var choiceItem = UnityEngine.Object.Instantiate(choiceItemPrefab);
			choiceItem.transform.GetChild(0).GetComponent<Text>().text = question.Answers[i];

			var offset = i * choiceItem.GetComponent<RectTransform>().rect.height;
			contentTotalHeight += choiceItem.GetComponent<RectTransform>().rect.height;

			choiceItem.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -offset);

			var answer = new TempAnswers
			{
				Question = question.Question,
				Answer = question.Answers[i]
			};
			choiceItem.GetComponent<Button>().onClick.AddListener(delegate
			{
				OnDialogueOptionClick(answer);
			});
			choiceItem.transform.SetParent(scrollRect.content, false);
		}
		var rectContent = scrollRect.content;
		rectContent.sizeDelta = new Vector2(0, contentTotalHeight);

		ResizeOptions(dialogueObject);
	}

	public void ResizeOptions(GameObject dialogueObject)
	{
		int smallestFontSize = 0;
		foreach (var obj in dialogueObject.GetComponent<ScrollRect>().content)
		{
			var textObj = obj as Transform;
			if (textObj != null)
			{
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
		}
		foreach (var obj in dialogueObject.GetComponent<ScrollRect>().content)
		{
			var textObj = obj as Transform;
			if (textObj != null)
			{
				var text = textObj.GetComponentInChildren<Text>();
				if (!text)
				{
					continue;
				}
				text.fontSize = smallestFontSize;
			}
		}
	}

	private void OnDialogueOptionClick(TempAnswers answer)
	{
		_tempAnswers.Add(answer);
		NextQuestion();
	}

	private void NextQuestion()
	{
		if (_questions.Count > 0)
		{
			UpdateQuestion(_questions.First());
			_questions.RemoveAt(0);
		}
		else
		{
			// continue to next state
			LogAnswers();
			SceneManager.LoadScene(0);
			//FinishClickedEvent();
		}
	}

	private void LogAnswers()
	{
		var str = "";
		foreach (var answer in _tempAnswers)
		{
			str += answer.Question + " " + answer.Answer + "\n";
		}
		Debug.Log(str);
	}
}
