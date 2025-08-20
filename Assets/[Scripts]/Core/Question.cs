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

        [NonSerialized] public float Points = 1f;
        [NonSerialized] public float ScoredPoints = 0f;
        [NonSerialized] public float HintValue = 0.25f;

        public List<Answer> ShuffleAnswers()
        {
            for (int i = 0; i < Answers.Count; i++)
            {
                Answer answer = Answers[i];
                int randomIndex = Random.Range(i, Answers.Count);
                Answers[i] = Answers[randomIndex];
                Answers[randomIndex] = answer;
            }

            return Answers;
        }
    }
}
