using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X9;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject ProfilePanel;
    public GameObject LoginPanel;
    public GameObject SignInPanel;

    public TMP_InputField Login;
    public TMP_InputField Password;
    public TMP_Text ErrorText, ErrorText2;

    public TMP_Text NickName, Content, Score, KilledEnemy, GameCount;
    public TMP_InputField RegNickName, RegContent, RegLogin, RegPassword;

    private string connectionString = "Server=localhost;Database=totec;Uid=root;Pwd=rootPass;Charset=utf8mb4";

    public Animator anim;

    void Start()
    {
        if (PlayerPrefs.HasKey("UserID"))
        {
            int userID = PlayerPrefs.GetInt("UserID");
            UserInfo(userID);
        }
    }

    
    void Update()
    {
        
    }

    public void StartGame()
    {
        if (PlayerPrefs.HasKey("UserID"))
            SceneManager.LoadScene("Scenes/TrainingZone");
        else
            LoginPanel.SetActive(true);
    }

    public void Profile()
    {
        if (PlayerPrefs.HasKey("UserID"))
        {
            if (anim.GetBool("IsOpen"))
                anim.SetBool("IsOpen", false);
            else
                anim.SetBool("IsOpen", true);
        }
        else if (!PlayerPrefs.HasKey("UserID") && LoginPanel.activeSelf)
            LoginPanel.SetActive(false);
        else if (!PlayerPrefs.HasKey("UserID") && !LoginPanel.activeSelf)
            LoginPanel.SetActive(true);
    }

    public void SigninPanel()
    {
        if (SignInPanel.activeSelf && !PlayerPrefs.HasKey("UserID"))
            SignInPanel.SetActive(false);
        else if (!PlayerPrefs.HasKey("UserID") && !SignInPanel.activeSelf)
            SignInPanel.SetActive(true);
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("UserID");
        anim.SetBool("IsOpen", false);
    }

    public void CreateAccount()
    {
        int dopInfoId = -1;
        int statisticId = -1;
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string dopInfoQuery = "INSERT INTO dop_info (Nickname, Image, Content) VALUES (@Nickname, @Image, @Content); SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(dopInfoQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Nickname", RegNickName.text);
                    cmd.Parameters.AddWithValue("@Image", "defult.png");
                    cmd.Parameters.AddWithValue("@Content", RegContent.text);
                    dopInfoId = Convert.ToInt32(cmd.ExecuteScalar());
                    Debug.Log(dopInfoId);
                }

                string statisticQuery = "INSERT INTO statistic (Score, gameCount, killedEnemy) VALUES (@Score, @gameCount, @KilledEnemy); SELECT LAST_INSERT_ID();";
                using (MySqlCommand cmd = new MySqlCommand(statisticQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Score", 0);
                    cmd.Parameters.AddWithValue("@gameCount", 0);
                    cmd.Parameters.AddWithValue("@KilledEnemy", 0);
                    statisticId = Convert.ToInt32(cmd.ExecuteScalar());
                    Debug.Log(statisticId);
                }

                if (dopInfoId > 0 && statisticId > 0)
                {
                    string userQuery = "INSERT INTO users (login, password, idInfo, idStatistic) VALUES (@username, @password, @dopInfoId, @statisticId)";
                    using (MySqlCommand cmd = new MySqlCommand(userQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", RegLogin.text);
                        cmd.Parameters.AddWithValue("@password", RegPassword.text);
                        cmd.Parameters.AddWithValue("@dopInfoId", dopInfoId);
                        cmd.Parameters.AddWithValue("@statisticId", statisticId);

                        int result = cmd.ExecuteNonQuery();
                    }
                }
                ErrorText2.text = "The account has been successfully created!";
                SignInPanel.SetActive(false);
            }
            catch (Exception ex)
            {
                ErrorText2.text = "Error: " + ex.Message;
            }
        }
    }

    public void EnterLogin()
    {
        string login = Login.text;
        string password = Password.text;

        int userID = AuthenticateUser(login, password);

        if (userID > 0)
        {
            PlayerPrefs.SetInt("UserID", userID);
            PlayerPrefs.Save();
            UserInfo(userID);
            LoginPanel.SetActive(false);
            anim.SetBool("IsOpen", true);
        }
        else
        {
            ErrorText.text = "Invalid login or password.";
        }
    }

    private int AuthenticateUser(string login, string password)
    {
        int userId = -1;

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string query = "SELECT * FROM users WHERE login = @username AND password = @password";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader.GetInt32("id");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error: " + ex.Message);
            }
        }

        return userId;
    }

    private void UserInfo(int userID)
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string query = @"
                SELECT di.Nickname, di.Content, s.Score, s.gameCount, s.killedEnemy
                FROM users u
                JOIN dop_info di ON u.idInfo = di.id
                JOIN statistic s ON u.idStatistic = s.id
                WHERE u.id = @userId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            NickName.text = reader.GetString("Nickname");
                            Content.text = reader.GetString("Content");
                            Score.text = "Score: " + reader.GetString("Score");
                            GameCount.text = "Game Count: " + reader.GetString("gameCount");
                            KilledEnemy.text = "Killed Enemy: " + reader.GetString("killedEnemy");
                        }
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
