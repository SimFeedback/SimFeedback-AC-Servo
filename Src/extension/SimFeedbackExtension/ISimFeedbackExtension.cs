//
// Copyright (c) 2019 Rausch IT
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.
//
//
using SimFeedback.conf;
using System.Windows.Forms;

namespace SimFeedback.extension
{
    public interface ISimFeedbackExtension
    {
        /// <summary>
        /// The Name of this extension.
        /// Will be used for identication.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Short description of what this extension is doing
        /// </summary>
        string Info { get; }

        /// <summary>
        /// Version info
        /// </summary>
        string Version { get; }

        /// <summary>
        /// The Author
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Extension is started and running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Should this extension started with SimFeedback?
        /// </summary>
        bool IsAutoStart { get; set; }

        /// <summary>
        /// Is this Extension activated
        /// </summary>
        bool IsActivated { get; set; }

        /// <summary>
        /// Initialize the extension
        /// </summary>
        void Init(SimFeedbackExtensionFacade facade, ExtensionConfig config);

        /// <summary>
        /// Start the extension
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the extension
        /// </summary>
        void Stop();

        /// <summary>
        /// Extension needs its own tab in the GUI
        /// </summary>
        bool NeedsOwnTab { get; }

        /// <summary>
        /// Extension has a GUI control
        /// </summary>
        bool HasControl { get; }

        /// <summary>
        /// The GUI control
        /// </summary>
        Control ExtensionControl { get; }

    }
}
