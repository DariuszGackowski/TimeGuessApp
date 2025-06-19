using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace Core
{
    [System.Serializable]
    public class Chapter
    {
        public LocalizedString Name;

        public LocalizedString Description;

        public List<Question> Questions = new List<Question>();

        public int ScoredPoints = 0;

        public void GetScoredPoints()
        {
            foreach (var question in Questions)
            {
                ScoredPoints += (int)question.ScoredPoints;
            }
        }        
    }
}