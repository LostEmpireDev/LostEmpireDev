using System.Collections;
using System.Collections.Generic;
using System.IO;
using System; 
using UnityEngine;
using UnityEngine.UI;
public enum SelectionSate {Character, Soldier}
public class BaseControl : MonoBehaviour {
    //List of Battalions and Characters available 
    public List<Characters> availableChars = new List<Characters>();
    private List<Soldiers> availableSoldiers = new List<Soldiers>();
    public List<Battalion> Battalions = new List<Battalion>();
    // store indexing 
    int battalionIndex = 0;
    string path;
    int numBat;
    public SelectionSate state = SelectionSate.Character;
    bool Transitioning = true; 
    // projected display values 
    private float displayAtk, displayDef, displayHP, displayMagic;
    private float teamDisplayAtk, teamDisplayDef, teamDisplayCommand, teamDisplayMagic;
    // 
    private RectTransform teamAtkBar, teamDefBar, teamMagicBar, teamCommandBar; 
    private RectTransform atkBar, defBar, healthBar, magicBar;
    private InputField BattalionDisplayName;
    private Text CharacterName;
    private Image rightArrow, leftArrow;
    // Use this for initialization
    void Start() {
        Screen.orientation = ScreenOrientation.AutoRotation;
        path = Application.dataPath;
        path += "/Resources/Stats/BaseControl.txt";
        StreamReader read = new StreamReader(path, true);
        LoadAvailChars(read);
        LoadAvailSoldiers(read);
        displayAtk = 0;
        displayDef = 0;
        displayHP = 0;
        teamDisplayAtk = 0;
        teamDisplayCommand = 0;
        teamDisplayDef = 0;
        teamDisplayMagic = 0; 
        LoadBattalions(read);
        read.Close();

    }
    private void Awake()
    {
        BattalionDisplayName = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<InputField>();
        atkBar = GameObject.Find("Stats").transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>();
        defBar = GameObject.Find("Stats").transform.GetChild(1).transform.GetChild(0).GetComponent<RectTransform>();
        healthBar = GameObject.Find("Stats").transform.GetChild(2).transform.GetChild(0).GetComponent<RectTransform>();
        magicBar = GameObject.Find("Stats").transform.GetChild(3).transform.GetChild(0).GetComponent<RectTransform>();
        teamAtkBar = GameObject.Find("TeamStats").transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>();
        teamDefBar = GameObject.Find("TeamStats").transform.GetChild(1).transform.GetChild(0).GetComponent<RectTransform>();
        teamMagicBar = GameObject.Find("TeamStats").transform.GetChild(2).transform.GetChild(0).GetComponent<RectTransform>();
        teamCommandBar = GameObject.Find("TeamStats").transform.GetChild(3).transform.GetChild(0).GetComponent<RectTransform>();
        CharacterName = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<Text>();

    }
    /// <summary>
    /// Load Available Characters into Character List
    /// </summary>
    /// <param name="read"></param>
    private void LoadAvailChars(StreamReader read)
    {
        string compName = read.ReadLine();
        while (compName != "-1")
        {
            availableChars.Add(CheckAndCreateChar(compName));
            compName = read.ReadLine();
        }
    }
    private void LoadAvailSoldiers(StreamReader read)
    {
        string compName = read.ReadLine();
        while (compName != "-1")
        { 
            availableSoldiers.Add(CheckAndCreateSoldier(compName));
            availableSoldiers[availableSoldiers.Count - 1].grandIndex = availableSoldiers.Count - 1; 
            compName = read.ReadLine();
        }
    }
    /// <summary>
    /// Check Which Character is being refered to
    /// </summary>
    /// <param name="value">Comp Name of Character</param>
    /// <returns>The Corresponding Character</returns>
    private Characters CheckAndCreateChar(string value)
    {
        Characters gen;
        gen = gameObject.AddComponent(typeof(Characters)) as Characters;
        gen.Start();
        gen.StatInitializer(value);
        return gen;
    }
    private Soldiers CheckAndCreateSoldier(string value, int i)
    {
        Soldiers gen;
        gen = gameObject.AddComponent(typeof(Soldiers)) as Soldiers;
        gen.Start();
        gen.StatInitializer(value, i);
        return gen;
    }
    private Soldiers CheckAndCreateSoldier(string value)
    {
        Soldiers gen;
        gen = gameObject.AddComponent(typeof(Soldiers)) as Soldiers;
        gen.Start();
        gen.StatInitializer(value);
        return gen;
    }
    /// <summary>
    /// Load made Battalions into list
    /// </summary>
    /// <param name="read"></param>
    private void LoadBattalions(StreamReader read)
    {
        LoadSoldiersInOptions(); 
        int.TryParse(read.ReadLine(), out numBat);
        for (int i = 0; i < numBat; i++)
        {
            Battalion gen = gameObject.AddComponent(typeof(Battalion)) as Battalion;
            gen.Name = read.ReadLine();
            gen.commander = read.ReadLine();
            gen.InitiateCommand(availableChars);
            gen.avaialableSoldiers = availableSoldiers; 
            for (int j = 0; j < 4; j++)
                gen.soldiers.Add(CheckAndCreateSoldier(read.ReadLine(),j));
            Battalions.Add(gen);
        }
        Battalions[battalionIndex].LoadCommand();
        StartCoroutine(Battalions[battalionIndex].LoadSoldiers());
        StartCoroutine(DisplayStats(true, availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayBattalionName(Battalions[battalionIndex].Name, true));
        StartCoroutine(InitiateParticles(availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, true));
        StartCoroutine(DisplayName(Battalions[battalionIndex].ComChar));
        StartCoroutine(DisplayArrows());
        StartCoroutine(DisplayTeamStat(true, Battalions[battalionIndex])); 
    }
    public void SoldierClicked(int i)
    {
        StartCoroutine(DisplayTeamStat(false, Battalions[battalionIndex])); 
        Battalions[battalionIndex].ChangeSoldier(i, state);       
        StartCoroutine(DisplayTeamStat(true, Battalions[battalionIndex])); 
    }
    public void SoldierOptionClicked(int i)
    {
        Battalions[battalionIndex].ChangeSpecificSoldier(i); 
    }
    /// <summary>
    /// Animate the Stats of a Character or Battalion (Battalion overload needs to coded) 
    /// </summary>
    /// <param name="fadeIn">True if Fading in, False Otherwise</param>
    /// <param name="target">The Character or Battalion who's stats are taken</param>
    /// <returns></returns>
    public IEnumerator DisplayStats(bool fadeIn, Characters target)
    {
        const int min = -4;
        const int max = 460;
        if (fadeIn)
        {
            while (displayAtk != 0 || displayDef != 0 || displayHP != 0)
            {
                yield return null;
            }
            displayAtk = 0;
            displayDef = 0;
            displayHP = 0;
            atkBar.position = new Vector3(atkBar.position.x, -4, atkBar.position.z);
            defBar.position = new Vector3(defBar.position.x, -4, defBar.position.z);
            magicBar.position = new Vector3(magicBar.position.x, -4, magicBar.position.z);
            healthBar.position = new Vector3(healthBar.position.x, -4, healthBar.position.z);
            float speed = 0.4f;
            float feed = 10f;
            yield return new WaitForSecondsRealtime(0.5f);
            while (displayAtk < target.Attack || displayDef < target.Defense || displayHP < target.HP)
            {
                if (displayAtk < target.Attack)
                    displayAtk += Time.deltaTime * feed;
                if (displayDef < target.Defense)
                    displayDef += Time.deltaTime * feed;
                if (displayHP < target.HP)
                    displayHP += Time.deltaTime * feed;
                if (displayMagic < target.Magic)
                    displayMagic += Time.deltaTime * feed;
                atkBar.position = new Vector3(atkBar.position.x, min + displayAtk * speed, atkBar.position.z);
                defBar.position = new Vector3(defBar.position.x, min + displayDef * speed, defBar.position.z);
                magicBar.position = new Vector3(magicBar.position.x, min + displayMagic * speed, magicBar.position.z);
                healthBar.position = new Vector3(healthBar.position.x, min + displayHP * speed, healthBar.position.z);
                yield return null;
            }

        }
        else
        {
            float speed = 0.4f;
            float feed = 15f;
            while (atkBar.position.y > -4 || defBar.position.y > -4 || magicBar.position.y > -4 || healthBar.position.y > -4)
            {
                if (displayAtk > 0)
                    displayAtk -= Time.deltaTime * feed;
                if (displayDef > 0)
                    displayDef -= Time.deltaTime * feed;
                if (displayHP > 0)
                    displayHP -= Time.deltaTime * feed;
                if (displayMagic > 0)
                    displayMagic -= Time.deltaTime * feed;
                atkBar.position = new Vector3(atkBar.position.x, min + displayAtk * speed, atkBar.position.z);
                defBar.position = new Vector3(defBar.position.x, min + displayDef * speed, defBar.position.z);
                magicBar.position = new Vector3(magicBar.position.x, min + displayMagic * speed, magicBar.position.z);
                healthBar.position = new Vector3(healthBar.position.x, min + displayHP * speed, healthBar.position.z);
                yield return null;
            }
            if (displayHP < 0)
                displayHP = 0;
            if (displayDef < 0)
                displayDef = 0;
            if (displayAtk < 0)
                displayAtk = 0;
            if (displayMagic < 0)
                displayMagic = 0;
        }
    }
    private IEnumerator DisplayTeamStat(bool fadeIn, Battalion target)
    {
        float min = 10.6f;
        if (fadeIn)
        {
            while (teamDisplayAtk != 0 || teamDisplayCommand != 0 || teamDisplayDef != 0 || teamDisplayMagic != 0)
            {
                yield return null;
            }
            teamAtkBar.position = new Vector3(min, teamAtkBar.position.y);
            teamDefBar.position = new Vector3(min, teamDefBar.position.y);
            teamMagicBar.position = new Vector3(min, teamMagicBar.position.y);
            teamCommandBar.position = new Vector3(min, teamCommandBar.position.y);
            float speed = 0.02f;
            float feed = 10f;
            float atkTotal, defTotal, magicTotal, commandTotal;
            atkTotal = 0;
            defTotal = 0;
            magicTotal = 0;
            commandTotal = 0;
            for (int i = 0; i < 4; i++)
            {
                atkTotal += target.soldiers[i].Attack;
                defTotal += target.soldiers[i].Defense;
                magicTotal += target.soldiers[i].Magic;
            }
            atkTotal += target.ComChar.Attack * 2;
            defTotal += target.ComChar.Defense * 2;
            magicTotal += target.ComChar.Magic * 2;
            commandTotal += target.ComChar.Attack + target.ComChar.Defense + target.ComChar.Magic + target.ComChar.HP;
            atkTotal /= 6;
            defTotal /= 6;
            magicTotal /= 6;
            commandTotal /= 4;
            yield return new WaitForSecondsRealtime(0.5f);
            while (teamDisplayAtk < atkTotal || teamDisplayCommand < commandTotal || teamDisplayDef < defTotal || teamDisplayMagic < magicTotal)
            {
                if (teamDisplayAtk < atkTotal)
                {
                    teamDisplayAtk += Time.deltaTime * feed;
                    teamAtkBar.position = new Vector3(teamAtkBar.position.x - teamDisplayAtk * speed, teamAtkBar.position.y, teamAtkBar.position.z);
                }
                if (teamDisplayDef < defTotal)
                {
                    teamDisplayDef += Time.deltaTime * feed;
                    teamDefBar.position = new Vector3(teamDefBar.position.x - teamDisplayDef * speed, teamDefBar.position.y, teamDefBar.position.z);
                }
                if (teamDisplayCommand < commandTotal)
                {
                    teamDisplayCommand += Time.deltaTime * feed;
                    teamCommandBar.position = new Vector3(teamCommandBar.position.x - teamDisplayCommand * speed, teamCommandBar.position.y, teamCommandBar.position.z);
                }
                if (teamDisplayMagic < magicTotal)
                {
                    teamDisplayMagic += Time.deltaTime * feed;
                    teamMagicBar.position = new Vector3(teamMagicBar.position.x - teamDisplayMagic * speed, teamMagicBar.position.y, teamMagicBar.position.z);
                }
                yield return null;
            }

        }
        else
        {
            float speed = -0.02f;
            float feed = 10f;

            while (teamDisplayAtk > 0 || teamDisplayCommand > 0 || teamDisplayDef > 0 || teamDisplayMagic > 0)
            {
                                            
                if (teamDisplayAtk > 0)
                {
                    teamDisplayAtk -= Time.deltaTime * feed;
                    teamAtkBar.position = new Vector3(teamAtkBar.position.x - teamDisplayAtk * speed, teamAtkBar.position.y, teamAtkBar.position.z);
                }
                if (teamDisplayDef > 0)
                {
                    teamDisplayDef -= Time.deltaTime * feed;
                    teamDefBar.position = new Vector3(teamDefBar.position.x - teamDisplayDef * speed, teamDefBar.position.y, teamDefBar.position.z);
                }
                if (teamDisplayCommand > 0)
                {
                    teamDisplayCommand -= Time.deltaTime * feed;
                    teamCommandBar.position = new Vector3(teamCommandBar.position.x - teamDisplayCommand * speed, teamCommandBar.position.y, teamCommandBar.position.z);
                }
                if (teamDisplayMagic > 0)
                {
                    teamDisplayMagic -= Time.deltaTime * feed;
                    teamMagicBar.position = new Vector3(teamMagicBar.position.x - teamDisplayMagic * speed, teamMagicBar.position.y, teamMagicBar.position.z);
                }
                yield return null;
            }
            teamDisplayDef = 0;
            teamDisplayMagic = 0;
            teamDisplayCommand = 0;
            teamDisplayAtk = 0; 
        }
    }
    /// <summary>
    /// Display the name of the Character 
    /// </summary>
    /// <param name="Target">Target Character</param>
    /// <returns></returns>
    private IEnumerator DisplayName(Characters Target)
    {

        yield return new WaitForSecondsRealtime(2f);
        while (CharacterName.text.Length > 0)
            yield return null;
        int counter = 0;
        int len = Target.Name.Length;
        CharacterName.fontSize = 90 - len;
        bool attempt = false;
        while (counter < Target.Name.Length)
        {
            if (counter > 10 && Target.Name[counter] == ' ' && attempt)
            {
                CharacterName.text += "\n";
                CharacterName.fontSize = 20;
            }
            else
                CharacterName.text += Target.Name[counter];
            counter++;
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Transitioning = false; 
    }
    /// <summary>
    /// Display the left and right arrows to switch characters
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisplayArrows()
    {
        rightArrow = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Image>();
        leftArrow = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<Image>();
        while (rightArrow.color.a < 1)
        {
            rightArrow.color = new Color(rightArrow.color.r, rightArrow.color.g, rightArrow.color.b, rightArrow.color.a + Time.deltaTime);
            leftArrow.color = new Color(leftArrow.color.r, leftArrow.color.g, leftArrow.color.b, leftArrow.color.a + Time.deltaTime);
            yield return null;
        }
    }
    private IEnumerator HideArrows()
    {
        rightArrow = GameObject.Find("Canvas").transform.GetChild(2).GetComponent<Image>();
        leftArrow = GameObject.Find("Canvas").transform.GetChild(3).GetComponent<Image>();
        while (rightArrow.color.a > 0)
        {
            rightArrow.color = new Color(rightArrow.color.r, rightArrow.color.g, rightArrow.color.b, rightArrow.color.a - Time.deltaTime);
            leftArrow.color = new Color(leftArrow.color.r, leftArrow.color.g, leftArrow.color.b, leftArrow.color.a - Time.deltaTime * 20f);
            yield return null;
        }
    }
    /// <summary>
    /// Clear the Character Name 
    /// </summary>
    /// <returns></returns>
    private IEnumerator ClearName()
    {
        while (CharacterName.text.Length > 0)
        {
            CharacterName.text = CharacterName.text.Remove(CharacterName.text.Length - 1);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    /// <summary>
    /// Display the name of a Battalion
    /// </summary>
    /// <param name="nextName">Name of the Battalion</param>
    /// <param name="slideIn">Sliding in or out</param>
    /// <returns></returns>
    private IEnumerator DisplayBattalionName(string nextName, bool slideIn)
    {
        Rigidbody2D text = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Rigidbody2D>();
        int speed = 1300;
        if (slideIn)
        {
            BattalionDisplayName.text = nextName;
            text.position = new Vector2(2400, 860);
            while (text.position.x > 1400)
            {
                text.position = new Vector2(text.position.x - Time.deltaTime * speed, text.position.y);
                yield return null;
            }
            text.velocity = new Vector2(0, 0);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.3f);
            while (text.position.x < 2400)
            {
                text.position = new Vector2(text.position.x + Time.deltaTime * speed * 3, text.position.y);
                yield return null;
            }
            BattalionDisplayName.text = nextName;
            yield return new WaitForSecondsRealtime(0.2f);
            while (text.position.x > 1400)
            {
                text.position = new Vector2(text.position.x - Time.deltaTime * speed * 2, text.position.y);
                yield return null;
            }
        }
    }
    /// <summary>
    /// Play the Particles for a Character
    /// </summary>
    /// <param name="Target">Character being refered to</param>
    /// <returns></returns>
    private IEnumerator InitiateParticles(Characters Target)
    {
        yield return null;
        //yield return new WaitForSecondsRealtime(2f);
        //GameObject obj = GameObject.Find("Particle System");
        //ParticleSystemRenderer render = obj.GetComponent<ParticleSystemRenderer>(); 
        //ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        //var emission = particle.emission;
        //var main = particle.main; 
        //emission.enabled = true;      
        //emission.burstCount = 1;
        //emission.rateOverTime = 0; 
        //emission.rateOverDistance = 20;
        //render.material = Target.mat;
        //emission.rateOverTime = 200f;
        //yield return new WaitForSecondsRealtime(1f);
        //emission.rateOverTime = 0; 
    }
    /// <summary>
    /// Show the symbol of the Character's empire
    /// </summary>
    /// <param name="Target"></param>
    /// <param name="fadeIn"></param>
    /// <returns></returns>
    private IEnumerator DisplaySymbol(Characters Target, bool fadeIn)
    {
        if (fadeIn)
        {
            yield return new WaitForSecondsRealtime(1f);
            foreach (GameObject banners in GameObject.FindGameObjectsWithTag("Symbol"))
            {
                if (banners.name != (Target.CompName + "Banner(Clone)"))
                {
                    Destroy(banners);
                }
            }
            int max = 50;
            float speed = 0.5f;
            GameObject value = Instantiate(Target.symbol);
            value.name = Target.CompName + "Symbol";
            SpriteRenderer alpha = value.GetComponent<SpriteRenderer>();
            alpha.color = new Color(alpha.color.r, alpha.color.g, alpha.color.b, 0);
            while (alpha.color.a < max)
            {
                alpha.color = new Color(alpha.color.r, alpha.color.g, alpha.color.b, alpha.color.a + Time.deltaTime * speed);
                yield return null;
            }
        }
        else
        {
            SpriteRenderer alpha = GameObject.Find(Target.CompName + "Symbol").GetComponent<SpriteRenderer>();
            float speed = 5;
            while (alpha.color.a > 0)
            {
                alpha.color = new Color(alpha.color.r, alpha.color.g, alpha.color.b, alpha.color.a - Time.deltaTime * speed);
                yield return null;
            }
            Destroy(GameObject.Find(Target.CompName + "Symbol"));
        }
    }
    /// <summary>
    /// Save the Current Characters and Battalions
    /// </summary>
    private void SaveChanges()
    {
        Battalions[battalionIndex].Name = BattalionDisplayName.text;
        using (StreamWriter write = new StreamWriter(path))
        {
            for (int i = 0; i < availableChars.Count; i++)
            {
                write.WriteLine(availableChars[i].CompName);
            }
            write.WriteLine(-1);
            for (int i = 0; i < availableSoldiers.Count; i++)
            {
                write.WriteLine(availableSoldiers[i].CompName);
            }
            write.WriteLine(-1);
            write.WriteLine(numBat);
            for (int i = 0; i < Battalions.Count; i++)
            {
                write.WriteLine(Battalions[i].Name);
                write.WriteLine(availableChars[Battalions[i].CharIndex].CompName);
                for (int j = 0; j < 4; j++)
                    write.WriteLine(Battalions[i].soldiers[j].CompName);
            }
        }
    }
    /// <summary>
    /// Load all Character Icons 
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadCharGrid()
    {
        RectTransform pan = GameObject.Find("CharacterGrid").transform.GetChild(0).GetComponent<RectTransform>();
        bool slideIn = (pan.position.x >= 2000);
        if (slideIn)
        {
            while (pan.position.x >= 960)
            {
                pan.position = new Vector3(pan.position.x - 40, pan.position.y, pan.position.z);
                yield return null;
            }
        }
        else
        {
            while (pan.position.x <= 2800)
            {
                pan.position = new Vector3(pan.position.x + 60, pan.position.y, pan.position.z);
                yield return null;
            }
        }
    }
    private IEnumerator HideTeamStats()
    {
        Transform underline = GameObject.Find("TeamStats").transform.GetChild(4).GetComponent<Transform>(); 
        RectTransform[] stats = new RectTransform[4];
        Debug.Log(underline.position.x); 
        for (int i = 0; i < 4; i++)
            stats[i] = GameObject.Find("TeamStats").transform.GetChild(i).GetComponent<RectTransform>(); 
        while (underline.position.x < 18.5)
        {
            underline.position = new Vector3(underline.position.x + Time.deltaTime * 9, underline.position.y, underline.position.z);
            for (int i = 0; i < 4; i++)
                stats[i].position = new Vector3(stats[i].position.x + Time.deltaTime * 9, stats[i].position.y, stats[i].position.z);
            yield return null; 
        }
    }
    private IEnumerator ShowTeamStats()
    {
        Transform underline = GameObject.Find("TeamStats").transform.GetChild(4).GetComponent<Transform>();
        RectTransform[] stats = new RectTransform[4];
        for (int i = 0; i < 4; i++)
            stats[i] = GameObject.Find("TeamStats").transform.GetChild(i).GetComponent<RectTransform>();
        while (underline.position.x > 7.48)
        {
            underline.position = new Vector3(underline.position.x - Time.deltaTime * 9, underline.position.y, underline.position.z);
            for (int i = 0; i < 4; i++)
                stats[i].position = new Vector3(stats[i].position.x - Time.deltaTime * 9, stats[i].position.y, stats[i].position.z);
            yield return null;
        }
        Debug.Log(underline.position.x); 
    }
    private IEnumerator HideCharacter()
    {
        Transform command = GameObject.Find(Battalions[battalionIndex].ComChar.CompName + "Banner(Clone)").GetComponent<Transform>();
        RectTransform[] stats = new RectTransform[4];
        for (int i = 0; i < 4; i++)
            stats[i] = GameObject.Find("Stats").transform.GetChild(i).GetComponent<RectTransform>();
        while (command.position.x > -11.5)
        {
            command.position = new Vector3(command.position.x - Time.deltaTime * 9, command.position.y, command.position.z);
            for (int i = 0; i < 4; i++)
                stats[i].position = new Vector3(stats[i].position.x - Time.deltaTime * 9, stats[i].position.y, stats[i].position.z);
            yield return null;
        }
    }
    private IEnumerator ShowCharacter()
    {
        Transform command = GameObject.Find(Battalions[battalionIndex].ComChar.CompName + "Banner(Clone)").GetComponent<Transform>();
        RectTransform[] stats = new RectTransform[4];
        for (int i = 0; i < 4; i++)
            stats[i] = GameObject.Find("Stats").transform.GetChild(i).GetComponent<RectTransform>();
        while (command.position.x < -6.5)
        {
            command.position = new Vector3(command.position.x + Time.deltaTime * 9, command.position.y, command.position.z);
            for (int i = 0; i < 4; i++)
                stats[i].position = new Vector3(stats[i].position.x + Time.deltaTime * 9, stats[i].position.y, stats[i].position.z);
            yield return null;
        }
    }
    private IEnumerator HideBack()
    {
        RectTransform text = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<RectTransform>();
        SpriteRenderer arrow = GameObject.Find("BackArrow").GetComponent<SpriteRenderer>();

        while (arrow.color.a > 0 || text.position.x > -1200)
        {
            if (arrow.color.a > 0)
                arrow.color = new Color(arrow.color.r, arrow.color.g, arrow.color.b, arrow.color.a - Time.deltaTime);
            if (text.position.x > -1200)
                text.position = new Vector3(text.position.x - Time.deltaTime * 1000f, text.position.y, text.position.z);
            yield return null;
        }
    }
    private IEnumerator ShowBack()
    {
        RectTransform text = GameObject.Find("Canvas").transform.GetChild(4).GetComponent<RectTransform>();
        SpriteRenderer arrow = GameObject.Find("BackArrow").GetComponent<SpriteRenderer>();

        while (arrow.color.a < 1 || text.position.x < -700)
        {
            if (arrow.color.a < 1)
                arrow.color = new Color(arrow.color.r, arrow.color.g, arrow.color.b, arrow.color.a + Time.deltaTime);
            if (text.position.x < -700)
                text.position = new Vector3(text.position.x + Time.deltaTime * 1000f, text.position.y, text.position.z);
            yield return null;
        }
    }
    private IEnumerator PushSoldiers()
    {
        int num = 4;
        Transform[] pos = new Transform[num];
        Rigidbody2D[] move = new Rigidbody2D[num];
        for (int i = 0; i < num; i++)
        {
            string name = Battalions[battalionIndex].soldiers[i].CompName + "Banner(Clone)" + i + Battalions[battalionIndex].Name;
            pos[i] = GameObject.Find(name).GetComponent<Transform>();
            move[i] = GameObject.Find(name).GetComponent<Rigidbody2D>();
        }
        while (pos[0].position.y < 400)
        {
            for (int i = 0; i < num; i++)
            {
                move[i].velocity = new Vector2(0, move[i].velocity.y + Time.deltaTime * 800f);
            }
            yield return null;
        }
        while (move[0].velocity.y > 0)
        {
            for (int i = 0; i < num; i++)
                move[i].velocity = new Vector2(0, move[i].velocity.y - Time.deltaTime * 460f);
            yield return null;
        }
        foreach (Rigidbody2D a in move)
            a.velocity = new Vector2(0, 0);
        while (pos[0].position.x > 200f)
        {
            for (int i = num - 1; i >= 0; i--)
            {
                move[i].velocity = new Vector2(move[i].velocity.x - Time.deltaTime * (num - 1 - i) * 900f, 0);
            }

            yield return null;
        }
        while (move[0].velocity.y > 0)
        {
            for (int i = 0; i < num; i++)
            {
                move[i].velocity = new Vector2(move[i].velocity.x - Time.deltaTime * i, 0);
                pos[i].position = new Vector3(pos[i].position.x - Time.deltaTime * 8f, 0);
            }
            yield return null;
        }
        foreach (Rigidbody2D a in move)
            a.velocity = new Vector2(0, 0);
    }
    private IEnumerator PullSoldiers()
    {
        int num = 4;
        Transform[] pos = new Transform[num];
        Rigidbody2D[] move = new Rigidbody2D[num];
        for (int i = 0; i < num; i++)
        {
            string name = Battalions[battalionIndex].soldiers[i].CompName + "Banner(Clone)" + i + Battalions[battalionIndex].Name;
            pos[i] = GameObject.Find(name).GetComponent<Transform>();
            move[i] = GameObject.Find(name).GetComponent<Rigidbody2D>();
        }
        while (pos[0].position.x < 700)
        {
            for (int i = num - 1; i >= 0; i--)
            {
                move[i].velocity = new Vector2(move[i].velocity.x +Time.deltaTime * (num - 1 - i) * 900f, 0);
            }
            yield return null;
        }
        while (move[0].velocity.y > 0)
        {
            for (int i = 0; i < num; i++)
            {
                move[i].velocity = new Vector2(move[i].velocity.x - Time.deltaTime * i, 0);
                pos[i].position = new Vector3(pos[i].position.x - Time.deltaTime * 8f, 0);
            }
            yield return null;
        }
        foreach (Rigidbody2D a in move)
            a.velocity = new Vector2(0, 0);
        while (pos[0].position.y > 250)
        {
            for (int i = 0; i < num; i++)
            {
                move[i].velocity = new Vector2(0, move[i].velocity.y - Time.deltaTime * 900f);
            }
            yield return null;
        }
        while (move[0].velocity.y > 0)
        {
            for (int i = 0; i < num; i++)
                move[i].velocity = new Vector2(0, move[i].velocity.y + Time.deltaTime * 23f);
            yield return null;
        }
        foreach (Rigidbody2D a in move)
            a.velocity = new Vector2(0, 0);
    }
    private void LoadSoldiersInOptions()
    {
        for (int i = 0; i < availableSoldiers.Count; i++)
        {
            GameObject add = Instantiate(Resources.Load<GameObject>("CharBanner/Soldiers/Button"));
            add.GetComponent<Image>().sprite = Resources.Load<Sprite>("CharBanner/Soldiers/" + availableSoldiers[i].CompName);
            add.name = availableSoldiers[i].CompName + "Button";
            availableSoldiers[i].optionSoldier = true; 
            add.GetComponent<Transform>().position = new Vector3(300, -500 * i+250);
            availableSoldiers[i].SetUpBtn(add);
            add.transform.SetParent(GameObject.Find("Canvas").transform.GetChild(5).transform.GetChild(0).transform.GetChild(0), true);
        }
        
    }
    private IEnumerator LoadSoldierOptions()
    {
        CanvasGroup pos = GameObject.Find("Canvas").transform.GetChild(5).GetComponent<CanvasGroup>();
        while (pos.alpha > 0)
            yield return null;
        while (pos.alpha < 1)
        {
            pos.alpha += Time.deltaTime; 
            for (int i = 0; i < availableSoldiers.Count; i++)
            {
                Image val = GameObject.Find("Canvas").transform.GetChild(5).transform.GetChild(0).transform.GetChild(0).GetChild(i).GetComponent<Image>(); 
                val.color = new Color(val.color.r, val.color.g, val.color.b, val.color.a + Time.deltaTime); 
            }
            yield return null; 
        }
    }
    private IEnumerator HideSoldierOptions()
    {
        CanvasGroup pos = GameObject.Find("Canvas").transform.GetChild(5).GetComponent<CanvasGroup>();
        while (pos.alpha > 0)
        {
            pos.alpha -= Time.deltaTime;
            for (int i = 0; i < availableSoldiers.Count; i++)
            {
                Image val = GameObject.Find("Canvas").transform.GetChild(5).transform.GetChild(0).transform.GetChild(0).GetChild(i).GetComponent<Image>();
                val.color = new Color(val.color.r, val.color.g, val.color.b, val.color.a - Time.deltaTime);
            }
            yield return null;
        }
    }
    private void LoadSoldierSelect()
    {
        if (state == SelectionSate.Character)
        {
            StopAllCoroutines();
            StartCoroutine(HideTeamStats()); 
            Battalions[battalionIndex].LoadSelectBar(true); 
            StartCoroutine(HideCharacter());
            StartCoroutine(PushSoldiers());
            StartCoroutine(HideArrows());
            StartCoroutine(ClearName());
            StartCoroutine(HideBack());
            StartCoroutine(LoadSoldierOptions());
            state = SelectionSate.Soldier; 
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowTeamStats()); 
            Battalions[battalionIndex].LoadSelectBar(false); 
            StartCoroutine(HideSoldierOptions()); 
            StartCoroutine(PullSoldiers()); 
            StartCoroutine(ShowCharacter());
            StartCoroutine(ShowBack());
            StartCoroutine(DisplayName(Battalions[battalionIndex].ComChar));
            state = SelectionSate.Character; 
        }
    }
   
    /// <summary>
    /// Load a specific character from  the available Characters
    /// </summary>
    /// <param name="compName"></param>
    public void LoadSpecificCharacter(string compName)
    {
        Characters newChar = availableChars.Find(e => e.CompName == compName);
        //StopAllCoroutines();
        StartCoroutine(ClearName());
        StartCoroutine(DisplayStats(false, availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayTeamStat(false, Battalions[battalionIndex])); 
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, false));
        Battalions[battalionIndex].LoadCommand(newChar);
        StartCoroutine(DisplayStats(true, availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayTeamStat(true, Battalions[battalionIndex]));
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, true));
        StartCoroutine(InitiateParticles(availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayName(availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(LoadCharGrid()); 
    }
    /// <summary>
    /// Transition to the next character in the list of Available Characters
    /// </summary>
    /// <param name="isRight">Direction of the Transition</param>
    private void ChangeCharacter(bool isRight)
    {
        StopAllCoroutines();
        int tempI;
        if (isRight)
            tempI = battalionIndex - 1;
        else
            tempI = battalionIndex + 1; 
        if (tempI < 0)
            tempI = Battalions.Count - 1;
        else if (tempI == Battalions.Count)
            tempI = 0;
        if (GameObject.Find(availableChars[Battalions[tempI].CharIndex].CompName + "Banner(Clone)"))
        {
            Battalions[tempI].ComChar.StopAllCoroutines();
            Destroy(GameObject.Find(Battalions[tempI].ComChar.CompName + "Symbol"));
            Destroy(GameObject.Find(Battalions[tempI].ComChar.CompName + "Banner(Clone)"));
        }
        StartCoroutine(ClearName());
        StartCoroutine(DisplayStats(false, availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayTeamStat(false, Battalions[battalionIndex]));
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, false));
        if (isRight)
            Battalions[battalionIndex].LoadNextCommand(true);
        else
            Battalions[battalionIndex].LoadNextCommand(false);
        StartCoroutine(DisplayStats(true, availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayTeamStat(true, Battalions[battalionIndex]));
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, true));
        StartCoroutine(InitiateParticles(availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayName(availableChars[Battalions[battalionIndex].CharIndex]));
    }
    /// <summary>
    /// Transition to the next Battalion in the list of Battalions
    /// </summary>
    /// <param name="isRight">Direction of the Tranisiton</param>
    private void ChangeBattalion(bool isRight)
    {
        StopAllCoroutines();
        StartCoroutine(DisplayStats(false, availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayTeamStat(false, Battalions[battalionIndex]));
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, false));
        if (isRight)
            LoadNextBattalion(true);
        else
            LoadNextBattalion(false); 
        StartCoroutine(DisplaySymbol(Battalions[battalionIndex].ComChar, true));
        StartCoroutine(DisplayTeamStat(true, Battalions[battalionIndex]));
        StartCoroutine(DisplayStats(true, availableChars[Battalions[battalionIndex].CharIndex]));
    }
    /// <summary>
    /// Ready assets for Transitioning 
    /// </summary>
    /// <param name="isRight">Driection of Transition</param>
    private void LoadNextBattalion(bool isRight)
    {
        int tempI;
        if (isRight)
            tempI = battalionIndex - 1;
        else
            tempI = battalionIndex + 1;
        if (tempI < 0)
            tempI = Battalions.Count - 1;
        else if (tempI == Battalions.Count)
            tempI = 0;
        foreach (GameObject banners in GameObject.FindGameObjectsWithTag("Banner"))
        {
            if (banners.name == (availableChars[Battalions[tempI].CharIndex].CompName + "Banner(Clone)"))
            {
                Battalions[tempI].ComChar.StopAllCoroutines();
                Destroy(GameObject.Find(Battalions[tempI].ComChar.CompName + "Symbol"));
                Destroy(banners);
            }
        }
        SaveChanges(); 
        StartCoroutine(ClearName());
        Battalions[battalionIndex].ComChar.StopAllCoroutines();
        Battalions[battalionIndex].ComChar.FadeBannerOut();
        int prev = battalionIndex; 
        if (isRight)
            battalionIndex++;
        else
            battalionIndex--;
        if (battalionIndex == Battalions.Count)
            battalionIndex = 0;
        else if (battalionIndex < 0)
            battalionIndex = Battalions.Count - 1;
        StartCoroutine(DisplayBattalionName(Battalions[battalionIndex].Name, false));
        StartCoroutine(Battalions[battalionIndex].RefreshSoldiers(Battalions[prev].soldiers));
        Battalions[battalionIndex].ComChar.StopAllCoroutines();
        Battalions[battalionIndex].LoadCommand();
        StartCoroutine(InitiateParticles(availableChars[Battalions[battalionIndex].CharIndex]));
        StartCoroutine(DisplayName(availableChars[Battalions[battalionIndex].CharIndex])); 
    }
    // Update is called once per frame
    void Update()
    {
        if (!BattalionDisplayName.isFocused)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Moved)
                    {
                        if (new Rect(new Vector2(700, 200), new Vector2(1200, 1250)).Contains(touch.position) && !Transitioning)
                        {
                            if (Math.Abs((touch.deltaPosition.y)) > Math.Abs(touch.deltaPosition.x) && state == SelectionSate.Character)
                            {
                                LoadSoldierSelect();                             
                            }
                            else
                            {
                                if (touch.deltaPosition.x > 0)
                                    ChangeBattalion(true);
                                else
                                    ChangeBattalion(false);
                                Transitioning = true; 
                            }
                        }
                    }
                    if (touch.phase == TouchPhase.Ended && state == SelectionSate.Character)
                    {
                        if (new Rect(new Vector2(200, 300), new Vector2(500, 700)).Contains(touch.position))
                            ChangeCharacter(true);
                        else if (new Rect(new Vector2(0, 300), new Vector2(200, 700)).Contains(touch.position))
                            ChangeCharacter(false);                      
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                ChangeCharacter(true);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                ChangeCharacter(false);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangeBattalion(true);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                LoadSoldierSelect(); 
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangeBattalion(false);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                SaveChanges();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(LoadCharGrid()); 
            }
        }
        else
        {
            TouchScreenKeyboard.Open(""); 
        }
    }
}
