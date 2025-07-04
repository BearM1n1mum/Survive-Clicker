using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameManagerManager : MonoBehaviour
{
    // population, wood, gold, food, stone, iron, tools, days

    [Header("Resources")]
    [SerializeField] private int days; //DONE
    [SerializeField] private int workers; //DONE
    [SerializeField] private int unemployed; //DONE
    [SerializeField] private int wood; //DONE
    [SerializeField] private int gold;
    [SerializeField] private int food; //DONE
    [SerializeField] private int stone;
    [SerializeField] private int iron;
    [SerializeField] private int tools;

    [Header("Building")]
    //farm, house,iron mines,gold mines,woodcutter, blacksmith,quarry
    [SerializeField] private int farm; // 1 house takes 4 people, more houses more population  //DONE
    [SerializeField] private int house; //DONE
    [SerializeField] private int ironMines;
    [SerializeField] private int goldMines;
    [SerializeField] private int quarry;
    [SerializeField] private int woodcutter; //DONE
    [SerializeField] private int blacksmith;

    [Header("Resources Text")]
    [SerializeField] private TMP_Text daysText;
    [SerializeField] private TMP_Text populationText;
    [SerializeField] private TMP_Text woodText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text ironText;

    [Header("Buildings Text")]
    [SerializeField] private TMP_Text houseText;
    [SerializeField] private TMP_Text farmText;
    [SerializeField] private TMP_Text woodCutterText;
    [SerializeField] private TMP_Text notificationText;

    bool IsGameRunning = false;


    private float timer;

    

    private void Update()
    {
        // one minute is one day
        TimeOfDay();

        if(Input.GetKeyDown(KeyCode.Alpha1))
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
        if (timer >= 1)
        {
            days++;
            FoodGathering();
            FoodProduction();
            WoodProduction();
            FoodConsumption(1);
            UpdateText();
            IncreasePopulation();
            timer = 0;
        }
        
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
            woodcutter++;
        }
        else
        {
            string text = $"You need more {5 - wood} wood or {1-iron} iron or {1 - unemployed} people";
            StartCoroutine(NotificationText(text));
        }
    }

    private void WoodProduction()
    {
        wood += woodcutter * 2;
    }

    private void UpdateText()
    {
        daysText.text = $"Day: {days}";
        //Resources
        populationText.text =$"Population: {Population()} / {GetMaxPopulation()} \n workers: {workers} \n Unemployed: {unemployed}";
        woodText.text =$"Wood: {wood}";
        foodText.text = $"Food: {food}";
        ironText.text = $"Iron: {iron}";
        //Buildings
        farmText.text = $"Farm: {farm}"; 
        houseText.text = $"House: {house}";
        woodCutterText.text = $"Woodcutter: {woodcutter}";
    }

    private IEnumerator NotificationText(string text)
    {
        notificationText.text = text;
        yield return new WaitForSeconds(2);
        notificationText.text = string.Empty ;
    }

}
