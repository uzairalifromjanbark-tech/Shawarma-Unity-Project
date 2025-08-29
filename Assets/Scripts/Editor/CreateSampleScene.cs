using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace ShawarmaLegend.EditorTools
{
    public static class CreateSampleScene
    {
        [MenuItem("Tools/Create Shawarma Sample Scene")] 
        public static void CreateScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            var gameRoot = new GameObject("GameRoot");
            var gm = gameRoot.AddComponent<GameManager>();
            var om = gameRoot.AddComponent<OrderManager>();

            // Canvas and EventSystem are already created by DefaultGameObjects
            var canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
                var canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Ensure EventSystem exists for mouse clicks
            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            var hud = canvasObj.AddComponent<HUDController>();

            // Create TMP objects
            var dayGo = new GameObject("DayText");
            dayGo.transform.SetParent(canvasObj.transform);
            var dayText = dayGo.AddComponent<TextMeshProUGUI>();
            dayText.rectTransform.anchorMin = new Vector2(0, 1);
            dayText.rectTransform.anchorMax = new Vector2(0, 1);
            dayText.rectTransform.pivot = new Vector2(0, 1);
            dayText.rectTransform.anchoredPosition = new Vector2(20, -20);
            dayText.fontSize = 32;
            dayText.text = "Day 1";

            var coinsGo = new GameObject("CoinsText");
            coinsGo.transform.SetParent(canvasObj.transform);
            var coinsText = coinsGo.AddComponent<TextMeshProUGUI>();
            coinsText.rectTransform.anchorMin = new Vector2(1, 1);
            coinsText.rectTransform.anchorMax = new Vector2(1, 1);
            coinsText.rectTransform.pivot = new Vector2(1, 1);
            coinsText.rectTransform.anchoredPosition = new Vector2(-20, -20);
            coinsText.fontSize = 32;
            coinsText.text = "$ 0";

            // Buttons
            Button MakeButton(string name, Vector2 anchor, Vector2 pos)
            {
                var go = new GameObject(name);
                go.transform.SetParent(canvasObj.transform);
                var image = go.AddComponent<Image>();
                image.color = new Color(0.1f, 0.6f, 0.3f, 0.9f);
                var btn = go.AddComponent<Button>();
                var rt = go.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(180, 60);
                rt.anchorMin = anchor;
                rt.anchorMax = anchor;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = pos;

                var labelGo = new GameObject("Label");
                labelGo.transform.SetParent(go.transform);
                var label = labelGo.AddComponent<TextMeshProUGUI>();
                label.text = name;
                label.alignment = TextAlignmentOptions.Center;
                var lrt = label.GetComponent<RectTransform>();
                lrt.anchorMin = Vector2.zero;
                lrt.anchorMax = Vector2.one;
                lrt.offsetMin = Vector2.zero;
                lrt.offsetMax = Vector2.zero;
                label.fontSize = 28;
                return btn;
            }

            var startBtn = MakeButton("Start Day", new Vector2(0.5f, 0), new Vector2(-120, 40));
            var endBtn = MakeButton("End Day", new Vector2(0.5f, 0), new Vector2(120, 40));
            var nextBtn = MakeButton("Next Day", new Vector2(0.5f, 0.5f), new Vector2(0, -40));

            // Wire HUD
            hud.GetType().GetField("dayText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, dayText);
            hud.GetType().GetField("coinsText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, coinsText);
            hud.GetType().GetField("startDayButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, startBtn);
            hud.GetType().GetField("endDayButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, endBtn);
            hud.GetType().GetField("nextDayButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, nextBtn);

            // Timer text
            var timerGo = new GameObject("TimerText");
            timerGo.transform.SetParent(canvasObj.transform);
            var timerText = timerGo.AddComponent<TextMeshProUGUI>();
            timerText.rectTransform.anchorMin = new Vector2(0.5f, 1);
            timerText.rectTransform.anchorMax = new Vector2(0.5f, 1);
            timerText.rectTransform.pivot = new Vector2(0.5f, 1);
            timerText.rectTransform.anchoredPosition = new Vector2(0, -20);
            timerText.fontSize = 36;
            timerText.alignment = TextAlignmentOptions.Center;
            timerText.text = "02:00";
            hud.GetType().GetField("timerText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, timerText);

            // Day End Panel
            var panel = new GameObject("DayEndPanel");
            panel.transform.SetParent(canvasObj.transform);
            var panelImg = panel.AddComponent<Image>();
            panelImg.color = new Color(0, 0, 0, 0.6f);
            var prt = panel.GetComponent<RectTransform>();
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;
            panel.SetActive(false);

            var msgGo = new GameObject("Message");
            msgGo.transform.SetParent(panel.transform);
            var msg = msgGo.AddComponent<TextMeshProUGUI>();
            msg.text = "Day Finished";
            msg.alignment = TextAlignmentOptions.Center;
            var mrt = msg.GetComponent<RectTransform>();
            mrt.anchorMin = new Vector2(0.5f, 0.5f);
            mrt.anchorMax = new Vector2(0.5f, 0.5f);
            mrt.pivot = new Vector2(0.5f, 0.5f);
            mrt.anchoredPosition = new Vector2(0, 20);
            mrt.sizeDelta = new Vector2(300, 80);
            msg.fontSize = 40;

            nextBtn.transform.SetParent(panel.transform);

            hud.GetType().GetField("dayEndPanel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(hud, panel);

            // Link OrderManager in GameManager
            gm.GetType().GetField("orderManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(gm, om);

            // Orders Panel
            var ordersPanel = new GameObject("OrdersPanel");
            ordersPanel.transform.SetParent(canvasObj.transform);
            var opRT = ordersPanel.AddComponent<RectTransform>();
            opRT.anchorMin = new Vector2(0, 0.5f);
            opRT.anchorMax = new Vector2(0, 1);
            opRT.pivot = new Vector2(0, 1);
            opRT.anchoredPosition = new Vector2(10, -80);
            opRT.sizeDelta = new Vector2(320, 400);

            var vp = ordersPanel.AddComponent<ScrollRect>();
            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(ordersPanel.transform);
            var viewportRT = viewport.AddComponent<RectTransform>();
            viewportRT.anchorMin = Vector2.zero;
            viewportRT.anchorMax = Vector2.one;
            viewportRT.offsetMin = Vector2.zero;
            viewportRT.offsetMax = Vector2.zero;
            var mask = viewport.AddComponent<Mask>();
            mask.showMaskGraphic = false;
            var vpImg = viewport.AddComponent<Image>();
            vpImg.color = new Color(1, 1, 1, 0.05f);

            var content = new GameObject("Content");
            content.transform.SetParent(viewport.transform);
            var contentRT = content.AddComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.offsetMin = new Vector2(0, 0);
            contentRT.offsetMax = new Vector2(0, 0);
            var vlg = content.AddComponent<VerticalLayoutGroup>();
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = true;
            vlg.spacing = 6f;
            var fitter = content.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            vp.viewport = viewportRT;
            vp.content = contentRT;

            // Ticket prefab (runtime instance parented; not a Unity prefab asset)
            var ticketGO = new GameObject("TicketTemplate");
            var tImg = ticketGO.AddComponent<Image>();
            tImg.color = new Color(0.9f, 0.9f, 0.9f, 0.95f);
            var tRT = ticketGO.GetComponent<RectTransform>();
            tRT.sizeDelta = new Vector2(0, 80);
            var tv = ticketGO.AddComponent<OrderTicketView>();

            var ingGo = new GameObject("Ingredients");
            ingGo.transform.SetParent(ticketGO.transform);
            var ingText = ingGo.AddComponent<TextMeshProUGUI>();
            var ingRT = ingText.rectTransform;
            ingRT.anchorMin = new Vector2(0, 0.5f);
            ingRT.anchorMax = new Vector2(0.7f, 0.5f);
            ingRT.pivot = new Vector2(0, 0.5f);
            ingRT.anchoredPosition = new Vector2(10, 0);
            ingRT.sizeDelta = new Vector2(0, 60);
            ingText.fontSize = 20;
            ingText.enableWordWrapping = false;
            tv.GetType().GetField("ingredientsText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tv, ingText);

            var timeGo = new GameObject("Time");
            timeGo.transform.SetParent(ticketGO.transform);
            var timeText = timeGo.AddComponent<TextMeshProUGUI>();
            var timeRT = timeText.rectTransform;
            timeRT.anchorMin = new Vector2(0.7f, 0.5f);
            timeRT.anchorMax = new Vector2(1f, 0.5f);
            timeRT.pivot = new Vector2(1f, 0.5f);
            timeRT.anchoredPosition = new Vector2(-10, 0);
            timeRT.sizeDelta = new Vector2(0, 60);
            timeText.fontSize = 24;
            timeText.alignment = TMPro.TextAlignmentOptions.MidlineRight;
            tv.GetType().GetField("timerText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(tv, timeText);

            var ctrl = canvasObj.AddComponent<OrdersUIController>();
            ctrl.GetType().GetField("orderManager", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ctrl, om);
            ctrl.GetType().GetField("ticketsContainer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ctrl, contentRT);
            ctrl.GetType().GetField("ticketPrefab", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(ctrl, tv);

            // Shawarma Slicer Panel
            var slicerPanel = new GameObject("SlicerPanel");
            slicerPanel.transform.SetParent(canvasObj.transform);
            var slicerRT = slicerPanel.AddComponent<RectTransform>();
            slicerRT.anchorMin = new Vector2(0.5f, 0.5f);
            slicerRT.anchorMax = new Vector2(0.5f, 0.5f);
            slicerRT.pivot = new Vector2(0.5f, 0.5f);
            slicerRT.anchoredPosition = new Vector2(0, 0);
            slicerRT.sizeDelta = new Vector2(300, 200);

            var slicerBg = slicerPanel.AddComponent<Image>();
            slicerBg.color = new Color(0.8f, 0.6f, 0.4f, 0.9f);

            var slicerTitle = new GameObject("Title");
            slicerTitle.transform.SetParent(slicerPanel.transform);
            var titleText = slicerTitle.AddComponent<TextMeshProUGUI>();
            titleText.text = "Shawarma Slicer";
            titleText.fontSize = 24;
            titleText.alignment = TextAlignmentOptions.Center;
            titleText.color = Color.black;
            var titleRT = titleText.rectTransform;
            titleRT.anchorMin = new Vector2(0, 1);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.pivot = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -20);
            titleRT.sizeDelta = new Vector2(0, 40);

            var meatFill = new GameObject("MeatFill");
            meatFill.transform.SetParent(slicerPanel.transform);
            var fillImg = meatFill.AddComponent<Image>();
            fillImg.color = new Color(0.6f, 0.3f, 0.1f, 0.8f);
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Vertical;
            fillImg.fillAmount = 0f;
            var fillRT = fillImg.rectTransform;
            fillRT.anchorMin = new Vector2(0.5f, 0.5f);
            fillRT.anchorMax = new Vector2(0.5f, 0.5f);
            fillRT.pivot = new Vector2(0.5f, 0.5f);
            fillRT.anchoredPosition = new Vector2(0, 0);
            fillRT.sizeDelta = new Vector2(100, 120);

            var slicerScript = slicerPanel.AddComponent<ShawarmaSlicer>();
            slicerScript.GetType().GetField("meatFillImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(slicerScript, fillImg);

            // Assembly Panel
            var assemblyPanel = new GameObject("AssemblyPanel");
            assemblyPanel.transform.SetParent(canvasObj.transform);
            var assemblyRT = assemblyPanel.AddComponent<RectTransform>();
            assemblyRT.anchorMin = new Vector2(1, 0.5f);
            assemblyRT.anchorMax = new Vector2(1, 0.5f);
            assemblyRT.pivot = new Vector2(1, 0.5f);
            assemblyRT.anchoredPosition = new Vector2(-10, 0);
            assemblyRT.sizeDelta = new Vector2(300, 200);

            var assemblyBg = assemblyPanel.AddComponent<Image>();
            assemblyBg.color = new Color(0.4f, 0.8f, 0.6f, 0.9f);

            var assemblyTitle = new GameObject("Title");
            assemblyTitle.transform.SetParent(assemblyPanel.transform);
            var assemblyTitleText = assemblyTitle.AddComponent<TextMeshProUGUI>();
            assemblyTitleText.text = "Assembly Station";
            assemblyTitleText.fontSize = 24;
            assemblyTitleText.alignment = TextAlignmentOptions.Center;
            assemblyTitleText.color = Color.black;
            var assemblyTitleRT = assemblyTitleText.rectTransform;
            assemblyTitleRT.anchorMin = new Vector2(0, 1);
            assemblyTitleRT.anchorMax = new Vector2(1, 1);
            assemblyTitleRT.pivot = new Vector2(0.5f, 1);
            assemblyTitleRT.anchoredPosition = new Vector2(0, -20);
            assemblyTitleRT.sizeDelta = new Vector2(0, 40);

            var ingredientsText = new GameObject("IngredientsList");
            ingredientsText.transform.SetParent(assemblyPanel.transform);
            var ingListText = ingredientsText.AddComponent<TextMeshProUGUI>();
            ingListText.text = "No ingredients";
            ingListText.fontSize = 18;
            ingListText.alignment = TextAlignmentOptions.Center;
            ingListText.color = Color.black;
            var ingListRT = ingListText.rectTransform;
            ingListRT.anchorMin = new Vector2(0, 0.5f);
            ingListRT.anchorMax = new Vector2(1, 0.5f);
            ingListRT.pivot = new Vector2(0.5f, 0.5f);
            ingListRT.anchoredPosition = new Vector2(0, 0);
            ingListRT.sizeDelta = new Vector2(0, 60);

            var addMeatBtn = MakeButton("Add Meat", new Vector2(0.5f, 0.2f), new Vector2(-60, 0));
            addMeatBtn.transform.SetParent(assemblyPanel.transform);
            addMeatBtn.GetComponentInChildren<TextMeshProUGUI>().fontSize = 16;

            var wrapBtn = MakeButton("Wrap", new Vector2(0.5f, 0.2f), new Vector2(60, 0));
            wrapBtn.transform.SetParent(assemblyPanel.transform);
            wrapBtn.GetComponentInChildren<TextMeshProUGUI>().fontSize = 16;

            var assemblyScript = assemblyPanel.AddComponent<ShawarmaAssembly>();
            assemblyScript.GetType().GetField("slicer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(assemblyScript, slicerScript);
            assemblyScript.GetType().GetField("ingredientsListText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(assemblyScript, ingListText);
            assemblyScript.GetType().GetField("wrapButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(assemblyScript, wrapBtn);

            // Wire Add Meat button
            /*addMeatBtn.onClick.AddListener(() => {
                assemblyScript.AddMeatFromSlicer();
            });*/

            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}


