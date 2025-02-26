using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using NotificationService.Infrastructure.MessageConsumers;
using NotificationService.Models;
using NotificationService.Services;
using SharedLibrary.Models.Messages.RfqEvents;
using Xunit;

namespace NotificationService.Tests.Consumers
{
    public class RfqEmailNotificationConsumerTests
    {
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<ITemplateService> _templateServiceMock;
        private readonly Mock<ILogger<RfqEmailNotificationConsumer>> _loggerMock;
        private readonly RfqEmailNotificationConsumer _consumer;
        private readonly Mock<ConsumeContext<RfqEmailNotificationMessage>> _contextMock;

        public RfqEmailNotificationConsumerTests()
        {
            _emailServiceMock = new Mock<IEmailService>();
            _templateServiceMock = new Mock<ITemplateService>();
            _loggerMock = new Mock<ILogger<RfqEmailNotificationConsumer>>();
            _contextMock = new Mock<ConsumeContext<RfqEmailNotificationMessage>>();

            _consumer = new RfqEmailNotificationConsumer(
                _emailServiceMock.Object,
                _templateServiceMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Consume_WithValidMessage_SendsEmailToAllRecipients()
        {
            // Arrange
            var message = new RfqEmailNotificationMessage
            {
                RfqId = Guid.NewGuid(),
                ContractTitle = "Test RFQ",
                RecipientEmails = new List<string> { "test1@example.com", "test2@example.com" },
                RfqLink = "http://example.com/rfq/123",
                Deadline = DateTime.UtcNow.AddDays(7)
            };

            _contextMock.Setup(x => x.Message).Returns(message);

            var template = "<html>Template {{ContractTitle}}</html>";
            _templateServiceMock
                .Setup(x => x.GetTemplateAsync("RfqNotification"))
                .ReturnsAsync(template);

            _templateServiceMock
                .Setup(x => x.ReplaceTemplateVariables(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns("<html>Processed Template</html>");

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            _emailServiceMock.Verify(
                x => x.SendEmailAsync(It.IsAny<EmailModel>()),
                Times.Exactly(2)
            );
        }

        [Fact]
        public async Task Consume_WhenEmailServiceFails_ContinuesWithOtherRecipients()
        {
            // Arrange
            var message = new RfqEmailNotificationMessage
            {
                RfqId = Guid.NewGuid(),
                ContractTitle = "Test RFQ",
                RecipientEmails = new List<string> { "test1@example.com", "test2@example.com" },
                RfqLink = "http://example.com/rfq/123",
                Deadline = DateTime.UtcNow.AddDays(7)
            };

            _contextMock.Setup(x => x.Message).Returns(message);

            var template = "<html>Template {{ContractTitle}}</html>";
            _templateServiceMock
                .Setup(x => x.GetTemplateAsync("RfqNotification"))
                .ReturnsAsync(template);

            _templateServiceMock
                .Setup(x => x.ReplaceTemplateVariables(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Returns("<html>Processed Template</html>");

            _emailServiceMock
                .Setup(x => x.SendEmailAsync(It.Is<EmailModel>(e => e.To == "test1@example.com")))
                .ThrowsAsync(new Exception("SMTP error"));

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            _emailServiceMock.Verify(
                x => x.SendEmailAsync(It.Is<EmailModel>(e => e.To == "test2@example.com")),
                Times.Once
            );
        }

        [Fact]
        public async Task Consume_WithTemplateVariables_ReplacesCorrectly()
        {
            // Arrange
            var message = new RfqEmailNotificationMessage
            {
                RfqId = Guid.NewGuid(),
                ContractTitle = "Test RFQ",
                RecipientEmails = new List<string> { "test@example.com" },
                RfqLink = "http://example.com/rfq/123",
                Deadline = new DateTime(2024, 1, 1, 12, 0, 0)
            };

            _contextMock.Setup(x => x.Message).Returns(message);

            var template = "{{ContractTitle}} - {{Deadline}}";
            _templateServiceMock
                .Setup(x => x.GetTemplateAsync("RfqNotification"))
                .ReturnsAsync(template);

            Dictionary<string, string> capturedVariables = null;
            _templateServiceMock
                .Setup(x => x.ReplaceTemplateVariables(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
                .Callback<string, Dictionary<string, string>>((_, vars) => capturedVariables = vars)
                .Returns("Processed Template");

            // Act
            await _consumer.Consume(_contextMock.Object);

            // Assert
            Assert.NotNull(capturedVariables);
            Assert.Equal("Test RFQ", capturedVariables["ContractTitle"]);
            Assert.Equal("01 Jan 2024 12:00", capturedVariables["Deadline"]);
            Assert.Equal("http://example.com/rfq/123", capturedVariables["RfqLink"]);
        }

        [Fact]
        public async Task Consume_WhenTemplateServiceFails_LogsErrorAndThrows()
        {
            // Arrange
            var message = new RfqEmailNotificationMessage
            {
                RfqId = Guid.NewGuid(),
                RecipientEmails = new List<string> { "test@example.com" }
            };

            _contextMock.Setup(x => x.Message).Returns(message);

            _templateServiceMock
                .Setup(x => x.GetTemplateAsync("RfqNotification"))
                .ThrowsAsync(new Exception("Template not found"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _consumer.Consume(_contextMock.Object));

            _emailServiceMock.Verify(
                x => x.SendEmailAsync(It.IsAny<EmailModel>()),
                Times.Never
            );
        }
    }
}