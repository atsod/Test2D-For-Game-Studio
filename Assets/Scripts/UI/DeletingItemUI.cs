using UnityEngine;

public class DeletingItemUI : MonoBehaviour
{
    public void DeleteYes()
    {
        GetComponent<Inventory>().DeleteItemFromInventory(true);
    }

    public void DeleteNo()
    {
        GetComponent<Inventory>().DeleteItemFromInventory(false);
    }
}
