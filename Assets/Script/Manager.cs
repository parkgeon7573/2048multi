using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField]
    GameObject[] nums;
    [SerializeField]
    GameObject quit;
    [SerializeField]
    Text score;
    [SerializeField]
    Text bestScore;
    [SerializeField]
    Text plus;

    GameObject[,] Square = new GameObject[4, 4];
    Vector3 firstPos;
    Vector3 gap;
    bool isWait;
    bool isMove;
    bool stop;
    int currentScore;
    int x;
    int y;
    int blank;
    int combineCnt;
    int block;

    void Start()
    {
        Spawn();
        Spawn();
        bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
    }

    void Update()
    {
        QuitGame();
        Slide();
    }

    void QuitGame()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("종료하시겠습니까 로그 띄우기");
            Application.Quit();
        }
    }

    void Spawn()
    {
        while(true)
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);
            if (Square[x, y] == null)  
                break;
        }
        Square[x, y] = Instantiate(Random.Range(0, int.Parse(score.text) > 800 ? 4 : 8) > 0 ? nums[0] : nums[1]);
        Square[x, y].transform.position = new Vector3(1.2f * x - 1.8f, 1.2f * y - 1.8f, 0);
        Square[x, y].GetComponent<Animator>().SetTrigger("Spawn");
    }

    void Slide()
    {
        if (stop) return;
        if(Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isWait = true;
            firstPos = Input.GetMouseButtonDown(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position;
        }
        if(Input.GetMouseButton(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            gap = (Input.GetMouseButton(0) ? Input.mousePosition : (Vector3)Input.GetTouch(0).position) - firstPos;
            if (gap.magnitude < 100) return;
            gap.Normalize();

            if (isWait)
            {
                isWait = false;
                //up
                if (gap.y > 0 && gap.x > -0.5 && gap.x < 0.5)
                {
                    for (int k = 0; k <= 3; k++)
                        for (int j = 0; j <= 2; j++)
                            for (int i = 3; i >= j + 1; i--)
                            {
                                MoveOrCombine(k, i-1, k, i);
                            }
                }
                //down
                else if (gap.y < 0 && gap.x > -0.5 && gap.x < 0.5)
                {
                    for (int k = 0; k <= 3; k++)
                        for (int j = 3; j >= 1; j--)
                            for (int i = 0; i <= j - 1; i++)
                            {
                                MoveOrCombine(k, i + 1, k, i);
                            }
                }
                //right
                else if (gap.x > 0 && gap.y > -0.5 && gap.y < 0.5)
                {
                    for (int k = 0; k <= 3; k++)
                        for (int j = 0; j <= 2; j++)
                            for (int i = 3; i >= j + 1; i--)
                            {
                                MoveOrCombine(i-1, k, i, k);
                            }
                }
                //left
                else if (gap.x < 0 && gap.y > -0.5 && gap.y < 0.5)
                {
                    for (int k = 0; k <= 3; k++)
                        for (int j = 3; j >= 1; j--)
                            for (int i = 0; i <= j - 1; i++)
                            {
                                MoveOrCombine(i + 1, k, i, k);
                            }
                }
                else return;
                if (isMove)
                {

                    isMove = false; 
                    Spawn();
                    blank = 0;
                    combineCnt = 0;

                    //score
                    if(currentScore > 0)
                    {
                        plus.text = "+" + currentScore.ToString() + "    ";
                        plus.GetComponent<Animator>().SetTrigger("PlusBack");
                        plus.GetComponent<Animator>().SetTrigger("Plus");
                        score.text = (int.Parse(score.text) + currentScore).ToString();
                        if (PlayerPrefs.GetInt("BestScore", 0) < int.Parse(score.text)) PlayerPrefs.SetInt("BestScore", int.Parse(score.text));
                        bestScore.text = PlayerPrefs.GetInt("BestScore").ToString();
                        currentScore = 0;
                    }

                    for (x = 0; x <= 3; x++)
                        for (y = 0; y <= 3; y++)
                        {
                            if (Square[x, y] == null) { blank++; continue; }
                            if (Square[x, y].tag == "Combine") Square[x, y].tag = "Untagged";
                        }
                    if (blank == 0)
                    {
                        for (y = 0; y <= 3; y++)
                            for (x = 0; x <= 2; x++)
                            {
                                if (Square[x, y].name == Square[x + 1, y].name) combineCnt++;
                            }                                
                        for (x = 0; x <= 3; x++)
                            for (y = 0; y <= 2; y++)
                            {
                                if (Square[x, y].name == Square[x, y + 1].name) combineCnt++;
                            }
                        if (combineCnt == 0) { stop = true; quit.SetActive(true); return; }
                    }                    
                }                
            }
        }
    }

    void MoveOrCombine(int x1, int y1, int x2, int y2)
    {
        if(Square[x2, y2] == null && Square[x1, y1] != null)
        {
            isMove = true; 
            Square[x1, y1].GetComponent<Moving>().Move(x2, y2, false);
            Square[x2, y2] = Square[x1, y1];
            Square[x1, y1] = null;
        }
        if (Square[x1, y1] != null && Square[x2, y2] != null && Square[x1, y1].name == Square[x2, y2].name && Square[x1, y1].tag != "Combine" && Square[x2, y2].tag != "Combine")
        {
            isMove = true;
            for (block = 0; block <= 16; block++)
            {
                if (Square[x2, y2].name == nums[block].name + "(Clone)") break;
            }
            Square[x1, y1].GetComponent<Moving>().Move(x2, y2, true);
            Destroy(Square[x2, y2]);
            Square[x1, y1] = null;
            Square[x2, y2] = Instantiate(nums[block + 1]);
            Square[x2, y2].transform.position = new Vector3(1.2f * x2 - 1.8f, 1.2f * y2 - 1.8f, 0);
            Square[x2, y2].tag = "Combine";
            Square[x2, y2].GetComponent<Animator>().SetTrigger("Combine");
            currentScore += (int)Mathf.Pow(2, block + 2);
        }
    }

    public void Restart()
    {
        Debug.Log("재시작하시겠습니까 로그 띄우기");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


/*for (int x = 0; x < 4; x++)
{
    for (int y = 1; y < 4; y++)
    {
        if (Square[x, y] != null)
        {
            int ty = y;
            while (ty > 0)
            {
                int ny = ty - 1;
                if (Square[x, ny] == null)
                {
                    MoveOrCombine(x, ty, x, ny);
                    ty--;
                }
                else if (Square[x, ny].name == Square[x, ty].name && Square[x, ny].tag != "Combine" && Square[x, ty].tag != "Combine")
                {
                    MoveOrCombine(x, ty, x, ny);
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }
}
*/