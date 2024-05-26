using HMIApp.Archivizations;

namespace HMIApp.Tests
{
    public class Tests
    {

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
    }
}