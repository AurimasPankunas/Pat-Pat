using UnityEngine;
using TMPro;
using System.Collections;

public class AnimalGraphicalFeedback : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float displayDuration = 1f;
    [SerializeField] private float fontSize = 3f;
    [SerializeField] private Color textColor = Color.yellow;
    [SerializeField] private GameObject player;
    [SerializeField] private string message = "Money earned: ";
    private double money = 0.0;
    private Animal animal;
    private PlayerBalance playerBalance;

    public void ShowFeedback()
    {
        //Time.timeScale = 20;
        animal = GetComponent<Animal>();
        playerBalance = FindFirstObjectByType<PlayerBalance>();
        StartCoroutine(SpawnFloatingText(message, true));
    }

    public IEnumerator SpawnFloatingText(string message, bool addMoney = true)
    {
        if(addMoney && animal != null)
        {
            money = playerBalance.AddPettingMoney(animal);
        } else if(addMoney)
        {
            money = -99999.99;
        }

        //Create the object entirely in code
        GameObject textObj = new GameObject("FloatingText");
        textObj.transform.position = transform.position + Vector3.up;

        //Assing textmesh components
        TextMeshPro tmp = textObj.AddComponent<TextMeshPro>();
        tmp.text = addMoney ? message + money.ToString() + "$" : message;
        tmp.fontSize = fontSize;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Center;

        float elapsed = 0f;

        //Move the text loop
        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;

            //Move the text up
            textObj.transform.position += Vector3.up * floatSpeed * Time.deltaTime;

            //Fading text
            float alpha = Mathf.Lerp(1f, 0f, elapsed / displayDuration);
            tmp.color = new Color(textColor.r, textColor.g, textColor.b, alpha);

            //Direct the text towards the player (player camera is used cause nothing else works and I have no idea why)
            player = Camera.main.gameObject;
            Vector3 dirToPlayer = player.transform.position - textObj.transform.position;
            textObj.transform.rotation = Quaternion.LookRotation(-dirToPlayer);
            yield return null;
        }

        Destroy(textObj);
    }
}