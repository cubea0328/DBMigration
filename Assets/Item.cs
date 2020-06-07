using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    public Text text;
    public Toggle toggle;
    public InputField field;

    public void SetItem(string str)
    {
        text.text = str;
    }

    public bool GetCheck()
    {
        return toggle.isOn;
    }

    public string GetQuery()
    {
        string query;
        query = "db.createCollection(\"" + text.text+"\",{";
        if (GetCheck())
        {
            query += "autoIndexId:true";
        }
        if (field.text != "")
        {
            query+=GetCheck() ? "," : "";
            query += "capped:true,";
            query += "size:" + field.text;
        }
        query += "})";
        return query;
    }
}
