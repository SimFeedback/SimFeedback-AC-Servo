using SimFeedback.extension;
using System.Windows.Forms;

namespace TestExtension
{
    public class TestExtension : AbstractSimFeedbackExtension
    {
        public TestExtension()
        {
            Name = "Test Extension";
            Info = "Simple Test Extension";
            Version = "1.0.0";
            Author = "saxxon66";
            HasControl = false;
        }

        public override void Start()
        {
            MessageBox.Show("Test Extension started.");
        }

        public override void Stop()
        {
            MessageBox.Show("Test Extension stopped.");
        }

        
    }
}
