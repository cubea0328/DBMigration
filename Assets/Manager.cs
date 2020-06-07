using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    private int step = 0;
    public GameObject item;
    public List<GameObject> items;

    public Text AlertText;
    public GameObject[] panels = new GameObject[3];
    public Button next;
    
    public InputField[] userInfo = new InputField[4];
    public InputField[] dbInfo=new InputField[2];
    public InputField dbName;
    public RectTransform rect;
    public Text elapsed;
    private float time;
    public bool flag;

    public GameObject icon;
    public Sprite[] sprites= new Sprite[3];
    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Step1();
    }

    // Update is called once per frame
    void Update()
    {
        if (step == 1) return;
        if (icon.transform.position.x != (step - 2) * 5)
        {
            icon.transform.position = Vector3.SmoothDamp(icon.transform.position, new Vector3((step - 2) * 5, 3.9f, 0),
                ref velocity, .2f);
        }
    }

    public void ToNextStep()
    {
        if (step == 1) Step2();
        else
        {
            Step3();
        }
    }
    
    void Step1()
    {
        icon.GetComponent<SpriteRenderer>().sprite = sprites[0];
        step = 1;
        panels[0].gameObject.SetActive(true);
        
    }

    void Step2()
    {
        System.Diagnostics.Stopwatch sw=new Stopwatch();
        sw.Start();
        icon.GetComponent<SpriteRenderer>().sprite = sprites[1];
        step = 2;
        panels[0].gameObject.SetActive(false);
        panels[1].gameObject.SetActive(true);
        GetComponent<Connect>().ConnectMySql(GenerateConStr());
        GenerateItems(GetComponent<File>().ReadFile());
        sw.Stop();
        time += sw.ElapsedMilliseconds;
    }

    void Step3()
    {
        System.Diagnostics.Stopwatch sw=new Stopwatch();
        sw.Start();
        next.gameObject.SetActive(false);
        icon.GetComponent<SpriteRenderer>().sprite = sprites[2];
        step = 3;
        panels[1].gameObject.SetActive(false);
        panels[2].gameObject.SetActive(true);
        GetComponent<File>().WriteFile(GetMongoInfo(),GenerateQuery());
        sw.Stop();
        time += sw.ElapsedMilliseconds;
        elapsed.text = "소요 시간 : "+time+"ms";
    }
    
    void GenerateItems(List<string> list)
    {
        int num = list.Count;
        items=new List<GameObject>();
        rect.sizeDelta=new Vector2(0,num*40);
        for (int i = 0; i < num; i++)
        {
            GameObject go = Instantiate(item,rect);
            go.GetComponent<RectTransform>().anchoredPosition=new Vector2(-20,num*20-i*30-20);
            go.transform.localScale = Vector3.one;
            go.GetComponent<Item>().SetItem(list[i]);
            items.Add(go);
        }

    }

    string GenerateConStr()
    {
        string str = "server=" + userInfo[0].text + ";user=" + userInfo[1].text + ";pwd=" +
                     userInfo[2].text + ";database=" + userInfo[3].text + ";";
        return str;
    }

    string[] GetMongoInfo()
    {
        string[] strArr=new string[2];
        strArr[0] = dbInfo[0].text;
        strArr[1] = dbInfo[1].text;
        return strArr;
    }
    
    List<string> GenerateQuery()
    {
        string dbStr = dbName.text;
        List<string> addQueries=new List<string>();
        addQueries.Add(dbStr);
        foreach (GameObject item in items)
        {
            addQueries.Add(item.GetComponent<Item>().GetQuery());
        }

        return addQueries;
    }
}
