using UnityEngine;

public class Moving : MonoBehaviour
{
    bool isMove;
    bool _combine;
    int _x2;
    int _y2;
    // Update is called once per frame
    void Update()
    {
        if (isMove) Move(_x2, _y2, _combine);
    }

    public void Move(int x2, int y2, bool isCombine)
    {
        isMove = true;
        _combine = isCombine;
        _x2 = x2;
        _y2 = y2;
       transform.position = Vector3.MoveTowards(transform.position, new Vector3(1.2f * x2 - 1.8f, 1.2f * y2 - 1.8f, 0), 0.3f);
       if(transform.position == new Vector3(1.2f * x2 - 1.8f, 1.2f * y2 - 1.8f, 0))
        {
            isMove = false;
            if (isCombine)
            {
                _combine = false;
                Destroy(gameObject);
            }
        }
    }
}
