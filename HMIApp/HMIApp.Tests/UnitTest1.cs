using HMIApp.Archivizations;

namespace HMIApp.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void WhenBitInByteIsChecked_ShouldReturnTrueOrFalse()
        {
            //arrange - przygotowanie testu
            var _app = new App();
            byte b = 0b00001100;
            int bitnumber = 2;

            //act - uruchamiamy test
            var result = _app.GetBit(b, bitnumber);

            //assert - sprawdzamy czy spelnione zostaly warunki
            Assert.That(result, Is.True);   
        }

        [Test]
        public void ChangeNumberOfWorkShift_WhenHourOfDayIsChange()
        {
            //arrange - przygotowanie testu
            var _archivization = new Archivization();

            //act - uruchamiamy test
            var result = _archivization.NumberOfProductionShift();

            //assert - sprawdzamy czy spelnione zostaly warunki
            int HourOfDay = DateTime.Now.Hour;

            if (HourOfDay >= 6 && HourOfDay < 14)
            {
                Assert.Equals(result, 1);
            }
            else if (HourOfDay >= 14 && HourOfDay < 22)
            {
                Assert.Equals(result, 2);
            }
            else if (HourOfDay >= 22 && HourOfDay < 6)
            {
                Assert.Equals(result, 3);
            }           
        }
    }
}