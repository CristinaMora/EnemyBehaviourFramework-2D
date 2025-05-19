using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Holds data for one spawn point and the prefab to spawn there
public class SpawnInfo
{
    [SerializeField]
    public GameObject _prefabToSpawn; //what

    [SerializeField]
    public Transform _spawnPoint;  //where
}

public class SpawnerActuator : Actuator
{
    [SerializeField, Min(0)]
    private float _spawnInterval = 5f; // Time interval between spawns

    [SerializeField]
    private bool _infiniteEnemies = true; // If true, enemies will spawn indefinitely

    [SerializeField, HideInInspector]
    private int _numofTimesToSpawn = 0; // Total number of enemies to spawn if not infinite

    [SerializeField]
    private List<SpawnInfo> _spawnPoints = new List<SpawnInfo>(); // List of spawn points and prefabs

    private Timer _timer; // Controls time between spawns
    private int _numofTimeSpawned; // Tracks how many enemies have been spawned

    AnimatorManager _animatorManager;

    // Called when the actuator starts
    public override void StartActuator()
    {
        _timer = new Timer(_spawnInterval); // Create a timer for spawning
        _timer.Start(); // Start the timer
        _numofTimeSpawned = 0;
        _animatorManager = this.gameObject.GetComponent<AnimatorManager>();
    }

    // Called every frame if is in the actual State
    public override void UpdateActuator()
    {
        _timer.Update(Time.deltaTime); // Update the timer

        // If time is up, spawn enemies and restart the timer
        if (_timer.GetTimeRemaining() <= 0)
        {
            SpawnEvent(); //spawn enemies
            _timer.Start(); // Reset timer for next spawn cycle
        }
    }

    // Called when the actuator is destroyed
    public override void DestroyActuator()
    {
        // No specific cleanup required currently
    }


    public void Spawn()
    {
        // Spawn each prefab at its corresponding spawn point
        foreach (var spawnInfo in _spawnPoints)
        {
            if (spawnInfo._prefabToSpawn != null && spawnInfo._spawnPoint != null)
            {
                Instantiate(spawnInfo._prefabToSpawn, spawnInfo._spawnPoint.position, spawnInfo._spawnPoint.rotation);
            }
            else
            {
                Debug.LogWarning("Try to spawn object but there is no prefab or point to spawn");
            }
        }
    }
    // Handles the actual spawning of enemies
    private void SpawnEvent()
    {
        // Check if we can spawn more enemies (infinite or within limit)
        if (!_infiniteEnemies && _numofTimeSpawned >= _numofTimesToSpawn) return;

        _numofTimeSpawned++;

        if (_animatorManager == null || !_animatorManager.isActiveAndEnabled)
        {
            Spawn();
        }
        else
        {
            // Trigger spawn animation/event if available
            _animatorManager.SpawnEvent();
        }
    }
    #region Setters and getters

    public bool GetInfiniteEnemies() => _infiniteEnemies;

    public void SetNumberOfEnemiesToSpawn(int newValue)
    {
        _numofTimesToSpawn = newValue;
    }

    public int GetNumberOfEnemiesToSpawn() => _numofTimesToSpawn;

    #endregion
}
