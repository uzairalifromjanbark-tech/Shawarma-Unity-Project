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

            EditorSceneManager.MarkAllScenesDirty();
        }
    }
}


