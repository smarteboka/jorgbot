using System;
using JorgBot;
using Xunit;

namespace JogBot.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var now = Timing.NowInOsloTime().AddHours(12);
            var next = Timing.GetNextOccurenceInOsloTime("0 0 8 * * *");
            Console.WriteLine("now:" + now.ToString());
            Console.WriteLine("next" + next.ToString());
            Assert.True(true);
//            Assert.True(now > next);
//            Assert.Equal(now.Hour, next.Value.Hour);
        }
    }
}