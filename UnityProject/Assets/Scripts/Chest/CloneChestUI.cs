using System.Collections.Generic;
using Clone.Items;
using Clone.Player;
using Clone.World;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Clone.ChestUI
{
    public class CloneChestUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject panel;

        [SerializeField]
        private Transform slotContainer;

        [SerializeField]
        private GameObject slotPrefab;

        [SerializeField]
        private Button storeSelectedButton;

        [SerializeField]
        private Text statusText;

        private readonly List<CloneChestSlotUI> slotUIs = new List<CloneChestSlotUI>();
        private CloneChest boundChest;

        private void Awake()
        {
            BuildSlots();

            if (panel != null)
                panel.SetActive(false);

            if (storeSelectedButton != null)
                storeSelectedButton.onClick.AddListener(OnStoreSelectedClicked);
        }

        private void OnDestroy()
        {
            UnbindChest();

            if (storeSelectedButton != null)
                storeSelectedButton.onClick.RemoveListener(OnStoreSelectedClicked);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
                Toggle();
            else if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
                Close();
        }

        public void Toggle()
        {
            if (panel != null && panel.activeSelf)
            {
                Close();
                return;
            }

            var player = ClonePlayer.LocalPlayer;
            if (player == null)
                return;

            if (!CloneChest.TryGetNearbyChest(player, CloneChest.DefaultReach, out CloneChest chest))
            {
                SetStatus("No chest nearby");
                return;
            }

            BindChest(chest);
            chest.ServerRequestOpenChest();

            if (panel != null)
                panel.SetActive(true);

            SetStatus("Shared chest");
            RefreshSlots();
        }

        public void Close()
        {
            UnbindChest();

            if (panel != null)
                panel.SetActive(false);

            SetStatus(string.Empty);
        }

        public void OnStoreSelectedClicked()
        {
            var inventory = ClonePlayer.LocalPlayer != null ? ClonePlayer.LocalPlayer.Inventory : null;
            if (inventory == null || boundChest == null)
                return;

            boundChest.ServerRequestStore(inventory.SelectedSlot, 999999);
        }

        public void RequestWithdraw(int slotIndex, int amount)
        {
            if (boundChest == null)
                return;

            boundChest.ServerRequestWithdraw(slotIndex, amount);
        }

        private void BindChest(CloneChest chest)
        {
            if (chest == null)
                return;

            if (boundChest == chest)
                return;

            UnbindChest();
            boundChest = chest;
            chest.ChestOpened += OnChestOpened;
            chest.ChestStored += OnChestStored;
            chest.ChestWithdrawn += OnChestWithdrawn;
            chest.ChestStateRefreshed += OnChestStateRefreshed;
        }

        private void UnbindChest()
        {
            if (boundChest == null)
                return;

            boundChest.ChestOpened -= OnChestOpened;
            boundChest.ChestStored -= OnChestStored;
            boundChest.ChestWithdrawn -= OnChestWithdrawn;
            boundChest.ChestStateRefreshed -= OnChestStateRefreshed;
            boundChest = null;
        }

        private void OnChestOpened()
        {
            SetStatus("Shared chest opened");
            RefreshSlots();
        }

        private void OnChestStored(int slot, int itemId, int amount)
        {
            SetStatus("Stored " + amount + " x " + ResolveItemName(itemId));
            RefreshSlots();
        }

        private void OnChestWithdrawn(int slot, int itemId, int amount)
        {
            SetStatus("Withdrew " + amount + " x " + ResolveItemName(itemId));
            RefreshSlots();
        }

        private void OnChestStateRefreshed()
        {
            RefreshSlots();
        }

        private void RefreshSlots()
        {
            for (int i = 0; i < slotUIs.Count; i++)
            {
                var slotUI = slotUIs[i];
                if (slotUI != null)
                    slotUI.Refresh(boundChest);
            }
        }

        private static string ResolveItemName(int itemId)
        {
            if (itemId == CloneInventorySlot.EmptyId)
                return string.Empty;

            var definition = CloneItemCatalog.Get(itemId);
            return definition != null ? definition.Name : string.Empty;
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }

        // Runtime-built chest panel: no scene objects or prefabs required.
        // Created once for the owning player; [E] toggles the window.
        public static void EnsureFor()
        {
            if (FindFirstObjectByType<CloneChestUI>() != null)
                return;

            GameObject root = new GameObject("CloneChestCanvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.SetActive(false);

            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;

            CanvasScaler scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            if (Object.FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("CloneChestEventSystem",
                    typeof(EventSystem), typeof(StandaloneInputModule));
                eventSystem.transform.SetParent(root.transform, false);
            }

            CloneChestUI ui = root.AddComponent<CloneChestUI>();
            BuildPanel(ui, root.transform);

            root.SetActive(true);
        }

        private static void BuildPanel(CloneChestUI ui, Transform canvasTransform)
        {
            GameObject panel = new GameObject("ChestPanel", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(canvasTransform, false);
            panel.SetActive(false);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0.06f, 0.06f, 0.08f, 0.95f);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.sizeDelta = new Vector2(640f, 420f);

            Text title = CreateText(panel.transform, "ChestTitle", 20, TextAnchor.MiddleCenter);
            title.text = "Shared Chest";
            Stretch(title, new Vector2(16f, -44f), new Vector2(-16f, -10f));

            ui.statusText = CreateText(panel.transform, "ChestStatus", 13, TextAnchor.MiddleLeft);
            ui.statusText.color = new Color(0.8f, 0.8f, 0.8f);
            Stretch(ui.statusText, new Vector2(16f, -70f), new Vector2(-16f, -46f));

            GameObject grid = new GameObject("ChestSlots", typeof(RectTransform), typeof(GridLayoutGroup));
            grid.transform.SetParent(panel.transform, false);
            RectTransform gridRect = grid.GetComponent<RectTransform>();
            gridRect.anchorMin = new Vector2(0f, 1f);
            gridRect.anchorMax = new Vector2(1f, 1f);
            gridRect.offsetMin = new Vector2(16f, -330f);
            gridRect.offsetMax = new Vector2(-16f, -74f);
            GridLayoutGroup layout = grid.GetComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(80f, 54f);
            layout.spacing = new Vector2(6f, 6f);
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = 7;
            layout.childAlignment = TextAnchor.UpperCenter;
            ui.slotContainer = grid.transform;

            for (int i = 0; i < CloneChest.ChestSlotCount; i++)
            {
                GameObject slot = new GameObject("Slot" + i, typeof(RectTransform), typeof(Image), typeof(Button));
                slot.transform.SetParent(grid.transform, false);

                Image slotImage = slot.GetComponent<Image>();
                slotImage.color = new Color(0.15f, 0.15f, 0.18f, 0.9f);

                Text itemName = CreateText(slot.transform, "ItemName", 13, TextAnchor.MiddleCenter);
                Stretch(itemName, new Vector2(6f, 4f), new Vector2(-6f, -4f));

                Text amount = CreateText(slot.transform, "Amount", 12, TextAnchor.LowerRight);
                Stretch(amount, new Vector2(6f, 4f), new Vector2(-6f, -4f));

                CloneChestSlotUI slotUI = slot.AddComponent<CloneChestSlotUI>();
                slotUI.Bind(ui, i);
                ui.slotUIs.Add(slotUI);
            }

            GameObject storeButton = new GameObject("StoreSelected", typeof(RectTransform), typeof(Image), typeof(Button));
            storeButton.transform.SetParent(panel.transform, false);
            Image storeImage = storeButton.GetComponent<Image>();
            storeImage.color = new Color(0.2f, 0.35f, 0.2f, 0.95f);
            RectTransform storeRect = storeButton.GetComponent<RectTransform>();
            storeRect.anchorMin = new Vector2(0f, 0f);
            storeRect.anchorMax = new Vector2(1f, 0f);
            storeRect.offsetMin = new Vector2(16f, 12f);
            storeRect.offsetMax = new Vector2(-16f, 52f);
            Text storeText = CreateText(storeButton.transform, "StoreLabel", 15, TextAnchor.MiddleCenter);
            storeText.text = "Store Selected Item";
            Stretch(storeText, new Vector2(8f, 2f), new Vector2(-8f, -2f));
            ui.storeSelectedButton = storeButton.GetComponent<Button>();

            ui.panel = panel;
        }

        private static Text CreateText(Transform parent, string name, int fontSize, TextAnchor alignment)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return text;
        }

        private static void Stretch(Text text, Vector2 offsetMin, Vector2 offsetMax)
        {
            RectTransform rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private void BuildSlots()
        {
            if (slotContainer == null || slotPrefab == null)
                return;

            for (int i = 0; i < CloneChest.ChestSlotCount; i++)
            {
                GameObject slotObject = Instantiate(slotPrefab, slotContainer);
                var slotUI = slotObject.GetComponent<CloneChestSlotUI>();
                if (slotUI != null)
                {
                    slotUI.Bind(this, i);
                    slotUIs.Add(slotUI);
                }
            }
        }
    }

    public class CloneChestSlotUI : MonoBehaviour
    {
        [SerializeField]
        private Text itemNameText;

        [SerializeField]
        private Text amountText;

        private CloneChestUI owner;
        private int slotIndex = -1;

        public int SlotIndex => slotIndex;

        public void Bind(CloneChestUI newOwner, int newSlotIndex)
        {
            owner = newOwner;
            slotIndex = newSlotIndex;
            Button button = GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(ClickSlot);
            }
        }

        public void Refresh(CloneChest chest)
        {
            if (chest == null)
            {
                Clear();
                return;
            }

            int itemId = chest.GetViewItemId(slotIndex);
            int amount = chest.GetViewAmount(slotIndex);

            if (itemId == CloneInventorySlot.EmptyId)
            {
                Clear();
                return;
            }

            var definition = CloneItemCatalog.Get(itemId);

            if (itemNameText != null)
                itemNameText.text = definition != null ? definition.Name : string.Empty;

            if (amountText != null)
                amountText.text = amount > 1 ? amount.ToString() : string.Empty;

            var button = GetComponent<Button>();
            if (button != null)
                button.interactable = true;
        }

        public void Clear()
        {
            if (itemNameText != null)
                itemNameText.text = string.Empty;

            if (amountText != null)
                amountText.text = string.Empty;
        }

        public void ClickSlot()
        {
            if (owner != null && slotIndex >= 0)
                owner.RequestWithdraw(slotIndex, 999999);
        }
    }
}
