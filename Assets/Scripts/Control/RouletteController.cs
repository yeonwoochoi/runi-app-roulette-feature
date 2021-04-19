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
        /// <summary> Roulette image to rotate in unity </summary>
        [SerializeField] private Image roulettePlate;
        
        /// <summary> A needle at the top of the roulette and indicates what kind of reward is received. </summary>
        [SerializeField] private Transform needle;
        
        /// <summary> An image showing what rewards you have received </summary>
        [SerializeField] private Image rewardImage;
        
        /// <summary> A progress bar that fills while holding down the "Run" button, which is a button for rotating the Roulette. </summary>
        [SerializeField] private Image progressBar;
        
        /// <summary> Images of reward corresponding to the divided areas in the roulette. </summary>
        [SerializeField] private Image[] displayItemSlot;
        
        /// <summary>  </summary>
        [SerializeField] private RandomItemGenerator generator;

        /// <summary> The number of rewards corresponding to the divided areas in the roulette. </summary>
        private int itemCount;
        
        /// <summary> Whether the roulette is running </summary>
        private bool isRunning;
        
        /// <summary> Whether the "Run" button is being pressed or not </summary>
        private bool isButtonHeld;

        /// <summary> The amount of progress bar filled while pressing the "Run" button </summary>
        private float chargeAmount;
        
        /// <summary> The item that the <c>needle</c> is currently pointing to while the roulette is stopped. </summary>
        public RandomItem currentItem;

        /// <summary> Variable that points the <c>RotateRoulette</c>, which is a coroutine method that rotates the roulette. </summary>
        private IEnumerator rotateRoulette;

        /// <summary> Dictionary for counting the accumulated number of reward items. </summary>
        private Dictionary<RandomItemType, int> countArr;
        
        /// <summary> A number indicating how many times the expected reward item and the actual item matched. </summary>
        private int matchCount = 0;
        
        private void Start()
        {
            itemCount = displayItemSlot.Length;
            
            SetRewardImageSlot();
            
            isRunning = false;
            isButtonHeld = false;
            
            chargeAmount = 0;
            
            ShowRewardImage(false);
            
            // Set the currentItem pointed to by needle when the game is executed for the first time.
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

        
        /// <summary>
        /// Set sprites of reward items on each <c>displayItemSlot</c>.
        /// </summary>
        private void SetRewardImageSlot()
        {
            for (var i = 0; i < displayItemSlot.Length; i++)
            {
                displayItemSlot[i].sprite = generator.rewardSprites[i];
            }
        }

        /// <summary>
        /// A coroutine method that runs a roulette and show results of reward item.
        /// </summary>
        /// <param name="laps">The number of rotations determined in proportion to the <c>fillAmount</c> value.</param>
        /// <param name="targetItem">The reward item that the needle will finally point to as the roulette rotates.</param>
        /// <returns></returns>
        private IEnumerator RotateRoulette(int laps, RandomItem targetItem)
        {
            yield return new WaitForSeconds(0.1f);
            
            // Convert laps to angle.
            var lapAngle = laps * 360;
            
            // Get additional angle to arrive the area corresponding to the parameter "targetItem". 
            var additionalAngle = GetAdditionalAngle(currentItem, targetItem);
            
            // Calculate total angle that the roulette should rotate.
            var totalAngle = lapAngle + additionalAngle + 30;
            
            // This local variable "totalAngleMoved" is the angle at which the roulette actually rotates.
            // The local variable "totalAngle" is the expected value that the roulette should rotate.
            var totalAngleMoved = 0f;
            
            // Ratio of linear interpolation.
            var ratio = 0.008f;
            
            // Set initial lerp value.
            var startLerpValue = (float) GetStartLerpValue(ratio, totalAngle, 3000);

            // Rotate roulette by using linear interpolation.
            while (true)
            {
                yield return null;

                // Stop roulette when the rotation degree approaches zero.
                if (startLerpValue < 0.01f) break;
                
                startLerpValue = Mathf.Lerp(startLerpValue, 0, ratio);
                
                roulettePlate.transform.Rotate(0, 0, startLerpValue);

                // To measure the total angles that roulette is actually rotated.
                totalAngleMoved += startLerpValue;
            }
            
            yield return new WaitForSeconds(0.1f);
            
            ShowResult();
            
            CountResult();

            if (targetItem.Type == currentItem.Type)
            {
                matchCount++;
            }
        }

        /// <summary>
        /// Calculate "startLerpValue (Sum of accumulated "startLerpValue" = "totalAngle")
        /// </summary>
        /// <param name="ratio"></param>
        /// <param name="target"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private double GetStartLerpValue(float ratio, float target, int count)
        {
            return target * ratio / (1 - Math.Pow(1 - ratio, count));
        }


        /// <summary>
        /// After the rotation of the roulette stops, the reward item indicated by the needle is displayed in <c>rewardImage</c>.
        /// The image of item closest to the <c>needle</c> image among the item images of <c>displayItemSlot</c> is given as reward.
        /// </summary>
        public void ShowResult()
        {
            var closestIndex = -1;
            var closestDistance = 500f;
            var currentDistance = 0f;

            // Find the closest item with "needle" image.
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

            // Update "currentItem" value.
            currentItem = generator.itemList[closestIndex];
            
            // Rotate the roulette additionally to set the center of the area corresponding to the reward item to come to the "needle".
            roulettePlate.transform.rotation = Quaternion.Euler(Vector3.forward * 60 * closestIndex);
            
            // Display reward Image.
            ShowRewardImage(true, displayItemSlot[closestIndex].sprite);
            rewardImage.preserveAspect = true;
            
            isRunning = false;
        }

        
        /// <summary>
        /// Method called when end clicking the "Run" button.
        /// </summary>
        public void StopCharging()
        {
            if (isRunning) return;

            isButtonHeld = false;
            
            isRunning = true;
            
            var laps = (int) (chargeAmount * 10f);
            
            rotateRoulette = RotateRoulette(laps, generator.PickItem());
            
            StartCoroutine(rotateRoulette);
        }
        
        
        /// <summary>
        /// Method called when the "Run" button is pressed.
        /// </summary>
        public void StartCharging()
        {
            if (isRunning) return;
            
            if (rotateRoulette != null) StopCoroutine(rotateRoulette);
            
            isButtonHeld = true;

            StartCoroutine(FillProgressBar());
        }
        

        /// <summary>
        /// A coroutine method that fills the <c>fillAmount</c> value of the <c>progressBar</c> when the "Run" button that runs the roulette is pressed.
        /// </summary>
        /// <returns></returns>
        private IEnumerator FillProgressBar()
        {
            chargeAmount = 0;
            progressBar.fillAmount = 0f;

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
                
                progressBar.fillAmount = chargeAmount;
            }
        }
        
        
        /// <summary>
        /// A method that counts how many times each RandomItemType was rewarded.
        /// </summary>
        private void CountResult()
        {
            countArr[currentItem.Type]++;
        }
        
        
        /// <summary>
        /// A method that calculates the additional rotation angle so that the <c>needle</c> points to the reward item corresponding to the parameter <c>target</c>.
        /// </summary>
        /// <param name="current">The reward item that the needle is currently pointing to before running the roulette.</param>
        /// <param name="target">The reward item user will get.</param>
        /// <returns>The additional rotation angle to reach the area corresponding to the <c>target</c> reward item.</returns>
        private float GetAdditionalAngle(RandomItem current, RandomItem target)
        {
            // Each angle of the divided area within the roulette.
            var dividedAreaAngle = 360 / itemCount;
            
            // Calculate the angle difference between the area corresponding to the "current" reward item and the area corresponding to the "target" reward item.
            var currentAngle = generator.itemList.FindIndex(value => value.Type == current.Type) * dividedAreaAngle;
            var targetAngle = generator.itemList.FindIndex(value => value.Type == target.Type) * dividedAreaAngle;

            if (targetAngle - currentAngle <= 0)
            {
                return targetAngle - currentAngle + 360;
            }
            
            return targetAngle - currentAngle;
        }

        
        /// <summary>
        /// Show or hide the image of the item received by running the roulette in the <c>rewardImage</c>.
        /// </summary>
        /// <param name="flag">Whether to show a reward image or not.</param>
        /// <param name="reward">When the parameter <c>flag</c> is true, the Sprite <c>reward</c> is set to the <c>rewardImage</c>.</param>
        private void ShowRewardImage(bool flag, Sprite reward = null)
        {
            rewardImage.sprite = flag ? reward : null;
            rewardImage.color = new Color(255, 255, 255, flag ? 255 : 0);
        }
    }
}

