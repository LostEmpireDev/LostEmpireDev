using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI; 
using UnityEngine;
public enum SortStyle {Empire, Level, Favourites}
public class CharGridControl : MonoBehaviour {
    SortStyle order;
    public List<Characters> availableCharacters;
    GameObject baseIcon;
    BaseControl input;
    GameObject[,] CharEmpires  = new GameObject[10,10]; 
    bool activated = false; 
    RectTransform activator; 
	// Use this for initialization
	void Start () {
        order = SortStyle.Empire; 
	}
    private void Awake()
    {
        activator = GameObject.Find("CharacterGrid").transform.GetChild(0).GetComponent<RectTransform>();
        StartCoroutine(DisplayBack()); 
    }
    public IEnumerator SortEmpire()
    {
        CharEmpires = new GameObject[10, 10];
        baseIcon = Resources.Load<GameObject>("CharacterIcons/CharacterButton");
        input = GameObject.Find("Main Camera").GetComponent<BaseControl>();
        availableCharacters = input.availableChars; 
        float offsetx = 500;
        float offsety = 120; 
        float multiplier = 200f;
        float vertMult = 200f; 
        for (int i = 0; i < availableCharacters.Count; i++)
        {
            yield return new WaitForSecondsRealtime(0.02f);
            CharEmpires[(int)availableCharacters[i].Allegiance,(int)availableCharacters[i].CharacterClass] = Instantiate(baseIcon);
            RectTransform pos = CharEmpires[(int)availableCharacters[i].Allegiance, (int)availableCharacters[i].CharacterClass].GetComponent<RectTransform>();
            StartCoroutine(AnimateIcon(CharEmpires[(int)availableCharacters[i].Allegiance, (int)availableCharacters[i].CharacterClass], true)); 
            pos.position = new Vector3(offsetx + (int)availableCharacters[i].Allegiance * multiplier, offsety + (int)availableCharacters[i].CharacterClass * vertMult, pos.position.z); 
            CharButtons but = CharEmpires[(int)availableCharacters[i].Allegiance,(int)availableCharacters[i].CharacterClass].GetComponent<CharButtons>();
            but.LoadIcon(availableCharacters[i].CompName, availableCharacters[i].CharIcon); 
            CharEmpires[(int)availableCharacters[i].Allegiance,(int)availableCharacters[i].CharacterClass].transform.SetParent(this.transform, true); 
        }
    }
    public IEnumerator DisplayBack()
    {
        string back = "BACK";
        int counter = 0;
        SpriteRenderer arrow = GameObject.Find("BackArrow").GetComponent<SpriteRenderer>();
        Text word = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<Text>();
        while (counter < 5)
        {
            while (counter < 4)
            {
                word.text += back[counter];
                counter++;
                yield return new WaitForSecondsRealtime(0.1f);
            }
            counter--;
            yield return new WaitForSecondsRealtime(3f);
            //counter = 0; 
            while (counter >= 0)
            {
                word.text = word.text.Remove(counter);
                counter--;
                yield return new WaitForSecondsRealtime(0.5f);
            }
            counter = 0;
        }
    }
    IEnumerator AnimateIcon(GameObject input, bool animIn) 
    {
        RectTransform Target = input.GetComponent<RectTransform>(); 
        if (Target.localScale.x != 0)
            yield return null;
        float size = 1f;
        if (animIn)
        {
            float timeInFrames = 20f;
            float rot = 60f;
            while (Target.localScale.x < size)
            {
                Target.localScale = new Vector3(Target.localScale.x + size / timeInFrames, Target.localScale.y + size / timeInFrames, Target.localScale.z);
                Target.Rotate(Vector3.forward, rot / timeInFrames);
                yield return null;
            }
        }
        else
        {
            float timeInFrames = 10f;
            float rot = 60f;
            while (Target.localScale.x > 0)
            {
                Target.localScale = new Vector3(Target.localScale.x - size / timeInFrames, Target.localScale.y - size / timeInFrames, Target.localScale.z);
                Target.Rotate(Vector3.back, rot / timeInFrames);
                yield return null;
            }
            Destroy(input); 
        }
    }
	// Update is called once per frame
	void Update () {
        if (activator.position.x < 970 && !activated)
        {
            activated = true; 
            StartCoroutine(SortEmpire());
        }
        else if (activator.position.x > 1200 && activated)
        {
            activated = false;
            StopCoroutine(SortEmpire());
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    if (CharEmpires[i,j] != null)
                        StartCoroutine(AnimateIcon(CharEmpires[i, j], false));
                }
            }
        }
	}
}
