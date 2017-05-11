using System.Collections.Generic;
using System;
using UnityEngine;
using Cyber.Networking.Serverside;
using Cyber.Entities.SyncBases;
using Cyber.Console;

namespace Cyber.Entities {
    
    /// <summary>
    /// A database of the game's all syncable components. Syncable components are
    /// the instances of the subclasses of <see cref="SyncBase"/>.
    /// </summary>
    public class SyncDB : MonoBehaviour {
        
        private static readonly Type[] SyncableClasses = new Type[] {
            typeof(Character),
            typeof(Button),
            typeof(Door),
            typeof(Computer)
        };

        private int IDCounter = 0;
        private Dictionary<int, SyncBase> Database = new Dictionary<int, SyncBase>();
        private Dictionary<Type, List<int>> CategorizedDatabase = new Dictionary<Type, List<int>>();
        private Dictionary<Type, SyncHandletype> SyncHandletypes = new Dictionary<Type, SyncHandletype>();

        private List<int> StaticSyncBaseIDList = new List<int>();

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
                    Syncable.ID = ids[Index++];
                    AddSyncBaseToDatabase(Syncable);
                }
            }
        }

        /// <summary>
        /// Clears the given gameobject's <see cref="SyncBase"/>s from the Database.
        /// </summary>
        /// <param name="gameObject">The GameObject to remove.</param>
        public void RemoveEntity(GameObject gameObject) {
            for (int i = 0; i < SyncableClasses.Length; i++) {
                SyncBase Syncable = (SyncBase) gameObject.GetComponent(SyncableClasses[i]);
                if (Syncable != null) {
                    Database.Remove(Syncable.ID);
                    if (Server.IsRunning()) {
                        CategorizedDatabase[Syncable.GetType()].RemoveAll(x => x == Syncable.ID);
                    }
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
            if (!Database.ContainsKey(id)) {
                return null;
            }
            return Database[id];
        }

        /// <summary>
        /// Gives the database categorized into lists of their types.
        /// </summary>
        /// <returns>A dictionary of categorized SyncBases.</returns>
        public Dictionary<Type, List<int>> GetCategorizedDatabase() {
            return CategorizedDatabase;
        }

        /// <summary>
        /// Gets the Sync Handletypes currently known by the SyncDB.
        /// </summary>
        /// <returns>The Sync Handletypes by Type.</returns>
        public Dictionary<Type, SyncHandletype> GetSyncHandletypes() {
            return SyncHandletypes;
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

        /// <summary>
        /// Sets static objects for all objects in the world. This method should be called once per game launch ever.
        /// </summary>
        /// <param name="idList">The list of id's to be set. If null, will create new ids.</param>
        public void SetStaticObjectsIDs(int[] idList = null) {
            SyncBase[] SyncBases = GameObject.Find("/StaticWorld").GetComponentsInChildren<SyncBase>();
            Array.Sort(SyncBases, (a, b) => {
                Vector3 APos = a.gameObject.transform.position;
                float AComparison = APos.x * 677 + APos.y * 881 + APos.z * 313 + Array.IndexOf(SyncableClasses, a) * 463;
                Vector3 BPos = b.gameObject.transform.position;
                float BComparison = BPos.x * 677 + BPos.y * 881 + BPos.z * 313 + Array.IndexOf(SyncableClasses, b) * 463;

                return AComparison.CompareTo(BComparison);
            });
            if (idList == null) {
                foreach (SyncBase SyncBase in SyncBases) {
                    SyncBase.ID = CreateID();
                    AddSyncBaseToDatabase(SyncBase);
                    StaticSyncBaseIDList.Add(SyncBase.ID);
                }
            } else {
                for (int i = 0; i < Math.Min(SyncBases.Length, idList.Length); i++) {
                    SyncBases[i].ID = idList[i];
                    AddSyncBaseToDatabase(SyncBases[i]);
                }
            }
        }

        /// <summary>
        /// Returns the list of id's on the static objects that exist on the world by default.
        /// </summary>
        /// <returns></returns>
        public int[] GetStaticSyncBaseIDList() {
            return StaticSyncBaseIDList.ToArray();
        }

        private void AddSyncBaseToDatabase(SyncBase syncBase) {
            Database[syncBase.ID] = syncBase;
            if (Server.IsRunning()) {
                Type Type = syncBase.GetType();
                if (!CategorizedDatabase.ContainsKey(Type)) {
                    CategorizedDatabase.Add(Type, new List<int>());
                    SyncHandletypes.Add(Type, syncBase.GetSyncHandletype());
                }
                CategorizedDatabase[Type].Add(syncBase.ID);
            }
        }
    }
}