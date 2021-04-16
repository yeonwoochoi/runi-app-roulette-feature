using System;
using System.Collections;
using System.Collections.Generic;
using Generator;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace Control
{
    public class RouletteController : MonoBehaviour
    {
        [SerializeField] private Image roulettePlate;
        [SerializeField] private Transform needle;
        [SerializeField] private Image rewardImage;
        [SerializeField] private Image gaugeBar;
        [SerializeField] private Image[] displayItemSlot;
        [SerializeField] private RandomItemGenerator generator;

        private int itemCount = 6;
        private bool isRunning;
        private bool isButtonHeld;

        private float chargeAmount;
        public RandomItem currentItem;

        private IEnumerator rotateRoulette;

        private Dictionary<RandomItemType, int> countArr;
        private int matchCount = 0;

        
        private void Start()
        {
            SetRewardImageSlot();
            isRunning = false;
            isButtonHeld = false;
            chargeAmount = 0;
            rewardImage.sprite = null;
            rewardImage.color = new Color(255, 255, 255, 0);

            currentItem = generator.itemList[0];
            
            countArr = new Dictionary<RandomItemType, int>
            {
                [generator.itemList[0].Type] = 0,
                [generator.itemList[1].Type] = 0,
                [generator.itemList[2].Type] = 0,
                [generator.itemList[3].Type] = 0,
                [generator.itemList[4].Type] = 0,
                [generator.itemList[5].Type] = 0,
            };
        }

        
        // Set reward Image on roulette.
        private void SetRewardImageSlot()
        {
            for (var i = 0; i < displayItemSlot.Length; i++)
            {
                displayItemSlot[i].sprite = generator.rewardSprites[i];
            }
        }

        // Rotate Roulette.
        private IEnumerator RotateRoulette(int laps, RandomItem targetItem)
        {
            yield return new WaitForSeconds(0.1f);
            
            var lapAngle = laps * 360;
            var additionalAngle = ModifyTotalRotateAngle(currentItem, targetItem);
            var totalAngle = lapAngle + additionalAngle + 30;
            
            var delta = 0.008f;
            var startLerpValue = (float) GetStartLerpValue(delta, totalAngle, 3000);
            var totalAnglesMoved = 0f;
            
            while (true)
            {
                yield return null;

                if (startLerpValue < 0.01f) break;
                
                startLerpValue = Mathf.Lerp(startLerpValue, 0, delta);
                
                roulettePlate.transform.Rotate(0, 0, startLerpValue);

                totalAnglesMoved += startLerpValue;
            }
            
            yield return new WaitForSeconds(0.1f);
            
            ShowResult();
            
            CountResult();

            if (targetItem.Type == currentItem.Type)
            {
                matchCount++;
            }
        }

        // Calculate "startLerpValue" (Sum of accumulated "startLerpValue" = "totalAngle")
        private double GetStartLerpValue(float ratio, float target, int count)
        {
            return target * ratio / (1 - Math.Pow(1 - ratio, count));
        }


        // Show result on unity.
        public void ShowResult()
        {
            var closestIndex = -1;
            var closestDistance = 500f;
            var currentDistance = 0f;

            // Find the closest item with needle image.
            for (var i = 0; i < itemCount; i++)
            {
                currentDistance = Vector2.Distance(displayItemSlot[i].transform.position, needle.position);
                if (closestDistance > currentDistance)
                {
                    closestDistance = currentDistance;
                    closestIndex = i;
                }
            }

            // Catch error.
            if (closestIndex == -1)
            {
                Debug.Log("Something is wrong.");
                isRunning = false;
                return;
            }

            // Update currentItem value.
            currentItem = generator.itemList[closestIndex];
            
            // Correct rotate value after roulette stop rotating.
            roulettePlate.transform.rotation = Quaternion.Euler(Vector3.forward * 60 * closestIndex);
            
            // Display reward Image
            rewardImage.sprite = displayItemSlot[closestIndex].sprite;
            rewardImage.color = new Color(255, 255, 255, 255);
            rewardImage.preserveAspect = true;
            
            isRunning = false;
        }

        
        // This is called when end clicking "run button"
        public void StopCharging()
        {
            if (isRunning) return;

            isButtonHeld = false;
            
            isRunning = true;
            
            var laps = (int) (chargeAmount * 10f);
            
            rotateRoulette = RotateRoulette(laps, generator.PickItem());
            
            StartCoroutine(rotateRoulette);
        }
        
        
        // This is called when start clicking "run button"
        public void StartCharging()
        {
            if (isRunning) return;
            
            if (rotateRoulette != null) StopCoroutine(rotateRoulette);
            
            isButtonHeld = true;

            StartCoroutine(FillGaugeBar());
        }

        private IEnumerator FillGaugeBar()
        {
            chargeAmount = 0;
            gaugeBar.fillAmount = 0f;

            while (isButtonHeld)
            {
                yield return null;
                
                if (chargeAmount < 1)
                {
                    chargeAmount += Time.deltaTime * 0.5f;
                }
                else
                {
                    chargeAmount = 1;
                }
                
                gaugeBar.fillAmount = chargeAmount;
            }
        }
        
        
        private void CountResult()
        {
            countArr[currentItem.Type]++;
        }
        
        
        private float ModifyTotalRotateAngle(RandomItem current, RandomItem target)
        {
            var currentAngle = generator.itemList.FindIndex(value => value.Type == current.Type) * 60;
            var targetAngle = generator.itemList.FindIndex(value => value.Type == target.Type) * 60;

            if (targetAngle - currentAngle <= 0)
            {
                return targetAngle - currentAngle + 360;
            }
            
            return targetAngle - currentAngle;
        }
    }
}

