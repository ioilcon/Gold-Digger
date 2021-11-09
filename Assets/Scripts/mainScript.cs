using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mainScript : MonoBehaviour
{
    // Размер игрового поля size x size
    [SerializeField]
    int size = 10;

    // Префаб клетки, из которых будет собираться поле 
    [SerializeField]
    GameObject cellPrefab;

    // Текстовое поле для отображения кол-ва собранного золота
    [SerializeField]
    GameObject goldAmount;

    // Текстовое поле для отображения оставшегося количества попыток
    [SerializeField]
    GameObject pickaxesAmount;

    // Кнопка для перезапуска игры
    [SerializeField]
    Button resetButton;

    // Начальное количество попыток
    [SerializeField]
    int pickaxes = 20;

    // Необходимое условие для победы
    [SerializeField]
    int winCondition = 3;

    // Счётчик собранного золота
    int winCounter = 0;

    // Шанс 1-100 на получение золота при раскопке клетки
    [SerializeField]
    int goldChance = 15;

    // Изначальная глубина клеток
    [SerializeField]
    int maxDepth = 3;

    // Массив глубин клеток
    int[,] depths;

    // Игровое поле - массив клеток
    GameObject[,] cells;

    // Флаг, определяющий, не законлилась ли игра
    bool inGame = true;

    // Цвет, на который изменяется клетка при уменьшении "глубины". Дельта цвета
    Color digColourDelta;

    void Start()
    {
        // Инициализация переменных при начале игры
        depths = new int[size, size];
        cells = new GameObject[size, size];
        inGame = true;
        goldAmount.GetComponent<Text>().text = "0";
        pickaxesAmount.GetComponent<Text>().text = pickaxes.ToString();
        digColourDelta = cellPrefab.GetComponent<Image>().color;

        for (int i = 0; i < size; ++i) 
            for(int j = 0; j < size; ++j)
            {
                // Создание игрового поля, заполнение его префабами клеток
                depths[i, j] = maxDepth;
                cells[i, j] = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity, transform);
                RectTransform rt = cells[i, j].GetComponent<RectTransform>();
                // Поле занимает весь экран в высоту и половину (четверть экрана справа и слева свободны) экрана в ширину
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
        // Перезагрузка игры
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        resetButton.onClick.RemoveAllListeners();
    }

    public void OnPointerDown(BaseEventData data)
    {
        PointerEventData ptr = (PointerEventData) data;
        // Определение клетки поля, на которую нажал игрок (курсор/палец)
        int i = (int) ((ptr.position.x - Screen.width / 4) / (Screen.width / 2) * size );
        int j = (int) (ptr.position.y / Screen.height * size);
        Text cellText = cells[i, j].GetComponent<RectTransform>().GetChild(0).GetComponent<Text>();
        if (cellText.text == "G!")
        {
            // Если обрабатываемый клик попал на клетку с золотом
            // Добавление золота в "копилку" 
            ++winCounter;
            goldAmount.GetComponent<Text>().text = winCounter.ToString();
            if (winCounter >= winCondition) inGame = false;
            cellText.text = "";
        }
        else if (inGame && depths[i, j] > 0)
        {
            // Если игра не окончена и обрабатываемый клик попал на закрытую клетку (глубина не максимальная)
            --depths[i, j];
            --pickaxes;
            pickaxesAmount.GetComponent<Text>().text = pickaxes.ToString();
            // RGB компоненты цвета клетки изменяются на соответствующие у digColourDelta
            Image cellImage = cells[i, j].GetComponent<Image>();
            cellImage.color += digColourDelta;
            // Клетка раскапывается, проверяется, есть ли тут золото и остались ли ещё попытки
            int isGold = Random.Range(0, 100);
            if (isGold <= goldChance) cellText.text = "G!";
            if (pickaxes <= 0) inGame = false;
        }
    }

    void Update()
    {
        // Затычка, тут должно быть окошко с окончанием игры и результатом
        if (winCounter >= winCondition) Debug.Log("You win! :)");
        if (pickaxes <= 0) Debug.Log("You lose :(");
    }
}