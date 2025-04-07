using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

public class GameController : MonoBehaviour
{
    private string savePath;

    [Serializable]
    public class ObjectData
    {
        public string objectName;
        public Vector3 position;
        public bool isActive;
        public List<ComponentData> components = new();
    }

    [Serializable]
    public class ComponentData
    {
        public string type;
        public Dictionary<string, string> fields = new();
    }

    [Serializable]
    public class GameData
    {
        public List<ObjectData> objects = new();
    }

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    [ContextMenu("Save Game")]
    public void SaveGame()
    {
        GameData data = new();

        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.hideFlags != 0 || go.transform.parent != null) continue;

            ObjectData obj = new()
            {
                objectName = go.name,
                position = go.transform.position,
                isActive = go.activeSelf
            };

            foreach (Component comp in go.GetComponents<Component>())
            {
                if (comp == null || comp is Transform) continue;

                ComponentData compData = new() { type = comp.GetType().AssemblyQualifiedName };
                foreach (FieldInfo field in comp.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    try
                    {
                        object val = field.GetValue(comp);
                        if (val != null)
                            compData.fields[field.Name] = JsonUtility.ToJson(new Wrapper() { value = val });
                    }
                    catch { }
                }

                obj.components.Add(compData);
            }

            data.objects.Add(obj);
        }

        File.WriteAllText(savePath, JsonUtility.ToJson(data, true));
        Debug.Log("Сохранение завершено.");
    }

    [ContextMenu("Load Game")]
    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Файл сохранения не найден.");
            return;
        }

        string json = File.ReadAllText(savePath);
        GameData data = JsonUtility.FromJson<GameData>(json);

        // Удаляем все старые объекты
        foreach (GameObject go in FindObjectsOfType<GameObject>())
        {
            if (go.hideFlags != 0 || go.transform.parent != null) continue;
            Destroy(go);
        }

        foreach (ObjectData objData in data.objects)
        {
            GameObject obj = new GameObject(objData.objectName);
            obj.transform.position = objData.position;
            obj.SetActive(objData.isActive);

            foreach (ComponentData compData in objData.components)
            {
                Type type = Type.GetType(compData.type);
                if (type == null) continue;

                Component comp = obj.AddComponent(type);

                foreach (var fieldPair in compData.fields)
                {
                    FieldInfo field = type.GetField(fieldPair.Key, BindingFlags.Public | BindingFlags.Instance);
                    if (field == null) continue;

                    try
                    {
                        object value = JsonUtility.FromJson(fieldPair.Value, typeof(Wrapper)).GetType().GetField("value").GetValue(JsonUtility.FromJson(fieldPair.Value, typeof(Wrapper)));
                        field.SetValue(comp, value);
                    }
                    catch { }
                }
            }
        }

        Debug.Log("Загрузка завершена.");
    }

    [Serializable]
    private class Wrapper
    {
        public object value;
    }
}
