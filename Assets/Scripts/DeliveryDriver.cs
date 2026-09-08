using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DeliveryDriver : MonoBehaviour
{
    [Header("배달원 설정")]
    public float moveSpeed = 8f;
    public float rotationSpeed = 10.0f;

    [Header("상태")]
    public float currentMoney = 0;
    public float batteryLevel = 100f;
    public int deliveryCount = 0;

    //이벤트
    [System.Serializable]
    public class DriverEvents
    {
        [Header("이동 Event")]
        public UnityEvent OnMoveStarted;
        public UnityEvent OnmoveStopped;

        [Header("상태 변화 Event")]
        public UnityEvent<float> OnMoneyChanged;
        public UnityEvent<float> OnBatteryCharged;
        public UnityEvent<int> OnDeliveryCountCharged;

        [Header("경고 Event")]
        public UnityEvent OnLowBattery;
        public UnityEvent OnLowBatteryEmpty;
        public UnityEvent OnDeliveryCompleted;
    }

    public DriverEvents driverEvents;
    public bool isMoving = false;

    void Start()
    {
        //초기 상태 Event 발생
        driverEvents.OnMoneyChanged?.Invoke(currentMoney);
        driverEvents.OnBatteryCharged?.Invoke(batteryLevel);
        driverEvents.OnDeliveryCountCharged?.Invoke(deliveryCount);
    }


    void Update()
    {
        HandleMovemet();
    }
    void ChargeBattery(float amount)
    {
        float oldBattery = batteryLevel;
        batteryLevel += amount;
        batteryLevel = Mathf.Clamp(batteryLevel, 0, 100);

        //배터리 변화 Event 발생
        driverEvents.OnBatteryCharged?.Invoke(batteryLevel);

        //배터리 상태에 따른 경고
        if (oldBattery > 20f && batteryLevel <= 20f)
        {
            driverEvents.OnLowBattery?.Invoke(); //배터리 부족 이벤트 발생
        }
        if (oldBattery > 0 && batteryLevel <= 0)
        {
            driverEvents.OnLowBatteryEmpty?.Invoke(); //배터리 
        }
    }

    void HandleMovemet()
    {
        
        //배터리 체크
        if (batteryLevel <= 0)
        {
            if(isMoving)
            {
                StopMoving();
            }
            return;
        }

        //입력 받기
        Vector2 input = Keyboard.current != null ? new Vector2(
            (Keyboard.current.dKey.isPressed ? 1 : 0) -
            (Keyboard.current.aKey.isPressed ? 1 : 0),
            (Keyboard.current.wKey.isPressed ? 1 : 0) -
            (Keyboard.current.sKey.isPressed ? 1 : 0)
            ) : Vector2.zero;
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        if (moveDirection.magnitude > 0.1f)
        {
            if (!isMoving)
            {
                StopMoving();
            }

            //이동 처리
            moveDirection = moveDirection.normalized;
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

            //회전 처리
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
            ChargeBattery(-Time.deltaTime * 3.0f);
        }
    }
    void StartMoving()
    {
        isMoving = true;
        driverEvents.OnMoveStarted?.Invoke();
    }
    void StopMoving()
    {
        isMoving = false;
        driverEvents.OnmoveStopped?.Invoke();
    }

    public void AddMoney(float amount)
    {
        currentMoney += amount;
        driverEvents.OnMoneyChanged?.Invoke(currentMoney); //돈 획득 후 이벤트 처리
    }

    public void CompleteDelivery()
    {
        deliveryCount++;
        float reward = Random.Range(3000, 8000);
        AddMoney(reward);
        driverEvents.OnDeliveryCountCharged?.Invoke(deliveryCount);
        driverEvents.OnDeliveryCompleted?.Invoke();
    }

    public void ChargeBattery()
    {
        ChargeBattery(100f - batteryLevel);
    }

    public string GetStatusText()
    {
        return $"돈 : {currentMoney:F0} 원 | 배터리 : {batteryLevel:F1}% | 배달 : {deliveryCount}건";
    }

    public bool CanMove()
    {
        return batteryLevel > 0;
    }
}

