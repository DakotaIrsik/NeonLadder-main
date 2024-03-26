using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Tests.Managers
{
    public class DialogueManagerTests
    {
        private Mock<IDialogueManager> manager;

        [SetUp]
        public void Setup()
        {
            manager = new Mock<IDialogueManager>();
        }

        [Test]
        public void Spanish_locale_Sets_Correct_String_Table()
        {
            manager.Setup(m => m.InitializeLanguage(SystemLanguage.Spanish));
            Assert.AreEqual(manager.Object.currentStringTable.name, "DialogueStringTable_es");
        }
    }
}
