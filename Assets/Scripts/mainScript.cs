using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mainScript : MonoBehaviour
{
    [SerializeField]
    int size = 10;

    [SerializeField]
    GameObject cellPrefab;

    [SerializeField]
    GameObject goldAmount;

    [SerializeField]
    GameObject pickaxesAmount;

    [SerializeField]
    Button resetButton;

    [SerializeField]
    int pickaxes = 20;

    [SerializeField]
    int winCondition = 3;

    int winCounter = 0;

    [SerializeField]
    int goldChance = 15;

    int[,] depths;
    GameObject[,] cells;
    bool inGame = true;
    Color digColourDelta;

    void Start()
    {
        depths = new int[size, size];
        cells = new GameObject[size, size];
        inGame = true;
        goldAmount.GetComponent<Text>().text = "0";
        pickaxesAmount.GetComponent<Text>().text = pickaxes.ToString();
        digColourDelta = cellPrefab.GetComponent<Image>().color;
        for (int i = 0; i < size; ++i) 
            for(int j = 0; j < size; ++j)
            {
                depths[i, j] = 3;
                cells[i, j] = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, transform);
                RectTransform rt = cells[i, j].GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(i * 0.5f / size + 0.25f, j * 1.0f / size);
                rt.anchorMax = new Vector2((i + 1) * 0.5f / size + 0.25f, (j + 1) * 1.0f / size);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
    }

    private void OnEnable()
    {
        resetButton.onClick.AddListener(() => buttonCallBack());
    }

    private void buttonCallBack()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        resetButton.onClick.RemoveAllListeners();
    }

    public void OnPointerDown(BaseEventData data)
    {
        PointerEventData ptr = (PointerEventData) data;
        int i = (int) ((ptr.position.x - Screen.width / 4) / (Screen.width / 2) * size );
        int j = (int) (ptr.position.y / Screen.height * size);
        // Debug.Log("Click on [" + i + ", " + j + "] cell");
        if (inGame)
        {
            Text cellText = cells[i, j].GetComponent<RectTransform>().GetChild(0).GetComponent<Text>();
            if (cellText.text == "G!")
            {
                ++winCounter;
                goldAmount.GetComponent<Text>().text = winCounter.ToString();
                if (winCounter >= winCondition) inGame = false;
                cellText.text = ""; // depths[i, j].ToString();
            }
            else if (depths[i, j] > 0)
            {
                --depths[i, j];
                --pickaxes;
                pickaxesAmount.GetComponent<Text>().text = pickaxes.ToString();
                Image cellImage = cells[i, j].GetComponent<Image>();
                if (pickaxes <= 0) inGame = false;
                //cellText.text = depths[i, j].ToString();
                cellImage.color += digColourDelta;
                int isGold = Random.Range(0, 100);
                if (isGold <= goldChance) cellText.text = "G!";
                //else cellText.text = depths[i, j].ToString();
            }
        }
    }

    void Update()
    {
        if (winCounter >= winCondition) Debug.Log("You win! :)");
        if (pickaxes <= 0) Debug.Log("You lose :(");
    }
}