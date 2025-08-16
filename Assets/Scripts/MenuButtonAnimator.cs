// MenuButtonAnimator.cs (Final Version)
// This safe, self-contained script handles all button visual effects.

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

[RequireComponent(typeof(Image), typeof(Button))]
public class MenuButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonType { Normal, StartRun, SurvivorCamp }

    [Header("--- Configuration ---")]
    public ButtonType type = ButtonType.Normal;

    [Header("--- Color Settings ---")]
    public Color normalColor = new Color32(15, 23, 42, 204);
    public Color hoverColor = new Color32(30, 41, 59, 230);
    public Color downColor = new Color32(248, 113, 113, 255);

    [Header("--- Border Color Settings ---")]
    public Color normalBorderColor = new Color32(51, 65, 85, 230);
    public Color startRunHoverBorder = new Color32(248, 113, 113, 255);
    public Color campHoverBorder = new Color32(103, 232, 249, 255);

    private Image buttonImage;
    private Outline border;
    private TextMeshProUGUI mainText;
    private GameObject glitchTextObject;
    private Coroutine glitchCoroutine;
    private bool isPointerOver = false;

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        mainText = GetComponentInChildren<TextMeshProUGUI>();

        border = GetComponent<Outline>();
        if (border == null) { border = gameObject.AddComponent<Outline>(); }
        border.effectDistance = new Vector2(2, -2);

        buttonImage.color = normalColor;
        border.effectColor = normalBorderColor;

        if (type == ButtonType.StartRun)
        {
            CreateGlitchText();
        }
    }

    private void CreateGlitchText()
    {
        glitchTextObject = new GameObject("GlitchText");
        glitchTextObject.transform.SetParent(transform, false);

        RectTransform glitchRT = glitchTextObject.AddComponent<RectTransform>();
        glitchRT.anchorMin = Vector2.zero;
        glitchRT.anchorMax = Vector2.one;
        glitchRT.sizeDelta = Vector2.zero;
        glitchTextObject.AddComponent<Mask>().showMaskGraphic = false;
        Image maskImage = glitchTextObject.AddComponent<Image>();
        maskImage.raycastTarget = false;
        maskImage.color = new Color(0, 0, 0, 0);

        GameObject textChild = new GameObject("GlitchTextChild");
        textChild.transform.SetParent(glitchRT, false);

        TextMeshProUGUI glitchText = textChild.AddComponent<TextMeshProUGUI>();
        glitchText.font = mainText.font;
        glitchText.fontSize = mainText.fontSize;
        glitchText.alignment = mainText.alignment;
        glitchText.text = mainText.text;
        glitchText.color = new Color32(103, 232, 249, 255);
        glitchText.raycastTarget = false;

        RectTransform textRT = textChild.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.sizeDelta = new Vector2(4, 4);

        glitchTextObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        buttonImage.color = hoverColor;
        transform.localScale = new Vector3(1.02f, 1.02f, 1.02f);

        switch (type)
        {
            case ButtonType.StartRun:
                border.effectColor = startRunHoverBorder;
                if (glitchCoroutine == null)
                {
                    glitchCoroutine = StartCoroutine(GlitchEffect());
                }
                break;
            case ButtonType.SurvivorCamp:
                border.effectColor = campHoverBorder;
                break;
            default:
                border.effectColor = startRunHoverBorder;
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        buttonImage.color = normalColor;
        border.effectColor = normalBorderColor;
        transform.localScale = Vector3.one;

        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            glitchCoroutine = null;
            if (glitchTextObject != null) glitchTextObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        buttonImage.color = downColor;
        transform.localScale = new Vector3(0.98f, 0.98f, 0.98f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isPointerOver)
        {
            OnPointerEnter(eventData);
        }
        else
        {
            OnPointerExit(eventData);
        }
    }

    private IEnumerator GlitchEffect()
    {
        if (glitchTextObject == null) yield break;

        glitchTextObject.SetActive(true);
        RectTransform rt = glitchTextObject.GetComponent<RectTransform>();

        while (rt != null)
        {
            float yPos = Random.Range(-rt.rect.height, 0);
            rt.anchoredPosition = new Vector2(0, yPos);
            yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        }
    }

    void OnDisable()
    {
        if (glitchCoroutine != null)
        {
            StopCoroutine(glitchCoroutine);
            glitchCoroutine = null;
        }
    }
}
