using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _inventoryMainObject;
    [SerializeField] private RectTransform _movingObjectTransform;
    [SerializeField] private GameObject _deletePanel;

    [SerializeField] private Camera _camera;
    [SerializeField] private EventSystem _eventSystem;

    private InventoryData _dataBase;

    public List<ItemInventory> Items;
    private int _inventoryCapacity;

    private ItemInventory _currentItem;
    private int _currentID;

    private Vector3 _movingObjectOffset;

    private bool _isSwapModeOn;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }

        _dataBase = GetComponent<InventoryData>();

        Items = new();
        _inventoryCapacity = 30;

        _currentID = -1;

        _movingObjectOffset = new Vector3(1, -1, 0);
    }

    private void Start()
    {
        if(Items.Count == 0)
        {
            AddGraphics();
        }
    }

    private void Update()
    {
        if(_currentID != -1)
        {
            MoveItem();
        }
    }

    private void AddGraphics()
    {
        for(int i = 0; i < _inventoryCapacity; i++)
        {
            GameObject newItem = Instantiate(_itemPrefab, _inventoryMainObject.transform) as GameObject;

            newItem.name = i.ToString();

            ItemInventory ii = new();
            ii.ItemGameObject = newItem;

            RectTransform rt = newItem.GetComponent<RectTransform>();
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;
            newItem.GetComponentInChildren<RectTransform>().localScale = Vector3.one;

            Button tempButton = newItem.GetComponent<Button>();

            tempButton.onClick.AddListener(delegate { ToggleItemMode(_isSwapModeOn); });

            Items.Add(ii);
        }
    }

    public void UpdateInventory()
    {
        for(int i = 0; i < _inventoryCapacity; i++)
        {
            int itemInventoryId = Items[i].Id;

            if (itemInventoryId != 0 && Items[i].Count > 1)
            {
                Items[i].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = Items[i].Count.ToString();
            }
            else
            {
                Items[i].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

            Items[i].ItemGameObject.GetComponent<Image>().sprite = _dataBase.Items[itemInventoryId].Image;
        }
    }

    public void SearchForSameItem(Item item, int count)
    {
        for (int i = 0; i < _inventoryCapacity; i++)
        {
            if (Items[i].Id == item.Id)
            {
                if (Items[0].Count < 128)
                {
                    Items[i].Count += count;

                    if (Items[i].Count > 128)
                    {
                        count = Items[i].Count - 128;
                        Items[i].Count -= count;
                    }
                    else
                    {
                        count = 0;
                        i = _inventoryCapacity;
                    }
                }
            }
        }

        PlaceItemInEmptyCell(item, count);
    }

    private void PlaceItemInEmptyCell(Item item, int count)
    {
        if (count > 0)
        {
            for (int i = 0; i < _inventoryCapacity; i++)
            {
                if (Items[i].Id == 0)
                {
                    AddItem(i, item, count);
                    i = _inventoryCapacity;
                }
            }
        }

        UpdateInventory();
    }

    private void MoveItem()
    {
        Vector3 pos = Input.mousePosition + _movingObjectOffset;
        pos.z = _inventoryMainObject.GetComponent<RectTransform>().position.z;
        _movingObjectTransform.position = _camera.ScreenToWorldPoint(pos);
    }

    public void ToggleSwapMode(bool isSwapModeOn)
    {
        _isSwapModeOn = isSwapModeOn;
    }

    private void ToggleItemMode(bool isSwapModeOn)
    {
        if (isSwapModeOn)
        {
            SwapItemsMode();
        }
        else
        {
            DeleteItemMode();
        }
    }

    private void SwapItemsMode()
    {
        if (_currentID == -1)
        {
            _currentID = int.Parse(_eventSystem.currentSelectedGameObject.name);

            if (Items[_currentID].Id == 0)
            {
                _currentID = -1;
                return;
            }

            _currentItem = CopyInventoryItem(Items[_currentID]);

            _movingObjectTransform.gameObject.SetActive(true);
            _movingObjectTransform.GetComponent<Image>().sprite = _dataBase.Items[_currentItem.Id].Image;

            AddItem(_currentID, _dataBase.Items[0], 0);
        }
        else
        {
            ItemInventory itemInventory = Items[int.Parse(_eventSystem.currentSelectedGameObject.name)];

            if (_currentItem.Id != itemInventory.Id)
            {
                AddInventoryItem(_currentID, itemInventory);
                AddInventoryItem(int.Parse(_eventSystem.currentSelectedGameObject.name), _currentItem);
            }
            else
            {
                if (itemInventory.Count + _currentItem.Count <= 128)
                {
                    itemInventory.Count += _currentItem.Count;
                }
                else
                {
                    AddItem(_currentID, _dataBase.Items[itemInventory.Id], itemInventory.Count + _currentItem.Count - 128);
                    itemInventory.Count = 128;
                }

                if (itemInventory.Count > 1)
                {
                    itemInventory.ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemInventory.Count.ToString();
                }
            }

            _currentID = -1;

            _movingObjectTransform.gameObject.SetActive(false);
        }
    }

    private void AddItem(int id, Item item, int count)
    {
        Items[id].Id = item.Id;
        Items[id].Count = count;
        Items[id].ItemGameObject.GetComponent<Image>().sprite = item.Image;

        if (item.Id != 0 && count > 1)
        {
            Items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        }
        else
        {
            Items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    private void AddInventoryItem(int id, ItemInventory invItem)
    {
        Items[id].Id = invItem.Id;
        Items[id].Count = invItem.Count;
        Items[id].ItemGameObject.GetComponent<Image>().sprite = _dataBase.Items[invItem.Id].Image;

        if (invItem.Id != 0 && invItem.Count > 1)
        {
            Items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = invItem.Count.ToString();
        }
        else
        {
            Items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    private void DeleteItemMode()
    {
        _currentID = int.Parse(_eventSystem.currentSelectedGameObject.name);

        if (Items[_currentID].Id != 0)
        {
            _deletePanel.SetActive(true);
        }
        else
        {
            _currentID = -1;
        }
    }

    public void DeleteItemFromInventory(bool isDeleting)
    {
        if (isDeleting)
        {
            AddItem(_currentID, _dataBase.Items[0], 0);
        }

        _currentID = -1;

        _deletePanel.SetActive(false);
    }

    public bool DeleteAmountOfItem(int itemId, int itemAmount)
    {
        for (int i = 0; i < _inventoryCapacity; i++)
        {
            if (Items[i].Id == itemId)
            {
                Items[i].Count -= itemAmount;

                if (Items[i].Count == 0)
                {
                    AddItem(i, _dataBase.Items[0], 0);
                }

                UpdateInventory();

                return true;
            }
        }

        return false;
    }

    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory newItem = new();

        newItem.Id = old.Id;
        newItem.ItemGameObject = old.ItemGameObject;
        newItem.Count = old.Count;

        return newItem;
    }
}


[System.Serializable]
public class ItemInventory
{
    public int Id;
    public GameObject ItemGameObject;

    public int Count;
}



