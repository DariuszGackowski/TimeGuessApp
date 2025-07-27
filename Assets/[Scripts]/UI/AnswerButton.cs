using Core;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AnswerButton : MonoBehaviour
    {
        public bool IsCorrect;
        public string AnswerID;
        public TextMeshProUGUI AnswerText;

        public Button Button;
        public Image AnswerImage;
        public Color CorrectColor;
        public Color IncorrectColor;
        public void SetupAnswer(Answer answer, string table)
        {
            ResetButton();

            AnswerID = answer.AnswerID;
            IsCorrect = answer.IsCorrect;

            AnswerImage.color = Color.white;

            UIManager.SetLocalizedTextOnce(AnswerText, table, AnswerID);
        }
        public void OnClick()
        {
            StartCoroutine(HandleClick());
        }

        private IEnumerator HandleClick()
        {
            if (IsCorrect)
            {
                AnswerImage.color = CorrectColor;
            }
            else
            {
                AnswerImage.color = IncorrectColor;
            }
            yield return new WaitForSeconds(1f);

            UIManager.OnAnswerSelect.Invoke(IsCorrect);
        }
        public void RejectAnswer()
        {
            Button.interactable = false;
        }
        private void ResetButton()
        {
            AnswerImage.color = Color.white;
            IsCorrect = false;
            AnswerID = string.Empty;
            AnswerText.text = string.Empty;
            Button.interactable = true;
        }
    }
}