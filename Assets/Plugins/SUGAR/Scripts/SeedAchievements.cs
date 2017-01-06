#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;

using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using UnityEditor;
using UnityEngine;

namespace PlayGen.SUGAR.Unity
{
	public static class SeedAchievements
	{
		[MenuItem("Tools/SUGAR/Seed Game")]
		public static void SeedAchivements()
		{
			AdminLogIn window = ScriptableObject.CreateInstance<AdminLogIn>();
			window.title = "Seed Game";
			window.Show();
		}

        public static void LogInUser(string username, string password)
        {
            var unityManager = GameObject.FindObjectsOfType(typeof(SUGARUnityManager)).FirstOrDefault() as SUGARUnityManager;
            if (unityManager == null)
            {
                return;
            }
            SUGARManager.Client = new SUGARClient(unityManager.baseAddress);
            var response = LoginAdmin(username, password);
            if (response != null)
            {
                Debug.Log("Admin Login SUCCESS");
                var game = SUGARManager.Client.Game.Get(unityManager.gameToken).FirstOrDefault();
                if (game != null)
                {
                    Debug.Log("Game Found");
                    unityManager.gameId = game.Id;
                    SUGARManager.GameId = game.Id;
                }
                else
                {
                    Debug.Log("Creating Game");
                    var gameResponse = SUGARManager.Client.Game.Create(new GameRequest()
                    {
                        Name = unityManager.gameToken
                    });
                    if (gameResponse != null)
                    {
                        unityManager.gameId = gameResponse.Id;
                        SUGARManager.GameId = gameResponse.Id;
                    }
                    else
                    {
                        Debug.LogError("Unable to create game");
                        return;
                    }

                }
                CreateAchievements();
                CreateLeaderboards();
                SUGARManager.Client.Session.Logout();
            }
        }

        private static void CreateAchievements()
		{
			var achievementClient = SUGARManager.Client.Achievement;
			var gameId = SUGARManager.GameId;

            /*achievementClient.Create(new EvaluationCreateRequest
			{
				Name = "",
				Description = "",
				ActorType = ActorType.,
				GameId = gameId,
				Token = "",
				EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>
				{
				new EvaluationCriteriaCreateRequest
					{
						EvaluationDataKey = "",
						ComparisonType = ComparisonType.,
						CriteriaQueryType = CriteriaQueryType.,
						DataType = EvaluationDataType.,
						Scope = CriteriaScope.,
						Value = ""
					}
				}
			});*/

            var mod2 = 5;
            achievementClient.Create(new EvaluationCreateRequest()
            {
                Name = String.Format("Get {0} stars!", mod2),
                Description = String.Format("Accumulate at least {0} stars", mod2),
                ActorType = ActorType.User,
                GameId = gameId,
                Token = String.Format("get_{0}_stars", mod2),
                EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>()
            {
                new EvaluationCriteriaCreateRequest()
                {
                    EvaluationDataKey = "stars",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Sum,
                    EvaluationDataType = EvaluationDataType.Long,
                    Scope = CriteriaScope.Actor,
                    Value = String.Format("{0}", mod2)
                }
            }
            });

            var mod3 = 3;
            achievementClient.Create(new EvaluationCreateRequest()
            {
                Name = String.Format("Get {0} stars in one game!", mod3),
                Description = String.Format("Get {0} stars in a single game", mod3),
                ActorType = ActorType.User,
                GameId = gameId,
                Token = String.Format("get_{0}_stars_one_game", mod3),
                EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>()
            {
                new EvaluationCriteriaCreateRequest()
                {
                    EvaluationDataKey = "stars",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Any,
                    EvaluationDataType = EvaluationDataType.Long,
                    Scope = CriteriaScope.Actor,
                    Value = String.Format("{0}", mod3)
                }
            }
            });

            var mod4 = 2;
            achievementClient.Create(new EvaluationCreateRequest()
            {
                Name = String.Format("Play {0} games!", mod4),
                Description = String.Format("Play at least {0} games", mod4),
                ActorType = ActorType.User,
                GameId = gameId,
                Token = String.Format("play_{0}_games", mod4),
                EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>()
            {
                new EvaluationCriteriaCreateRequest()
                {
                    EvaluationDataKey = "plays",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Sum,
                    EvaluationDataType = EvaluationDataType.Long,
                    Scope = CriteriaScope.Actor,
                    Value = String.Format("{0}", mod4)
                }
            }
            });

            var mod5 = 2;
            achievementClient.Create(new EvaluationCreateRequest()
            {
                Name = String.Format("Accumulate {0} points!", mod5),
                Description = String.Format("Accumulate {0} points", mod5),
                ActorType = ActorType.User,
                GameId = gameId,
                Token = String.Format("score_{0}_point", mod5),
                EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>()
            {
                new EvaluationCriteriaCreateRequest()
                {
                    EvaluationDataKey = "score",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Sum,
                    EvaluationDataType = EvaluationDataType.Long,
                    Scope = CriteriaScope.Actor,
                    Value = String.Format("{0}", mod5)
                }
            }
            });
        }

		private static void CreateLeaderboards()
		{
			var leaderboardClient = SUGARManager.Client.Leaderboard;
			var gameId = SUGARManager.GameId;

			/*leaderboardClient.Create(new LeaderboardRequest
			{
				Token = "",
				GameId = gameId,
				Name = "",
				EvaluationDataKey = "",
				ActorType = ActorType.,
				EvaluationDataType = EvaluationDataType.,
				CriteriaScope = CriteriaScope.,
				LeaderboardType = LeaderboardType.
			});*/
		}

		private static AccountResponse LoginAdmin(string username, string password)
		{
			try
			{
				return SUGARManager.Client.Session.Login(new AccountRequest()
				{
					Name = username,
					Password = password,
					SourceToken = "SUGAR"
				});
			}
			catch (Exception ex)
			{
				Debug.Log("Error Logging in Admin");
				Debug.Log(ex.Message);
				return null;
			}
		}
	}

	public class AdminLogIn : EditorWindow
	{
		string username;
		string password;

		void OnGUI()
		{
			username = EditorGUILayout.TextField("Username", username, EditorStyles.textField);
			password = EditorGUILayout.PasswordField("Password", password);
			if (GUILayout.Button("Sign-in"))
			{
				SeedAchievements.LogInUser(username, password);
			}
		}
	}
}
#endif
