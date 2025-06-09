using MirraGames.SDK.Common;
using MirraGames.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

namespace Assets._scripts.UI
{
    public class LeaderboardMono:MonoBehaviour
    {
        [SerializeField] private Player playerPrefab;
        [SerializeField] private Transform _scrollView;
        [SerializeField] private Text _leaderboardName;
        [SerializeField] private int leaderbordCount = 100;
        public void Open()
        {
            MirraSDK.Achievements.GetLeaderboard("score", OnScoreTableResolve);
            gameObject.SetActive(true);
            //MirraSDK.Socials.GetScoreTable("score", 3, true, 3, OnScoreTableResolve, OnScoreTableError);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
        public void OnScoreTableResolve(Leaderboard table)
        {
            var player = table.players.Where(player => player.displayName == MirraSDK.Player.DisplayName).ToList();
            Debug.Log("Совпадений " + player.Count);
            List<PlayerScore> players = new List<PlayerScore>();
            if (player.Count > 0)
            {
                if (player[0].position > leaderbordCount)
                {
                    players = table.players.Where(player => player.position < leaderbordCount).ToList();
                    players.Add(player[0]);
                    Debug.Log("players1 " + players.Count);
                }
                else
                {
                    players = table.players.Where(player => player.position <= leaderbordCount).ToList();
                    Debug.Log("players2 " + players.Count);
                }
            }

            /*player = new List<PlayerScore>();
			players = new List<PlayerScore>();
			var playerScore = new PlayerScore();
			playerScore.position = 1;
			playerScore.displayName = "alex k";
			player.Add(playerScore);*/
            /*var playerScore2 = new PlayerScore();
            playerScore2.position = 1;
            players.Add(playerScore2);
            var playerScore3 = new PlayerScore();
            playerScore3.position = 2;
            players.Add(playerScore3);
            var playerScore4 = new PlayerScore();
            playerScore4.position = 3;
            players.Add(playerScore4);
            var playerScore5 = new PlayerScore();
            playerScore5.position = 4;
            players.Add(playerScore5);
            var playerScore6 = new PlayerScore();
            playerScore6.position = 5;
            players.Add(playerScore6);
            var playerScore7 = new PlayerScore();
            playerScore7.position = 6;
            players.Add(playerScore7);
			players.Add(playerScore);*/

            for (int i = 0; i < players.Count; i++)
            {
                Player playerGO = Instantiate(playerPrefab, _scrollView);
                if (player[0].position - 1 == i)
                {
                    playerGO.SetStatistic(players[i].position, players[i].displayName, players[i].score, players[i].profilePictureUrl, true);
                }
                else
                {
                    if (player[0].position > leaderbordCount && i == leaderbordCount-1)
                    {
                        playerGO.SetStatistic(players[i].position, players[i].displayName, players[i].score, players[i].profilePictureUrl, true);
                    }
                    else
                    {
                        playerGO.SetStatistic(players[i].position, players[i].displayName, players[i].score, players[i].profilePictureUrl, false);
                    }
                }

            }
        }

        public void OnScoreTableError()
        {
            _leaderboardName.gameObject.SetActive(false);
        }
    }
}
