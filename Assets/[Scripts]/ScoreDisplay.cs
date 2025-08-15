using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    public LocalizedString scoreString;

    void Start()
    {
        if (scoreString.TableReference != null)
        {
            // Przekazujemy obiekt anonimowy, który zawiera pole "number"
            var op = scoreString.GetLocalizedStringAsync(new { number = 25 });

            op.Completed += (opResult) =>
            {
                Debug.Log(opResult.Result);
            };
        }
    }
}