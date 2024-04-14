using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class MessageWorkList
    {
        private readonly List<Item> items = new();

        public int Count => items.Count;

        public void Get(int index, out object other, out RogueCharacterWork work)
        {
            var item = items[index];
            other = item.Other;
            work = item.Work;
        }

        public void Add(object other)
        {
            items.Add(new Item() { Other = other });
        }

        public void Add(RogueCharacterWork work)
        {
            items.Add(new Item() { Other = DeviceKw.EnqueueWork, Work = work });
        }

        private class Item
        {
            public object Other { get; set; }
            public RogueCharacterWork Work { get; set; }
        }
    }
}
