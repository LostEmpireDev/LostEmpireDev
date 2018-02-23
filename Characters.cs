using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System; 
using System.IO;
using UnityEngine;

public enum CharType { Archer, Swordsman, Lancer, Mage, Assasin };
public enum Empire { Roman, Greek, Persian, Chinese, Japanese, British, Egyption}
public class Characters : MonoBehaviour {
    public string Name, CompName;
    protected int attack, defense, hP, magic;
    protected float speed, dashSpeed;
    protected CharType CharClass;
    GameObject bannerImage;
    Empire allegiance;
    public GameObject symbol;
    public Material mat;
    public Sprite CharIcon; 
    string path;
    public void Start()
    {
        path = Application.dataPath;
        path += "/Resources/";
    }
    /// <summary>
    /// Store the stats from its file
    /// </summary>
    public void StatInitializer(string cName)
    {
        CompName = cName;        
        StreamReader read = new StreamReader(path + "Stats/" + CompName + "Stat.txt", true);
        Name = read.ReadLine();
        string emp = read.ReadLine();
        allegiance = (Empire)Enum.Parse(typeof(Empire), emp, true);
        string val = read.ReadLine();
        CharClass = (CharType)Enum.Parse(typeof(CharType), val, true); 
        int.TryParse(read.ReadLine(), out attack);
        int.TryParse(read.ReadLine(), out defense);
        int.TryParse(read.ReadLine(), out hP);
        int.TryParse(read.ReadLine(), out magic);
        read.Close(); 
        string newPath = "Environment/Menu/Symbols/";
        symbol = Resources.Load<GameObject>(newPath + allegiance.ToString().ToLower()+"Symbol");
        mat = Resources.Load<Material>(newPath + "Particles/" + allegiance.ToString().ToLower() + "Material"); 
        bannerImage = (GameObject)Resources.Load("CharBanner/" + CompName + "Banner");
        CharIcon = Resources.Load<Sprite>("CharacterIcons/Images/" + CompName.ToLower() + "Icon");

    }
    /// <summary>
    /// Instantiate and fade the Banner Image for this Character
    /// </summary>
    public void FadeBannerIn()
    {
        bannerImage = Resources.Load<GameObject>("CharBanner/" + CompName + "Banner") as GameObject;
        Instantiate(bannerImage);
        StartCoroutine(DoFade(true));
    }
    /// <summary>
    /// Fade and destroy an Instantiated Banner Image
    /// </summary>
    public void FadeBannerOut()
    {
        StartCoroutine(DoFade(false));
    }
    /// <summary>
    /// Fade Banner Images over time
    /// </summary>
    /// <param name="fadeIn">True if fadding in, False Otherwise</param>
    /// <returns></returns>
    IEnumerator DoFade(bool fadeIn)
    {

        SpriteRenderer canvas = GameObject.Find(CompName + "Banner(Clone)").GetComponent<SpriteRenderer>();
        if (fadeIn)
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, 0); 
        else
        {
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, 1);
        }
        float fadeSpeed = 0.5f;

        while (fadeIn)
        {
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, canvas.color.a + Time.deltaTime / fadeSpeed);
            yield return null;
        }
        while (!fadeIn && canvas != null)
        {
            canvas.color = new Color(canvas.color.r, canvas.color.g, canvas.color.b, canvas.color.a - Time.deltaTime / fadeSpeed);
            yield return null;
        }
        if (canvas != null && canvas.color.a <= 0)
        {
            this.StopAllCoroutines(); 
            Destroy(GameObject.Find(CompName + "Banner(Clone)"));
        }
    }
    public float DashSpeed
    {
        get
        {
            return dashSpeed; 
        }
        set
        {
            if (dashSpeed < 0)
                speed = 0;
            else
                speed = value; 
        }
    }
    public Empire Allegiance
    {
        get
        {
            return allegiance;
        }
    }
    public CharType CharacterClass
    {
        get
        {
            return CharClass; 
        }
    }
    public float Speed
    {
        get
        {
            if (speed == 0)
                return 0.1f;
            else
                return speed; 
        }
        set
        {
            if (speed < 0)
                speed = 0;
            else
                speed = value; 
        }
    }
    public int Defense
    {
        get
        {
            return defense; 
        }
        set
        {
            if (defense < 0)
                defense = 0;
            else
                defense = value; 
        }
    }
    public int Attack
    {
        get
        {
            return attack; 
        }
        set
        {
            if (value < 0)
                attack = 0;
            else
                attack = value; 
        }
    }
    public int Magic
    {
        get
        {
            return magic; 
        }
        set
        {
            if (value < 0)
                magic = 0;
            else
                magic = value; 
        }
    }
   public int HP
    {
        get
        {
            return hP; 
        }
        set
        {
            if (value < 0)
                hP = 0;
            else
                hP = value; 
        }
    }

}
