using System;
using System.Collections.Generic;

namespace DBD.Firebase.Sample
{
    [Serializable]
    public class RemoteConfigData
    {
        public string test;
        public TestModel test_model;
        public List<int> list;
        public List<TestModel> list_test_model;
    }
}