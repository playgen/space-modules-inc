using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Client;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using SUGAR.Unity;
using UnityEditor;
using UnityEngine;

public static class SeedAchievements
{

    [MenuItem("Tools/Seed Achievements")]
    public static void SeedAchivements()
    {
        SUGARManager.Client = new SUGARClient("http://localhost:62312/");
        SUGARManager.GameId = 0;
        var response = LoginAdmin();
        if (response != null)
        {
            Debug.Log("Admin Login SUCCESS");
            CreateAchievements();
        }


    }

    private static void CreateAchievements()
    {
        var achievementClient = SUGARManager.Client.Achievement;
        var gameId = SUGARManager.GameId;

        var mod1 = 1;
        achievementClient.Create(new EvaluationCreateRequest()
        {
            Name = String.Format("Score over {0} point!", mod1),
            Description = String.Format("Score over {0} point in a single game", mod1),
            ActorType = ActorType.User,
            GameId = gameId,
            Token = String.Format("score_{0}_point_one_game", mod1),
            EvaluationCriterias = new List<EvaluationCriteriaCreateRequest>()
            {
                new EvaluationCriteriaCreateRequest()
                {
                    Key = "score",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Any,
                    DataType = SaveDataType.Long,
                    Scope = CriteriaScope.Actor,
                    Value = String.Format("{0}", mod1)
                }
            }
        });

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
                    Key = "stars",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Sum,
                    DataType = SaveDataType.Long,
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
                    Key = "stars",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Any,
                    DataType = SaveDataType.Long,
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
                    Key = "plays",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Sum,
                    DataType = SaveDataType.Long,
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
                    Key = "score",
                    ComparisonType = ComparisonType.GreaterOrEqual,
                    CriteriaQueryType = CriteriaQueryType.Sum,
                    DataType = SaveDataType.Long,
                    Scope = CriteriaScope.Actor,
                    Value = String.Format("{0}", mod5)
                }
            }
        });

    }

    private static AccountResponse LoginAdmin()
    {
        try
        {
            return SUGARManager.Client.Account.Login(new AccountRequest()
            {
                Name = "admin",
                Password = "admin",
                AutoLogin = false,
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
