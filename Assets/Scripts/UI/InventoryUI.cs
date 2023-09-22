using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryObject;

    private Inventory _inventorySystem;

    private void Awake()
    {
        _inventorySystem = GetComponent<Inventory>();
    }

    public void OnInventoryButton()
    {
        _inventoryObject.SetActive(!_inventoryObject.activeSelf);

        if (_inventoryObject.activeSelf)
        {
            _inventorySystem.UpdateInventory();
        }
    }
}
