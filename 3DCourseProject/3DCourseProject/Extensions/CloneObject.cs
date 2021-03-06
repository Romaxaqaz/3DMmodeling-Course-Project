﻿using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _3DCourseProject.Extensions
{
    public static class CloneObject
    {
        public static object DeepClone(this object obj)
        {
            object objResult = null;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);

                ms.Position = 0;
                objResult = bf.Deserialize(ms);
            }
            return objResult;
        }
    }
}
