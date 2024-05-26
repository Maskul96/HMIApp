
using HMIApp.Archivizations;

namespace HMIApp.Tests
{
    public class ArchivizationTests
    {
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
