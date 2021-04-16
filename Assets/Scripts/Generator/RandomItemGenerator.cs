using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator
{
    public class RandomItemGenerator: MonoBehaviour
    {
        public Sprite[] rewardSprites;

        // Size of itemCodes, which is a storage of random items.
        public static readonly int MaxItemCount = 1000;
        
        // It serves as a repository where item codes are stored as much as max item count (= 1000) and are sequentially extracted.
        private int[] itemCodes = new int[MaxItemCount];
        
        // Refer to RandomItems as a index.
        public List<RandomItem> itemList;
        
        // Refer to RandomItem as a item code.
        private Dictionary<int, RandomItem> itemRef;

        // Refer to sprite as a random item type.
        private Dictionary<RandomItemType, Sprite> spriteMap;

        // Variable used when adding an item to itemCodes, which is a storage of random items.
        private int iteration = 0;

        private void Start()
        {
            // Create SpriteMap
            spriteMap = new Dictionary<RandomItemType, Sprite>
            {
                [RandomItemType.Bamboo1] = rewardSprites[0],
                [RandomItemType.Bamboo2] = rewardSprites[1],
                [RandomItemType.Bamboo3] = rewardSprites[2],
                [RandomItemType.Box] = rewardSprites[3],
                [RandomItemType.Ticket] = rewardSprites[4],
                [RandomItemType.Book] = rewardSprites[5]
            };

            // Get a list of item references.
            itemList = RandomItem.CreateItems();

            itemList = itemList.OrderBy(a => Guid.NewGuid()).ToList();

            var newRewardSprites = new Sprite[rewardSprites.Length];

            for (var i = 0; i < itemList.Count; i++)
            {
                newRewardSprites[i] = spriteMap[itemList[i].Type];
            }

            rewardSprites = newRewardSprites;
            
            // Convert a list of item references to a dictionary { itemCode: RandomItem }
            itemRef = itemList.ToDictionary(value => value.ItemCode, value => value);

            // Create an array consisting only of itemCode according to the probability of each item
            var currentIndex = 0;
            
            foreach (var itemRefValue in itemRef.Values)
            {
                for (var i = 0; i < itemRefValue.Count; i++)
                {
                    itemCodes[currentIndex] = itemRefValue.ItemCode;
                    currentIndex++;
                }
            }

            // Shuffle the itemCode array created according to probability.
            itemCodes = itemCodes.ToList().OrderBy(a => Guid.NewGuid()).ToArray();
        }

        private int PickItemCode()
        {
            return itemCodes[iteration++];
        }

        public RandomItem PickItem()
        {
            var itemCode = PickItemCode();

            // Add RandomItem's sprite
            var item = itemRef[itemCode];

            if (item.ItemSprite == null)
            {
                item.ItemSprite = spriteMap[item.Type];
            }

            return item;
        }
    }
}