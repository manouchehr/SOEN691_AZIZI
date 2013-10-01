using System;
using OPSSDKCommon.Model.Message;

namespace ozeki.opssdk.sms
{
    class TestSms
    {
        private static SmsHandlerSample _smsHandlerSample;

        public static void Main(string[] args)
        {
            _smsHandlerSample = new SmsHandlerSample("localhost" , "admin", "12345", "9000"); //Use your settings, and configured API Extension
            SubscribeSmsHandlerEvents();

            _smsHandlerSample.SendMessage("123456", "TestContent1");

            //Waiting for incoming messages(MessageReceived), or program end by user.
            Console.ReadLine();
            UnsubscribeSmsHandlerEvents();
        }

        private static void smsHandlerSample_MessageSubmitted(object sender, MessageResultEventArgs e)
        {
            Console.WriteLine("Message successfully submitted.");
        }

        private static void smsHandlerSample_MessageDelivered(object sender, MessageResultEventArgs e)
        {
            Console.WriteLine("Message successfully delivered.");
        }

        private static void smsHandlerSample_MessageReceived(object sender, Message message)
        {
            Console.WriteLine("Message received. Sender: {0}, Content: {1}", message.Sender, message.Content);
        }

        private static void SubscribeSmsHandlerEvents()
        {
            if (_smsHandlerSample != null)
            {
                _smsHandlerSample.MessageSubmitted += smsHandlerSample_MessageSubmitted;
                _smsHandlerSample.MessageDelivered += smsHandlerSample_MessageDelivered;
                _smsHandlerSample.MessageReceived += smsHandlerSample_MessageReceived;
            }
        }

        private static void UnsubscribeSmsHandlerEvents()
        {
            if (_smsHandlerSample != null)
            {
                _smsHandlerSample.MessageSubmitted -= smsHandlerSample_MessageSubmitted;
                _smsHandlerSample.MessageDelivered -= smsHandlerSample_MessageDelivered;
                _smsHandlerSample.MessageReceived -= smsHandlerSample_MessageReceived;
            }
        }
    }
}
