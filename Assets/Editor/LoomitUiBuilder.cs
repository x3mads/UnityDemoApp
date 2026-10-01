using System.IO;
using DemoApp;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

// Rebuilds the demo scene UI so it matches the Android demo app (Loomit dark Material 3 look).
// Run from the menu: Loomit > Rebuild Demo UI. The GameObject names looked up by SceneController are preserved.
public static class LoomitUiBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string SpriteDir = "Assets/DemoApp/UI/Sprites";
    private const string IconDir = "Assets/DemoApp/UI/Icons";
    private const int SpriteScale = 4;

    private static readonly Color Green = Hex("00FFB6");
    private static readonly Color Blue = Hex("024A5E");
    private static readonly Color DarkBlue = Hex("012A36");
    private static readonly Color SurfaceVariant = Hex("033D4D");
    private static readonly Color White = Color.white;
    private static readonly Color White70 = new Color(1, 1, 1, 0.7f);
    private static readonly Color White40 = new Color(1, 1, 1, 0.4f);
    private static readonly Color OnPrimaryDisabled = Hex("012A36", 0.6f);
    private static readonly Color Clear = new Color(0, 0, 0, 0);

    private enum Kind { Primary, Outlined }

    [MenuItem("Loomit/Rebuild Demo UI")]
    public static void Build()
    {
        Directory.CreateDirectory(SpriteDir);
        ImportIcons();

        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        foreach (var n in new[] { "MainView", "ToastCanvas", "ConfigCanvas" })
        {
            var go = GameObject.Find(n);
            if (go != null) Object.DestroyImmediate(go);
        }

        var cam = Object.FindObjectOfType<Camera>();
        if (cam != null)
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = DarkBlue;
        }

        BuildMainView();
        BuildConfigDialog();
        BuildToast();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("[LoomitUiBuilder] Demo UI rebuilt.");
    }

    // ---------------------------------------------------------------- main view

    private static void BuildMainView()
    {
        var root = NewCanvas("MainView", 0, true);
        root.AddComponent<Image>().color = DarkBlue;

        var safe = UI("SafeArea", root.transform);
        Stretch(safe);
        safe.AddComponent<SafeAreaFitter>();

        var scroll = UI("Scroll", safe.transform);
        Stretch(scroll);
        var viewport = UI("Viewport", scroll.transform);
        Stretch(viewport);
        var hit = viewport.AddComponent<Image>();
        hit.color = Clear;
        viewport.AddComponent<RectMask2D>();

        var content = UI("Content", viewport.transform);
        var crt = content.GetComponent<RectTransform>();
        crt.anchorMin = new Vector2(0, 1);
        crt.anchorMax = new Vector2(1, 1);
        crt.pivot = new Vector2(0.5f, 1);
        crt.sizeDelta = Vector2.zero;
        VLG(content, 0, 16, 16, 16, 88);
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var sr = scroll.AddComponent<ScrollRect>();
        sr.viewport = viewport.GetComponent<RectTransform>();
        sr.content = crt;
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Elastic;
        sr.scrollSensitivity = 30;

        var c = content.transform;
        BuildHeader(c);

        SectionTitle(c, "Setup", 0);
        var setup = Card(c, "SetupCard");
        Label(setup, "MediatorLabel", "Mediator", 15, White, true);
        Spacer(setup, 8);
        var panel = UI("MediatorsPanel", setup);
        VLG(panel, 0, 0, 0, 0, 0);
        MakeMediatorDropdown(panel.transform);
        var warning = UI("ReopenWarningLabel", setup);
        VLG(warning, 0, 12, 8, 12, 8);
        Img(warning, Fill(16), Blue);
        Ring(warning.transform, 16, 1, SurfaceVariant);
        Label(warning.transform, "ReopenWarningText", "To switch mediator, reopen the app", 13, White70, false);
        LE(warning, prefH: 48);
        warning.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
        warning.transform.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        Spacer(setup, 16);
        MakeButton(setup, "InitButton", "Initialize SDK", Kind.Primary, 48, true, null, true);

        SectionTitle(c, "Privacy", 24);
        var privacy = Card(c, "PrivacyCard");
        Label(privacy, "CmpSettingsLabel", "CMP Settings", 15, White, true);
        Spacer(privacy, 4);
        MakeToggle(privacy, "AutoCmpToggle", "Automatic CMP");
        MakeToggle(privacy, "FakeRegionToggle", "Fake EEA region");
        Spacer(privacy, 8);
        var cmpRow = Row(privacy, "CmpButtonsRow", 8);
        MakeButton(cmpRow, "ShowCmpFormButton", "Show form", Kind.Outlined, 48, false, null, true);
        MakeButton(cmpRow, "ResetCmpButton", "Reset", Kind.Outlined, 48, false, null, true);

        SectionTitle(c, "Ad Types", 24);
        var row1 = Row(c, "AdRow1", 8);
        MakeAdTile(row1, "ShowApoButton", "App Open", "ic_ad_app_open");
        MakeAdTile(row1, "ShowIttButton", "Interstitial", "ic_ad_interstitial");
        Spacer(c, 8);
        var row2 = Row(c, "AdRow2", 8);
        MakeAdTile(row2, "ShowRewButton", "Rewarded", "ic_ad_rewarded");
        MakeAdTile(row2, "ShowBannerButton", "Banner", "ic_ad_banner");

        Spacer(c, 24);
        var toggleRow = UI("UserPropertiesToggle", c);
        var th = HLG(toggleRow, 0, 0, 0, 8, 0);
        th.childAlignment = TextAnchor.MiddleLeft;
        LE(toggleRow, minH: 48);
        var toggleFill = toggleRow.AddComponent<Image>();
        toggleFill.color = Clear;
        var toggleLabel = Label(toggleRow.transform, "UserPropertiesLabel", "User Properties", 16, Green, true);
        toggleLabel.characterSpacing = 1f;
        toggleLabel.margin = new Vector4(8, 0, 0, 0);
        LE(toggleLabel.gameObject, flexW: 1);
        var chevron = UI("UserPropertiesChevron", toggleRow.transform);
        LE(chevron, prefW: 24, prefH: 24);
        var chevronImg = chevron.AddComponent<Image>();
        chevronImg.sprite = Icon("ic_chevron_down");
        chevronImg.color = Green;
        chevronImg.preserveAspect = true;
        chevronImg.raycastTarget = false;
        FinishButton(toggleRow, toggleFill, new System.Collections.Generic.List<LoomitStyle.Entry>(), true);
        Spacer(c, 8);

        var upCard = Card(c, "UserPropertiesCard");
        Label(upCard, "UPIdentityLabel", "Identity", 15, White, true);
        Spacer(upCard, 4);
        Label(upCard, "UPIdentityDesc", "The user ID is attached to the user properties sent to the SDK.", 13, White70, false);
        Spacer(upCard, 8);
        MakeInput(upCard, "UserIdInput", "User ID").text = "user-123";
        Spacer(upCard, 8);
        MakeButton(upCard, "SetUserIdButton", "Set user ID", Kind.Primary, 48, true, null, true);

        Spacer(upCard, 20);
        Label(upCard, "UPSampleLabel", "Sample data", 15, White, true);
        Spacer(upCard, 4);
        Label(upCard, "UPSampleDesc", "Preset values for testing; purchase summary is fixed at $49.99 USD across 5 purchases. Note: \"old_key\" is not created by these presets.", 13, White70, false);
        Spacer(upCard, 8);
        MakeButton(upCard, "SetInstallDateButton", "Set install date to now", Kind.Outlined, 48, true, null, true);
        Spacer(upCard, 8);
        MakeButton(upCard, "SetPurchaseSummaryButton", "Set purchase summary", Kind.Outlined, 48, true, null, true);
        Spacer(upCard, 8);
        MakeButton(upCard, "SetCustomPropertiesButton", "Set sample custom properties", Kind.Outlined, 48, true, null, true);
        Spacer(upCard, 8);
        MakeButton(upCard, "RemoveCustomPropertyButton", "Remove old_key (if present)", Kind.Outlined, 48, true, null, true);

        Spacer(upCard, 20);
        Label(upCard, "UPCurrentLabel", "Current properties", 15, White, true);
        Spacer(upCard, 4);
        Label(upCard, "UPCurrentDesc", "Fetch the latest user properties from the SDK and show them below.", 13, White70, false);
        Spacer(upCard, 8);
        MakeButton(upCard, "GetUserPropertiesButton", "Get current properties", Kind.Primary, 48, true, null, true);
        Spacer(upCard, 8);
        var propsBox = UI("UserPropertiesBox", upCard);
        Img(propsBox, Fill(12), SurfaceVariant);
        VLG(propsBox, 0, 12, 12, 12, 12);
        LE(propsBox, minH: 72);
        var propsText = Label(propsBox.transform, "UserPropertiesText",
            "Nothing fetched yet. Tap \"Get current properties\" to see the result here.", 12, White70, false);
        propsText.alignment = TextAlignmentOptions.TopLeft;

        Spacer(upCard, 20);
        Label(upCard, "UPResetLabel", "Reset", 15, White, true);
        Spacer(upCard, 4);
        Label(upCard, "UPResetDesc", "Clear custom properties removes only your custom keys. Clear all also removes the user ID and built-in fields.", 13, White70, false);
        Spacer(upCard, 8);
        MakeButton(upCard, "ClearCustomPropertiesButton", "Clear custom properties", Kind.Outlined, 48, true, null, true);
        Spacer(upCard, 8);
        MakeButton(upCard, "ClearUserPropertiesButton", "Clear all", Kind.Outlined, 48, true, null, true);

        SectionTitle(c, "Tracking", 24);
        MakeButton(c, "TrackPurchaseButton", "Track Test Purchase", Kind.Primary, 48, true, null, true);
        Spacer(c, 8);
        MakeButton(c, "TrackAppEventButton", "Track Test App Event", Kind.Primary, 48, true, null, true);
    }

    private static void BuildHeader(Transform parent)
    {
        var header = UI("Header", parent);
        var h = HLG(header, 12, 8, 12, 8, 16);
        h.childAlignment = TextAnchor.MiddleLeft;

        var logo = UI("Logo", header.transform);
        var li = logo.AddComponent<Image>();
        li.sprite = Icon("ic_app_logo");
        li.color = Green;
        li.preserveAspect = true;
        li.raycastTarget = false;
        LE(logo, prefW: 28, prefH: 40);

        var texts = UI("Titles", header.transform);
        VLG(texts, 0, 0, 0, 0, 0);
        LE(texts, flexW: 1);
        Label(texts.transform, "AppTitle", "XMediator Demo", 20, Green, true);
        Label(texts.transform, "AppTagline", "XMediator SDK showcase", 12, White70, false);

        MakeButton(header.transform, "DebuggingSuiteButton", null, Kind.Outlined, 48, true, "ic_bug", true, 48, 24);
    }

    // ---------------------------------------------------------------- dialog

    private static void BuildConfigDialog()
    {
        var canvas = NewCanvas("ConfigCanvas", 10, true);

        var panel = UI("DialogPanel", canvas.transform);
        Stretch(panel);
        panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.7f);

        var card = UI("DialogCard", panel.transform);
        var rt = card.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(1, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(24, 0);
        rt.offsetMax = new Vector2(-24, 0);
        Img(card, Fill(16), Blue);
        VLG(card, 10, 16, 16, 16, 16);
        card.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        Label(card.transform, "DialogTitle", "Custom configuration", 18, Green, true);
        var appKey = MakeInput(card.transform, "AppKey", "App key");
        var banner = MakeInput(card.transform, "BannerPlacementId", "Banner placement ID");
        var itt = MakeInput(card.transform, "IttPlacementId", "Interstitial placement ID");
        var rew = MakeInput(card.transform, "RewPlacementId", "Rewarded placement ID");
        var apo = MakeInput(card.transform, "ApoPlacementId", "App Open placement ID (optional)");
        Spacer(card.transform, 6);
        var row = Row(card.transform, "DialogButtons", 8);
        var cancel = MakeButton(row, "CancelButton", "Cancel", Kind.Outlined, 48, true, null, true);
        var accept = MakeButton(row, "AcceptButton", "Accept", Kind.Primary, 48, true, null, true);

        var dialog = panel.AddComponent<ConfigurationDialog>();
        var so = new SerializedObject(dialog);
        so.FindProperty("appKeyInput").objectReferenceValue = appKey;
        so.FindProperty("bannerPlacementInput").objectReferenceValue = banner;
        so.FindProperty("interstitialPlacementInput").objectReferenceValue = itt;
        so.FindProperty("rewardedPlacementInput").objectReferenceValue = rew;
        so.FindProperty("appOpenPlacementInput").objectReferenceValue = apo;
        so.FindProperty("confirmButton").objectReferenceValue = accept;
        so.FindProperty("cancelButton").objectReferenceValue = cancel;
        so.FindProperty("dialogPanel").objectReferenceValue = panel;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ---------------------------------------------------------------- toast

    private static void BuildToast()
    {
        var canvas = NewCanvas("ToastCanvas", 20, false);

        var panel = UI("ToastPanel", canvas.transform);
        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(0.5f, 0);
        rt.offsetMin = new Vector2(24, 96);
        rt.offsetMax = new Vector2(-24, 96);
        var bg = Img(panel, Fill(24), SurfaceVariant);
        bg.raycastTarget = false;
        VLG(panel, 0, 20, 12, 20, 12);
        panel.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var textGo = UI("ToastText", panel.transform);
        var text = textGo.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 14;
        text.color = White;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        panel.SetActive(false);

        var manager = canvas.AddComponent<ToastManager>();
        var so = new SerializedObject(manager);
        so.FindProperty("toastPanel").objectReferenceValue = panel;
        so.FindProperty("toastText").objectReferenceValue = text;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ---------------------------------------------------------------- components

    private static void SectionTitle(Transform parent, string text, float topSpace)
    {
        if (topSpace > 0) Spacer(parent, topSpace);
        var t = Label(parent, "Section_" + text.Replace(" ", ""), text, 16, Green, true);
        t.characterSpacing = 1f;
        t.margin = new Vector4(8, 0, 0, 0);
        Spacer(parent, 8);
    }

    private static Transform Card(Transform parent, string name)
    {
        var go = UI(name, parent);
        Img(go, Fill(16), Blue);
        VLG(go, 0, 16, 16, 16, 16);
        return go.transform;
    }

    private static Transform Row(Transform parent, string name, float spacing)
    {
        var go = UI(name, parent);
        var h = HLG(go, spacing, 0, 0, 0, 0);
        h.childForceExpandWidth = true;
        return go.transform;
    }

    private static TextMeshProUGUI Label(Transform parent, string name, string text, float size, Color color, bool bold)
    {
        var go = UI(name, parent);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        t.enableWordWrapping = true;
        t.raycastTarget = false;
        t.alignment = TextAlignmentOptions.MidlineLeft;
        return t;
    }

    private static Button MakeButton(Transform parent, string name, string label, Kind kind, float height, bool interactable,
        string iconName, bool flexible, float width = -1, int radius = 24)
    {
        var go = UI(name, parent);
        LE(go, prefH: height, prefW: width, flexW: width > 0 ? -1 : 1);

        var fill = go.AddComponent<Image>();
        fill.sprite = Fill(radius);
        fill.type = Image.Type.Sliced;
        fill.pixelsPerUnitMultiplier = SpriteScale;
        fill.color = kind == Kind.Primary ? Green : Clear;

        var entries = new System.Collections.Generic.List<LoomitStyle.Entry>();
        var accent = kind == Kind.Primary ? DarkBlue : Green;
        var accentOff = kind == Kind.Primary ? OnPrimaryDisabled : White40;

        if (kind == Kind.Primary)
        {
            entries.Add(Entry(fill, Green, White40));
        }
        else
        {
            var stroke = Ring(go.transform, radius, 1, Green);
            entries.Add(Entry(fill, Clear, Clear));
            entries.Add(Entry(stroke, Green, Clear));
        }

        if (label != null)
        {
            var t = Label(go.transform, "Label", label, 15, accent, true);
            t.alignment = TextAlignmentOptions.Center;
            Stretch(t.gameObject);
            entries.Add(Entry(t, accent, accentOff));
        }

        if (iconName != null)
        {
            var icon = UI("Icon", go.transform);
            var ir = icon.GetComponent<RectTransform>();
            ir.anchorMin = ir.anchorMax = new Vector2(0.5f, 0.5f);
            ir.sizeDelta = new Vector2(22, 22);
            var i = icon.AddComponent<Image>();
            i.sprite = Icon(iconName);
            i.raycastTarget = false;
            entries.Add(Entry(i, Green, White40));
        }

        return FinishButton(go, fill, entries, interactable);
    }

    private static void MakeAdTile(Transform parent, string name, string label, string iconName)
    {
        var go = UI(name, parent);
        LE(go, prefH: 92, flexW: 1);

        var fill = go.AddComponent<Image>();
        fill.sprite = Fill(14);
        fill.type = Image.Type.Sliced;
        fill.pixelsPerUnitMultiplier = SpriteScale;
        var stroke = Ring(go.transform, 14, 2, Green);

        var icon = UI("Icon", go.transform);
        var ir = icon.GetComponent<RectTransform>();
        ir.anchorMin = ir.anchorMax = new Vector2(0.5f, 1);
        ir.pivot = new Vector2(0.5f, 1);
        ir.sizeDelta = new Vector2(28, 28);
        ir.anchoredPosition = new Vector2(0, -16);
        var i = icon.AddComponent<Image>();
        i.sprite = Icon(iconName);
        i.raycastTarget = false;

        var t = Label(go.transform, "Label", label, 14, Green, true);
        t.alignment = TextAlignmentOptions.Center;
        var tr = t.rectTransform;
        tr.anchorMin = new Vector2(0, 0);
        tr.anchorMax = new Vector2(1, 0);
        tr.pivot = new Vector2(0.5f, 0);
        tr.sizeDelta = new Vector2(0, 24);
        tr.anchoredPosition = new Vector2(0, 14);

        var faded = new Color(SurfaceVariant.r, SurfaceVariant.g, SurfaceVariant.b, 0.4f);
        var entries = new System.Collections.Generic.List<LoomitStyle.Entry>
        {
            Entry(fill, SurfaceVariant, faded),
            Entry(stroke, Green, Clear),
            Entry(i, Green, White40),
            Entry(t, Green, White40)
        };
        FinishButton(go, fill, entries, false);
    }

    private static Button FinishButton(GameObject go, Image fill, System.Collections.Generic.List<LoomitStyle.Entry> entries, bool interactable)
    {
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = fill;
        btn.transition = Selectable.Transition.ColorTint;
        var cb = btn.colors;
        cb.normalColor = White;
        cb.highlightedColor = White;
        cb.selectedColor = White;
        cb.disabledColor = White;
        cb.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1);
        cb.fadeDuration = 0.08f;
        btn.colors = cb;
        btn.interactable = interactable;

        var style = go.AddComponent<LoomitStyle>();
        style.target = btn;
        style.entries = entries.ToArray();
        ApplyStyleNow(style, interactable);
        return btn;
    }

    private static void ApplyStyleNow(LoomitStyle style, bool interactable)
    {
        foreach (var e in style.entries)
            e.graphic.color = interactable ? e.enabledColor : e.disabledColor;
    }

    private static LoomitStyle.Entry Entry(Graphic g, Color on, Color off) =>
        new LoomitStyle.Entry { graphic = g, enabledColor = on, disabledColor = off };

    private static void MakeToggle(Transform parent, string name, string label)
    {
        var go = UI(name, parent);
        var h = HLG(go, 12, 0, 4, 0, 4);
        h.childAlignment = TextAnchor.MiddleLeft;
        h.childForceExpandWidth = false;
        LE(go, prefH: 40);

        var box = UI("Box", go.transform);
        LE(box, prefW: 22, prefH: 22);
        var boxImg = box.AddComponent<Image>();
        boxImg.sprite = Ring(6, 2);
        boxImg.type = Image.Type.Sliced;
        boxImg.pixelsPerUnitMultiplier = SpriteScale;

        var check = UI("Check", box.transform);
        var cr = check.GetComponent<RectTransform>();
        cr.anchorMin = Vector2.zero;
        cr.anchorMax = Vector2.one;
        cr.offsetMin = new Vector2(5, 5);
        cr.offsetMax = new Vector2(-5, -5);
        var checkImg = check.AddComponent<Image>();
        checkImg.sprite = Fill(3);
        checkImg.type = Image.Type.Sliced;
        checkImg.pixelsPerUnitMultiplier = SpriteScale;
        checkImg.raycastTarget = false;

        var t = Label(go.transform, "ToggleLabel", label, 15, White, false);
        LE(t.gameObject, flexW: 1);

        var toggle = go.AddComponent<Toggle>();
        toggle.targetGraphic = boxImg;
        toggle.graphic = checkImg;
        toggle.transition = Selectable.Transition.None;
        toggle.isOn = false;

        var style = go.AddComponent<LoomitStyle>();
        style.target = toggle;
        style.entries = new[]
        {
            Entry(boxImg, Green, White40),
            Entry(checkImg, Green, White40),
            Entry(t, White, White40)
        };
        ApplyStyleNow(style, true);
    }

    private static InputField MakeInput(Transform parent, string name, string placeholder)
    {
        var go = UI(name, parent);
        LE(go, prefH: 48);
        var bg = Img(go, Fill(12), SurfaceVariant);
        Ring(go.transform, 12, 1, White40);

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var textGo = UI("InputText", go.transform);
        Stretch(textGo);
        var rt = textGo.GetComponent<RectTransform>();
        rt.offsetMin = new Vector2(14, 6);
        rt.offsetMax = new Vector2(-14, -6);
        var text = textGo.AddComponent<Text>();
        text.font = font;
        text.fontSize = 15;
        text.color = White;
        text.alignment = TextAnchor.MiddleLeft;
        text.supportRichText = false;

        var phGo = UI("InputPlaceholder", go.transform);
        Stretch(phGo);
        var prt = phGo.GetComponent<RectTransform>();
        prt.offsetMin = new Vector2(14, 6);
        prt.offsetMax = new Vector2(-14, -6);
        var ph = phGo.AddComponent<Text>();
        ph.font = font;
        ph.fontSize = 15;
        ph.fontStyle = FontStyle.Italic;
        ph.color = White40;
        ph.alignment = TextAnchor.MiddleLeft;
        ph.text = placeholder;
        ph.raycastTarget = false;

        var input = go.AddComponent<InputField>();
        input.targetGraphic = bg;
        input.textComponent = text;
        input.placeholder = ph;
        input.caretColor = Green;
        input.customCaretColor = true;
        input.selectionColor = new Color(Green.r, Green.g, Green.b, 0.35f);
        return input;
    }

    private static void MakeMediatorDropdown(Transform parent)
    {
        var res = new TMP_DefaultControls.Resources
        {
            standard = Builtin("UI/Skin/UISprite.psd"),
            background = Builtin("UI/Skin/Background.psd"),
            inputField = Builtin("UI/Skin/InputFieldBackground.psd"),
            knob = Builtin("UI/Skin/Knob.psd"),
            checkmark = Builtin("UI/Skin/Checkmark.psd"),
            dropdown = Builtin("UI/Skin/DropdownArrow.psd"),
            mask = Builtin("UI/Skin/UIMask.psd")
        };
        var go = TMP_DefaultControls.CreateDropdown(res);
        go.name = "MediatorDropdown";
        go.transform.SetParent(parent, false);
        LE(go, prefH: 48);

        var dd = go.GetComponent<TMP_Dropdown>();
        dd.options.Clear();
        foreach (var o in new[] { "", "MAX", "GOOGLE ADS", "LEVEL PLAY", "X3M", "CUSTOM" })
            dd.options.Add(new TMP_Dropdown.OptionData(o));
        dd.value = 0;
        dd.RefreshShownValue();

        var rootImg = go.GetComponent<Image>();
        rootImg.sprite = Fill(12);
        rootImg.type = Image.Type.Sliced;
        rootImg.pixelsPerUnitMultiplier = SpriteScale;
        rootImg.color = SurfaceVariant;
        Ring(go.transform, 12, 1, Green).transform.SetSiblingIndex(1);

        var caption = dd.captionText;
        caption.color = White;
        caption.fontSize = 15;
        caption.rectTransform.offsetMin = new Vector2(14, 6);
        caption.rectTransform.offsetMax = new Vector2(-40, -6);

        var arrow = go.transform.Find("Arrow").GetComponent<Image>();
        arrow.sprite = Icon("ic_chevron_down");
        arrow.color = Green;
        arrow.preserveAspect = true;
        var art = arrow.rectTransform;
        art.sizeDelta = new Vector2(24, 24);
        art.anchoredPosition = new Vector2(-14, 0);

        var template = go.transform.Find("Template");
        var timg = template.GetComponent<Image>();
        timg.sprite = Fill(12);
        timg.type = Image.Type.Sliced;
        timg.pixelsPerUnitMultiplier = SpriteScale;
        timg.color = SurfaceVariant;
        var item = template.Find("Viewport/Content/Item");
        var itemBg = item.Find("Item Background").GetComponent<Image>();
        itemBg.color = SurfaceVariant;
        var itemLabel = item.Find("Item Label").GetComponent<TMP_Text>();
        itemLabel.color = White;
        itemLabel.fontSize = 15;
        item.Find("Item Checkmark").GetComponent<Image>().color = Green;
        var itemToggle = item.GetComponent<Toggle>();
        var tc = itemToggle.colors;
        tc.normalColor = White;
        tc.highlightedColor = new Color(0.75f, 0.75f, 0.75f, 1);
        tc.pressedColor = new Color(0.6f, 0.6f, 0.6f, 1);
        tc.selectedColor = new Color(0.75f, 0.75f, 0.75f, 1);
        itemToggle.colors = tc;
        template.gameObject.SetActive(false);
        var sb = template.Find("Scrollbar");
        if (sb != null) sb.gameObject.SetActive(false);
    }

    // ---------------------------------------------------------------- helpers

    private static GameObject NewCanvas(string name, int order, bool raycaster)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = order;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(360, 800);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0;
        if (raycaster) go.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        return go;
    }

    private static GameObject UI(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static Image Img(GameObject go, Sprite sprite, Color color)
    {
        var img = go.AddComponent<Image>();
        img.sprite = sprite;
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = SpriteScale;
        img.color = color;
        return img;
    }

    private static Image Ring(Transform parent, int radius, int stroke, Color color)
    {
        var go = UI("Stroke", parent);
        go.AddComponent<LayoutElement>().ignoreLayout = true;
        Stretch(go);
        var img = go.AddComponent<Image>();
        img.sprite = Ring(radius, stroke);
        img.type = Image.Type.Sliced;
        img.pixelsPerUnitMultiplier = SpriteScale;
        img.color = color;
        img.raycastTarget = false;
        return img;
    }

    private static void Spacer(Transform parent, float height)
    {
        var go = UI("Spacer", parent);
        LE(go, prefH: height, minH: height);
    }

    private static VerticalLayoutGroup VLG(GameObject go, float spacing, int l, int t, int r, int b)
    {
        var v = go.AddComponent<VerticalLayoutGroup>();
        v.spacing = spacing;
        v.padding = new RectOffset(l, r, t, b);
        v.childControlWidth = true;
        v.childControlHeight = true;
        v.childForceExpandWidth = true;
        v.childForceExpandHeight = false;
        return v;
    }

    private static HorizontalLayoutGroup HLG(GameObject go, float spacing, int l, int t, int r, int b)
    {
        var h = go.AddComponent<HorizontalLayoutGroup>();
        h.spacing = spacing;
        h.padding = new RectOffset(l, r, t, b);
        h.childControlWidth = true;
        h.childControlHeight = true;
        h.childForceExpandWidth = false;
        h.childForceExpandHeight = false;
        return h;
    }

    private static void LE(GameObject go, float minH = -1, float prefH = -1, float prefW = -1, float flexW = -1)
    {
        var le = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        if (minH >= 0) le.minHeight = minH;
        if (prefH >= 0) le.preferredHeight = prefH;
        if (prefW >= 0) le.preferredWidth = prefW;
        if (flexW >= 0) le.flexibleWidth = flexW;
    }

    private static Color Hex(string hex, float alpha = 1f)
    {
        ColorUtility.TryParseHtmlString("#" + hex, out var c);
        c.a = alpha;
        return c;
    }

    private static Sprite Builtin(string path) => AssetDatabase.GetBuiltinExtraResource<Sprite>(path);

    private static Sprite Icon(string name) => AssetDatabase.LoadAssetAtPath<Sprite>($"{IconDir}/{name}.png");

    private static Sprite Fill(int radius) => Rounded(radius, 0);

    private static Sprite Ring(int radius, int stroke) => Rounded(radius, stroke);

    private static void ImportIcons()
    {
        foreach (var path in Directory.GetFiles(IconDir, "*.png"))
        {
            var importer = (TextureImporter)AssetImporter.GetAtPath(path.Replace('\\', '/'));
            if (importer == null) continue;
            var settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            if (importer.textureType == TextureImporterType.Sprite && settings.spriteMeshType == SpriteMeshType.FullRect) continue;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 512;
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.SaveAndReimport();
        }
    }

    // Generates (once) a 9-sliceable rounded rectangle sprite: filled when stroke == 0, outline otherwise.
    // Textures are drawn at SpriteScale x, and Images use pixelsPerUnitMultiplier = SpriteScale to compensate.
    private static Sprite Rounded(int radius, int stroke)
    {
        var path = $"{SpriteDir}/rounded_r{radius}_s{stroke}.png";
        var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (existing != null) return existing;

        int r = radius * SpriteScale, s = stroke * SpriteScale, size = 2 * r + 8;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color32[size * size];
        float half = size / 2f;
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                float qx = Mathf.Abs(x + 0.5f - half) - (half - r);
                float qy = Mathf.Abs(y + 0.5f - half) - (half - r);
                float d = new Vector2(Mathf.Max(qx, 0), Mathf.Max(qy, 0)).magnitude + Mathf.Min(Mathf.Max(qx, qy), 0) - r;
                var a = Mathf.Clamp01(0.5f - d);
                if (s > 0) a -= Mathf.Clamp01(0.5f - (d + s));
                pixels[y * size + x] = new Color(1, 1, 1, Mathf.Clamp01(a));
            }
        }
        tex.SetPixels32(pixels);
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.spriteBorder = new Vector4(r, r, r, r);
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = true;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
