using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Net.NetworkInformation;
using System.Diagnostics;
using Moq;
using SetupTool;
using SetupTool.model;

namespace SetupToolTest
{
    [TestClass]
    public class ProgramTests
    {
        private PackageManifest _testManifest;

        [TestInitialize]
        public void Setup()
        {
            _testManifest = new PackageManifest
            {
                OnlinePackages = new List<Package>
                {
                    new Package { Name = "test-choco", Type = "choco" },
                    new Package { Name = "test-npm", Type = "npm", Source = "test-source" },
                    new Package { Name = "test-zip", Type = "zip", Source = "test.zip", TargetDir = "test-dir" },
                    new Package { Name = "test-msi", Type = "msi", Source = "test.msi" }
                }
            };

            // Set the private _manifest field in Program
            typeof(Program).GetField("_manifest", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .SetValue(null, _testManifest);
        }

        [TestMethod]
        public void CheckInternetConnection_ShouldReturnTrue_WhenPingSucceeds()
        {
            // Arrange
            

            // Act
            bool result = Program.CheckInternetConnection();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckInternetConnection_ShouldReturnFalse_WhenPingFails()
        {
            // Arrange
            var pingMock = new Mock<Ping>();
            pingMock.Setup(p => p.Send(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new PingReplyMock(IPStatus.TimedOut));

            // Act
            bool result = Program.CheckInternetConnection();

            // Assert
            Assert.IsFalse(result);
        }

    }
}
