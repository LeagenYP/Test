using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Timer harvestTimerImage;
    public Timer eatingTimerImage;
    public Timer attackTimerImage;

    public Image peasantCreateTimerImage;
    public Image warriorCreateTimerImage;

    public Button peasantButton;
    public Button warriorButton;

    public Text peasantButtonText;
    public Text warriorButtonText;

    public Text resousesInfo;
    public TextMeshProUGUI winFinalScreenInfo;
    public TextMeshProUGUI loseFinalScreenInfo;

    public int peasantCount;
    public int warriorCount;
    public int wheatCount;

    public int wheatMadePerPeasant;
    public int wheatEatedPerWarrior;

    public int peasantCost;
    public int warriorCost;

    public float peasantCreateTime;
    public float warriorCreateTime;

    public int attackIncreaseUnitsCount;
    public int nextAttack;
    public int roundsWithoutAttack;
    public int roundNumber;

    public Text enemyInfo;
    public GameObject enemyInfoPanel;

    private float peasantTimer = -2;
    private float warriorTimer = -2;
    private bool gamePaused = false;
    private int allWheatCount = 0;
    private int killCount = 0;
    private int attackCount = 0;

    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject PauseScreen;

    public AudioListener generalSound;

    public AudioSource buttonClickAudio, peasantSpawnSound, warriorSpawnSound, battleSound, warriorFeedSound, winSound, loseSound;

    void Start()
    {
        UpdateTextData();  // �������� ������ �� ���-�� ��������
        SetUnitCostHintText();  // ������������ ���� �� ������� ��� ������
    }

    void Update()
    {
        CheckButtonsUnitEnable();  // �������� �� ���������� ������
        CheckWinOrLoseState();  // �������� ������� ������ ��� ���������

            if (attackTimerImage.Tick)  // ������ �����
        {

            warriorCount -= nextAttack;
            killCount += nextAttack;  // ��������� ��� �������� ��������� ���������� 

            if(killCount != 0)
            {
                attackCount++;  // ������� ���-�� ������. ������� �������, ������ ����� ������ ����������� ������ ����
            }

            if (nextAttack > 3)  // ������ ���� ����������� ��� �������. ����� 50% ���� �� ��, ��� ��������� ����� �� �������� ������ �����
            {
                int chanceToDecreaseEnemtCount;
                chanceToDecreaseEnemtCount = Random.Range(0, 2);
                nextAttack -= chanceToDecreaseEnemtCount;
            }

            nextAttack += attackIncreaseUnitsCount;
            roundNumber++;
            LateStartAttack();  // ������� ��� ���������� �����
        }

        if (harvestTimerImage.Tick)  // ������ ����� �������
        {
            wheatCount += peasantCount * wheatMadePerPeasant;
            allWheatCount += wheatCount;
        }

        if (eatingTimerImage.Tick)  // ������ �������� �������
        {
            warriorFeedSound.Play();
            wheatCount -= warriorCount * wheatEatedPerWarrior;
        }

        if (peasantTimer > 0)  // ������ ��� �������� �����������
        {
            peasantTimer -= Time.deltaTime;
            peasantCreateTimerImage.fillAmount = peasantTimer / peasantCreateTime;
        }
        else if (peasantTimer > -1)  // ����� �� -1, ����� �� ������������ ��������
        {
            peasantSpawnSound.Play();
            peasantCreateTimerImage.fillAmount = 1;
            peasantButton.interactable = true;
            peasantCount += 1;
            peasantTimer = -2;  // ������������ ������������� �������� ��� ����������� �������
        }

        if (warriorTimer > 0)  // �� ���������� � �������
        {
            warriorTimer -= Time.deltaTime;
            warriorCreateTimerImage.fillAmount = warriorTimer / warriorCreateTime;
        }
        else if (warriorTimer > -1)
        {
            warriorSpawnSound.Play();
            warriorCreateTimerImage.fillAmount = 1;
            warriorButton.interactable = true;
            warriorCount += 1;
            warriorTimer = -2;
        }
        UpdateTextData();  // � ����� ������� ���������� ����������� ������
    }

    public void CreatePeasant()  // �������� �����������
    {
        buttonClickAudio.Play();
        wheatCount -= peasantCost;
        peasantTimer = peasantCreateTime;
        peasantButton.interactable = false;
    }

    public void CreateWarrior()  // �������� �����
    {
        buttonClickAudio.Play();
        wheatCount -= warriorCost;
        warriorTimer = warriorCreateTime;
        warriorButton.interactable = false;
    }

    public void UpdateTextData()  // �������� �������� ���� �� ������ � ����� �� �� ���������
    {
        resousesInfo.text = peasantCount + "\n" + warriorCount + "\n\n" + wheatCount + "\n\n\n" + nextAttack;
        enemyInfo.text = "����� �� ������: " + (roundsWithoutAttack - roundNumber);

        winFinalScreenInfo.text = attackCount + "\n" + allWheatCount + "\n" + killCount;
        loseFinalScreenInfo.text = attackCount + "\n" + allWheatCount + "\n" + killCount;
    }

    public void SetUnitCostHintText()  // ��������� ���� ��� ��������
    {
        peasantButtonText.text = "������ �����������" + " (" + peasantCost + " �." + ")";
        warriorButtonText.text = "������ �����" + " (" + warriorCost + " �." + ")";
    }
    public void CheckButtonsUnitEnable()
    {
        //for peasantButton
        if (wheatCount < peasantCost)
        {
            peasantButton.interactable = false;
        }
        else if (peasantTimer == -2)
        {
            peasantButton.interactable = true;
        }

        //for warriorButton
        if (wheatCount < warriorCost)
        {
            warriorButton.interactable = false;
        }
        else if (warriorTimer == -2)
        {
            warriorButton.interactable = true;
        }
    }

    public void LateStartAttack()
    {
        if (roundNumber < roundsWithoutAttack)
        {
            nextAttack = 0;
        } else
        {
            enemyInfoPanel.gameObject.SetActive(false);
            battleSound.Play();
        }
    }

    public void PauseGame()
    {
        buttonClickAudio.Play();

        if (!gamePaused)
        {
            Time.timeScale = 0;
            PauseScreen.SetActive(true);
            gamePaused = true;
        }
        else
        {
            Time.timeScale = 1;
            PauseScreen.SetActive(false);
            gamePaused = false;
        }
    }

    private void CheckWinOrLoseState()
    {
        if (warriorCount < 0 || wheatCount < 0)
        {
            loseSound.Play();

            warriorCount = 0;
            wheatCount = 0;
            Time.timeScale = 0;
            LoseScreen.SetActive(true);
        }

        if (wheatCount > 1000)
        {
            winSound.Play();

            wheatCount = 1000;
            Time.timeScale = 0;
            WinScreen.SetActive(true);
        }
    }
    public void RestartGame()  // ��� �������� ������ ������������ ����� 
    {
        buttonClickAudio.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void MuteSound()  // ��� ����� �������� �������� � ������ 
    {
        buttonClickAudio.Play();

        if (generalSound.enabled == true)
        {
            generalSound.enabled = false;
        } else
        {
            generalSound.enabled = true;
        }
    }
}
