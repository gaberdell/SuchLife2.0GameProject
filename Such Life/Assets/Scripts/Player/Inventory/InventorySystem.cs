using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

/* Codes provided by: Dan Pos - Game Dev Tutorials! */

[System.Serializable]
public class InventorySystem : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> Inventory;
    //public int InventorySize { get; set; }

    //Inventory getter
    public List<InventorySlot> InventorySlots => Inventory;

    public UnityAction<InventorySlot> OnInventorySlotChanged;

    public int InventorySize { get => InventorySlots.Count; }
    //public int InventorySize {return InventorySlots.Count; };

    //OLD CODE TO RE WRITE : event that activate when we add an item into our inventory 
    //public UnityAction<InventorySlot> OnInventorySlotChanged;
    //event that activate when we add an item into our inventory 

    /* Constructor */
    public InventorySystem(int size)
    {
        Inventory = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            Inventory.Add(new InventorySlot());
        }


    }


    public bool AddToInventory(InventoryItemData itemToAdd, int amountToAdd)
    {
        //if item exist in inventory
        if(ContainsItem(itemToAdd, out List<InventorySlot> invSlot))
        {
            //add item in and change inventory
            foreach (var emptySlot in invSlot)
            {
                //check to see if there are still room left in the slot
                if (emptySlot.IsEnoughSpaceInStack(amountToAdd))
                {
                    emptySlot.AddToStack(amountToAdd);
                    OnInventorySlotChanged?.Invoke(emptySlot);
                    //EventManager.UpdateInventorySlot(this, emptySlot);
                    return true;
                }
            }
        }
        
        
        if(HasFreeSlot(out InventorySlot freeSlot)) //Gets the first available slot
        {
            if (freeSlot.IsEnoughSpaceInStack(amountToAdd))
            {
                //add item into available slot
                freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                OnInventorySlotChanged?.Invoke(freeSlot);
                //EventManager.UpdateInventorySlot(this, freeSlot);
                return true;
            }
            /* implement such that to only take what can fill the stack and check for another free slot 
            to put the rest of the item in */
        }
        return false;
    }

    public bool ContainsItem(InventoryItemData itemToAdd, out List<InventorySlot> invSlot)
    {
        //check all inventory slot and create a list
        // i is just any item and Where() is just finding where is 
        //item i located at and put it in a list
        invSlot = InventorySlots.Where(i => i.ItemData == itemToAdd).ToList();
        return invSlot == null ? false : true; //if there are still inventory slot, return true, else return false
    }

    public bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = InventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot == null ? false : true;
    }

    public SavableSlot[] ToSavabaleSlots()
    {
        SavableSlot[] returnArray = new SavableSlot[InventorySlots.Count];

        for (int i = 0; i < InventorySlots.Count; i++)
        {
            returnArray[i].amount = InventorySlots[i].StackSize;
            returnArray[i].itemData = InventorySlots[i].ItemData;
        }

        return returnArray;
    }

    public void WriteInSavableSlots(SavableSlot[] savableSlots)
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            InventorySlots[i].UpdateInventorySlot(savableSlots[i].itemData, savableSlots[i].amount);
        }
    }
}

[System.Serializable]
public struct InventorySaveData
{
    public InventorySystem InvSystem;

    public Vector3 Position;
    public Quaternion Rotation;

    public InventorySaveData(InventorySystem _invSystem, Vector3 pos, Quaternion rot)
    {
        InvSystem = _invSystem;
        Position = pos;
        Rotation = rot;
    }

    public InventorySaveData(InventorySystem _invSystem)
    {
        InvSystem = _invSystem;
        Position = Vector3.zero;
        Rotation = Quaternion.identity;
    }
}
