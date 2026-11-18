#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Menu: Tools > RPS > Build All Scenes   (or build each scene separately)
// Builds MenuScene + GameScene, connects button events, and sets the Build Settings scene list.
public static class BuildRPSScene
{
    const string GamePath = "Assets/Scenes/GameScene.unity";
    const string MenuPath = "Assets/Scenes/MenuScene.unity";

    [MenuItem("Tools/RPS/Build All Scenes")]
    public static void BuildAll()
    {
        BuildMenu();
        BuildGame();
        SetBuildScenes();
        EditorSceneManager.OpenScene(MenuPath);
        Debug.Log("All scenes built. MenuScene is now the active scene.");
    }

    // =====================================================  GAME SCENE
    [MenuItem("Tools/RPS/Build Game Scene")]
    public static void BuildGame()
    {
        var scene = EditorSceneManager.OpenScene(GamePath);

        DestroyIfExists("Canvas");
        DestroyIfExists("GameManager");

        var canvasGO = MakeCanvas();

        var mainPanel = MakeImage("Main Panel", canvasGO.transform, new Color(0.12f, 0.14f, 0.2f));
        Stretch(mainPanel, 0, 0, 1, 1);

        var gm = new GameObject("GameManager");
        var controller = gm.AddComponent<GameController>();

        // Rock / Paper / Scissors buttons (name starts with the number the script reads)
        MakePictureButton("1_Rock Button", mainPanel.transform, "img_1", 0.30f, 0.16f, new Vector2(260, 260), 0, controller);
        MakePictureButton("2_Paper Button", mainPanel.transform, "img_2", 0.50f, 0.16f, new Vector2(260, 260), 0, controller);
        MakePictureButton("3_Scissor Button", mainPanel.transform, "img_3", 0.70f, 0.16f, new Vector2(170, 260), 90, controller);

        // Back to menu button (top-left)
        var menuBtn = MakeTextButton("Menu Button", mainPanel.transform, "Menu", new Vector2(0.09f, 0.95f), new Vector2(200, 60), 30);
        UnityEventTools.AddVoidPersistentListener(menuBtn.onClick, controller.OnMenuClick);

        var uiPanel = MakeImage("UI_Panel", mainPanel.transform, new Color(0.2f, 0.24f, 0.36f));
        Stretch(uiPanel, 0.05f, 0.32f, 0.95f, 0.90f);

        var imgYou = MakeImage("ImgYou", uiPanel.transform, Color.white);
        imgYou.GetComponent<Image>().sprite = Load("img_4");
        imgYou.GetComponent<Image>().preserveAspect = true;
        Stretch(imgYou, 0.05f, 0.35f, 0.38f, 0.95f);

        var imgCom = MakeImage("ImgCom", uiPanel.transform, Color.white);
        imgCom.GetComponent<Image>().sprite = Load("img_4");
        imgCom.GetComponent<Image>().preserveAspect = true;
        Stretch(imgCom, 0.62f, 0.35f, 0.95f, 0.95f);

        var result = MakeText("TxtResult_", uiPanel.transform, "Select the button below", 48, Color.white);
        result.resizeTextForBestFit = true;
        result.resizeTextMinSize = 20;
        result.resizeTextMaxSize = 60;
        result.fontStyle = FontStyle.Bold;
        Stretch(result.gameObject, 0.39f, 0.45f, 0.61f, 0.85f);

        MakeScore("ImgScore_You", "Text", "You", "TxtYou", uiPanel.transform, 0.08f, 0.32f);
        MakeScore("ImgScore_Com", "Text", "Computer", "TxtCom", uiPanel.transform, 0.62f, 0.92f);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("GameScene built and saved.");
    }

    // =====================================================  MENU SCENE
    [MenuItem("Tools/RPS/Build Menu Scene")]
    public static void BuildMenu()
    {
        // New empty scene with a camera, then remove the 3D light (not needed in 2D)
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        DestroyIfExists("Directional Light");

        var canvasGO = MakeCanvas();

        var bg = MakeImage("Background", canvasGO.transform, new Color(0.12f, 0.14f, 0.2f));
        Stretch(bg, 0, 0, 1, 1);

        var mm = new GameObject("MenuManager");
        var menu = mm.AddComponent<MenuController>();

        var title = MakeText("TxtTitle", bg.transform, "Rock  Paper  Scissors", 100, Color.white);
        title.fontStyle = FontStyle.Bold;
        title.resizeTextForBestFit = true;
        title.resizeTextMinSize = 30;
        title.resizeTextMaxSize = 110;
        Stretch(title.gameObject, 0.1f, 0.65f, 0.9f, 0.9f);

        // Decoration: the three hand pictures
        MakePicture("ImgRock", bg.transform, "img_1", 0.36f, 0.5f, new Vector2(150, 150), 0);
        MakePicture("ImgPaper", bg.transform, "img_2", 0.50f, 0.5f, new Vector2(150, 150), 0);
        MakePicture("ImgScissor", bg.transform, "img_3", 0.64f, 0.5f, new Vector2(100, 150), 90);

        var play = MakeTextButton("Play Button", bg.transform, "Play", new Vector2(0.5f, 0.30f), new Vector2(400, 100), 50);
        UnityEventTools.AddVoidPersistentListener(play.onClick, menu.OnPlayClick);

        var quit = MakeTextButton("Quit Button", bg.transform, "Quit", new Vector2(0.5f, 0.15f), new Vector2(400, 100), 50);
        UnityEventTools.AddVoidPersistentListener(quit.onClick, menu.OnQuitClick);

        EditorSceneManager.SaveScene(scene, MenuPath);
        Debug.Log("MenuScene built and saved.");
    }

    // Build Settings: MenuScene = index 0 (starts first), GameScene = index 1
    [MenuItem("Tools/RPS/Set Build Scenes")]
    public static void SetBuildScenes()
    {
        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(MenuPath, true),
            new EditorBuildSettingsScene(GamePath, true)
        };
        Debug.Log("Build Settings: 0 = MenuScene, 1 = GameScene");
    }

    // =====================================================  HELPERS
    static void DestroyIfExists(string name)
    {
        var go = GameObject.Find(name);
        if (go != null) Object.DestroyImmediate(go);
    }

    static Sprite Load(string n) { return Resources.Load<Sprite>(n); }

    static GameObject MakeCanvas()
    {
        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGO.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (Object.FindFirstObjectByType<EventSystem>() == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        return canvasGO;
    }

    static GameObject MakeImage(string name, Transform parent, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    // Anchors only (no fixed pixel size) so it scales with the screen
    static void Stretch(GameObject go, float xMin, float yMin, float xMax, float yMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(xMin, yMin);
        rt.anchorMax = new Vector2(xMax, yMax);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Fixed-size object pinned to one anchor point
    static void PinToPoint(GameObject go, Vector2 anchor, Vector2 size, float rotZ)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        rt.localRotation = Quaternion.Euler(0, 0, rotZ);
    }

    static Text MakeText(string name, Transform parent, string content, int size, Color color)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);
        var t = go.GetComponent<Text>();
        t.text = content;
        t.fontSize = size;
        t.color = color;
        t.alignment = TextAnchor.MiddleCenter;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return t;
    }

    static void MakePicture(string name, Transform parent, string sprite, float x, float y, Vector2 size, float rotZ)
    {
        var go = MakeImage(name, parent, Color.white);
        go.GetComponent<Image>().sprite = Load(sprite);
        PinToPoint(go, new Vector2(x, y), size, rotZ);
    }

    static void MakeScore(string boxName, string labelName, string label, string valueName,
                          Transform parent, float xMin, float xMax)
    {
        var box = MakeImage(boxName, parent, new Color(0.1f, 0.12f, 0.2f));
        Stretch(box, xMin, 0.04f, xMax, 0.30f);

        var l = MakeText(labelName, box.transform, label, 36, new Color(0.8f, 0.85f, 1f));
        Stretch(l.gameObject, 0, 0.5f, 1, 1);

        var v = MakeText(valueName, box.transform, "0", 60, Color.white);
        v.fontStyle = FontStyle.Bold;
        Stretch(v.gameObject, 0, 0, 1, 0.5f);
    }

    static void SetButtonColors(Button btn)
    {
        var colors = btn.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.95f, 1f);
        colors.pressedColor = new Color(0.55f, 0.75f, 1f);   // pressed-color feedback
        btn.colors = colors;
    }

    // Rock / Paper / Scissors button: picture + On Click -> OnButtonClick(this button)
    static void MakePictureButton(string name, Transform parent, string sprite, float x, float y,
                                  Vector2 size, float rotZ, GameController controller)
    {
        var go = MakeImage(name, parent, Color.white);
        var img = go.GetComponent<Image>();
        img.sprite = Load(sprite);
        PinToPoint(go, new Vector2(x, y), size, rotZ);

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        SetButtonColors(btn);

        UnityEventTools.AddObjectPersistentListener<GameObject>(btn.onClick, controller.OnButtonClick, go);
    }

    // Normal text button (Play, Quit, Menu)
    static Button MakeTextButton(string name, Transform parent, string label, Vector2 anchor, Vector2 size, int fontSize)
    {
        var go = MakeImage(name, parent, new Color(0.35f, 0.55f, 0.95f));
        PinToPoint(go, anchor, size, 0);

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();
        SetButtonColors(btn);

        var t = MakeText("Text", go.transform, label, fontSize, Color.white);
        t.fontStyle = FontStyle.Bold;
        Stretch(t.gameObject, 0, 0, 1, 1);
        return btn;
    }
}
#endif
