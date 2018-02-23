using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Battalion : MonoBehaviour {

    public string commander, Name;
    private int charIndex;
    public Characters ComChar;
    private List<Button> CharBtn = new List<Button>(); 
    private List<Characters> availableCharacters;
    public List<Soldiers> avaialableSoldiers; 
    public List<Soldiers> soldiers = new List<Soldiers>(); 
    private int soldierIndex = 0; 
	// Use this for initialization
	void Start () {
		
	}
    public void InitiateCommand(List<Characters> thisLoadChars)
    {
        availableCharacters = thisLoadChars;

        for (int i = 0; i < availableCharacters.Count; i++)
        {
            if (availableCharacters[i].CompName == commander)
            {              
                ComChar = availableCharacters[i];
                charIndex = i; 
            }
        }
    }
    public void LoadCommand()
    {        
        if (ComChar != null)
            ComChar.FadeBannerIn(); 
    }
    public void LoadCommand(Characters newChar)
    {
        availableCharacters[charIndex].StopAllCoroutines();
        availableCharacters[charIndex].FadeBannerOut();
        newChar.StopAllCoroutines();
        newChar.FadeBannerIn();
        ComChar = newChar;
        charIndex = availableCharacters.FindIndex(e => e.CompName == newChar.CompName); 
    }
    public void DeLoadCommand()
    {
        availableCharacters[charIndex].FadeBannerOut(); 
    }
    public void LoadNextCommand(bool isRight)
    {
        int tempI;
        if (isRight)
            tempI = charIndex - 1;
        else
            tempI = charIndex + 1;
        if (tempI < 0)
            tempI = availableCharacters.Count - 1;
        else if (tempI == availableCharacters.Count)
            tempI = 0;
        foreach (GameObject banners in GameObject.FindGameObjectsWithTag("Banner"))
        {
            if (banners.name == (availableCharacters[tempI].CompName + "Banner(Clone)"))
            {
                ComChar.StopAllCoroutines();
                string name = banners.name.Replace("Banner(Clone)", "Symbol");
                Destroy(GameObject.Find(name)); 
                Destroy(banners);
                break; 
            }
        }
        availableCharacters[charIndex].StopAllCoroutines(); 
        availableCharacters[charIndex].FadeBannerOut();
        if (isRight)
            charIndex++;
        else
            charIndex--;
        if (charIndex == availableCharacters.Count)
            charIndex = 0;
        else if (charIndex < 0)
            charIndex = availableCharacters.Count - 1; 
        availableCharacters[charIndex].FadeBannerIn(); 
        ComChar = availableCharacters[charIndex]; 
    }
    public IEnumerator LoadSoldiers()
    {
        yield return new WaitForSecondsRealtime(0.2f); 
        for(int i = 0; i < soldiers.Count; i++)
        {
            soldiers[i].FirstFadeBannerIn(Name); 
            yield return new WaitForSecondsRealtime(0.4f); 
        }
    }
    public IEnumerator RefreshSoldiers(List<Soldiers> prev)
    {
        yield return new WaitForSecondsRealtime(0.2f); 
        for (int i = 0; i < soldiers.Count; i++)
        {
            soldiers[i].FadeBannerOut(this.Name);
            soldiers[i].UpdateValues(soldiers[i], this.Name);
            soldiers[i].StopAllCoroutines(); 
            soldiers[i].FadeBannerIn(Name);
            yield return new WaitForSecondsRealtime(0.4f); 
        } 
    }
    public IEnumerator DeLoadSoldiers()
    {
        for(int i = 0; i < soldiers.Count; i++)
        {
            Debug.Log(soldiers[i].CompName); 
            soldiers[i].FadeBannerOut(Name);
        }
        yield return null; 
    }
    public void ChangeSoldier(int index, SelectionSate state)
    {
        if (state == SelectionSate.Character)
        {

            int idIndex = avaialableSoldiers.FindIndex(e=> e.CompName == soldiers[index].CompName);
            soldiers[index].FadeBannerOut(this.Name);
            idIndex++;
            if (idIndex > avaialableSoldiers.Count - 1)
                idIndex = 0;           
            soldiers[index].UpdateValues(avaialableSoldiers[idIndex], this.Name);
            soldiers[index].StopAllCoroutines(); 
            soldiers[index].FadeBannerIn(this.Name);
        }
        else
        {
            StartCoroutine(MoveSelectBar(index));
            soldierIndex = index; 
        }
    }
    public void ChangeSpecificSoldier(int index)
    {
        soldiers[soldierIndex].FadeBannerOut(this.Name);
        soldiers[soldierIndex].UpdateValues(avaialableSoldiers[index], this.Name);
        soldiers[soldierIndex].StopAllCoroutines(); 
        soldiers[soldierIndex].FadeBannerIn(SoldierIndex, this.Name, new Vector3(180 + soldierIndex * 508, 666, 0));
    }
    public void LoadSelectBar(bool fadeIn)
    {
        StartCoroutine(FadeSelectBar(fadeIn)); 
    }
    private IEnumerator MoveSelectBar(int i)
    {
        float speed = 20f;
        Transform pos = GameObject.Find("UnderLine").GetComponent<Transform>(); 
        if (i > soldierIndex)
        {
            while (pos.position.x < i*4.5f -7.3f)
            {
                pos.position = new Vector3(pos.position.x + Time.deltaTime*speed, pos.position.y);
                yield return null; 
            }
        }
        else
        {
            while (pos.position.x > i * 3f - 7.3f)
            {
                pos.position = new Vector3(pos.position.x - Time.deltaTime*speed, pos.position.y);
                yield return null;
            }
        }
    }
    private IEnumerator FadeSelectBar(bool fadeIn)
    {
        SpriteRenderer val = GameObject.Find("UnderLine").GetComponent<SpriteRenderer>();
        if (fadeIn)
            val.color = new Color(val.color.r, val.color.g, val.color.b, 0);
        else
            val.color = new Color(val.color.r, val.color.g, val.color.b, 1); 
        if (fadeIn)
        {
            while (val.color.a < 1)
            {
                val.color = new Color(val.color.r, val.color.g, val.color.b, val.color.a + Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (val.color.a > 0)
            {
                val.color = new Color(val.color.r, val.color.g, val.color.b, val.color.a - Time.deltaTime); 
            }
        }
                
    }
	// Update is called once per frame
	void Update () {
		
	}
    public int CharIndex
    {
        get
        {
            return charIndex; 
        }
    }
    public int SoldierIndex
    {
        get
        {
            return soldierIndex; 
        }
    }
        
}
