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
    public abstract class AbstractSimFeedbackExtension : ISimFeedbackExtension
    {
        public string Name { get; protected set; }

        public string Info { get; protected set; }

        public string Version { get; protected set; }

        public string Author { get; protected set; }

        /// <summary>
        /// The SimFeedback Facade
        /// </summary>
        /// <see cref="SimFeedbackExtensionFacade"/>
        protected SimFeedbackExtensionFacade SimFeedbackFacade { get; set; }

        public bool IsAutoStart { get; set; }

        public virtual void Init(SimFeedbackExtensionFacade facade, ExtensionConfig config)
        {
            SimFeedbackFacade = facade;
        }

        public abstract void Start();
        public abstract void Stop();

        public bool IsRunning { get; protected set; }
        
        public bool NeedsOwnTab { get; protected set; }

        public bool HasControl { get; protected set; }

        public bool IsActivated { get; set; }

        public virtual Control ExtensionControl => throw new System.NotImplementedException();

        protected void Log(string msg)
        {
            SimFeedbackFacade.Log(msg);
        }
        protected void LogDebug(string msg)
        {
            SimFeedbackFacade.LogDebug(msg);
        }
        protected void LogError(string msg)
        {
            SimFeedbackFacade.LogError(msg);
        }

    }
}
