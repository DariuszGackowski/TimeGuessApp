using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    [System.Serializable]
    public class Question
    {
        public Sprite Icon;

        public string QuestionTextID;
        public string QuoteID;
        public string HistoricalBackgroundID;

        public List<Answer> Answers = new List<Answer>();

        [NonSerialized] public bool HintQuoteUsed = false;
        [NonSerialized] public bool HintBackgroundUsed = false;
        [NonSerialized] public bool HintFiftyFiftyUsed = false;

        [NonSerialized] public float Points = 1f;
        [NonSerialized] public float ScoredPoints = 0f;
        [NonSerialized] public float HintValue = 0.25f;

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
