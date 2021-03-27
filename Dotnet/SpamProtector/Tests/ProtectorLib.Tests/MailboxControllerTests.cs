using Moq;

using NUnit.Framework;

using ProtectorLib.Controllers;
using ProtectorLib.Providers;

using System;
using System.Collections.Generic;

namespace ProtectorLib.Tests
{
    public class MailboxControllerTests
    {
        private Mock<IMailboxProvider> mockProviderA;
        private Mock<IMailboxProvider> mockProviderB;
        private Mock<IMailboxProvider> mockProviderC;
        private Mock<IMailboxProvider> mockProviderD;

        [SetUp]
        public void Setup()
        {
            mockProviderA = new Mock<IMailboxProvider>();
            mockProviderA.SetupGet(x => x.MailBoxName).Returns(nameof(mockProviderA));

            mockProviderB = new Mock<IMailboxProvider>();
            mockProviderA.SetupGet(x => x.MailBoxName).Returns(nameof(mockProviderB));

            mockProviderC = new Mock<IMailboxProvider>();
            mockProviderA.SetupGet(x => x.MailBoxName).Returns(nameof(mockProviderC));

            mockProviderD = new Mock<IMailboxProvider>();
            mockProviderA.SetupGet(x => x.MailBoxName).Returns(nameof(mockProviderD));
        }

        [Test]
        public void ShouldInitControllerAndReturnFirstProvider()
        {
            // Arrange 
            List<IMailboxProvider> providers = new();
            providers.Add(mockProviderA.Object);
            providers.Add(mockProviderB.Object);
            providers.Add(mockProviderC.Object);
            providers.Add(mockProviderD.Object);

            // Act
            MailboxController mailboxController = new(providers);

            // Assert
            Assert.IsNotNull(mailboxController);
            Assert.IsInstanceOf<IMailboxController>(mailboxController);
            Assert.IsNotNull(mailboxController.CurrentMailboxProvider);
            Assert.IsInstanceOf<IMailboxProvider>(mailboxController.CurrentMailboxProvider);
            Assert.AreEqual(mockProviderA.Object.MailBoxName, mailboxController.CurrentMailboxProvider.MailBoxName);
        }

        [Test]
        public void ShouldNotInitControllerIfProvidersEmpty()
        {
            // Arrange 
            List<IMailboxProvider> providers = new();
            MailboxController mailboxController = null;

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                mailboxController = new MailboxController(providers);
            });

            Assert.IsNull(mailboxController);
        }

        [Test]
        public void ShouldIterateProviders()
        {
            // Arrange 
            List<IMailboxProvider> providers = new();
            providers.Add(mockProviderA.Object);
            providers.Add(mockProviderB.Object);
            providers.Add(mockProviderC.Object);
            providers.Add(mockProviderD.Object);
            MailboxController mailboxController = new(providers);

            // Act & Assert
            Assert.AreEqual(mockProviderA.Object.MailBoxName, mailboxController.CurrentMailboxProvider.MailBoxName);

            mailboxController.SetNextProvider();
            Assert.AreEqual(mockProviderB.Object.MailBoxName, mailboxController.CurrentMailboxProvider.MailBoxName);

            mailboxController.SetNextProvider();
            Assert.AreEqual(mockProviderC.Object.MailBoxName, mailboxController.CurrentMailboxProvider.MailBoxName);

            mailboxController.SetNextProvider();
            Assert.AreEqual(mockProviderD.Object.MailBoxName, mailboxController.CurrentMailboxProvider.MailBoxName);

            mailboxController.SetNextProvider();
            Assert.AreEqual(mockProviderA.Object.MailBoxName, mailboxController.CurrentMailboxProvider.MailBoxName);
        }
    }
}