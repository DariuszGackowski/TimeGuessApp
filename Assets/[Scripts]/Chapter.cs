using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    [System.Serializable]
    public class Chapter
    {
        public Sprite Icon;

        public string LocalizationTableName;

        public string NameID;

        public string DescriptionID;

        public List<Question> Questions = new List<Question>();

        [NonSerialized] public int ScoredPoints = 0;
        [NonSerialized] public int MaxScoredPoints = 0;

        public List<Question> ShuffleQuestions()
        {
            for (int i = 0; i < Questions.Count; i++)
            {
                Question question = Questions[i];
                int randomIndex = Random.Range(i, Questions.Count);
                Questions[i] = Questions[randomIndex];
                Questions[randomIndex] = question;
            }

            return Questions;
        }
    }
}