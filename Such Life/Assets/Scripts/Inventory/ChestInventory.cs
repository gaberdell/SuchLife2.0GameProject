using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* Base codes provided by: Dan Pos - Game Dev Tutorials! with slight modification */


[RequireComponent(typeof(UniqueID))]
public class ChestInventory : InventoryHolder, IInteractable
{
    public UnityAction<IInteractable> OnInteractionComplete { get; set; }
    protected override void Awake()
    {
        base.Awake();
        SaveLoad.onLoadGame += LoadInventory;
    }

    private void Start()
    {
        var chestSaveData = new ChestSaveData(primaryInvSystem, transform.position, transform.rotation);
        SaveGameManager.data.chestDictionaryData.Add(GetComponent<UniqueID>().ID, chestSaveData);

    }

    private void LoadInventory(SaveData data)
    {
        // Check the save data for this specific chest inventory, and if its exist, load it
        //Maybe add in the chest manager of some sort to load all of the chest in the chest dictionary into the world 
        if (data.chestDictionaryData.TryGetValue(GetComponent<UniqueID>().ID, out ChestSaveData chestData))
        {
            this.primaryInvSystem = chestData.invSystem;
            this.transform.position = chestData.position;
            this.transform.rotation = chestData.rotation;
        }
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        // if any is listening out on this event (hence the ?), if yes, invoke it
        OnDynamicInventoryDisplayRequested?.Invoke(primaryInvSystem);
        interactSuccessful = true;
    }

    //this method will be use for later if the player interact with the chest, they can't move until
    //they close the chest
    public void EndInteraction()
    {

    }
}


[System.Serializable]
public struct ChestSaveData
{
    public InventorySystem invSystem;

    public Vector3 position;
    public Quaternion rotation;

    public ChestSaveData(InventorySystem _invSystem, Vector3 pos, Quaternion rot)
    {
        invSystem = _invSystem;
        position = pos;
        rotation = rot;
    }
}