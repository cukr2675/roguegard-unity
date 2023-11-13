using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace RoguegardUnity
{
    public class StandardRogueDeviceSaveTest
    {
        [Test]
        public void GetNewNumberingPath()
        {
            //Assert.AreEqual(@".\Save\ABC2.gard", StandardRogueDeviceSave.GetNewNumberingPath(@".\ABC"));
            //Assert.AreEqual(@".\Save\ABC2.gard", StandardRogueDeviceSave.GetNewNumberingPath(@".\ABC.gard"));
            //Assert.AreEqual(@".\Save\ABC2.zip", StandardRogueDeviceSave.GetNewNumberingPath(@".\ABC.zip"));
            //Assert.AreEqual(@".\Save\ABC2.gard", StandardRogueDeviceSave.GetNewNumberingPath(@"ABC"));
            //Assert.AreEqual(@".\Save\ABC2.gard", StandardRogueDeviceSave.GetNewNumberingPath(@"ABC.gard"));
            //Assert.AreEqual(@".\Save\ABC2.zip", StandardRogueDeviceSave.GetNewNumberingPath(@"ABC.zip"));

            //Assert.AreEqual(@".\Save\DEF1.gard", StandardRogueDeviceSave.GetNewNumberingPath(@".\DEF"));

            //// 同じディレクトリの同名ファイルでのみナンバリングを加算する
            //// 違うディレクトリに同名ファイルが存在しても無視しているか確認する
            //Assert.AreEqual(@".\Save\1\ABC1.gard", StandardRogueDeviceSave.GetNewNumberingPath(@".\1\ABC"));
        }
    }
}
