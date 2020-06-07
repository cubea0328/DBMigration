using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class Connect : MonoBehaviour
{
    public void ConnectMySql(string str)
    {
        MySqlConnection conn = new MySqlConnection(str);
        MySqlCommand cmd = new MySqlCommand();
        MySqlBackup backup = new MySqlBackup(cmd);
        
        cmd.Connection = conn;
        try
        {
            conn.Open();
        }
        catch (Exception ex)
        {
            conn.Close();
            return;
        }
        backup.ExportToFile(Application.persistentDataPath+"/dump.txt");
        conn.Close();
    }
}
