using System;
using System.Collections.Generic;
using UnityEngine;

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

        public void GetScoredPoints()
        {
            ScoredPoints = 0;

            foreach (var question in Questions)
            {
                ScoredPoints += (int)question.ScoredPoints;
            }

            MaxScoredPoints = Mathf.Max(MaxScoredPoints, ScoredPoints);
        }
    }
}