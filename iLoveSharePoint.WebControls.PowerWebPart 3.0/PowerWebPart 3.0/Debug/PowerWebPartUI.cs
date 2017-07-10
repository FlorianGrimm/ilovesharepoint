
using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using iLoveSharePoint.Debug;

namespace iLoveSharePoint.WebControls.Debug
{
    internal class PowerWebPartDebugRawUI : PSHostRawUserInterface
    {
        private IDebugConsole debugConsole = null;

        internal PowerWebPartDebugRawUI(IDebugConsole debugConsole)
        {
            this.debugConsole = debugConsole;
        }

        /// <summary>
        /// Get and set the background color of text ro be written.
        /// This maps pretty directly onto the corresponding .NET Console
        /// property.
        /// </summary>
        public override ConsoleColor BackgroundColor
        {
            get { return ConsoleColor.Black; }
            set {  }
        }

        /// <summary>
        /// Return the host buffer size adapted from on the .NET Console buffer size
        /// </summary>
        public override Size BufferSize
        {
            get 
            {   System.Drawing.Size debugConsoleSize = debugConsole.GetBufferSize();
                return new Size(debugConsoleSize.Width, debugConsoleSize.Height);
            }
            set {  }
        }

        /// <summary>
        /// This functionality is not currently implemented. The call fails with an exception.
        /// </summary>
        public override Coordinates CursorPosition
        {
            get { throw new NotImplementedException("The method or operation is not implemented."); }
            set { throw new NotImplementedException("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Return the cursor size taken directly from the .NET Console cursor size.
        /// </summary>
        public override int CursorSize
        {
            get { return 0; }
            set { }
        }

        /// <summary>
        /// This functionality is not currently implemented. The call simple returns silently.
        /// </summary>
        public override void FlushInputBuffer()
        {
            ;  //Do nothing...
        }

        /// <summary>
        /// Get and set the foreground color of text ro be written.
        /// This maps pretty directly onto the corresponding .NET Console
        /// property.
        /// </summary>
        public override ConsoleColor ForegroundColor
        {
            get { return ConsoleColor.White; }
            set {  }
        }

        /// <summary>
        /// This functionality is not currently implemented. The call fails with an exception.
        /// </summary>
        /// <param name="rectangle">Unused</param>
        /// <returns>Returns nothing - call fails.</returns>
        public override BufferCell[,] GetBufferContents(Rectangle rectangle)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Map directly to the corresponding .NET Console property.
        /// </summary>
        public override bool KeyAvailable
        {
            get { return Console.KeyAvailable; }
        }

        /// <summary>
        /// Return the MaxPhysicalWindowSize size adapted from the .NET Console
        /// LargestWindowWidth and LargestWindowHeight.
        /// </summary>
        public override Size MaxPhysicalWindowSize
        {
            get { return new Size(80, 300); }
        }

        /// <summary>
        /// Return the MaxWindowSize size adapted from the .NET Console
        /// LargestWindowWidth and LargestWindowHeight.
        /// </summary>
        public override Size MaxWindowSize
        {
            get { return new Size(80,300); }
        }

        /// <summary>
        /// This functionality is not currently implemented. The call fails with an exception.
        /// </summary>
        /// <param name="options">Unused</param>
        /// <returns>Nothing</returns>
        public override KeyInfo ReadKey(ReadKeyOptions options)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// This functionality is not currently implemented. The call fails with an exception.
        /// </summary>
        /// <param name="source">Unused</param>
        /// <param name="destination">Unused</param>
        /// <param name="clip">Unused</param>
        /// <param name="fill">Unused</param>
        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// This functionality is not currently implemented. The call fails with an exception.
        /// </summary>
        /// <param name="origin">Unused</param>
        /// <param name="contents">Unused</param>
        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        ///  This functionality is not currently implemented. The call fails with an exception.
        /// </summary>
        /// <param name="rectangle">Unused</param>
        /// <param name="fill">Unused</param>
        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Return the window position adapted from the Console window position information.
        /// </summary>
        public override Coordinates WindowPosition
        {
            get { return new Coordinates(0, 0); }
            set {  }
        }

        /// <summary>
        /// Return the window size adapted from the corresponding .NET Console calls.
        /// </summary>
        public override Size WindowSize
        {
            get { return new Size(80, 300); }
            set {  }
        }

        /// <summary>
        /// Mapped to the Console.Title property.
        /// </summary>
        public override string WindowTitle
        {
            get { return ""; }
            set { ; }
        }

    }
}