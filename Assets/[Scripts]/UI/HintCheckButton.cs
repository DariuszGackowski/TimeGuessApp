using UnityEngine;

namespace UI
{
    public class HintCheckButton : MonoBehaviour
    {
        public HintType HintType;

        public void OnClick()
        {
            UIManager.OnHintCheck.Invoke(HintType);
        }
    }
}
