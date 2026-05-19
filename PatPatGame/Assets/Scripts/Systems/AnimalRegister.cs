using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class AnimalRegister : MonoBehaviour
{
    [SerializeField]
    private AnimalManager animalManager;

    [SerializeField]
    private ShopItemDatabase shopItemDatabase;
    public List<AnimalData> registerAnimals;
    public int maxRegisterAnimals = 5;
    public event Action OnRegisterAnimalChanged;
    private TimeKeeper timeKeeper;
    private UIAnimalElement uIAnimalElement;

    void Start()
    {
        timeKeeper = FindFirstObjectByType<TimeKeeper>();
        GetDailyAnimals();
        // InvokeRepeating(nameof(GetDailyAnimals), 6f, 12f);
    }

    /// <summary>
    /// Adds animal data instance to the register list
    /// </summary>
    public void AddAnimalToRegister(AnimalData animalData)
    {
        registerAnimals.Add(animalData);
        OnRegisterAnimalChanged?.Invoke();
    }

    /// <summary>
    /// Removes animal from the register list based on index
    /// </summary>
    public AnimalData RemoveAnimal(int index)
    {
        if (!(index < registerAnimals.Count))
        {
            Debug.LogWarning("Index " + index + " is outside of registered animals range");
            return null;
        }

        AnimalData removedAnimal = registerAnimals[index];
        registerAnimals.RemoveAt(index);
        OnRegisterAnimalChanged?.Invoke();
        return removedAnimal;
    }

    /// <summary>
    /// Checks whether an animal can be taken in. 
    /// If there's enough spots - true, if not - false
    /// </summary>
    /// <returns></returns>
    public bool CanBeTakenIn()
    {
        int freeSpots = animalManager.spots.Count(s => s.isBought && !s.isOccupied);
        int miniAnimals = animalManager.miniAnimals.Count();
        if (freeSpots - miniAnimals > 0) 
            return true;
        return false;
    }

    /// <summary>
    /// Checks if an animal can be taken in and if it can removes it from the register and UI
    /// </summary>
    /// <param name="animal"></param>
    public void TakeInCheckAndRemove(
        AnimalData animal,
        Transform spawnPoint,
        TemplateContainer _UIanimalElement
    )
    {
        if(CanBeTakenIn())
        {
            _UIanimalElement.RemoveFromHierarchy();
            RemoveAnimal(animal, spawnPoint);
        }
    }



    /// <summary>
    /// Removes animal from the register list. If spawnPoint is given, spawns a mini animal
    /// </summary>
    /// <param name="animal"></param>
    public void RemoveAnimal(
        AnimalData animal,
        Transform spawnPoint
    )
    {
        registerAnimals.Remove(animal);
        OnRegisterAnimalChanged?.Invoke();
        if (spawnPoint != null)
        {
            animalManager.CreateMiniAnimal(animal, spawnPoint.position, spawnPoint.rotation);
        }
    }

    /// <summary>
    /// Creates a new animal data instance with randomized stats and puts it in the register
    /// </summary>
    public AnimalData CreateRandomAnimal()
    {
        int typeCount = animalManager.types.Count;
        int typeChoice = Random.Range(0, typeCount);
        AnimalType type = animalManager.types[typeChoice];
        string name = AnimalNameGenerator.GenerateName();
        int rarity = shopItemDatabase.RollDailyAnimalRarity();
        int level = Random.Range(1, 2);
        double happiness = Random.Range(0f, 1f);
        double food = Random.Range(0f, 1f);
        double water = Random.Range(0f, 1f);

        AnimalData animal = new AnimalData(type.id, name, rarity, level, happiness, food, water);
        AddAnimalToRegister(animal);

        return animal;
    }

    /// <summary>
    /// Creates animal with chest rarity
    /// </summary>
    public AnimalData CreateChestAnimal()
    {
        int chestLvl = GameManager.Instance.playerBalance.chestLevel;
        int typeCount = animalManager.types.Count;
        int typeChoice = Random.Range(0, typeCount);
        AnimalType type = animalManager.types[typeChoice];
        string name = AnimalNameGenerator.GenerateName();
        int rarity = shopItemDatabase.RollAnimalRarity(chestLvl);
        int level = Random.Range(1, 5);
        double happiness = Random.Range(0f, 1f);
        double food = Random.Range(0f, 1f);
        double water = Random.Range(0f, 1f);

        AnimalData animal = new AnimalData(type.id, name, rarity, level, happiness, food, water);
        AddAnimalToRegister(animal);

        return animal;
    }

    // Daily animal spawning test
    public void GetDailyAnimals()
    {
        if (registerAnimals.Count < maxRegisterAnimals)
        {
            var timePassed = DateTime.Now - timeKeeper.lastLogin;
            for (int i = 0; i < timePassed.Days; i++)
            {
                if (registerAnimals.Count == maxRegisterAnimals)
                {
                    return;
                }
                CreateRandomAnimal();
            }
        }
    }

    public AnimalRegisterSaveData Save()
    {
        AnimalRegisterSaveData data = new AnimalRegisterSaveData { animals = registerAnimals };
        return data;
    }

    public void Load(AnimalRegisterSaveData data)
    {
        registerAnimals.Clear();
        data.animals.ForEach(a => registerAnimals.Add(a));
    }

    private static class AnimalNameGenerator
    {
        static readonly string[] StartStrings = {
            "B", "Br", "J", "F", "S", "M", "C", "Ch", "L", "P",
            "K", "W", "G", "Z", "Tr", "T", "Gr", "Fr", "Pr", "N",
            "Sn", "R", "Sh", "St"
        };

        static readonly string[] ConnectiveStrings = {
            "ll", "tch", "l", "m", "n", "p", "r", "s", "t", "c", "rt", "ts"
        };

        static readonly string[] VowelStrings = { "a", "e", "i", "o", "u" };

        static readonly string[] EndStrings = { "ie", "o", "a", "ers", "ley" };

        static readonly Dictionary<char, string[]> VowelDictionary1 = new()
        {
            ['a'] = new[] { "nie", "bell", "bo", "boo", "bella", "s" },
            ['e'] = new[] { "ll", "llo", "", "o" },
            ['i'] = new[] { "ck", "e", "bo", "ba", "lo", "la", "to", "ta", "no", "na", "ni", "a", "o", "zor", "que", "ca", "co", "mi" },
            ['o'] = new[] { "nie", "ze", "dy", "da", "o", "ver", "la", "lo", "s", "ny", "mo", "ra" },
            ['u'] = new[] { "rt", "mo", "", "s" }
        };

        static readonly Dictionary<char, string[]> VowelDictionary2 = new()
        {
            ['a'] = new[] { "nny", "sper", "trina", "bo", "-bell", "boo", "lbert", "sko", "sh", "ck", "ishe", "rk" },
            ['e'] = new[] { "lla", "llo", "rnard", "cardo", "ffe", "ppo", "ppa", "tch", "x" },
            ['i'] = new[] { "llard", "lly", "lbo", "cky", "card", "ne", "nnie", "lbert", "nono", "nano", "nana", "ana", "nsy", "msy", "skers", "rdo", "rda", "sh" },
            ['o'] = new[] { "nie", "zzy", "do", "na", "la", "la", "ver", "ng", "ngus", "ny", "-mo", "llo", "ze", "ra", "ma", "cco", "z" },
            ['u'] = new[] { "ssie", "bbie", "ffy", "bba", "rt", "s", "mby", "mbo", "mbus", "ngus", "cky" }
        };

        static readonly string[] Blacklist = {
            "sex", "taboo", "fuck", "rape", "cock", "willy", "cum",
            "goock", "trann", "gook", "bitch", "shit", "pusie",
            "kike", "nigg", "puss"
        };

        static int RandoNext(int max) => Random.Range(0, max);
        static int RandoNext(int min, int max) => Random.Range(min, max);

        public static string GenerateName()
        {
            string source = "";
            int num = RandoNext(3, 6);

            // Start of string
            source = StartStrings[RandoNext(StartStrings.Length - 1)];

            // Alternate vowels and connectives
            for (int i = 1; i < num - 1; i++)
            {
                source += i % 2 != 0
                    ? VowelStrings[RandoNext(VowelStrings.Length)]
                    : ConnectiveStrings[RandoNext(ConnectiveStrings.Length)];

                if (source.Length >= num) break;
            }

            char lastChar = source[source.Length - 1];
            bool lastIsVowel = VowelStrings.Contains(lastChar.ToString());

            if (Random.value < 0.5 && !lastIsVowel)
            {
                // 50% chance: add an end string if last char is not a vowel
                source += EndStrings[RandoNext(EndStrings.Length)];
            }
            else if (lastIsVowel)
            {
                // 80% chance: append from vowel dictionaries
                if (Random.value < 0.8)
                {
                    char vowelKey = source[source.Length - 1];
                    if (source.Length <= 3)
                    {
                        var dict = VowelDictionary2[vowelKey];
                        source += dict[RandoNext(dict.Length - 1)];
                    }
                    else
                    {
                        var dict = VowelDictionary1[vowelKey];
                        source += dict[RandoNext(dict.Length - 1)];
                    }
                }
            }
            else
            {
                // Fallback: add a random vowel
                source += VowelStrings[RandoNext(VowelStrings.Length)];
            }

            // Fix double-vowel patterns: insert consonant between them
            for (int i = source.Length - 1; i > 2; i--)
            {
                char c = source[i];
                if (!VowelStrings.Contains(c.ToString())) continue;

                char twoBack = source[i - 2];
                if (!VowelStrings.Contains(twoBack.ToString())) continue;

                char between = source[i - 1];
                string insert = between switch
                {
                    'c' => "k",
                    'l' => "n",
                    'r' => "k",
                    _ => null
                };

                if (insert != null)
                {
                    source = source.Substring(0, i) + insert + source.Substring(i);
                    i--;
                }
            }

            // Small chance to double a short name (e.g. "Ka-Ka")
            if (source.Length <= 3 && Random.value < 0.1)
            {
                source = Random.value < 0.5
                    ? source + source
                    : source + "-" + source;
            }

            // If very short and ends in 'e', maybe add m/p/b
            if (source.Length <= 2 && source[source.Length - 1] == 'e')
            {
                float r = Random.value;
                source += r < 0.3 ? "m" : (r < 0.5 ? "p" : "b");
            }

            // Blacklist check
            string lower = source.ToLower();
            if (Blacklist.Any(word => lower.Contains(word)))
                source = Random.value > 0.5 ? "Bobo" : "Wumbus";

            return source;
        }
    }
}

[System.Serializable]
public struct AnimalRegisterSaveData
{
    public List<AnimalData> animals;
}
