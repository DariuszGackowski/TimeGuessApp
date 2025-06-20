using System;
using System.Collections.Generic;

namespace Core
{
    [System.Serializable]
    public class Chapter
    {
        public string LocalizationTableName;

        public string NameID;

        public string DescriptionID;

        public List<Question> Questions = new List<Question>();

        [NonSerialized] public int ScoredPoints = 0;

        public void GetScoredPoints()
        {
            foreach (var question in Questions)
            {
                ScoredPoints += (int)question.ScoredPoints;
            }
        }
    }
}