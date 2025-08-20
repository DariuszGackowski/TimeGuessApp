using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

namespace UI
{
    public class ChapterButton : MonoBehaviour
    {
        public LocalizeStringEvent LocalizeStringEvent;
        private LocalizedString LocalizedString => LocalizeStringEvent.StringReference;

        public int ChapterIndex = -1;

        public void OnClick()
        {
            UIManager.OnChapterSelect.Invoke(ChapterIndex);
        }
        public void SetMaxScoredPoints(int maxScoredPoints)
        {
            UIManager.SetArgumentOnce(LocalizedString, maxScoredPoints.ToString());
        }
    }
}