using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotsController : MonoBehaviour
{
    [Header("Properties Dots Trail Parabola")]
    [Tooltip("Array trail list where the ball will move")]
    [SerializeField] private List<Transform> _objectTrailsList = new List<Transform>();
    [Tooltip("Objects (dot) stored in the array as children")]
    [SerializeField] private GameObject _pointDotTrail;
    [Tooltip("An object in which the path of the ball is recorded as children")]
    [SerializeField] private GameObject _trailParabola;
    [Tooltip("Ball moving on the path")]
    [SerializeField] private GameObject _ball;

    [Header("Start and end objects of the parabola")]
    [Tooltip("Starting vertex of the parabola")]
    [SerializeField] private Transform _postionStart;
    [Tooltip("Ending vertex of the parabola")]
    [SerializeField] private Transform _postionEnd;

    [Header("Properties Parabola (Grow)")]
    [Tooltip("Height of the parabola")]
    [SerializeField] private float _heightParabola;
    [Tooltip("Increase in the height of the parabola over a period of time")]
    [SerializeField] private float _heightParabolaGrow = 1.5f;
    [Tooltip("Time of parabola rise")]
    [SerializeField] private float _timeToGrowParabola = 1f;

    //Parabolic Dots
    [Header("Parabolic Dots")]
    [Tooltip("The number of even dots in the parabola")]
    [SerializeField] private int _countDotsParabola=20;

    RectTransform _rectTranformEnd;                                                 //variable that takes data about the position of the parabola's end vertex
    Vector3 calculatePostionStart;                                                  //variable that enables calculations to be made at the initial vertex of the parabola
    Vector3 calculatePostionEnd;                                                    //variable that enables calculations to be made at the final vertex of the parabola
    BallController ballController;                                                  //script in the ball object

    private void Start()
    {
        _rectTranformEnd = _postionEnd.GetComponent<RectTransform>();
        ballController = _ball.GetComponent<BallController>();
    }


    #region Calculating The Properties Of a Parabola
    public void InitiationParabola()
    {
        SetPostionStarAndEnd();
        InstantieTrailPoints();
        StartCoroutine(CalculateEdgeEnd());
    }
    public void CalculatePoint()
    {
        if (_objectTrailsList.Count != 0)
        {
            CheckPositionDotsParabolic();
            CalculateValuePostionParabolic();
        }
    }
    private void CalculateHeightParabola()
    {
        _heightParabola += _heightParabolaGrow;
    }
    private IEnumerator CalculateEdgeEnd()
    {
        if (_rectTranformEnd.anchoredPosition.x<0)
        {
            _rectTranformEnd.anchoredPosition = new Vector2(CheckCorrcetDistanceToEnd(_rectTranformEnd,10f), _rectTranformEnd.anchoredPosition.y);
            CalculateHeightParabola();
            yield return new WaitForSeconds(_timeToGrowParabola);
            CalculateHeightParabola();
            _rectTranformEnd.anchoredPosition = new Vector2(CheckCorrcetDistanceToEnd(_rectTranformEnd, 10f), _rectTranformEnd.anchoredPosition.y);
            if (ballController.GetIsDrawParabolic())
                StartCoroutine(CalculateEdgeEnd());
        }
        else if (_rectTranformEnd.anchoredPosition.x >= 0)
        {
            if (ballController != null)
                ballController.InputThrowBall();
        }
       
    }
    private float CheckCorrcetDistanceToEnd(RectTransform rectTransformAnchorecPos, float additionCountDistance)
    {
        float distance = rectTransformAnchorecPos.anchoredPosition.x + additionCountDistance;
        if (distance >= 0)
        {
            return 0;
        }
        else
            return distance;
    }
    #endregion

    #region Validation Calculate
    private void CalculateValuePostionParabolic()
    {
        calculatePostionStart = _postionStart.transform.position;
        calculatePostionEnd = _postionEnd.transform.position;
    }
    private void SetPostionStarAndEnd()
    {
        _postionStart.transform.position = _ball.transform.position;
        _postionEnd.transform.position = _ball.transform.position;
    }
    #endregion

    #region Mechanism Dots Points
    private void CheckPositionDotsParabolic()
    {
        float count = _countDotsParabola;
        int j = 0;
        for (float i = 1; i < count + 1; i++)
        {
            Vector3 p = SampleParabola(calculatePostionStart, calculatePostionEnd, _heightParabola, i / count);

            if (i % 2 == 0)
            {
                if (j < _objectTrailsList.Count)
                {
                    _objectTrailsList[j].position = p;
                    j++;
                }
            }
        }

    }

    private void InstantieTrailPoints()
    {
        float count = _countDotsParabola;
        for (float i = 1; i < count + 1; i++)
        {
            Vector3 p = SampleParabola(calculatePostionStart, calculatePostionEnd, _heightParabola, i / count);

            if (i % 2 == 0)
            {
                GameObject point = (Instantiate(_pointDotTrail, p, Quaternion.identity));
                Transform transformPoint = point.GetComponent<Transform>();
                transformPoint.transform.SetParent(_trailParabola.transform);
                _objectTrailsList.Add(transformPoint);
            }
        }
    }
    #endregion

    #region Draw Gizmos
    void OnDrawGizmos()
    {
        //Draw the parabola by sample a few times
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(calculatePostionStart, calculatePostionEnd);
        float count = 20;
        Vector3 lastP = calculatePostionStart;
        for (float i = 0; i < count + 1; i++)
        {
            Vector3 p = SampleParabola(calculatePostionStart, calculatePostionEnd, _heightParabola, i / count);
            Gizmos.color = i % 2 == 0 ? Color.red : Color.black;
            Gizmos.DrawLine(lastP, p);
            lastP = p;
        }
    }
    #endregion

    #region Parabola sampling function
    Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
    #endregion

    #region Get And Set Value Dots
    public List<Transform> GetObjectTrailList()
    {
        return _objectTrailsList;
    }
    public void SetBallObject(GameObject ball)
    {
        _ball = ball;
    }
    public float GetTimeToGrowParabola()
    {
        return _timeToGrowParabola;
    }
    public void SetTimeToGrowParabola(float time)
    {
        _timeToGrowParabola = time;
    }
    #endregion
}
