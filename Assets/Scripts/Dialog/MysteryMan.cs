using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MysteryMan : MonoBehaviour
{
    [Header("General")]
    public GameObject dialogBox;
    public GameObject dialogScence;
    public GameObject dialogBG;
    public GameObject dialogChoices;
    public GameObject leftArea;
    public Transform leftAreaPivot;
    public GameObject leftWeapon;
    public GameObject rightArea;
    public Transform rightAreaPivot;
    public GameObject rightWeapon;
    private bool active = false;

    [Header("Dialog")]
    public SpriteRenderer Player;
    public SpriteRenderer Man;
    public Color unActiveSpeak = Color.gray;
    private Color activeSpeak = Color.white;
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI speakerText;
    public AudioSource audioSource;
    public GameObject choicePanel;
    public TextMeshProUGUI choiceText;
    private int currentIndex = 0;

    [System.Serializable]
    public class Choice
    {
        public string speaker;
        public string text_en;
        public string text_th;
    }

    List<Choice> choices = new List<Choice>
    {
        new Choice { speaker = "M", text_en = "I see the ambition inside you." },
        new Choice { speaker = "P", text_en = "(Who is he? His look is very untruthful.)" },
        new Choice { speaker = "M", text_en = "Hehe.. I feel that our meeting is destined do you want me to help you?" },
        new Choice { speaker = "P", text_en = "Why do you want to help me?" },
        new Choice { speaker = "M", text_en = "Nothing much I just want you to dethrone that tyrant king for me." },
        new Choice { speaker = "M", text_en = "I think we have same goal why don’t you accept my help?" },
    };

    private bool isDialogActive = false;

    void Update()
    {
        // Check if the player presses F to go to the next dialog
        if (Input.GetKeyDown(KeyCode.F) && isDialogActive)
        {
            DisplayDialog();
        }
    }


    void DisplayDialog()
    {
        if (currentIndex < choices.Count)
        {
            Choice choice = choices[currentIndex];
            dialogText.text = choice.text_en;

            if (choice.speaker == "M")
            {
                speakerText.text = "Mystery Man";

                Man.color = activeSpeak;
                Player.color = unActiveSpeak;
            }
            else
            {
                speakerText.text = "Player";

                Player.color = activeSpeak;
                Man.color = unActiveSpeak;
            }

            currentIndex++;
            isDialogActive = true;
        }
        else
        {
            Player.color = unActiveSpeak;
            Man.color = unActiveSpeak;

            DisplayChoices();
        }
    }

    void DisplayChoices()
    {
        dialogChoices.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!active)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                active = true;
                StartDialog();
            }
        }

    }

    private void StartDialog()
    {
        dialogBox.SetActive(true);
        dialogScence.SetActive(true);
        dialogBG.SetActive(true);
        isDialogActive = true;

        DisplayDialog();
    }

    private void EndDialog()
    {
        dialogBox.SetActive(false);
        dialogScence.SetActive(false);
        dialogBG.SetActive(false);
        dialogChoices.SetActive(false);
        isDialogActive = false;
    }

    public void AcceptMysteryMan()
    {
        EndDialog();

        Instantiate(leftWeapon, leftAreaPivot.position, Quaternion.identity);
        Instantiate(rightWeapon, rightAreaPivot.position, Quaternion.identity);
    }
    public void UnAcceptMysteryMan()
    {
        EndDialog();

        Instantiate(rightWeapon, rightAreaPivot.position, Quaternion.identity);
    }
}
