using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerManager : MonoBehaviour
{
    [SerializeField] private MainMenuManager mainMenuManager;

    // population, wood, gold, food, stone, iron, tools, days

    [Header("Resources")]
    [SerializeField] private int days; //DONE
    [SerializeField] private int population;
    [SerializeField] private int workers; //DONE
    [SerializeField] private int unemployed; //DONE
    [SerializeField] private int wood; //DONE
    [SerializeField] private int gold; // from gold mines
    [SerializeField] private int food; //DONE
    [SerializeField] private int stone; // from quarry
    [SerializeField] private int iron; // from iron mines
    [SerializeField] private int tools; // from blacksmith

    // days passing and having house gives people, farm gives food, lumbermill gives wood, blacksmith makes tools,
    // quarry/gold/ironmine unlocks when blacksmith is built, blacksmith limited to 1 per 50 people, mines give 1 gold/iron per person per day
    // 

    [Header("Building")]
    //farm, house,iron mines,gold mines,woodcutter, blacksmith,quarry
    [SerializeField] private int farm; // 1 house takes 4 people, more houses more population  //DONE
    [SerializeField] private int house = 2; //DONE gives 1 person every day
    [SerializeField] private int lumberMill; //DONE
    [SerializeField] private int blacksmithhut; // gives tools,needs 1 worker, costs 5 wood to build,
                                             // has to have tools to be able to send people to iron/gold mines / quarry
    [SerializeField] private int quarry; //gives stone, every worker in quarry produces 4 stones/day
    [SerializeField] private int ironMines; //gives iron, every worker in iron mine produces 1 iron/day
    [SerializeField] private int goldMines; //gives gold, every worker in fold mine produces 1 gold/2days

    [Header("Resources Text")]
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text populationText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text toolsText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text ironText;
    [SerializeField] private TMP_Text goldText;

    [Header("Buildings Text")]
    [SerializeField] private TMP_Text houseText;
    [SerializeField] private TMP_Text farmText;
    [SerializeField] private TMP_Text blacksmithhutText;
    [SerializeField] private TMP_Text blacksmithhutCostText;
    [SerializeField] private TMP_Text blacksmithhutBuiltText;
    [SerializeField] private TMP_Text quarryText;
    [SerializeField] private TMP_Text ironMinesText;
    [SerializeField] private TMP_Text goldMinesText;
    [SerializeField] private TMP_Text lumberMillText;
    [SerializeField] private TMP_Text notificationText;

    [Header("Buildings Text")]

    [SerializeField] private Button blacksmithHutButton;
    [SerializeField] private Button ironMinesButton;
    [SerializeField] private Button lumberMillButton;
    [SerializeField] private Button quarryButton;
    [SerializeField] private Button goldMinesButton;
    [SerializeField] private Button trollButton;

    [Header("Sprites and audio")]
    [SerializeField] public Image daysCount;
    [SerializeField] private AudioSource toolsAudio;
    [SerializeField] private AudioClip toolsAudioClip;

    bool IsGameRunning = false;
    private float timer;
   

    private void Start()
    {
        blacksmithhutBuiltText.gameObject.SetActive(false);
        notificationText.gameObject.SetActive(false);
        ironMinesButton.gameObject.SetActive(false);
        lumberMillButton.gameObject.SetActive(false);
        quarryButton.gameObject.SetActive(false);
        goldMinesButton.gameObject.SetActive(false);
        trollButton.gameObject.SetActive(false);
        population = house * 4;
        UpdateText();
    }

    private void Update()
    {
        // one minute is one day
        TimeOfDay();
        ImageFill();
        SmallDisclaimerButton();




        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1;
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 2;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 3;
        }
        if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 4;
        }
        if(Input.GetKeyDown(KeyCode.Alpha5))
        {
            Time.timeScale = 20;
        }
    }
    public void InitialiseGame()
    {
        IsGameRunning = true;
        UpdateText();
        GameReset();
    }

    private void TimeOfDay()
    {
        if(!IsGameRunning)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= 12)
        {
            days++;
            daysCount.fillAmount = 0;
            FoodGathering();
            WoodGathering();
            FoodProduction();
            WoodProduction();
            ToolProduction();
            IronProduction();
            GoldProduction();
            StoneProduction();
            FoodConsumption(1);
            //IncreasePopulation();
            NewBuildingsUnlocked();
            UpdateText();
            WinCondition();
            LoseCondition();
           



            timer = 0;
        }
        
    }

    private void ImageFill()
    {
        float dayDuration = 12f;
        daysCount.fillAmount = timer / dayDuration;
    }

    private void IncreasePopulation()
    {
        if (days % 1 == 0)
        {
            if (GetMaxPopulation() > Population())
            {
                unemployed += house;
                if (GetMaxPopulation() < Population())
                {
                    unemployed = GetMaxPopulation() - workers;
                }
            }
        }
    }

    private int Population()
    {
        return workers + unemployed;
    }

    private void FoodConsumption(int foodConsumed)
    {
        food -= foodConsumed * Population();
    }

    private void WoodGathering()
    {
        wood += unemployed / 2;
    }

    private void FoodGathering()
    {
        food += unemployed / 2;
    }

    private void FoodProduction()
    {
        food += farm * 4;
    }

    private void IronProduction()
    {
        if (days % 3 == 0)
        {
            iron += ironMines * 2; 
        }
    }
    private void GoldProduction()
    {
        if (days % 4 == 0)
        {
            gold += goldMines * 2; 
        }
    }



    private int GetMaxPopulation() //number of max house * 4
    {

        int maxPopulation = house * 4;
        return maxPopulation;
    }

    
    

    private void WorkerAssign( int amount)
    {
       
        unemployed -= amount;
        workers += amount;
      
    }

    private bool CanAssignWorker(int amount)
    {
        return unemployed >= amount;
    }

    // TODO: make this method a class
    private void BuildCost(int woodCost, int stoneCost, int workerAssign)
    {
        if(wood >= woodCost && stone >= stoneCost && unemployed >=workerAssign)
        {
            wood -= woodCost;
            stone -= stoneCost;
            unemployed -= workerAssign;
            workers += workerAssign;
        }
    }


    private void WoodProduction()
    {
        wood += lumberMill * 2;
    }

    private void ToolProduction()
    {
        tools += blacksmithhut * 4;
    }

    private void StoneProduction()
    {
        stone += quarry * 4;
    
    }

   

    private void UpdateText()
    {
        daysText.text = $"Day: {days}";
        //Resources
        populationText.text =$"Population: {Population()} / {GetMaxPopulation()} \n workers: {workers} \n Unemployed: {unemployed}";
        woodText.text =$"Wood: {wood}";
        foodText.text = $"Food: {food}";
        toolsText.text = $"Tools: {tools}";
        stoneText.text = $"stone: {stone}";
        ironText.text = $"Iron: {iron}";
        goldText.text = $"Gold: {gold}";
        //Buildings
        farmText.text = $"Farm: {farm}"; 
        houseText.text = $"House: {house}";
        lumberMillText.text = $"LumberMill: {lumberMill}";
        blacksmithhutText.text = $"Blacksmith: {blacksmithhut}";
        ironMinesText.text = $"Iron Mines: {ironMines}";
        quarryText.text = $"Iron Mines: {quarry}";
        goldMinesText.text = $"Gold Mines: {goldMines}";
    }

    private IEnumerator NotificationText(string text)
    {
        notificationText.gameObject.SetActive(true);
        notificationText.text = text;
        yield return new WaitForSeconds(2);
        notificationText.gameObject.SetActive(false);
        //notificationText.text = string.Empty ;

    }

    public void BuildHouse()
    {
        if (wood >= 2)
        {
            wood -= 2;
            house++;
            
            if (GetMaxPopulation() > Population())
            {
                unemployed = GetMaxPopulation();
                if (GetMaxPopulation() < Population())
                {
                    unemployed = GetMaxPopulation() - workers;
                }
            }
            UpdateText();

        }
        else
        {
            string text = $"You need more {2 - wood} wood";
            StartCoroutine(NotificationText(text));
        }
    }

    public void BuildFarm()
    {
        //izgradi se farma

        if (wood >= 10 && CanAssignWorker(2))
        {
            wood -= 10;
            farm++;
            WorkerAssign(2);
            UpdateText();
        }
        else
        {
            string text = $"You need more {19 - wood} wood or {2 - unemployed} people";
            StartCoroutine(NotificationText(text));
        }
    }

    public void BuildBlackSmithHut()
    {
        if (wood >= 5 && CanAssignWorker(2))
        {

            wood -= 5;
            WorkerAssign(2);
            blacksmithhut++;
            tools++;
            UpdateText();
            ironMinesText.gameObject.SetActive(true);
        }
        else
        {
            string text = $"You need {5 - wood} wood or {1 - unemployed} people";
            StartCoroutine(NotificationText(text));
        }
    }

    

    private void NewBuildingsUnlocked()
    {
        if (blacksmithhut == 1)
        {
            ironMinesButton.gameObject.SetActive(true);
            lumberMillButton.gameObject.SetActive(true);
            quarryButton.gameObject.SetActive(true);
            goldMinesButton.gameObject.SetActive(true);
            blacksmithhutCostText.gameObject.SetActive(false);
            blacksmithhutBuiltText.gameObject.SetActive(true);

            blacksmithHutButton.interactable = false;

        }
        else
        {
            ironMinesButton.gameObject.SetActive(false);
            lumberMillButton.gameObject.SetActive(false);
            quarryButton.gameObject.SetActive(false);
            goldMinesButton.gameObject.SetActive(false);
            blacksmithhutCostText.gameObject.SetActive(true);
            blacksmithhutBuiltText.gameObject.SetActive(false);

            blacksmithHutButton.interactable = true;
        }
    }



    public void BuildIronMine()
    {
        if (wood >= 10 && CanAssignWorker(2))
        {
            tools -= 2;
            ironMines++;
            WorkerAssign(2);
            UpdateText();

        }
        else
        {
            string text = $"You need {2 - tools} more tools or {2 - unemployed} more people";
            StartCoroutine(NotificationText(text));
        }
        
    }

    public void BuildLumberMill()
    {
        if (wood >= 5 && iron > 0 && CanAssignWorker(1))
        {
            tools -= 2;
            iron--;
            wood -= 5;
            WorkerAssign(1);
            lumberMill++;
            UpdateText();
        }
        else
        {
            string text = $"You need {5 - wood} more wood or {1 - iron} more iron or {2 - tools} more tools or {1 - unemployed} more people";
            StartCoroutine(NotificationText(text));
        }
    }

    public void BuildQuarry()
    {
        if (wood >= 10 && CanAssignWorker(2))
        {
            tools -= 4;
            quarry++;
            WorkerAssign(4);
            UpdateText();

        }
        else
        {
            string text = $"You need {2 - tools} more tools or {4 - unemployed} more people";
            StartCoroutine(NotificationText(text));
        }

    }

    public void BuildGoldMine()
    {
        if (wood >= 10 && CanAssignWorker(2))
        {
            tools -= 5;
            goldMines++;
            WorkerAssign(5);
            UpdateText();

        }
        else
        {
            string text = $"You need {2 - tools} more tools or {4 - unemployed} more people";
            StartCoroutine(NotificationText(text));
        }

    }

    public void PlayToolsAudio()
    {
        toolsAudio.PlayOneShot(toolsAudioClip);
    }

    private void WinCondition()
    {
        if(gold >= 100)
        {
            mainMenuManager.WinScenario();
            GameReset();
        }
            
    }
    private void GameReset()
    {
        gold = 0;
        iron = 0;
        stone = 0;
        tools = 0;
        food = 0;
        wood = 20;
        unemployed = 0;
        workers = 0;
        house = 0;
        farm = 0;
        blacksmithhut = 0;
        ironMines = 0;
        lumberMill = 0;
        quarry = 0;
        goldMines = 0;
        days = 0;
        timer = 0;
    }

    private void LoseCondition()
    {
        

        if (food <= -40)
        {
            mainMenuManager.LooseScenario();
            GameReset();
        }
    }

    private void SmallDisclaimerButton()
    {
        if(gold >= 50)
        {
            trollButton.gameObject.SetActive(true);
        }
        else
        {
            trollButton.gameObject.SetActive(false);
        }
    }
}
