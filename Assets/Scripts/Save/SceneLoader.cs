using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SceneLoader : MonoBehaviour
{
    private string savePath = "gameSave.json";

    [System.Serializable]
    public class GameData
    {
        public List<GameController.ObjectData> objectsData;
    }

    private void Start()
    {
        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        yield return null;

        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            GameData gameData = JsonUtility.FromJson<GameData>(json);

            if (gameData.objectsData != null)
            {
                foreach (var objData in gameData.objectsData)
                {
                    GameObject obj = new GameObject(objData.objectName);
                    obj.transform.position = objData.position;
                    obj.SetActive(objData.isActive);
                }
            }
        }
        else
        {
            Debug.LogWarning("Файл сохранения не найден");
        }
    }
}
