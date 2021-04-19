using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Generator
{
    /// <summary>
    /// The main function of this class <c>RandomItemGenerator</c> is to create <c>itemCodes</c>, which is an array of items in random order.
    /// <c>itemCodes</c> consists of the items corresponding to <c>RandomItemType</c> as much as <c>MaxItemCount</c> according to each <c>Probability</c>.
    /// </summary>
    public class RandomItemGenerator: MonoBehaviour
    {
        
        /// <summary> This is a variable that receives item sprites to be listed in the roulette through the unity inspector. </summary>
        /// <remarks> The order of items must be in the order of Bamboo1 -> Bamboo2 -> Bamboo3 -> Box -> Ticket -> Book. </remarks>
        public Sprite[] rewardSprites;

        
        /// <summary> Size of <c>itemCodes</c>. </summary>
        public static readonly int MaxItemCount = 10000;
        
        /// <summary>
        /// In this array <c>itemCodes</c>, the <c cref="RandomItem">ItemCode</c> of items are stored in random order.
        /// It serves as a storage where <c cref="RandomItem">ItemCode</c>s are stored as much as <c>MaxItemCount</c> and are sequentially extracted.
        /// </summary>
        private int[] itemCodes = new int[MaxItemCount];
        
        /// <summary> The items stored in this list appear in the unity roulette in order. </summary>
        public List<RandomItem> itemList;
        
        /// <summary> <c cref="RandomItem">ItemCode</c>s extracted from <c>itemCodes</c> refer to this dictionary to get data. </summary>
        private Dictionary<int, RandomItem> itemRef;

        /// <summary> To refer to Sprite of items with <c>RandomItemType</c>. </summary> 
        private Dictionary<RandomItemType, Sprite> spriteMap;

        /// <summary> Variable used when adding an item to itemCodes/ </summary>
        private int iteration = 0;

        
        private void Start()
        {
            // Create spriteMap.
            spriteMap = new Dictionary<RandomItemType, Sprite>
            {
                [RandomItemType.Bamboo1] = rewardSprites[0],
                [RandomItemType.Bamboo2] = rewardSprites[1],
                [RandomItemType.Bamboo3] = rewardSprites[2],
                [RandomItemType.Box] = rewardSprites[3],
                [RandomItemType.Ticket] = rewardSprites[4],
                [RandomItemType.Book] = rewardSprites[5]
            };

            // Get a list of RandomItem for reference.
            itemList = RandomItem.CreateItems();
            
            // Randomly shuffle the order of the elements in the itemList.
            itemList = itemList.OrderBy(a => Guid.NewGuid()).ToList();

            // Corrects the order of rewardSprites according to the order of the elements in the itemList.
            var newRewardSprites = new Sprite[rewardSprites.Length];

            for (var i = 0; i < itemList.Count; i++)
            {
                newRewardSprites[i] = spriteMap[itemList[i].Type];
            }

            rewardSprites = newRewardSprites;
            
            // Convert a list of item references to a dictionary. { itemCode: RandomItem }
            itemRef = itemList.ToDictionary(value => value.ItemCode, value => value);

            // Create an array consisting only of ItemCode according to the Probability of each item.
            var currentIndex = 0;
            
            foreach (var itemRefValue in itemRef.Values)
            {
                for (var i = 0; i < itemRefValue.Count; i++)
                {
                    itemCodes[currentIndex] = itemRefValue.ItemCode;
                    currentIndex++;
                }
            }

            // For random extraction, the order of the elements in the itemCodes is shuffled randomly.
            itemCodes = itemCodes.ToList().OrderBy(a => Guid.NewGuid()).ToArray();
        }

        /// <summary><c cref="RandomItem">ItemCode</c>s are extracted in order from randomly shuffled <c>itemCodes</c>.</summary>
        /// <returns><c cref="RandomItem">ItemCode</c> of the item to be returned from the <c>PickItem</c></returns>
        /// <see cref="PickItem"/>
        private int PickItemCode()
        {
            return itemCodes[iteration++];
        }

        /// <summary>This method finally returns the randomly selected item.</summary>
        /// <returns>The reward to be given to the user each time the Roulette is run is the <c>RandomItem</c> returned from this method.</returns>
        public RandomItem PickItem()
        {
            var itemCode = PickItemCode();

            // Add RandomItem's sprite.
            var item = itemRef[itemCode];

            if (item.ItemSprite == null)
            {
                item.ItemSprite = spriteMap[item.Type];
            }

            return item;
        }
    }
}