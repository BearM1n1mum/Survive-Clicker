using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerManager : MonoBehaviour
{
    // population, wood, gold, food, stone, iron, tools, days

    [Header("Resources")]
    [SerializeField] private int days; //DONE
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
    [SerializeField] private int house; //DONE gives 1 person every day
    [SerializeField] private int ironMines; //gives iron, every worker in iron mine produces 1 iron/day
    [SerializeField] private int goldMines; //gives gold, every worker in iron mine produces 1 gold/day
    [SerializeField] private int quarry; //gives stone every, worker in iron mine produces 4 stones/day
    [SerializeField] private int lumberMill; //DONE
    [SerializeField] private int blacksmithhut; // gives tools,needs 1 worker, costs 5 wood to build,
                                             // has to have tools to be able to send people to iron/gold mines / quarry

    [Header("Resources Text")]
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text populationText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text toolsText;
    [SerializeField] private TMP_Text stoneText;
    [SerializeField] private TMP_Text ironText;
    [SerializeField] private TMP_Text goldText;

    [Header("Buildings Text")]
    [SerializeField] private TMP_Text houseText;
    [SerializeField] private TMP_Text farmText;
    [SerializeField] private TMP_Text lumberMillText;
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private TMP_Text blacksmithhutText;

    bool IsGameRunning = false;
    public Image daysCount;

    private float timer;
    private float fillTimer;
    private int dayDuration = 12;



    private void Update()
    {
        // one minute is one day
        TimeOfDay();
        FillImage();

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
            Time.timeScale = 8;
        }
    }
    public void InitialiseGame()
    {
        IsGameRunning = true;
        UpdateText();
    }

    private void TimeOfDay()
    {
        if(!IsGameRunning)
        {
            return;
        }
        timer += Time.deltaTime;
        if (timer >= dayDuration)
        {
            days++;
            daysCount.fillAmount = 0;
            FoodGathering();
            FoodProduction();
            WoodProduction();
            FoodConsumption(1);
            UpdateText();
            IncreasePopulation();
            timer = 0;
        }
        
    }
    private void FillImage()
    {
        fillTimer += Time.deltaTime;
        daysCount.fillAmount = fillTimer / dayDuration;
    }

    private void IncreasePopulation()
    {
        if (days % 2 == 0)
        {
            if (GetMaxPopulation() > Population())
            {
                unemployed += house;
                if(GetMaxPopulation() < Population())
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

    private void FoodGathering()
    {
        food += unemployed / 2;
    }

    private void FoodProduction()
    {
        food += farm * 4;
    }

    private int GetMaxPopulation() //number of max house * 4
    {

        int maxPopulation = house * 4;
        return maxPopulation;
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
    public void BuildHouse()
    {
        if(wood >= 2)
        {
            wood -= 2;
            house++;
            UpdateText();

        }
        else
        {
            string text = $"You need more {2 - wood} wood";
            StartCoroutine(NotificationText(text));
        }
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

    public void BuildWoodCutter()
    {
        if (wood >= 5 && iron > 0 && CanAssignWorker(1))
        {
            iron--;
            wood -= 5;
            WorkerAssign(1);
            UpdateText();
            lumberMill++;
        }
        else
        {
            string text = $"You need more {5 - wood} wood or {1-iron} iron or {1 - unemployed} people";
            StartCoroutine(NotificationText(text));
        }
    }
    public void BuildBlackSmithHut()
    {
        if(wood>=5 && CanAssignWorker(1))
        {

        wood -= 5;
        WorkerAssign(1);
        UpdateText();
        blacksmithhut++;
        }
        else
        {
            string text = $"You need {5 - wood} or {1 - unemployed}";
            StartCoroutine(NotificationText(text));
        }
    }

    private void WoodProduction()
    {
        wood += lumberMill * 2;
    }

    private void UpdateText()
    {
        daysText.text = $"Day: {days}";
        //Resources
        populationText.text =$"Population: {Population()} / {GetMaxPopulation()} \n workers: {workers} \n Unemployed: {unemployed}";
        woodText.text =$"Wood: {wood}";
        foodText.text = $"Food: {food}";
        ironText.text = $"Iron: {tools}";
        ironText.text = $"stone: {stone}";
        ironText.text = $"Iron: {iron}";
        ironText.text = $"Gold: {gold}";
        //Buildings
        farmText.text = $"Farm: {farm}"; 
        houseText.text = $"House: {house}";
        lumberMillText.text = $"LumberMill: {lumberMill}";
        blacksmithhutText.text = $"Blacksmith: {blacksmithhut}";
    }

    private IEnumerator NotificationText(string text)
    {
        notificationText.text = text;
        yield return new WaitForSeconds(2);
        notificationText.text = string.Empty ;
    }

}
