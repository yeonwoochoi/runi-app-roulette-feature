using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Generator
{
    public class RandomItem
    {
        public int ItemCode { get; set; }
        public RandomItemType Type { get; set; }
        public double Probability { get; set; }

        public Sprite ItemSprite { get; set; } = null;

        public int Count => (int) (RandomItemGenerator.MaxItemCount * Probability);

        // Returns a list of item references.
        public static List<RandomItem> CreateItems()
        {
            return new List<RandomItem>()
                .AddItem(new RandomItem
                {
                    ItemCode = 1001,
                    Type = RandomItemType.Bamboo1,
                    Probability = 0.8
                }).AddItem(new RandomItem
                {
                    ItemCode = 1002,
                    Type = RandomItemType.Bamboo2,
                    Probability = 0.1
                }).AddItem(new RandomItem
                {
                    ItemCode = 1003,
                    Type = RandomItemType.Bamboo3,
                    Probability = 0.06
                }).AddItem(new RandomItem
                {
                    ItemCode = 1004,
                    Type = RandomItemType.Box,
                    Probability = 0.02
                }).AddItem(new RandomItem
                {
                    ItemCode = 1005,
                    Type = RandomItemType.Ticket,
                    Probability = 0.015
                }).AddItem(new RandomItem
                {
                    ItemCode = 1006,
                    Type = RandomItemType.Book,
                    Probability = 0.005
                });
        }
    }

    public static class RandomItemUtils
    {
        public static List<RandomItem> AddItem(this List<RandomItem> items, RandomItem item)
        {
            items.Add(item);
            return items;
        }
    }

    public enum RandomItemType
    {
        Bamboo1,
        Bamboo2,
        Bamboo3,
        Book,
        Ticket,
        Box
    }
}