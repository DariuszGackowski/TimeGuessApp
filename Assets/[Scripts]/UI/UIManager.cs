using Core;
using DG.Tweening;
using ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace UI
{
    public enum HintType
    {
        QuoteHint,
        BackgroundHint,
        HalfHint
    }
    public class UIManager : MonoBehaviour
    {
        public static UnityEvent<int> OnChapterSelect = new();
        public static UnityEvent<HintType> OnHintSelect = new();
        public static UnityEvent<HintType> OnHintCheck = new();
        public static UnityEvent<bool> OnAnswerSelect = new();
        public static UnityEvent OnBadAnswerSelect = new();
        public static UnityEvent<bool> OnSelectionBlocked = new();

        [Header("Main Setup")]
        public QuestionsListSO QuestionsData;
        public List<ChapterButton> ChapterButtons;
        public GameObject MainSegment;
        public GameObject ChapterSegment;
        public GameObject QuestionSegment;
        public GameObject SettingsSegment;
        public GameObject LoadingSegment;
        public GameObject LoadingCircle;

        [Header("Settings Setup")]
        public TMP_Dropdown LanguageDropdown;

        [Header("Chapter Setup")]
        public TextMeshProUGUI ChapterNameText;
        public TextMeshProUGUI ChapterDescriptionText;
        public TextMeshProUGUI ChapterMaxPointsText;
        public TextMeshProUGUI QuestionCountText;
        public Image ChapterImage;

        [Header("Question Setup")]
        public TextMeshProUGUI QuestionText;
        public TextMeshProUGUI QuestionValueText1;
        public TextMeshProUGUI QuestionValueText2;
        public TextMeshProUGUI QuestionValueText3;
        public TextMeshProUGUI ScoredPointsText;
        public AnswerButton Answer1;
        public AnswerButton Answer2;
        public AnswerButton Answer3;
        public AnswerButton Answer4;
        public HintButton HintButton1;
        public HintButton HintButton2;
        public HintButton HintButton3;
        public TextMeshProUGUI QuoteHintText1;
        public TextMeshProUGUI QuoteHintText2;
        public TextMeshProUGUI BackgroundHintText1;
        public TextMeshProUGUI BackgroundHintText2;
        public GameObject QuoteHintObject1;
        public GameObject QuoteHintObject2;
        public GameObject BackgroundHintObject1;
        public GameObject BackgroundHintObject2;
        public GameObject AnswerBarObject;
        public GameObject HintBarObject;
        public GameObject DescriptionBarObject;
        public GameObject ScorePopup;
        public Image QuestionImage;

        private List<Question> _currentQuestions;
        private int _currentQuestion;
        private float _scoredPoints;
        private Chapter _currentChapter;
        private List<Locale> _availableLocales;

        public void Awake()
        {
            Application.targetFrameRate = 60;

            ShowLoadingCircle(MainSegment);

            ShowMainSegment();
            InitializeLanguageDropdown();

            OnChapterSelect.AddListener(ChapterSegmentSetup);
            OnAnswerSelect.AddListener(CheckAnswer);
            OnHintSelect.AddListener(HintSetup);
            OnHintCheck.AddListener(CheckHint);
            OnBadAnswerSelect.AddListener(ShowCorrectAnswer);

            RefreshChapterButtons();
        }
        public static void SetLocalizedTextOnce(TMP_Text uiText, string table, string entry)
        {
            var localizedString = new LocalizedString(table, entry);

            localizedString.GetLocalizedStringAsync().Completed += handle =>
            {
                uiText.SetText(handle.Result);
            };
        }
        #region Main Segment UI
        //event in buttons
        private void OpenAllSegments()
        {
            MainSegment.SetActive(true);
            ChapterSegment.SetActive(true);
            QuestionSegment.SetActive(true);
            SettingsSegment.SetActive(true);
            LoadingSegment.SetActive(true);

            ScorePopup.SetActive(true);
        }
        private void HideAllSegments(GameObject segmentToActivate)
        {
            MainSegment.SetActive(false);
            ChapterSegment.SetActive(false);
            QuestionSegment.SetActive(false);
            SettingsSegment.SetActive(false);
            LoadingSegment.SetActive(false);

            ScorePopup.SetActive(false);

            segmentToActivate.SetActive(true);
        }
        private void ShowLoadingCircle(GameObject segmentToActivate)
        {
            OpenAllSegments();

            LoadingCircle.SetActive(true);
            float singleRotationDuration = 1f;
            int loops = Mathf.RoundToInt(0.5f / singleRotationDuration);

            LoadingCircle.transform.DORotate(
                new Vector3(0, 0, -360),
                singleRotationDuration,
                RotateMode.LocalAxisAdd)
                .SetLoops(loops, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    HideLoadingCircle();
                    HideAllSegments(segmentToActivate);
                });
        }
        private void HideLoadingCircle()
        {
            LoadingCircle.transform.DOKill();
            LoadingCircle.SetActive(false);
        }
        public void BackToMainMenu()
        {
            MainSegment.SetActive(true);
            ChapterSegment.SetActive(false);
            QuestionSegment.SetActive(false);
            SettingsSegment.SetActive(false);
        }
        private void ShowMainSegment()
        {
            if (MainSegment == null || ChapterSegment == null || QuestionSegment == null)
            {
                Debug.LogError("Segment is not assigned in the UIManager.");
                return;
            }
            MainSegment.SetActive(true);
            ChapterSegment.SetActive(false);
            QuestionSegment.SetActive(false);
            SettingsSegment.SetActive(false);
        }
        private void RefreshChapterButtons()
        {
            for (int i = 0; i < QuestionsData.Chapters.Count; i++)
            {
                ChapterButtons.Where(button => button.ChapterIndex == i).FirstOrDefault()?.SetMaxScoredPoints(QuestionsData.Chapters[i].MaxScoredPoints);
            }
        }
        #endregion
        #region Chapter Segment UI
        private void ShowChapterSegment()
        {
            MainSegment.SetActive(false);
            ChapterSegment.SetActive(true);
            QuestionSegment.SetActive(false);
            SettingsSegment.SetActive(false);
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

            ShowLoadingCircle(ChapterSegment);

            _currentChapter = chapter;

            SetLocalizedTextOnce(ChapterNameText, chapter.LocalizationTableName, chapter.NameID);
            SetLocalizedTextOnce(ChapterDescriptionText, chapter.LocalizationTableName, chapter.DescriptionID);

            ChapterMaxPointsText.SetText(chapter.MaxScoredPoints.ToString());
            QuestionCountText.SetText(chapter.Questions.Count.ToString());

            ChapterImage.sprite = chapter.Icon;

            ShowChapterSegment();
        }
        //event in buttons
        public void StartChapter()
        {
            QuestionSegmentSetup();

            ShowQuestionSegment();
        }
        #endregion
        #region Question Segment UI
        public void ShowQuestionSegment()
        {
            MainSegment.SetActive(false);
            ChapterSegment.SetActive(false);
            QuestionSegment.SetActive(true);
            SettingsSegment.SetActive(false);
        }
        private void CheckAnswer(bool isCorrect)
        {
            if (isCorrect)
            {
                float pointsToScore = _currentQuestions[_currentQuestion].Points;
                float hintValue = _currentQuestions[_currentQuestion].HintValue;

                _scoredPoints += pointsToScore - (HintButton1.IsUsed ? hintValue : 0f) - (HintButton2.IsUsed ? hintValue : 0f) - (HintButton3.IsUsed ? hintValue : 0f);
            }
            else
            {
                _scoredPoints += 0f;
            }

            SetNextQuestion();
        }
        private void ShowCorrectAnswer()
        {
            List<AnswerButton> allAnswers = new List<AnswerButton> { Answer1, Answer2, Answer3, Answer4 };
            foreach (var answer in allAnswers)
            {
                if (answer.IsCorrect)
                {
                    answer.AnswerImage.color = answer.CorrectColor;
                }
            }
            ShowAnswerBar();
        }
        private void QuestionSegmentSetup()
        {
            _currentQuestions = _currentChapter.ShuffleQuestions();
            _currentQuestion = 0;

            SetQuestion(_currentQuestions[0]);

            ShowAnswerBar();
        }
        private void SetQuestion(Question question)
        {
            string table = _currentChapter.LocalizationTableName;
            string questionNumber = (_currentQuestion + 1).ToString();

            QuestionValueText1.SetText(questionNumber);
            QuestionValueText2.SetText(questionNumber);
            QuestionValueText3.SetText(questionNumber);

            QuestionImage.sprite = question.Icon;
            SetLocalizedTextOnce(QuestionText, table, question.QuestionTextID);

            SetLocalizedTextOnce(QuoteHintText1, table, question.QuoteID);
            SetLocalizedTextOnce(QuoteHintText2, table, question.QuoteID);
            SetLocalizedTextOnce(BackgroundHintText1, table, question.HistoricalBackgroundID);
            SetLocalizedTextOnce(BackgroundHintText2, table, question.HistoricalBackgroundID);

            List<Answer> answers = question.ShuffleAnswers();

            Answer1.SetupAnswer(answers[0], table);
            Answer2.SetupAnswer(answers[1], table);
            Answer3.SetupAnswer(answers[2], table);
            Answer4.SetupAnswer(answers[3], table);

            HintButton1.ResetHintButton();
            HintButton2.ResetHintButton();
            HintButton3.ResetHintButton();

            QuoteHintObject1.SetActive(false);
            QuoteHintObject2.SetActive(false);
            BackgroundHintObject1.SetActive(false);
            BackgroundHintObject2.SetActive(false);
            ScorePopup.SetActive(false);

            OnSelectionBlocked.Invoke(false);
        }
        private void SetNextQuestion()
        {
            _currentQuestion++;
            if (_currentQuestion < _currentQuestions.Count)
            {
                SetQuestion(_currentQuestions[_currentQuestion]);
            }
            else
            {
                EndChapter();
            }
        }
        private void EndChapter()
        {
            _currentChapter.MaxScoredPoints = Mathf.Max(_currentChapter.MaxScoredPoints, (int)_scoredPoints);

            ScoredPointsText.SetText(_scoredPoints.ToString());
            ScorePopup.SetActive(true);

            StartCoroutine(DelayedPostScoreActions());
        }
        private IEnumerator DelayedPostScoreActions()
        {
            yield return new WaitForSeconds(2f);

            RefreshChapterButtons();
            BackToMainMenu();
        }
        //event in buttons
        public void ShowAnswerBar()
        {
            AnswerBarObject.SetActive(true);
            HintBarObject.SetActive(false);
            DescriptionBarObject.SetActive(false);
        }
        //event in buttons
        public void ShowHintsBar()
        {
            AnswerBarObject.SetActive(false);
            HintBarObject.SetActive(true);
            DescriptionBarObject.SetActive(false);
        }
        //event in buttons
        public void ShowDescriptionBar()
        {
            AnswerBarObject.SetActive(false);
            HintBarObject.SetActive(false);
            DescriptionBarObject.SetActive(true);
        }
        private void HintSetup(HintType hintType)
        {
            switch (hintType)
            {
                case HintType.QuoteHint:
                    QuoteHintObject1.SetActive(true);
                    break;
                case HintType.BackgroundHint:
                    BackgroundHintObject1.SetActive(true);
                    break;
                case HintType.HalfHint:
                    RejectAnswers();
                    ShowAnswerBar();
                    break;
                default:
                    Debug.LogWarning("This hint type isn't implemented");
                    break;
            }
        }
        public void RejectAnswers()
        {
            List<AnswerButton> allAnswers = new List<AnswerButton> { Answer1, Answer2, Answer3, Answer4 };

            List<AnswerButton> incorrectAnswers = allAnswers.FindAll(answer => !answer.IsCorrect);

            if (incorrectAnswers.Count < 3)
            {
                Debug.LogWarning("There are not enough enough answers to reject!!!");
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                int randomIndex = Random.Range(0, incorrectAnswers.Count);
                AnswerButton selected = incorrectAnswers[randomIndex];

                selected.RejectAnswer();
                incorrectAnswers.RemoveAt(randomIndex);
            }
        }
        public void CheckHint(HintType hintType)
        {
            switch (hintType)
            {
                case HintType.QuoteHint:
                    QuoteHintObject2.SetActive(true);
                    BackgroundHintObject2.SetActive(false);
                    break;
                case HintType.BackgroundHint:
                    QuoteHintObject2.SetActive(false);
                    BackgroundHintObject2.SetActive(true);
                    break;
                default:
                    Debug.LogWarning("This hint type isn't implemented");
                    break;
            }

            ShowDescriptionBar();
        }
        #endregion
        #region Settings Segment UI
        private void InitializeLanguageDropdown()
        {
            _availableLocales = LocalizationSettings.AvailableLocales.Locales;

            List<string> options = new List<string>();
            int selectedIndex = 0;

            for (int i = 0; i < _availableLocales.Count; i++)
            {
                var locale = _availableLocales[i];

                string languageName = locale.Identifier.CultureInfo?.NativeName ?? locale.Identifier.Code;
                languageName = languageName.FirstCharacterToUpper();
                options.Add(languageName);

                string savedCode = PlayerPrefs.GetString("LanguageCode", "");
                if (!string.IsNullOrEmpty(savedCode) && locale.Identifier.Code == savedCode)
                {
                    selectedIndex = i;
                }
            }

            LanguageDropdown.ClearOptions();
            LanguageDropdown.AddOptions(options);
            LanguageDropdown.value = selectedIndex;
            LanguageDropdown.onValueChanged.AddListener(OnLanguageSelected);

            SetLocaleByIndex(selectedIndex);
        }
        private void OnLanguageSelected(int index)
        {
            ShowLoadingCircle(SettingsSegment);

            SetLocaleByIndex(index);

            string code = _availableLocales[index].Identifier.Code;
            PlayerPrefs.SetString("LanguageCode", code);
            PlayerPrefs.Save();
        }
        private void SetLocaleByIndex(int index)
        {
            if (index >= 0 && index < _availableLocales.Count)
            {
                LocalizationSettings.SelectedLocale = _availableLocales[index];
            }
        }
        //event in buttons
        public void ResetChapters()
        {
            foreach (var chapter in QuestionsData.Chapters)
            {
                chapter.MaxScoredPoints = 0;
            }

            RefreshChapterButtons();
        }
        //event in buttons
        public void ShowSettingsSegment()
        {
            MainSegment.SetActive(false);
            ChapterSegment.SetActive(false);
            QuestionSegment.SetActive(false);
            SettingsSegment.SetActive(true);
        }
        #endregion
    }
}