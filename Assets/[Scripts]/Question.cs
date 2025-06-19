using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Core
{
    [System.Serializable]
    public class Question
    {
        public Sprite Icon;

        public LocalizedString Quote;
        public LocalizedString HistoricalBackground;

        public List<Answer> Answers = new List<Answer>();

        public bool HintQuoteUsed = false;
        public bool HintBackgroundUsed = false;
        public bool HintFiftyFiftyUsed = false;

        public float Points = 1f;
        public float ScoredPoints = 0f;
        public float HintValue = 0.25f;

        public void ShuffleAnswers()
        {
            for (int i = 0; i < Answers.Count; i++)
            {
                Answer answer = Answers[i];
                int randomIndex = Random.Range(i, Answers.Count);
                Answers[i] = Answers[randomIndex];
                Answers[randomIndex] = answer;
            }
        }
        public void UseHintQuote()
        {
            if (!HintQuoteUsed)
            {
                ScoredPoints -= HintValue;
                HintQuoteUsed = true;
            }
        }
        public void UseHintBackground()
        {
            if (!HintBackgroundUsed)
            {
                ScoredPoints -= HintValue;
                HintBackgroundUsed = true;
            }
        }
        public void UseHintFiftyFifty()
        {
            if (!HintFiftyFiftyUsed)
            {
                ScoredPoints -= HintValue;
                HintFiftyFiftyUsed = true;
            }
        }
        public void ResetHints()
        {
            HintQuoteUsed = false;
            HintBackgroundUsed = false;
            HintFiftyFiftyUsed = false;
        }
        public void ResetScoredPoints()
        {
            ScoredPoints = Points;
            ResetHints();
        }
    }
}
