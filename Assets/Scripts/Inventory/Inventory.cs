using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _inventoryMainObject;
    [SerializeField] private RectTransform _movingObjectTransform;

    [SerializeField] private Camera _camera;
    [SerializeField] private EventSystem _eventSystem;

    private InventoryData _dataBase;

    private bool _isTransparentModeOn;

    private List<ItemInventory> _items = new ();
    private int _inventoryCapacity;

    private ItemInventory _currentItem;
    private int _currentID;

    private Vector3 _movingObjectOffset;

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

        _items = new();
        _inventoryCapacity = 45;

        _currentID = -1;

        _movingObjectOffset = new Vector3(1, -1, 0);
    }

    private void Start()
    {
        if(_items.Count == 0)
        {
            AddGraphics();
        }

        // тестовое заполнение предметами
        /*for(int i = 0; i < _inventoryCapacity; i++)
        {
            AddItem(i, _dataBase.Items[Random.Range(0, _dataBase.Items.Count)], Random.Range(1, 100));
        }
        UpdateInventory();*/
    }

    private void Update()
    {
        if(_currentID != -1)
        {
            MoveObject();
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

            tempButton.onClick.AddListener(delegate { SelectObject(); });

            _items.Add(ii);
        }
    }

    public void UpdateInventory()
    {
        for(int i = 0; i < _inventoryCapacity; i++)
        {
            int itemInventoryId = _items[i].Id;

            if (itemInventoryId != 0 && _items[i].Count > 1)
            {
                _items[i].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = _items[i].Count.ToString();
            }
            else
            {
                _items[i].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }

            _items[i].ItemGameObject.GetComponent<Image>().sprite = _dataBase.Items[itemInventoryId].Image;
        }
    }

    public void SearchForSameItem(Item item, int count)
    {
        for (int i = 0; i < _inventoryCapacity; i++)
        {
            if (_items[i].Id == item.Id)
            {
                if (_items[0].Count < 128)
                {
                    _items[i].Count += count;

                    if (_items[i].Count > 128)
                    {
                        count = _items[i].Count - 128;
                        _items[i].Count -= count;
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
                if (_items[i].Id == 0)
                {
                    AddItem(i, item, count);
                    i = _inventoryCapacity;
                }
            }
        }

        UpdateInventory();
    }

    public void SelectObject()
    {
        Debug.Log(_currentID);
        if (_currentID == -1)
        {
            _currentID = int.Parse(_eventSystem.currentSelectedGameObject.name);
            _currentItem = CopyInventoryItem(_items[_currentID]);

            _movingObjectTransform.gameObject.SetActive(true);
            _movingObjectTransform.GetComponent<Image>().sprite = _dataBase.Items[_currentItem.Id].Image;

            AddItem(_currentID, _dataBase.Items[0], 0);
        }
        else
        {
            ItemInventory itemInventory = _items[int.Parse(_eventSystem.currentSelectedGameObject.name)];

            if(_currentItem.Id != itemInventory.Id) 
            {
                AddInventoryItem(_currentID, itemInventory);
                AddInventoryItem(int.Parse(_eventSystem.currentSelectedGameObject.name), _currentItem);
            }
            else
            {
                if(itemInventory.Count + _currentItem.Count <= 128)
                {
                    itemInventory.Count += _currentItem.Count;
                }
                else
                {
                    AddItem(_currentID, _dataBase.Items[itemInventory.Id], itemInventory.Count + _currentItem.Count - 128);
                    itemInventory.Count = 128;
                }

                if(itemInventory.Count > 1)
                {
                    itemInventory.ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = itemInventory.Count.ToString();
                }
            }

            _currentID = -1;

            _movingObjectTransform.gameObject.SetActive(false);
        }
    }

    private void MoveObject()
    {
        Vector3 pos = Input.mousePosition + _movingObjectOffset;
        pos.z = _inventoryMainObject.GetComponent<RectTransform>().position.z;
        _movingObjectTransform.position = _camera.ScreenToWorldPoint(pos);
    }

    public void AddItem(int id, Item item, int count)
    {
        _items[id].Id = item.Id;
        _items[id].Count = count;
        _items[id].ItemGameObject.GetComponent<Image>().sprite = item.Image;

        if (item.Id != 0 && count > 1)
        {
            _items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = count.ToString();
        }
        else
        {
            _items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public void AddInventoryItem(int id, ItemInventory invItem)
    {
        _items[id].Id = invItem.Id;
        _items[id].Count = invItem.Count;
        _items[id].ItemGameObject.GetComponent<Image>().sprite = _dataBase.Items[invItem.Id].Image;

        if (invItem.Id != 0 && invItem.Count > 1)
        {
            _items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = invItem.Count.ToString();
        }
        else
        {
            _items[id].ItemGameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public ItemInventory CopyInventoryItem(ItemInventory old)
    {
        ItemInventory newItem = new ();

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
