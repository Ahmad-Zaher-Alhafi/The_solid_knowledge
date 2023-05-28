using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Controllers {
    public class BlocksDataController : MonoBehaviour, IController {
        private const string URL = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";

        public void FetchData(Action<List<BlockData>> onFinishCallback) {
            StartCoroutine(FetchFromServer(onFinishCallback));
        }

        private IEnumerator FetchFromServer(Action<List<BlockData>> onFinishedCallback) {
            using UnityWebRequest webRequest = UnityWebRequest.Get(URL);

            yield return webRequest.SendWebRequest();

            if (webRequest.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError) {
                Debug.LogError(webRequest.error);
            } else {
                string json = webRequest.downloadHandler.text;
                json = "{ \n \"Blocks\": " + json + "\n }";

                Data myObjects = JsonUtility.FromJson<Data>(json);
                onFinishedCallback(myObjects.Blocks.ToList());
            }
        }

        [Serializable]
        public class Data {
            public List<BlockData> Blocks = new();
        }

        [Serializable]
        public class BlockData {
            public int id;
            public string subject;
            public string grade;
            public int mastery;
            public string domainid;
            public string domain;
            public string cluster;
            public string standardid;
            public string standarddescription;

            public void Init(BlockData blockData) {
                id = blockData.id;
                subject = blockData.subject;
                grade = blockData.grade;
                mastery = blockData.mastery;
                domainid = blockData.domainid;
                domain = blockData.domain;
                cluster = blockData.cluster;
                standardid = blockData.standardid;
                standarddescription = blockData.standarddescription;
            }
        }

        public void Restart() {

        }
    }
}