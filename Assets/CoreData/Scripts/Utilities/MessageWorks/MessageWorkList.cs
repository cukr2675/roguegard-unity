using System.Collections.Generic;

namespace Roguegard
{
    public class MessageWorkList
    {
        private readonly List<Tuple> works = new();

        public int Count => works.Count;

        public void Get(int index, out object other, out RogueCharacterWork work)
        {
            var tuple = works[index];
            other = tuple.Other;
            work = tuple.Work;
        }

        public void Add(object other)
        {
            works.Add(new Tuple() { Other = other });
        }

        public void Add(RogueCharacterWork work)
        {
            works.Add(new Tuple() { Other = DeviceKw.EnqueueWork, Work = work });
        }

        private class Tuple
        {
            public object Other { get; set; }
            public RogueCharacterWork Work { get; set; }
        }
    }
}
