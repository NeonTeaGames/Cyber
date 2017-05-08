using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SyncDB : MonoBehaviour {
    private static readonly Type[] SyncableClasses = new Type[]{
        typeof(Character)
    };

    private uint IDCounter = 0;
    private Dictionary<uint, SyncBase> Database = new Dictionary<uint, SyncBase>();

    public void AddEntity(GameObject gameObject, uint[] ids) {
        int Index = 0;
        for (int i = 0; i < SyncableClasses.Length; i++) {
            SyncBase Syncable = (SyncBase) gameObject.GetComponent(SyncableClasses[i]);
            if (Syncable != null) {
                Syncable.ID = ids[Index];
                Database[ids[Index++]] = Syncable;
            }
        }
    }

    public uint[] GetEntityIDs(GameObject gameObject, bool newIDs = false) {
        List<uint> IDs = new List<uint>();
        for (int i = 0; i < SyncableClasses.Length; i++) {
            SyncBase Syncable = (SyncBase) gameObject.GetComponent(SyncableClasses[i]);
            if (Syncable != null) {
                if (newIDs) {
                    Syncable.ID = CreateID();
                }
                IDs.Add(Syncable.ID);
            }
        }
        uint[] IDArray = new uint[IDs.Count];
        for (int i = 0; i < IDs.Count; i++) {
            IDArray[i] = IDs[i];
        }
        return IDArray;
    }

    public uint[] GetNewEntityIDs(GameObject gameObject) {
        return GetEntityIDs(gameObject, true);
    }

    public SyncBase Get(uint id) {
        return Database[id];
    }

    public uint CreateID() {
        uint ID;
        try {
            ID = IDCounter++;
        } catch (OverflowException) {
            ID = 0;
            IDCounter = 1;
        }
        while (Database.ContainsKey(ID) && ID < uint.MaxValue) {
            ID++;
            if (ID < uint.MaxValue - 1) IDCounter = ID + 1;
        }
        if (Database.ContainsKey(ID)) {
            // Somehow we've managed to fill up the whole database.
            // I can't even imagine why or how.
            Debug.LogError("!!!WARNING!!! The SyncDB is full. Update the game to use longs instead of uints. !!!WARNING!!!");
        }
        return ID;
    }
}
