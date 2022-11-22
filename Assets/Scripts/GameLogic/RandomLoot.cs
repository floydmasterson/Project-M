using Kryz.CharacterStats.Examples;
using UnityEngine;

public class RandomLoot : MonoBehaviour
{
    public static RandomLoot Instance;

    [SerializeField][Range(0, 20)] private int howMany = 0;
    [SerializeField] private int[] table = { 50, 20, 10, 10, 10 };
    [SerializeField] private Item[] ItemList = new Item[20];
    [HideInInspector] public Item Item;
    private int total;
    private int randomNumber;

    void Start()
    {
    
        Instance = this;
        itemGen(howMany);
    }
    public void itemGen(int amount)
    {
        for (int i = 0; i < amount;)
        {
            foreach (var item in table)
            {
                total += item;
            }
            randomNumber = Random.Range(0, total);
            Debug.Log("new Number1: " + randomNumber);


            for (int a = 0; a < table.Length; a++)
            {
                if (randomNumber <= table[a])
                {
                    Item = ItemList[a];
                    Debug.Log(ItemList[a]);
                    Debug.Log("gain");

                    //load items into chest


                    amount = amount - 1;
                    Debug.Log(amount);
                    randomNumber = 0;
                    total = 0;

                    if (amount == 0)
                    {
                        Debug.Log("amount is zero");


                       
                    }
                }
                else
                {
                    randomNumber -= table[a];
                    Debug.Log("tryagain");
                    Debug.Log("new Number2: " + randomNumber);
                }

            }

        }
    }
}

