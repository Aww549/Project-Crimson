// MainMenuStyler.cs (Updated)
// This script procedurally generates the main menu UI.
// It now automatically links the canvas to the logic script.

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MainMenuStyler : MonoBehaviour
{
    [Header("--- Core References ---")]
    [Tooltip("The main canvas to build the UI under.")]
    public Canvas parentCanvas;
    [Tooltip("A prefab for the menu buttons. Must have MenuButtonAnimator attached.")]
    public GameObject buttonPrefab;
    [Tooltip("The font for the main title (e.g., Bebas Neue SDF).")]
    public TMP_FontAsset titleFont;

    // Optional decorative sprites
    [Tooltip("(Optional) The background image for the menu.")]
    public Sprite backgroundSprite;
    [Tooltip("(Optional) The bloody handprint sprite.")]
    public Sprite handprintSprite;

    private MainMenu mainMenuLogic;

    void Start()
    {
        mainMenuLogic = GetComponent<MainMenu>();
        if (mainMenuLogic == null)
        {
            Debug.LogError("MainMenuStyler requires the MainMenu script on the same GameObject!");
            return;
        }

        // **THE FIX IS HERE**
        // The styler now tells the logic script which canvas to disable.
        if (parentCanvas != null)
        {
            mainMenuLogic.mainMenuCanvasObject = parentCanvas.gameObject;
        }

        CreateBackground();
        CreateTitle();
        CreateButtons();
        CreateDecorations();
    }

    // ... (The rest of the script is identical, no need to copy it again)
    void CreateBackground()
    {
        if (backgroundSprite == null) return;
        GameObject bgObject = new GameObject("BackgroundImage");
        bgObject.transform.SetParent(parentCanvas.transform, false);
        Image bgImage = bgObject.AddComponent<Image>();
        bgImage.sprite = backgroundSprite;
        bgImage.color = new Color(0.5f, 0.5f, 0.5f); // Darken for contrast

        RectTransform rt = bgObject.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.SetAsFirstSibling();
    }

    void CreateTitle()
    {
        GameObject titleContainer = new GameObject("TitleContainer");
        titleContainer.transform.SetParent(parentCanvas.transform, false);

        VerticalLayoutGroup layout = titleContainer.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;

        ContentSizeFitter fitter = titleContainer.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        RectTransform titleRT = titleContainer.GetComponent<RectTransform>();
        titleRT.anchorMin = new Vector2(0.5f, 0.75f);
        titleRT.anchorMax = new Vector2(0.5f, 0.75f);
        titleRT.pivot = new Vector2(0.5f, 0.5f);
        titleRT.anchoredPosition = Vector2.zero;
        titleRT.sizeDelta = new Vector2(1000, 0);

        GameObject titleObject = new GameObject("TitleText");
        titleObject.transform.SetParent(titleContainer.transform, false);
        TextMeshProUGUI titleText = titleObject.AddComponent<TextMeshProUGUI>();
        titleText.text = "THE LAST MILE";
        titleText.font = titleFont;
        titleText.fontSize = 140;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color32(226, 232, 240, 255);
        titleText.outlineColor = new Color32(220, 38, 38, 255);
        titleText.outlineWidth = 0.1f;
    }

    void CreateButtons()
    {
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(parentCanvas.transform, false);
        VerticalLayoutGroup layoutGroup = buttonContainer.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 20;
        layoutGroup.childAlignment = TextAnchor.MiddleCenter;

        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
        layoutGroup.childForceExpandHeight = true;
        layoutGroup.childForceExpandWidth = true;

        RectTransform containerRT = buttonContainer.GetComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.5f, 0.5f);
        containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.pivot = new Vector2(0.5f, 0.5f);
        containerRT.anchoredPosition = new Vector2(0, -50);
        containerRT.sizeDelta = new Vector2(400, 250);

        var buttonConfigs = new List<(string, UnityEngine.Events.UnityAction, MenuButtonAnimator.ButtonType)>
        {
            ("START RUN", mainMenuLogic.StartRun, MenuButtonAnimator.ButtonType.StartRun),
            ("SURVIVOR CAMP", mainMenuLogic.GoToSurvivorCamp, MenuButtonAnimator.ButtonType.SurvivorCamp),
            ("EXIT", mainMenuLogic.ExitGame, MenuButtonAnimator.ButtonType.Normal)
        };

        foreach (var config in buttonConfigs)
        {
            GameObject buttonGO = Instantiate(buttonPrefab, buttonContainer.transform);
            buttonGO.name = config.Item1 + " Button";
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = config.Item1;
            buttonGO.GetComponent<Button>().onClick.AddListener(config.Item2);

            MenuButtonAnimator animator = buttonGO.GetComponent<MenuButtonAnimator>();
            if (animator != null)
            {
                animator.type = config.Item3;
            }
        }
    }

    void CreateDecorations()
    {
        if (handprintSprite == null) return;
        GameObject handprintObject = new GameObject("BloodyHandprint");
        handprintObject.transform.SetParent(parentCanvas.transform, false);
        Image handprintImage = handprintObject.AddComponent<Image>();
        handprintImage.sprite = handprintSprite;
        handprintImage.color = new Color(1, 1, 1, 0.3f);
        handprintImage.raycastTarget = false;

        RectTransform rt = handprintObject.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(-50, 50);
        rt.sizeDelta = new Vector2(150, 150);
        rt.localRotation = Quaternion.Euler(0, 0, -15);
    }
}
