using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DungeonGameCilent : MonoBehaviour
{
    [SerializeField] private string serverUrl = "http://127.0.0.1:3000";
    [SerializeField] private Text playerText;
    [SerializeField] private Text roomText;
    [SerializeField] private Text messageText;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button chestButton;
    [SerializeField] private Button restButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button returnButton;

    private void SendAction(string action)
    {
        string json = JsonUtility.ToJson(new DungeonActionDequest { action = action });
        StartCoroutine(Post("/api/dungeon/action", json));
    }
    private IEnumerator GetState()
    {
        using UnityWebRequest request = UnityWebRequest.Get(serverUrl + "/api/game/state");

        yield return request.SendWebRequest();
    }

    private IEnumerator Post(string path, string json)
    {
        using UnityWebRequest request = new UnityWebRequest(serverUrl + path, "POST");
        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
    }
}
