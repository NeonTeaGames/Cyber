using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Cyber.Entities {
    
    /// <summary>
    /// A database of the game's all syncable components. Syncable components are
    /// the instances of the subclasses of <see cref="SyncBase"/>.
    /// </summary>
    public class SyncDB : MonoBehaviour {
        
        private static readonly Type[] SyncableClasses = new Type[] {
            typeof(Character)
        };

        private int IDCounter = 0;
        private Dictionary<int, SyncBase> Database = new Dictionary<int, SyncBase>();

        /// <summary>
        /// Add an entity to the database with the given IDs.
        /// </summary>
        /// <param name="gameObject">Game object.</param>
        /// <param name="ids">The IDs. Should be from <see cref="GetEntityIDs"/> or
        /// <see cref="GetNewEntityIDs"/>, since the order is important.</param>
        public void AddEntity(GameObject gameObject, int[] ids) {
            int Index = 0;
            for (int i = 0; i < SyncableClasses.Length; i++) {
                SyncBase Syncable = (SyncBase)gameObject.GetComponent(SyncableClasses[i]);
                if (Syncable != null) {
                    Syncable.ID = ids[Index];
                    Database[ids[Index++]] = Syncable;
                }
            }
        }

        /// <summary>
        /// Makes an ordered list of the given gameobject's syncable components' 
        /// IDs.
        /// </summary>
        /// <returns>The IDs.</returns>
        /// <param name="gameObject">Game object.</param>
        /// <param name="newIDs">Whether or not new IDs are created. 
        /// <see cref="GetNewEntityIDs"/> is a shorthand for this function with 
        /// this parameter set to true.</param>
        public int[] GetEntityIDs(GameObject gameObject, bool newIDs = false) {
            List<int> IDs = new List<int>();
            for (int i = 0; i < SyncableClasses.Length; i++) {
                SyncBase Syncable = (SyncBase)gameObject.GetComponent(SyncableClasses[i]);
                if (Syncable != null) {
                    if (newIDs) {
                        Syncable.ID = CreateID();
                    }
                    IDs.Add(Syncable.ID);
                }
            }
            int[] IDArray = new int[IDs.Count];
            for (int i = 0; i < IDs.Count; i++) {
                IDArray[i] = IDs[i];
            }
            return IDArray;
        }

        /// <summary>
        /// Creates an ordered list of the given gameobject's syncable components'
        /// IDs. See <see cref="GetEntityIDs"/> for more information.
        /// </summary>
        /// <returns>The new IDs.</returns>
        /// <param name="gameObject">Game object.</param>
        public int[] GetNewEntityIDs(GameObject gameObject) {
            return GetEntityIDs(gameObject, true);
        }

        /// <summary>
        /// Get a synced component by its ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        public SyncBase Get(int id) {
            return Database[id];
        }

        /// <summary>
        /// Creates a new ID which isn't in use yet.
        /// </summary>
        /// <returns>A new, free ID.</returns>
        public int CreateID() {
            int ID;
            try {
                ID = IDCounter++;
            } catch (OverflowException) {
                ID = 0;
                IDCounter = 1;
            }
            while (Database.ContainsKey(ID) && ID < int.MaxValue) {
                ID++;
                if (ID < int.MaxValue - 1)
                    IDCounter = ID + 1;
            }
            if (Database.ContainsKey(ID)) {
                // Somehow we've managed to fill up the whole database.
                // I can't even imagine why or how.
                Debug.LogError("!!!WARNING!!! The SyncDB is full. Update the game to use longs instead of uints. !!!WARNING!!!");
            }
            return ID;
        }
    }
}