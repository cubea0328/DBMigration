using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class File : MonoBehaviour
{
    public Dictionary<string, List<string>> collections = new Dictionary<string, List<string>>();
    public Dictionary<string, List<string>> documents = new Dictionary<string, List<string>>();

    public List<string> parsed;

    public List<string> ReadFile()
    {
        string path = Application.persistentDataPath+"/dump.txt";
        StreamReader reader=new StreamReader(path);
        string colName="";
        string line;
        int mode = -1; //-1=Ignore 0=create table 1=insert document
        char[] charToParse={ '`', ',','(',')','\''};
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("CREATE TABLE IF NOT EXISTS"))
            {
                string str = line.Split('`')[1];
                mode = 0;
                colName = str;
                collections.Add(colName, new List<string>());
                continue;
            }

            if (line.StartsWith("INSERT INTO"))
            {
                string str = line.Split('`')[1];
                mode = 1;
                colName = str;
                documents.Add(colName, new List<string>());
                continue;
            }

            switch (mode)
            {
                case 0:
                    if (line.StartsWith("  `"))
                    {
                        string str=line.Split('`')[1];
                        collections[colName].Add(str);
                    }
                    else
                    {
                        mode = -1;
                    }
                    break;
                case 1:
                    if (line[0]=='(')
                    {
                        parsed = Trim(line.Split(charToParse));
                        for (int i = 0; i < parsed.Count; i++)
                        {
                            documents[colName].Add(parsed[i]);
                        }
                    }
                    else
                    {
                        mode = -1;
                    }
                    break;
            }
        }

        return collections.Keys.ToList();
    }
    
    public void WriteFile(string[] mongoInfo, List<string> data)
    {
        string path = Application.persistentDataPath+"/query.js";
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine("db=db.getSiblingDB('"+data[0]+"');");
        data.RemoveAt(0);
        foreach(string str in data)
        {
            writer.WriteLine(str);
        }
        
        foreach (string key in documents.Keys)
        {
            for (int i = 0; i < documents[key].Count / collections[key].Count; ++i)
            {
                string query= "db." + key + ".insert({";
                for(int j = 0; j < collections[key].Count; j++)
                {
                    query += (j == 0 ? "" : ", ");
                    query += "\""+collections[key][j] + "\":\"" + documents[key][(i * collections[key].Count) + j]+"\"";
                }
                query += "})";
                writer.WriteLine(query);
            }
        }
        writer.Close();
        try
        {
            System.Diagnostics.Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "cmd.exe";
            string exe = "mongo "+mongoInfo[0]+":"+mongoInfo[1]+" "+Application.persistentDataPath+"/query.js";
            process.StartInfo.Arguments = "/c" + exe;
            process.EnableRaisingEvents = true;
            process.Start();
            process.WaitForExit();
            System.IO.File.Delete(Application.persistentDataPath+"/dump.txt");
            System.IO.File.Delete(Application.persistentDataPath+"/query.js");
        }
        catch (Exception ex)
        {
            
        }

    }
    
    public List<string> Trim(string[] strArr)
    {
        List<string> strList = new List<string>();
        for(int i = 0; i < strArr.Length; ++i)
        {
            if (strArr[i] != "" && strArr[i]!=" " && strArr[i]!=";") strList.Add(strArr[i]);
        }
        return strList;
    }
}
