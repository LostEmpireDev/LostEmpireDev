using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI; 
using UnityEngine;

public class Soldiers : MonoBehaviour
{ 
    private string compName, Name, description;
    private int magic, hp, attack, defense;
    public GameObject bannerImage;
    Button btn;
    public int index;
    public int grandIndex = 0; 
    public bool optionSoldier = false;
    BaseControl control; 
    string path;
    public void Start()
    {
        path = Application.dataPath;
        path += "/Resources/";
    }
    private void Awake()
    {
      
    }
    public void StatInitializer(string name)
    {
        compName = name;
        StreamReader read = new StreamReader(path + "Stats/" + compName + "Stat.txt", true);
        Name = read.ReadLine();
        description = read.ReadLine();
        int.TryParse(read.ReadLine(), out attack);
        int.TryParse(read.ReadLine(), out defense);
        int.TryParse(read.ReadLine(), out hp);
        int.TryParse(read.ReadLine(), out magic);
        bannerImage = Resources.Load<GameObject>("CharBanner/Soldiers/Button");
    }
    public void StatInitializer(string name, int i)
    {
        compName = name;
        index = i; 
        StreamReader read = new StreamReader(path + "Stats/" + compName + "Stat.txt", true);
        Name = read.ReadLine();
        description = read.ReadLine();
        int.TryParse(read.ReadLine(), out attack);
        int.TryParse(read.ReadLine(), out defense);
        int.TryParse(read.ReadLine(), out hp);
        int.TryParse(read.ReadLine(), out magic);
        bannerImage = Resources.Load<GameObject>("CharBanner/Soldiers/Button");
    }
    public void FirstFadeBannerIn(string Battalion)
    {
        bannerImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharBanner/Soldiers/" + compName);
        GameObject val = Instantiate(bannerImage);
        val.transform.SetParent(GameObject.Find("Soldiers").transform, true);
        val.name = compName + "Banner(Clone)" + index + Battalion;
        RectTransform pos = val.GetComponent<RectTransform>();
        pos.position = new Vector3(800 + index * 300, 240, 0);
        StartCoroutine(DoFade(true, index, Battalion, compName));
        btn = val.GetComponent<Button>();
        btn.onClick.AddListener(Clicked);
    }
    public void FadeBannerIn(string Battalion)
    {
        bannerImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharBanner/Soldiers/" + compName);
        GameObject val = GameObject.Find("Soldiers").transform.GetChild(index).gameObject;
        val.GetComponent<Image>().sprite = bannerImage.GetComponent<Image>().sprite;
        RectTransform pos = val.GetComponent<RectTransform>();
        pos.position = new Vector3(800 + index*300, 240, 0);
        StartCoroutine(DoFade(true, index, Battalion, compName));
       
    }
    public void FadeBannerIn(int i, string Battalion, Vector3 value)
    {
        bannerImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharBanner/Soldiers/" + compName);
        GameObject val = GameObject.Find("Soldiers").transform.GetChild(index).gameObject;
        val.GetComponent<Image>().sprite = bannerImage.GetComponent<Image>().sprite; 
        RectTransform pos = val.GetComponent<RectTransform>();
        pos.position = value;
        StartCoroutine(DoFade(true, i, Battalion, compName)); 

    }
    public void SetUpBtn(GameObject val)
    {
        btn = val.GetComponent<Button>();
        btn.onClick.AddListener(Clicked); 
    }
    public void FadeBannerOut(string Battalion)
    {
        StartCoroutine(DoFade(false, index, Battalion, compName));
    }
    public void UpdateValues(Soldiers value, string bat)
    {
        grandIndex = value.grandIndex;
        bannerImage = value.bannerImage;
        compName = value.compName;
        Name = value.name;
        bannerImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharBanner/Soldiers/" + compName);
        GameObject val = GameObject.Find("Soldiers").transform.GetChild(index).gameObject;
        val.GetComponent<Image>().sprite = bannerImage.GetComponent<Image>().sprite;
        val.name = compName + "Banner(Clone)" + index + bat;
    }
    public void Clicked()
    {
        BaseControl control = GameObject.Find("Main Camera").GetComponent<BaseControl>();
        if (optionSoldier)
        {
            control.SoldierOptionClicked(grandIndex); 
        }
        else
            control.SoldierClicked(index); 
    }
    public GameObject GetInstance()
    {
        return GameObject.Find("Soldiers").transform.GetChild(index).gameObject; 
    }
    IEnumerator DoFade(bool fadeIn, int i, string Battalion, string tempName)
    {
        Image canvas = GameObject.Find("Soldiers").transform.GetChild(index).GetComponent<Image>(); 
      
        if (fadeIn)
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, 0);
        else
        {
            yield return new WaitForSecondsRealtime(1f);
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, 1);
        }
        float fadeSpeed = 0.5f;

        while (fadeIn && canvas.color.a < 1)
        {
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, canvas.color.a + Time.deltaTime / fadeSpeed);
            yield return null;
        }
        while (!fadeIn && canvas.color.a > 0)
        {
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, canvas.color.a - Time.deltaTime / fadeSpeed);
            yield return null;
        }
    }
    public string CompName
    {
        get
        {
            return compName; 
        }
    }
    public int Attack
    {
        get
        {
            return attack; 
        }
    }
    public int Defense
    {
        get
        {
            return defense; 
        }
    }
    public int Magic
    {
        get
        {
            return magic; 
        }
    }
}
