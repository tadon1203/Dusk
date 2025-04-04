using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Dusk.Modules.Visual;

public class MenuModule : BaseModule
{
    private GameObject _menuCanvas;
    private ScrollRect _scrollView;
    private ToggleGroup _toggleGroup;
    
    public MenuModule() : base("Menu", "Shows a menu to manage Dusk", ModuleType.Visual, KeyCode.Insert) {}

    public override void OnEnable()
    {
        CreateMenuCanvas();
    }

    public override void OnDisable()
    {
        if (_menuCanvas != null)
        {
            UnityEngine.Object.Destroy(_menuCanvas);
        }
    }
    
    private void CreateMenuCanvas()
    {
        // Create Canvas
        _menuCanvas = new GameObject("Dusk Menu Canvas");
        var canvas = _menuCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _menuCanvas.AddComponent<CanvasScaler>();
        _menuCanvas.AddComponent<GraphicRaycaster>();

        // Create background panel
        var panel = new GameObject("Menu Panel");
        panel.transform.SetParent(_menuCanvas.transform);
        var panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(300, 500);
        panelRect.anchoredPosition = Vector2.zero;
        
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        // Add title
        var title = new GameObject("Title");
        title.transform.SetParent(panel.transform);
        var titleText = title.AddComponent<Text>();
        titleText.text = "Dusk Menu";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontSize = 24;
        
        var titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 40);
        titleRect.anchoredPosition = new Vector2(0, -20);

        // Create scroll view
        var scrollView = new GameObject("Scroll View");
        scrollView.transform.SetParent(panel.transform);
        _scrollView = scrollView.AddComponent<ScrollRect>();
        
        var scrollRect = scrollView.GetComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.sizeDelta = new Vector2(-20, -60);
        scrollRect.anchoredPosition = new Vector2(0, -30);

        // Create viewport
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform);
        var viewportImage = viewport.AddComponent<Image>();
        viewportImage.color = new Color(0.15f, 0.15f, 0.15f, 0.5f);
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.anchoredPosition = Vector2.zero;

        _scrollView.viewport = viewportRect;

        // Create content
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform);
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0.5f, 1);
        contentRect.anchorMax = new Vector2(0.5f, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(260, 0);
        contentRect.anchoredPosition = Vector2.zero;

        _scrollView.content = contentRect;
        _toggleGroup = content.AddComponent<ToggleGroup>();

        // Add scrollbar
        var scrollbar = new GameObject("Scrollbar");
        scrollbar.transform.SetParent(scrollView.transform);
        var scrollbarComp = scrollbar.AddComponent<Scrollbar>();
        scrollbarComp.direction = Scrollbar.Direction.BottomToTop;
        
        var scrollbarRect = scrollbar.GetComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.sizeDelta = new Vector2(20, 0);
        scrollbarRect.anchoredPosition = Vector2.zero;

        _scrollView.verticalScrollbar = scrollbarComp;

        // Create handle for scrollbar
        var handle = new GameObject("Handle");
        handle.transform.SetParent(scrollbar.transform);
        handle.AddComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        
        var handleRect = handle.GetComponent<RectTransform>();
        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = new Vector2(1, 1);
        handleRect.sizeDelta = Vector2.zero;
        handleRect.anchoredPosition = Vector2.zero;

        scrollbarComp.handleRect = handleRect;

        // Populate with modules
        PopulateModuleList(contentRect);
    }

    private void PopulateModuleList(RectTransform parent)
    {
        float yOffset = -10f;
        float itemHeight = 60f;
        
        foreach (var module in ModuleManager.GetModules().OrderBy(m => m.Name))
        {
            if (module == this) continue;

            // Create module item
            var item = new GameObject(module.Name + " Toggle");
            item.transform.SetParent(parent);
            
            var itemRect = item.AddComponent<RectTransform>();
            itemRect.anchorMin = new Vector2(0.5f, 1);
            itemRect.anchorMax = new Vector2(0.5f, 1);
            itemRect.pivot = new Vector2(0.5f, 1);
            itemRect.sizeDelta = new Vector2(260, itemHeight);
            itemRect.anchoredPosition = new Vector2(0, yOffset);
            
            // Add background
            var bg = new GameObject("Background");
            bg.transform.SetParent(item.transform);
            var bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.7f);
            
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.sizeDelta = Vector2.zero;
            bgRect.anchoredPosition = Vector2.zero;

            // Add toggle
            var toggle = item.AddComponent<Toggle>();
            toggle.isOn = module.Enabled;
            toggle.group = _toggleGroup;
            toggle.onValueChanged.AddListener(new Action<bool>(value => 
            {
                if (module.Enabled != value)
                {
                    module.Toggle();
                }
            }));
            
            // Add checkmark
            var check = new GameObject("Checkmark");
            check.transform.SetParent(item.transform);
            var checkImage = check.AddComponent<Image>();
            checkImage.color = Color.green;
            
            var checkRect = check.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0, 0.5f);
            checkRect.anchorMax = new Vector2(0, 0.5f);
            checkRect.sizeDelta = new Vector2(20, 20);
            checkRect.anchoredPosition = new Vector2(10, 0);

            toggle.graphic = checkImage;

            // Add module name
            var nameText = new GameObject("Name");
            nameText.transform.SetParent(item.transform);
            var textComp = nameText.AddComponent<Text>();
            textComp.text = module.Name;
            textComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComp.color = Color.white;
            textComp.alignment = TextAnchor.MiddleLeft;
            textComp.fontSize = 16;
            
            var nameRect = nameText.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0);
            nameRect.anchorMax = new Vector2(1, 0.5f);
            nameRect.sizeDelta = new Vector2(-40, 0);
            nameRect.anchoredPosition = new Vector2(40, 0);

            // Add description
            var descText = new GameObject("Description");
            descText.transform.SetParent(item.transform);
            var descComp = descText.AddComponent<Text>();
            descComp.text = module.Description;
            descComp.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            descComp.color = new Color(0.8f, 0.8f, 0.8f);
            descComp.alignment = TextAnchor.UpperLeft;
            descComp.fontSize = 12;
            
            var descRect = descText.GetComponent<RectTransform>();
            descRect.anchorMin = new Vector2(0, 0.5f);
            descRect.anchorMax = new Vector2(1, 1);
            descRect.sizeDelta = new Vector2(-40, 0);
            descRect.anchoredPosition = new Vector2(40, 0);

            yOffset -= itemHeight + 5f;
        }

        // Update content height
        parent.sizeDelta = new Vector2(parent.sizeDelta.x, -yOffset + 10f);
    }
}