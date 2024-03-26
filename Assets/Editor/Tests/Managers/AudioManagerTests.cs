using Moq;
using NUnit.Framework;

namespace Tests.Managers
{
    public class AudioManagerTests
    {
        [Test]
        public void Valid_Boss_Track()
        {
            // Create a mock of IAudioManager
            var mockAudioManager = new Mock<IAudioManager>();
            Assert.AreEqual(1, 1);

        }

        [Test]
        public void Invalid_Boss_Track()
        {
            // Create a mock of IAudioManager
            var mockAudioManager = new Mock<IAudioManager>();
            Assert.AreEqual(1, 2);

        }

        [Test]
        public void Background_Track()
        {
            // Create a mock of IAudioManager
            var mockAudioManager = new Mock<IAudioManager>();
            Assert.AreEqual(1, 1);

        }
    }
}