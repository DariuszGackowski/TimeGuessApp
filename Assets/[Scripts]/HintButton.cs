using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HintButton : MonoBehaviour
    {
        public HintType HintType;

        public bool IsUsed;

        public Image HintImage;
        public Sprite UsedHintSprite;
        public Sprite UnusedHintSprite;

        private void ShowHint()
        {
            HintImage.sprite = UsedHintSprite;

            UIManager.OnHintSelect.Invoke(HintType);
        }
        public void ResetHintButton() 
        {
            HintImage.sprite = UnusedHintSprite;

            IsUsed = false;
        }
        public void OnClick() 
        {
            if (IsUsed) return;

            ShowHint();

            IsUsed = true;
        }
    }
}
