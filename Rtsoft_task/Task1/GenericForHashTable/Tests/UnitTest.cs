using GenericForHashTable;
using System.Collections.Generic;
using Xunit;

namespace Tests
{
    class TestDataGenerator
    {
    
        public static HashTableWrapper<int, string> GetTestData()
        {
            HashTableWrapper<int, string> table_ = new HashTableWrapper<int, string>();
            table_.Add(0, "Нуль");
            table_.Add(1, "Один");
            table_.Add(2, "Два");
            table_.Add(3, "Три");

            return table_;
        }
    }


    public class ValidityTest
    {
        [Fact]
        public void CheckCount()
        {
            var table_ = TestDataGenerator.GetTestData();
            Assert.True(table_.Count == 4, $"Counting was failed");
        }

        [Fact]
        public void Clear()
        {
            var table_ = TestDataGenerator.GetTestData();
            table_.Clear();
            Assert.True(table_.Count == 0, $"Table wasn't cleared successfully");
        }

        [Fact]
        public void AddOneElement()
        {
            var table_ = TestDataGenerator.GetTestData();
            int key = 4;
            string val = "Четыре";
            table_.Add(key, "Четыре");
            Assert.True(table_[key] == val, $"Failed add key = {key} and value = {val} to container");
        }

        [Fact]
        public void LookThrow()
        {
            var table_ = TestDataGenerator.GetTestData();
            List<string> vars = new List<string> { "Нуль", "Один", "Два", "Три" };
           
            for (int i = 0; i < table_.Count; ++i)
                Assert.True(table_[i] == vars[i], "Counting was failed");
        }

        [Fact]
        public void CheckEnumerator()
        {
            var table_ = TestDataGenerator.GetTestData();

            List<int> keys = new List<int> { 0, 1, 2, 3 };
            List<string> values = new List<string> { "Нуль", "Один", "Два", "Три" };
            keys.Reverse();
            values.Reverse();

            int keyIndex = 0;
            int valuesIndex = 0;
            foreach (var item in table_)
            {
                Assert.True(item.Key == keys[keyIndex++] &&
                     item.Value == values[valuesIndex++], "Enumerating was failed");
            }
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(10, false)]
        public void ContainsKey(int key, bool expectedResult)
        {
            var table_ = TestDataGenerator.GetTestData();
            Assert.True(table_.ContainsKey(key) == expectedResult, $"Container doesn't contain key = {key}");
        }

        [Theory]
        [InlineData(2, true)]
        [InlineData(20, false)]
        public void RemoveOneElementByKey(int key, bool expectedResult)
        {
            var table_ = TestDataGenerator.GetTestData();
            Assert.True(table_.Remove(key) == expectedResult, $"Failed to remove element by key = {key}");
        }

    }
}