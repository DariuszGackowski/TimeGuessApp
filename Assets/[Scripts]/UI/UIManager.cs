using Core;
using ScriptableObjects;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public static UnityEvent<int> OnChapterSelect = new();

        [Header("Main Setup")]
        public QuestionsListSO QuestionsData;

        public List<ChapterButton> ChapterButtons;

        public GameObject MainSegment;
        public GameObject ChapterSegment;

        [Header("Chapter Setup")]
        public TextMeshProUGUI ChapterNameText;
        public TextMeshProUGUI ChapterDescriptionText;
        public TextMeshProUGUI ChapterMaxPointsText;
        public TextMeshProUGUI QuestionCountText;
        public Image ChapterImage;

        public void Awake()
        {
            if (MainSegment == null || ChapterSegment == null)
            {
                Debug.LogError("Segment is not assigned in the UIManager.");
                return;
            }
            MainSegment.SetActive(true);
            ChapterSegment.SetActive(false);
        }
        public void Start()
        {
            OnChapterSelect.AddListener(ChapterSegmentSetup);

            RefreshChapterButtons();
        }
        public void RefreshChapterButtons()
        {
            for (int i = 0; i < QuestionsData.Chapters.Count; i++)
            {
                ChapterButtons.Where(button => button.ChapterIndex == i).FirstOrDefault()?.SetMaxScoredPoints(QuestionsData.Chapters[i].MaxScoredPoints);
            }

        }
        public void ShowMainSegment()
        {
            if (MainSegment == null || ChapterSegment == null)
            {
                Debug.LogError("Segment is not assigned in the UIManager.");
            }

            MainSegment.SetActive(true);
            ChapterSegment.SetActive(false);
        }
        public void ShowChapterSegment()
        {
            if (MainSegment == null || ChapterSegment == null)
            {
                Debug.LogError("Segment is not assigned in the UIManager.");
            }
            MainSegment.SetActive(false);
            ChapterSegment.SetActive(true);
        }
        private void ChapterSegmentSetup(int chapterIndex)
        {
            if (QuestionsData == null || QuestionsData.Chapters.Count == 0)
                return;

            Chapter chapter = QuestionsData.Chapters[chapterIndex];

            if (chapter == null)
            {
                Debug.LogError($"Chapter at index {chapterIndex} is null.");
                return;
            }

            SetLocalizedTextOnce(ChapterNameText, chapter.LocalizationTableName, chapter.NameID);
            SetLocalizedTextOnce(ChapterDescriptionText, chapter.LocalizationTableName, chapter.DescriptionID);

            ChapterMaxPointsText.SetText(chapter.MaxScoredPoints.ToString());
            QuestionCountText.SetText(chapter.Questions.Count.ToString());

            ChapterImage.sprite = chapter.Icon;

            ShowChapterSegment();
        }
        private void SetLocalizedTextOnce(TMP_Text uiText, string table, string entry)
        {
            var localizedString = new LocalizedString(table, entry);

            localizedString.GetLocalizedStringAsync().Completed += handle =>
            {
                uiText.SetText(handle.Result);
            };
        }
    }
}