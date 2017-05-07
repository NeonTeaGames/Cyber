using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    public GameObject PCEntityPrefab;
    public GameObject NPCEntityPrefab;

    private int LastID = 0;
    private Dictionary<int, GameObject> SpawnedEntities = new Dictionary<int, GameObject>();

    /// <summary>
    /// Spawns an entity and returns that entity.
    /// </summary>
    /// <param name="type">Type.</param>
    /// <param name="position">Position.</param>
    public GameObject Spawn(EntityType type, Vector3 position) {
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
            int ID = CreateID();
            Spawned.GetComponent<SyncBase>().ID = ID;
            SpawnedEntities[ID] = Spawned;
        }
        return Spawned;
    }

    public GameObject Get(int id) {
        return SpawnedEntities[id];
    }

    private void Start() {
        Spawn(EntityType.PC, new Vector3());
    }

    private void Update() {
        if (Input.GetButtonDown("Jump")) {
            Spawn(EntityType.NPC, new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-2f, 2f)));
        }
    }

    private int CreateID() {
        return LastID++;
    }
}
