using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public SyncDB SyncDB;
    public GameObject PCEntityPrefab;
    public GameObject NPCEntityPrefab;

    /// <summary>
    /// Spawns an entity and returns that entity.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="position">Position.</param>
    public GameObject Spawn(EntityType type, Vector3 position, uint[] ids) {
        GameObject Spawned = null;
        switch (type) {
        case EntityType.PC:
            Spawned = Instantiate(PCEntityPrefab, position, new Quaternion(), transform);
            break; 
        case EntityType.NPC:
            Spawned = Instantiate(NPCEntityPrefab, position, new Quaternion(), transform);
            break;
        }
        if (Spawned != null) {
            SyncDB.AddEntity(Spawned, ids);
        }
        return Spawned;
    }

    private void Start() {
        Spawn(EntityType.PC, new Vector3(), new uint[]{ SyncDB.CreateID() });
    }

    private void Update() {
        if (Input.GetButtonDown("Jump") && !Term.IsVisible()) {
            Spawn(EntityType.NPC, new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-2f, 2f)), new uint[]{ SyncDB.CreateID() });
        }
    }
}
