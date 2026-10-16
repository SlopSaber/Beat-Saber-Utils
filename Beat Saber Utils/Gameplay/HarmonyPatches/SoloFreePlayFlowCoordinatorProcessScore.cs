using System.Linq;
using HarmonyLib;
using IPA.Utilities;

namespace BS_Utils.Gameplay.HarmonyPatches
{
    [HarmonyPatch(typeof(LevelCompletionResultsHelper))]
    [HarmonyPatch(nameof(LevelCompletionResultsHelper.ProcessScore), MethodType.Normal)]
    class SoloFreePlayFlowCoordinatorProcessScore
    {
        static bool Prefix()
        {
            return !ScoreSubmission.WasDisabled && !ScoreSubmission.disabled && !ScoreSubmission.prolongedDisable;
        }
    }

    [HarmonyPatch(typeof(PrepareLevelCompletionResults))]
    [HarmonyPatch(nameof(PrepareLevelCompletionResults.FillLevelCompletionResults), MethodType.Normal)]
    class ScoreSubmissionInsurance1
    {
        private static readonly FieldAccessor<GameplayCoreSceneSetupData, PracticeSettings>.Accessor PracticeSettingsAccessor = FieldAccessor<GameplayCoreSceneSetupData, PracticeSettings>.GetAccessor("practiceSettings");
#if BEAT_SABER_1453
        private static readonly FieldAccessor<LevelCompletionResultsBase, LevelScore>.Accessor CompletionScoreAccessor = FieldAccessor<LevelCompletionResultsBase, LevelScore>.GetAccessor("score");
#else
        private static readonly FieldAccessor<LevelCompletionResults, int>.Accessor MultipliedScoreAccessor = FieldAccessor<LevelCompletionResults, int>.GetAccessor("multipliedScore");
#endif
        static void Postfix(ref LevelCompletionResults __result, LevelCompletionResults.LevelEndStateType levelEndStateType)
        {
            if ((ScoreSubmission.WasDisabled || ScoreSubmission.disabled || ScoreSubmission.prolongedDisable) && levelEndStateType == LevelCompletionResults.LevelEndStateType.Cleared && Plugin.scenesTransitionSetupData != null)
            {
                var gameplayCoreSceneSetupData = Plugin.scenesTransitionSetupData._sceneSetupDataArray.OfType<GameplayCoreSceneSetupData>().FirstOrDefault();
                if (gameplayCoreSceneSetupData != null)
                {
                    PracticeSettingsAccessor(ref gameplayCoreSceneSetupData) = new PracticeSettings();
                }

                Plugin.scenesTransitionSetupData = null;
#if BEAT_SABER_1453
                LevelCompletionResultsBase results = __result;
                var score = results.score;
                CompletionScoreAccessor(ref results) = new LevelScore(-score.multiplied, score.modified, score.maxMultiplied, score.maxModified);
#else
                MultipliedScoreAccessor(ref __result) = -__result.multipliedScore;
#endif
            }
        }
    }

}
