using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ForUI : MonoBehaviour
{
    public GameObject E_Button;
    public GameObject DialogPanel;
    public TMP_Text text, ScoreText;

    private string connectionString = "Server=localhost;Database=totec;Uid=root;Pwd=rootPass;Charset=utf8mb4";

    private List<string> DialogList = new List<string>();

    public static int Score = 0;
    public static int EnemyCount = 5;

    public GameObject ComplitePanel;

    int StatisticID = 0;
    int CountChange = 1;

    private void Start()
    {
        MySqlConnection connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();

            string Statisticquery = @"SELECT * FROM users WHERE id = @UserID;";
            using (MySqlCommand cmd = new MySqlCommand(Statisticquery, connection))
            {
                cmd.Parameters.AddWithValue("@UserID", PlayerPrefs.GetInt("UserID"));

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        StatisticID = Convert.ToInt32(reader["idStatistic"]);
                    }
                }
            }

            string NewStatisticquery = @"SELECT * FROM statistic WHERE id = @ID;";
            using (MySqlCommand cmd = new MySqlCommand(NewStatisticquery, connection))
            {
                cmd.Parameters.AddWithValue("@ID", StatisticID);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) // Важно: вызовите Read() перед доступом к данным
                    {
                        PlayerPrefs.SetInt("Score", Convert.ToInt32(reader["Score"]));
                        PlayerPrefs.SetInt("GameCount", Convert.ToInt32(reader["gameCount"]));
                        PlayerPrefs.SetInt("KilledEnemy", Convert.ToInt32(reader["killedEnemy"]));
                        PlayerPrefs.Save();
                    }
                }
            }

            string query = "SELECT Content FROM dialogs;";
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DialogList.Add(reader["Content"].ToString());
                }
            }

        }
        catch (MySqlException ex)
        {
            Debug.LogError("MySQL Error: " + ex.ToString());
        }
    }

    void Update()
    {
        if (Player.IsDialog == true)
            E_Button.SetActive(true);
        else
            E_Button.SetActive(false);

        if (!DialogPanel.activeSelf && Player.IsDialog == true && Input.GetKeyDown(KeyCode.E))
        {
            text.text = DialogList[Player.index - 1];

            DialogPanel.SetActive(true);
        }
        else if (DialogPanel.activeSelf && Input.GetKeyDown(KeyCode.E))
            DialogPanel.SetActive(false);

        if (EnemyCount == 0 && CountChange == 1)
        {
            CountChange--;
            ComplitePanel.SetActive(true);
            Player.IsComplite = true;
            ScoreText.text = "Score: " + Score;

            int NewScore = PlayerPrefs.GetInt("Score") + Score;
            int GameCount = PlayerPrefs.GetInt("GameCount") + 1;
            int KilledEnemy = PlayerPrefs.GetInt("KilledEnemy") + 5;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string Statisticquery = @"UPDATE statistic SET Score = @Score, gameCount = @gameCount, killedEnemy = @killedEnemy WHERE id = @Id";
                    using (MySqlCommand cmd = new MySqlCommand(Statisticquery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", StatisticID);
                        cmd.Parameters.AddWithValue("@Score", NewScore);
                        cmd.Parameters.AddWithValue("@gameCount", GameCount);
                        cmd.Parameters.AddWithValue("@killedEnemy", KilledEnemy);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            // Обновление прошло успешно, сохраняем данные в PlayerPrefs
                            PlayerPrefs.SetInt("Score", NewScore);
                            PlayerPrefs.SetInt("GameCount", GameCount);
                            PlayerPrefs.SetInt("KilledEnemy", KilledEnemy);
                            PlayerPrefs.Save();
                        }
                        else
                        {
                            Debug.LogError("Error: No rows were updated.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error: " + ex.Message);
                }
            }
        }
    }

    public void ReturnMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
