using Core;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Questions", menuName = "Scriptable Objects/Questions")]
    public class QuestionsListSO : ScriptableObject
    {
        public List<Chapter> Chapters = new List<Chapter>();
    }
}