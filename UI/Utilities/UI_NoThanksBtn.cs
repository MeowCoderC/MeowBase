using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class UI_NoThanksBtn : MonoBehaviour
{
    [SerializeField] private float delayTime = 2f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Button noThanksBtn;
    [SerializeField] private Text text;
    public Action onNoThanksClick;

    void Reset()
    {
        noThanksBtn = GetComponent<Button>();
        text = GetComponentInChildren<Text>();
    }

    void Awake()
    {
        noThanksBtn.onClick.AddListener(OnClickNoThanks);
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnClickNoThanks()
    {
        onNoThanksClick?.Invoke();
    }

    public void DelayShowNoThanksBtn()
    {
        noThanksBtn.enabled = false;
        text.color = new Color(1, 1, 1, 0); 
        StartCoroutine(ActiveNoThanksBtnCoroutine());
    }

    private IEnumerator ActiveNoThanksBtnCoroutine()
    {
        yield return new WaitForSecondsRealtime(delayTime);
        noThanksBtn.enabled = true;
        StartCoroutine(FadeInText());
    }

    private IEnumerator FadeInText()
    {
        float elapsedTime = 0f;
        Color startColor = text.color;
        Color endColor = new Color(1, 1, 1, 1);

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            text.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            yield return null;
        }

        text.color = endColor; 
    }
}
