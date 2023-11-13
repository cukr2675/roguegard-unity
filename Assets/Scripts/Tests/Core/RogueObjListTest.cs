using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Roguegard
{
    public class RogueObjListTest
    {
        [Test]
        public void Add1RogueObj()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            objs.Add(obj);

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj, objs[0]);
        }

        [Test]
        public void Add1Null()
        {
            var objs = new RogueObjList();
            objs.Add(null);

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(null, objs[0]);
        }

        [Test]
        public void AddUnique2RogueObjs()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            Assert.True(objs.TryAddUnique(obj));

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj, objs[0]);

            Assert.False(objs.TryAddUnique(obj));

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj, objs[0]);
        }

        [Test]
        public void AddUniqueNullThrowsException()
        {
            var objs = new RogueObjList();

            Assert.Throws<System.ArgumentNullException>(() => objs.TryAddUnique(null));
        }

        [Test]
        public void InsertRogueObj()
        {
            var objs = new RogueObjList();
            var obj1 = new RogueObj();
            var obj2 = new RogueObj();
            objs.Add(obj1);
            objs.Insert(0, obj2);

            Assert.AreEqual(objs.Count, 2);
            Assert.AreEqual(obj2, objs[0]);
            Assert.AreEqual(obj1, objs[1]);
        }

        [Test]
        public void InsertNull()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            objs.Add(obj);
            objs.Insert(0, null);

            Assert.AreEqual(objs.Count, 2);
            Assert.AreEqual(null, objs[0]);
            Assert.AreEqual(obj, objs[1]);
        }

        [Test]
        public void RemoveRogueObj()
        {
            var objs = new RogueObjList();
            var obj1 = new RogueObj();
            var obj2 = new RogueObj();
            objs.Add(obj1);
            objs.Add(obj2);

            Assert.True(objs.Remove(obj1));

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj2, objs[0]);

            Assert.True(objs.Remove(obj2));

            Assert.AreEqual(objs.Count, 0);
        }

        [Test]
        public void RemoveNull()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            objs.Add(null);
            objs.Add(obj);
            objs.Remove(null);

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj, objs[0]);
        }

        [Test]
        public void RemoveNotContainsItemReturnsFalse()
        {
            var objs = new RogueObjList();
            var obj1 = new RogueObj();
            var obj2 = new RogueObj();
            objs.Add(obj1);

            Assert.False(objs.Remove(obj2));
            Assert.False(objs.Remove(null));
            Assert.True(objs.Remove(obj1));
            Assert.False(objs.Remove(obj1));
        }

        [Test]
        public void RemoveRogueObjAt()
        {
            var objs = new RogueObjList();
            var obj1 = new RogueObj();
            var obj2 = new RogueObj();
            objs.Add(obj1);
            objs.Add(obj2);
            objs.RemoveAt(0);

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj2, objs[0]);

            objs.RemoveAt(0);

            Assert.AreEqual(objs.Count, 0);
        }

        [Test]
        public void RemoveNullAt()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            objs.Add(null);
            objs.Add(obj);
            objs.RemoveAt(0);

            Assert.AreEqual(objs.Count, 1);
            Assert.AreEqual(obj, objs[0]);
        }

        [Test]
        public void RemoveAtOutOfRangeThrowsException()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            objs.Add(obj);

            Assert.Throws<System.ArgumentOutOfRangeException>(() => objs.RemoveAt(-1));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => objs.RemoveAt(1));
            objs.RemoveAt(0);
            Assert.Throws<System.ArgumentOutOfRangeException>(() => objs.RemoveAt(0));
        }

        [Test]
        public void Clear()
        {
            var objs = new RogueObjList();
            var obj = new RogueObj();
            objs.Add(null);
            objs.Add(obj);
            objs.Clear();

            Assert.AreEqual(objs.Count, 0);
        }
    }
}
