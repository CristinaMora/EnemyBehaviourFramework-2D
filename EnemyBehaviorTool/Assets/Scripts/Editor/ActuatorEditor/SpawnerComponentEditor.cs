using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnerActuator))]
public class SpawnerComponentEditor : Editor
{
    private GUIContent infiniteEnemiesLabel = new GUIContent("Infinite Enemies", "If true, the spawner will spawn enemies indefinitely. If false, it will spawn a limited number of times.");
    private GUIContent numOfEnemiesToSpawnLabel = new GUIContent("Amount Of Spawn Event", "The number of times the enemies are spawned.");
    private GUIContent spawnIntervalLabel = new GUIContent("Spawn Interval", "The time interval (in seconds) between each spawn");
    private GUIContent spawnListLabel = new GUIContent("Spawn Points", "List of prefabs to spawn and their respective spawn point");

    private SerializedProperty infiniteEnemiesProp;
    private SerializedProperty numOfEnemiesToSpawnProp;
    private SerializedProperty spawnIntervalProp;
    private SerializedProperty spawnListProp;

    private void OnEnable()
    {
        
        infiniteEnemiesProp = serializedObject.FindProperty("_infiniteEnemies");
        numOfEnemiesToSpawnProp = serializedObject.FindProperty("_numofTimesToSpawn");
        spawnIntervalProp = serializedObject.FindProperty("_spawnInterval");
        spawnListProp = serializedObject.FindProperty("_spawnList");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("If you are using an AnimatorManager, you must specify when objects should spawn using Animation Events. CallSpawn().", MessageType.Warning);

        EditorGUILayout.PropertyField(infiniteEnemiesProp, infiniteEnemiesLabel);
        if (!infiniteEnemiesProp.boolValue)
          EditorGUILayout.PropertyField(numOfEnemiesToSpawnProp, numOfEnemiesToSpawnLabel);
        
        EditorGUILayout.PropertyField(spawnIntervalProp, spawnIntervalLabel);
        EditorGUILayout.PropertyField(spawnListProp, spawnListLabel);

        serializedObject.ApplyModifiedProperties();
    }
}