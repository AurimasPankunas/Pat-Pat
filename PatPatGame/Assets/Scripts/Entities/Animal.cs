using System;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Animal : MonoBehaviour
{
    private const double MinimumHappiness = 0.01;
    private const double DrainPerSecond   = 1.0 / (72.0 * 3600.0);

    public AnimalData data;
    [SerializeField] private string animalName;
    [SerializeField] private AnimalType type;
    [SerializeField] private int rarity = 1;
    [SerializeField] private AudioClip idleSound;
    [field:SerializeField] public ParticleSystem statParticleSystem { get; private set; }
    private double minStatDifferenceForParticles = 0.02;
    private SoundManager soundManager;
    private EffectPlayer effectPlayer;


    void Awake()
    {
        soundManager = GameManager.Instance.soundManager;
        effectPlayer = GameManager.Instance.effectPlayer;
    }
    // play idle sound randomly (example + testing)
    void FixedUpdate()
    {
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (soundManager.isTesting && idleSound != null && rand < 0.002)
            soundManager.PlaySoundAtPosition(idleSound, transform.position);
    }

    private static double RarityMultiplier(int rarity) => Math.Pow(1.6, rarity - 1);
    private static double LevelMultiplier(int level) => Math.Pow(level, 0.6);
    private static double NeedsEffect(double food, double water)
    {
        double needs = food * 0.5 + water * 0.5;
        return 0.1 + needs * 0.9;
    }
    private static double BondEffect(double bond) => 1.0 + bond * 0.5;

    public void Initialize(string name, AnimalType type, int rarity)
    {
        data = new AnimalData(type.id, name, rarity);
    }
    public void Initialize()
    {
        data = new AnimalData(type.id, animalName, rarity);
    }

    public void UpdateHappiness(bool isOnline, double afkProgress = 0, double dt = 1)
    {
        double k = isOnline
            ? 0.0000020
            : 0.0000032 + (0.0000035 * afkProgress);

        double newHappiness = data.happiness - k * dt * Math.Pow(data.happiness, 1.15);
        data.happiness = newHappiness < MinimumHappiness ? MinimumHappiness : newHappiness;
    }
    public void UpdateBond(double dt = 1)
    {
        double increase = 0.00001 * dt * Math.Pow(data.happiness, 1.5);
        data.bond = Math.Min(4.0, data.bond + increase);
        // PlayStatsParticleEffect(data.bond / 4.0, Color.red);
    }
    public void UpdateNeeds(double dt = 1)
    {
        data.food  = Math.Max(0.0, data.food  - DrainPerSecond * dt);
        data.water = Math.Max(0.0, data.water - DrainPerSecond * dt);
    }
    public void UpdateFood(double foodAmount)
    {
        data.food = Math.Clamp(data.food + foodAmount, 0.0, 1.0);
        PlayStatsParticleEffect(data.food, Color.sandyBrown);
    }

    public void UpdateWater(double waterAmount)
    {
        data.water = Math.Clamp(data.water + waterAmount, 0.0, 1.0);
        PlayStatsParticleEffect(data.water, Color.lightBlue);
    }

    public double IncomeCalculation(bool isOnline, double @base = 0.01, double afkProgress = 0)
    {
        double r     = RarityMultiplier(data.rarity);
        double l     = LevelMultiplier(data.level);
        double needs = NeedsEffect(data.food, data.water);
        double b     = BondEffect(data.bond);

        double onlineMultiplier = isOnline
            ? 1.0
            : Math.Exp(-3.2 * afkProgress);

        return @base * r * l * data.happiness * b * onlineMultiplier * needs;
    }

    public void PettingHappinessIncrease(int gloveRarity)
    {
        double difference = data.happiness;

        double bonus = 0.20 + 0.10 * (gloveRarity - 1);
        data.happiness = Math.Min(data.happiness + bonus, 1.0);

        difference = data.happiness - difference;
        PlayStatsParticleEffect(data.happiness, difference, Color.yellow);
    }

    public double PettingIncome(int gloveRarity)
    {
        PettingHappinessIncrease(gloveRarity);
    
        // Naudojame tiesioginį gloveRarity, nes Pow(x, 1) = x
        double income =
            (20.0 / 13.5) *
            RarityMultiplier(data.rarity) *
            LevelMultiplier(data.level) *
            (1.0 + data.happiness) *
            BondEffect(data.bond) *
            (1.0 + (double)gloveRarity) * NeedsEffect(data.food, data.water);
    
        return Math.Round(income, 2);
    }

    public void LevelUp()
    {
        data.level += 1;
    }

    public void SetAnimalName(string name)
    {
        data.animalName = name;
    }

    private void PlayStatsParticleEffect(double value, double difference, Color color)
    {
        if (value > 0.99 && difference > minStatDifferenceForParticles && statParticleSystem != null && effectPlayer != null)
        {
            EffectOptions options = new EffectOptions { color = color };
            effectPlayer.Play(statParticleSystem, transform.position, Quaternion.identity, options);
        }
    }

    private void PlayStatsParticleEffect(double value, Color color)
    {
        if (value > 0.99 && statParticleSystem != null && effectPlayer != null)
        {
            EffectOptions options = new EffectOptions { color = color };
            effectPlayer.Play(statParticleSystem, transform.position, Quaternion.identity, options);
        }
    }
}
