using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Networking;

namespace OriDeSRDC
{
    public class SRDCLoader : MonoBehaviour
    {
        public static SRDCLoader Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private readonly List<Leaderboard> requestsInProgress = new List<Leaderboard>();

        private static readonly Dictionary<Leaderboard, string> leaderboardGetMap = new Dictionary<Leaderboard, string>
        {
            [Leaderboard.Explorer] = "https://www.speedrun.com/api/v1/leaderboards/v1pwgmd8/category/zdnw64ed?embed=players&var-wle6xqe8=5lm8p48q", // All Skills No OOB/TA
            [Leaderboard.SpeedRunner] = "https://www.speedrun.com/api/v1/leaderboards/v1pwgmd8/category/jdrqz7gk?embed=players&var-j84k62yn=rqv69rrl", // All Cells No NG+
            [Leaderboard.Survivor] = "https://www.speedrun.com/api/v1/leaderboards/v1pwgmd8/category/q25oyqyk?embed=players&var-68kmd2zl=zqom9351" // Any% No NG+
        };

        public void Load(Leaderboard leaderboard, Action<string> callback)
        {
            if (requestsInProgress.Contains(leaderboard))
                return;

            StartCoroutine(GetCoroutine(leaderboard, callback));
        }

        private IEnumerator GetCoroutine(Leaderboard leaderboard, Action<string> callback)
        {
            requestsInProgress.Add(leaderboard);

            string url = leaderboardGetMap[leaderboard];
            Debug.Log("Sending GET to " + url);
            UnityWebRequest wr = UnityWebRequest.Get(url);

            yield return wr.Send();

            // TODO error handling

            callback(wr.downloadHandler.text);

            requestsInProgress.Remove(leaderboard);
        }
    }
}
