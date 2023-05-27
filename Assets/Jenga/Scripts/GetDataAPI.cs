using System;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class GetDataAPI : MonoBehaviour
{
    [SerializeField] string apiURL;
    [SerializeField] JengaStackTowerManager jengaStackTowerManagerScript;
    void Start()
    {
        StartCoroutine(GetRequest(apiURL));
    }

    
    IEnumerator GetRequest(string uri) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;
                responseText = FixJson(responseText);
                
                var responseData = JsonConvert.DeserializeObject<StackDataWrapper>(responseText, new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    Formatting = Formatting.Indented
                });
         
                var stacks = responseData.stack.GroupBy(data => data.grade)
                    .Select(grp => grp.ToList())
                    .ToList();

                for (int i = 0; i < stacks.Count; i++)
                {
                    if(stacks[i].Count < 3) continue;
                    
                    var tt  = stacks[i].OrderBy(data => data.domain).ThenBy(data => data.cluster).ThenBy(data => data.standardid)
                        .ToList();
                    jengaStackTowerManagerScript.Init(stacks[i]);
                }
            }
        }
    }
    
    string FixJson(string value)
    {
        value = "{\"stack\":" + value + "}";
        return value;
    }
}

[Serializable]
public class StackData
{
    public int id { get; set; }
    public string subject { get; set; }
    public string grade { get; set; }
    public int mastery { get; set; }
    public string domainid { get; set; }
    public string domain { get; set; }
    public string cluster { get; set; }
    public string standardid { get; set; }
    public string standarddescription { get; set; }
}
[Serializable]
public class StackDataWrapper
{
    public StackData[] stack;
}