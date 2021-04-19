using System.Collections.Generic;
using UnityEngine;

namespace Generator
{
    /// <summary>
    /// Data about the roulette reward item is as fields in this class.
    /// </summary>
    public class RandomItem
    {
        /// <value> Unique number corresponding to the Item.</value>
        /// <seealso cref="CreateItems"/>
        public int ItemCode { get; set; }
        
        /// <value> Type of this item. </value>
        /// <seealso cref="RandomItemType"/>
        public RandomItemType Type { get; set; }
        
        /// <value> Probability of this item being picked. </value>
        /// <seealso cref="CreateItems"/>
        private double Probability { get; set; }

        /// <value>
        /// It is not initially set, but is set just before returning the <c>RandomItem</c> from the <c>PickItem</c> method.
        /// </value>
        /// <seealso cref="Generator.RandomItemGenerator.PickItem"/>
        public Sprite ItemSprite { get; set; } = null;

        /// <value>
        /// Returns the calculated value of how many <c>RandomItem</c>s corresponding to each <c>RandomItemType</c> should exist in <c>itemCodes</c>.
        /// </value>
        /// <seealso cref="Generator.RandomItemGenerator.itemCodes"/>
        public int Count => (int) (RandomItemGenerator.MaxItemCount * Probability);

        /// <summary>
        /// A method to create a List for reference of <c>RandomItem</c>s to be given as rewards in roulette.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="Generator.RandomItemGenerator.itemList"/>
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

    /// <summary>
    /// A method to simplify the code for adding elements to the List consisting of <c>RandomItem</c>s for reference.
    /// </summary>
    /// <seealso cref="RandomItem.CreateItems"/>
    public static class RandomItemUtils
    {
        public static List<RandomItem> AddItem(this List<RandomItem> items, RandomItem item)
        {
            items.Add(item);
            return items;
        }
    }

    /// <summary>
    /// Types of items to be rewarded in roulette
    /// </summary>
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