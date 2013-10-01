using System;
using OPSSDK;
using OPSSDKCommon.Model.Message;

namespace ozeki.opssdk.sms
{
    class SmsHandlerSample
    {
        /// <summary>
        /// OpsClient will connect to the PBX, and handle communication
        /// </summary>
        private OpsClient opsClient;

        /// <summary>
        /// IAPIExtension will forwarding the events and messages
        /// </summary>
        private IAPIExtension apiExtension;

        /// <summary>
        /// Event triggered, when the sent message is arrived to the Service Provider (some service provider does send notification about that)
        /// </summary>
        public event EventHandler<MessageResultEventArgs> MessageSubmitted;

        /// <summary>
        /// Event triggered, when the sent message is arrived to the Recipient phone (some service provider does send notification about that)
        /// </summary>
        public event EventHandler<MessageResultEventArgs> MessageDelivered;

        /// <summary>
        /// Event triggered, when the API Extension we have connected to recieved an SMS. Then the messages is forwarded through this event.
        /// </summary>
        public event EventHandler<Message> MessageReceived;

        /// <summary>
        /// Handler of SMS Sending and receiving
        /// </summary>
        /// <param name="serverAddress">The address of your Ozeki Phone System XE PBX</param>
        /// <param name="username">Valid Username for your Ozeki Phone System XE PBX</param>
        /// <param name="password">Valid Password for the given Username</param>
        /// <param name="apiExtensionId">The ID of an existing API Extension in PBX. You can receive SMS through that Extension.</param>
        public SmsHandlerSample(string serverAddress, string username, string password, string apiExtensionId = null)
        {
            if (!TryCreateConnectToClient(serverAddress, username, password)) return;
            if (!TrySetApiExtension(apiExtensionId)) return;
        }

        /// <summary>
        /// Create an OPS Client, and try to login into PBX with the given parameters
        /// </summary>
        /// <param name="serverAddress">The address of your Ozeki Phone System XE PBX</param>
        /// <param name="username">Valid Username for your Ozeki Phone System XE PBX</param>
        /// <param name="password">Valid Password for the given Username</param>
        /// <returns>Can or cannot connect to the PBX</returns>
        private bool TryCreateConnectToClient(string serverAddress, string username, string password)
        {
            opsClient = new OpsClient();
            opsClient.ErrorOccurred += ClientOnErrorOccurred;

            var result = opsClient.Login(serverAddress, username, password);
            if (!result)
            {
                Console.WriteLine("Cannot connect to the server, please check the login details and the availability of your PBX! Press Enter to continue!");
                Console.ReadLine();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try to connect to an API extension, what will handle the communication from/to the PBX.
        /// </summary>
        /// <param name="apiExtensionId">The ID of selected API Extension. If not given, use of the default SYSTEM Api Extension. But in this case, you cannot receiving SMS</param>
        /// <returns>Can or cannot connect to the API Extension</returns>
        private bool TrySetApiExtension(string apiExtensionId)
        {
            if (opsClient == null)
                return false;

            apiExtension = string.IsNullOrWhiteSpace(apiExtensionId)
                               ? opsClient.GetAPIExtension()
                               : opsClient.GetAPIExtension(apiExtensionId);

            if (apiExtension == null)
            {
                Console.WriteLine("Cannot find API Extension. Press Enter to continue!");
                Console.ReadLine();
                return false;
            }

            SubscribeAPIExtensionEvents();
            return true;
        }

        /// <summary>
        /// Sending SMS through Ozeki Phone System XE
        /// </summary>
        /// <param name="recipient">The (telephone)number of the recipient device</param>
        /// <param name="messageContent">The content of the SMS message</param>
        public void SendMessage(string recipient, string messageContent)
        {
            if (apiExtension == null)
                return;

            apiExtension.SendMessage(new SMSMessage(recipient, messageContent));
        }

        /// <summary>
        /// An error has been occured during communication with the PBX
        /// </summary>
        /// <param name="sender">Information about the sender</param>
        /// <param name="e">Information about the error</param>
        private void ClientOnErrorOccurred(object sender, ErrorInfo e)
        {
            Console.WriteLine(e.Message);
            Console.WriteLine("Press Enter to exit!");
            UnsubscribeAPIExtensionEvents();
            Console.ReadLine();
            Environment.Exit(0);
        }

        /// <summary>
        /// Event triggered, when the sent message is arrived to the Service Provider (some service provider does send notification about that)
        /// Forwarded to MessageSubmitted event.
        /// </summary>
        private void apiExtension_MessageSubmitted(object sender, MessageResultEventArgs e)
        {
            var handler = MessageSubmitted;

            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Event triggered, when the sent message is arrived to the Recipient phone (some service provider does send notification about that)
        /// Forwarded to MessageDelivered event.
        /// </summary>
        private void apiExtension_MessageDelivered(object sender, MessageResultEventArgs e)
        {
            var handler = MessageDelivered;

            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Event triggered, when the API Extension we have connected to recieved an SMS. Then is forwarded through this event.
        /// Forwarded to MessageReceived event.
        /// </summary>
        private void apiExtension_MessageReceived(object sender, Message e)
        {
            var handler = MessageReceived;

            if (handler != null)
                handler(this, e);
        }

        private void SubscribeAPIExtensionEvents()
        {
            if (apiExtension != null)
            {
                apiExtension.MessageSubmitted += apiExtension_MessageSubmitted;
                apiExtension.MessageDelivered += apiExtension_MessageDelivered;
                apiExtension.MessageReceived += apiExtension_MessageReceived;
            }
        }

        private void UnsubscribeAPIExtensionEvents()
        {
            if (apiExtension != null)
            {
                apiExtension.MessageDelivered -= apiExtension_MessageDelivered;
                apiExtension.MessageSubmitted -= apiExtension_MessageSubmitted;
                apiExtension.MessageReceived -= apiExtension_MessageReceived;
            }
        }

        ~SmsHandlerSample()
        {
            UnsubscribeAPIExtensionEvents();
        }
    }
}
