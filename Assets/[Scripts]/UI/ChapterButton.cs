using TMPro;
using UnityEngine;

namespace UI
{
    public class ChapterButton : MonoBehaviour
    {
        public int ChapterIndex = -1;

        public TextMeshProUGUI MaxScoredPointsText;

        public void OnClick()
        {
            UIManager.OnChapterSelect.Invoke(ChapterIndex);
        }

        public void SetMaxScoredPoints(int maxScoredPoints) 
        {
            MaxScoredPointsText.SetText(maxScoredPoints.ToString());
        }
    }
}
