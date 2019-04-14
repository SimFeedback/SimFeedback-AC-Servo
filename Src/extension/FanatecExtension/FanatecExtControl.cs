using SharpDX.DirectInput;
using SimFeedback.extension;
using SimFeedback.extension.fanatec;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;


namespace SimFeedback.extension
{
    public partial class FanatecExtControl : UserControl
    {
        private static readonly int TIME_INTERVAL_IN_MILLISECONDS = 1000 / 60;
        private static readonly int MAX_16BIT = 65536;

        private DirectInput directInput;
        private Guid wheelGuid = Guid.Empty;
        private bool isStarted = false;
        private Joystick joystick;
        private JoystickState joystickState;
        private System.Threading.Timer pollTimer;
        // @see https://inputsimulator.codeplex.com/
        private InputSimulator inputSim;

        bool leftButtonDown = false;
        bool leftButtonUp = true;
        double mouseSpeed = 2.0;
        int pollCounter = 0;

        FanatecExtension fanatecExt;
        SimFeedbackExtensionFacade simFeedbackFacade;

        List<KeyValuePair<Guid, string>> joystickList = new List<KeyValuePair<Guid, string>>();

        Mouse mouse;
        MouseState mouseState;

        public FanatecExtControl(FanatecExtension ext, SimFeedbackExtensionFacade facade)
        {
            fanatecExt = ext;
            simFeedbackFacade = facade;
            // Initialize DirectInput
            directInput = new DirectInput();

            InitializeComponent();
            
            ListInputDevices();
            trackBarMouseSpeed.Value = (int)(mouseSpeed * 10.0);
        }



        private void ListInputDevices()
        {

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Driving, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                joystickList.Add(
                    new KeyValuePair<Guid, string>(joystickGuid, deviceInstance.InstanceName));
            }

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                joystickList.Add(
                    new KeyValuePair<Guid, string>(joystickGuid, deviceInstance.InstanceName));
            }

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices))
            {
                joystickGuid = deviceInstance.InstanceGuid;
                joystickList.Add(
                    new KeyValuePair<Guid, string>(joystickGuid, deviceInstance.InstanceName));
            }


            comboBoxJoysticks.DataSource = new BindingSource(joystickList, null);
            comboBoxJoysticks.DisplayMember = "Value";
            comboBoxJoysticks.ValueMember = "Key";

            // Default set the first Guid to be used
            // Autostart Mouse if Fanatec is detected
            //if (joystickList.Count > 0)
            //{
            //    wheelGuid = joystickList.First().Key;
            //    if (joystickList.First().Value.IndexOf("FANATEC") != -1)
            //    {
            //        StartStopToggle();
            //    }
            //}

        }

        public void Start()
        {
            if (wheelGuid == Guid.Empty)
            {
                MessageBox.Show("Please select Controller", "Notice", MessageBoxButtons.OK);
                isStarted = false;
                buttonStart.Text = "Start";
                return;
            }

            inputSim = new InputSimulator();

            // Joystick
            joystick = new Joystick(directInput, wheelGuid);
            joystick.Properties.AxisMode = DeviceAxisMode.Absolute;
            joystick.Acquire();


            joystickState = new JoystickState();

            // Mouse
            mouse = new Mouse(directInput);
            //mouse.Properties.
            mouse.Acquire();
            mouseState = new MouseState();

            // Start the poll thread using a Timer
            pollTimer = new System.Threading.Timer(Poll, null, TIME_INTERVAL_IN_MILLISECONDS, Timeout.Infinite);

            isStarted = true;
            buttonStart.Text = "Stop";

            fanatecExt.SetIsRunning(true);
        }

        public void Stop()
        {

            if (!isStarted) return;

            if (pollTimer != null)
            {
                pollTimer.Dispose();
            }

            // Joystick 
            if (joystick != null)
            {
                joystick.Unacquire();
                joystick.Dispose();
                joystick = null;
                joystickState = null;
            }

            // Mouse
            if (mouse != null) { 
                mouse.Unacquire();
                mouse.Dispose();
                mouse = null;
                mouseState = null;
            }

            inputSim = null;

            isStarted = false;
            buttonStart.Text = "Start";

            fanatecExt.SetIsRunning(false);
        }


        void Poll(Object state)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                joystick.Acquire();

                // Joystick
                joystick.Poll();

                MoveMouse();

                PushButtons();

                //if (pollCounter % 2 == 0)
                //{
                //    PushButtons();
                //}

                if (isStarted)
                {
                    pollTimer.Change(Math.Max(0, TIME_INTERVAL_IN_MILLISECONDS - watch.ElapsedMilliseconds), Timeout.Infinite);
                }
                else
                {
                    Stop();
                }

                if (pollCounter == Int16.MaxValue) pollCounter = 0;
                pollCounter++;
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    Trace.Write(e.ToString());
                }
            }
        }

        internal bool IsValid()
        {
            if (joystickList.Count > 0)
            {
                return (joystickList.First().Value.IndexOf("FANATEC") != -1);
            }
            return false;
        }

        private void PushButtons()
        {
            try
            {
                //TraceButtons();

                joystick.GetCurrentState(ref joystickState);
                bool[] buttons = joystickState.Buttons;

                //bool isButtonPushed = false;
                //foreach(bool i in buttons) {
                //    if (i)
                //    {
                //        isButtonPushed = true;
                //        break;
                //    }     
                //}
                //if (!isButtonPushed) return;

                // Mouse Click, push the joystick button
                if (buttons[11])
                {
                    if (leftButtonUp)
                    {
                        inputSim.Mouse.LeftButtonDown();
                        leftButtonDown = true;
                        leftButtonUp = false;
                    }
                }
                else
                {
                    if (leftButtonDown)
                    {
                        inputSim.Mouse.LeftButtonUp();
                        leftButtonUp = true;
                        leftButtonDown = false;
                    }
                }

                // Oben links Grün
                if (buttons[9])
                {
                    inputSim.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
                }

                // Unten links, Blau,  Orange 
                if (buttons[6])
                {
                    simFeedbackFacade.IncrementOverallIntensity();
                }
                if (buttons[8])
                {
                    simFeedbackFacade.DecrementOverallIntensity();
                }

                // unten rechts schwarz
                if (buttons[4])
                {
                    inputSim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                    inputSim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                    System.Threading.Thread.Sleep(100);
                    inputSim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
                    System.Threading.Thread.Sleep(200);
                }

                // unten links schwarz

                // Assetto Corsa
                // Toggle ASW with hotkeys:
                // CTRL+Numpad1: Disable ASW, go back to the original ATW mode
                // CTRL+Numpad2: Force apps to 45Hz, ASW disabled
                // CTRL+Numpad3: Force apps to 45Hz, ASW enabled
                // CTRL+Numpad4: Enable auto-ASW (default, use this first)
                if (buttons[5])
                {
                    inputSim.Keyboard.KeyDown(VirtualKeyCode.CONTROL);
                    if (awsState == AWS.DISABLED)
                    {
                        inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD4);
                        awsState = AWS.ENABLED;
                    }
                    else
                    {
                        inputSim.Keyboard.KeyPress(VirtualKeyCode.NUMPAD1);
                        awsState = AWS.DISABLED;
                    }
                    inputSim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
                }
            } catch(Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    Trace.Write(e.ToString());
                }
            }
        }

        AWS awsState = AWS.DISABLED;
        private bool autostart;

        enum AWS
        {
            DISABLED = 1,
            DISABLE_FORCE_45_HZ = 2,
            ENABLE_FORCE_45_HZ = 3,
            ENABLED = 4
        }


        private void MoveMouse()
        {
            try
            {
                // Mouse
                mouse.GetCurrentState(ref mouseState);

                mouseState.X = mouseState.X + joystickState.X;
                decimal moveX = Map(joystickState.X, 0, MAX_16BIT, -1, 1);
                int moveXBy = (int)(Math.Pow((double)moveX * mouseSpeed * 1.2, 3));

                mouseState.Y = mouseState.Y + joystickState.Y;
                decimal moveY = Map(joystickState.Y, 0, MAX_16BIT, -1, 1);
                int moveYBy = (int)(Math.Pow((double)moveY * mouseSpeed * 1.0
                    , 3));

                inputSim.Mouse.MoveMouseBy(moveXBy, moveYBy);
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                    Trace.Write(e.ToString());
                }
            }

        }

        private void TraceButtons()
        {
            try { 
                joystick.GetCurrentState(ref joystickState);
                bool[] buttons = joystickState.Buttons;

                for (int i = 0; i < buttons.Length; i++)
                {
                    if (buttons[i])
                        simFeedbackFacade.Log("Button #" + i + " pressed");
                }
                }
            catch (SharpDX.SharpDXException)
            {

            }
        }

        public decimal Map(decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        private void comboBoxJoysticks_SelectionChangeCommitted(object sender, EventArgs e)
        {
            wheelGuid = ((KeyValuePair<Guid, string>)comboBoxJoysticks.SelectedItem).Key;

            UpdateButtonList();
        }

        private void UpdateButtonList()
        {
            Joystick joy = new Joystick(directInput, wheelGuid);
            joy.Acquire();

            //foreach (DeviceObjectInstance doi in joy.GetObjects(DeviceObjectTypeFlags.Button))
            //{
            //    //ObjectProperties prop = joy.GetObjectPropertiesById(doi.ObjectId);
            //    fanatecExt.SimFeedbackFacade.Log(doi.ObjectId.InstanceNumber.ToString());
            //}

            joy.Unacquire();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            StartStopToggle();
        }

        private void ParentForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                StartStopToggle();
            }
        }

        public void StartStopToggle()
        {
            if (isStarted)
            {
                isStarted = false;
                buttonStart.Text = "Start";
            }
            else
            {
                isStarted = true;
                buttonStart.Text = "Stop";
                Start();
            }
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isStarted)
                StartStopToggle();
        }

        private void trackBarMouseSpeed_Scroll(object sender, EventArgs e)
        {
            mouseSpeed = trackBarMouseSpeed.Value * 0.1;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.FormClosing += ParentForm_FormClosing;
                this.ParentForm.KeyDown += ParentForm_KeyDown;
                this.ParentForm.KeyPreview = true;
            }
        }


    }
}
