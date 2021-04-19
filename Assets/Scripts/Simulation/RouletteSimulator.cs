using System;
using System.Collections;
using System.Collections.Generic;
using Control;
using Generator;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Simulation
{
    public class RouletteSimulator: MonoBehaviour
    {
        [SerializeField] private RandomItemGenerator generator;
        [SerializeField] private RouletteController controller;
        [SerializeField] private Image roulettePlate;
        
        private IEnumerator rotateRoulette;
        private Dictionary<RandomItemType, int> countArr;
        private int match = 0;
        private string[][] log;
        
        private void Start()
        {
            countArr = new Dictionary<RandomItemType, int>
            {
                [generator.itemList[0].Type] = 0,
                [generator.itemList[1].Type] = 0,
                [generator.itemList[2].Type] = 0,
                [generator.itemList[3].Type] = 0,
                [generator.itemList[4].Type] = 0,
                [generator.itemList[5].Type] = 0,
            };

            log = new string[RandomItemGenerator.MaxItemCount][];
        }

        public void OnClickSimulatorBtn()
        {
            rotateRoulette = RotateRoulette();

            StartCoroutine(rotateRoulette);
        }
        
        private IEnumerator RotateRoulette()
        {
            var count = RandomItemGenerator.MaxItemCount;
            
            for (var i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(0.1f);

                var targetItem = generator.PickItem();
                
                var random = new Random();
                var laps = random.Next(0, 11);
                
                var lapAngle = laps * 360;
                var additionalAngle = GetAdditionalAngle(controller.currentItem, targetItem);
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

                controller.ShowResult();
                
                CountResult();

                if (targetItem.Type == controller.currentItem.Type)
                {
                    match++;
                }
                
                Debug.Log($"loop : {i + 1} matchCount : {match} \r\n" +
                          $"expect : {targetItem.Type} current : {controller.currentItem.Type} match : {match}\r\n" +
                          $"{RandomItemType.Bamboo1} : {countArr[RandomItemType.Bamboo1]}\r\n" +
                          $"{RandomItemType.Bamboo2} : {countArr[RandomItemType.Bamboo2]}\r\n" +
                          $"{RandomItemType.Bamboo3} : {countArr[RandomItemType.Bamboo3]}\r\n" +
                          $"{RandomItemType.Box} : {countArr[RandomItemType.Box]}\r\n" +
                          $"{RandomItemType.Ticket} : {countArr[RandomItemType.Ticket]}\r\n" +
                          $"{RandomItemType.Book} : {countArr[RandomItemType.Book]}");

                var columnData = new SimulationData(i + 1, targetItem.Type, controller.currentItem.Type, startLerpValue);
                log[i] = columnData.ToArray();
            }
            CsvController.WriteToCsv(log);
        }
        
        private double GetStartLerpValue(float ratio, float target, int count)
        {
            return target * ratio / (1 - Math.Pow(1 - ratio, count));
        }
        
        private void CountResult()
        {
            countArr[controller.currentItem.Type]++;
        }
        
        private float GetAdditionalAngle(RandomItem current, RandomItem target)
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